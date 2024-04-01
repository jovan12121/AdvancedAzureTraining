using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ProjectTasksFunction.Model;
using Container = Microsoft.Azure.Cosmos.Container;

namespace ProjectTasksFunction
{
    public class DataCopyingFunction
    {
        private readonly ILogger _logger;
        private static readonly string sqlServerConnectionString = Environment.GetEnvironmentVariable("ProjectDb");
        private static readonly string cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
        private static readonly string databaseName = "ProjectTasks";
        private static readonly string containerName = "ProjectTasksContainer";
        private static readonly CosmosClient cosmosClient = new CosmosClient(cosmosConnectionString);
        private static readonly Database database = cosmosClient.GetDatabase(databaseName);
        private static readonly Container container = database.GetContainer(containerName);
        public DataCopyingFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DataCopyingFunction>();
        }

        private List<Project> ReadAllProjectsFromSqlServer()
        {
            List<Project> projects = new List<Project>();
            using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
            {
                connection.Open();

                string projectQuery = "SELECT Id, ProjectName, Code FROM Projects";
                using (SqlCommand projectCommand = new SqlCommand(projectQuery, connection))
                {
                    using (SqlDataReader projectReader = projectCommand.ExecuteReader())
                    {
                        while (projectReader.Read())
                        {
                            Project project = new Project
                            {
                                Id = projectReader.GetInt64(projectReader.GetOrdinal("Id")),
                                ProjectName = projectReader.GetString(projectReader.GetOrdinal("ProjectName")),
                                Code = projectReader.GetString(projectReader.GetOrdinal("Code")),
                                Tasks = new List<Task_>()
                            };
                            project.id = project.Id.ToString();
                            projects.Add(project);
                        }
                    }
                }

                foreach (var project in projects)
                {
                    string taskQuery = "SELECT Id, TaskName, TaskDescription FROM Tasks WHERE ProjectId = @ProjectId";
                    using (SqlCommand taskCommand = new SqlCommand(taskQuery, connection))
                    {
                        taskCommand.Parameters.AddWithValue("@ProjectId", project.Id);

                        using (SqlDataReader taskReader = taskCommand.ExecuteReader())
                        {
                            while (taskReader.Read())
                            {
                                Task_ task = new Task_
                                {
                                    Id = taskReader.GetInt64(taskReader.GetOrdinal("Id")),
                                    TaskName = taskReader.GetString(taskReader.GetOrdinal("TaskName")),
                                    TaskDescription = taskReader.GetString(taskReader.GetOrdinal("TaskDescription"))
                                };
                                task.ProjectId = project.Id;
                                project.Tasks.Add(task);
                            }
                        }
                    }
                }
            }
            return projects;
        }
        private async Task CopyDataToCosmosDb(List<Project> projects)
        {
            foreach (var project in projects)
            {
                try
                {
                    var existingItem = await container.ReadItemAsync<Project>(project.Id.ToString(), new PartitionKey(project.Id));
                    if (existingItem != null)
                    {
                        project.Id = existingItem.Resource.Id; 
                        await container.ReplaceItemAsync(project, project.Id.ToString(), new PartitionKey(project.Id));
                    }
                    else
                    {
                        await container.CreateItemAsync(project);
                    }
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await container.CreateItemAsync(project);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while copying data for project with ID {project.Id} to Cosmos DB.");
                }
            }

            var cosmosProjects = container.GetItemQueryIterator<Project>("SELECT * FROM c");
            while (cosmosProjects.HasMoreResults)
            {
                var response = await cosmosProjects.ReadNextAsync();
                foreach (var cosmosProject in response)
                {
                    if (projects.All(p => p.Id != cosmosProject.Id))
                    {
                        try
                        {
                            await container.DeleteItemAsync<Project>(cosmosProject.Id.ToString(), new PartitionKey(cosmosProject.Id));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"An error occurred while deleting project with ID {cosmosProject.Id} from Cosmos DB.");
                        }
                    }
                }
            }
        }


        [Function("DataCopyingFunction")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation("C# Timer trigger function processed a request.");
            try
            {
                List<Project> projects = ReadAllProjectsFromSqlServer();
                await CopyDataToCosmosDb(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while copying data to Cosmos DB.");
            }
        }
        public class MyInfo
        {
            public MyScheduleStatus ScheduleStatus { get; set; }

            public bool IsPastDue { get; set; }
        }

        public class MyScheduleStatus
        {
            public DateTime Last { get; set; }

            public DateTime Next { get; set; }

            public DateTime LastUpdated { get; set; }
        }
    }
}
