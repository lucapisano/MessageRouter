namespace MessageRouter
{
    public class IncomingQueueDefinition: QueueDefinition
    {
        public MessageRetrieveArguments MessageRetrieve { get; set; }
    }
}