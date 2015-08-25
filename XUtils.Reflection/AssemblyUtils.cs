using System;
using System.IO;
using System.Reflection;
namespace XUtils.Reflection
{
	public class AssemblyUtils
	{
		public static string GetInternalFileContent(string assemblyFolderPath, string fileName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(assemblyFolderPath + fileName);
			if (manifestResourceStream == null)
			{
				return string.Empty;
			}
			StreamReader streamReader = new StreamReader(manifestResourceStream);
			return streamReader.ReadToEnd();
		}
	}
}
