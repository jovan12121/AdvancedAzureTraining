using ProjectTasks.Enums;
using System.Text.Json.Serialization;

namespace ProjectTasks.Model
{
    public class Task_
    {
        public long Id { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }

        public DateTime DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }

        public TaskStatus_ Status { get; set; }
        [JsonIgnore]
        public Project Project { get; set; }
        public long ProjectId { get; set; }
        [JsonIgnore]
        public List<FileAttachment> Files { get; set; }
        public Dictionary<string, string>? MetaData { get; set; }
    }
}
