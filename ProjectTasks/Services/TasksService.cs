using ProjectTasks.DTO;
using ProjectTasks.Enums;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using ProjectTasks.Repository;

namespace ProjectTasks.Services
{
    public class TasksService : ITasksService
    {
        private readonly IProjectTaskRepository _repository;
        private readonly IRabbitMQMessagingService _rabbitmqMessagingService;

        public TasksService(IProjectTaskRepository repository, IRabbitMQMessagingService rabbitmqMessagingService)
        {
            _repository = repository;
            _rabbitmqMessagingService = rabbitmqMessagingService;
        }
        public async Task<Task_> AddTaskAsync(AddTaskDTO addTaskDTO)
        {
            if (await _repository.GetProjectAsync(addTaskDTO.ProjectId) != null)
            {
                DateTime? dateStarted = addTaskDTO.DateStarted == null ? DateTime.Now : addTaskDTO.DateStarted;
                Task_ taskToAdd = new Task_ { TaskDescription = addTaskDTO.TaskDescription, TaskName = addTaskDTO.TaskName, ProjectId = addTaskDTO.ProjectId, DateStarted = (DateTime)dateStarted, Status = Enums.TaskStatus_.IN_PROGRESS, MetaData = addTaskDTO.MetaData };
                return await _repository.AddTaskAsync(taskToAdd);
            }
            _rabbitmqMessagingService.PublishMessage("Error occured: Project with that ID doesn't exist!");
            throw new ApplicationException("Project with that ID doesn't exist!");
        }

        public async Task<bool> DeleteTaskAsync(long taskId)
        {
            try
            {
                await _repository.DeleteTaskAsync(taskId);
                return true;
            }
            catch (Exception ex)
            {
                _rabbitmqMessagingService.PublishMessage("Error occured: Task with " + taskId + " not found.");
                throw new ApplicationException("Task with " + taskId + " not found.");
            }
        }

        public async Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId)
        {
            return await _repository.GetAllTasksFromProjectAsync(projectId);
        }

        public async Task<Task_> GetTaskAsync(long taskId)
        {
            try
            {
                return await _repository.GetTaskAsync(taskId);
            }
            catch (Exception ex)
            {
                _rabbitmqMessagingService.PublishMessage("Error occured: Task with Id " + taskId + " doesn't exist.");
                throw new ApplicationException("Task with Id " + taskId + " doesn't exist.");
            }
        }

        public async Task<Task_> UpdateTaskAsync(EditTaskDTO editTaskDTO)
        {
            Task_ taskToUpdate = await _repository.GetTaskAsync(editTaskDTO.Id);
            if (taskToUpdate == null)
            {
                _rabbitmqMessagingService.PublishMessage("Error occured: Task doesn't exist.");
                throw new ApplicationException("Task doesn't exist.");
            }
            taskToUpdate.TaskDescription = editTaskDTO.TaskDescription;
            taskToUpdate.TaskName = editTaskDTO.TaskName;
            return await _repository.UpdateTaskAsync(taskToUpdate);
        }

        public async Task<Task_> UpdateTaskStatusAsync(long taskId, TaskStatus_ taskStatus)
        {
            Task_ taskToUpdateStatus = await _repository.GetTaskAsync(taskId);
            taskToUpdateStatus.Status = taskStatus;
            if (taskStatus == TaskStatus_.FAILED || taskStatus == TaskStatus_.COMPLETE)
            {
                taskToUpdateStatus.DateFinished = DateTime.Now;
            }
            await _repository.UpdateTaskAsync(taskToUpdateStatus);
            return taskToUpdateStatus;

        }
    }
}
