using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects.Common
{
	public class ValidationHelper
	{
		protected readonly object Row;
		protected readonly PXCache Cache;

		public ValidationHelper(PXCache cache, object row)
		{
			Row = row;
			Cache = cache;
		}

		public bool SetErrorEmptyIfNull<TField>(object value)
			where TField : IBqlField
		{
			return SetErrorEmptyIfNull<TField>(Cache, Row, value);
		}

		public static bool SetErrorEmptyIfNull<TField>(PXCache cache, object row, object value)
			where TField : IBqlField
		{
			if (value == null)
			{
				cache.RaiseExceptionHandling<TField>(row, null, new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(TField).Name));
				return false;
			}

			return true;
		}
	}
}
