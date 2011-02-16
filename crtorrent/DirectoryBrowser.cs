/**
 * crtorrent class for browsing directories
 * 
 * May be deleted
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

        public FileInfo[] getFiles()
        {
            return RootDirectory.GetFiles();
        }

        public FileInfo[] getAllFiles()
        {
            return RootDirectory.GetFiles("*", SearchOption.AllDirectories);
        }
    }
}
