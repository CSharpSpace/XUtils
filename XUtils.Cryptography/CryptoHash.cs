using System;
using System.Security.Cryptography;
namespace XUtils.Cryptography
{
	public class CryptoHash : ICrypto
	{
		protected CryptoConfig _encryptionOptions;
		protected HashAlgorithm _algorithm;
		public CryptoConfig Settings
		{
			get
			{
				return this._encryptionOptions;
			}
		}
		public CryptoHash()
		{
			this._encryptionOptions = new CryptoConfig();
			this._algorithm = CryptographyUtils.CreateHashAlgoMd5();
		}
		public CryptoHash(string key, HashAlgorithm algorithm)
		{
			this._encryptionOptions = new CryptoConfig(true, key);
			this._algorithm = algorithm;
		}
		public CryptoHash(CryptoConfig options, HashAlgorithm algorithm)
		{
			this._encryptionOptions = options;
			this._algorithm = algorithm;
		}
		public void SetAlgorithm(HashAlgorithm algorithm)
		{
			this._algorithm = algorithm;
		}
		public string Encrypt(string plaintext)
		{
			if (!this._encryptionOptions.Encrypt)
			{
				return plaintext;
			}
			return CryptographyUtils.Encrypt(this._algorithm, plaintext);
		}
		public string Decrypt(string base64Text)
		{
			throw new NotSupportedException("Can not decrypt hash algorithm.");
		}
		public bool IsMatch(string encrypted, string plainText)
		{
			string strB = this.Encrypt(plainText);
			return string.Compare(encrypted, strB, false) == 0;
		}
	}
}
