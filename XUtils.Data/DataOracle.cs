using System;
using System.Data;
using System.Data.OracleClient;
namespace XUtils.Data
{
	public class DataOracle : DataBase
	{
		public override IDbConnection GetConnection(string connectionString)
		{
			return new OracleConnection(connectionString);
		}
		protected override IDbDataAdapter GetDataAdapter()
		{
			return new OracleDataAdapter();
		}
		protected override void DeriveParameters(IDbCommand cmd)
		{
			if (!(cmd is OracleCommand))
			{
				throw new ArgumentException("The command provided is not a OleDbCommand instance.", "cmd");
			}
			OracleCommandBuilder.DeriveParameters((OracleCommand)cmd);
		}
		public override IDataParameter BuildParameter()
		{
			return new OracleParameter();
		}
		private string GetParameterName(string parameterName)
		{
			return parameterName;
		}
		public override IDataParameter BuildParameter(string parameterName)
		{
			return new OracleParameter
			{
				ParameterName = this.GetParameterName(parameterName.Trim())
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType)
		{
			return new OracleParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, object paramValue)
		{
			OracleParameter oracleParameter = this.BuildParameter(parameterName, dbType) as OracleParameter;
			oracleParameter.Value = paramValue;
			return oracleParameter;
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection)
		{
			OracleParameter oracleParameter = new OracleParameter();
			oracleParameter.ParameterName = this.GetParameterName(parameterName);
			oracleParameter.DbType = dbType;
			if (dbType == DbType.Object)
			{
				oracleParameter.OracleType = OracleType.Cursor;
			}
			oracleParameter.Direction = paramDirection;
			return oracleParameter;
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection, int size)
		{
			OracleParameter oracleParameter = this.BuildParameter(parameterName, dbType, paramDirection) as OracleParameter;
			oracleParameter.Size = size;
			return oracleParameter;
		}
		public override bool DrHasRows(IDataReader dataReader)
		{
			if (dataReader == null || !(dataReader is OracleDataReader))
			{
				throw new ArgumentException("The dataReader provided is not a OracleDataReader instance. please check your codes!", "dataReader");
			}
			OracleDataReader oracleDataReader = dataReader as OracleDataReader;
			return oracleDataReader.HasRows;
		}
	}
}
