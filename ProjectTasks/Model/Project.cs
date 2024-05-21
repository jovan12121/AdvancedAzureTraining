using ProjectTasks.Enums;

namespace ProjectTasks.Model
{
    public class Project
    {
        public long Id { get; set; }
        public string ProjectName { get; set; }
        public string Code { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }

        public ProjectStatus Status { get; set; }
        public List<Task_> Tasks { get; set; }
        public List<FileAttachment> Files { get; set; }
    }
}
