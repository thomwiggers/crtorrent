using System;
using System.Collections.Generic;
using System.IO;
namespace crtorrent
{

    class DirectoryBrowser
    {
        public DirectoryInfo RootDirectory
        {
            get;
            set;
        }

        public DirectoryBrowser(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException("Path "+path+" not found");
            }
            RootDirectory = new DirectoryInfo(path);
        }
        public DirectoryBrowser getSubDirectory(string subdirectory)
        {
            if(Path.IsPathRooted(subdirectory))
            {
                if(Directory.GetParent(subdirectory).FullName == RootDirectory.FullName)
                {
                    return new DirectoryBrowser(subdirectory);
                }
            }
            //If we end up here, user specified a relative directory:
            //Test if it is a relative Directory:
            foreach (DirectoryInfo d in RootDirectory.GetDirectories())
            {
               if(d.Name == subdirectory)
               {
                   return new DirectoryBrowser(d.FullName);
               }
            }
            throw new DirectoryNotFoundException("Subdirectory "+subdirectory + " not found");
        }

        public DirectoryBrowser[] getSubDirectories()
        {
            List<DirectoryBrowser> returnList = new List<DirectoryBrowser>();
            foreach(DirectoryInfo d in RootDirectory.GetDirectories())
            {
                returnList.Add(new DirectoryBrowser(d.FullName));
            }
            return returnList.ToArray();
        }
    }
}
