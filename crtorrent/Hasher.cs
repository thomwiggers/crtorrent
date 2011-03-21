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

namespace crtorrent 
{
	class Hasher
	{
        private int bufferSize = 16777216;
        private CancellationTokenSource cancelToken;
		
		//Number of threads
		private int numThreads;
		//piecelength
		private int pieceLength;
		
		// File paths
		private List<string> files = new List<string>();
		public List<string> Files
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
		/// Initializing with certain values
		///
		public Hasher(CancellationTokenSource cancellationTokenSource, int threads, double usePieceLenght)
		{
            cancelToken = cancellationTokenSource;
			numThreads = threads;
			pieceLength = (int)usePiecelength;
		}
		public Hasher(CancellationTokenSource cancellationTokenSource, int threads, int usePieceLenght)
		{
            cancelToken = cancellationTokenSource;
			numThreads = threads;
			pieceLength = usePiecelength;
		}
		
		///
		/// Initialising with defaults
		///
		public Hasher(CancellationTokenSource cancellationTokenSource)
		{
            cancelToken = cancellationTokenSource;
			numThreads = 1;
			pieceLength = Math.pow(2,18);
		}
		
		///
		/// Add a file (duh)
		///
		public void AddFile(string path)
		{
			files.Add(path);
		}
		
		///
		/// Prepare for hashing
		///
		private void ChunkFiles(string[] filenames)
		{
			string[] filenames = Files.ToArray();
			Chunk currentChunk = null;
			int offset = 0;

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
					int needed = pieceLength - currentChunk.Length;
					if (needed == 0)
						throw new InvalidOperationException("Something went wonky, shouldn't be here");

					offset = needed;
					currentChunk.Sources.Add(new ChunkSource()
					{
						Filename = fi.FullName,
						StartPosition = 0,
						Length = (int)Math.Min(fi.Length, (long)needed)
					});

					if (currentChunk.Length >= pieceLength)
					{
						chunks.Add(currentChunk = new Chunk());
					}
				}

				//
				// Note: Using integer division here
				//
				for (int i = 0; i < (fi.Length - offset) / pieceLength; i++)
				{
					chunks.Add(currentChunk = new Chunk());
					currentChunk.Sources.Add(new ChunkSource()
					{
						Filename = fi.FullName,
						StartPosition = i * pieceLength + offset,
						Length = pieceLength
					});

					Debug.WriteLine(String.Format("Chunk source created: Offset = {0,10}, Length = {1,10}", currentChunk.Sources[0].StartPosition, currentChunk.Sources[0].Length));
				}

				int leftover = (int)(fi.Length - offset) % pieceLength;
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
					offset = 0;
				}

			}
		}
		
		///
		/// Calculate Hashes
		///
	    private void ComputeHashes()
		{
			if (chunks == null || chunks.Count == 0)
				return;

			Dictionary<string, MemoryMappedFile> files = new Dictionary<string, MemoryMappedFile>();

			foreach (var chunk in chunks)
			{
				MemoryMappedFile mms = null;
				byte[] buffer = new byte[pieceLength];
				ConcurrentDictionary<string, MemoryMappedFile> files = new ConcurrentDictionary<string, MemoryMappedFile>();
				Parallel.ForEach(chunks, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (chunk, state, index) =>
				{
					MemoryMappedFile mms = null;
					byte[] buffer = new byte[pieceLength];

					foreach (var source in chunk.Sources)
					{
						if (!files.TryGetValue(source.Filename, out mms)) {
							{
								Debug.WriteLine(String.Format("Opening {0}", source.Filename));
								files.Add(source.Filename, mms = MemoryMappedFile.CreateFromFile(source.Filename, FileMode.Open));
							}
						}

						var view = mms.CreateViewStream(source.StartPosition, source.Length);
						view.Read(buffer, 0, source.Length);
					}

					Debug.WriteLine("Done reading sources");
					using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
					{
						sha1.Initialize();
						chunk.Hash = sha1.ComputeHash(buffer);
					}
					Debug.WriteLine(String.Format("Computed hash: {0}", String.Join("-", chunk.Hash.Select(h => h.ToString("X2")).ToArray())));
				});
			}

			foreach (var f in files.Values)
			{
				f.Dispose();
			}
		}

		///
		/// Get all hashes as byte[] 
		///
		public byte[] GetHashes()
		{
			if (chunks == null || chunks.Count == 0)
				return;
			
			long hashLength = chunks.Count * pieceLength;
			
			List<byte> hash = new List<byte>();
			
			foreach (Chunk chunk in chunks)
			{
				hash.AddRange(chunk.Hash);
			}
			
			return hash.ToArray();
		}
	}
}