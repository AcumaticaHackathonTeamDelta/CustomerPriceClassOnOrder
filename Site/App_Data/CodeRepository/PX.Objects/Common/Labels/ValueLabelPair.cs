using System;

namespace PX.Objects.Common
{
	public struct ValueLabelPair
	{
		public string Value { get; private set; }
		public string Label { get; private set; }

		/// <summary>
		/// Initializes a new instance of <see cref="ValueLabelPair"/> using a
		/// value and its corresponding label.
		/// </summary>
		public ValueLabelPair(string value, string label)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (label == null) throw new ArgumentNullException(nameof(label));

			this.Value = value;
			this.Label = label;
		}
	}
}
