using System;
namespace XUtils.Threading.Base.Internal
{
	internal class STPInstanceNullPerformanceCounter : STPInstancePerformanceCounter
	{
		public override void Increment()
		{
		}
		public override void IncrementBy(long value)
		{
		}
		public override void Set(long val)
		{
		}
	}
}
