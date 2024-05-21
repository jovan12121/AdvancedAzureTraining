using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTasksFunction.Model
{
    public enum TaskStatus_
    {
        IN_PROGRESS = 0,
        COMPLETE = 1,
        FAILED = 2
    }
    public class Task_
    {
        public long Id { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }

        public DateTime DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }

        public int Status { get; set; }
        public long ProjectId { get; set; }
        public Dictionary<string, string>? MetaData { get; set; }
        public List<FileAttachment> Files { get; set; }

    }
}
