using System.Threading.Tasks;

namespace MessageRouter
{
    public interface IBroker
    {
        /// <summary>
        /// this methods retrieves the next message
        /// internally, it must ensure the connection to the broker is alive
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        Task<Message> GetNextMessage(MessageRetrieveArguments arguments);
        Task SendMessage(OutgoingMessage message);
        Task AcknowledgeMessage(Message message);
    }
}
