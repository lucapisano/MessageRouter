namespace MessageRouter
{
    public class MessageRetrieveArguments
    {
        public string QueueName { get; set; }
        /// <summary>
        /// indicates if the client shall acknowledge the message as soon as it arrives
        /// if false, the client MUST acknowledge the message manually
        /// </summary>
        public bool AutoAck { get; set; } = true;
    }
}
