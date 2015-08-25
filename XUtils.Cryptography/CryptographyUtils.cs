using System;
using System.Security.Cryptography;
using System.Text;
namespace XUtils.Cryptography
{
	public class CryptographyUtils
	{
		public static string Encrypt(HashAlgorithm hashAlgorithm, string dataToHash)
		{
			string text = "";
			string[] array = new string[16];
			byte[] bytes = Encoding.ASCII.GetBytes(dataToHash);
			byte[] array2 = hashAlgorithm.ComputeHash(bytes);
			for (int i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i].ToString("x");
				text += array[i];
			}
			return text;
		}
		public static bool IsHashMatch(HashAlgorithm hashAlgorithm, string hashedText, string unhashedText)
		{
			string strB = CryptographyUtils.Encrypt(hashAlgorithm, unhashedText);
			return string.Compare(hashedText, strB, false) == 0;
		}
		public static string Encrypt(SymmetricAlgorithm algorithm, string plaintext, string key)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			algorithm.Key = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(key));
			algorithm.Mode = CipherMode.ECB;
			ICryptoTransform cryptoTransform = algorithm.CreateEncryptor();
			byte[] bytes = Encoding.ASCII.GetBytes(plaintext);
			return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
		}
		public static string Decrypt(SymmetricAlgorithm algorithm, string base64Text, string key)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			algorithm.Key = mD5CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(key));
			algorithm.Mode = CipherMode.ECB;
			ICryptoTransform cryptoTransform = algorithm.CreateDecryptor();
			byte[] array = Convert.FromBase64String(base64Text);
			return Encoding.ASCII.GetString(cryptoTransform.TransformFinalBlock(array, 0, array.Length));
		}
		public static T CreateAlgo<T>(string fullyQualifiedTypeName) where T : class
		{
			object obj = Activator.CreateInstance(Type.GetType(fullyQualifiedTypeName));
			return obj as T;
		}
		public static SymmetricAlgorithm CreateSymmAlgoTripleDes()
		{
			return new TripleDESCryptoServiceProvider();
		}
		public static HashAlgorithm CreateHashAlgoMd5()
		{
			return new MD5CryptoServiceProvider();
		}
	}
}
