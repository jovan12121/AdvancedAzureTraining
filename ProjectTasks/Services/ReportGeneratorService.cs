using ProjectTasks.Enums;
using ProjectTasks.Interfaces;
using ProjectTasks.Model;

namespace ProjectTasks.Services
{
    public class ReportGeneratorService : IReportGeneratorService
    {
        private readonly IProjectTaskRepository _repository;

        public ReportGeneratorService(IProjectTaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GenerateReportAboutProjects(DateTime? startDate, DateTime? endDate, ProjectStatus? projectStatus)
        {
            List<Project> projects = await _repository.GetProjectsAsync();
            string retVal = "";
            if(startDate != null)
            {
                projects = projects.Where(p=>p.DateStarted >= startDate).ToList();
            }
            if(endDate != null)
            {
                projects = projects.Where(p=> p.DateFinished <= endDate).ToList();
            }
            if(projectStatus != null)
            {
                projects = projects.Where(p => p.Status == projectStatus).ToList();
            }
            if(projects.Count == 0)
            {
                return "There are not projects that meet this filters.";
            }
            foreach (Project project in projects)
            {
                retVal += $"Project name: {project.ProjectName}\n";
                retVal += $"Code: {project.Code}\n";
                retVal += $"Start date: {project.DateStarted.ToString("G")}\n";
                string? endDateString = project.DateFinished != null ? project.DateFinished?.ToString("G") : "Not finished";
                retVal += $"End date: {endDateString}\n.";
                retVal += $"Project status: {project.Status.ToString().ToLower()}.\n";
                List<Task_> tasks = await _repository.GetAllTasksFromProjectAsync(project.Id);
                int tasksCount = tasks.Count;
                int finishedTasksCount = tasks.Where(t=>t.Status == TaskStatus_.COMPLETE).Count();
                int failedTasksCount = tasks.Where(t => t.Status == TaskStatus_.FAILED).Count();
                int inProgressTasksCount = tasks.Where(t => t.Status == TaskStatus_.COMPLETE).Count();
                retVal += $"Number of tasks: {tasksCount}.\nFinished tasks: {finishedTasksCount}.\nFailed tasks:{failedTasksCount}.\nTasks in progress: {inProgressTasksCount}.\n";
            }
            return retVal;
        }

        public async Task<string> GenerateReportAboutTasks(DateTime? startDate, DateTime? endDate, TaskStatus_? taskStatus, long projectId)
        {
            List<Task_> tasks = await _repository.GetAllTasksFromProjectAsync(projectId);
            string retVal = "";
            if(startDate!=null)
            {
                tasks = tasks.Where(t => t.DateStarted >= startDate).ToList();
            }
            if(endDate!=null)
            {
                tasks = tasks.Where(t=>t.DateFinished<= endDate).ToList();
            }
            if(taskStatus!=null)
            {
                tasks = tasks.Where(t=>t.Status == taskStatus).ToList();
            }
            if (tasks.Count == 0)
            {
                return "There are not tasks in project that meet this filters.";
            }
            retVal += $"Number of tasks: {tasks.Count}.\n";
            foreach (Task_ task in tasks)
            {
                retVal += $"Task name: {task.TaskName}.\n";
                retVal += $"Task description: {task.TaskDescription}.\n";
                retVal += $"Date started: {task.DateStarted.ToString("G")}.\n";
                string? endDateString = task.DateFinished != null ? task.DateFinished?.ToString("G") : "Not finished";
                retVal += $"Date finished: {endDateString}.\n";
                retVal += $"Task status: {task.Status.ToString().ToLower()}\n";

            }
            return retVal;
        }
    }
}
