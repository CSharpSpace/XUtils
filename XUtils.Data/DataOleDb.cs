using System;
using System.Data;
using System.Data.OleDb;
namespace XUtils.Data
{
	public class DataOleDb : DataBase
	{
		public override IDbConnection GetConnection(string connectionString)
		{
			return new OleDbConnection(connectionString);
		}
		protected override IDbDataAdapter GetDataAdapter()
		{
			return new OleDbDataAdapter();
		}
		protected override void DeriveParameters(IDbCommand cmd)
		{
			if (!(cmd is OleDbCommand))
			{
				throw new ArgumentException("The command provided is not a OleDbCommand instance.", "cmd");
			}
			OleDbCommandBuilder.DeriveParameters((OleDbCommand)cmd);
		}
		public override IDataParameter BuildParameter()
		{
			return new OleDbParameter();
		}
		private string GetParameterName(string parameterName)
		{
			if (parameterName.IndexOf("@") == -1)
			{
				"@" + parameterName;
			}
			return parameterName;
		}
		public override IDataParameter BuildParameter(string parameterName)
		{
			return new OleDbParameter
			{
				ParameterName = this.GetParameterName(parameterName)
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType)
		{
			return new OleDbParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, object paramValue)
		{
			OleDbParameter oleDbParameter = this.BuildParameter(parameterName, dbType) as OleDbParameter;
			oleDbParameter.Value = paramValue;
			return oleDbParameter;
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection)
		{
			return new OleDbParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType,
				Direction = paramDirection
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection, int size)
		{
			return new OleDbParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType,
				Direction = paramDirection,
				Size = size
			};
		}
		public override bool DrHasRows(IDataReader dataReader)
		{
			if (dataReader == null || !(dataReader is OleDbDataReader))
			{
				throw new ArgumentException("The dataReader provided is not a OleDbDataReader instance.", "dataReader");
			}
			OleDbDataReader oleDbDataReader = dataReader as OleDbDataReader;
			return oleDbDataReader.HasRows;
		}
	}
}
