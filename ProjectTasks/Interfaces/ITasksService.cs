using ProjectTasks.DTO;
using ProjectTasks.Enums;
using ProjectTasks.Model;

namespace ProjectTasks.Interfaces
{
    public interface ITasksService
    {
        Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId);
        Task<bool> DeleteTaskAsync(long taskId);
        Task<Task_> GetTaskAsync(long taskId);
        Task<Task_> UpdateTaskAsync(EditTaskDTO editTaskDTO);
        Task<Task_> AddTaskAsync(AddTaskDTO addTaskDTO);
        Task<Task_> UpdateTaskStatusAsync(long taskId, TaskStatus_ taskStatus);
    }
}
