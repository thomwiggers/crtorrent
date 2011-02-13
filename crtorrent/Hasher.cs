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
        double pieceLenght;
        List<string> hashes = new List<string>();

        Hasher(double pieceLength, int threads = 1)
        {
            threadCount = threads;
            this.pieceLenght = pieceLength;
            
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

        byte[] GetNextBlock(double offset)
        {
        }
*/
        void HashFile(string path)
        {
            using (FileStream fin = File.OpenRead(path))
            {
                byte[] buffer = new byte[(int)pieceLenght];
                int pieceNum = 0;
                long remaining = fin.Length;
                int done = 0;
                int offset = 0;
                while (remaining > 0)
                {
                    while (done < pieceLenght)
                    {
                        int read = fin.Read(buffer, offset, (int)pieceLenght);

                        //if read == 0, EOF reached
                        if (read == 0)
                            break;

                        offset += read;
                        remaining -= read;
                    }
                    HashPiece(buffer, pieceNum);
                    done = 0;
                    pieceNum++;
                    buffer = new byte[(int)pieceLenght];
                }
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
