using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
namespace crtorrent
{
    class Hasher
    {
        int threadCount;
        double pieceLength;
        int numPiece;
        int totalPieces;
        long totalByteSize;
        List<string> hashes = new List<string>();
        List<string> files = new List<string>();
        int fileIndex = 0;
        List<BufferedStream> streams = new List<BufferedStream>();
        BufferedStream bufferFile;
        BufferedStream BufferFile
        {
            get
            {
                lock (this)
                {
                    if (bufferFile == null)
                    {
                        BufferNextFile();
                    }
                    return bufferFile;
                }
            }
        }
        Hasher(double pieceLength, int threads = 1)
        {
            threadCount = threads;
            this.pieceLength = pieceLength;
        }
/*      BufferedStream getStream()
        {
            if (currentOpenFile == null)
            {
                if (files[nextFileIndex].Length < pieceLenght * 20)
                {
                    currentOpenFile = new BufferedStream(files[nextFileIndex], (int)files[nextFileIndex].Length);
                    nextFileIndex++;
                }
            }
            return currentOpenFile;
        }
*/
        void BufferNextFile()
        {
            files.RemoveAt(0);
            bufferFile = new BufferedStream(File.OpenRead(files[0]));
        }
        void Initialize()
        {
            for (fileIndex = 0; fileIndex < threadCount && fileIndex < files.Count; fileIndex++)
            {
			    streams.Add(new BufferedStream(File.OpenRead(files[i]), 10 * (int)pieceLength));
            }
            totalPieces = (int)Math.Ceiling(totalByteSize / pieceLength);
        }
        BufferedStream GetStream(int stream)
        {
            return streams[stream];
        }

        byte[] GetNextPiece()
        {
            int currentPiece;
            lock (this)
            {
                currentPiece = numPiece;
                ++numPiece;   
            }
            
            double remaining = GetStream(0).Length - currentPiece * pieceLength;
            int bufferSize = (int)pieceLength;
            byte[] buffer = new byte[bufferSize];
            int done = 0;
            while (remaining > 0)
            {
                while (done < pieceLength)
                {
                    int toRead = (int)Math.Min(pieceLength, remaining);
                    int read = BufferFile.Read(buffer, done, toRead);

                    //if read == 0, EOF reached
                    if (read == 0)
                    {
                        bufferFile = null;
                        if (files.Count == 0)
                        {
                            remaining = 0;
                            break;
                        }
                        
                    }
                    done += read;
                    remaining -= read;
                }
                done = 0;    
            }
                    
            return buffer;

        }
        void addFile(string path)
        {
            files.Add(path);
            totalByteSize += File.OpenRead(path).Length;
        }

        void HashFiles()
        {
            while (numPiece < totalPieces)
            {
                int piece = numPiece;
                HashPiece(GetNextPiece(), piece);
            }
        }

        string getFileMD5(string path)
        {
            using (FileStream fin = File.OpenRead(path))
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5.Initialize();
                return BitConverter.ToString(md5.ComputeHash(fin));   
            }
        }

        void HashPiece(byte[] piece, int pieceNum)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                sha1.Initialize();
                byte[] hash = sha1.ComputeHash(piece);
                hashes[pieceNum] = UTF8Encoding.UTF8.GetString(hash);
            }
        }
    }
}
