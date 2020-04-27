using RabbitMQ.Client;
using RMQ.Application;

namespace RMQ.Infrastructure.Common
{
    public class Queue
    {
        public IConnection Connection;
        public IModel Model;

    }
    public class QueueBuilder
    {
        protected Queue QueueBuilderInfo;

        public QueueBuilder()
        {
            QueueBuilderInfo = new Queue();
        }

        public Queue Build() => QueueBuilderInfo;
    }


    public class QueueCreateConnectionBuilder<T> : QueueBuilder where T : QueueCreateConnectionBuilder<T>
    {
        public T CreateConnection()
        {
            QueueBuilderInfo.Connection = SingletonConnection.GetInstance().CreateConnection();
            return (T)this;
        }
    }
    public class QueueCreateBuilder<T> :
        QueueCreateConnectionBuilder<QueueCreateBuilder<T>> where T : QueueCreateBuilder<T>
    {

        public T CreateQueue()
        {
            var model = QueueBuilderInfo
                                        .Connection
                                        .CreateModel();
            /*
             * Message durability
               We have learned how to make sure that even if the consumer dies, the task isn't lost. But our tasks will still be lost if RabbitMQ server stops.
               When RabbitMQ quits or crashes it will forget the queues and messages unless you tell it not to. Two things are required to make sure that messages aren't lost: we need to mark both the queue and messages as durable.
               First, we need to make sure that the queue will survive a RabbitMQ node restart queue.In order to do so, we need to declare it as durable:
            */
            model.QueueDeclare(queue: Utility.QUEUE_NAME,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            
         
            QueueBuilderInfo.Model = model;

            return (T)this;
        }
    }


    public class QueueDirectorBuilder : QueueCreateBuilder<QueueDirectorBuilder>
    {
        public static QueueDirectorBuilder NewQueueBuilderInfo = new QueueDirectorBuilder();
    }
}
