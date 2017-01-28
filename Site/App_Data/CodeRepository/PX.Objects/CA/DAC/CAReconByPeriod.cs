namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CS;

	[System.SerializableAttribute()]
	[PXProjection(typeof(Select5<CARecon,
		InnerJoin<FinPeriod, On<FinPeriod.endDate, Greater<CARecon.reconDate>, And<CARecon.reconciled, Equal<boolTrue>, And<CARecon.voided, Equal<boolFalse>>>>>,
		Aggregate<GroupBy<CARecon.cashAccountID,
			Max<CARecon.reconDate,
			GroupBy<FinPeriod.finPeriodID
		>>>>>))]
	[PXCacheName(Messages.CAReconByPeriod)]
	public partial class CAReconByPeriod : PX.Data.IBqlTable
	{
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField {}
		[CashAccount(IsKey = true, BqlField = typeof(CARecon.cashAccountID))]
		public virtual Int32? CashAccountID { get; set; }
		#endregion
		#region LastReconDate
		public abstract class lastReconDate : PX.Data.IBqlField {}
		[PXDBDate(BqlField = typeof(CARecon.reconDate))]
		[PXUIField(DisplayName = "Last Reconciliation Date")]
		public virtual DateTime? LastReconDate { get; set; }
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField {}
		protected String _FinPeriodID;
		[FinPeriodID(IsKey = true, BqlField = typeof(FinPeriod.finPeriodID))]
		public virtual String FinPeriodID { get; set; }
		#endregion
	}
}
