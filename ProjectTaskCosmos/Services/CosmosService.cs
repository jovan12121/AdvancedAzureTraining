using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using ProjectTaskCosmos.Controllers;
using ProjectTaskCosmos.Model;
using System.Configuration;

namespace ProjectTaskCosmos.Services
{
    public class CosmosService
    {
        private readonly CosmosClient _client;
        static readonly string[] scopeRequiredByApi = new string[] { "ReadAccess" };
        public CosmosService(IConfiguration configuration)
        {
            var keyVaultEndpoint = configuration.GetSection("KeyVault:BaseUrl").Value;
            var clientId = configuration.GetSection("AzureAd:ClientId").Value;
            var clientSecret = configuration.GetSection("AzureAd:ClientSecret").Value;
            var tenantId = configuration.GetSection("AzureAd:TenantId").Value;
            var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new ClientSecretCredential(tenantId, clientId, clientSecret));
            var cosmosConnectionString = secretClient.GetSecret("cosmosDbConnectionString").Value.Value;
            //var cosmosConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["CosmosDbConnectionString"];
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
        public async Task<List<FileAttachment>> GetFilesFromProjectAsync(long projectId)
        {
            var queryable = container.GetItemLinqQueryable<Project>();
            using FeedIterator<Project> feed = queryable.Where(p => p.Id == projectId).ToFeedIterator();
            Project? projectToFind = null;
            while (feed.HasMoreResults)
            {
                var response = await feed.ReadNextAsync();
                foreach (var project in response)
                {
                    if (project.Id == projectId)
                    {
                        projectToFind = project;
                    }
                }
            }
            if (projectToFind == null) throw new Exception("Project not found");
            return projectToFind.Files;
        }

    }
}
