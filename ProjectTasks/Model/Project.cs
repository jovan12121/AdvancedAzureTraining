namespace ProjectTasks.Model
{
    public class Project
    {
        public long Id { get; set; }
        public string ProjectName { get; set; }
        public string Code { get; set; }
        public List<Task_> Tasks { get; set; }
    }
}
