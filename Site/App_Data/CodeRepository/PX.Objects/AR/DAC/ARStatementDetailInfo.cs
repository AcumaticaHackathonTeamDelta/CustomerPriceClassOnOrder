using System;
using PX.Data;
using PX.Objects.CM;
	
namespace PX.Objects.AR
{
	/// <summary>
	/// This Class is used in the AR Statement report. Cash Sales and Cash Returns are excluded to prevent them from appearing in the Statement Report.
	/// </summary>
	[PXProjection(typeof(Select2<ARStatementDetail,
						InnerJoin<ARRegister, On<ARStatementDetail.docType,Equal<ARRegister.docType>, 
							And<ARStatementDetail.refNbr,Equal<ARRegister.refNbr>>>,
					   LeftJoin<Standalone.ARInvoice, On<Standalone.ARInvoice.docType, Equal<ARRegister.docType>,
							   And<Standalone.ARInvoice.refNbr, Equal<ARRegister.refNbr>>>,
						LeftJoin<Standalone.ARPayment, On<Standalone.ARPayment.docType,Equal<ARRegister.docType>, 
							And<Standalone.ARPayment.refNbr,Equal<ARRegister.refNbr>>>>>>,
						Where<ARRegister.docType,NotEqual<AR.ARDocType.cashSale>,
						And<ARRegister.docType, NotEqual<AR.ARDocType.cashReturn>>>>), Persistent = false)]
    [Serializable]
	[PXCacheName(Messages.ARStatementDetailInfo)]
	public partial class ARStatementDetailInfo: ARStatementDetail
	{
        #region BranchID
        public new abstract class branchID : PX.Data.IBqlField
        {
        }
        #endregion
        #region StatementDate
        public new abstract class statementDate : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocType
		public new abstract class docType : PX.Data.IBqlField
		{
		}
		#endregion
		#region RefNbr
		public new abstract class refNbr : PX.Data.IBqlField
		{
		}
		#endregion
		#region DocBalance
		public new abstract class docBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryDocBalance
		public new abstract class curyDocBalance : PX.Data.IBqlField
		{
		}
		#endregion
		#region IsOpen
		public abstract new class isOpen : PX.Data.IBqlField
		{
		}
		
		#endregion

		#region DocStatementDate
		public abstract class docStatementDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocStatementDate;
		[PXDBDate( BqlField= typeof(ARRegister.statementDate))]
		public virtual DateTime? DocStatementDate
		{
			get
			{
				return this._DocStatementDate;
			}
			set
			{
				this._DocStatementDate = value;
			}
		}
		#endregion
		#region DocDesc
		public abstract class docDesc : PX.Data.IBqlField
		{
		}
		protected String _DocDesc;
		[PXDBString(60, IsUnicode = true,BqlField=typeof(ARRegister.docDesc))]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DocDesc
		{
			get
			{
				return this._DocDesc;
			}
			set
			{
				this._DocDesc = value;
			}
		}
		#endregion

		#region PrintDocType
		public abstract class printDocType : PX.Data.IBqlField
		{
		}
		[PXString(3, IsFixed = true)]
		[ARDocType.PrintList()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.Visible, Enabled = true)]
		public virtual String PrintDocType
		{
			get
			{
				return this._DocType;
			}
		}
		#endregion
		#region Payable
		public abstract class payable : IBqlField { }

		public virtual Boolean? Payable
		{
			[PXDependsOnFields(typeof(docType))]
			get
			{
				return ARDocType.Payable(this._DocType);
			}		
		}
        #endregion
        #region BalanceSign
        public abstract class balanceSign : IBqlField { }

        public virtual decimal? BalanceSign
        {
            [PXDependsOnFields(typeof(docType))]
            get
            {
                return ARDocType.SignBalance(this._DocType);
            }
        }
        #endregion
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDBDate(BqlField = typeof(ARRegister.docDate) )]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong(BqlField=typeof(ARRegister.curyInfoID))]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CuryOrigDocAmt
		public abstract class curyOrigDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryOrigDocAmt;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCurrency(typeof(ARStatementDetailInfo.curyInfoID), typeof(ARStatementDetailInfo.origDocAmt),BqlField=typeof(ARRegister.curyOrigDocAmt))]
		[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryOrigDocAmt
		{
			get
			{
				return this._CuryOrigDocAmt;
			}
			set
			{
				this._CuryOrigDocAmt = value;
			}
		}
		#endregion
		#region OrigDocAmt
		public abstract class origDocAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _OrigDocAmt;
		[PXDBBaseCury(BqlField = typeof(ARRegister.origDocAmt))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? OrigDocAmt
		{
			get
			{
				return this._OrigDocAmt;
			}
			set
			{
				this._OrigDocAmt = value;
			}
		}
		#endregion
		
		#region InvoiceNbr
		public abstract class invoiceNbr : PX.Data.IBqlField
		{
		}
		protected String _InvoiceNbr;
		[PXDBString(40, IsUnicode = true,BqlField= typeof(Standalone.ARInvoice.invoiceNbr))]
		[PXUIField(DisplayName = "Customer Ref. Nbr.", Visibility = PXUIVisibility.SelectorVisible, Required=false)]
		public virtual String InvoiceNbr
		{
			get
			{
				return this._InvoiceNbr;
			}
			set
			{
				this._InvoiceNbr = value;
			}
		}
		#endregion
		#region DueDate
		public abstract class dueDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DueDate;
		[PXDBDate(BqlField=typeof(ARRegister.dueDate))]
		[PXUIField(DisplayName = "Due Date", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? DueDate
		{
			get
			{
				return this._DueDate;
			}
			set
			{
				this._DueDate = value;
			}
		}
		#endregion
		#region ExtRefNbr
		public abstract class extRefNbr : PX.Data.IBqlField
		{
		}
		protected String _ExtRefNbr;
		[PXDBString(40, IsUnicode = true, BqlField=typeof(Standalone.ARPayment.extRefNbr))]
		[PXUIField(DisplayName = "Payment Ref.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ExtRefNbr
		{
			get
			{
				return this._ExtRefNbr;
			}
			set
			{
				this._ExtRefNbr = value;
			}
		}
		#endregion

		#region DocExtRefNbr
		[PXString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ext. Ref.#", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String DocExtRefNbr
		{
			[PXDependsOnFields(typeof(payable),typeof(invoiceNbr),typeof(extRefNbr))]
			get
			{
				return (this.Payable.HasValue?(this.Payable.Value ? this._InvoiceNbr : this.ExtRefNbr):String.Empty);
			}

		}
		#endregion
		#region CuryOrigDocAmtSigned
		
		[PXDecimal()]
		[PXUIField(DisplayName = "Origin. Amt", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryOrigDocAmtSigned
		{
            [PXDependsOnFields(typeof(docType), typeof(balanceSign), typeof(curyOrigDocAmt))]
			get
			{
                return BalanceSign * CuryOrigDocAmt;
			}			
		}
		#endregion
		#region OrigDocAmtSigned
		[PXDecimal()]
		public virtual Decimal? OrigDocAmtSigned
		{
			[PXDependsOnFields(typeof(docType), typeof(balanceSign), typeof(origDocAmt))]
			get
			{
                return BalanceSign * OrigDocAmt;
			}
		}
		#endregion

		#region CuryDocBalanceSigned

		[PXDecimal()]
		[PXUIField(DisplayName = "Amount Due", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? CuryDocBalanceSigned
		{
			[PXDependsOnFields(typeof(payable),typeof(curyDocBalance))]
			get
			{
				return (this.Payable.HasValue? ((bool)this.Payable? this._CuryDocBalance : -this._CuryDocBalance):null);
			}
		}
		#endregion
		#region DocBalanceSigned
		[PXDecimal()]
		public virtual Decimal? DocBalanceSigned
		{
			[PXDependsOnFields(typeof(payable),typeof(docBalance))]
			get
			{
				return (this.Payable.HasValue? (this.Payable.Value? this._DocBalance : -this._DocBalance):null);
			}
		}
		#endregion		
	}
}
