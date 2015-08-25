using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
namespace XUtils.Data
{
	public class DataSqlServer : DataBase
	{
		public override IDbConnection GetConnection(string connectionString)
		{
			return new SqlConnection(connectionString);
		}
		protected override IDbDataAdapter GetDataAdapter()
		{
			return new SqlDataAdapter();
		}
		protected override void DeriveParameters(IDbCommand cmd)
		{
			if (!(cmd is SqlCommand))
			{
				throw new ArgumentException("The command provided is not a SqlCommand instance.", "cmd");
			}
			SqlCommandBuilder.DeriveParameters((SqlCommand)cmd);
		}
		public override IDataParameter BuildParameter()
		{
			return new SqlParameter();
		}
		private string GetParameterName(string parameterName)
		{
			string result = parameterName;
			if (parameterName.IndexOf("@") == -1)
			{
				result = "@" + parameterName;
			}
			return result;
		}
		public override IDataParameter BuildParameter(string parameterName)
		{
			return new SqlParameter
			{
				ParameterName = this.GetParameterName(parameterName)
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType)
		{
			return new SqlParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, object paramValue)
		{
			SqlParameter sqlParameter = this.BuildParameter(parameterName, dbType) as SqlParameter;
			sqlParameter.Value = paramValue;
			return sqlParameter;
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection)
		{
			return new SqlParameter
			{
				ParameterName = this.GetParameterName(parameterName),
				DbType = dbType,
				Direction = paramDirection
			};
		}
		public override IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection, int size)
		{
			SqlParameter sqlParameter = this.BuildParameter(parameterName, dbType, paramDirection) as SqlParameter;
			sqlParameter.Size = size;
			return sqlParameter;
		}
		public override bool DrHasRows(IDataReader dataReader)
		{
			if (dataReader == null || !(dataReader is SqlDataReader))
			{
				throw new ArgumentException("The dataReader provided is not a SqlDataReader instance.", "dataReader");
			}
			SqlDataReader sqlDataReader = dataReader as SqlDataReader;
			return sqlDataReader.HasRows;
		}
		protected override void ClearCommand(IDbCommand command)
		{
			bool flag = true;
			foreach (IDataParameter dataParameter in command.Parameters)
			{
				if (dataParameter.Direction != ParameterDirection.Input)
				{
					flag = false;
				}
			}
			if (flag)
			{
				command.Parameters.Clear();
			}
		}
		public XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
		{
			return this.ExecuteXmlReader(connection, commandType, commandText, null);
		}
		public XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			bool flag = false;
			SqlCommand sqlCommand = new SqlCommand();
			XmlReader result;
			try
			{
				this.PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters, out flag);
				XmlReader xmlReader = sqlCommand.ExecuteXmlReader();
				sqlCommand.Parameters.Clear();
				result = xmlReader;
			}
			catch
			{
				if (flag)
				{
					connection.Close();
				}
				throw;
			}
			return result;
		}
		public XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				ArrayList arrayList = new ArrayList();
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				for (int i = 0; i < spParameterSet.Length; i++)
				{
					IDataParameter value = spParameterSet[i];
					arrayList.Add(value);
				}
				SqlParameter[] commandParameters = (SqlParameter[])arrayList.ToArray(typeof(SqlParameter));
				base.AssignParameterValues(commandParameters, parameterValues);
				return this.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			return this.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
		}
		public XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
		{
			return this.ExecuteXmlReader(transaction, commandType, commandText, null);
		}
		public XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			SqlCommand sqlCommand = new SqlCommand();
			bool flag = false;
			this.PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			XmlReader result = sqlCommand.ExecuteXmlReader();
			sqlCommand.Parameters.Clear();
			return result;
		}
		public XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				ArrayList arrayList = new ArrayList();
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				for (int i = 0; i < spParameterSet.Length; i++)
				{
					IDataParameter value = spParameterSet[i];
					arrayList.Add(value);
				}
				SqlParameter[] commandParameters = (SqlParameter[])arrayList.ToArray(typeof(SqlParameter));
				base.AssignParameterValues(commandParameters, parameterValues);
				return this.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			return this.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
		}
		public XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				ArrayList arrayList = new ArrayList();
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				for (int i = 0; i < spParameterSet.Length; i++)
				{
					IDataParameter value = spParameterSet[i];
					arrayList.Add(value);
				}
				SqlParameter[] commandParameters = (SqlParameter[])arrayList.ToArray(typeof(SqlParameter));
				base.AssignParameterValues(commandParameters, dataRow);
				return this.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			return this.ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
		}
		public XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				ArrayList arrayList = new ArrayList();
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				for (int i = 0; i < spParameterSet.Length; i++)
				{
					IDataParameter value = spParameterSet[i];
					arrayList.Add(value);
				}
				SqlParameter[] commandParameters = (SqlParameter[])arrayList.ToArray(typeof(SqlParameter));
				base.AssignParameterValues(commandParameters, dataRow);
				return this.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			return this.ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
		}
	}
}
