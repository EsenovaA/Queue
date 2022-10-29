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
                var receiver = new Receiver();
                receiver.Receive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());   
            }
        }
    }
}
