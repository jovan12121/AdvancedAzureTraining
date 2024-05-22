
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTasksFunction.Model
{
    public enum ProjectStatus
    {
        IN_PROGRESS = 0,
        COMPLETE = 1,
        FAILED = 2
    }
    public class Project
    {
        public string id { get; set; }
        public long Id { get; set; }
        public string ProjectName { get; set; }
        public string Code { get; set; }
        public DateTime DateStarted { get; set; }
        public DateTime? DateFinished { get; set; }

        public int Status { get; set; }
        public List<Task_> Tasks { get; set; }
        public List<FileAttachment> Files { get; set; }
    }
}
