using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using ProjectTaskCosmos.Controllers;
using ProjectTaskCosmos.Model;

namespace ProjectTaskCosmos.Services
{
    public class CosmosService
    {
        private readonly CosmosClient _client;
        public CosmosService()
        {
            var cosmosConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["CosmosDbConnectionString"];
            _client = new CosmosClient(connectionString: cosmosConnectionString);
        }
        private Container container
        {
            get => _client.GetDatabase("ProjectTasks").GetContainer("ProjectTasksContainer");
        }
        public async Task<List<Project>> GetProjectsAsync()
        {
            var queryable = container.GetItemLinqQueryable<Project>();
            using FeedIterator<Project> feed = queryable.ToFeedIterator();
            List<Project> projects = new List<Project>();
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                foreach (Project item in response)
                {
                    projects.Add(item);
                }
            }
            return projects;
        }
        public async Task<Project> GetProjectAsync(long id)
        {
            var queryable = container.GetItemLinqQueryable<Project>();
            using FeedIterator<Project> feed = queryable.Where(p => p.Id == id).ToFeedIterator();

            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                foreach (var project in response)
                {
                    if (project.Id == id)
                    {
                        return project;
                    }
                }
            }
            throw new Exception("Project with that id doesn't exist!");
        }

    }
}
