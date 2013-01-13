using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Thom.Bencode;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Thom.Crtorrent
{

	/// <summary>
	/// Metafile.
	/// </summary>
	class Metafile
	{
		/// <summary>
		/// The files.
		/// </summary>
		private List<FileInfo> files = new List<FileInfo> ();
        
		/// <summary>
		/// The metafile as a Bencode Dictionary.
		/// </summary>
		internal BencodeDictionary metafile;
        
		/// <summary>
		/// The hasher.
		/// </summary>
		private Hasher hasher;

		/// <summary>
		/// Initializes a new instance of the <see cref="Thom.Crtorrent.Metafile"/> class.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		/// <param name='announceUrls'>
		/// Announce urls.
		/// </param>
		/// <param name='privateFlag'>
		/// Private flag.
		/// </param>
		/// <param name='setDateFlag'>
		/// Set date flag.
		/// </param>
		/// <param name='comment'>
		/// Comment.
		/// </param>
		/// <param name='outputFilename'>
		/// Output filename.
		/// </param>
		/// <param name='threads'>
		/// Threads.
		/// </param>
		/// <param name='pieceLenght'>
		/// Piece lenght.
		/// </param>
		/// <param name='creator'>
		/// Creator.
		/// </param>
		/// <param name='cancelToken'>
		/// Cancel token.
		/// </param>
		internal Metafile (string path, string[] announceUrls, bool privateFlag,
            bool setDateFlag, string comment, string outputFilename, int threads, 
            double pieceLenght, string creator, CancellationTokenSource cancelToken)
		{
			hasher = new Hasher (cancelToken, threads, pieceLenght);
			metafile = new BencodeDictionary ();
            
			BencodeDictionary infoDict = new BencodeDictionary ();
            
			infoDict.Add ("private", privateFlag ? 1 : 0);
			infoDict.Add ("piece length", (long)pieceLenght);

			if (setDateFlag) {
				TimeSpan t = (DateTime.UtcNow - new DateTime (1970, 1, 1));
				int timestamp = (int)t.TotalSeconds;
				metafile.Add ("creation date", timestamp);
			}
			if (comment != string.Empty) {
				metafile.Add ("comment", comment);
			}
			metafile.Add ("created by", creator);
			metafile.Add ("announce", announceUrls [0]);
			metafile.Add ("encoding", Encoding.UTF8.WebName);
			if (announceUrls.Length > 1) {
				metafile.AddList ("announce-list", announceUrls);
			}
            
			try {
				string targetType = "NOTFOUND";
				if (File.Exists (path)) {
					targetType = "FILE";
				} else if (Directory.Exists (path)) {
					targetType = "DIR";
				}
				if (targetType == "NOTFOUND") {
					throw new FileNotFoundException ();
				}
				if (targetType == "DIR") {
					DirectoryInfo dir = new DirectoryInfo (path);
					files.AddRange (dir.GetFiles ("*", SearchOption.AllDirectories));
					infoDict.Add ("name", dir.Name);
                    
                    
					BencodeList fileList = new BencodeList ();
                    
					string[] rootPathSegements = dir.FullName.Split (Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

					foreach (FileInfo file in files) {
						BencodeDictionary fileDictionary = new BencodeDictionary ();

						string filePath = file.FullName;

						hasher.AddFile (filePath);
						fileDictionary.Add ("length", file.Length);

						Task t = Task.Factory.StartNew (() =>
						{
							using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider()) {
								md5.Initialize ();
								FileStream f = File.OpenRead (filePath);
								byte[] hash = md5.ComputeHash (f);
								StringBuilder sb = new StringBuilder ();
								foreach (byte h in hash) {
									sb.Append (h.ToString ("X2"));
								}
								fileDictionary.Add ("md5sum", sb.ToString ());
								f.Dispose ();
							}
						}
						);

						// Pad maken
						string[] segments = filePath.Split (Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
						BencodeList bencodePath = new BencodeList ();
						for (int i = 0; i < segments.Length; i++) {
							if (rootPathSegements.Length > i) {
								if (segments [i] == rootPathSegements [i]) {
									continue;
								}
							} else {
								bencodePath.Add (segments [i]);
							}
						}
						fileDictionary.Add ("path", bencodePath);

						t.Wait ();
						fileList.Add (fileDictionary);
					}
                    
					infoDict.Add ("files", fileList);
					hasher.ChunkFiles ();

				}
				if (targetType == "FILE") {
					FileInfo fi = new FileInfo (path);
					files.Add (fi);
					hasher.AddFile (path);
					Task t = Task.Factory.StartNew (hasher.ChunkFiles);
					infoDict.Add ("name", fi.Name);
					infoDict.Add ("length", fi.Length);
					using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider()) {
						md5.Initialize ();
						FileStream f = File.OpenRead (path);
						byte[] hash = md5.ComputeHash (f);
						StringBuilder sb = new StringBuilder ();
						foreach (byte h in hash) {
							sb.Append (h.ToString ("X2"));
						}
						infoDict.Add ("md5sum", sb.ToString ());
						f.Dispose ();
					}

					t.Wait ();
				}

				// hashen
				hasher.ComputeHashes ();
				infoDict.Add ("pieces", hasher.GetHashes ());

				//afronden
				metafile.Add ("info", infoDict);

				Debug.WriteLine ("Bencode Dictionary: \n\n" + metafile.ToString ());

			} catch (DirectoryNotFoundException e) {
				throw new FatalException (e);
			} catch (FileNotFoundException e) {
				throw new FatalException (e);
			} catch (IOException e) {
				throw new FatalException (e);
			}
		}
	}

}
