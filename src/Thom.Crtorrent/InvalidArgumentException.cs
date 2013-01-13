//  This file is a part of Crtorrent. Crtorrent creates torrent metainfo files 
//  from directories and files.
//  InvalidArgumentException.cs
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

namespace Thom.Crtorrent
{
	/// <summary>
	/// Invalid argument exception.
	/// </summary>
	internal class InvalidArgumentException : Exception
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.InvalidArgumentException"/> class.
		/// </summary>
		internal InvalidArgumentException ()
            : base()
		{ 
            
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.InvalidArgumentException"/> class.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		internal InvalidArgumentException (string message)
            : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.InvalidArgumentException"/> class.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		/// <param name='innerException'>
		/// Inner exception.
		/// </param>
		internal InvalidArgumentException (string message, Exception innerException)
            : base(message, innerException)
		{
		}
	}
}

