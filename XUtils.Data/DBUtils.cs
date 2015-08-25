using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XUtils.Messages;
namespace XUtils.Data
{
	public class DBUtils<TEntity> where TEntity : class, new()
	{
		private DataChange_EventHandler onChange;
		private DataBase db;
		public string TableName = string.Empty;
		public event DataChange_EventHandler OnDataChange
		{
			add
			{
				this.onChange = (DataChange_EventHandler)Delegate.Combine(this.onChange, value);
			}
			remove
			{
				this.onChange = (DataChange_EventHandler)Delegate.Remove(this.onChange, value);
			}
		}
		public DataBase DB
		{
			get
			{
				return this.db;
			}
			set
			{
				this.db = value;
			}
		}
		public static DBUtils<TEntity> New()
		{
			return new DBUtils<TEntity>();
		}
		public DBUtils()
		{
			this.TableName = typeof(TEntity).Name;
		}
		public BoolResult<bool> Exists(string where)
		{
			return this.Exists(where, null);
		}
		public BoolResult<bool> Exists(string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT COUNT(1) FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.Scalar(stringBuilder.ToString(), parameters);
		}
		public BoolResult<TType> Max<TType>(string field)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT MAX({0}) FROM {1}", field, this.TableName);
            return this.db.Scalar<TType>(stringBuilder.ToString());
		}
		public BoolResult<TType> Max<TType>(string field, string where)
		{
			return this.Max<TType>(field, where, null);
		}
		public BoolResult<TType> Max<TType>(string field, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT MAX({0}) FROM {1} WHERE {0} ", field, this.TableName, where);
            return this.db.Scalar<TType>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<TType> Sum<TType>(string field)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT MAX({0}) FROM {1}", field, this.TableName);
            return this.db.Scalar<TType>(stringBuilder.ToString());
		}
		public BoolResult<TType> Sum<TType>(string field, string where)
		{
			return this.Sum<TType>(field, where, null);
		}
		public BoolResult<TType> Sum<TType>(string field, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT SUM({0}) FROM {1} WHERE {0} ", field, this.TableName, where);
            return this.db.Scalar<TType>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<int> Count()
		{
			return this.Count(string.Empty, null);
		}
		public BoolResult<int> Count(string where)
		{
			return this.Count(where, null);
		}
		public BoolResult<int> Count(string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT COUNT(1) FROM {0} ", this.TableName);
			if (!string.IsNullOrEmpty(where))
			{
				stringBuilder.AppendFormat("WHERE {0}", where);
			}
            return this.db.Scalar<int>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<int> MultiCount(string from)
		{
			return this.MultiCount(from, string.Empty, null);
		}
		public BoolResult<int> MultiCount(string from, string where)
		{
			return this.MultiCount(from, where, null);
		}
		public BoolResult<int> MultiCount(string from, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT COUNT(1) FROM {0} ", from);
			if (!string.IsNullOrEmpty(where))
			{
				stringBuilder.AppendFormat("WHERE {0}", where);
			}
            return this.db.Scalar<int>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<DataTable> ToTable()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} ", this.TableName);
			return this.db.ToDataTable(stringBuilder.ToString());
		}
		public BoolResult<DataTable> ToTable(string where)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToTable(where, parameters);
		}
		public BoolResult<DataTable> ToTable(string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.ToDataTable(stringBuilder.ToString(), parameters);
		}
		public BoolResult<DataTable> ToTable(string where, string orderby)
		{
			return this.ToTable(where, orderby, null);
		}
		public BoolResult<DataTable> ToTable(string where, string orderby, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ORDER BY {2}", this.TableName, where, orderby);
			return this.db.ToDataTable(stringBuilder.ToString(), parameters);
		}
		public BoolResult<DataTable> ToMultiTable(string fields, string from)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {1} FROM {0} ", from, fields);
			return this.db.ToDataTable(stringBuilder.ToString());
		}
		public BoolResult<DataTable> ToMultiTable(string fields, string from, string where)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiTable(fields, from, where, parameters);
		}
		public BoolResult<DataTable> ToMultiTable(string fields, string from, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			return this.db.ToDataTable(stringBuilder.ToString(), parameters);
		}
		public BoolResult<DataTable> ToMultiTable(string fields, string from, string where, string orderby)
		{
			return this.ToMultiTable(fields, from, where, orderby, null);
		}
		public BoolResult<DataTable> ToMultiTable(string fields, string from, string where, string orderby, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {3} FROM {0} WHERE {1} ORDER BY {2}", new object[]
			{
				from,
				where,
				orderby,
				fields
			});
			return this.db.ToDataTable(stringBuilder.ToString(), parameters);
		}
		public BoolResult<TEntity> ToSingle(string where)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToSingle(where, parameters);
		}
		public BoolResult<TEntity> ToSingle(string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
            return this.db.RefToSingle<TEntity>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<TEntity> ToSingle(string where, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			return this.ToSingle(where, rowMapper, null);
		}
		public BoolResult<TEntity> ToSingle(string where, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.ToSingle(stringBuilder.ToString(), rowMapper, parameters);
		}
		public BoolResult<TEntity> ToSingle(string where, Func<IDataReader, TEntity> func)
		{
			return this.ToSingle(where, func, null);
		}
		public BoolResult<TEntity> ToSingle(string where, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.ToSingle(stringBuilder.ToString(), func, parameters);
		}
		public void ToSingle(string where, Action<IDataReader> action)
		{
			this.ToSingle(where, action, null);
		}
		public void ToSingle(string where, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			this.db.ToSingle(stringBuilder.ToString(), action, parameters);
		}
		public BoolResult<TMultiEntity> ToMultiSingle<TMultiEntity>(string fields, string from, string where) where TMultiEntity : class, new()
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiSingle<TMultiEntity>(fields, from, where, parameters);
		}
		public BoolResult<TMultiEntity> ToMultiSingle<TMultiEntity>(string fields, string from, string where, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
            return this.db.RefToSingle<TMultiEntity>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<TMultiEntity> ToMultiSingle<TMultiEntity>(string fields, string from, string where, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			return this.ToMultiSingle<TMultiEntity>(fields, from, where, rowMapper, null);
		}
		public BoolResult<TMultiEntity> ToMultiSingle<TMultiEntity>(string fields, string from, string where, IRowMapper<IDataReader, TMultiEntity> rowMapper, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			return this.db.ToSingle(stringBuilder.ToString(), rowMapper, parameters);
		}
		public BoolResult<TMultiEntity> ToMultiSingle<TMultiEntity>(string fields, string from, string where, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			return this.ToMultiSingle<TMultiEntity>(fields, from, where, func, null);
		}
		public BoolResult<TMultiEntity> ToMultiSingle<TMultiEntity>(string fields, string from, string where, Func<IDataReader, TMultiEntity> func, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			return this.db.ToSingle(stringBuilder.ToString(), func, parameters);
		}
		public void ToSingle(string fields, string from, string where, Action<IDataReader> action)
		{
			this.ToSingle(fields, from, where, action, null);
		}
		public void ToSingle(string fields, string from, string where, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			this.db.ToSingle(stringBuilder.ToString(), action, parameters);
		}
		public BoolResult<IList<TEntity>> ToList()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} ", this.TableName);
			return this.db.RefToList<TEntity>(stringBuilder.ToString());
		}
		public BoolResult<IList<TEntity>> ToList(string where)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToList(where, parameters);
		}
		public BoolResult<IList<TEntity>> ToList(string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.RefToList<TEntity>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<IList<TEntity>> ToList(string where, string orderby)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToList(where, orderby, parameters);
		}
		public BoolResult<IList<TEntity>> ToList(string where, string orderby, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ORDER BY {2}", this.TableName, where, orderby);
			return this.db.RefToList<TEntity>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {1} FROM {0} ", from, fields);
            return this.db.RefToList<TMultiEntity>(stringBuilder.ToString());
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where) where TMultiEntity : class, new()
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiList<TMultiEntity>(fields, from, where, parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
            return this.db.RefToList<TMultiEntity>(stringBuilder.ToString(), parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, string orderby) where TMultiEntity : class, new()
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiList<TMultiEntity>(fields, from, where, orderby, parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, string orderby, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {3} FROM {0} WHERE {1} ORDER BY {2}", new object[]
			{
				from,
				where,
				orderby,
				fields
			});
            return this.db.RefToList<TMultiEntity>(stringBuilder.ToString(), parameters);
		}
		public void ToList(Action<IDataReader> action)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} ", this.TableName);
			this.db.ToList(stringBuilder.ToString(), action);
		}
		public void ToList(string where, Action<IDataReader> action)
		{
			this.ToList(where, action, null);
		}
		public void ToList(string where, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			this.db.ToList(stringBuilder.ToString(), action, parameters);
		}
		public void ToList(string where, string orderby, Action<IDataReader> action)
		{
			this.ToList(where, orderby, action, null);
		}
		public void ToList(string where, string orderby, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ORDER BY {2}", this.TableName, where, orderby);
			this.db.ToList(stringBuilder.ToString(), action, parameters);
		}
		public void ToMultiList(string fields, string from, Action<IDataReader> action)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {1} FROM {0} ", from, fields);
			this.db.ToList(stringBuilder.ToString(), action);
		}
		public void ToMultiList(string fields, string from, string where, Action<IDataReader> action)
		{
			this.ToMultiList(fields, from, where, action, null);
		}
		public void ToMultiList(string fields, string from, string where, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			this.db.ToList(stringBuilder.ToString(), action, parameters);
		}
		public void ToMultiList(string fields, string from, string where, string orderby, Action<IDataReader> action)
		{
			this.ToMultiList(fields, from, where, orderby, action, null);
		}
		public void ToMultiList(string fields, string from, string where, string orderby, Action<IDataReader> action, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {3} FROM {0} WHERE {1} ORDER BY {2}", new object[]
			{
				from,
				where,
				orderby,
				fields
			});
			this.db.ToList(stringBuilder.ToString(), action, parameters);
		}
		public BoolResult<IList<TEntity>> ToList(IRowMapper<IDataReader, TEntity> rowMapper)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} ", this.TableName);
			return this.db.ToList(stringBuilder.ToString(), rowMapper);
		}
		public BoolResult<IList<TEntity>> ToList(string where, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			return this.ToList(where, rowMapper, null);
		}
		public BoolResult<IList<TEntity>> ToList(string where, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.ToList(stringBuilder.ToString(), rowMapper, parameters);
		}
		public BoolResult<IList<TEntity>> ToList(string where, string orderby, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			return this.ToList(where, orderby, rowMapper, null);
		}
		public BoolResult<IList<TEntity>> ToList(string where, string orderby, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ORDER BY {2}", this.TableName, where, orderby);
			return this.db.ToList(stringBuilder.ToString(), rowMapper, parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {1} FROM {0} ", from, fields);
			return this.db.ToList(stringBuilder.ToString(), rowMapper);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			return this.ToMultiList<TMultiEntity>(fields, from, where, rowMapper, null);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, IRowMapper<IDataReader, TMultiEntity> rowMapper, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			return this.db.ToList(stringBuilder.ToString(), rowMapper, parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, string orderby, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			return this.ToMultiList<TMultiEntity>(fields, from, where, orderby, rowMapper, null);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, string orderby, IRowMapper<IDataReader, TMultiEntity> rowMapper, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {3} FROM {0} WHERE {1} ORDER BY {2}", new object[]
			{
				from,
				where,
				orderby,
				fields
			});
			return this.db.ToList(stringBuilder.ToString(), rowMapper, parameters);
		}
		public BoolResult<IList<TEntity>> ToList(Func<IDataReader, TEntity> func)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} ", this.TableName);
			return this.db.ToList(stringBuilder.ToString(), func);
		}
		public BoolResult<IList<TEntity>> ToList(string where, Func<IDataReader, TEntity> func)
		{
			return this.ToList(where, func, null);
		}
		public BoolResult<IList<TEntity>> ToList(string where, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ", this.TableName, where);
			return this.db.ToList(stringBuilder.ToString(), func, parameters);
		}
		public BoolResult<IList<TEntity>> ToList(string where, string orderby, Func<IDataReader, TEntity> func)
		{
			return this.ToList(where, orderby, func, null);
		}
		public BoolResult<IList<TEntity>> ToList(string where, string orderby, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT * FROM {0} WHERE {1} ORDER BY {2}", this.TableName, where, orderby);
			return this.db.ToList(stringBuilder.ToString(), func, parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {1} FROM {0} ", from, fields);
			return this.db.ToList(stringBuilder.ToString(), func);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			return this.ToMultiList<TMultiEntity>(fields, from, where, func, null);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, Func<IDataReader, TMultiEntity> func, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {2} FROM {0} WHERE {1} ", from, where, fields);
			return this.db.ToList(stringBuilder.ToString(), func, parameters);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, string orderby, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			return this.ToMultiList<TMultiEntity>(fields, from, where, orderby, func, null);
		}
		public BoolResult<IList<TMultiEntity>> ToMultiList<TMultiEntity>(string fields, string from, string where, string orderby, Func<IDataReader, TMultiEntity> func, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT {3} FROM {0} WHERE {1} ORDER BY {2}", new object[]
			{
				from,
				where,
				orderby,
				fields
			});
			return this.db.ToList(stringBuilder.ToString(), func, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, string.Empty, string.Empty);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, string.Empty);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, params IDataParameter[] parameters)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, string.Empty, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, orderby);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, string orderby, params IDataParameter[] parameters)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, orderby, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToPagedList(fields, pkField, pageSize, pageNumber, where, orderby, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby, params IDataParameter[] parameters)
		{
			string text = this.ToPagedSql(fields, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.Count(where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TEntity>> result = this.db.RefToList<TEntity>(text.ToString(), parameters);
			PagedList<TEntity> item = new PagedList<TEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, string.Empty, string.Empty, rowMapper);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, string.Empty, rowMapper);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, string.Empty, rowMapper, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, string orderby, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, orderby, rowMapper);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, string orderby, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, orderby, rowMapper, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby, IRowMapper<IDataReader, TEntity> rowMapper)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToPagedList(fields, pkField, pageSize, pageNumber, where, orderby, rowMapper, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby, IRowMapper<IDataReader, TEntity> rowMapper, params IDataParameter[] parameters)
		{
			string text = this.ToPagedSql(fields, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.Count(where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TEntity>> result = this.db.ToList(text.ToString(), rowMapper, parameters);
			PagedList<TEntity> item = new PagedList<TEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, Func<IDataReader, TEntity> func)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, string.Empty, string.Empty, func);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, Func<IDataReader, TEntity> func)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, string.Empty, func);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, string.Empty, func, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, string orderby, Func<IDataReader, TEntity> func)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, orderby, func);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string pkField, int pageSize, int pageNumber, string where, string orderby, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			return this.ToPagedList("*", pkField, pageSize, pageNumber, where, orderby, func, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby, Func<IDataReader, TEntity> func)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToPagedList(fields, pkField, pageSize, pageNumber, where, orderby, func, parameters);
		}
		public BoolResult<PagedList<TEntity>> ToPagedList(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby, Func<IDataReader, TEntity> func, params IDataParameter[] parameters)
		{
			string text = this.ToPagedSql(fields, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.Count(where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TEntity>> result = this.db.ToList(text.ToString(), func, parameters);
			PagedList<TEntity> item = new PagedList<TEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string pkField, int pageSize, int pageNumber)
		{
			return this.ToPagedTable("*", pkField, pageSize, pageNumber, string.Empty, string.Empty);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string pkField, int pageSize, int pageNumber, string where)
		{
			return this.ToPagedTable("*", pkField, pageSize, pageNumber, where, string.Empty);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string pkField, int pageSize, int pageNumber, string where, params IDataParameter[] parameters)
		{
			return this.ToPagedTable("*", pkField, pageSize, pageNumber, where, string.Empty, parameters);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			return this.ToPagedTable("*", pkField, pageSize, pageNumber, where, orderby);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string pkField, int pageSize, int pageNumber, string where, string orderby, params IDataParameter[] parameters)
		{
			return this.ToPagedTable("*", pkField, pageSize, pageNumber, where, orderby, parameters);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToPagedTable(fields, pkField, pageSize, pageNumber, where, orderby, parameters);
		}
		public BoolResult<Paged<DataTable>> ToPagedTable(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby, params IDataParameter[] parameters)
		{
			string text = this.ToPagedSql(fields, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.Count(where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<Paged<DataTable>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<DataTable> result = this.db.ToDataTable(text.ToString(), parameters);
			Paged<DataTable> item = new Paged<DataTable>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<Paged<DataTable>>(item, result.Errors.IsValid, "", result.Errors);
		}
		private string ToPagedSql(string fields, string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT TOP {0} {1} ", pageSize, fields);
			stringBuilder.AppendFormat("FROM {0} ", this.TableName);
			if (!string.IsNullOrEmpty(where))
			{
				stringBuilder.AppendFormat("WHERE {0} AND ", where);
			}
			else
			{
				stringBuilder.Append("WHERE ");
			}
			stringBuilder.AppendFormat("{0} NOT IN (SELECT TOP {2} {0} FROM {1} ", pkField, this.TableName, pageSize * (pageNumber - 1));
			if (!string.IsNullOrEmpty(where))
			{
				stringBuilder.AppendFormat("WHERE {0} ", where);
			}
			if (!string.IsNullOrEmpty(orderby))
			{
				stringBuilder.AppendFormat("ORDER BY {0}) ORDER BY {0}", orderby);
			}
			else
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, string.Empty, string.Empty);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty, parameters);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby) where TMultiEntity : class, new()
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, orderby, parameters);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			string text = this.ToMultiPagedSql(fields, from, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.MultiCount(from, where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TMultiEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
            BoolResult<IList<TMultiEntity>> result = this.db.RefToList<TMultiEntity>(text.ToString(), parameters);
			PagedList<TMultiEntity> item = new PagedList<TMultiEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TMultiEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, string.Empty, string.Empty, rowMapper);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty, rowMapper);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, IRowMapper<IDataReader, TMultiEntity> rowMapper, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty, rowMapper, parameters);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby, IRowMapper<IDataReader, TMultiEntity> rowMapper) where TMultiEntity : class, new()
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, orderby, rowMapper, parameters);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby, IRowMapper<IDataReader, TMultiEntity> rowMapper, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			string text = this.ToMultiPagedSql(fields, from, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.MultiCount(from, where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TMultiEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TMultiEntity>> result = this.db.ToList(text.ToString(), rowMapper, parameters);
			PagedList<TMultiEntity> item = new PagedList<TMultiEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TMultiEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, string.Empty, string.Empty, func);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty, func);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, Func<IDataReader, TMultiEntity> func, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty, func, parameters);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby, Func<IDataReader, TMultiEntity> func) where TMultiEntity : class, new()
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiPagedList<TMultiEntity>(fields, from, pkField, pageSize, pageNumber, where, string.Empty, func, parameters);
		}
		public BoolResult<PagedList<TMultiEntity>> ToMultiPagedList<TMultiEntity>(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby, Func<IDataReader, TMultiEntity> func, params IDataParameter[] parameters) where TMultiEntity : class, new()
		{
			string text = this.ToMultiPagedSql(fields, from, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.MultiCount(from, where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TMultiEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TMultiEntity>> result = this.db.ToList(text.ToString(), func, parameters);
			PagedList<TMultiEntity> item = new PagedList<TMultiEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TMultiEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public BoolResult<Paged<DataTable>> ToMultiPagedTable(string fields, string from, string pkField, int pageSize, int pageNumber)
		{
			return this.ToMultiPagedTable(fields, from, pkField, pageSize, pageNumber, string.Empty, string.Empty);
		}
		public BoolResult<Paged<DataTable>> ToMultiPagedTable(string fields, string from, string pkField, int pageSize, int pageNumber, string where)
		{
			return this.ToMultiPagedTable(fields, from, pkField, pageSize, pageNumber, where, string.Empty);
		}
		public BoolResult<Paged<DataTable>> ToMultiPagedTable(string fields, string from, string pkField, int pageSize, int pageNumber, string where, params IDataParameter[] parameters)
		{
			return this.ToMultiPagedTable(fields, from, pkField, pageSize, pageNumber, where, string.Empty, parameters);
		}
		public BoolResult<Paged<DataTable>> ToMultiPagedTable(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			IDataParameter[] parameters = new IDataParameter[0];
			return this.ToMultiPagedTable(fields, from, pkField, pageSize, pageNumber, where, orderby, parameters);
		}
		public BoolResult<Paged<DataTable>> ToMultiPagedTable(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby, params IDataParameter[] parameters)
		{
			string text = this.ToMultiPagedSql(fields, from, pkField, pageSize, pageNumber, where, orderby);
			BoolResult<int> boolResult = this.MultiCount(from, where, parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<Paged<DataTable>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<DataTable> result = this.db.ToDataTable(text.ToString(), parameters);
			Paged<DataTable> item = new Paged<DataTable>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<Paged<DataTable>>(item, result.Errors.IsValid, "", result.Errors);
		}
		private string ToMultiPagedSql(string fields, string from, string pkField, int pageSize, int pageNumber, string where, string orderby)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT TOP {0} {1} ", pageSize, fields);
			stringBuilder.AppendFormat("FROM {0} ", from);
			if (!string.IsNullOrEmpty(where))
			{
				stringBuilder.AppendFormat("WHERE {0} AND ", where);
			}
			else
			{
				stringBuilder.Append("WHERE ");
			}
			stringBuilder.AppendFormat("{0} NOT IN (SELECT TOP {2} {0} FROM {1} ", pkField, from, pageSize * (pageNumber - 1));
			if (!string.IsNullOrEmpty(where))
			{
				stringBuilder.AppendFormat("WHERE {0} ", where);
			}
			if (!string.IsNullOrEmpty(orderby))
			{
				stringBuilder.AppendFormat("ORDER BY {0}) ORDER BY {0}", orderby);
			}
			else
			{
				stringBuilder.Append(")");
			}
			return stringBuilder.ToString();
		}
		public BoolResult<int> Insert(string columns, string values)
		{
			return this.Insert(columns, values, null);
		}
		public BoolResult<int> Insert(string columns, string values, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("INSERT INTO {0} ", this.TableName);
			stringBuilder.AppendFormat("({0})", columns);
			stringBuilder.Append("VALUES");
			stringBuilder.AppendFormat("({0});select @@IDENTITY", values);
            BoolResult<int> boolResult = this.db.Scalar<int>(stringBuilder.ToString(), parameters);
			if (this.onChange != null && boolResult.Success)
			{
				this.DataChange(OperateType.Insert, this.db.GetConnection(this.db.ConnectionString), stringBuilder.ToString(), parameters, boolResult);
			}
			return boolResult;
		}
		public int Insert(IDbTransaction tran, string columns, string values)
		{
			return this.Insert(tran, columns, values, null);
		}
		public int Insert(IDbTransaction tran, string columns, string values, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("INSERT INTO {0} ", this.TableName);
			stringBuilder.AppendFormat("({0})", columns);
			stringBuilder.Append("VALUES");
			stringBuilder.AppendFormat("({0});select @@IDENTITY", values);
			return this.db.Scalar<int>(tran, stringBuilder.ToString(), parameters);
		}
		public BoolResult<int> Update(string keyValue, string where)
		{
			return this.Update(keyValue, where, null);
		}
		public BoolResult<int> Update(string keyValue, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("UPDATE {0} SET ", this.TableName);
			stringBuilder.AppendFormat("{0} ", keyValue);
			stringBuilder.AppendFormat("WHERE {0} ", where);
			BoolResult<int> boolResult = this.db.NonQuery(stringBuilder.ToString(), parameters);
			if (this.onChange != null && boolResult.Success)
			{
				this.DataChange(OperateType.Update, this.db.GetConnection(this.db.ConnectionString), stringBuilder.ToString(), parameters, boolResult);
			}
			return boolResult;
		}
		public int Update(IDbTransaction tran, string keyValue, string where)
		{
			return this.Update(tran, keyValue, where, null);
		}
		public int Update(IDbTransaction tran, string keyValue, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("UPDATE {0} SET ", this.TableName);
			stringBuilder.AppendFormat("{0} ", keyValue);
			stringBuilder.AppendFormat("WHERE {0} ", where);
			return this.db.NonQuery(tran, stringBuilder.ToString(), parameters);
		}
		public BoolResult<int> Delete(string where)
		{
			return this.Delete(where, null);
		}
		public BoolResult<int> Delete(string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DELETE FROM {0} WHERE {1}", this.TableName, where);
			BoolResult<int> boolResult = this.db.NonQuery(stringBuilder.ToString(), parameters);
			if (this.onChange != null && boolResult.Success)
			{
				this.DataChange(OperateType.Delete, this.db.GetConnection(this.db.ConnectionString), stringBuilder.ToString(), parameters, boolResult);
			}
			return boolResult;
		}
		public int Delete(IDbTransaction tran, string where)
		{
			return this.Delete(tran, where, null);
		}
		public int Delete(IDbTransaction tran, string where, params IDataParameter[] parameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DELETE FROM {0} WHERE {1}", this.TableName, where);
			return this.db.NonQuery(tran, stringBuilder.ToString(), parameters);
		}
		private void DataChange(OperateType type, IDbConnection connection, string sql, IDataParameter[] parameters, BoolResult<int> result)
		{
			if (this.onChange != null)
			{
				DataChangeEventArgs e = new DataChangeEventArgs(connection, type, this.TableName, sql, parameters, result);
				this.onChange.BeginInvoke(this, e, delegate(IAsyncResult c)
				{
					this.onChange.EndInvoke(c);
				}, null);
			}
		}
	}
}
