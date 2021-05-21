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

    [luca pisano]: <https://lucapisano.it>