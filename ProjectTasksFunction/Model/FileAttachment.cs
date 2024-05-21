using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTasksFunction.Model
{
    public class FileAttachment
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public long? ProjectId { get; set; }
        public Project Project { get; set; }
        public long? TaskId { get; set; }
        public Task_ Task { get; set; }
    }
}
