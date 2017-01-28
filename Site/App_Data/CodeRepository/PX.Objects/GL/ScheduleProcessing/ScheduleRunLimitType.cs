using System.Collections.Generic;

using PX.Data;

using PX.Objects.Common;

namespace PX.Objects.GL
{
	public class ScheduleRunLimitType : ILabelProvider
	{
		public const string RunTillDate = "D";
		public const string RunMultipleTimes = "M";

		public IEnumerable<ValueLabelPair> ValueLabelPairs => new ValueLabelList
		{
			{ RunTillDate, Messages.OnThisDate },
			{ RunMultipleTimes, Messages.AfterThisNumberOfScheduleExecutions },
		};

		public class runTillDate : Constant<string>
		{
			public runTillDate() : base(RunTillDate) { }
		}

		public class runMultipleTimes : Constant<string>
		{
			public runMultipleTimes() : base(RunMultipleTimes) { }
		}
	}
}
