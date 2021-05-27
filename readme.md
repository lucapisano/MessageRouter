# MessageRouter
## By [Luca Pisano]

[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://github.com/lucapisano/MessageRouter)

MessageRouter is a docker ready, configuration based message routing software which enables users to route messages based on JSON and XML selectors.

It interacts with message brokers such as RabbitMQ and evaluates the message body to route it to other queues.

Standard MQ brokers route messages inferring on headers to achieve high performance in message delivery. MessageRouter reads the body of the message, evaluates it against queries (written in JSON or XML format) and routes the message to the relevant queue.
This approach might result in performance loss compared to header only routing, but enables for greater routing flexibility.

## Features

- Configuration based routing
- Configuration HOT reload
- Multiple broker support, current implementation provides RabbitMQ support, but the architecture can be easily expanded to other brokers such as Kafka

This is an example of how you can configure the router to inspect and route messages using JSON and XML expressions
```json
{
  "GeneralOptions": {
    "Rules": [
      {
        "IncomingQueue": {
          "BrokerId": "rabbitmq",
          "MessageRetrieve": {
            "QueueName": "input_queue"
          }
        },
        "Conditions": [
          {
            "Query": "//note/to[@id='aa']",
            "Type": "application/xml"
          },
          {
            "Query": "$.Manufacturers[?(@.Int > 1 )]",
            "Type": "application/json"
          }
        ],
        "OutgoingQueue": {
          "BrokerId": "rabbitmq",
          "QueueName": "output_exchange"
        }
      }
    ],
    "Brokers": [
      {
        "Id": "rabbitmq",
        "ImplementationTypeName": "MessageRouter.Providers.RabbitMqBroker"
      }
    ]
  },
  "RabbitMqOptions": {
    "ConnectionString": "amqp://guest:guest@localhost:5672"
  },
  "RoutingInterval": "00:00:10"
}
```
    [luca pisano]: <https://lucapisano.it>