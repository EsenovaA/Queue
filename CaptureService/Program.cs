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
                using(var queue = new Queue())
				{
					Listener.Listen(queue);
				}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
