using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using XUtils.Serialization;
namespace XUtils.Net
{
	public class FlowControl : TimerBase
	{
		private static object syncLock = new object();
		private static FlowControl flow = null;
		private decimal _setSize = 1024m;
		private decimal _netBytes;
		private decimal _Kbyte = 1024m;
		private Action<decimal> _actionMonitoring;
		public static FlowControl Singleton
		{
			get
			{
				if (FlowControl.flow == null)
				{
					object obj;
					Monitor.Enter(obj = FlowControl.syncLock);
					try
					{
						if (FlowControl.flow == null)
						{
							FlowControl.flow = new FlowControl();
						}
					}
					finally
					{
						Monitor.Exit(obj);
					}
				}
				return FlowControl.flow;
			}
		}
		public decimal Size
		{
			get
			{
				return this._setSize / 8m;
			}
			set
			{
				object obj;
				Monitor.Enter(obj = FlowControl.syncLock);
				try
				{
					this._setSize = value * 8m;
				}
				finally
				{
					Monitor.Exit(obj);
				}
			}
		}
		public FlowControl() : base(1000)
		{
			this.timer.AutoReset = true;
			this.timer.Start();
		}
		public override void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (this._actionMonitoring != null)
			{
				this._actionMonitoring(this._netBytes / 8m);
			}
		}
		public void Monitoring(Action<decimal> action)
		{
			this._actionMonitoring = action;
		}
		public void Limit(DataTable dataTable, Action<DataRow[]> Callback)
		{
			Monitor.Enter(FlowControl.syncLock);
			try
			{
				if (dataTable != null && Callback != null)
				{
					decimal _mSize = this._setSize;
					DataTable newDataTable = dataTable.Clone();
					List<DataRow> rows = new List<DataRow>();
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					Array.ForEach<DataRow>(dataTable.Select(), delegate(DataRow dr)
					{
						newDataTable.Clear();
						newDataTable.ImportRow(dr);
						rows.Add(dr);
						try
						{
							byte[] array = FormatterHelper.Serialize(newDataTable);
							decimal d = array.Length / this._Kbyte;
							_mSize -= d;
							this._netBytes += d;
						}
						catch
						{
						}
						if (_mSize < 0m)
						{
							if (stopwatch.Elapsed.Milliseconds < 1000)
							{
								Thread.Sleep(1000 - stopwatch.Elapsed.Milliseconds);
							}
							Callback(rows.ToArray());
							rows.Clear();
							stopwatch.Reset();
							stopwatch.Start();
							_mSize = this._setSize;
							this._netBytes = 0m;
						}
					});
					Callback(rows.ToArray());
					this._netBytes = 0m;
				}
			}
			finally
			{
				Monitor.Exit(FlowControl.syncLock);
			}
		}
		public void Limit<TEntity>(List<TEntity> entities, Action<TEntity[]> Callback) where TEntity : class, new()
		{
			Monitor.Enter(FlowControl.syncLock);
			try
			{
				if (entities != null && Callback != null && entities.Count != 0)
				{
					decimal _mSize = this._setSize;
					List<TEntity> newEntities = new List<TEntity>();
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					Array.ForEach<TEntity>(entities.ToArray(), delegate(TEntity entity)
					{
						newEntities.Add(entity);
						try
						{
							byte[] array = FormatterHelper.Serialize(entity);
							decimal d = array.Length / this._Kbyte;
							_mSize -= d;
							this._netBytes += d;
						}
						catch
						{
						}
						if (_mSize < 0m)
						{
							if (stopwatch.Elapsed.Milliseconds < 1000)
							{
								Thread.Sleep(1000 - stopwatch.Elapsed.Milliseconds);
							}
							Callback(newEntities.ToArray());
							newEntities.Clear();
							stopwatch.Reset();
							stopwatch.Start();
							_mSize = this._setSize;
							this._netBytes = 0m;
						}
					});
					Callback(newEntities.ToArray());
					this._netBytes = 0m;
				}
			}
			finally
			{
				Monitor.Exit(FlowControl.syncLock);
			}
		}
	}
}
