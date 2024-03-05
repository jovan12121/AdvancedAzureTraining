using ProjectTasks.DTO;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;
using ProjectTasks.Repository;

namespace ProjectTasks.Services
{
    public class TasksService : ITasksService
    {
        private readonly IProjectTaskRepository _repository;
        public TasksService(IProjectTaskRepository repository)
        {
            _repository = repository;
        }
        public async Task<Task_> AddTaskAsync(AddTaskDTO addTaskDTO)
        {
            if (_repository.GetProjectAsync(addTaskDTO.ProjectId) != null)
            {
                Task_ taskToAdd = new Task_ { TaskDescription = addTaskDTO.TaskDescription, TaskName = addTaskDTO.TaskName, ProjectId = addTaskDTO.ProjectId };
                return await _repository.AddTaskAsync(taskToAdd);
            }
            throw new Exception("Project with that ID doesn't exist!");
        }

        public async Task<bool> DeleteTaskAsync(long taskId)
        {
            await _repository.DeleteTaskAsync(taskId);
            return true;
        }

        public async Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId)
        {
            return await _repository.GetAllTasksFromProjectAsync(projectId);
        }

        public async Task<Task_> GetTaskAsync(long taskId)
        {
            return await _repository.GetTaskAsync(taskId);
        }

        public async Task<Task_> UpdateTaskAsync(EditTaskDTO editTaskDTO)
        {
            Task_ taskToUpdate = await _repository.GetTaskAsync(editTaskDTO.Id);
            taskToUpdate.TaskDescription = editTaskDTO.TaskDescription;
            taskToUpdate.TaskName = editTaskDTO.TaskName;
            return await _repository.UpdateTaskAsync(taskToUpdate);
        }
    }
}
