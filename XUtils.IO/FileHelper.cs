using System;
using System.IO;
using System.Text;
using System.Web;
namespace XUtils.IO
{
	public class FileHelper : IDisposable
	{
		private bool _alreadyDispose;
		public static int maxBufferLength = 1024;
		~FileHelper()
		{
			this.Dispose();
		}
		protected virtual void Dispose(bool isDisposing)
		{
			if (this._alreadyDispose)
			{
				return;
			}
			this._alreadyDispose = true;
		}
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		public static string GetFileExtName(string filename)
		{
			int num = filename.LastIndexOf(".");
			int length = filename.Length;
			return filename.Substring(num, length - num);
		}
		public static void ReWriteFile(string Path, string Strings, Encoding StringEncoding)
		{
			if (!File.Exists(Path))
			{
				FileStream fileStream = File.Create(Path);
				fileStream.Close();
			}
			StreamWriter streamWriter = new StreamWriter(Path, false, StringEncoding);
			streamWriter.Write(Strings);
			streamWriter.Close();
			streamWriter.Dispose();
		}
		public static string ReadFile(string Path, Encoding StringEncoding)
		{
			string result = null;
			if (!File.Exists(Path))
			{
				return result;
			}
			StreamReader streamReader = new StreamReader(Path, StringEncoding);
			result = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			return result;
		}
		public static void FileAdd(string Path, string strings)
		{
			StreamWriter streamWriter = File.AppendText(Path);
			streamWriter.Write(strings);
			streamWriter.Flush();
			streamWriter.Close();
		}
		public static void FileCoppy(string orignFile, string NewFile)
		{
			File.Copy(orignFile, NewFile, true);
		}
		public static void FileDel(string Path)
		{
			File.Delete(Path);
		}
		public static void FileMove(string orignFile, string NewFile)
		{
			File.Move(orignFile, NewFile);
		}
		public static void FolderCreate(string orignFolder, string NewFloder)
		{
			Directory.SetCurrentDirectory(orignFolder);
			Directory.CreateDirectory(NewFloder);
		}
		public static void DeleteFolder(string dir)
		{
			if (Directory.Exists(dir))
			{
				string[] fileSystemEntries = Directory.GetFileSystemEntries(dir);
				for (int i = 0; i < fileSystemEntries.Length; i++)
				{
					string text = fileSystemEntries[i];
					if (File.Exists(text))
					{
						File.Delete(text);
					}
					else
					{
						FileHelper.DeleteFolder(text);
					}
				}
				Directory.Delete(dir);
			}
		}
		public static void CopyDir(string srcPath, string aimPath)
		{
			try
			{
				if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
				{
					aimPath += Path.DirectorySeparatorChar;
				}
				if (!Directory.Exists(aimPath))
				{
					Directory.CreateDirectory(aimPath);
				}
				string[] fileSystemEntries = Directory.GetFileSystemEntries(srcPath);
				string[] array = fileSystemEntries;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (Directory.Exists(text))
					{
						FileHelper.CopyDir(text, aimPath + Path.GetFileName(text));
					}
					else
					{
						File.Copy(text, aimPath + Path.GetFileName(text), true);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.ToString());
			}
		}
		public static string SavePostedFile(DirectoryInfo DirPath, HttpPostedFile PostedFile)
		{
			if (PostedFile.ContentLength == 0)
			{
				throw new FileNotFoundException(string.Format("上传文件[{0}]内容为空", PostedFile.FileName));
			}
			string fullName = DirPath.FullName;
			string str = DateTime.Now.Ticks.ToString();
			string fileExtName = FileHelper.GetFileExtName(PostedFile.FileName);
			string text = str + "." + fileExtName;
			PostedFile.SaveAs(fullName + text);
			return text;
		}
		public static void ReadBytes(string fullName, Action<byte[]> action)
		{
			FileInfo fileInfo = new FileInfo(fullName);
			try
			{
				long num = 0L;
				while (num < fileInfo.Length)
				{
					byte[] obj = FileHelper.ReadFileData(fullName, num, FileHelper.maxBufferLength);
					if (action != null)
					{
						action(obj);
					}
					if (fileInfo.Length - num > (long)FileHelper.maxBufferLength)
					{
						num += (long)FileHelper.maxBufferLength;
					}
					else
					{
						num += fileInfo.Length - num;
					}
				}
			}
			catch
			{
			}
		}
		public static byte[] ReadFileData(string FileName, long startPosition, int maxBufferLength)
		{
			FileStream fileStream = new FileStream(FileName, FileMode.Open);
			byte[] result;
			try
			{
				if (fileStream.Length < startPosition)
				{
					result = null;
				}
				else
				{
					byte[] array;
					if (startPosition + (long)maxBufferLength <= fileStream.Length)
					{
						array = new byte[maxBufferLength];
					}
					else
					{
						array = new byte[fileStream.Length - startPosition];
					}
					fileStream.Seek(startPosition, SeekOrigin.Begin);
					fileStream.Read(array, 0, array.Length);
					result = array;
				}
			}
			finally
			{
				fileStream.Close();
				fileStream.Dispose();
			}
			return result;
		}
	}
}
