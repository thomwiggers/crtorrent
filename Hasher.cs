/**
 * crtorrent
 * 
 * Hashing helper
 * 
    crtorrent creates torrent metainfo files from directories and files.
    Copyright (C) 2011  Thom Wiggers

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Security.Cryptography;

namespace crtorrent 
{
	class Hasher
	{
        private CancellationTokenSource cancelToken;
		
		//Number of threads
		private int numThreads;
		//piecelength
		private long pieceLength;
		
		// File paths
		private List<string> files = new List<string>();
		internal List<string> Files
		{
			get
			{
				return files;
			}
		}
        private ConcurrentDictionary<string,List<byte>> hashes = new ConcurrentDictionary<string,List<byte>>();

		///
		/// Chunks, in a list
		///
		private List<Chunk> Chunks = new List<Chunk>();
		
        ///
        /// Chunk count
        /// 
        internal int NumChunks
        {
            get
            {
                return Chunks.Count;
            }
        }


		///
		/// Initializing with certain values
		///
		internal Hasher(CancellationTokenSource cancellationTokenSource, int threads, double usePieceLength)
		{
            cancelToken = cancellationTokenSource;
			numThreads = threads;
            pieceLength = (int) usePieceLength;
		}
        internal Hasher(CancellationTokenSource cancellationTokenSource, int threads, int usePieceLength)
		{
            cancelToken = cancellationTokenSource;
			numThreads = threads;
            pieceLength = usePieceLength;
		}
		
		///
		/// Initialising with defaults
		///
		internal Hasher(CancellationTokenSource cancellationTokenSource)
		{
            cancelToken = cancellationTokenSource;
			numThreads = 1;
			pieceLength = (int)Math.Pow(2,18);
		}
		
		///
		/// Add a file (duh)
		///
		internal void AddFile(string path)
		{
			files.Add(path);
		}
		
		///
		/// Prepare for hashing
		///
		internal  void ChunkFiles()
		{
			string[] filenames = Files.ToArray();
			Chunk currentChunk = null;
			

            foreach (string filename in filenames)
            {
                cancelToken.Token.ThrowIfCancellationRequested();
                long offset = 0;

                FileInfo fi = new FileInfo(filename);
                if (!fi.Exists)
                    throw new FileNotFoundException(filename);

                Debug.WriteLine(String.Format("File: {0}", filename));
                
                //check if we don't already have a chunk
                if (currentChunk == null)
                {
                    currentChunk = new Chunk();
                    Chunks.Add(currentChunk);
                }

                long bytesLeft = fi.Length;
                while (bytesLeft > 0)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();
                    long needed = pieceLength - currentChunk.Length;
                    //check if chunk full.
                    if (needed == 0)
                    {
                        Debug.WriteLine("Adding new chunk, chunk full");
                        currentChunk = new Chunk();
                        Chunks.Add(currentChunk);
                        needed = pieceLength;
                    }

                    // add new chunk for this file.
                    ChunkSource cs = new ChunkSource()
                    {
                        Filename = filename,
                        Length = Math.Min((fi.Length - offset), needed),
                        StartPosition = offset
                    };
                    Debug.WriteLine(String.Format("ChunkSource created: \n    Fn: {0}\n    Lenght: {1}\n     Offset = {2}",
                                                    filename, Math.Min((fi.Length - offset), needed).ToString(), offset.ToString()));
                    offset += cs.Length;
                    bytesLeft -= cs.Length;
                    currentChunk.Sources.Add(cs);

                }
                        
            }

		}
		
		///
		/// Calculate Hashes
		///
        internal void ComputeHashes()
        {
            if (Chunks == null || Chunks.Count == 0)
                return;

            Dictionary<string, MemoryMappedFile> files = new Dictionary<string, MemoryMappedFile>();
            Parallel.ForEach(Chunks, new ParallelOptions() { CancellationToken = cancelToken.Token, MaxDegreeOfParallelism = numThreads }, (chunk, state, index) =>
            {
                MemoryMappedFile mms = null;
                byte[] buffer = new byte[(int)pieceLength];
                if (chunk.Length < pieceLength)
                    buffer = new byte[(int)chunk.Length];
                int offset = 0;

                foreach (var source in chunk.Sources)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();
                    lock (files)
                    {
                        if (!files.TryGetValue(source.Filename, out mms))
                        {
                            Debug.WriteLine(String.Format("Opening {0}", source.Filename));
                            files.Add(source.Filename, mms = MemoryMappedFile.CreateFromFile(source.Filename, FileMode.Open));
                        }
                    }

                    MemoryMappedViewStream view = mms.CreateViewStream(source.StartPosition, source.Length, MemoryMappedFileAccess.Read);
                    view.Read(buffer, offset, (int)source.Length);
                    offset += (int)source.Length;
                }

                Debug.WriteLine("Done reading sources");
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {

                    sha1.Initialize();
                    chunk.Hash = sha1.ComputeHash(buffer);
                }

                Debug.WriteLine(String.Format("Computed hash: {0}", String.Join("-", chunk.Hash.Select(h => h.ToString("X2")).ToArray())));
            });

            Parallel.ForEach(files.Values, (x) =>
            {
                x.Dispose();
            });
        }


		///
		/// Get all hashes as byte[] 
		///
		internal byte[] GetHashes()
		{
			if (Chunks == null || Chunks.Count == 0)
				return null;
			
			long hashLength = Chunks.Count * pieceLength;
			
			List<byte> hash = new List<byte>();
			
			foreach (Chunk chunk in Chunks)
			{
				hash.AddRange(chunk.Hash);
			}
			
			return hash.ToArray();
		}
	}
}