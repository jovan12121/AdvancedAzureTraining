using ProjectTaskCosmos.Interfaces;
using ProjectTaskCosmos.Model;

namespace ProjectTaskCosmos.Services
{
    public class TasksService : ITasksService
    {
        private CosmosService _comsosService;
        public TasksService(IConfiguration configuration)
        {
            _comsosService = new CosmosService(configuration);
        }
        public async Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId)
        {
            Project project = await _comsosService.GetProjectAsync(projectId);
            return project.Tasks;
        }

        public async Task<Task_> GetTaskAsync(long taskId)
        {
            List<Project> projects = await _comsosService.GetProjectsAsync();
            foreach (Project project in projects)
            {
                Task_? t = project.Tasks.FirstOrDefault(t => t.Id == taskId);
                if(t is not null)
                {
                    return t;
                }
            }
            throw new Exception("Task with that id doesn't exist!");
        }
    }
}
