using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Providers
{
    public class RabbitMqBroker : IBroker, IDisposable
    {
        private readonly IOptionsMonitor<RabbitMqOptions> _options;
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMqBroker(IOptionsMonitor<RabbitMqOptions> options, ILogger<RabbitMqBroker> logger=default)
        {
            _options = options;
            _logger = logger;
        }
        async Task EnsureConnectionAlive()
        {
            if (_channel == default || _connection == default || !_connection.IsOpen || !_channel.IsOpen)
            {
                Dispose();
                ConnectionFactory factory = new ConnectionFactory { Uri = new Uri(_options.CurrentValue.ConnectionString) };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
        }
        public async Task<Message> GetNextMessage(MessageRetrieveArguments arguments)
        {
            try
            {
                await EnsureConnectionAlive();
                if (arguments == default)
                    throw new ArgumentNullException(nameof(arguments));
                BasicGetResult result = _channel.BasicGet(arguments.QueueName, arguments.AutoAck);
                if (result == null)
                {
                    return default;
                }
                else
                {
                    return ToMessage(result);
                    //if autoAck, this is not required. Otherwise must be called at the end of message handling
                    //_channel.BasicAck(result.DeliveryTag, false);
                }
            }
            catch(Exception e)
            {
                _logger?.LogError(e,"error reading next message");
                return default;
            }
        }
        Message ToMessage(BasicGetResult result)
        {
            var o = new Message { };
            o.MessageId = result.DeliveryTag.ToString();
            o.Body = result.Body.ToArray();//here bytes are transferred to RAM memory, so that caller has no problems if memory exausts
            o.Header = result.BasicProperties.Headers;
            string contentType = default;
            if (result.BasicProperties.Headers.ContainsKey("Content-Type"))
            {
                contentType = Encoding.UTF8.GetString(result.BasicProperties.Headers["Content-Type"] as byte[]);
            }
            else
                contentType = result.BasicProperties.ContentType;
            if (contentType != default)
                AddHeaderSafe("Content-Type", contentType, o.Header, true);
            //TODO: add any other header
            return o;
        }
        void AddHeaderSafe(string key, object value, IDictionary<string,object>destination, bool replace = false)
        {
            if (value == default)
                return;
            if (!destination.ContainsKey(key) && !replace)
                destination.Add(key, value);
            else if(replace)
            {
                destination[key] = value;
            }
        }
        public async Task SendMessage(OutgoingMessage message)
        {
            _channel.BasicPublish(exchange: message.QueueName, routingKey: "", mandatory: true,
                                 basicProperties: GetBasicProperties(message.Message.Header),
                                 body: message.Message.Body);
        }
        IBasicProperties GetBasicProperties(IDictionary<string, object> header)
        {
            IBasicProperties props = _channel.CreateBasicProperties();
            props.Headers = header;
            return props;
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _channel = default;
                _connection = default;
            }
            catch (Exception) { }
        }

        public async Task AcknowledgeMessage(Message message)
        {
            if (message == default)
                throw new ArgumentNullException(nameof(message));
            _channel.BasicAck(Convert.ToUInt64(message.MessageId), false);
        }
    }
}
