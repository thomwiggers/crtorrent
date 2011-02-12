/**
 * crtorrent
 * 
 * Bencode dictionary
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
    public class BencodeDictionary : Dictionary<string,IBencodeItem>,IBencodeItem
    {
        BencodeDictionary(IDictionary<string,IBencodeItem> dictionary)
            : base(dictionary)
        {

        }
        BencodeDictionary(IDictionary<string, string> dictionary)
        {
            foreach (KeyValuePair<string, string> item in dictionary)
            {
                Add(item.Key, new BencodeString(item.Value));
            }
        }
        BencodeDictionary(IDictionary<string, int> dictionary)
        {
            foreach (KeyValuePair<string, int> item in dictionary)
            {
                Add(item.Key, new BencodeInt(item.Value));
            }
        }

        public override string ToString()
        {
            string returnString = "";
            if (this.Count > 0)
            {
                returnString = "d";
                foreach (KeyValuePair<string, IBencodeItem> item in this)
                {
                    returnString += (new BencodeString(item.Key)).ToString() + item.Value.ToString();
                }
                returnString += "e";
            }
            return returnString;
        }
        public void Add(string key, string value)
        {
            Add(key,new BencodeString(value);
        }
        public void Add(string key, int value)
        {
            Add(key, new BencodeInt(value));
        }
        public void Add(string key, string[] list)
        {
            Add(key, new BencodeList(list));
        }
        public void Add(string key, int[] list)
        {
            Add(key, new BencodeList(list));
        }
        public void Add(string key, IBencodeItem[] list)
        {
            Add(key, new BencodeList(list));
        }
    }
}
