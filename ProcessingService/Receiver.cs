using QueueCommon;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProcessingService
{
    public class Receiver
    {
        private const string _logFolder = @"..\..\..\LogFolder";

        private readonly Queue _queue;

        private List<Chunk> chunks = new List<Chunk>();

        public Receiver()
        {
            _queue = new Queue();
        }

        public void Receive()
        {
            var consumer = new EventingBasicConsumer(_queue.Channel);

            consumer.Received += (model, ea) => OnMessageReceived(model, ea);

            _queue.Channel.BasicConsume(queue: Contracts.QueueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private void OnMessageReceived(object model, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            Console.WriteLine($"Received message with {body.Length} length.");

            IDictionary<string, object> headers = args.BasicProperties.Headers;
            bool isChunk = Convert.ToBoolean(headers[Contracts.IsChunk]);
            
            if (isChunk)
            {
                ProcessChunk(headers, body);
                return;
            }

            ProcessMessage(body);
        }

        private void ProcessMessage(byte[] body)
        {
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
                Console.WriteLine($"Directory by path {_logFolder} created.");
            }

            File.WriteAllBytes(Path.Combine(_logFolder, GenerateFileName()), body);
        }

        private void ProcessChunk(IDictionary<string, object> headers, byte[] body)
        {
            string identifier = Encoding.UTF8.GetString((headers[Contracts.Identifier] as byte[]));
            int number = (int)headers[Contracts.Number];
            bool isLastChunk = Convert.ToBoolean(headers[Contracts.Finished]);
            chunks.Add(new Chunk()
            {
                Identifier = identifier,
                Number = number,
                Data = body
            });

            if (isLastChunk)
            {
                ProcessChunkedMessage();
            }

            Console.WriteLine($"Processed chunk message with {body.Length} length.");
            return;
        }

        private void ProcessChunkedMessage()
        {
            var result = BuildMessageFromChunks();
            ProcessMessage(result);
        }

        private byte[] BuildMessageFromChunks()
        {
            var sortedChunks = chunks.OrderBy(c => c.Number).ToArray();
            var resultMessage = new List<byte>();
            foreach (var chunk in sortedChunks)
            {
                resultMessage.AddRange(chunk.Data);
            }

            chunks = null; 

            return resultMessage.ToArray();
        }

        private string GenerateFileName()
        {
            return string.Format("ReceivedFile_{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
        }
    }
}
