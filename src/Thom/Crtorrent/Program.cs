/**
 * crtorrent's central class
 * 
 * Entry point of executable
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
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

namespace Thom.Crtorrent
{
    class Program
    {
        private static CancellationTokenSource cancelToken = new CancellationTokenSource();
        public const string appName = "crtorrent";
        public const string version = "0.2 delta";
        public const string fullVersionInformation = "beta build, Second release.";
        const string HELP = "help";
        const string COPYRIGHT = "copyright";
        const string VERSION = "version";
        const string INTRO = "intro";
        protected internal static string comment = "Created with crtorrent " + version;
        // -1 means let C# decide.
        protected internal static int numThreads = -1;
        protected internal static int NumThreads
        {
            get { return numThreads; }
            set { if (value > 0) { numThreads = value; } }
        }

        protected internal static bool privateFlag = false;
        protected internal static bool verboseFlag = false;
        protected internal static bool dateFlag = true;
        protected internal static string name = string.Empty;
        protected internal static HashSet<string> announceUrls = new HashSet<string>();
        private static string AnnounceUrl
        {
            set {
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute)) 
                { 
                    announceUrls.Add(value);
                }
                else if (value.Contains(","))
                {
                    foreach(string url in value.Split(','))
                    {
                        AnnounceUrl = url;
                    }
                }
            }
        }
        protected internal static string path = "";
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

        public static void Main(string[] args)
        {
            try
            {
                Console.TreatControlCAsInput = false;
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

                PrintSection(INTRO);
                args = Environment.GetCommandLineArgs();

                for (int i = 0; i < args.Length; i++)
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
                                    PieceLength = MakeDouble(args[i + 1], "piece-length");
                                    break;
                                case "outfile":
                                case "o":
                                    outputFile = args[i + 1];
                                    break;
                                case "p":
                                case "private":
                                    privateFlag = true;
                                    break;
                                case "v":
                                case "verbose":
                                    verboseFlag = true;
                                    break;
                                case "no-date":
                                case "d":
                                    dateFlag = false;
                                    break;
                                case "name":
                                case "n":
                                    name = args[i + 1];
                                    break;
                            }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw new FatalException("Error: Argument \"" + args[i] + "\" needs a value", e);
                    }
                    catch (InvalidArgumentException e)
                    {
                        throw new FatalException(e);
                    }

                }

                if (args.Length < 2)
                {
                     throw new FatalException("Not enough parameters");
                }



                //check if before last parameter doesn't take a value
                string beforeLastArg = args[args.Length - 2].TrimStart('-');
                switch (beforeLastArg)
                {

                    case "c":
                    case "comment":
                    case "threads":
                    case "t":
                    case "announce":
                    case "a":
                    case "piece-length":
                    case "l":
                    case "outfile":
                    case "o":
                    case "name":
                    case "n":
                        throw new FatalException("Error: No target specified");
                }

                path = args[args.Length - 1];
                           
                //validatie:
                if (announceUrls.Count < 1)
                {
                    throw new FatalException("No announce url specified");
                }
                if (outputFile == null)
                    outputFile = String.Format("{0}.torrent", Path.GetFileNameWithoutExtension(path));

                if (File.Exists(outputFile))
                    throw new FatalException(String.Format("{0} already exists", outputFile));
                if (verboseFlag)
                {
                    Console.WriteLine("################# DETAILS ########################");
                    Console.WriteLine("comment:       {0}", comment);
                    Console.WriteLine("Piece Lenghth  {0}", PieceLength.ToString());
                    Console.WriteLine("NumThreads:    {0}", numThreads);
                    Console.WriteLine("Output file:   {0}", outputFile);
                    Console.WriteLine("Private:       {0}", privateFlag);
                    Console.WriteLine("Path:          {0}", path);
                    Console.WriteLine("Announce urls: ");
                    foreach (string url in announceUrls)
                    {
                        Console.WriteLine("  Url:         {0}", url);
                    }
                }



                //beginnen maar
                Metafile metafile = new Metafile(path, announceUrls.ToArray(), privateFlag, dateFlag, comment, outputFile, numThreads, Math.Pow(2,18), appName + " " + version, cancelToken);

                File.WriteAllBytes(outputFile, metafile.metafile.ToBytes());
                

                
                

            }
            catch (FatalException e)
            {
                Console.WriteLine("A fatal error occurred, and I could not continue: \r\n");
                Console.WriteLine(e.Message);
                if (e.InnerException != null && verboseFlag)
                {
                    Console.WriteLine("@@@Inner Exception: {0}", e.Message); 
                }
                if (verboseFlag)
                {
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine("\r\n Type {0} --help for help about this program", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
            }
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Intercepted CancelKeyPress");
            Console.WriteLine("Aborting...");
            e.Cancel = true;
            cancelToken.Cancel();
            throw new NotImplementedException();
        }


        static double MakeDouble(string value, string parameter = "")
        {
            double result = 0;
            if(!double.TryParse(value, out result)){
                throw new InvalidArgumentException("ERORR: Parameter \"" + parameter +"\" needs to be a number");
            }
            return result;

        }
        static int MakeInt(string value, string parameter = "")
        {
            int result = 0;
            if (!int.TryParse(value, out result))
            {
                throw new InvalidArgumentException("ERORR: Parameter \"" + parameter + "\" needs to be a number");
            }
            return result;

        }

        static void PrintSection(string section)
        {
            switch (section)
            {
                case HELP:
                    Console.WriteLine(@"
--help (-h)                    Displays this help message.
--version                      Displays version information.
--copyright                    Displays copyright and warranty information.
--threads (-t) <value>         Sets the number of threads used.
                                    (Defaults to 'let system decide')
--comment (-c) <comment>       Sets the comment in the metafile.
--piece-length (-l) <length>   Sets the piecelength to 2^<length>.
--announce (-a) <url>[,<url>]  Sets the announce URL. Use more than one parameter
                               to specify an announce list. Erronenous URLs are 
                               omitted without warning.
--private (-p)                 Sets the private flag.
--outfile (-o) <filename>      Sets the name of the output file.
                                    (Defaults to input <file/foldername>.torrent)
--verbose                      Prints extra information about what is going on.

");
                    break;
                case COPYRIGHT:
                    Console.WriteLine(@"
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
along with this program.  If not, see <http://www.gnu.org/licenses/>.");
                    break;
                case VERSION:
                    Console.WriteLine("This is crtorrent version {0} © {1} Thom Wiggers",version, DateTime.UtcNow.Year);
                    Console.WriteLine(fullVersionInformation);
                    Console.WriteLine("This software is available under the GPL Public License");
                    break;
                case INTRO:
                    Console.WriteLine(@"
        crtorrent  Copyright (C) 2011-2013  Thom Wiggers
This program comes with ABSOLUTELY NO WARRANTY; for details type `-copyright'.
This is free software, and you are welcome to redistribute it
under certain conditions; type `-copyright' for details.

Version number: " + version + "\r\n\r\n");
                    break;

            }

        }
    }
}
