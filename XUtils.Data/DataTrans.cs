using System;
using System.Collections.Generic;
using System.Data;
using XUtils.Messages;
using XUtils.ValidationSupport;
namespace XUtils.Data
{
	public static class DataTrans
	{
		public static BoolResult<bool> RunTransaction(this DataBase db, Action<IDbTransaction> action)
		{
			ValidationResults validationResults = new ValidationResults();
			IDbConnection connection = db.GetConnection(db.ConnectionString);
			connection.Open();
			IDbTransaction dbTransaction = connection.BeginTransaction();
			try
			{
				action(dbTransaction);
				dbTransaction.Commit();
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (connection != null || connection.State == ConnectionState.Open)
				{
					connection.Close();
					connection.Dispose();
				}
			}
			return new BoolResult<bool>(validationResults.IsValid, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<bool> RunTransaction(this DataBase db, IList<Transaction> trans)
		{
			return db.RunTransaction(delegate(IDbTransaction transaction)
			{
				foreach (Transaction current in trans)
				{
					switch (current.TransType)
					{
					case TransType.Text:
						db.ExecuteNonQuery(transaction, CommandType.Text, current.CommandText, current.Parameters);
						break;
					case TransType.SP:
						db.ExecuteNonQuery(transaction, current.CommandText, current.Parameters);
						break;
					}
				}
			});
		}
	}
}
