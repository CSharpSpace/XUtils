using System;
using System.Runtime.InteropServices;
using System.Text;
namespace XUtils
{
	public class ConsoleProgressBar
	{
		public struct COORD
		{
			public short X;
			public short Y;
			public COORD(short x, short y)
			{
				this.X = x;
				this.Y = y;
			}
		}
		private struct SMALL_RECT
		{
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}
		private struct CONSOLE_SCREEN_BUFFER_INFO
		{
			public ConsoleProgressBar.COORD dwSize;
			public ConsoleProgressBar.COORD dwCursorPosition;
			public int wAttributes;
			public ConsoleProgressBar.SMALL_RECT srWindow;
			public ConsoleProgressBar.COORD dwMaximumWindowSize;
		}
		private const int STD_OUTPUT_HANDLE = -11;
		private int mHConsoleHandle;
		private ConsoleProgressBar.COORD barCoord;
		private StringBuilder progressBar = new StringBuilder();
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetStdHandle(int nStdHandle);
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput, out ConsoleProgressBar.CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);
		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int SetConsoleCursorPosition(int hConsoleOutput, ConsoleProgressBar.COORD dwCursorPosition);
		public ConsoleProgressBar()
		{
			this.mHConsoleHandle = ConsoleProgressBar.GetStdHandle(-11);
			this.barCoord = this.GetCursorPos();
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine();
		}
		public void SetCursorPos(short x, short y)
		{
			ConsoleProgressBar.SetConsoleCursorPosition(this.mHConsoleHandle, new ConsoleProgressBar.COORD(x, y));
		}
		public ConsoleProgressBar.COORD GetCursorPos()
		{
			ConsoleProgressBar.CONSOLE_SCREEN_BUFFER_INFO cONSOLE_SCREEN_BUFFER_INFO;
			ConsoleProgressBar.GetConsoleScreenBufferInfo(this.mHConsoleHandle, out cONSOLE_SCREEN_BUFFER_INFO);
			return cONSOLE_SCREEN_BUFFER_INFO.dwCursorPosition;
		}
		public void Update(int transferredBytes, int totalBytes, string message)
		{
			ConsoleProgressBar.COORD cursorPos = this.GetCursorPos();
			this.SetCursorPos(this.barCoord.X, this.barCoord.Y);
			int num;
			if (totalBytes != 0)
			{
				num = (int)((double)transferredBytes * 100.0 / (double)totalBytes);
			}
			else
			{
				num = 0;
			}
			this.progressBar.Length = 0;
			this.progressBar.Append(num);
			this.progressBar.Append("% [");
			for (double num2 = 0.0; num2 < 50.0; num2 += 1.0)
			{
				if (num2 * 2.0 < (double)num)
				{
					this.progressBar.Append(">");
				}
				else
				{
					this.progressBar.Append("-");
				}
			}
			this.progressBar.Append("] ");
			if (totalBytes != 0)
			{
				int n = (int)((double)transferredBytes / 1000.0);
				int n2 = (int)((double)totalBytes / 1000.0);
				this.progressBar.Append(this.comma(n) + "K/" + this.comma(n2) + "K\n");
			}
			else
			{
				this.progressBar.Append("0.0K\n");
			}
			this.progressBar.Append(message);
			this.progressBar.Append("                        \n");
			Console.Write(this.progressBar);
			this.SetCursorPos(cursorPos.X, cursorPos.Y);
		}
		private string comma(int n)
		{
			string text = n.ToString();
			for (int i = text.Length - 3; i > 0; i -= 3)
			{
				text = text.Insert(i, ",");
			}
			return text;
		}
	}
}
