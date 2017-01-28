using System;
using PX.Data;
using PX.Objects.GL;

namespace PX.Objects.AP
{
	[Serializable]
	public class MISC1099EFileFilter : IBqlTable
	{
		#region MasterBranchID

		public abstract class masterBranchID : IBqlField
		{
		}
		protected int? _MasterBranchID;
		[MasterBranch1099EFile(DisplayName = "Transmitter")]
		public virtual int? MasterBranchID
		{
			get
			{
				return _MasterBranchID;
			}
			set
			{
				_MasterBranchID = value;
			}
		}

		#endregion

		#region FinYear

		public abstract class finYear : PX.Data.IBqlField
		{
		}
		protected String _FinYear;
		[PXDBString(4, IsFixed = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "1099 Year", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<AP1099Year.finYear>))]
		public virtual String FinYear
		{
			get
			{
				return this._FinYear;
			}
			set
			{
				this._FinYear = value;
			}
		}
		#endregion
		#region Include
		public abstract class include : IBqlField
		{
			public class ListAttribute : PXStringListAttribute
			{
				public ListAttribute()
					: base(
					new string[] { TransmitterOnly, AllMarkedBranches },
					new string[] { Messages.TransmitterOnly, Messages.AllMarkedBranches })
				{ }
			}
			public const string TransmitterOnly = "T";
			public const string AllMarkedBranches = "A";

			public class transmitterOnly : Constant<string>
			{
				public transmitterOnly() : base(TransmitterOnly) { }
			}

			public class allMarkedBranches : Constant<string>
			{
				public allMarkedBranches() : base(AllMarkedBranches) { }
			}
		}
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Prepare for")]
		[include.List]
		[PXDefault(include.TransmitterOnly)]
		public virtual string Include { get; set; }
		#endregion

		#region IsPriorYear

		public abstract class isPriorYear : PX.Data.IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Prior Year")]
		public virtual bool? IsPriorYear { get; set; }

		#endregion

		#region IsCorrectionReturn

		public abstract class isCorrectionReturn : PX.Data.IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Correction File")]
		public virtual bool? IsCorrectionReturn { get; set; }

		#endregion

		#region IsLastFiling

		public abstract class isLastFiling : PX.Data.IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Last Filing")]
		public virtual bool? IsLastFiling { get; set; }

		#endregion

		#region ReportingDirectSalesOnly

		public abstract class reportingDirectSalesOnly : PX.Data.IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Direct Sales only")]
		public virtual bool? ReportingDirectSalesOnly { get; set; }

		#endregion

		#region IsTestMode

		public abstract class isTestMode : PX.Data.IBqlField { }
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Test File")]
		public virtual bool? IsTestMode { get; set; }

		#endregion
	}
}
