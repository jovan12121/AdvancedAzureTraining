using Microsoft.AspNetCore.Mvc;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using System.IO.Compression;

namespace ProjectTasks.Services
{
    public class FilesServiceLocal : IFilesService
    {
        private readonly IProjectTaskRepository _repository;
        private readonly IRabbitMQMessagingService _messagingService;
        private readonly string _directoryPath = "Files";

        public FilesServiceLocal(IProjectTaskRepository repository, IRabbitMQMessagingService messagingService)
        {
            _repository = repository;
            _messagingService = messagingService;
        }
        public async Task<FileAttachment> AddFileToProjectAsync(IFormFile file, long projectId)
        {
            if (file == null || file.Length == 0)
            {
                _messagingService.PublishMessage("Error occured: File is null or empty.");
                throw new ApplicationException("File is null or empty.");
            }
            string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            string projectDirectory = Path.Combine(_directoryPath, "Projects", projectId.ToString());
            string filePath = Path.Combine(projectDirectory, fileName);
            Directory.CreateDirectory(projectDirectory);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            FileAttachment fileAttachment = new FileAttachment { Name = fileName, Path = filePath, ProjectId = projectId };
            return await _repository.AddFileToProjectAsync(fileAttachment);
        }


        public async Task<FileAttachment> AddFileToTaskAsync(IFormFile file, long taskId)
        {
            if (file == null || file.Length == 0)
            { 
                _messagingService.PublishMessage("Error occured: File is null or empty.");
                throw new ApplicationException("File is null or empty.");
            }
            string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            string projectDirectory = Path.Combine(_directoryPath, "Tasks", taskId.ToString());
            string filePath = Path.Combine(projectDirectory, fileName);
            Directory.CreateDirectory(projectDirectory);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            FileAttachment fileAttachment = new FileAttachment { Name = fileName, Path = filePath, TaskId = taskId };
            return await _repository.AddFileToTasksAsync(fileAttachment);
        }

        public async Task<bool> DeleteFileFromProjectAsync(long fileId, long projectId)
        {
            FileAttachment file = await _repository.GetFileAsync(fileId);
            if(file is null)
            {
                _messagingService.PublishMessage("Error occured: File not found.");
                throw new ApplicationException("File not found.");
            }
            if(file.ProjectId != projectId)
            {
                _messagingService.PublishMessage("Error occured: File is not in given project.");
                throw new ApplicationException("File is not in given project.");
            }
            File.Delete(file.Path);
            await _repository.DeleteFileAsync(fileId);
            return true;
        }

        public async Task<bool> DeleteFileFromTaskAsync(long fileId, long taskId)
        {
            FileAttachment file = await _repository.GetFileAsync(fileId);
            if (file is null)
            {
                _messagingService.PublishMessage("Error occured: File not found.");
                throw new ApplicationException("File not found.");
            }
            if (file.TaskId != taskId)
            {
                _messagingService.PublishMessage("Error occured: File is not in given task.");
                throw new ApplicationException("File is not in given task.");
            }
            File.Delete(file.Path);
            await _repository.DeleteFileAsync(fileId);
            return true;
        }

        public async Task<MemoryStream> DownloadFilesFromProjectAsync(long projectId)
        {
            List<FileAttachment> fileAttachments = await _repository.GetFilesFromProjectAsync(projectId);
            MemoryStream memoryStream = new MemoryStream();

            using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var fileAttachment in fileAttachments)
                {
                    string filePath = fileAttachment.Path;
                    string entryName = Path.GetFileName(filePath);

                    ZipArchiveEntry entry = archive.CreateEntry(entryName);

                    using (Stream entryStream = entry.Open())
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
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
                    string filePath = fileAttachment.Path;
                    string entryName = Path.GetFileName(filePath);

                    ZipArchiveEntry entry = archive.CreateEntry(entryName);

                    using (Stream entryStream = entry.Open())
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        await fileStream.CopyToAsync(entryStream);
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
