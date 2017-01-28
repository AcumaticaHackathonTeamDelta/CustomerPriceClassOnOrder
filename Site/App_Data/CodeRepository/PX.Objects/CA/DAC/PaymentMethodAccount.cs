namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	
	[System.SerializableAttribute()]
	[PXProjection(typeof(Select2<PaymentMethodAccount, InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<PaymentMethodAccount.cashAccountID>>>>), new Type[] { typeof (PaymentMethodAccount) })]
	[PXCacheName(Messages.PaymentMethodAccount)]
	public partial class PaymentMethodAccount : PX.Data.IBqlTable
	{
		#region PaymentMethodID
		public abstract class paymentMethodID : PX.Data.IBqlField
		{
		}
		protected String _PaymentMethodID;
		[PXDBString(10, IsUnicode = true, IsKey = true, InputMask = ">aaaaaaaaaa")]
		[PXDefault(typeof(PaymentMethod.paymentMethodID))]
		[PXParent(typeof(Select<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Current<PaymentMethodAccount.paymentMethodID>>>>))]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID>))]
		[PXUIField(DisplayName="Payment Method")]
		public virtual String PaymentMethodID
		{
			get
			{
				return this._PaymentMethodID;
			}
			set
			{
				this._PaymentMethodID = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[GL.CashAccount(DisplayName = "Cash Account", Visibility = PXUIVisibility.Visible,IsKey = true, DescriptionField =typeof(CashAccount.descr))]
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
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(BqlField = typeof(CashAccount.branchID))]
		public virtual Int32? BranchID
		{
			get
			{
				return this._BranchID;
			}
			set
			{
				this._BranchID = value;
			}
		}
		#endregion
		#region UseForAP
		public abstract class useForAP : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseForAP;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use in AP")]
		public virtual Boolean? UseForAP
		{
			get
			{
				return this._UseForAP;
			}
			set
			{
				this._UseForAP = value;
			}
		}
		#endregion
		#region APIsDefault
		public abstract class aPIsDefault : PX.Data.IBqlField
		{
		}
		protected Boolean? _APIsDefault;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "AP Default")]
		public virtual Boolean? APIsDefault
		{
			get
			{
				return this._APIsDefault;
			}
			set
			{
				this._APIsDefault = value;
			}
		}
		#endregion
		#region APAutoNextNbr
		public abstract class aPAutoNextNbr : PX.Data.IBqlField
		{
		}
		protected Boolean? _APAutoNextNbr;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "AP - Suggest Next Number")]
		public virtual Boolean? APAutoNextNbr
		{
			get
			{
				return this._APAutoNextNbr;
			}
			set
			{
				this._APAutoNextNbr = value;
			}
		}
		#endregion
		#region APLastRefNbr
		public abstract class aPLastRefNbr : PX.Data.IBqlField
		{
		}
		protected String _APLastRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "AP Last Reference Number")]
		public virtual String APLastRefNbr
		{
			get
			{
				return this._APLastRefNbr;
			}
			set
			{
				this._APLastRefNbr = value;
			}
		}
		#endregion
		#region APLastRefNbrIsNull
		protected Boolean? _APLastRefNbrIsNull = false;
		public virtual Boolean? APLastRefNbrIsNull
		{
			get
			{
				return this._APLastRefNbrIsNull;
			}
			set
			{
				this._APLastRefNbrIsNull = value;
			}
		}
		#endregion
		#region APBatchLastRefNbr
		public abstract class aPBatchLastRefNbr : PX.Data.IBqlField
		{
		}
		protected String _APBatchLastRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Last Reference Number")]
		public virtual String APBatchLastRefNbr
		{
			get
			{
				return this._APBatchLastRefNbr;
			}
			set
			{
				this._APBatchLastRefNbr = value;
			}
		}
		#endregion
		#region UseForAR
		public abstract class useForAR : PX.Data.IBqlField
		{
		}
		protected Boolean? _UseForAR;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Use in AR")]
		public virtual Boolean? UseForAR
		{
			get
			{
				return this._UseForAR;
			}
			set
			{
				this._UseForAR = value;
			}
		}
		#endregion
		#region ARIsDefault
		public abstract class aRIsDefault : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARIsDefault;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "AR Default")]
		public virtual Boolean? ARIsDefault
		{
			get
			{
				return this._ARIsDefault;
			}
			set
			{
				this._ARIsDefault = value;
			}
		}
		#endregion
		#region ARIsDefaultForRefund
		public abstract class aRIsDefaultForRefund : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARIsDefaultForRefund;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "AR Default For Refund")]
		public virtual Boolean? ARIsDefaultForRefund
		{
			get
			{
				return this._ARIsDefaultForRefund;
			}
			set
			{
				this._ARIsDefaultForRefund = value;
			}
		}
		#endregion
		#region ARAutoNextNbr
		public abstract class aRAutoNextNbr : PX.Data.IBqlField
		{
		}
		protected Boolean? _ARAutoNextNbr;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "AR - Suggest Next Number")]
		public virtual Boolean? ARAutoNextNbr
		{
			get
			{
				return this._ARAutoNextNbr;
			}
			set
			{
				this._ARAutoNextNbr = value;
			}
		}
		#endregion
		#region ARLastRefNbr
		public abstract class aRLastRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ARLastRefNbr;
		[PXDBString(40, IsUnicode = true)]
		[PXUIField(DisplayName = "AR Last Reference Number")]
		public virtual String ARLastRefNbr
		{
			get
			{
				return this._ARLastRefNbr;
			}
			set
			{
				this._ARLastRefNbr = value;
			}
		}
		#endregion
		#region APLastRefNbrIsNull
		protected Boolean? _ARLastRefNbrIsNull = false;
		public virtual Boolean? ARLastRefNbrIsNull
		{
			get
			{
				return this._ARLastRefNbrIsNull;
			}
			set
			{
				this._ARLastRefNbrIsNull = value;
			}
		}
		#endregion        
	}


	public class PaymentMethodAccountUsage
	{
		public const string UseForAP = "P";
		public const string UseForAR = "R";
		public class useForAP : Constant<string>
		{
			public useForAP() : base(UseForAP) { }
		}
		public class useForAR : Constant<string>
		{
			public useForAR() : base(UseForAR) { }
		}        
	}
}
