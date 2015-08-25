using System;
using System.Linq.Expressions;
using System.Reflection;
namespace XUtils
{
	public class ExpressionHelper
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
		public static string GetPropertyName(Expression<Func<object>> exp)
		{
			MemberExpression memberExpression = exp.Body as MemberExpression;
			if (memberExpression == null)
			{
				throw new InvalidOperationException("Not a member access.");
			}
			PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
			return propertyInfo.Name;
		}
		public static object GetPropertyNameAndValue(Expression<Func<object>> exp, ref string propName)
		{
			Expression arg_06_0 = exp.Body;
			PropertyInfo propertyInfo = null;
			if (exp.Body is MemberExpression)
			{
				propertyInfo = (((MemberExpression)exp.Body).Member as PropertyInfo);
			}
			else
			{
				if (exp.Body is UnaryExpression)
				{
					Expression operand = ((UnaryExpression)exp.Body).Operand;
					propertyInfo = (((MemberExpression)operand).Member as PropertyInfo);
				}
			}
			object result = exp.Compile().DynamicInvoke(new object[0]);
			propName = propertyInfo.Name;
			return result;
		}
		private static void Build<T>(Expression<Func<T, bool>> predicate)
		{
			string format = "{0} {1} {2}";
			BinaryExpression binaryExpression = (BinaryExpression)predicate.Body;
			MemberExpression memberExpression = (MemberExpression)binaryExpression.Left;
			string name = memberExpression.Member.Name;
			string text = RepositoryExpressionTypeHelper.GetText(binaryExpression.NodeType);
			string text2 = "";
			object obj = null;
			if (binaryExpression.Right is ConstantExpression)
			{
				ConstantExpression constantExpression = (ConstantExpression)binaryExpression.Right;
				text2 = RepositoryExpressionValueHelper.GetVal(constantExpression.Value);
			}
			else
			{
				if (binaryExpression.Right is MemberExpression)
				{
					MemberExpression arg_83_0 = (MemberExpression)binaryExpression.Right;
					obj = Expression.Lambda(binaryExpression.Right, new ParameterExpression[0]).Compile().DynamicInvoke(new object[0]);
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
			string value = string.Format(format, name, text, text2);
			Console.WriteLine(value);
		}
	}
}
