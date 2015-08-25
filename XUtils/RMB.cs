using System;
using System.Text.RegularExpressions;
namespace XUtils
{
	public class RMB
	{
		public static string ToChinese(double x)
		{
			string input = x.ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");
			string input2 = Regex.Replace(input, "((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\\.]|$))))", "${b}${z}");
			return Regex.Replace(input2, ".", (Match m) => "负元 零壹贰叁肆伍陆柒捌玖       分角拾佰仟萬億兆京垓秭穰"[(int)(m.Value[0] - '-')].ToString());
		}
		public static string ToChinese(string x)
		{
			double x2 = TypeParsers.ConvertTo<double>(x);
			return RMB.ToChinese(x2);
		}
	}
}
