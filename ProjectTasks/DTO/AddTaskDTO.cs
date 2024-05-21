namespace ProjectTasks.DTO
{
    public class AddTaskDTO
    {
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public long ProjectId { get; set; }
        public DateTime? DateStarted { get; set; }
        public Dictionary<string,string> MetaData { get; set; }

    }

}
