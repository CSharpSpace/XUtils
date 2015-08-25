using System;
using System.Data;
using XUtils.Messages;
using XUtils.ValidationSupport;
namespace XUtils.Data
{
	public static class DataExecute
	{
		public static BoolResult<DataSet> ToDataSet(this DataBase db, string strSql)
		{
			return db.ToDataSet(strSql, null);
		}
		public static BoolResult<DataSet> ToDataSet(this DataBase db, string strSql, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			DataSet item = null;
			try
			{
				item = db.ExecuteDataset(db.ConnectionString, CommandType.Text, strSql, parameters);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<DataSet>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<DataTable> ToDataTable(this DataBase db, string strSql)
		{
			return db.ToDataTable(strSql, null);
		}
		public static BoolResult<DataTable> ToDataTable(this DataBase db, string strSql, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			DataTable item = null;
			try
			{
				item = db.ExecuteDataset(db.ConnectionString, CommandType.Text, strSql, parameters).Tables[0];
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<DataTable>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<TType> Scalar<TType>(this DataBase db, string strSql)
		{
            return db.Scalar<TType>(strSql, null);
		}
		public static BoolResult<TType> Scalar<TType>(this DataBase db, string strSql, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			TType item = default(TType);
			try
			{
				object input = db.ExecuteScalar(db.ConnectionString, CommandType.Text, strSql, parameters);
				item = TypeParsers.ConvertTo<TType>(input);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<TType>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static TType Scalar<TType>(this DataBase db, IDbTransaction tran, string strSql)
		{
            return db.Scalar<TType>(tran, strSql, null);
		}
		public static TType Scalar<TType>(this DataBase db, IDbTransaction tran, string strSql, params IDataParameter[] parameters)
		{
			TType tType = default(TType);
			object input = db.ExecuteScalar(tran, CommandType.Text, strSql, parameters);
			return TypeParsers.ConvertTo<TType>(input);
		}
		public static BoolResult<int> NonQuery(this DataBase db, string strSql)
		{
			return db.NonQuery(strSql, null);
		}
		public static BoolResult<int> NonQuery(this DataBase db, string strSql, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			int item = -1;
			try
			{
				item = db.ExecuteNonQuery(db.ConnectionString, CommandType.Text, strSql, parameters);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<int>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static int NonQuery(this DataBase db, IDbTransaction tran, string strSql)
		{
			return db.NonQuery(tran, strSql, null);
		}
		public static int NonQuery(this DataBase db, IDbTransaction tran, string strSql, params IDataParameter[] parameters)
		{
			return db.ExecuteNonQuery(tran, CommandType.Text, strSql, parameters);
		}
		public static BoolResult<bool> Scalar(this DataBase db, string strSql)
		{
			return db.Scalar(strSql, null);
		}
		public static BoolResult<bool> Scalar(this DataBase db, string strSql, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			bool item = false;
			try
			{
				object obj = db.ExecuteScalar(db.ConnectionString, CommandType.Text, strSql, parameters);
				item = (obj != null && obj != DBNull.Value && int.Parse(obj.ToString()) > 0);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<bool>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static bool Scalar(this DataBase db, IDbTransaction tran, string strSql)
		{
			return db.Scalar(tran, strSql, null);
		}
		public static bool Scalar(this DataBase db, IDbTransaction tran, string strSql, params IDataParameter[] parameters)
		{
			object obj = db.ExecuteScalar(db.ConnectionString, CommandType.Text, strSql, parameters);
			return obj != null && obj != DBNull.Value && int.Parse(obj.ToString()) > 0;
		}
		public static BoolResult<DataSet> SPToDataSet(this DataBase db, string spName)
		{
			return db.SPToDataSet(spName, null);
		}
		public static BoolResult<DataSet> SPToDataSet(this DataBase db, string spName, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			DataSet item = null;
			try
			{
				item = db.ExecuteDataset(db.ConnectionString, spName, parameters);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<DataSet>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<DataTable> SPToDataTable(this DataBase db, string spName)
		{
			return db.SPToDataTable(spName, null);
		}
		public static BoolResult<DataTable> SPToDataTable(this DataBase db, string spName, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			DataTable item = null;
			try
			{
				item = db.ExecuteDataset(db.ConnectionString, spName, parameters).Tables[0];
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<DataTable>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<int> SPNonQuery(this DataBase db, string spName)
		{
			return db.SPNonQuery(spName, null);
		}
		public static BoolResult<int> SPNonQuery(this DataBase db, string spName, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			int item = -1;
			try
			{
				item = db.ExecuteSPNonQuery(db.ConnectionString, spName, parameters);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<int>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static int SPNonQuery(this DataBase db, IDbTransaction tran, string spName)
		{
			return db.SPNonQuery(tran, spName, null);
		}
		public static int SPNonQuery(this DataBase db, IDbTransaction tran, string spName, params IDataParameter[] parameters)
		{
			return db.ExecuteSPNonQuery(tran, spName, parameters);
		}
		public static BoolResult<TType> SPScalar<TType>(this DataBase db, string spName)
		{
            return db.SPScalar<TType>(spName, null);
		}
		public static BoolResult<TType> SPScalar<TType>(this DataBase db, string spName, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			TType item = default(TType);
			try
			{
				object input = db.ExecuteScalar(db.ConnectionString, spName, parameters);
				item = TypeParsers.ConvertTo<TType>(input);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<TType>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static TType SPScalar<TType>(this DataBase db, IDbTransaction tran, string spName)
		{
            return db.SPScalar<TType>(tran, spName, null);
		}
		public static TType SPScalar<TType>(this DataBase db, IDbTransaction tran, string spName, params IDataParameter[] parameters)
		{
			object input = db.ExecuteScalar(tran, spName, parameters);
			return TypeParsers.ConvertTo<TType>(input);
		}
		public static BoolResult<bool> SPScalar(this DataBase db, string spName)
		{
			return db.SPScalar(spName, null);
		}
		public static BoolResult<bool> SPScalar(this DataBase db, string spName, params IDataParameter[] parameters)
		{
			ValidationResults validationResults = new ValidationResults();
			bool item = false;
			try
			{
				object obj = db.ExecuteScalar(db.ConnectionString, spName, parameters);
				item = (obj != null && obj != DBNull.Value && int.Parse(obj.ToString()) > 0);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			return new BoolResult<bool>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static bool SPScalar(this DataBase db, IDbTransaction tran, string spName)
		{
			return db.SPScalar(tran, spName, null);
		}
		public static bool SPScalar(this DataBase db, IDbTransaction tran, string spName, params IDataParameter[] parameters)
		{
			object obj = db.ExecuteScalar(db.ConnectionString, spName, parameters);
			return obj != null && obj != DBNull.Value && int.Parse(obj.ToString()) > 0;
		}
	}
}
