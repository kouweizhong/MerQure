﻿using MerQure.RbMQ;
using MerQure.RbMQ.Content;
using System;
using System.Collections.Generic;

namespace MerQure.Samples
{
    public static class SimpleExample
    {
        public static void Run()
        {
            var messagingService = new MessagingService();

            // RabbitMQ init
            messagingService.DeclareExchange("simple.exchange");
            messagingService.DeclareQueue("simple.queue");
            messagingService.DeclareBinding("simple.exchange", "simple.message.*", "simple.queue");

            // Get the publisher and declare Exhange where publish messages
            var publisher = messagingService.GetPublisher("simple.exchange");

            // publish messages
            for (int i = 0; i <= 10; i++)
            {
                publisher.Publish(new Message("simple.message.test" + i, string.Format("Hello world {0} !", i)));
            }

            // Get the consumer on the existing queue and consume its messages
            var consumer = messagingService.GetConsumer("simple.queue");
            var random = new Random();
            consumer.Consume((object sender, IMessagingEvent args) =>
            {
                // we simulate the delivery success
                if (random.Next() % 2 == 0)
                {
                    Console.WriteLine("Retry " + args.Message.GetRoutingKey());
                    // send NACK: negative acknowlegdment to the queue
                    consumer.RejectDeliveredMessage(args);
                }
                else
                {
                    Console.WriteLine(args.Message.GetBody());
                    // send ACK: acknowlegdment to the queue 
                    consumer.AcknowlegdeDeliveredMessage(args);
                }
            });
        }
    }
}
