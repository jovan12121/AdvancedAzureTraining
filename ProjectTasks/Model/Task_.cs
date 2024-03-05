namespace ProjectTasks.Model
{
    public class Task_
    {
        public long Id { get; set; }
        public string TaskName { get;set; }
        public string TaskDescription { get; set; }
        public Project Project { get; set; }
        public long ProjectId { get; set; }
    }
}
