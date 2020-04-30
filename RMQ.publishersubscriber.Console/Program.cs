using RabbitMQ.Client;
using RMQ.Application;
using RMQ.Infrastructure.Common;
using static System.Console;
using System.Text;

namespace RMQ.publishersubscriber.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            for(var i = 0; i < 20; i++)
            {
                PublishQueues();

            }

           
            WriteLine(" Press [enter] to exit.");
            ReadLine();
        }
        /*
         * The core idea in the messaging model in RabbitMQ is that the producer never sends any messages directly to a queue. 
         * Actually, quite often the producer doesn't even know if a message will be delivered to any queue at all.
           Instead, the producer can only send messages to an exchange. An exchange is a very simple thing. 
           On one side it receives messages from producers and the other side it pushes them to queues. The exchange must know exactly what to do with a message it receives.
           Should it be appended to a particular queue? Should it be appended to many queues? Or should it get discarded. The rules for that are defined by the exchange type.
         */
        static void PublishQueues()
        {
            var builder = QueueDirectorBuilder
                            .NewQueueBuilderInfo
                            .CreateConnection()
                            .CreateExchange()
                            .Build();

            var property = builder.Model.CreateBasicProperties();
            property.Persistent = true;
            var message = Encoding.UTF8.GetBytes("just another random messages");
            builder.Model.BasicPublish(exchange: Utility.EXCHANGE_NAME,
                                       routingKey: "",
                                       basicProperties: property,
                                       body: message);

            WriteLine(" [x] Sent {0}", message);
        }
    }
}
