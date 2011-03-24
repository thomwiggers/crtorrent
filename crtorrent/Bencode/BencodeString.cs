/**
 * crtorrent
 * 
 *  Bencode String Container
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
namespace crtorrent.Bencode
{
    internal class BencodeString : IBencodeItem
    {
        internal string Value
        {
            set;
            get;
        }

        internal BencodeString()
        {

        }
        internal BencodeString(string value)
        {
            this.Value = value;
        }

        internal override string ToString()
        {
            if (Value != null)
            {
                return Value.Length + ":" + Value;
            }
            return "";
        }


    }
}
