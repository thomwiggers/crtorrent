//  This file is a part of Crtorrent. Crtorrent creates torrent metainfo files 
//  from directories and files.
//  Chunk.cs
//
//  Author:
//       Thom Wiggers <thom@thomwiggers.nl>
//
//  Copyright (c) 2013 Thom Wiggers
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.Linq;

namespace Thom.Crtorrent
{
    
	/// <summary>
	/// Chunk that needs to be hashed.
	/// </summary>
    internal class Chunk
    {
		/// <summary>
		/// The sources from which to obtain this chunk.
		/// </summary>
        private List<ChunkSource> _sources = new List<ChunkSource>();

		/// <summary>
		/// Gets the sources from which to obtain this chunk.
		/// </summary>
		/// <value>
		/// The sources.
		/// </value>
        internal IList<ChunkSource> Sources { get { return _sources; } }

		/// <summary>
		/// Gets or sets a value indicating whether this instance hash.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance hash; otherwise, <c>false</c>.
		/// </value>
        internal byte[] Hash { get; set; }
        
		/// <summary>
		/// Gets the total length of the sources.
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
		internal long Length
        {
            get { return Sources.Select(s => s.Length).Sum(); }
        }
    }
}
