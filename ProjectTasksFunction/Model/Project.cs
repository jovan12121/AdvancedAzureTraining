using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTasksFunction.Model
{
    internal class Project
    {
        public string id { get; set; }
        public long Id { get; set; }
        public string ProjectName { get; set; }
        public string Code { get; set; }
        public List<Task_> Tasks { get; set; }
    }
}
