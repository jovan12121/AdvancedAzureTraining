using ProjectTaskCosmos.Model;

namespace ProjectTaskCosmos.Interfaces
{
    public interface IProjectsService
    {
        Task<List<Project>> GetProjectsAsync();
        Task<Project> GetProjectAsync(long projectId);
    }
}
