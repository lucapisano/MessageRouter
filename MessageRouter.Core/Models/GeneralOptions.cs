using System;
using System.Collections.Generic;

namespace MessageRouter
{
    public class GeneralOptions
    {
        public List<RoutingRule> Rules { get; set; } = new List<RoutingRule>();
        public List<BrokerDefinition> Brokers { get; set; } = new List<BrokerDefinition>();
        public TimeSpan RoutingInterval { get; set; } = TimeSpan.FromMilliseconds(5*1000);
    }
}
