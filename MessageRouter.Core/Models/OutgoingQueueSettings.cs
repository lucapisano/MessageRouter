using System;

namespace MessageRouter
{
    public class OutgoingQueueSettings
    {
        /// <summary>
        /// the interval between sending cycles
        /// </summary>
        public TimeSpan SendingInterval { get; set; } = TimeSpan.FromSeconds(1);
    }
}
