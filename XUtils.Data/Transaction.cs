using System;
using System.Data;
namespace XUtils.Data
{
	public class Transaction
	{
		public TransType TransType
		{
			get;
			set;
		}
		public string CommandText
		{
			get;
			set;
		}
		public IDataParameter[] Parameters
		{
			get;
			set;
		}
	}
}
