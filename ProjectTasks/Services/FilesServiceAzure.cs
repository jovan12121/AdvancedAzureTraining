using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using System.IO.Compression;


namespace ProjectTasks.Services
{
    public class FilesServiceAzure : IFilesService
    {
        private readonly IProjectTaskRepository _repository;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly IRabbitMQMessagingService _rabbitMQMessagingService;
        public FilesServiceAzure(IConfiguration configuration, IProjectTaskRepository repository, IRabbitMQMessagingService rabbitMQMessagingService)
        {
            _repository = repository;
            var keyVaultEndpoint = configuration.GetSection("KeyVault:BaseUrl").Value;
            var clientId = configuration.GetSection("AzureAd:ClientId").Value;
            var clientSecret = configuration.GetSection("AzureAd:ClientSecret").Value;
            var tenantId = configuration.GetSection("AzureAd:TenantId").Value;
            var secretClient = new SecretClient(new Uri(keyVaultEndpoint), new ClientSecretCredential(tenantId, clientId, clientSecret));
            var connectionString = secretClient.GetSecret("BlobConnectionString").Value.Value;
            var containerName = secretClient.GetSecret("BlobContainerName").Value.Value;
            //string connectionString = configuration.GetSection("BlobConnectionString").Value.ToString();
            //string containerName = configuration.GetSection("BlobContainerName").Value.ToString();
            _blobContainerClient = new BlobContainerClient(connectionString: connectionString, blobContainerName: containerName);
            _rabbitMQMessagingService = rabbitMQMessagingService;
        }

        public async Task<FileAttachment> AddFileToProjectAsync(IFormFile file, long projectId)
        {
            if(file == null || file.Length == 0)
            {
                _rabbitMQMessagingService.PublishMessage("Error occured: File is null or empty.");
                throw new ApplicationException("File is null or empty.");
            }

            string fileName = file.Name + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + Path.GetExtension(file.FileName);

            BlobClient blobClient = _blobContainerClient.GetBlobClient($"Project_{projectId}/{fileName}");
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream,overwrite:true);
            }
            FileAttachment fileAttachment = new FileAttachment { Name = fileName, Path = blobClient.Uri.ToString(), ProjectId = projectId };
            return await _repository.AddFileToProjectAsync(fileAttachment);
        }

        public async Task<FileAttachment> AddFileToTaskAsync(IFormFile file, long taskId)
        {
            if (file == null || file.Length == 0)
            {
                _rabbitMQMessagingService.PublishMessage("Error occured: File is null or empty.");
                throw new ApplicationException("File is null or empty.");
            }

            string fileName = file.Name + DateTime.Now.ToString("ddMMyyyyHHmmssfff") + Path.GetExtension(file.FileName);

            BlobClient blobClient = _blobContainerClient.GetBlobClient($"Task_{taskId}/{fileName}");
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
            FileAttachment fileAttachment = new FileAttachment { Name = fileName, Path = blobClient.Uri.ToString(), TaskId = taskId };
            return await _repository.AddFileToProjectAsync(fileAttachment);
        }

        public async Task<bool> DeleteFileFromProjectAsync(long fileId, long projectId)
        {
            FileAttachment file = await _repository.GetFileAsync(fileId);
            if(file is null)
            {
                _rabbitMQMessagingService.PublishMessage("Error occured: File doesn't exist.");
                throw new ApplicationException("File doesn't exist.");
            }
            if(file.ProjectId!=projectId)
            {
                _rabbitMQMessagingService.PublishMessage("Error occured: File is not in given project.");
                throw new ApplicationException("File is not in given project.");
            }
            BlobClient blobClient = _blobContainerClient.GetBlobClient($"Project_{projectId}/{file.Name}");
            await blobClient.DeleteAsync();
            await _repository.DeleteFileAsync(fileId);
            return true;
        }

        public async Task<bool> DeleteFileFromTaskAsync(long fileId, long taskId)
        {
            FileAttachment file = await _repository.GetFileAsync(fileId);
            if (file is null)
            {
                _rabbitMQMessagingService.PublishMessage("Error occured: File doesn't exist.");
                throw new ApplicationException("File doesn't exist.");
            }
            if (file.TaskId != taskId)
            {
                _rabbitMQMessagingService.PublishMessage("Error occured: File is not in given task.");
                throw new ApplicationException("File is not in given task.");
            }
            BlobClient blobClient = _blobContainerClient.GetBlobClient($"Task_{taskId}/{file.Name}");
            await blobClient.DeleteAsync();
            await _repository.DeleteFileAsync(fileId);
            return true;
        }

        public async Task<MemoryStream> DownloadFilesFromProjectAsync(long projectId)
        {
            List<FileAttachment> fileAttachments = await _repository.GetFilesFromProjectAsync(projectId);
            MemoryStream memoryStream = new MemoryStream();
            using(ZipArchive archive = new ZipArchive(memoryStream,ZipArchiveMode.Create,true)) 
            {
                foreach (var fileAttachment in fileAttachments)
                {
                    string fileName = fileAttachment.Name;
                    string path = $"Project_{projectId}/{fileAttachment.Name}";
                    BlobClient blobClient = _blobContainerClient.GetBlobClient(path);

                    ZipArchiveEntry entry = archive.CreateEntry(fileName);
                    using(Stream entryStream = entry.Open())
                    {
                        await blobClient.DownloadToAsync(entryStream);
                    }
                }
            }
            memoryStream.Seek(0,SeekOrigin.Begin);
            return memoryStream;
        }

        public async Task<MemoryStream> DownloadFilesFromTaskAsync(long taskId)
        {
            List<FileAttachment> fileAttachments = await _repository.GetFilesFromTaskAsync(taskId);
            MemoryStream memoryStream = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileAttachment in fileAttachments)
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
