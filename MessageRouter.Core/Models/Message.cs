using System.Collections.Generic;

namespace MessageRouter
{
    public class Message
    {
        public override string ToString()
        {
            return $"{MessageId}";
        }
        public string MessageId { get; set; }
        public IDictionary<string, object> Header { get; set; }
        public byte[] Body { get; set; }
    }
}
