using ProjectTasks.Model;

namespace ProjectTasks.Interfaces
{
    public interface IFilesService
    {
        Task<FileAttachment> AddFileToProjectAsync(IFormFile file, long projectId);

        Task<FileAttachment> AddFileToTaskAsync(IFormFile file, long taskId);

        Task<MemoryStream> DownloadFilesFromProjectAsync(long projectId);
        Task<MemoryStream> DownloadFilesFromTaskAsync(long taskId);

        Task<bool> DeleteFileFromProjectAsync(long fileId, long projectId);
        Task<bool> DeleteFileFromTaskAsync(long fileId, long taskId);

    }
}
