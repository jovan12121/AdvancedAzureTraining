using ProjectTasks.Enums;

namespace ProjectTasks.Interfaces
{
    public interface IReportGeneratorService
    {
        public Task<string> GenerateReportAboutProjects(DateTime? startDate, DateTime? endDate, ProjectStatus? projectStatus);
        public Task<string> GenerateReportAboutTasks(DateTime? startDate, DateTime? endDate, TaskStatus_? taskStatus, long projectId);

    }
}
