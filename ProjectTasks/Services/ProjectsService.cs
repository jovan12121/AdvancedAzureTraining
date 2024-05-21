using ProjectTasks.DTO;
using ProjectTasks.Enums;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using ProjectTasks.Repository;

namespace ProjectTasks.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectTaskRepository _repository;
        private readonly IRabbitMQMessagingService _rabbitmqMessagingService;
        public ProjectsService(IProjectTaskRepository repository, IRabbitMQMessagingService rabbitmqMessagingService)
        {
            _repository = repository;
            _rabbitmqMessagingService = rabbitmqMessagingService;
        }

        public async Task<Project> AddProjectAsync(AddProjectDTO addProjectDTO)
        {
            DateTime? dateStarted = addProjectDTO.DateStarted == null ? DateTime.Now : addProjectDTO.DateStarted;
            Project projectToAdd = new Project() { Code = addProjectDTO.Code, ProjectName = addProjectDTO.ProjectName, Tasks = new List<Task_>(), DateStarted = (DateTime)dateStarted, Status = Enums.ProjectStatus.IN_PROGRESS };
            return await _repository.AddProjectAsync(projectToAdd);
        }

        public async Task<bool> DeleteProjectAsync(long projectId)
        {
            try
            {
                //List<FileAttachment> fileAttachments = await _repository.GetFilesFromProject(projectId);
                //foreach (var fileAttachment in fileAttachments)
                //{
                //    File.Delete(fileAttachment.Path);
                //}
                await _repository.DeleteProjectAsync(projectId);
                return true;
            }
            catch (Exception ex)
            {
                _rabbitmqMessagingService.PublishMessage("Error occured: Project with Id " + projectId + " doesn't exist.");
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
                _rabbitmqMessagingService.PublishMessage("Error occured: Project with Id " + projectId + " doesn't exist.");
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

        public async Task<Project> UpdateProjectStatusAsync(long projectId, ProjectStatus projectStatus)
        {
            Project projectToUpdateStatus = await _repository.GetProjectAsync(projectId);
            projectToUpdateStatus.Status = projectStatus;
            if(projectStatus == ProjectStatus.FAILED || projectStatus == ProjectStatus.COMPLETE)
            {
                projectToUpdateStatus.DateFinished = DateTime.Now;
            }
            await _repository.UpdateProjectAsync(projectToUpdateStatus);
            return projectToUpdateStatus; 
        }
    }
}
