using System;
using System.Collections;
using System.Diagnostics;
using System.Timers;
namespace XUtils.Net.Network
{
	public class NetworkMonitor
	{
		private Timer timer;
		private ArrayList adapters;
		private ArrayList monitoredAdapters;
		public NetworkAdapter[] Adapters
		{
			get
			{
				return (NetworkAdapter[])this.adapters.ToArray(typeof(NetworkAdapter));
			}
		}
		public NetworkMonitor()
		{
			this.adapters = new ArrayList();
			this.monitoredAdapters = new ArrayList();
			this.EnumerateNetworkAdapters();
			this.timer = new Timer(1000.0);
			this.timer.Elapsed += new ElapsedEventHandler(this.timer_Elapsed);
		}
		private void EnumerateNetworkAdapters()
		{
			PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
			string[] instanceNames = performanceCounterCategory.GetInstanceNames();
			for (int i = 0; i < instanceNames.Length; i++)
			{
				string text = instanceNames[i];
				if (!(text == "MS TCP Loopback interface"))
				{
					NetworkAdapter networkAdapter = new NetworkAdapter(text);
					networkAdapter.dlCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", text);
					networkAdapter.ulCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", text);
					this.adapters.Add(networkAdapter);
				}
			}
		}
		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (NetworkAdapter networkAdapter in this.monitoredAdapters)
			{
				networkAdapter.refresh();
			}
		}
		public void StartMonitoring()
		{
			if (this.adapters.Count > 0)
			{
				foreach (NetworkAdapter networkAdapter in this.adapters)
				{
					if (!this.monitoredAdapters.Contains(networkAdapter))
					{
						this.monitoredAdapters.Add(networkAdapter);
						networkAdapter.init();
					}
				}
				this.timer.Enabled = true;
			}
		}
		public void StartMonitoring(NetworkAdapter adapter)
		{
			if (!this.monitoredAdapters.Contains(adapter))
			{
				this.monitoredAdapters.Add(adapter);
				adapter.init();
			}
			this.timer.Enabled = true;
		}
		public void StopMonitoring()
		{
			this.monitoredAdapters.Clear();
			this.timer.Enabled = false;
		}
		public void StopMonitoring(NetworkAdapter adapter)
		{
			if (this.monitoredAdapters.Contains(adapter))
			{
				this.monitoredAdapters.Remove(adapter);
			}
			if (this.monitoredAdapters.Count == 0)
			{
				this.timer.Enabled = false;
			}
		}
	}
}
