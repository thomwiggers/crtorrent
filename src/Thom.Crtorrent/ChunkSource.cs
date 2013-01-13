//  This file is a part of Crtorrent. Crtorrent creates torrent metainfo files 
//  from directories and files.
//  ChunkSource.cs
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

namespace Thom.Crtorrent
{

	/// <summary>
	/// Chunk source.
	/// </summary>
    internal class ChunkSource
    {
		/// <summary>
		/// Gets or sets the filename.
		/// </summary>
		/// <value>
		/// The filename.
		/// </value>
        internal string Filename { get; set; }

		/// <summary>
		/// Gets or sets the start position.
		/// </summary>
		/// <value>
		/// The start position.
		/// </value>
        internal long StartPosition { get; set; }

		/// <summary>
		/// Gets or sets the length.
		/// </summary>
		/// <value>
		/// The length.
		/// </value>
        internal long Length { get; set; }
    }
}
