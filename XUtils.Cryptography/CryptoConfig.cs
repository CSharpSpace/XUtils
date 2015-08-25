using System;
namespace XUtils.Cryptography
{
	public class CryptoConfig
	{
		private bool _encrypt = true;
		private string _internalKey = "71xu7yi71";
		public bool Encrypt
		{
			get
			{
				return this._encrypt;
			}
		}
		public string InternalKey
		{
			get
			{
				return this._internalKey;
			}
		}
		public CryptoConfig()
		{
		}
		public CryptoConfig(bool encrypt, string key)
		{
			this._encrypt = encrypt;
			this._internalKey = key;
		}
	}
}
