using System;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Text.RegularExpressions;
namespace XUtils.Data
{
	public class DataOdbc : DataBase
	{
		private static Regex _regExpr = new Regex("\\{.*call|CALL\\s\\w+.*}", RegexOptions.Compiled);
		public override IDbConnection GetConnection(string connectionString)
		{
			OdbcConnection odbcConnection = new OdbcConnection(connectionString);
			if (odbcConnection.State == ConnectionState.Open)
			{
				odbcConnection.Close();
			}
			return odbcConnection;
		}
		protected override IDbDataAdapter GetDataAdapter()
		{
			return new OdbcDataAdapter();
		}
		protected override void DeriveParameters(IDbCommand cmd)
		{
			if (!(cmd is OdbcCommand))
			{
				throw new ArgumentException("The command provided is not a OdbcCommand instance.", "cmd");
			}
			OdbcCommandBuilder.DeriveParameters((OdbcCommand)cmd);
		}
		public override IDataParameter BuildParameter()
		{
			return new OdbcParameter();
		}
		private string GetParameterName(string parameterName)
		{
			return parameterName;
		}
		public override IDataParameter BuildParameter(string parameterName)
		{
			return new OdbcParameter
			{
				ParameterName = this.GetParameterName(parameterName)
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType)
		{
			return new OdbcParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, object paramValue)
		{
			IDataParameter dataParameter = this.BuildParameter(parameterName, dbType);
			dataParameter.Value = paramValue;
			return dataParameter;
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection)
		{
			return new OdbcParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType,
				Direction = paramDirection
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection, int size)
		{
			return new OdbcParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType,
				Direction = paramDirection
			};
		}
		public override bool DrHasRows(IDataReader dataReader)
		{
			if (dataReader == null || !(dataReader is OdbcDataReader))
			{
				throw new ArgumentException("The dataReader provided is not a OdbcDataReader instance.", "dataReader");
			}
			OdbcDataReader odbcDataReader = dataReader as OdbcDataReader;
			return odbcDataReader.HasRows;
		}
		protected override void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDataParameter[] commandParameters, out bool mustCloseConnection)
		{
			base.PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
			if (command.CommandType == CommandType.StoredProcedure && !DataOdbc._regExpr.Match(command.CommandText).Success && command.CommandText.Trim().IndexOf(" ") == -1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (command.Parameters.Count != 0)
				{
					bool flag = true;
					for (int i = 0; i < command.Parameters.Count; i++)
					{
						OdbcParameter odbcParameter = command.Parameters[i] as OdbcParameter;
						if (odbcParameter.Direction != ParameterDirection.ReturnValue)
						{
							if (flag)
							{
								flag = false;
								stringBuilder.Append("(?");
							}
							else
							{
								stringBuilder.Append(",?");
							}
						}
					}
					stringBuilder.Append(")");
				}
				command.CommandText = "{ call " + command.CommandText + stringBuilder.ToString() + " }";
			}
		}
	}
}
