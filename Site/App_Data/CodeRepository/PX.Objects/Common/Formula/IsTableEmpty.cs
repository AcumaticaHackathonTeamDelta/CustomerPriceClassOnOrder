using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;

namespace PX.Objects.Common
{
	public sealed class IsTableEmpty<Table> : 
		IBqlOperand, IBqlCreator 
		where Table: class, IBqlTable, new()
	{
		public void Verify(PXCache cache, object item, List<object> parameters, ref bool? result, ref object value)
		{
			PXCache itemCache = cache.Graph?.Caches[typeof(Table)];

			if (itemCache?.Current != null)
			{
				value = false;
				return;
			}

			value = 
				(Table)PXSelect<Table>.SelectWindowed(cache.Graph, 0, 1) == null;
		}

		public void Parse(
			PXGraph graph, 
			List<IBqlParameter> pars, 
			List<Type> tables, 
			List<Type> fields, 
			List<IBqlSortColumn> sortColumns, 
			StringBuilder text, 
			BqlCommand.Selection selection)
		{ }
	}
}
