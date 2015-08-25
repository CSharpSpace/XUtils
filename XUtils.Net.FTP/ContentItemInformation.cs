using System;
namespace XUtils.Net.FTP
{
	public class ContentItemInformation
	{
		private ContentItemType type;
		private int rights;
		private int linksCount;
		private string owner;
		private string group;
		private string name;
		private long size;
		private DateTime lastChange;
		private DateTime created;
		public ContentItemType ItemType
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}
		public int Rights
		{
			get
			{
				return this.rights;
			}
			set
			{
				this.rights = value;
			}
		}
		public int LinksCount
		{
			get
			{
				return this.linksCount;
			}
			set
			{
				this.linksCount = value;
			}
		}
		public string Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}
		public string Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}
		public long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}
		public DateTime LastChange
		{
			get
			{
				return this.lastChange;
			}
			set
			{
				this.lastChange = value;
			}
		}
		public DateTime Created
		{
			get
			{
				return this.created;
			}
			set
			{
				this.created = value;
			}
		}
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
	}
}
