using System;
using System.Collections.Generic;
using System.Data;
using XUtils.Messages;
namespace XUtils.Data
{
	public static class DataPaging
	{
		public const string SPName = "Proc_Paging";
		public static BoolResult<PagedList<TEntity>> SPToPagedList<TEntity>(this DataBase db, PagedSettings pagedSettings, int pageNumber, int pageSize) where TEntity : class, new()
		{
			IDataParameter[] parameters = DataPaging.BuildParams(db, pagedSettings, true, pageSize, pageNumber);
			IDataParameter[] parameters2 = DataPaging.BuildParams(db, pagedSettings, false, pageSize, pageNumber);
            BoolResult<int> boolResult = db.SPScalar<int>("Proc_Paging", parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
            BoolResult<IList<TEntity>> result = db.RefSPToList<TEntity>("Proc_Paging", parameters2);
			PagedList<TEntity> item = new PagedList<TEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public static BoolResult<PagedList<TEntity>> SPToPagedList<TEntity>(this DataBase db, PagedSettings pagedSettings, int pageNumber, int pageSize, IRowMapper<IDataReader, TEntity> mapper) where TEntity : class, new()
		{
			IDataParameter[] parameters = DataPaging.BuildParams(db, pagedSettings, true, pageSize, pageNumber);
			IDataParameter[] parameters2 = DataPaging.BuildParams(db, pagedSettings, false, pageSize, pageNumber);
            BoolResult<int> boolResult = db.SPScalar<int>("Proc_Paging", parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TEntity>> result = db.SPToList("Proc_Paging", mapper, parameters2);
			PagedList<TEntity> item = new PagedList<TEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public static BoolResult<PagedList<TEntity>> SPToPagedList<TEntity>(this DataBase db, PagedSettings pagedSettings, int pageNumber, int pageSize, Func<IDataReader, TEntity> func) where TEntity : class, new()
		{
			IDataParameter[] parameters = DataPaging.BuildParams(db, pagedSettings, true, pageSize, pageNumber);
			IDataParameter[] parameters2 = DataPaging.BuildParams(db, pagedSettings, false, pageSize, pageNumber);
            BoolResult<int> boolResult = db.SPScalar<int>("Proc_Paging", parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<PagedList<TEntity>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<IList<TEntity>> result = db.SPToList("Proc_Paging", func, parameters2);
			PagedList<TEntity> item = new PagedList<TEntity>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<PagedList<TEntity>>(item, result.Errors.IsValid, "", result.Errors);
		}
		public static BoolResult<Paged<DataTable>> SPToPagedTable(this DataBase db, PagedSettings pagedSettings, int pageNumber, int pageSize)
		{
			IDataParameter[] parameters = DataPaging.BuildParams(db, pagedSettings, true, pageSize, pageNumber);
			IDataParameter[] parameters2 = DataPaging.BuildParams(db, pagedSettings, false, pageSize, pageNumber);
            BoolResult<int> boolResult = db.SPScalar<int>("Proc_Paging", parameters);
			if (!boolResult.Success)
			{
				return new BoolResult<Paged<DataTable>>(null, boolResult.Errors.IsValid, "", boolResult.Errors);
			}
			BoolResult<DataTable> result = db.SPToDataTable("Proc_Paging", parameters2);
			Paged<DataTable> item = new Paged<DataTable>(pageNumber, pageSize, boolResult.Item, result.Item);
			boolResult.Errors.EachFull(delegate(string error)
			{
				result.Errors.Add(error);
			});
			return new BoolResult<Paged<DataTable>>(item, result.Errors.IsValid, "", result.Errors);
		}
		private static IDataParameter[] BuildParams(DataBase db, PagedSettings pagedSettings, bool isTotal, int pageSize, int pageNumber)
		{
			if (isTotal)
			{
				return new IDataParameter[]
				{
					db.BuildParameter("TableName", pagedSettings.TableName),
					db.BuildParameter("Fields", string.Empty),
					db.BuildParameter("SortField", string.Empty),
					db.BuildParameter("PageSize", DbType.AnsiString),
					db.BuildParameter("PageNumber", DbType.AnsiString),
					db.BuildParameter("IsTotalRecords", 1),
					db.BuildParameter("OrderType", DbType.AnsiString),
					db.BuildParameter("Where", pagedSettings.Where)
				};
			}
			return new IDataParameter[]
			{
				db.BuildParameter("TableName", pagedSettings.TableName),
				db.BuildParameter("Fields", pagedSettings.Fields),
				db.BuildParameter("SortField", pagedSettings.SortField),
				db.BuildParameter("PageSize", pageSize),
				db.BuildParameter("PageNumber", pageNumber),
				db.BuildParameter("IsTotalRecords", DbType.AnsiString),
				db.BuildParameter("OrderType", (int)pagedSettings.OrderType),
				db.BuildParameter("Where", pagedSettings.Where)
			};
		}
	}
}
