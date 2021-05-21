using MessageRouter.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace MessageRouter
{
    public class RouterEngine
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger _logger;
        private bool _continue = false;
        private Action _action;
        private Task _task;

        public RouterEngine(IServiceProvider sp)
        {
            _sp = sp;
            _logger = _sp.GetRequiredService<ILogger<RouterEngine>>();            
        }
        public void Start()
        {
            _continue = true;
            _action = async () => { await RunAsync(); };
            _task = Task.Run(_action);
        }
        public void Stop()
        {
            _continue = false;
            _task.Wait();
        }
        IEnumerable<IBroker> ListAvailableBrokers(IServiceProvider sp)
        {
            return sp.GetServices<IBroker>();
        }
        IBroker GetBroker(IServiceProvider sp, string brokerDefinitionId)
        {
            BrokerDefinition brokerDefinition = GetOptions(sp).Brokers.FirstOrDefault(x => x.Id == brokerDefinitionId);
            var brokers = ListAvailableBrokers(sp);
            var service = brokers.FirstOrDefault(x => x.GetType().FullName == brokerDefinition.ImplementationTypeName /*|| x.GetType().Name==brokerDefinition.ImplementationTypeName*/);
            /*
            Type brokerType = Type.GetType(brokerDefinition.ImplementationTypeName);
            if (brokerType == default)
            {
                _logger?.LogWarning($"broker type {brokerDefinition.ImplementationTypeName} not found in GAC. Please ensure to have imported the relative implementation into service collection");
                return default;
            }
            var service = sp.GetService(brokerType);
            */
            if (service == default)
            {
                _logger?.LogWarning($"broker type {brokerDefinition.ImplementationTypeName} not found in GAC. Please ensure to have imported the relative implementation into service collection");
            }
            else if (service is IBroker)
            {
                return (IBroker)service;
            }
            else
            {
                _logger?.LogWarning($"broker type {brokerDefinition.ImplementationTypeName} does not implement {typeof(IBroker).FullName}");
                return default;
            }
            return default;
        }
        IEnumerable<IBroker> GetBrokers(IServiceProvider sp)
        {
            var incomingDefinitions = GetOptions(sp).Rules.Select(x => x.IncomingQueue).ToList();
            return incomingDefinitions.Select(def => {
                BrokerDefinition brokerDefinition = GetOptions(sp).Brokers.FirstOrDefault(x => x.Id == def.BrokerId);
                return GetBroker(sp, brokerDefinition.Id);
            })
                .Where(x=>x!=default);
        }
        GeneralOptions GetOptions(IServiceProvider sp)
        {
            return sp.GetRequiredService<IOptionsMonitor<GeneralOptions>>().CurrentValue;
        }
        internal async Task RunAsync()
        {
            while (_continue)
            {
                using (var scope = _sp.CreateScope())
                {
                    var options = GetOptions(scope.ServiceProvider);
                    try
                    {
                        var brokers = GetBrokers(scope.ServiceProvider);
                        foreach (var broker in brokers)
                        {
                            try
                            {
                                var brokerDefinition = options.Brokers.First(x => x.ImplementationTypeName == broker.GetType().FullName);
                                IEnumerable<RoutingRule> brokerRules = options.Rules
                                    .Where(rule=>rule.IncomingQueue.BrokerId==brokerDefinition.Id);
                                foreach (var rule in brokerRules)
                                {
                                    Message message = default;
                                    message = await broker.GetNextMessage(rule.IncomingQueue.MessageRetrieve);
                                    if (message == default)
                                        continue;
                                    var applicableRules = options.Rules;
                                    var applicableConditions = applicableRules.SelectMany(x => x.Conditions.Where(x => x.Type == GetContentTypeHeader(message)));
                                    bool conditionsVerified = applicableConditions.Any();
                                    foreach (var condition in applicableConditions)
                                    {
                                        conditionsVerified = EvaluateCondition(message, condition);
                                        if (!conditionsVerified)
                                            break;
                                    }
                                    _logger?.LogDebug($"message {message} condition evaluation result = {conditionsVerified}");
                                    if (conditionsVerified)
                                    {
                                        OutgoingQueueDefinition outgoingQueue = rule.OutgoingQueue;
                                        if (outgoingQueue != default)
                                        {
                                            var outgoingBroker = GetBroker(scope.ServiceProvider, outgoingQueue.BrokerId);
                                            if (outgoingBroker != default)
                                            {
                                                await outgoingBroker.SendMessage(new OutgoingMessage
                                                {
                                                    QueueName = outgoingQueue.QueueName,
                                                    Message = message
                                                });
                                                _logger?.LogDebug($"message {message} routed to queue {outgoingQueue.QueueName} on broker {outgoingQueue.BrokerId}");
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                _logger?.LogError(e, $"error processing messages from broker {broker.GetType().FullName}");
                            }
                        }
                        
                    }
                    finally
                    {
                        await Task.Delay(options.RoutingInterval);
                    }
                }
            }
        }
        string GetMessageBodyAsString(Message message)
        {
            return Encoding.UTF8.GetString(message.Body);
        }
        bool EvaluateCondition(Message message, RoutingCondition condition)
        {
            switch(condition.Type)
            {
                case "application/json":
                    {
                        JObject o = JObject.Parse(GetMessageBodyAsString(message));
                        bool res = o.SelectToken(condition.Query) != default;
                        return res;
                    }
                case "application/xml":
                    {
                        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(GetMessageBodyAsString(message))))
                        {
                            var doc = new XPathDocument(stream);
                            var navigator = doc.CreateNavigator();
                            return navigator.Select(condition.Query).Count > 0;
                        }
                    }
                default: {
                        return false; 
                    }
            }
        }
        string GetContentTypeHeader(Message message)
        {
            return message.Header["Content-Type"] as string;
        }
    }
}
