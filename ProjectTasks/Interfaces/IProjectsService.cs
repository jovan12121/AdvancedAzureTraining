using ProjectTasks.DTO;
using ProjectTasks.Model;

namespace ProjectTasks.Interfaces
{
    public interface IProjectsService
    {
        Task<List<Project>> GetProjectsAsync();
        Task<Project> GetProjectAsync(long projectId);
        Task<bool> DeleteProjectAsync(long projectId);
        Task<Project> AddProjectAsync(AddProjectDTO addProjectDto);
        Task<Project> UpdateProjectAsync(EditProjectDTO editProjectDTO);
    }
}
