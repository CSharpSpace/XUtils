using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
namespace XUtils
{
	public static class CodeTimer
	{
		public static void Initialize()
		{
			Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			CodeTimer.Time("", 1, delegate(int i)
			{
			});
			CodeTimer.Time(delegate
			{
			});
			CodeTimer.Time("", delegate
			{
			});
		}
		public static void Time(string name, int iteration, Action<int> action)
		{
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			ConsoleColor foregroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(name);
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
			int[] array = new int[GC.MaxGeneration + 1];
			for (int i = 0; i <= GC.MaxGeneration; i++)
			{
				array[i] = GC.CollectionCount(i);
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ulong cycleCount = CodeTimer.GetCycleCount();
			for (int j = 0; j < iteration; j++)
			{
				action(j);
			}
			ulong num = CodeTimer.GetCycleCount() - cycleCount;
			stopwatch.Stop();
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine("\tTime Elapsed:\t" + stopwatch.ElapsedMilliseconds.ToString("N0") + "ms");
			Console.WriteLine("\tCPU Cycles:\t" + num.ToString("N0"));
			for (int k = 0; k <= GC.MaxGeneration; k++)
			{
				int num2 = GC.CollectionCount(k) - array[k];
				Console.WriteLine(string.Concat(new object[]
				{
					"\tGen ",
					k,
					": \t\t",
					num2
				}));
			}
			Console.WriteLine();
		}
		private static ulong GetCycleCount()
		{
			ulong result = 0uL;
			CodeTimer.QueryThreadCycleTime(CodeTimer.GetCurrentThread(), ref result);
			return result;
		}
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentThread();
		public static string Time(string Explain, Action action)
		{
			if (action != null)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				action();
				stopwatch.Stop();
				string arg = CodeTimer.SerializationTime(stopwatch);
				return string.Format("[{0}] - Time Elapsed : {1}", Explain, arg);
			}
			return string.Empty;
		}
		public static string Time(Action action)
		{
			if (action != null)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				action();
				stopwatch.Stop();
				return string.Format("Time Elapsed : {0}", CodeTimer.SerializationTime(stopwatch));
			}
			return string.Empty;
		}
		private static string SerializationTime(Stopwatch stopWatch)
		{
			return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", new object[]
			{
				stopWatch.Elapsed.Hours,
				stopWatch.Elapsed.Minutes,
				stopWatch.Elapsed.Seconds,
				stopWatch.Elapsed.Milliseconds
			});
		}
	}
}
