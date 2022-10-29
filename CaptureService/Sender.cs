using QueueCommon;
using System;

namespace CaptureService
{
    public class Sender
    {
        public static void Send(Queue queue, byte[] message)
        {
            if(message == null || message.Length == 0)
            {
                return;
            }

            queue.PublishMessage(message);
            Console.WriteLine($"Sent message with {message.Length} bytes.");
        }

        public static void SendLargeMessage(Queue queue, byte[] message)
        {
            if (message == null || message.Length == 0)
            {
                return;
            }

            queue.PublishLargeMessage(message, string.Format("LargeFile_{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now));
            Console.WriteLine($"Sent message with {message.Length} bytes.");

            return;
        }
    }
}
