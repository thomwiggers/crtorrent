using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
/// <summary>
/// NON THREADED
/// </summary>
/// 


public static class Constants
{
    public const long CHUNK_SIZE_IN_BYTES = 524288;       //256kiB
}

public class ChunkSource
{
    public string Filename { get; set; }
    public long StartPosition { get; set; }
    public long Length { get; set; }
}

public class Chunk
{
    private List<ChunkSource> _sources = new List<ChunkSource>();

    public IList<ChunkSource> Sources { get { return _sources; } }
    public byte[] Hash { get; set; }
    public long Length
    {
        get { return Sources.Select(s => s.Length).Sum(); }
    }
}

static class Program
{
    static void Main()
    {
        Debug.AutoFlush = true;
        DirectoryInfo di = new DirectoryInfo(@"Z:\ISOs\Adobe CS5 Master");
        Stopwatch sw = new Stopwatch();
        sw.Start();
        string[] filenames = di.GetFiles("*", SearchOption.AllDirectories).Select(fi => fi.FullName).OrderBy(n => n).ToArray();
        var chunks = ChunkFiles(filenames);
        ComputeHashes(chunks);
        sw.Stop();
        Debug.WriteLine("");
        Debug.WriteLine("TOTAL EXECUTION TIME: {0}", sw.Elapsed.TotalSeconds);
    }

    private static List<Chunk> ChunkFiles(string[] filenames)
    {
        List<Chunk> chunks = new List<Chunk>();
        Chunk currentChunk = null;
        long offset = 0;

        foreach (string filename in filenames)
        {
            FileInfo fi = new FileInfo(filename);
            if (!fi.Exists)
                throw new FileNotFoundException(filename);

            Debug.WriteLine(String.Format("File: {0}", filename));
            //
            // First, start off by either starting a new chunk or 
            // by finishing a leftover chunk from a previous file.
            //
            if (currentChunk != null)
            {
                //
                // We get here if the previous file had leftover bytes that left us with an incomplete chunk
                //
                long needed = Constants.CHUNK_SIZE_IN_BYTES - currentChunk.Length;
                if (needed == 0)
                    throw new InvalidOperationException("Something went wonky, shouldn't be here");

                offset = needed;
                currentChunk.Sources.Add(new ChunkSource()
                {
                    Filename = fi.FullName,
                    StartPosition = 0,
                    Length = Math.Min(fi.Length, (long)needed)
                });

                if (currentChunk.Length >= Constants.CHUNK_SIZE_IN_BYTES)
                {
                    chunks.Add(currentChunk = new Chunk());
                }
            }
            else
            {
                offset = 0;
            }

            //
            // Note: Using integer division here
            //
            for (int i = 0; i < (fi.Length - offset) / Constants.CHUNK_SIZE_IN_BYTES; i++)
            {
                chunks.Add(currentChunk = new Chunk());
                currentChunk.Sources.Add(new ChunkSource()
                {
                    Filename = fi.FullName,
                    StartPosition = i * Constants.CHUNK_SIZE_IN_BYTES + offset,
                    Length = Constants.CHUNK_SIZE_IN_BYTES
                });

                Debug.WriteLine(String.Format("Chunk source created: Offset = {0,11}, Length = {1,11}", currentChunk.Sources[0].StartPosition, currentChunk.Sources[0].Length));
            }

            long leftover = (fi.Length - offset) % Constants.CHUNK_SIZE_IN_BYTES;
            if (leftover > 0)
            {
                chunks.Add(currentChunk = new Chunk());
                currentChunk.Sources.Add(new ChunkSource()
                {
                    Filename = fi.FullName,
                    StartPosition = (int)(fi.Length - leftover),
                    Length = leftover
                });
            }
            else
            {
                currentChunk = null;
            }

        }

        return chunks;
    }

    private static void ComputeHashes(IList<Chunk> chunks)
    {
        if (chunks == null || chunks.Count == 0)
            return;

        Dictionary<string, MemoryMappedFile> files = new Dictionary<string, MemoryMappedFile>();
        List<Task> tasks = new List<Task>();


        foreach (var chunk in chunks)
        {
            MemoryMappedFile mms = null;
            byte[] buffer = new byte[Constants.CHUNK_SIZE_IN_BYTES];

            Stopwatch sw = Stopwatch.StartNew();
            foreach (var source in chunk.Sources)
            {
                lock (files)
                {
                    if (!files.TryGetValue(source.Filename, out mms))
                    {
                        Debug.WriteLine(String.Format("Opening {0}", source.Filename));
                        files.Add(source.Filename, mms = MemoryMappedFile.CreateFromFile(source.Filename, FileMode.Open));
                    }
                }

                var view = mms.CreateViewStream(source.StartPosition, source.Length);
                view.Read(buffer, 0, (int)source.Length);
            }
            sw.Stop();
            Debug.WriteLine("Done reading sources in {0}ms", sw.Elapsed.TotalMilliseconds);
            HashParameters h = new HashParameters() { Chunk = chunk, Buffer = buffer };
            tasks.Add(Task.Factory.StartNew(HashPiece, h));
        }

        foreach (var x in files.Values)
        {
            x.Dispose();
        }
    }


    private static void HashPiece(object parameters)
    {
        HashParameters h = (HashParameters)parameters;
        MD5 md5 = MD5CryptoServiceProvider.Create();
        md5.Initialize();
        h.Chunk.Hash = md5.ComputeHash(h.Buffer);
        Debug.WriteLine(String.Format("Computed hash: {0}", BitConverter.ToString(h.Chunk.Hash)));
        h.Buffer = new byte[1];
    }

}
class HashParameters
{
    public Chunk Chunk { get; set; }
    public byte[] Buffer { get; set; }
}

