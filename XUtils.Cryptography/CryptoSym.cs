using System;
using System.Security.Cryptography;
namespace XUtils.Cryptography
{
	public class CryptoSym : ICrypto
	{
		protected CryptoConfig _encryptionOptions;
		protected SymmetricAlgorithm _algorithm;
		public CryptoConfig Settings
		{
			get
			{
				return this._encryptionOptions;
			}
		}
		public CryptoSym()
		{
			this._encryptionOptions = new CryptoConfig();
			this._algorithm = CryptographyUtils.CreateSymmAlgoTripleDes();
		}
		public CryptoSym(string key)
		{
			this._encryptionOptions = new CryptoConfig(true, key);
			this._algorithm = CryptographyUtils.CreateSymmAlgoTripleDes();
		}
		public CryptoSym(string key, SymmetricAlgorithm algorithm)
		{
			this._encryptionOptions = new CryptoConfig(true, key);
			this._algorithm = algorithm;
		}
		public CryptoSym(CryptoConfig options, SymmetricAlgorithm algorithm)
		{
			this._encryptionOptions = options;
			this._algorithm = algorithm;
		}
		public void SetAlgorithm(SymmetricAlgorithm algorithm)
		{
			this._algorithm = algorithm;
		}
		public virtual string Encrypt(string plaintext)
		{
			if (!this._encryptionOptions.Encrypt)
			{
				return plaintext;
			}
			return CryptographyUtils.Encrypt(this._algorithm, plaintext, this._encryptionOptions.InternalKey);
		}
		public virtual string Decrypt(string base64Text)
		{
			if (!this._encryptionOptions.Encrypt)
			{
				return base64Text;
			}
			return CryptographyUtils.Decrypt(this._algorithm, base64Text, this._encryptionOptions.InternalKey);
		}
		public bool IsMatch(string encrypted, string plainText)
		{
			string strA = this.Decrypt(encrypted);
			return string.Compare(strA, plainText, false) == 0;
		}
	}
}
