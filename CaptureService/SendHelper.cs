using QueueCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureService
{
    public class SendHelper
    {
        public static void Send(Queue queue, byte[] message)
        {
            if (message.Length > Contracts.ChunkSize)
            {
                Sender.SendLargeMessage(queue, message);
            }
            else
            { 
                Sender.Send(queue, message); 
            }
        }
    }
}
