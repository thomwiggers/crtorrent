using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using crtorrent.Bencode;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace crtorrent
{

    class Metafile
    {
        private List<FileInfo> files = new List<FileInfo>();
        private DirectoryBrowser mainDirectory;
        private BencodeDictionary metafile;
        private Hasher hasher;

        internal Metafile(string path, string[] announceUrls, bool privateFlag,
            bool setDateFlag, string comment, string outputFilename, int threads, 
            double pieceLenght, string creator, CancellationTokenSource cancelToken)
        {
            hasher = new Hasher(cancelToken, threads, pieceLenght);
            metafile = new BencodeDictionary();
            
            BencodeDictionary infoDict = new BencodeDictionary();
            infoDict.Add("private", 1);
            infoDict.Add("piece length", (long)pieceLenght);

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
            metafile.Add("encoding", Encoding.UTF8.WebName);
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
                    DirectoryInfo dir = new DirectoryInfo(path);
                    files.Union(dir.GetFiles("*", SearchOption.AllDirectories));
                    infoDict.Add("name", dir.Name);
                    
                    
                    BencodeList fileList = new BencodeList();
                    
                    string[] rootPathSegements = dir.Name.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                    Parallel.ForEach(files, 
                            new ParallelOptions() {
                                MaxDegreeOfParallelism = threads,
                                CancellationToken = cancelToken.Token
                            }, 
                            (file, state, index) =>
                    {
                        BencodeDictionary filesDictionary = new BencodeDictionary();
                        string filePath = file.FullName;
                        
                        hasher.AddFile(filePath);
                        filesDictionary.Add("length", file.Length);
                        
                        // Pad maken
                        string[] segments = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        BencodeList bencodePath = new BencodeList();
                        for (int i = 0; i < segments.Length; i++)
                        {
                            if (rootPathSegements.Length > i)
                                if (segments[i] == rootPathSegements[i])
                                    continue;
                            else
                            {
                                bencodePath.Add(segments[i]);
                            }
                        }
                        filesDictionary.Add("path", bencodePath);

                        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                        {
                            md5.Initialize();
                            byte[] hash = md5.ComputeHash(File.OpenRead(path));
                            StringBuilder sb = new StringBuilder();
                            foreach (byte h in hash)
                            {
                                sb.Append(h.ToString("X2"));
                            }
                            filesDictionary.Add("md5sum", sb.ToString());
                        }
                    });

                }
                if (targetType == "FILE")
                {
                    FileInfo fi = new FileInfo(path);
                    files.Add(fi);
                    hasher.AddFile(path);
                    Task t = Task.Factory.StartNew(hasher.ComputeHashes);
                    infoDict.Add("name", fi.Name);
                    infoDict.Add("length", fi.Length);
                    using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                    {
                        md5.Initialize();
                        byte[] hash = md5.ComputeHash(File.OpenRead(path));
                        StringBuilder sb = new StringBuilder();
                        foreach (byte h in hash)
                        {
                            sb.Append(h.ToString("X2"));
                        }
                        infoDict.Add("md5sum", sb.ToString());
                    }

                    t.Wait();
                }

                // hashen
                hasher.ComputeHashes();
                infoDict.Add("pieces", Encoding.UTF8.GetString(hasher.GetHashes()));

                //afronden
                metafile.Add("info", infoDict);
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
