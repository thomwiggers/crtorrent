/**
 * crtorrent
 * 
 *  Bencode tester
 * 
 *  Build using: csc BencodeTest.cs /recurse:*.cs
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
namespace crtorrent.Bencode
{
    class BencodeTest
    {
        static void Main(string[] args)
        {
            BencodeInt testI = new BencodeInt(199999);
            Console.WriteLine("Testing BencodeInt(199999).ToString(): \n {0}", testI);
            Console.WriteLine("");

            BencodeString testS = new BencodeString("hello world");
            Console.WriteLine("Testing BencodeString(\"hello world\").ToString(): \n{0}", testS);
            Console.WriteLine("");

            BencodeList testListI = new BencodeList(10, 20, 30, 40, 50);
            Console.WriteLine("Testing BencodeList(10, 20, 30, 40, 50).ToString(): \n{0}", testListI);
            Console.WriteLine("");

            BencodeList testListS = new BencodeList("hello", "world", " \npornobob");
            Console.WriteLine("Testing BencodeList(\"hello\", \"world\", \" pornobob\").ToString(): \n{0}", testListS);
            Console.WriteLine("");

            BencodeList testListIface = new BencodeList(new BencodeInt(10), new BencodeString("hello"));
            Console.WriteLine("Testing BencodeList(new BencodeInt(10), new BencodeString(10)).ToString(): \n{0}", testListIface);
            Console.WriteLine("");

            BencodeDictionary testDict = new BencodeDictionary();
            testDict.Add("test", 10);
            testDict.Add("info", "sterf");
            testDict.Add("dodo", new BencodeList(10, 11, 12));
            testDict.Add("dada", 10, 20, 30);
            Console.WriteLine("Testing Dictionary: \n{0}", testDict);

            Console.ReadLine();
        }
    }
}
