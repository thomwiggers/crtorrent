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
		
		// Files with information
		// Layout:
		//    num -> (path->hash)
		private List<string> files = new List<string>();
		public List<string> Files
		{
			get
			{
				return files;
			}
		}
        private ConcurrentDictionary<string,List<byte>> hashes = new ConcurrentDictionary<string,List<byte>>();
        public ConcurrentDictionary<string,List<byte>> Hashes
        {
            get
            {
                return hashes;
            }
        }
        private ConcurrentDictionary<string,List<string>> md5Sums = new ConcurrentDictionary<string,List<byte>>();
        public ConcurrentDictionary<string,List<string>> Md5Sums
        {
            get
            {
                return md5Sums;
            }
        }
		
		// Initializing with certain values
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
		
		//initialising with defaults
		public Hasher(CancellationTokenSource cancellationTokenSource)
		{
            cancelToken = cancellationTokenSource;
			numThreads = 1;
			pieceLength = Math.pow(2,18);
		}
		
		//Add a file (duh)
		public void AddFile(string path)
		{
			files.Add(path);
		}
		
		
		
	}
}