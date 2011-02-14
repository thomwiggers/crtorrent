/**
 * crtorrent
 * 
 *  Bencode List container
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

using System.Collections.Generic;
namespace crtorrent.Bencode
{

    class BencodeList : List<IBencodeItem>,IBencodeItem
    {
        public BencodeList()
        {

        }
        public BencodeList(params int[] list)
        {
            foreach (int item in list)
            {
                Add(item); 
            }
        }
        public BencodeList(params string[] list)
        {
            foreach (string item in list)
            {
                Add(item);
            }
        }
        public BencodeList(IBencodeItem[] list)
        {
            foreach (IBencodeItem item in list)
            {
                Add(item);
            }
        }
        public void Add(string item)
        {
            Add(new BencodeString(item));
        }
        public void Add(int item)
        {
            Add(new BencodeInt(item));
        }
        public override string ToString()
        {
            string returnString = "";
            if (this.Count > 0)
            {
                returnString = "l";
                foreach (IBencodeItem item in this)
                {
                    returnString += item.ToString();
                }
                returnString += "e";
            }
            return returnString;
        }
    }
}
