using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using ProjectTaskCosmos.Interfaces;
using ProjectTaskCosmos.Model;
using System.IO.Compression;

namespace ProjectTaskCosmos.Services
{
    public class AzureFilesService : IFilesService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private CosmosService cosmosService;
        public AzureFilesService(IConfiguration configuration)
        {
            var keyVaultEndpoint = configuration.GetSection("KeyVault:BaseUrl").Value;
            var clientId = configuration.GetSection("AzureAd:ClientId").Value;
            var clientSecret = configuration.GetSection("AzureAd:ClientSecret").Value;
            var tenantId = configuration.GetSection("AzureAd:TenantId").Value;
            var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new ClientSecretCredential(tenantId, clientId, clientSecret));
            var connectionString = secretClient.GetSecret("BlobConnectionString").Value.Value;
            var containerName = secretClient.GetSecret("BlobContainerName").Value.Value;

            cosmosService = new CosmosService(configuration);
            //string connectionString = configuration.GetSection("BlobConnectionString").Value.ToString();
            //string containerName = configuration.GetSection("BlobContainerName").Value.ToString();
            _blobContainerClient = new BlobContainerClient(connectionString: connectionString, blobContainerName: containerName);
        }
        public async Task<MemoryStream> DownloadFilesFromProjectAsync(long projectId)
        {
            List<FileAttachment> fileAttachments = await cosmosService.GetFilesFromProjectAsync(projectId);
            MemoryStream memoryStream = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileAttachment in fileAttachments)
                {
                    string fileName = fileAttachment.Name;
                    string path = $"Project_{projectId}/{fileAttachment.Name}";
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(path);

                    ZipArchiveEntry entry = archive.CreateEntry(fileName);
                    using (Stream entryStream = entry.Open())
                    {
                        await blobClient.DownloadToAsync(entryStream);
                    }
                }
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public async Task<MemoryStream> DownloadFilesFromTaskAsync(long taskId)
        {
            List<Project> projects = await cosmosService.GetProjectsAsync();
            Task_? task = null;
            foreach (var project in projects)
            {
                foreach(var item in project.Tasks)
                {
                    if(item.Id == taskId)
                    {
                        task = item; break;
                    }
                }
            }
            if (task == null) throw new Exception("Task not found");
            MemoryStream memoryStream = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileAttachment in task.Files)
                {
                    string fileName = fileAttachment.Name;
                    string path = $"Task_{taskId}/{fileAttachment.Name}";
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(path);

                    ZipArchiveEntry entry = archive.CreateEntry(fileName);
                    using (Stream entryStream = entry.Open())
                    {
                        await blobClient.DownloadToAsync(entryStream);
                    }
                }
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
