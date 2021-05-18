using System;
namespace MessageRouter
{
    public class RoutingCondition
    {
        /// <summary>
        /// the value of the condition
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// if null, <see cref="Query"/> is evaluated as raw textual expression.
        /// Otherwise matches the Content-Type header of the message.
        /// e.g. If Content-Type header is application/json, <see cref="Query"/> is evaluated against a json object
        /// </summary>
        public string Type { get; set; }
    }
}