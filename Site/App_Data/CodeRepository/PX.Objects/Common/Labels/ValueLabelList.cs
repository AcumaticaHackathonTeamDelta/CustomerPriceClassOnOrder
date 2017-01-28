using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.Common
{
	public class ValueLabelList : IEnumerable<ValueLabelPair>
	{
		private readonly List<ValueLabelPair> _valueLabelPairs = new List<ValueLabelPair>();

		public void Add(string value, string label)
		{
			_valueLabelPairs.Add(new ValueLabelPair(value, label));
		}

		public IEnumerator<ValueLabelPair> GetEnumerator() =>
			_valueLabelPairs.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			_valueLabelPairs.GetEnumerator();
	}
}
