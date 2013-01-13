//  This file is a part of Crtorrent. Crtorrent creates torrent metainfo files 
//  from directories and files.
//
//  FatalException.cs
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
	/// Fatal exception.
	/// </summary>
    internal class FatalException : Exception
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.FatalException"/> class.
		/// </summary>
        internal FatalException() 
            : base("ERROR: FATAL ERROR")
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.FatalException"/> class.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
        internal FatalException(String message)
            : base(message)
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.FatalException"/> class.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		/// <param name='innerException'>
		/// Inner exception.
		/// </param>
        internal FatalException(String message, Exception innerException)
            : base(message, innerException)
        {

        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.FatalException"/> class.
		/// </summary>
		/// <param name='innerException'>
		/// Inner exception.
		/// </param>
        internal FatalException(Exception innerException)
            : base(innerException.Message, innerException)
        {

        }
    }
}
