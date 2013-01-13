/**
 * crtorrent
 * 
 *  Bencode String Container
 * 
    crtorrent creates torrent metainfo files from directories and files.
    Copyright (C) 2011-2013  Thom Wiggers

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
using System.Text;
using System.Linq;
using System.Collections.Generic;
namespace Thom.Bencode
{
    internal class BencodeString : IBencodeItem
    {
        internal string Value
        {
            set;
            get;
        }
        internal byte[] ByteValue
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
        internal BencodeString(byte[] value)
        {
            this.ByteValue = value;
        }

        public override string ToString()
        {
            if (Value != null)
            {
                return String.Format("{0}:{1}", Encoding.UTF8.GetByteCount(Value), Value);
            }
            return "";
        }

        public byte[] ToBytes()
        {
            if (ByteValue == null)
            {
                string returnString = String.Format("{0}:{1}", Encoding.UTF8.GetByteCount(Value), Value);
                return UTF8Encoding.UTF8.GetBytes(returnString);
            }
            else
            {
                List<byte> blist = new List<byte>();
                blist.AddRange(Encoding.UTF8.GetBytes(String.Format("{0}:", ByteValue.Length)));
                blist.AddRange(ByteValue);
                return blist.ToArray();
            }
        }

    }
}
