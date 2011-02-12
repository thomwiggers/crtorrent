using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace crtorrent
{

    class MetafileCreator
    {

        public MetafileCreator(string path, string[] announceUrls, bool privateFlag,
            bool setDateFlag, string comment, string outputFilename, int threads, double pieceLenght)
        {
            try
            {
                
                string targetType = "NOTFOUND";
                if (File.Exists(path))
                {
                    targetType = "FILE";
                }
                else if (Directory.Exists(path))
                {
                    targetType = "DIR";
                }
                if (targetType == "NOTFOUND")
                {
                    throw new FileNotFoundException();
                }
                if (targetType == "DIR")
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    string rootDir = dirInfo.ToString();
                    Console.WriteLine("-------Directory Listing:-----------");
                    foreach (string dir in getDirectoryContents(dirInfo).ToArray())
                    {
                        Console.WriteLine(dir);
                    }
                }
            }
            catch (DirectoryNotFoundException e)
            {
                throw new FatalException(e);
            }
            catch (FileNotFoundException e)
            {
                throw new FatalException(e);
            }
            catch (IOException e)
            {
                throw new FatalException(e);
            }
        }

        private IList<string> getDirectoryContents(DirectoryInfo dir)
        {
            IList<string> list = new List<string>();
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                list.Add(subdir.ToString());
                foreach (string subsubdirs in getDirectoryContents(subdir).ToArray())
                {
                    Console.WriteLine(subsubdirs);
                }
            }
            return list;
        }
    }

}
