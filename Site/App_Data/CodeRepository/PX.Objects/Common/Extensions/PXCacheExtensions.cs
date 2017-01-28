using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;

namespace PX.Objects.Common.Extensions
{
	public static class PXCacheExtensions
	{
		public static IEqualityComparer<TDAC> GetKeyComparer<TDAC>(this PXCache<TDAC> cache) 
			where TDAC : class, IBqlTable, new()
		{
			return new CustomComparer<TDAC>(cache.GetObjectHashCode, cache.ObjectsEqual);
		}
	}
}
