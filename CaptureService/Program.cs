using QueueCommon;
using System;
using System.Threading;

namespace CaptureService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Error: Dispose isn't used
                var queue = new Queue();
                Listener.Listen(queue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
