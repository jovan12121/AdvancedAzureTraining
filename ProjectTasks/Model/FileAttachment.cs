namespace ProjectTasks.Model
{
    public class FileAttachment
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Path { get;set; }
        public long? ProjectId { get; set; }
        public Project Project { get; set; }
        public long? TaskId { get; set; }
        public Task_ Task { get; set; }
    }
}
