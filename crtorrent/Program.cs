/**
 * crtorrent's central class
 * 
 * Entry point of executable
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
namespace crtorrent
{
    class Program
    {
        const string version = "0.1a";
        const string HELP = "help";
        const string COPYRIGHT = "copyright";
        const string VERSION = "version";
        const string INTRO = "intro";
        static string comment = "Created with crtorrent " + version;
        protected internal static int numThreads = 1;
        protected internal static bool privateFlag = false;
        private static int NumThreads
        {
            get { return numThreads; }
            set { if (value > 0) { numThreads = value; } }
        }
        protected internal static List<string> announceUrl = new List<string>();
        private static string AnnounceUrl
        {
            set { 
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute)) 
                { 
                    announceUrl.Add(value); 
                }
            }
        }
        protected internal static double pieceLength = Math.Pow(2,18);
        private static double PieceLength
        {
            get { return pieceLength; }
            set
            {
                pieceLength = Math.Pow(2, value < 18 && value >30 ? 18 : value);
            }
        }

        private static string outputFile = null;

        static void Main(string[] args)
        {
            PrintSection(INTRO);
            args = Environment.GetCommandLineArgs();
            
            for (int i =0; i < args.Length; i++)
            {
                try
                {
                    if (args[i].StartsWith("-"))
                    {
                        string arg = args[i].TrimStart('-');
                        switch (arg)
                        {
                            case "help":
                            case "h":
                                PrintSection(HELP);
                                break;
                            case "version":
                                PrintSection(VERSION);
                                break;
                            case "c":
                            case "comment":
                                comment = args[i + 1];
                                break;
                            case "threads":
                            case "t":
                                NumThreads = MakeInt(args[i + 1], "threads");
                                break;
                            case "copyright":
                                PrintSection(COPYRIGHT);
                                break;
                            case "announce":
                            case "a":
                                AnnounceUrl = args[i + 1];
                                break;
                            case "piece-length":
                            case "l":
                                PieceLength = MakeDouble(args[i + 1], "threads");
                                break;
                            case "outfile":
                            case "o":
                                outputFile = args[i + 1];
                                break;
                            case "p":
                            case "private":
                                privateFlag = true;
                                break;
                        }
                    }
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new FatalException("Error: Argument \"" + args[i] + "\" needs a value", e);
                }
                catch (InvalidArgumentException e){
                    throw new FatalException(e);
                }

            }

            Console.WriteLine("#################DEBUG########################");
            Console.WriteLine("comment: " + comment);
            Console.WriteLine("Piece Lenghth " + PieceLength.ToString());
            Console.WriteLine("NumThreads:   " + numThreads);
            Console.WriteLine("Announce urls: ");
            foreach (string url in announceUrl.ToArray())
            {
                Console.WriteLine("Url:    " + url);
            }
        }


        static double MakeDouble(string value, string parameter = "")
        {
            double result = 0;
            if(!double.TryParse(value, out result)){
                throw new InvalidArgumentException("ERORR: Parameter \"" + parameter +"\" needs to be an number");
            }
            return result;

        }
        static int MakeInt(string value, string parameter = "")
        {
            int result = 0;
            if (!int.TryParse(value, out result))
            {
                Console.WriteLine("ERORR: Parameter \"" + parameter +"\" needs to be an integer");
            }
            return result;

        }

        static void PrintSection(string section)
        {
            switch (section)
            {
                case HELP:
                    Console.WriteLine("Use -h --help or -help for help");
                    break;
                case COPYRIGHT:
                    Console.WriteLine(@"
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
along with this program.  If not, see <http://www.gnu.org/licenses/>.");
                    break;
                case VERSION:
                    Console.WriteLine("This is crtorrent version " + version + " copyright Thom Wiggers");
                    Console.WriteLine("This software is available under the GPL Public License");
                    break;
                case INTRO:
                    Console.WriteLine(@"
        crtorrent  Copyright (C) 2011  Thom Wiggers
This program comes with ABSOLUTELY NO WARRANTY; for details type `-copyright'.
This is free software, and you are welcome to redistribute it
under certain conditions; type `-copyright' for details.

Version number: " + version + "\r\n\r\n");
                    break;

            }

        }
    }
}
