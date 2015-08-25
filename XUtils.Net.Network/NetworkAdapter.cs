using System;
using System.Diagnostics;
namespace XUtils.Net.Network
{
	public class NetworkAdapter
	{
		private long dlSpeed;
		private long ulSpeed;
		private long dlValue;
		private long ulValue;
		private long dlValueOld;
		private long ulValueOld;
		internal string name;
		internal PerformanceCounter dlCounter;
		internal PerformanceCounter ulCounter;
		public string Name
		{
			get
			{
				return this.name;
			}
		}
		public long DownloadSpeed
		{
			get
			{
				return this.dlSpeed;
			}
		}
		public long UploadSpeed
		{
			get
			{
				return this.ulSpeed;
			}
		}
		public double DownloadSpeedKbps
		{
			get
			{
				return (double)this.dlSpeed / 1024.0;
			}
		}
		public double UploadSpeedKbps
		{
			get
			{
				return (double)this.ulSpeed / 1024.0;
			}
		}
		internal NetworkAdapter(string name)
		{
			this.name = name;
		}
		internal void init()
		{
			this.dlValueOld = this.dlCounter.NextSample().RawValue;
			this.ulValueOld = this.ulCounter.NextSample().RawValue;
		}
		internal void refresh()
		{
			this.dlValue = this.dlCounter.NextSample().RawValue;
			this.ulValue = this.ulCounter.NextSample().RawValue;
			this.dlSpeed = this.dlValue - this.dlValueOld;
			this.ulSpeed = this.ulValue - this.ulValueOld;
			this.dlValueOld = this.dlValue;
			this.ulValueOld = this.ulValue;
		}
		public override string ToString()
		{
			return this.name;
		}
	}
}
