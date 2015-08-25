using System;
namespace XUtils.Cryptography
{
	public interface ICrypto
	{
		CryptoConfig Settings
		{
			get;
		}
		string Encrypt(string plaintext);
		string Decrypt(string base64Text);
		bool IsMatch(string encrypted, string plainText);
	}
}
