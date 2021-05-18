using System.Collections.Generic;

namespace MessageRouter
{
    public class GeneralOptions
    {
        public List<RoutingRule> Rules { get; set; } = new List<RoutingRule>();
        public List<BrokerDefinition> Brokers { get; set; } = new List<BrokerDefinition>();
    }
}
