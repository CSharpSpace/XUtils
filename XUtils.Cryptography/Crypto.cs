using System;
using System.Security.Cryptography;
using System.Text;
namespace XUtils.Cryptography
{
	public class Crypto
	{
		private static ICrypto _provider;
		public static ICrypto Provider
		{
			get
			{
				return Crypto._provider;
			}
		}
		static Crypto()
		{
			Crypto._provider = new CryptoSym();
		}
		public static void Init(ICrypto service)
		{
			Crypto._provider = service;
		}
		public static string Encrypt(string plaintext)
		{
			return Crypto._provider.Encrypt(plaintext);
		}
		public static string Decrypt(string base64Text)
		{
			return Crypto._provider.Decrypt(base64Text);
		}
		public static bool IsMatch(string encrypted, string plainText)
		{
			return Crypto._provider.IsMatch(encrypted, plainText);
		}
		public static string ToMD5Hash(string input)
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}
	}
}
