using QueueCommon;
using System;

namespace ProcessingService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
				using(var queue = new Queue())
				{
					var receiver = new Receiver();
					receiver.Receive(queue);
				}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());   
            }
        }
    }
}
