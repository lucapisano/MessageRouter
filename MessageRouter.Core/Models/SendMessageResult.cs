namespace MessageRouter
{
    public class SendMessageResult
    {
        public override string ToString()
        {
            return Sent.ToString();
        }
        public bool Sent { get; set; }
    }
}
