using RabbitMQ.Client;
using RMQ.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMQ.Infrastructure.Common
{
    public class Queue
    {
        public  IConnection Connection;
        public  IModel Model;
       
    }
    public  class QueueBuilder
    {
        protected Queue QueueBuilderInfo;

        public QueueBuilder()
        {
            QueueBuilderInfo = new Queue();
        }

        public Queue Build() => QueueBuilderInfo;
    }


    public class QueueCreateConnection<T>: QueueBuilder where T: QueueCreateConnection<T>
    {
        public T CreateConnection(string hostname, string username, string password)
        {

            QueueBuilderInfo.Connection = SingletonConnection.GetInstance().CreateConnection();
            return (T)this;
        }
    }
    public class QueueCreateBuilder<T>:
        QueueCreateConnection<QueueCreateBuilder<T>> where T: QueueCreateBuilder<T>
    {
       
       public T CreateQueue()
        {
            var model = QueueBuilderInfo
                                        .Connection
                                        .CreateModel();

            model.QueueDeclare(Utility.QUEUE_NAME, true, false, false, null);

            QueueBuilderInfo.Model = model;

            return (T)this;
        }
    }


    public class QueueDirectorBuilder: QueueCreateBuilder<QueueDirectorBuilder>
    {
        public static QueueDirectorBuilder NewQueueBuilderInfo = new QueueDirectorBuilder();
    }
}
