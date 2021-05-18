namespace MessageRouter
{
    public class OutgoingMessage
    {
        public Message Message { get; set; }
        public string QueueName { get; set; }
    }
}
