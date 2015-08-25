using System;
using System.Collections.Generic;
using System.Data;
using XUtils.Messages;
using XUtils.ValidationSupport;
namespace XUtils.Data
{
	public static class DataQuery
	{
		public static BoolResult<TEntity> RefToSingle<TEntity>(this DataBase db, string strSql) where TEntity : class, new()
		{
            return db.RefToSingle<TEntity>(strSql, null);
		}
		public static BoolResult<TEntity> RefToSingle<TEntity>(this DataBase db, string strSql, params IDataParameter[] parameters) where TEntity : class, new()
		{
			ValidationResults validationResults = new ValidationResults();
			TEntity item = default(TEntity);
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				DynamicBuilder<TEntity> dynamicBuilder = DynamicBuilder<TEntity>.CreateBuilder(dataReader);
				if (dataReader.Read())
				{
					item = dynamicBuilder.Build(dataReader);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<TEntity>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<TEntity> ToSingle<TEntity>(this DataBase db, string strSql, Func<IDataReader, TEntity> func)
		{
			return db.ToSingle(strSql, func, null);
		}
		public static BoolResult<TEntity> ToSingle<TEntity>(this DataBase db, string strSql, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(func, "Func<IDataReader, TEntity> is null");
			ValidationResults validationResults = new ValidationResults();
			TEntity item = default(TEntity);
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				if (dataReader.Read())
				{
					item = func(dataReader);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<TEntity>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static void ToSingle(this DataBase db, string strSql, Action<IDataReader> action)
		{
			db.ToSingle(strSql, action, null);
		}
		public static void ToSingle(this DataBase db, string strSql, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(action, "Action<IDataReader> is null");
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				if (dataReader.Read())
				{
					action(dataReader);
				}
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
		}
		public static BoolResult<TEntity> ToSingle<TEntity>(this DataBase db, string strSql, IRowMapper<IDataReader, TEntity> rowMapper) where TEntity : class, new()
		{
			return db.ToSingle(strSql, rowMapper, null);
		}
		public static BoolResult<TEntity> ToSingle<TEntity>(this DataBase db, string strSql, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters) where TEntity : class, new()
		{
			Guard.IsNotNull(rowMapper, "IRowMapper is null");
			ValidationResults validationResults = new ValidationResults();
			TEntity item = default(TEntity);
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				if (dataReader.Read())
				{
					item = rowMapper.MapRow(dataReader, 0);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<TEntity>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<TEntity> RefSPToSingle<TEntity>(this DataBase db, string spName) where TEntity : class, new()
		{
            return db.RefSPToSingle<TEntity>(spName, null);
		}
		public static BoolResult<TEntity> RefSPToSingle<TEntity>(this DataBase db, string spName, params IDataParameter[] parameters) where TEntity : class, new()
		{
			ValidationResults validationResults = new ValidationResults();
			TEntity item = default(TEntity);
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				DynamicBuilder<TEntity> dynamicBuilder = DynamicBuilder<TEntity>.CreateBuilder(dataReader);
				if (dataReader.Read())
				{
					item = dynamicBuilder.Build(dataReader);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<TEntity>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<TEntity> SPToSingle<TEntity>(this DataBase db, string spName, Func<IDataReader, TEntity> func)
		{
			return db.SPToSingle(spName, func, null);
		}
		public static BoolResult<TEntity> SPToSingle<TEntity>(this DataBase db, string spName, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(func, "Func<IDataReader, TEntity> is null");
			ValidationResults validationResults = new ValidationResults();
			TEntity item = default(TEntity);
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				if (dataReader.Read())
				{
					item = func(dataReader);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<TEntity>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static void SPToSingle(this DataBase db, string spName, Action<IDataReader> action)
		{
			db.SPToSingle(spName, action, null);
		}
		public static void SPToSingle(this DataBase db, string spName, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(action, "Action<IDataReader> is null");
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				if (dataReader.Read())
				{
					action(dataReader);
				}
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
		}
		public static BoolResult<TEntity> SPToSingle<TEntity>(this DataBase db, string spName, IRowMapper<IDataReader, TEntity> rowMapper) where TEntity : class, new()
		{
			return db.SPToSingle(spName, rowMapper, null);
		}
		public static BoolResult<TEntity> SPToSingle<TEntity>(this DataBase db, string spName, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters) where TEntity : class, new()
		{
			Guard.IsNotNull(rowMapper, "IRowMapper is null");
			ValidationResults validationResults = new ValidationResults();
			TEntity item = default(TEntity);
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				if (dataReader.Read())
				{
					item = rowMapper.MapRow(dataReader, 0);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<TEntity>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<IList<TEntity>> RefToList<TEntity>(this DataBase db, string strSql) where TEntity : class, new()
		{
			return db.RefToList<TEntity>(strSql, null);
		}
		public static BoolResult<IList<TEntity>> RefToList<TEntity>(this DataBase db, string strSql, params IDataParameter[] parameters) where TEntity : class, new()
		{
			ValidationResults validationResults = new ValidationResults();
			IList<TEntity> list = new List<TEntity>();
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				DynamicBuilder<TEntity> dynamicBuilder = DynamicBuilder<TEntity>.CreateBuilder(dataReader);
				while (dataReader.Read())
				{
					TEntity item = dynamicBuilder.Build(dataReader);
					list.Add(item);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<IList<TEntity>>(list, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<IList<TEntity>> ToList<TEntity>(this DataBase db, string strSql, Func<IDataReader, TEntity> func)
		{
			return db.ToList(strSql, func, null);
		}
		public static BoolResult<IList<TEntity>> ToList<TEntity>(this DataBase db, string strSql, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(func, "Func<IDataReader, TEntity> is null");
			ValidationResults validationResults = new ValidationResults();
			IList<TEntity> list = new List<TEntity>();
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				while (dataReader.Read())
				{
					list.Add(func(dataReader));
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<IList<TEntity>>(list, validationResults.IsValid, string.Empty, validationResults);
		}
		public static void ToList(this DataBase db, string strSql, Action<IDataReader> action)
		{
			db.ToList(strSql, action, null);
		}
		public static void ToList(this DataBase db, string strSql, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(action, "Action<IDataReader> is null");
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				while (dataReader.Read())
				{
					action(dataReader);
				}
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
		}
		public static BoolResult<IList<TEntity>> ToList<TEntity>(this DataBase db, string strSql, IRowMapper<IDataReader, TEntity> rowMapper) where TEntity : class, new()
		{
			return db.ToList(strSql, rowMapper, null);
		}
		public static BoolResult<IList<TEntity>> ToList<TEntity>(this DataBase db, string strSql, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters) where TEntity : class, new()
		{
			Guard.IsNotNull(rowMapper, "IRowMapper is null");
			ValidationResults validationResults = new ValidationResults();
			IList<TEntity> item = new List<TEntity>();
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteReader(db.ConnectionString, CommandType.Text, strSql, parameters);
				item = rowMapper.MapRows(dataReader);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<IList<TEntity>>(item, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<IList<TEntity>> RefSPToList<TEntity>(this DataBase db, string spName) where TEntity : class, new()
		{
            return db.RefSPToList<TEntity>(spName, null);
		}
		public static BoolResult<IList<TEntity>> RefSPToList<TEntity>(this DataBase db, string spName, params IDataParameter[] parameters) where TEntity : class, new()
		{
			ValidationResults validationResults = new ValidationResults();
			IList<TEntity> list = new List<TEntity>();
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				DynamicBuilder<TEntity> dynamicBuilder = DynamicBuilder<TEntity>.CreateBuilder(dataReader);
				while (dataReader.Read())
				{
					TEntity item = dynamicBuilder.Build(dataReader);
					list.Add(item);
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<IList<TEntity>>(list, validationResults.IsValid, string.Empty, validationResults);
		}
		public static BoolResult<IList<TEntity>> SPToList<TEntity>(this DataBase db, string spName, Func<IDataReader, TEntity> func)
		{
			return db.SPToList(spName, func, null);
		}
		public static BoolResult<IList<TEntity>> SPToList<TEntity>(this DataBase db, string spName, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(func, "Func<IDataReader, TEntity> is null");
			ValidationResults validationResults = new ValidationResults();
			IList<TEntity> list = new List<TEntity>();
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				while (dataReader.Read())
				{
					list.Add(func(dataReader));
				}
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<IList<TEntity>>(list, validationResults.IsValid, string.Empty, validationResults);
		}
		public static void SPToList(this DataBase db, string spName, Action<IDataReader> action)
		{
			db.SPToList(spName, action, null);
		}
		public static void SPToList(this DataBase db, string spName, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			Guard.IsNotNull(action, "Action<IDataReader> is null");
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				while (dataReader.Read())
				{
					action(dataReader);
				}
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
		}
		public static BoolResult<IList<TEntity>> SPToList<TEntity>(this DataBase db, string spName, IRowMapper<IDataReader, TEntity> rowMapper) where TEntity : class, new()
		{
			return db.SPToList(spName, rowMapper, null);
		}
		public static BoolResult<IList<TEntity>> SPToList<TEntity>(this DataBase db, string spName, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters) where TEntity : class, new()
		{
			Guard.IsNotNull(rowMapper, "IRowMapper is null");
			ValidationResults validationResults = new ValidationResults();
			IList<TEntity> item = new List<TEntity>();
			IDataReader dataReader = null;
			try
			{
				dataReader = db.ExecuteSPReader(db.ConnectionString, spName, parameters);
				item = rowMapper.MapRows(dataReader);
			}
			catch (Exception ex)
			{
				validationResults.Add(ex.Message);
			}
			finally
			{
				if (dataReader != null)
				{
					dataReader.Close();
				}
			}
			return new BoolResult<IList<TEntity>>(item, validationResults.IsValid, string.Empty, validationResults);
		}
	}
}
