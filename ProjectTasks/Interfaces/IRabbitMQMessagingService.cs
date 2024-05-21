namespace ProjectTasks.Interfaces
{
    public interface IRabbitMQMessagingService
    {
        void PublishMessage(string message);
    }
}
