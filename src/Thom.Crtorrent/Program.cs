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
		/// <summary>
		/// The cancel token.
		/// </summary>
		private static CancellationTokenSource cancelToken = new CancellationTokenSource();
        
		/// <summary>
		/// The name of the application
		/// </summary>
		public const string APP_NAME = "crtorrent";

		/// <summary>
		/// The App version
		/// </summary>
        public const string APP_VERSION = "2.0 development";

		/// <summary>
		/// The Full Version info
		/// </summary>
        public const string FULL_VERSION_INFORMATION = "Refactored/rewritten release.";

		/// <summary>
		/// The Help command
		/// </summary>
        const string HELP = "help";

		/// <summary>
		/// The copyright command
		/// </summary>
        const string COPYRIGHT = "copyright";

		/// <summary>
		/// The version command.
		/// </summary>
        const string VERSION = "version";

		/// <summary>
		/// The intro command
		/// </summary>
        const string INTRO = "intro";

		/// <summary>
		/// The comment used in the torrent file.
		/// </summary>
        protected internal static string comment = "Created with crtorrent " + APP_VERSION;
        
		/// <summary>
		/// The number of threads.
		/// </summary>
		/// -1 means let C# decide.
        protected internal static int numThreads = -1;
        
		/// <summary>
		/// Gets or sets the number of threads.
		/// </summary>
		/// <value>
		/// The number of threads.
		/// </value>
		protected internal static int NumThreads
        {
            get { return numThreads; }
            set { if (value > 0) { numThreads = value; } }
        }

		/// <summary>
		/// The private flag.
		/// </summary>
        protected internal static bool PrivateFlag = false;

		/// <summary>
		/// The verbose output flag.
		/// </summary>
        protected internal static bool VerboseFlag = false;

		/// <summary>
		/// The date flag.
		/// </summary>
        protected internal static bool DateFlag = true;

		/// <summary>
		/// The name.
		/// </summary>
		protected internal static string Name = string.Empty;

		/// <summary>
		/// The announce urls.
		/// </summary>
        protected internal static HashSet<string> AnnounceUrls = new HashSet<string>();

		/// <summary>
		/// Sets the announce URL.
		/// </summary>
		/// <value>
		/// The announce URL.
		/// </value>
        private static string AnnounceUrl
        {
            set {
                if (Uri.IsWellFormedUriString(value, UriKind.Absolute)) 
                { 
                    AnnounceUrls.Add(value);
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

		/// <summary>
		/// The path.
		/// </summary>
        private static string path = "";
        
		/// <summary>
		/// The length of the piece.
		/// </summary>
		private static double pieceLength = Math.Pow(2,18);

		/// <summary>
		/// Gets or sets the length of the piece.
		/// </summary>
		/// <value>
		/// The length of the piece.
		/// </value>
        protected static double PieceLength
        {
            get { return pieceLength; }
            set
            {
                pieceLength = Math.Pow(2, value < 18 && value >30 ? 18 : value);
            }
        }

		/// <summary>
		/// The output file.
		/// </summary>
        private static string outputFile = null;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
        public static void Main(string[] args)
        {
            try
            {
                Console.TreatControlCAsInput = false;
                Console.CancelKeyPress += new ConsoleCancelEventHandler(ConsoleCancelKeyPress);

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
                                    PrivateFlag = true;
                                    break;
                                case "v":
                                case "verbose":
                                    VerboseFlag = true;
                                    break;
                                case "no-date":
                                case "d":
                                    DateFlag = false;
                                    break;
                                case "name":
                                case "n":
                                    Name = args[i + 1];
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
                if (AnnounceUrls.Count < 1)
                {
                    throw new FatalException("No announce url specified");
                }
                if (outputFile == null)
                    outputFile = String.Format("{0}.torrent", Path.GetFileNameWithoutExtension(path));

                if (File.Exists(outputFile))
                    throw new FatalException(String.Format("{0} already exists", outputFile));
                if (VerboseFlag)
                {
                    Console.WriteLine("################# DETAILS ########################");
                    Console.WriteLine("comment:       {0}", comment);
                    Console.WriteLine("Piece Lenghth  {0}", PieceLength.ToString());
                    Console.WriteLine("NumThreads:    {0}", numThreads);
                    Console.WriteLine("Output file:   {0}", outputFile);
                    Console.WriteLine("Private:       {0}", PrivateFlag);
                    Console.WriteLine("Path:          {0}", path);
                    Console.WriteLine("Announce urls: ");
                    foreach (string url in AnnounceUrls)
                    {
                        Console.WriteLine("  Url:         {0}", url);
                    }
                }



                //beginnen maar
                Metafile metafile = new Metafile(path, AnnounceUrls.ToArray(), PrivateFlag, DateFlag, comment, outputFile, numThreads, Math.Pow(2,18), APP_NAME + " " + APP_VERSION, cancelToken);

                File.WriteAllBytes(outputFile, metafile.metafile.ToBytes());
                

                
                

            }
            catch (FatalException e)
            {
                Console.WriteLine("A fatal error occurred, and I could not continue: \r\n");
                Console.WriteLine(e.Message);
                if (e.InnerException != null && VerboseFlag)
                {
                    Console.WriteLine("@@@Inner Exception: {0}", e.Message); 
                }
                if (VerboseFlag)
                {
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine("\r\n Type {0} --help for help about this program", Path.GetFileName(Environment.GetCommandLineArgs()[0]));
            }
        }

		/// <summary>
		/// Handles the cancel event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
        static void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Intercepted CancelKeyPress");
            Console.WriteLine("Aborting...");
            e.Cancel = true;
            cancelToken.Cancel();
            throw new NotImplementedException();
        }

		/// <summary>
		/// Makes a double from a string
		/// </summary>
		/// <returns>
		/// The double.
		/// </returns>
		/// <param name='value'>
		/// Value.
		/// </param>
		/// <param name='parameter'>
		/// Parameter.
		/// </param>
        static double MakeDouble(string value, string parameter = "")
        {
            double result = 0;
            if(!double.TryParse(value, out result)){
                throw new InvalidArgumentException("ERROR: Parameter \"" + parameter +"\" needs to be a number");
            }
            return result;

        }

		/// <summary>
		/// Make an int from a string
		/// </summary>
		/// <returns>
		/// The int.
		/// </returns>
		/// <param name='value'>
		/// Value.
		/// </param>
		/// <param name='parameter'>
		/// Parameter.
		/// </param>
        static int MakeInt(string value, string parameter = "")
        {
            int result = 0;
            if (!int.TryParse(value, out result))
            {
                throw new InvalidArgumentException("ERROR: Parameter \"" + parameter + "\" needs to be a number");
            }
            return result;

        }

		/// <summary>
		/// Prints sections section.
		/// </summary>
		/// <param name='section'>
		/// Section.
		/// </param>
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
any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.");
                    break;
                case VERSION:
                    Console.WriteLine("This is crtorrent version {0} © {1} Thom Wiggers", APP_VERSION, DateTime.UtcNow.Year);
                    Console.WriteLine(FULL_VERSION_INFORMATION);
                    Console.WriteLine("This software is available under the GPL Public License version 3");
                    break;
                case INTRO:
                    Console.WriteLine(@"
        crtorrent  Copyright (C) 2011-2013  Thom Wiggers
This program comes with ABSOLUTELY NO WARRANTY; for details type `-copyright'.
This is free software, and you are welcome to redistribute it
under certain conditions; type `-copyright' for details.

Version number: " + APP_VERSION + "\r\n\r\n");
                    break;

            }

        }
    }
}
