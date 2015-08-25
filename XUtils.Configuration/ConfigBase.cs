using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
namespace XUtils.Configuration
{
	public abstract class ConfigBase<TKey, TValue> : ConfigContainer<TKey, TValue>
	{
		private IList<string> _configFiles = new List<string>();
		public IList<string> ConfigFiles
		{
			get
			{
				return this._configFiles;
			}
			private set
			{
				this._configFiles = value;
			}
		}
		public string DirectoryPath
		{
			get;
			set;
		}
		public virtual void DirectorySearch(string directory, string searchPattern = "*.config")
		{
			string[] files = Directory.GetFiles(directory, searchPattern);
			for (int i = 0; i < files.Length; i++)
			{
				string item = files[i];
				this._configFiles.Add(item);
			}
			string[] directories = Directory.GetDirectories(directory);
			for (int j = 0; j < directories.Length; j++)
			{
				string directory2 = directories[j];
				this.DirectorySearch(directory2, searchPattern);
			}
		}
        public virtual System.Configuration.Configuration GetConfiguration(string file)
		{
			return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
			{
				ExeConfigFilename = file
			}, ConfigurationUserLevel.None);
		}
	}
}
