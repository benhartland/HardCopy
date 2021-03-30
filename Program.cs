using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SD_Recovery
{
	class Program
	{
		static bool copyMode = true;
		static void Main(string[] args)
		{

			//if (File.Exists("skip")) skipNext = File.ReadAllLines("skip");
			//if (!copyMode) 

			//Recurse("G:\\DCIM\\100ANDRO", "E:\\DCIM\\100ANDRO");
			Recurse("E:\\DCIM\\100ANDRO", "S:\\DCIM\\100ANDRO");

			//if (File.Exists("skip")) toCopy = File.ReadAllLines("copy");
			//if (copyMode) 
			HardCopy();
		}

		static List<string> toCopy = new List<string>();

		static void HardCopy()
		{
			foreach (string source in toCopy)
			{
				//string target = source.Replace("G:\\", "E:\\");
				string target = source.Replace("E:\\", "S:\\");

				Directory.CreateDirectory(Path.GetDirectoryName(target));

				// Open target file stream
				using (Stream targetFile = new FileStream(target, FileMode.Create, FileAccess.Write))
				{
					Console.WriteLine(source + " # " + new FileInfo(source).Length);

					byte[] bytes = new byte[new FileInfo(source).Length];

					int numBytesToRead = bytes.Length;
					int numBytesRead = 0;

					int byteLimit = 16;


					while (1 == 1) {
						try
						{
							using (Stream sourceFile = File.OpenRead(source))
							{
								int n = sourceFile.Read(bytes, numBytesRead, numBytesToRead < byteLimit ? numBytesToRead : byteLimit);

								Console.WriteLine(source + ": " + numBytesRead + "/" + numBytesToRead + "(" + n + ")");

								numBytesRead += n;
								numBytesToRead -= n;

								if (numBytesToRead == 0) break;
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Could not stream bytes " + ex.Message);
						}
					}

					// Write out the whole file
					targetFile.Write(bytes, 0, bytes.Length);

					Console.WriteLine(DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss") + " Finished reading " + source);
				}
			}
		}

		static void Copy()
		{
			while (toCopy.Count() > 0)
			{
				int i = new Random().Next(0, toCopy.Count()-1);
				try
				{
					if (File.Exists(toCopy[i].Replace("G:\\", "E:\\")))
					{
						toCopy.RemoveAt(i);
						Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Skipping Exists " + toCopy[i] + ", leftover: " + toCopy.Count());
						continue; 
					}
					if (Path.GetExtension(toCopy[i]).ToLower() == ".mp4")  Console.WriteLine("Warning MP4");

					File.Copy(toCopy[i], toCopy[i].Replace("G:\\", "E:\\"));

					toCopy.RemoveAt(i);

					Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Copied " + toCopy[i] + ", leftover: " + toCopy.Count());
				}
				catch
				{
					Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Cannot copy " + toCopy[i]);
				}
			}
		}

		static void Recurse(string source, string destination)
		{
			try
			{
				string[] dirs = Directory.GetDirectories(source);

				foreach (string dir in dirs)
				{
					Recurse(dir, destination + "\\" + Path.GetFileName(dir));
				}
			}
			catch
			{
				Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Skipping " + source);
				return;
			}

			try
			{
				string[] files = Directory.GetFiles(source, "*.JPG");


				foreach (string file in files)
				{
					try
					{
						string target = destination + "\\" + Path.GetFileName(file);

						if (!File.Exists(target))
						{
							toCopy.Add(file);
							Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": To Copy " + file);
							continue;
						}
						
					}
					catch
					{
						Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Cannot index " + file);
					}
				}
			}
			catch
			{
				Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": Skipping " + source);
			}
		}
	}
}
