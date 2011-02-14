using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using crtorrent.Bencode;
namespace crtorrent
{

    class Metafile
    {
        private List<FileInfo> files = new List<FileInfo>();
        private DirectoryBrowser mainDirectory;
        private BencodeDictionary metafile;
        private Hasher hasher;

        public Metafile(string path, string[] announceUrls, bool privateFlag,
            bool setDateFlag, string comment, string outputFilename, int threads, double pieceLenght, string creator)
        {
            hasher = new Hasher(pieceLenght, threads);
            metafile = new BencodeDictionary();
            BencodeDictionary infoDict = new BencodeDictionary();
            metafile.Add("info", infoDict);
            if (setDateFlag)
            {
                TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                int timestamp = (int)t.TotalSeconds;
                metafile.Add("creation date", timestamp);
            }
            if(comment != string.Empty)
            {
                metafile.Add("comment", comment);
            }
            metafile.Add("created by", creator);
            metafile.Add("announce", announceUrls[0]);
            
            if(announceUrls.Length > 1)
            {
                metafile.AddList("announce-list", announceUrls);
            }
            
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
                    mainDirectory = new DirectoryBrowser(path);
                    files.Union(mainDirectory.getAllFiles());
                }
                if (targetType == "FILE")
                {
                    hasher.HashFile(path);

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
