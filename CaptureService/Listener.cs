using QueueCommon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace CaptureService
{
    public class Listener
    {
        private const string _pathToListenedFolder = @"..\..\..\ListenedFolder";
        private const string _pathToNotSuitFolder = @"..\..\..\NotSuit";
        private const string _extension = ".zip";

        public static void Listen(Queue queue)
        {
            while (true)
            {
                var file = TryListen();
                if (file != null)
                {
                    SendHelper.Send(queue, file);
                }
            }
        }

        private static byte[] TryListen()
        {
            try
            {
                CreateDirectoriesIfNotExist();

                var filePaths = Directory.GetFiles(_pathToListenedFolder);

                if (filePaths == null || !filePaths.Any())
                {
                    return null;
                }

                var firstFilePath = filePaths.First();
                var fileInfo = new FileInfo(firstFilePath);

                if (fileInfo.Extension != _extension)
                {
                    Console.WriteLine($"File {fileInfo.FullName} detected. But extension {fileInfo.Extension} doesn't suit. Replace file to {_pathToNotSuitFolder}.");
                    File.Move(firstFilePath, Path.Combine(_pathToNotSuitFolder, fileInfo.Name));
                    return null;
                }


                var firstFile = File.ReadAllBytes(firstFilePath);
                Console.WriteLine($"File {fileInfo.FullName} detected.");

                File.Delete(firstFilePath);
                Console.WriteLine($"File {fileInfo.FullName} deleted.");

                return firstFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private static void CreateDirectoriesIfNotExist()
        {
            if (!Directory.Exists(_pathToListenedFolder))
            {
                Directory.CreateDirectory(_pathToListenedFolder);
                Console.WriteLine($"Directory by path {_pathToListenedFolder} created.");
            }

            if (!Directory.Exists(_pathToNotSuitFolder))
            {
                Directory.CreateDirectory(_pathToNotSuitFolder);
                Console.WriteLine($"Directory by path {_pathToNotSuitFolder} created.");
            }
        }

    }

 
}
