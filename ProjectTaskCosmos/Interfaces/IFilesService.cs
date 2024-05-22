namespace ProjectTaskCosmos.Interfaces
{
    public interface IFilesService
    {
        Task<MemoryStream> DownloadFilesFromProjectAsync(long projectId);
        Task<MemoryStream> DownloadFilesFromTaskAsync(long taskId);
    }
}
