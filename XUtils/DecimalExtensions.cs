using System;
namespace XUtils
{
	public static class DecimalExtensions
	{
		public static int DigitAtPosition(this decimal number, int position)
		{
			if (position <= 0)
			{
				throw new ArgumentException("Position must be positive.");
			}
			if (number < 0m)
			{
				number = Math.Abs(number);
			}
			return number.SanitizedDigitAtPosition(position);
		}
		private static int SanitizedDigitAtPosition(this decimal sanitizedNumber, int validPosition)
		{
			sanitizedNumber -= Math.Floor(sanitizedNumber);
			if (sanitizedNumber == 0m)
			{
				return 0;
			}
			if (validPosition == 1)
			{
				return (int)(sanitizedNumber * 10m);
			}
			return (sanitizedNumber * 10m).SanitizedDigitAtPosition(validPosition - 1);
		}
		public static decimal Pow(this decimal number, decimal exponent)
		{
			if (exponent == 0m)
			{
				return 1m;
			}
			if (number == 0m || number == 1m)
			{
				return number;
			}
			decimal num = number;
			if (Math.Truncate(exponent) < exponent)
			{
				return new decimal(Math.Pow(decimal.ToDouble(number), decimal.ToDouble(exponent)));
			}
			decimal d = (exponent < 0m) ? Math.Abs(exponent) : exponent;
			int num2 = 1;
			while (num2 < d)
			{
				num *= number;
				num2++;
			}
			if (exponent > 0m)
			{
				return num;
			}
			return 1m / num;
		}
	}
}
