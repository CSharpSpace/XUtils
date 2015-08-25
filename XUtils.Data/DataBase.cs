using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Common;
namespace XUtils.Data
{
	public abstract class DataBase : ICloneable
	{
		private enum AdoConnectionOwnership
		{
			Internal,
			External
		}
		protected static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
		public string ConnectionString
		{
			get;
			set;
		}
		public abstract IDbConnection GetConnection(string connectionString);
		protected abstract IDbDataAdapter GetDataAdapter();
		protected abstract void DeriveParameters(IDbCommand cmd);
		public abstract IDataParameter BuildParameter();
		public virtual IDataParameter GetInStringPara(string parameterName, string paraValue)
		{
			return this.BuildParameter(parameterName, DbType.String, paraValue);
		}
		public virtual IDataParameter GetInIntegerPara(string parameterName, int paraValue)
		{
			return this.BuildParameter(parameterName, DbType.Int32, paraValue);
		}
		public virtual IDataParameter GetOutParameter(string parameterName, DbType dbType)
		{
			return this.BuildParameter(parameterName, dbType, ParameterDirection.Output);
		}
		public virtual IDataParameter GetOutParameter(string parameterName, DbType dbType, int size)
		{
			return this.BuildParameter(parameterName, dbType, ParameterDirection.Output, size);
		}
		public virtual IDataParameter GetReturnParameter(string paramName)
		{
			return this.BuildParameter(paramName, DbType.Int32, ParameterDirection.ReturnValue);
		}
		public virtual IDataParameter GetReturnParameter()
		{
			return this.BuildParameter("ReturnValue", DbType.Int32, ParameterDirection.ReturnValue);
		}
		public abstract IDataParameter BuildParameter(string parameterName);
		public abstract IDataParameter BuildParameter(string parameterName, DbType dbType);
		public abstract IDataParameter BuildParameter(string parameterName, DbType dbType, object paramValue);
		public abstract IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection);
		public abstract IDataParameter BuildParameter(string parameterName, DbType dbType, ParameterDirection paramDirection, int size);
		public abstract bool DrHasRows(IDataReader dataReader);
		public virtual IDataParameter BuildParameter(string name, object value)
		{
			IDataParameter dataParameter = this.BuildParameter();
			dataParameter.ParameterName = name;
			dataParameter.Value = value;
			return dataParameter;
		}
		protected virtual void AttachParameters(IDbCommand command, IDataParameter[] commandParameters)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			if (commandParameters != null)
			{
				for (int i = 0; i < commandParameters.Length; i++)
				{
					IDataParameter dataParameter = commandParameters[i];
					if (dataParameter != null)
					{
						if ((dataParameter.Direction == ParameterDirection.InputOutput || dataParameter.Direction == ParameterDirection.Input) && dataParameter.Value == null)
						{
							dataParameter.Value = DBNull.Value;
						}
						command.Parameters.Add(dataParameter);
					}
				}
			}
		}
		protected void AssignParameterValues(IDataParameter[] commandParameters, DataRow dataRow)
		{
			if (commandParameters == null || dataRow == null)
			{
				return;
			}
			DataColumnCollection columns = dataRow.Table.Columns;
			int num = 0;
			for (int i = 0; i < commandParameters.Length; i++)
			{
				IDataParameter dataParameter = commandParameters[i];
				if (dataParameter.ParameterName == null || dataParameter.ParameterName.Length <= 1)
				{
					throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: '{1}'.", num, dataParameter.ParameterName));
				}
				if (columns.Contains(dataParameter.ParameterName))
				{
					dataParameter.Value = dataRow[dataParameter.ParameterName];
				}
				else
				{
					if (columns.Contains(dataParameter.ParameterName.Substring(1)))
					{
						dataParameter.Value = dataRow[dataParameter.ParameterName.Substring(1)];
					}
				}
				num++;
			}
		}
		protected void AssignParameterValues(IDataParameter[] commandParameters, object[] parameterValues)
		{
			if (commandParameters == null || parameterValues == null)
			{
				return;
			}
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}
			int i = 0;
			int num = commandParameters.Length;
			while (i < num)
			{
				if (parameterValues[i] is IDataParameter)
				{
					IDataParameter dataParameter = (IDataParameter)parameterValues[i];
					if (dataParameter.Value == null)
					{
						commandParameters[i].Value = DBNull.Value;
					}
					else
					{
						commandParameters[i].Value = dataParameter.Value;
					}
				}
				else
				{
					if (parameterValues[i] == null)
					{
						commandParameters[i].Value = DBNull.Value;
					}
					else
					{
						commandParameters[i].Value = parameterValues[i];
					}
				}
				i++;
			}
		}
		protected virtual void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDataParameter[] commandParameters, out bool mustCloseConnection)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			if (commandText == null || commandText.Length == 0)
			{
				throw new ArgumentNullException("commandText");
			}
			if (connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				connection.Open();
			}
			else
			{
				mustCloseConnection = false;
			}
			command.Connection = connection;
			command.CommandText = commandText;
			if (transaction != null)
			{
				if (transaction.Connection == null)
				{
					throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
				}
				command.Transaction = transaction;
			}
			command.CommandType = commandType;
			if (commandParameters != null)
			{
				this.AttachParameters(command, commandParameters);
			}
		}
		protected virtual void ClearCommand(IDbCommand command)
		{
		}
		public virtual DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			return this.ExecuteDataset(connectionString, commandType, commandText, null);
		}
		public virtual DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			DataSet result;
			using (IDbConnection connection = this.GetConnection(connectionString))
			{
				connection.Open();
				result = this.ExecuteDataset(connection, commandType, commandText, commandParameters);
			}
			return result;
		}
		public virtual DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual DataSet ExecuteDataset(IDbConnection connection, CommandType commandType, string commandText)
		{
			return this.ExecuteDataset(connection, commandType, commandText, null);
		}
		public virtual DataSet ExecuteDataset(IDbConnection connection, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			IDbCommand dbCommand = connection.CreateCommand();
			bool flag = false;
			this.PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out flag);
			if (ConfigurationManager.AppSettings["DbCommandExecuteTime"] != null)
			{
				dbCommand.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["DbCommandExecuteTime"]);
			}
			else
			{
				dbCommand.CommandTimeout = 6000;
			}
			IDbDataAdapter dbDataAdapter = null;
			DataSet result;
			try
			{
				dbDataAdapter = this.GetDataAdapter();
				dbDataAdapter.SelectCommand = dbCommand;
				DataSet dataSet = new DataSet();
				dbDataAdapter.Fill(dataSet);
				dbCommand.Parameters.Clear();
				result = dataSet;
			}
			finally
			{
				if (flag)
				{
					connection.Close();
				}
				if (dbDataAdapter != null)
				{
					IDisposable disposable = dbDataAdapter as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			return result;
		}
		public virtual DataSet ExecuteDataset(IDbConnection connection, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
		}
		public virtual DataSet ExecuteDataset(IDbTransaction transaction, CommandType commandType, string commandText)
		{
			return this.ExecuteDataset(transaction, commandType, commandText, null);
		}
		public virtual DataSet ExecuteDataset(IDbTransaction transaction, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			IDbCommand dbCommand = transaction.Connection.CreateCommand();
			bool flag = false;
			this.PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			IDbDataAdapter dbDataAdapter = null;
			DataSet result;
			try
			{
				dbDataAdapter = this.GetDataAdapter();
				dbDataAdapter.SelectCommand = dbCommand;
				DataSet dataSet = new DataSet();
				dbDataAdapter.Fill(dataSet);
				dbCommand.Parameters.Clear();
				result = dataSet;
			}
			finally
			{
				if (dbDataAdapter != null)
				{
					IDisposable disposable = dbDataAdapter as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
			return result;
		}
		public virtual DataSet ExecuteDataset(IDbTransaction transaction, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			return this.ExecuteNonQuery(connectionString, commandType, commandText, null);
		}
		public virtual int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			int result;
			using (IDbConnection connection = this.GetConnection(connectionString))
			{
				connection.Open();
				result = this.ExecuteNonQuery(connection, commandType, commandText, commandParameters);
			}
			return result;
		}
		public virtual int ExecuteSPNonQuery(string connectionString, string spName, params IDataParameter[] commandParameters)
		{
			return this.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
		}
		public virtual int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual int ExecuteNonQuery(IDbConnection connection, CommandType commandType, string commandText)
		{
			return this.ExecuteNonQuery(connection, commandType, commandText, null);
		}
		public virtual int ExecuteNonQuery(IDbConnection connection, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			IDbCommand dbCommand = connection.CreateCommand();
			bool flag = false;
			this.PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out flag);
			int result = dbCommand.ExecuteNonQuery();
			dbCommand.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
			return result;
		}
		public virtual int ExecuteNonQuery(IDbCommand cmd, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("command");
			}
			IDbConnection connection = cmd.Connection;
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			bool flag = false;
			this.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out flag);
			int result = cmd.ExecuteNonQuery();
			cmd.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
			return result;
		}
		public virtual int ExecuteNonQuery(IDbCommand cmd, string connectionString, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (cmd == null)
			{
				throw new ArgumentNullException("command");
			}
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			int result;
			using (IDbConnection connection = this.GetConnection(connectionString))
			{
				connection.Open();
				bool flag = false;
				this.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out flag);
				int num = cmd.ExecuteNonQuery();
				cmd.Parameters.Clear();
				result = num;
			}
			return result;
		}
		public virtual int ExecuteNonQuery(IDbConnection connection, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
		}
		public virtual int ExecuteNonQuery(IDbTransaction transaction, CommandType commandType, string commandText)
		{
			return this.ExecuteNonQuery(transaction, commandType, commandText, null);
		}
		public virtual int ExecuteSPNonQuery(IDbTransaction transaction, string spName, params IDataParameter[] commandParameters)
		{
			return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
		}
		public virtual int ExecuteNonQuery(IDbTransaction transaction, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			IDbCommand dbCommand = transaction.Connection.CreateCommand();
			bool flag = false;
			this.PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			int result = dbCommand.ExecuteNonQuery();
			dbCommand.Parameters.Clear();
			return result;
		}
		public virtual int ExecuteNonQuery(IDbTransaction transaction, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
		}
		private IDataReader ExecuteReader(IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDataParameter[] commandParameters, DataBase.AdoConnectionOwnership connectionOwnership)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			bool flag = false;
			IDbCommand dbCommand = connection.CreateCommand();
			if (ConfigurationManager.AppSettings["CommandTimeout"] != null)
			{
				dbCommand.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["CommandTimeout"]);
			}
			else
			{
				dbCommand.CommandTimeout = 6000;
			}
			IDataReader result;
			try
			{
				this.PrepareCommand(dbCommand, connection, transaction, commandType, commandText, commandParameters, out flag);
				IDataReader dataReader;
				if (connectionOwnership == DataBase.AdoConnectionOwnership.External)
				{
					dataReader = dbCommand.ExecuteReader();
				}
				else
				{
					dataReader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
				}
				this.ClearCommand(dbCommand);
				result = dataReader;
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
		public virtual IDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			return this.ExecuteReader(connectionString, commandType, commandText, null);
		}
		public virtual IDataReader ExecuteSPReader(string connectionString, string spName, params IDataParameter[] commandParameters)
		{
			return this.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
		}
		public virtual IDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			IDbConnection dbConnection = null;
			IDataReader result;
			try
			{
				dbConnection = this.GetConnection(connectionString);
				dbConnection.Open();
				result = this.ExecuteReader(dbConnection, null, commandType, commandText, commandParameters, DataBase.AdoConnectionOwnership.Internal);
			}
			catch
			{
				if (dbConnection != null)
				{
					dbConnection.Close();
				}
				throw;
			}
			return result;
		}
		public virtual IDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual IDataReader ExecuteReader(IDbConnection connection, CommandType commandType, string commandText)
		{
			return this.ExecuteReader(connection, commandType, commandText, null);
		}
		public virtual IDataReader ExecuteReader(IDbConnection connection, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			return this.ExecuteReader(connection, null, commandType, commandText, commandParameters, DataBase.AdoConnectionOwnership.External);
		}
		public virtual IDataReader ExecuteReader(IDbConnection connection, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteReader(connection, CommandType.StoredProcedure, spName);
		}
		public virtual IDataReader ExecuteReader(IDbTransaction transaction, CommandType commandType, string commandText)
		{
			return this.ExecuteReader(transaction, commandType, commandText, null);
		}
		public virtual IDataReader ExecuteReader(IDbTransaction transaction, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			return this.ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, DataBase.AdoConnectionOwnership.External);
		}
		public virtual IDataReader ExecuteReader(IDbTransaction transaction, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			return this.ExecuteScalar(connectionString, commandType, commandText, null);
		}
		public virtual object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			IDbConnection dbConnection = null;
			object result;
			try
			{
				dbConnection = this.GetConnection(connectionString);
				dbConnection.Open();
				result = this.ExecuteScalar(dbConnection, commandType, commandText, commandParameters);
			}
			finally
			{
				IDisposable disposable = dbConnection;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return result;
		}
		public virtual object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText)
		{
			return this.ExecuteScalar(connection, commandType, commandText, null);
		}
		public virtual object ExecuteScalar(IDbConnection connection, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			IDbCommand dbCommand = connection.CreateCommand();
			if (ConfigurationManager.AppSettings["DbCommandExecuteTime"] != null)
			{
				dbCommand.CommandTimeout = int.Parse(ConfigurationManager.AppSettings["DbCommandExecuteTime"]);
			}
			else
			{
				dbCommand.CommandTimeout = 6000;
			}
			bool flag = false;
			this.PrepareCommand(dbCommand, connection, null, commandType, commandText, commandParameters, out flag);
			object result = dbCommand.ExecuteScalar();
			dbCommand.Parameters.Clear();
			if (flag)
			{
				connection.Close();
			}
			return result;
		}
		public virtual object ExecuteScalar(IDbConnection connection, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
		}
		public virtual object ExecuteScalar(IDbTransaction transaction, CommandType commandType, string commandText)
		{
			return this.ExecuteScalar(transaction, commandType, commandText, null);
		}
		public virtual object ExecuteScalar(IDbTransaction transaction, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			IDbCommand dbCommand = transaction.Connection.CreateCommand();
			bool flag = false;
			this.PrepareCommand(dbCommand, transaction.Connection, transaction, commandType, commandText, commandParameters, out flag);
			object result = dbCommand.ExecuteScalar();
			dbCommand.Parameters.Clear();
			return result;
		}
		public virtual object ExecuteScalar(IDbTransaction transaction, string spName, params object[] parameterValues)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			IDbConnection dbConnection = null;
			try
			{
				dbConnection = this.GetConnection(connectionString);
				dbConnection.Open();
				this.FillDataset(dbConnection, commandType, commandText, dataSet, tableNames);
			}
			finally
			{
				IDisposable disposable = dbConnection;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		public virtual void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params IDataParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			IDbConnection dbConnection = null;
			try
			{
				dbConnection = this.GetConnection(connectionString);
				dbConnection.Open();
				this.FillDataset(dbConnection, commandType, commandText, dataSet, tableNames, commandParameters);
			}
			finally
			{
				IDisposable disposable = dbConnection;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		public virtual void FillDataset(string connectionString, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			IDbConnection dbConnection = null;
			try
			{
				dbConnection = this.GetConnection(connectionString);
				dbConnection.Open();
				this.FillDataset(dbConnection, spName, dataSet, tableNames, parameterValues);
			}
			finally
			{
				IDisposable disposable = dbConnection;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		public virtual void FillDataset(IDbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
			this.FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
		}
		public virtual void FillDataset(IDbConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params IDataParameter[] commandParameters)
		{
			this.FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
		}
		public virtual void FillDataset(IDbConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				this.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
				return;
			}
			this.FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
		}
		public virtual void FillDataset(IDbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
			this.FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
		}
		public virtual void FillDataset(IDbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params IDataParameter[] commandParameters)
		{
			this.FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
		}
		public virtual void FillDataset(IDbTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction");
			}
			if (transaction != null && transaction.Connection == null)
			{
				throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (parameterValues != null && parameterValues.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, parameterValues);
				this.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, spParameterSet);
				return;
			}
			this.FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
		}
		private void FillDataset(IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params IDataParameter[] commandParameters)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (dataSet == null)
			{
				throw new ArgumentNullException("dataSet");
			}
			IDbCommand dbCommand = connection.CreateCommand();
			bool flag = false;
			this.PrepareCommand(dbCommand, connection, transaction, commandType, commandText, commandParameters, out flag);
			IDbDataAdapter dbDataAdapter = null;
			try
			{
				dbDataAdapter = this.GetDataAdapter();
				dbDataAdapter.SelectCommand = dbCommand;
				if (tableNames != null && tableNames.Length > 0)
				{
					string str = "Table";
					for (int i = 0; i < tableNames.Length; i++)
					{
						if (tableNames[i] == null || tableNames[i].Length == 0)
						{
							throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
						}
						dbDataAdapter.TableMappings.Add(str + ((i == 0) ? "" : i.ToString()), tableNames[i]);
					}
				}
				dbDataAdapter.Fill(dataSet);
				dbCommand.Parameters.Clear();
			}
			finally
			{
				IDisposable disposable = dbDataAdapter as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			if (flag)
			{
				connection.Close();
			}
		}
		public virtual void UpdateDataset(IDbCommand insertCommand, IDbCommand deleteCommand, IDbCommand updateCommand, DataSet dataSet, string tableName)
		{
			if (insertCommand == null)
			{
				throw new ArgumentNullException("insertCommand");
			}
			if (deleteCommand == null)
			{
				throw new ArgumentNullException("deleteCommand");
			}
			if (updateCommand == null)
			{
				throw new ArgumentNullException("updateCommand");
			}
			if (tableName == null || tableName.Length == 0)
			{
				throw new ArgumentNullException("tableName");
			}
			IDbDataAdapter dbDataAdapter = null;
			try
			{
				bool flag = false;
				dbDataAdapter = this.GetDataAdapter();
				IDataParameter[] array = new IDataParameter[updateCommand.Parameters.Count];
				updateCommand.Parameters.CopyTo(array, 0);
				updateCommand.Parameters.Clear();
				this.PrepareCommand(updateCommand, updateCommand.Connection, null, updateCommand.CommandType, updateCommand.CommandText, array, out flag);
				dbDataAdapter.UpdateCommand = updateCommand;
				array = new IDataParameter[insertCommand.Parameters.Count];
				insertCommand.Parameters.CopyTo(array, 0);
				insertCommand.Parameters.Clear();
				this.PrepareCommand(insertCommand, insertCommand.Connection, null, insertCommand.CommandType, insertCommand.CommandText, array, out flag);
				dbDataAdapter.InsertCommand = insertCommand;
				array = new IDataParameter[deleteCommand.Parameters.Count];
				deleteCommand.Parameters.CopyTo(array, 0);
				deleteCommand.Parameters.Clear();
				this.PrepareCommand(deleteCommand, deleteCommand.Connection, null, deleteCommand.CommandType, deleteCommand.CommandText, array, out flag);
				dbDataAdapter.DeleteCommand = deleteCommand;
				if (dbDataAdapter is DbDataAdapter)
				{
					((DbDataAdapter)dbDataAdapter).Update(dataSet, tableName);
				}
				else
				{
					dbDataAdapter.TableMappings.Add(tableName, "Table");
					dbDataAdapter.Update(dataSet);
				}
				dataSet.AcceptChanges();
			}
			finally
			{
				IDisposable disposable = dbDataAdapter as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
		public virtual IDbCommand CreateCommand(IDbConnection connection, string spName, params string[] sourceColumns)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			IDbCommand dbCommand = connection.CreateCommand();
			dbCommand.CommandText = spName;
			dbCommand.CommandType = CommandType.StoredProcedure;
			if (sourceColumns != null && sourceColumns.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				for (int i = 0; i < sourceColumns.Length; i++)
				{
					spParameterSet[i].SourceColumn = sourceColumns[i];
				}
				this.AttachParameters(dbCommand, spParameterSet);
			}
			return dbCommand;
		}
		public virtual int ExecuteNonQueryTypedParams(string connectionString, string spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual int ExecuteNonQueryTypedParams(IDbConnection connection, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
		}
		public virtual int ExecuteNonQueryTypedParams(IDbTransaction transaction, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual DataSet ExecuteDatasetTypedParams(string connectionString, string spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual DataSet ExecuteDatasetTypedParams(IDbConnection connection, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
		}
		public virtual DataSet ExecuteDatasetTypedParams(IDbTransaction transaction, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual IDataReader ExecuteReaderTypedParams(string connectionString, string spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual IDataReader ExecuteReaderTypedParams(IDbConnection connection, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteReader(connection, CommandType.StoredProcedure, spName);
		}
		public virtual IDataReader ExecuteReaderTypedParams(IDbTransaction transaction, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual object ExecuteScalarTypedParams(string connectionString, string spName, DataRow dataRow)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			if (dataRow != null && dataRow.ItemArray.Length > 0)
			{
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connectionString, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
		}
		public virtual object ExecuteScalarTypedParams(IDbConnection connection, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
		}
		public virtual object ExecuteScalarTypedParams(IDbTransaction transaction, string spName, DataRow dataRow)
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
				IDataParameter[] spParameterSet = this.GetSpParameterSet(transaction.Connection, spName);
				this.AssignParameterValues(spParameterSet, dataRow);
				return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
			}
			return this.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
		}
		public virtual IDataParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return this.GetSpParameterSet(connectionString, spName, false);
		}
		public virtual IDataParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				throw new ArgumentNullException("connectionString");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			IDataParameter[] spParameterSetInternal;
			using (IDbConnection connection = this.GetConnection(connectionString))
			{
				spParameterSetInternal = this.GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
			}
			return spParameterSetInternal;
		}
		public virtual IDataParameter[] GetSpParameterSet(IDbConnection connection, string spName)
		{
			return this.GetSpParameterSet(connection, spName, false);
		}
		public virtual IDataParameter[] GetSpParameterSet(IDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (!(connection is ICloneable))
			{
				throw new ArgumentException("can磘 discover parameters if the connection doesn磘 implement the ICloneable interface", "connection");
			}
			IDbConnection connection2 = (IDbConnection)((ICloneable)connection).Clone();
			return this.GetSpParameterSetInternal(connection2, spName, includeReturnValueParameter);
		}
		private IDataParameter[] GetSpParameterSetInternal(IDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
			IDataParameter[] array = ADOHelperParameterCache.GetCachedParameterSet(connection.ConnectionString, spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : ""));
			if (array == null)
			{
				IDataParameter[] array2 = this.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				ADOHelperParameterCache.CacheParameterSet(connection.ConnectionString, spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : ""), array2);
				array = ADOHelperParameterCache.CloneParameters(array2);
			}
			return array;
		}
		private IDataParameter[] DiscoverSpParameterSet(IDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (spName == null || spName.Length == 0)
			{
				throw new ArgumentNullException("spName");
			}
			IDbCommand dbCommand = connection.CreateCommand();
			dbCommand.CommandText = spName;
			dbCommand.CommandType = CommandType.StoredProcedure;
			connection.Open();
			this.DeriveParameters(dbCommand);
			connection.Close();
			if (!includeReturnValueParameter)
			{
				dbCommand.Parameters.RemoveAt(0);
			}
			IDataParameter[] array = new IDataParameter[dbCommand.Parameters.Count];
			dbCommand.Parameters.CopyTo(array, 0);
			IDataParameter[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				IDataParameter dataParameter = array2[i];
				dataParameter.Value = DBNull.Value;
			}
			return array;
		}
		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
