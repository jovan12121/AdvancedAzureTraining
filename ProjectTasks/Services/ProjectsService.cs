using ProjectTasks.DTO;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using ProjectTasks.Repository;

namespace ProjectTasks.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectTaskRepository _repository;

        public ProjectsService(IProjectTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<Project> AddProjectAsync(AddProjectDTO addProjectDTO)
        {
            Project projectToAdd = new Project() { Code = addProjectDTO.Code, ProjectName = addProjectDTO.ProjectName, Tasks = new List<Task_>() };
            return await _repository.AddProjectAsync(projectToAdd);
        }

        public async Task<bool> DeleteProjectAsync(long projectId)
        {
            try
            {
                await _repository.DeleteProjectAsync(projectId);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Project with Id " + projectId + " doesn't exist.");
            }
        }

        public async Task<Project> GetProjectAsync(long projectId)
        {
            try
            {
                return await _repository.GetProjectAsync(projectId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Project with Id " + projectId + " doesn't exist.");
            }
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _repository.GetProjectsAsync();
        }

        public async Task<Project> UpdateProjectAsync(EditProjectDTO editProjectDTO)
        {
            Project projectToUpdate = await _repository.GetProjectAsync(editProjectDTO.Id);
            projectToUpdate.ProjectName = editProjectDTO.ProjectName;
            projectToUpdate.Code = editProjectDTO.Code;
            return await _repository.UpdateProjectAsync(projectToUpdate);
        }
    }
}
