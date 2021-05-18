using System;

namespace MessageRouter
{
    public class IncomingQueueSettings
    {
        /// <summary>
        /// the number of messages retrieved per cycle
        /// </summary>
        public int RetrieveBatchSize { get; set; } = 1;
        /// <summary>
        /// the interval between cycles
        /// </summary>
        public TimeSpan RetrievePollingInterval { get; set; } = TimeSpan.FromSeconds(10);
    }
}
