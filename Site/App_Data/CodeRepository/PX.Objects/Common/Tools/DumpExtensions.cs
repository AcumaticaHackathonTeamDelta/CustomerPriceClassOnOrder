using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using PX.Objects.Common.Extensions;

namespace PX.Objects.Common.Tools
{
	public static class DumpExtensions
	{
		private const string Indent = "    ";

		public static string Dump(this object obj, Action<object, StringBuilder> dumpSingleObject)
		{
			var sb = new StringBuilder();

			var enumeration = obj as IEnumerable;
			if (enumeration != null)
			{
				sb.AppendLine(obj.ToString());
				sb.AppendLine("{");

				foreach (var item in enumeration)
				{
					dumpSingleObject(item, sb);
					sb.Append(",");
				}

				sb.AppendLine("}");
			}
			else
			{
				dumpSingleObject(obj, sb);
			}

			return sb.ToString();
		}

		public static string Dump(this object obj)
		{
			return Dump(obj, (ob, sb) => DumpForSingleObject(ob, sb));
		}

		public static string DumpForSingleObject(object obj, StringBuilder sb,  int getFullImageStackLevel = -1)
		{
			getFullImageStackLevel++;

			PropertyInfo[] propertyInfos = obj.GetType().GetProperties();

			var indent = GetIndent(getFullImageStackLevel);
			var detailIndent = GetIndent(getFullImageStackLevel + 1);

			sb.AppendLine();
			sb.AppendLine(string.Concat(indent, "{"));

			foreach (var propertyInfo in propertyInfos)
			{
				var value = propertyInfo.GetValue(obj, null);

				if (value != null)
				{
					if (value.IsComplex())
					{
						value = DumpForSingleObject(value, sb, getFullImageStackLevel + 1);
					}

					if (value is string)
					{
						value = string.Concat("\"", value, "\"");
					}

					if (value is decimal
					    || value is decimal?)
					{
						value = ((decimal)value).ToString(CultureInfo.InvariantCulture);
					}
				}
				else
				{
					value = "null";
				}

				sb.AppendLine(string.Format("{0}{1}: {2}", detailIndent, propertyInfo.Name, value));
			}

			sb.AppendLine(string.Concat(indent, "}"));

			return sb.ToString();
		}

		private static string GetIndent(int level)
		{
			return string.Join(string.Empty, Enumerable.Repeat(Indent, level));
		}
	}
}
