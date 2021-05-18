namespace MessageRouter
{
    public class QueueDefinition
    {
        /// <summary>
        /// <see cref="BrokerDefinition"/>
        /// </summary>
        public string BrokerId { get; set; }
        public string QueueName { get; set; }
    }
}