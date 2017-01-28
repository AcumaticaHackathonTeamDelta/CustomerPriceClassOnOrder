using System;
using System.Collections.Generic;

namespace PX.Objects.Common.Extensions
{
	public static class ObjectExtensions
	{
		public static bool IsComplex(this object obj)
		{
			return !obj.GetType().IsPrimitive
				   && obj.GetType() != typeof(string)
				   && obj.GetType() != typeof(decimal)
				   && obj.GetType() != typeof(decimal?)
				   && obj.GetType() != typeof(DateTime)
				   && obj.GetType() != typeof(DateTime?);
		}

		public static TObject[] SingleToArray<TObject>(this TObject obj)
		{
			return new[] { obj };
		}

		public static List<TObject> SingleToList<TObject>(this TObject obj)
		{
			return new List<TObject>() { obj };
		}
	}
}
