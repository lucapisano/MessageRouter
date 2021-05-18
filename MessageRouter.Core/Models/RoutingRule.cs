using MessageRouter;
using System.Collections.Generic;

namespace MessageRouter
{
    public class RoutingRule
    {
        /// <summary>
        /// fetches only messages coming from this queue
        /// </summary>
        public IncomingQueueDefinition IncomingQueue { get; set; }
        /// <summary>
        /// the conditions, which satisfied all together, filter incoming messages to be redirected to outgoing message queue
        /// </summary>
        public List<RoutingCondition> Conditions { get; set; } = new List<RoutingCondition>();
        public OutgoingQueueDefinition OutgoingQueue { get; set; }
    }
}