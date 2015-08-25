using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
namespace XUtils.IO
{
	public class WatcherTimer
	{
		private int TimeoutMillis = 3000;
		private FileSystemWatcher fsw = new FileSystemWatcher();
		private Timer m_timer;
		private List<string> files = new List<string>();
		private FileSystemEventHandler fswHandler;
		~WatcherTimer()
		{
			GC.Collect();
			GC.SuppressFinalize(this);
		}
		public WatcherTimer(FileSystemEventHandler watchHandler)
		{
			this.m_timer = new Timer(new TimerCallback(this.OnTimer), null, -1, -1);
			this.fswHandler = watchHandler;
		}
		public WatcherTimer(FileSystemEventHandler watchHandler, int timerInterval)
		{
			this.m_timer = new Timer(new TimerCallback(this.OnTimer), null, -1, -1);
			this.TimeoutMillis = timerInterval;
			this.fswHandler = watchHandler;
		}
		public void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			Mutex mutex = new Mutex(false, "FSW");
			mutex.WaitOne();
			if (!this.files.Contains(e.Name))
			{
				this.files.Add(e.Name);
			}
			mutex.ReleaseMutex();
			this.m_timer.Change(this.TimeoutMillis, -1);
		}
		private void OnTimer(object state)
		{
			List<string> list = new List<string>();
			Mutex mutex = new Mutex(false, "FSW");
			mutex.WaitOne();
			list.AddRange(this.files);
			this.files.Clear();
			mutex.ReleaseMutex();
			foreach (string current in list)
			{
				this.fswHandler(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, string.Empty, current));
			}
		}
	}
}
