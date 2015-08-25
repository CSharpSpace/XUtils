using System;
using System.Collections.Generic;
using System.Linq.Expressions;
namespace XUtils
{
	internal class RepositoryExpressionTypeHelper
	{
		private static IDictionary<ExpressionType, string> _map;
		static RepositoryExpressionTypeHelper()
		{
			RepositoryExpressionTypeHelper._map = new Dictionary<ExpressionType, string>();
			RepositoryExpressionTypeHelper._map[ExpressionType.Equal] = "=";
			RepositoryExpressionTypeHelper._map[ExpressionType.NotEqual] = "<>";
			RepositoryExpressionTypeHelper._map[ExpressionType.GreaterThanOrEqual] = ">=";
			RepositoryExpressionTypeHelper._map[ExpressionType.LessThanOrEqual] = "<=";
			RepositoryExpressionTypeHelper._map[ExpressionType.LessThan] = "<";
			RepositoryExpressionTypeHelper._map[ExpressionType.GreaterThan] = ">";
		}
		public static string GetText(ExpressionType expType)
		{
			if (!RepositoryExpressionTypeHelper._map.ContainsKey(expType))
			{
				throw new ArgumentException("expresion type :" + expType.ToString() + "not supported.");
			}
			return RepositoryExpressionTypeHelper._map[expType];
		}
	}
}
