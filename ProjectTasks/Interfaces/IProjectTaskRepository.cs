using ProjectTasks.Infrastracture;
using ProjectTasks.Model;

namespace ProjectTasks.Interfaces
{
    public interface IProjectTaskRepository
    {
        public Task<Task_> AddTaskAsync(Task_ task);
        public Task DeleteTaskAsync(long taskId);
        public Task<Task_> UpdateTaskAsync(Task_ task);
        public Task<Task_> GetTaskAsync(long taskId);
        public Task<Project> AddProjectAsync(Project project);

        public Task DeleteProjectAsync(long projectId);

        public  Task<Project> UpdateProjectAsync(Project project);
        public  Task<Project> GetProjectAsync(long projectId);
        public  Task<List<Project>> GetProjectsAsync();
        public  Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId);
        public  Task<FileAttachment> AddFileToProjectAsync(FileAttachment fileAttachment);
        public Task<FileAttachment> AddFileToTasksAsync(FileAttachment fileAttachment);
        public Task<List<FileAttachment>> GetFilesFromProjectAsync(long projectId);
        public Task<List<FileAttachment>> GetFilesFromTaskAsync(long taskId);
        public Task<FileAttachment> GetFileAsync(long fileId);
        public Task DeleteFileAsync(long fileId);

    }
}
