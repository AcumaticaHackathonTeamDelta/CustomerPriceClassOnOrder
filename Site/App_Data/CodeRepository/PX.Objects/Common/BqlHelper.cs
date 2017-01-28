using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PX.Data;

namespace PX.Objects.Common
{
	public static class BqlHelper
	{
		private static readonly Dictionary<Tuple<Type, Type>, bool> _parametersEqualityCache =
			new Dictionary<Tuple<Type, Type>, bool>();

		/// <summary>
		/// Ensures that the first command's parameters have the same type as
		/// the second command's parameters. Can be helpful to keep graph 
		/// views and selects inside their delegates synchronized in terms 
		/// of BQL parameters.
		/// </summary>
		public static void EnsureParametersEqual(this BqlCommand firstCommand, BqlCommand secondCommand)
		{
			if (firstCommand == null) throw new ArgumentNullException(nameof(firstCommand));
			if (secondCommand == null) throw new ArgumentNullException(nameof(secondCommand));

			Tuple<Type, Type> cacheLookupKey = Tuple.Create(firstCommand.GetSelectType(), secondCommand.GetSelectType());
			Tuple<Type, Type> reverseCacheLookupKey = Tuple.Create(cacheLookupKey.Item2, cacheLookupKey.Item1);

			bool checkResult;

			if (!_parametersEqualityCache.ContainsKey(cacheLookupKey) &&
				!_parametersEqualityCache.ContainsKey(reverseCacheLookupKey))
			{
				IBqlParameter[] firstCommandParameters = firstCommand.GetParameters();
				IBqlParameter[] secondCommandParameters = secondCommand.GetParameters();

				if (firstCommandParameters.Length != secondCommandParameters.Length)
				{
					checkResult = false;
				}
				else
				{
					checkResult = firstCommandParameters
						.Zip(secondCommandParameters, (x, y) => x.GetType() == y.GetType())
						.All(x => x);
				}

				_parametersEqualityCache[cacheLookupKey] = checkResult;
			}
			else
			{
				checkResult = _parametersEqualityCache[cacheLookupKey];
			}

			if (!checkResult)
			{
				throw new PXException(Messages.BqlCommandsHaveDifferentParameters);
			}
		}
	}
}
