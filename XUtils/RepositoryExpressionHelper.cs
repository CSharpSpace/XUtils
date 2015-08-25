using System;
using System.Linq.Expressions;
using System.Reflection;
namespace XUtils
{
	public class RepositoryExpressionHelper
	{
		public static string GetPropertyName<T>(Expression<Func<T, object>> exp)
		{
			MemberExpression memberExpression = null;
			if (exp.Body is MemberExpression)
			{
				memberExpression = (exp.Body as MemberExpression);
			}
			if (exp.Body is UnaryExpression)
			{
				UnaryExpression unaryExpression = exp.Body as UnaryExpression;
				if (unaryExpression.Operand is MemberExpression)
				{
					memberExpression = (unaryExpression.Operand as MemberExpression);
				}
			}
			if (memberExpression == null)
			{
				throw new InvalidOperationException("Not a member access.");
			}
			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
			return propertyInfo.Name;
		}
		public static string BuildSinglePropertyCondition<T>(Expression<Func<T, bool>> predicate)
		{
			string format = "{0} {1} {2}";
			BinaryExpression binaryExpression = (BinaryExpression)predicate.Body;
			MemberExpression memberExpression = null;
			if (binaryExpression.Left is MemberExpression)
			{
				memberExpression = (MemberExpression)binaryExpression.Left;
			}
			else
			{
				if (binaryExpression.Left is UnaryExpression)
				{
					memberExpression = (((UnaryExpression)binaryExpression.Left).Operand as MemberExpression);
				}
			}
			string name = memberExpression.Member.Name;
			string text = RepositoryExpressionTypeHelper.GetText(binaryExpression.NodeType);
			string text2 = "";
			object obj = null;
			if (binaryExpression.Right is ConstantExpression)
			{
				ConstantExpression constantExpression = (ConstantExpression)binaryExpression.Right;
				text2 = RepositoryExpressionHelper.GetVal(constantExpression.Value);
			}
			else
			{
				if (binaryExpression.Right is MemberExpression)
				{
					MemberExpression arg_B7_0 = (MemberExpression)binaryExpression.Right;
					obj = Expression.Lambda(binaryExpression.Right, new ParameterExpression[0]).Compile().DynamicInvoke(new object[0]);
				}
				else
				{
					obj = Expression.Lambda(binaryExpression.Right, new ParameterExpression[0]).Compile().DynamicInvoke(new object[0]);
					text2 = obj.ToString();
				}
			}
			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
			if (propertyInfo.PropertyType == typeof(string))
			{
				text2 = string.Format("'{0}'", text2.Replace("'", "''"));
			}
			else
			{
				if (propertyInfo.PropertyType == typeof(DateTime))
				{
					text2 = "'" + ((DateTime)obj).ToShortDateString() + "'";
				}
			}
			return string.Format(format, name, text, text2);
		}
		public static string GetVal(object val)
		{
			if (val.GetType() != typeof(bool) && val.GetType() != typeof(bool))
			{
				return val.ToString();
			}
			if (!(bool)val)
			{
				return "0";
			}
			return "1";
		}
	}
}
