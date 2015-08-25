using System;
using System.Diagnostics;
namespace XUtils.Threading.Base.Internal
{
	internal class STPPerformanceCounter
	{
		private readonly PerformanceCounterType _pcType;
		protected string _counterHelp;
		protected string _counterName;
		public string Name
		{
			get
			{
				return this._counterName;
			}
		}
		public STPPerformanceCounter(string counterName, string counterHelp, PerformanceCounterType pcType)
		{
			this._counterName = counterName;
			this._counterHelp = counterHelp;
			this._pcType = pcType;
		}
		public void AddCounterToCollection(CounterCreationDataCollection counterData)
		{
			CounterCreationData value = new CounterCreationData(this._counterName, this._counterHelp, this._pcType);
			counterData.Add(value);
		}
	}
}
