using System;
using System.Data;
using XUtils.Messages;
namespace XUtils.Data
{
	public class DataChangeEventArgs : EventArgs
	{
		public IDbConnection Connection
		{
			get;
			private set;
		}
		public string TableName
		{
			get;
			private set;
		}
		public OperateType Type
		{
			get;
			private set;
		}
		public string Sql
		{
			get;
			private set;
		}
		public BoolResult<int> Result
		{
			get;
			private set;
		}
		public IDataParameter[] Parameters
		{
			get;
			private set;
		}
		public DataChangeEventArgs(IDbConnection connection, OperateType Type, string tableName, string Sql, IDataParameter[] parameters, BoolResult<int> result)
		{
			this.Connection = connection;
			this.TableName = tableName;
			this.Type = Type;
			this.Sql = Sql;
			this.Parameters = parameters;
			this.Result = result;
		}
	}
}
