namespace QueueCommon
{
    public static class Contracts
    {
        public const string QueueName = "File";
        public const string HostName = "localhost";
        public const string Identifier = "identifier";
        public const string Number = "number";
        public const string Finished = "finished";
        public const string IsChunk = "IsChunk";
        public const int ChunkSize = 8192;

        //https://app.diagrams.net/#G1M9KILvIRLx3BPJbJX0Y0etegqL5WW3JY
    }

    public class Chunk
    {
        public string Identifier { get; set; }
        public int Number { get; set; }
        public byte[] Data { get; set; }
    }
}
