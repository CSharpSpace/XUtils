using System;
using System.Text;
namespace XUtils
{
	public class TextSplitter
	{
		public static string CheckAndSplitText(string text, int maxCharsInWord)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			bool flag = false;
			int num = 0;
			int indexOfSpacer = text.GetIndexOfSpacer(num, ref flag);
			if (indexOfSpacer < 0 && text.Length > maxCharsInWord)
			{
				return TextSplitter.SplitWord(text, maxCharsInWord, " ");
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (num < text.Length && indexOfSpacer > 0)
			{
				int num2 = indexOfSpacer - num;
				string text2 = text.Substring(num, num2);
				string str = flag ? Environment.NewLine : " ";
				if (num2 > maxCharsInWord)
				{
					string str2 = TextSplitter.SplitWord(text2, maxCharsInWord, " ");
					stringBuilder.Append(str2 + str);
				}
				else
				{
					stringBuilder.Append(text2 + str);
				}
				num = (flag ? (indexOfSpacer + 2) : (indexOfSpacer + 1));
				indexOfSpacer = text.GetIndexOfSpacer(num, ref flag);
			}
			if (num < text.Length && indexOfSpacer < 0)
			{
				int num3 = text.Length - num;
				string text3 = text.Substring(num, num3);
				if (flag)
				{
					string arg_DE_0 = Environment.NewLine;
				}
				if (num3 > maxCharsInWord)
				{
					string value = TextSplitter.SplitWord(text3, maxCharsInWord, " ");
					stringBuilder.Append(value);
				}
				else
				{
					stringBuilder.Append(text3);
				}
			}
			return stringBuilder.ToString();
		}
		public static string SplitWord(string text, int charsPerWord, string spacer)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			int numberOfTimesToSplit = TextSplitter.GetNumberOfTimesToSplit(text.Length, charsPerWord);
			if (numberOfTimesToSplit == 0)
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 1; i <= numberOfTimesToSplit; i++)
			{
				string value = (i < numberOfTimesToSplit) ? text.Substring(num, charsPerWord) : text.Substring(num);
				stringBuilder.Append(value);
				if (i < numberOfTimesToSplit)
				{
					stringBuilder.Append(spacer);
				}
				num += charsPerWord;
			}
			return stringBuilder.ToString();
		}
		public static int GetNumberOfTimesToSplit(int wordLength, int maxCharsInWord)
		{
			if (wordLength <= maxCharsInWord)
			{
				return 0;
			}
			int num = wordLength / maxCharsInWord;
			int num2 = wordLength % maxCharsInWord;
			if (num2 > 0)
			{
				num++;
			}
			return num;
		}
	}
}
