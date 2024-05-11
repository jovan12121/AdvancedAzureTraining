using Microsoft.EntityFrameworkCore;
using ProjectTasks.Infrastracture;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;

namespace ProjectTasks.Repository
{
    public class ProjectTaskRepository : IProjectTaskRepository
    {
        private DatabaseContext _databaseContext;
        public ProjectTaskRepository(DatabaseContext databaseContext) 
        {
            _databaseContext = databaseContext;
        }
        public async Task<Task_> AddTaskAsync(Task_ task)
        {
            await _databaseContext.Tasks.AddAsync(task);
            await _databaseContext.SaveChangesAsync();
            return task;
        }
        public async Task DeleteTaskAsync(long taskId)
        {
            Task_ taskToDelete = _databaseContext.Tasks.FirstOrDefault(t=>t.Id == taskId);
            if (taskToDelete == null)
            {
                throw new ApplicationException("Task not found.");
            }
            _databaseContext.Tasks.Remove(taskToDelete);
            await _databaseContext.SaveChangesAsync();
        }
        public async Task<Task_> UpdateTaskAsync(Task_ task)
        {
            Task_ retVal = _databaseContext.Tasks.Update(task).Entity;
            await _databaseContext.SaveChangesAsync();
            return retVal;
        }
        public async Task<Task_> GetTaskAsync(long taskId)
        {
            Task_ retVal = await _databaseContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if(retVal == null)
            {
                throw new ApplicationException("Task not found");
            }
            return retVal;

        }
        public async Task<Project> AddProjectAsync(Project project)
        {
            await _databaseContext.Projects.AddAsync(project);
            await _databaseContext.SaveChangesAsync();
            return project;
        }

        public async Task DeleteProjectAsync(long projectId)
        {
            Project projectToDelete = await _databaseContext.Projects.FirstOrDefaultAsync(p=>p.Id == projectId);
            if (projectToDelete == null)
            {
                throw new ApplicationException("Project not found.");
            }
            _databaseContext.Projects.Remove(projectToDelete);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<Project> UpdateProjectAsync(Project project)
        {
            Project retVal = _databaseContext.Projects.Update(project).Entity;
            await _databaseContext.SaveChangesAsync();
            return retVal;
        }
        public async Task<Project> GetProjectAsync(long projectId)
        {
            Project retVal = await _databaseContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
            if(retVal == null)
            {
                throw new ApplicationException("Project not found.");
            }
            return retVal;
        }
        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _databaseContext.Projects.Include(project=>project.Tasks).ToListAsync();
        }
        public async Task<List<Task_>> GetAllTasksFromProjectAsync(long projectId)
        {
            return await _databaseContext.Tasks.Where(t=>t.ProjectId == projectId).ToListAsync();
        }

        public async Task<FileAttachment> AddFileToProjectAsync(FileAttachment fileAttachment)
        {
            await _databaseContext.Files.AddAsync(fileAttachment);
            await _databaseContext.SaveChangesAsync();
            return fileAttachment;
        }

        public async Task<FileAttachment> AddFileToTasksAsync(FileAttachment fileAttachment)
        {
            await _databaseContext.Files.AddAsync(fileAttachment);
            await _databaseContext.SaveChangesAsync();
            return fileAttachment;
        }

        public async Task<List<FileAttachment>> GetFilesFromProjectAsync(long projectId)
        {
            return await _databaseContext.Files.Where(f => f.ProjectId == projectId).ToListAsync();
        }

        public async Task<List<FileAttachment>> GetFilesFromTaskAsync(long taskId)
        {
            return await _databaseContext.Files.Where(f => f.TaskId == taskId).ToListAsync();
        }

        public async Task<FileAttachment> GetFileAsync(long fileId)
        {
            return await _databaseContext.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        }
        public async Task DeleteFileAsync(long fileId)
        {
            await _databaseContext.Files.Where(f => f.Id == fileId).ExecuteDeleteAsync();
            await _databaseContext.SaveChangesAsync();
        }
    }
}
