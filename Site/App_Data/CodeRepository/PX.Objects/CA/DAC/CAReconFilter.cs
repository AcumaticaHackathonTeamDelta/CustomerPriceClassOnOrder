namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.CS;
    using PX.Objects.CR;
	using PX.Objects.GL;
	using PX.Objects.CM;

	[System.SerializableAttribute()]
	public partial class CAReconFilter :  PX.Data.IBqlTable
	{
		#region ReconNbr
		public abstract class reconNbr : PX.Data.IBqlField
		{
		}
		protected String _ReconNbr;
		[PXString(15, IsUnicode = true, InputMask = "")]
		[PXDefault()]
		[PXUIField(DisplayName = "Ref. Number", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<CARecon.reconNbr, Where<CARecon.cashAccountID, 
									Equal<Optional<CARecon.cashAccountID>>, Or<Optional<CARecon.cashAccountID>, IsNull>>>),
					typeof(CARecon.reconNbr), typeof(CARecon.cashAccountID), typeof(CARecon.reconDate), typeof(CARecon.status))] //,
		public virtual String ReconNbr
		{
			get
			{
				return this._ReconNbr;
			}
			set
			{
				this._ReconNbr = value;
			}
		}
		#endregion
        #region CashAccountID
        public abstract class cashAccountID : PX.Data.IBqlField
        {
        }
        protected Int32? _CashAccountID;
        [CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where<CashAccount.reconcile, Equal<boolTrue>, And<Match<Current<AccessInfo.userName>>>>>),  IsKey = true, Visibility = PXUIVisibility.SelectorVisible, Enabled = true)]
		[PXDefault()]
        public virtual Int32? CashAccountID
        {
            get
            {
                return this._CashAccountID;
            }
            set
            {
                this._CashAccountID = value;
            }
        }
        #endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				this._EndDate = value;
			}
		}
		#endregion
	}
}
