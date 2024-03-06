using ProjectTaskCosmos.Model;

namespace ProjectTaskCosmos.Interfaces
{
    public interface ITasksService
    {
        Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId);
        Task<Task_> GetTaskAsync(long taskId);
    }
}
