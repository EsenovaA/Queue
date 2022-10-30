using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace QueueCommon
{
    public class Queue: IDisposable
    {
        private IConnection _connection;
        private IModel _channel;

        public IModel Channel { get { return _channel; } }

        public Queue()
        {
            InitQueue();
        }

        private void InitQueue()
        {
            var factory = new ConnectionFactory() { HostName = Contracts.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: Contracts.QueueName,
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            Console.WriteLine($"Queue {Contracts.QueueName} declared.");
        }

        public void PublishMessage(byte[] message)
        {
            _channel.BasicPublish(exchange: "",
                                     routingKey: Contracts.QueueName,
                                     basicProperties: null,
                                     body: message);
        }

        public void PublishLargeMessage(byte[] message, string identifier)
        {
            int remainingFileSize = Convert.ToInt32(message.Length); // Warning: no need in this convertion

            bool finished = false;
            byte[] buffer;
            int index = 0;
            int number = 0;

            while (true)
            {
                if (remainingFileSize <= 0) 
                { 
                    break; 
                }

                int read;

                if (remainingFileSize > Contracts.ChunkSize)
                {
                    buffer = new byte[Contracts.ChunkSize];
                    Array.Copy(message, index, buffer, 0, Contracts.ChunkSize);
                    index += Contracts.ChunkSize;
                    read = Contracts.ChunkSize;
                }
                else
                {
                    buffer = new byte[remainingFileSize];
                    Array.Copy(message, index, buffer, 0, remainingFileSize);
                    read = remainingFileSize;
                    finished = true;
                }

                IBasicProperties basicProperties = _channel.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add(Contracts.IsChunk, true);
                basicProperties.Headers.Add(Contracts.Identifier, identifier);//if there are multiple chunked messages, then treat them separately
                basicProperties.Headers.Add(Contracts.Number, number);
                basicProperties.Headers.Add(Contracts.Finished, finished);

                _channel.BasicPublish("", routingKey: Contracts.QueueName, basicProperties, buffer);
                Console.WriteLine($"Chunk published with size {buffer.Length}, IsLast: {finished}.");
                remainingFileSize -= read;
                number++;
            }
            
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            _channel.Close();
            _channel.Dispose();
        }
    }
}
