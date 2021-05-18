namespace MessageRouter
{
    public class BrokerDefinition
    {
        /// <summary>
        /// Identifies a broker
        /// </summary>
        public string Id { get; set; }
        public IncomingQueueSettings DefaultIncomingQueueSettings { get; set; }
        public OutgoingQueueSettings DefaultOutgoingQueueSettings { get; set; }
        /// <summary>
        /// the fully qualified name of implementation type
        /// it must be loaded in GAC at service provider build time
        /// it must implement <see cref="IBroker"/>
        /// </summary>
        public string ImplementationTypeName { get; set; }
    }
}
