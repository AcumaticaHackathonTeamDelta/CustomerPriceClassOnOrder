using System;
using System.Collections.Generic;
using System.Collections;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CM;

namespace PX.Objects.AP
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class APDocumentEnq : PXGraph<APDocumentEnq>
	{
		#region Internal Types
		[Serializable]
		public partial class APDocumentFilter : IBqlTable
		{
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
            [Branch()]
            [PXRestrictor(typeof(Where<True, Equal<True>>), GL.Messages.BranchInactive, ReplaceInherited = true)]
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
			#region VendorID
			public abstract class vendorID : PX.Data.IBqlField
			{
			}
			protected Int32? _VendorID;
			[Vendor(DescriptionField = typeof(Vendor.acctName))]
			[PXDefault()]
			public virtual Int32? VendorID
			{
				get
				{
					return this._VendorID;
				}
				set
				{
					this._VendorID = value;
				}
			}
			#endregion
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[GL.Account(null, typeof(Search5<Account.accountID,
					InnerJoin<APHistory, On<Account.accountID, Equal<APHistory.accountID>>>,
					Where<Match<Current<AccessInfo.userName>>>,
					Aggregate<GroupBy<Account.accountID>>>),
			   DisplayName = "AP Account", DescriptionField = typeof(GL.Account.description))]
			public virtual Int32? AccountID
			{
				get
				{
					return this._AccountID;
				}
				set
				{
					this._AccountID = value;
				}
			}
			#endregion
			#region SubCD
			public abstract class subCD : PX.Data.IBqlField
			{
			}
			protected String _SubCD;
			[PXDBString(30, IsUnicode = true)]
			[PXUIField(DisplayName = "AP Subaccount", Visibility = PXUIVisibility.Invisible, FieldClass = SubAccountAttribute.DimensionName)]
			[PXDimension("SUBACCOUNT", ValidComboRequired = false)]
			public virtual String SubCD
			{
				get
				{
					return this._SubCD;
				}
				set
				{
					this._SubCD = value;
				}
			}
				#endregion
			#region FinPeriodID
			public abstract class finPeriodID : PX.Data.IBqlField
			{
			}
			protected String _FinPeriodID;
			[APAnyPeriodFilterable()]
			[PXUIField(DisplayName = "Period")]
			public virtual String FinPeriodID
			{
				get
				{
					return this._FinPeriodID;
				}
				set
				{
					this._FinPeriodID = value;
				}
			}
			#endregion
			#region ByFinancialPeriod
			public abstract class byFinancialPeriod : PX.Data.IBqlField
			{
			}
			protected bool? _ByFinancialPeriod;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "By Financial Period")]
			public virtual bool? ByFinancialPeriod
			{
				get
				{
					return this._ByFinancialPeriod;
				}
				set
				{
					this._ByFinancialPeriod = value;
				}
			}
			#endregion
			#region DocType
			public abstract class docType : PX.Data.IBqlField
			{
			}
			protected String _DocType;
			[PXDBString(3, IsFixed = true)]
			[PXDefault()]
			[APDocType.List()]
			[PXUIField(DisplayName = "Type")]
			public virtual String DocType
			{
				get
				{
					return this._DocType;
				}
				set
				{
					this._DocType = value;
				}
			}
			#endregion
			#region ShowAllDocs
			public abstract class showAllDocs : PX.Data.IBqlField
			{
			}
			protected bool? _ShowAllDocs;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Show All Documents")]
			public virtual bool? ShowAllDocs
			{
				get
				{
					return this._ShowAllDocs;
				}
				set
				{
					this._ShowAllDocs = value;
				}
			}
			#endregion
			#region IncludeUnreleased
			public abstract class includeUnreleased : PX.Data.IBqlField
			{
			}
			protected bool? _IncludeUnreleased;
			[PXDBBool()]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Include Unreleased Documents")]
			public virtual bool? IncludeUnreleased
			{
				get
				{
					return this._IncludeUnreleased;
				}
				set
				{
					this._IncludeUnreleased = value;
				}
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
			[PXSelector(typeof(CM.Currency.curyID), CacheGlobal = true)]
			[PXUIField(DisplayName = "Currency")]
			public virtual String CuryID
			{
				get
				{
					return this._CuryID;
				}
				set
				{
					this._CuryID = value;
				}
			}
			#endregion
			#region SubCD Wildcard
			public abstract class subCDWildcard : PX.Data.IBqlField { };
			[PXDBString(30, IsUnicode = true)]
			public virtual String SubCDWildcard
			{
				get
				{
					return SubCDUtils.CreateSubCDWildcard(this._SubCD, SubAccountAttribute.DimensionName);
				}
			}



			#endregion
			#region CuryBalanceSummary
			public abstract class curyBalanceSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryBalanceSummary;
			[PXCury(typeof(APDocumentFilter.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Balance by Documents (Currency)", Enabled = false)]
			public virtual Decimal? CuryBalanceSummary
			{
				get
				{
					return this._CuryBalanceSummary;
				}
				set
				{
					this._CuryBalanceSummary = value;
				}
			}
			#endregion
			#region BalanceSummary
			public abstract class balanceSummary : PX.Data.IBqlField
			{
			}
			protected Decimal? _BalanceSummary;
            [PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Balance by Documents",Enabled=false)]
			public virtual Decimal? BalanceSummary
			{
				get
				{
					return this._BalanceSummary;
				}
				set
				{
					this._BalanceSummary = value;
				}
			}
			#endregion
			#region CuryVendorBalance
			public abstract class curyVendorBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryVendorBalance;
            [PXCury(typeof(APDocumentFilter.curyID))]
            [PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Current Balance (Currency)", Enabled = false)]
			public virtual Decimal? CuryVendorBalance
			{
				get
				{
					return this._CuryVendorBalance;
				}
				set
				{
					this._CuryVendorBalance = value;
				}
			}
			#endregion
			#region VendorBalance
			public abstract class vendorBalance: PX.Data.IBqlField
			{
			}
			protected Decimal? _VendorBalance;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Current Balance", Enabled = false)]
			public virtual Decimal? VendorBalance
			{
				get
				{
					return this._VendorBalance;
				}
				set
				{
					this._VendorBalance = value;
				}
			}
			#endregion

			#region CuryVendorDepositsBalance

			public abstract class curyVendorDepositsBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryVendorDepositsBalance;
			[PXCury(typeof(APDocumentFilter.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Prepayments Balance (Currency)", Enabled = false)]
			public virtual Decimal? CuryVendorDepositsBalance
			{
				get
				{
					return this._CuryVendorDepositsBalance;
				}
				set
				{
					this._CuryVendorDepositsBalance = value;
				}
			}
			#endregion
			#region VendorDepositsBalance

			public abstract class vendorDepositsBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _VendorDepositsBalance;
            [PXDBBaseCury()]
            [PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Prepayments Balance", Enabled = false)]
			public virtual Decimal? VendorDepositsBalance
			{
				get
				{
					return this._VendorDepositsBalance;
				}
				set
				{
					this._VendorDepositsBalance = value;
				}
			}
			#endregion

			#region CuryDifference
			public abstract class curyDifference : PX.Data.IBqlField
			{
			}

			[PXCury(typeof(APDocumentFilter.curyID))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Balance Discrepancy (Currency)", Enabled = false)]
			public virtual Decimal? CuryDifference
			{
				get
				{
					return (this._CuryBalanceSummary - (this._CuryVendorBalance + this._CuryVendorDepositsBalance));
				}
			}
			#endregion
			#region Difference
			public abstract class difference : PX.Data.IBqlField
			{
			}

            [PXDBBaseCury()]
            [PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Balance Discrepancy", Enabled = false)]
			public virtual Decimal? Difference
			{
				get
				{
					return (this._BalanceSummary - (this._VendorBalance + this._VendorDepositsBalance));
				}
			}
#endregion
			public virtual void ClearSummary() 
			{
				this._VendorBalance = Decimal.Zero;
				this._VendorDepositsBalance= Decimal.Zero;
				this._BalanceSummary = Decimal.Zero;
				this._CuryVendorBalance = Decimal.Zero;
				this._CuryVendorDepositsBalance = Decimal.Zero;				
				this.CuryBalanceSummary = Decimal.Zero;
			}
			
		}

		[Serializable()]
        [PXPrimaryGraph(typeof(APDocumentEnq), Filter = typeof(APDocumentFilter))]
		[PXCacheName(Messages.APHistoryForReport)]
        public partial class APHistoryForReport : APHistory
        { }
		[Serializable()]
		public partial class APDocumentResult : APRegister
		{
			#region DocType
			public new abstract class docType : PX.Data.IBqlField
			{
			}
			[PXDBString(3, IsKey = true, IsFixed = true)]
			[PXDefault()]
			[APDocType.List()]
			[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = true, TabOrder = 0)]
			public override String DocType
			{
				get
				{
					return this._DocType;
				}
				set
				{
					this._DocType = value;
				}
			}
			#endregion
			#region RefNbr
			public new abstract class refNbr : PX.Data.IBqlField
			{
			}
			[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
			[PXDefault()]
			[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.SelectorVisible, TabOrder = 1)]
			[PXSelector(typeof(Search<APRegister.refNbr>), Filterable = true)]
			public override String RefNbr
			{
				get
				{
					return this._RefNbr;
				}
				set
				{
					this._RefNbr = value;
				}
			}
			#endregion
			#region CuryOrigDocAmt
			[PXDefault(TypeCode.Decimal, "0.0")]
			//[PXDBCury(typeof(APRegister.curyInfoID))]
			[PXDBCurrency(typeof(APRegister.curyInfoID), typeof(APRegister.origDocAmt))]
			[PXUIField(DisplayName = "Currency Origin. Amount")]
			public override Decimal? CuryOrigDocAmt
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
			[PXDBBaseCury()]
			[PXUIField(DisplayName = "Origin. Amount")]
			[PXDefault(TypeCode.Decimal, "0.0")]
			public override Decimal? OrigDocAmt
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
			#region RGOLAmt
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName="RGOL Amount")]
			public override Decimal? RGOLAmt
			{
				get
				{
					return this._RGOLAmt;
				}
				set
				{
					this._RGOLAmt = value;
				}
			}
			#endregion
			#region ExtRefNbr
			public abstract class extRefNbr : PX.Data.IBqlField
			{
			}
			protected String _ExtRefNbr;
			[PXString(30, IsUnicode = true)]
			[PXUIField(DisplayName = "Vendor Invoice Nbr./Payment Nbr.")]
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
			#region PaymentMethodID
			public abstract class paymentMethodID : PX.Data.IBqlField
			{
			}
			protected String _PaymentMethodID;
			[PXString(10, IsUnicode = true)]
			[PXUIField(DisplayName = "Payment Method")]
		//	[PXSelector(typeof(PX.Objects.CA.PaymentMethod.paymentMethodID))]
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
			
			#region CuryBegBalance
			public abstract class curyBegBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryBegBalance;
			[PXCurrency(typeof(APRegister.curyInfoID),typeof(APDocumentResult.begBalance))]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Currency Period. Beg. Balance")]
			public virtual Decimal? CuryBegBalance
			{
				get
				{
					return this._CuryBegBalance;
				}
				set
				{
					this._CuryBegBalance = value;
				}
			}
			#endregion
			#region BegBalance
			public abstract class begBalance : PX.Data.IBqlField
			{
			}
			protected Decimal? _BegBalance;
			[PXBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Period. Beg. Balance")]
			public virtual Decimal? BegBalance
			{
				get
				{
					return this._BegBalance;
				}
				set
				{
					this._BegBalance = value;
				}
			}
			#endregion
			#region CuryDiscActTaken			
			public abstract class curyDiscActTaken : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryDiscActTaken;
			[PXDefault(TypeCode.Decimal, "0.0")]
			//[PXCury(typeof(APRegister.curyID))]
			[PXCurrency(typeof(APRegister.curyInfoID), typeof(APDocumentResult.discActTaken))]
			[PXUIField(DisplayName = "Currency Cash Discount Taken")]
			public virtual Decimal? CuryDiscActTaken
			{
				get
				{
					return this._CuryDiscActTaken;
				}
				set
				{
					this._CuryDiscActTaken = value;
				}
			}
			#endregion
			#region DiscActTaken
			public abstract class discActTaken : PX.Data.IBqlField
			{
			}
			protected Decimal? _DiscActTaken;
			[PXBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Cash Discount Taken")]
			public virtual Decimal? DiscActTaken
			{
				get
				{
					return this._DiscActTaken;
				}
				set
				{
					this._DiscActTaken = value;
				}
			}
			#endregion		
			#region CuryActualBalance
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(APRegister.curyInfoID), typeof(APRegister.docBal), BaseCalc = false)]
			[PXUIField(DisplayName = "Currency Balance")]
			public override Decimal? CuryDocBal
			{
				get
				{
					return this._CuryDocBal;
				}
				set
				{
					this._CuryDocBal = value;
				}
			}
			#endregion
			#region ActualBalance
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Balance")]
			public override Decimal? DocBal
			{
				get
				{
					return this._DocBal;
				}
				set
				{
					this._DocBal = value;
				}
			}
			#endregion
			#region CuryTaxWheld			
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXDBCurrency(typeof(APRegister.curyInfoID), typeof(APRegister.taxWheld))]
			[PXUIField(DisplayName = "Currency Tax Withheld")]
			public override Decimal? CuryTaxWheld
			{
				get
				{
					return this._CuryTaxWheld;
				}
				set
				{
					this._CuryTaxWheld = value;
				}
			}
			
			#endregion
			#region TaxWheld
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]			
			[PXUIField(DisplayName = "Tax Withheld")]
			public override Decimal? TaxWheld
			{
				get
				{
					return this._TaxWheld;
				}
				set
				{
					this._TaxWheld = value;
				}
			}

			#endregion
		}
		#endregion

		#region Views/Selects

		public PXSetup<APSetup> APSetup;
		public PXSetup<Company> Company;
		public PXFilter<APDocumentFilter> Filter;
		[PXFilterable]
		public PXSelectOrderBy<APDocumentResult, OrderBy<Desc<APDocumentResult.docDate>>> Documents;

        protected virtual IEnumerable documents()
        {
            APDocumentFilter header = Filter.Current;
            if (header == null)
            {
                yield break;
            }

            PXSelectBase<APDocumentResult> sel = new PXSelectReadonly2<APDocumentResult,
                InnerJoin<Sub, On<Sub.subID, Equal<APDocumentResult.aPSubID>>,
                LeftJoin<APInvoice, On<APInvoice.docType, Equal<APDocumentResult.docType>,
                And<APInvoice.refNbr, Equal<APDocumentResult.refNbr>>>,
                LeftJoin<APPayment, On<APPayment.docType, Equal<APDocumentResult.docType>,
                And<APPayment.refNbr, Equal<APDocumentResult.refNbr>>>>>>,
                Where<APRegister.vendorID, Equal<Current<APDocumentFilter.vendorID>>>,
                OrderBy<Desc<APDocumentResult.docDate>>>(this);

            if (header.BranchID != null)
            {
                sel.WhereAnd<Where<APRegister.branchID, Equal<Current<APDocumentFilter.branchID>>>>();
            }

            if (header.AccountID != null)
            {
                sel.WhereAnd<Where<APRegister.aPAccountID, Equal<Current<APDocumentFilter.accountID>>>>();
            }
            if ((header.IncludeUnreleased ?? false) == false)
            {
                sel.WhereAnd<Where<Where<APRegister.released, Equal<True>, Or<APRegister.prebooked, Equal<True>>>>>();
            }
            else
            {
                sel.WhereAnd<Where<APRegister.scheduled, Equal<False>, And<Where<APRegister.voided, Equal<False>, Or<APRegister.released, Equal<True>>>>>>();
            }
			if (!SubCDUtils.IsSubCDEmpty(header.SubCD))
            {
                sel.WhereAnd<Where<Sub.subCD, Like<Current<APDocumentFilter.subCDWildcard>>>>();
            }

            if (header.DocType != null)
            {
                sel.WhereAnd<Where<APRegister.docType, Equal<Current<APDocumentFilter.docType>>>>();
            }

            if (header.CuryID != null)
            {
                sel.WhereAnd<Where<APRegister.curyID, Equal<Current<APDocumentFilter.curyID>>>>();
            }
            bool byPeriod = (header.FinPeriodID != null);
            PXSelectBase<APAdjust> selectAdjusted = selectAdjusted = new PXSelectJoin<APAdjust,
                                                InnerJoin<APRegister, On<APAdjust.adjdDocType, Equal<APRegister.docType>, And<APAdjust.adjdRefNbr, Equal<APRegister.refNbr>>>>,
                                                Where<APAdjust.adjgDocType, Equal<Required<APAdjust.adjgDocType>>,
                                                And<APAdjust.adjgRefNbr, Equal<Required<APAdjust.adjgRefNbr>>>>>(this);

            PXSelectBase<APAdjust> selectAdjusting = new PXSelectJoin<APAdjust, InnerJoin<APRegister, On<APAdjust.adjgDocType, Equal<APRegister.docType>, And<APAdjust.adjgRefNbr, Equal<APRegister.refNbr>>>>,
                                                Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>,
                                                And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>>>>(this);
													
			PXSelectBase<APRegister> selectVoiding = new PXSelect<APRegister,
												Where<APRegister.docType, Equal<APDocType.voidCheck>, 
												And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>(this);

            if (!byPeriod)
            {
                if (header.ShowAllDocs == false)
                {
                    sel.WhereAnd<Where<APRegister.openDoc, Equal<True>>>();
                }
            }
            else
            {
                if (header.ByFinancialPeriod == true)
                {
                    sel.WhereAnd<Where<APRegister.finPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
                    sel.WhereAnd<Where<APRegister.closedFinPeriodID, GreaterEqual<Current<APDocumentFilter.finPeriodID>>,
                                        Or<APRegister.closedFinPeriodID, IsNull>>>();

                    selectAdjusted.WhereAnd<Where<APAdjust.adjgFinPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
                    selectAdjusting.WhereAnd<Where<APAdjust.adjgFinPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
					selectVoiding.WhereAnd<Where<APRegister.finPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
                }
                else
                {
                    sel.WhereAnd<Where<APRegister.tranPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
                    sel.WhereAnd<Where<APRegister.closedTranPeriodID, GreaterEqual<Current<APDocumentFilter.finPeriodID>>,
                                        Or<APRegister.closedTranPeriodID, IsNull>>>();

                    selectAdjusted.WhereAnd<Where<APAdjust.adjgTranPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
                    selectAdjusting.WhereAnd<Where<APAdjust.adjgTranPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
					selectVoiding.WhereAnd<Where<APRegister.tranPeriodID, LessEqual<Current<APDocumentFilter.finPeriodID>>>>();
                }
            }

            bool anyDoc = false;
            foreach (PXResult<APDocumentResult, Sub, APInvoice, APPayment> reg in sel.Select())
            {
				APDocumentResult res = (APDocumentResult)Documents.Cache.CreateCopy((APDocumentResult)reg);
                APInvoice invoice = reg;
                APPayment payment = reg;
                bool? isPaying = res.Paying;
                if (!isPaying.HasValue)
                {
                    //Invalid/unknown data - skip record - user notification???
                    continue;
                }
                if (!(invoice.RefNbr == null && res.Voided == true && payment.DocType == APDocType.Prepayment))
					if (res.Voided == true && res.Prebooked == true && res.DocType != APDocType.QuickCheck) continue;

                if (res.DocType == APDocType.Prepayment && (!String.IsNullOrEmpty(invoice.RefNbr) && String.IsNullOrEmpty(payment.RefNbr)))
                {
                    //Prepayment requiest- should not be cosidered if there is no "payment" part of it;
                    continue;
                }

                if (isPaying ?? false)
                {
                    res.ExtRefNbr = ((res.DocType == APDocType.Prepayment || res.DocType == APDocType.DebitAdj)
                            && !String.IsNullOrEmpty(invoice.RefNbr)) ? invoice.InvoiceNbr : payment.ExtRefNbr;
                    res.PaymentMethodID = payment.PaymentMethodID;
                    //res.RGOLAmt = payment.RGOLAmt; 
                }
                else
                {
                    res.ExtRefNbr = invoice.InvoiceNbr;
                    res.PaymentMethodID = null;
                }

                bool isPrepayment = (res.DocType == APDocType.Prepayment)
                                    && (!String.IsNullOrEmpty(invoice.RefNbr))
                                    && (!String.IsNullOrEmpty(payment.RefNbr));
                if (isPrepayment)
                {
                    res.OrigDocAmt = Decimal.Zero;
                    res.CuryOrigDocAmt = Decimal.Zero;
                }

                if (byPeriod)
                {
                    bool byFinPeriod = header.ByFinancialPeriod ?? false;
                    string periodToCompare = byFinPeriod ? res.FinPeriodID : res.TranPeriodID;
                    bool createdInPeriod = (periodToCompare == header.FinPeriodID);
                    res.DocBal = res.OrigDocAmt;
                    res.CuryDocBal = res.CuryOrigDocAmt;
                    res.BegBalance = (periodToCompare == header.FinPeriodID) ? Decimal.Zero : res.OrigDocAmt;
                    res.CuryBegBalance = (periodToCompare == header.FinPeriodID) ? Decimal.Zero : res.CuryOrigDocAmt;
                    res.DiscActTaken = Decimal.Zero;
                    res.CuryDiscActTaken = Decimal.Zero;
                    res.RGOLAmt = Decimal.Zero;
                    res.WhTaxBal = res.OrigWhTaxAmt;
                    res.CuryWhTaxBal = res.CuryOrigWhTaxAmt;

                    if (invoice != null && invoice.InstallmentCntr != null)
                    {
                        //Filter out master record for multiple installments
                        continue;
                    }
                    //Scan payments, which were applyed to invoice (for invoices)
                    foreach (PXResult<APAdjust, APRegister> iAdg in selectAdjusting.Select(res.DocType, res.RefNbr))
                    {
                        APAdjust it = iAdg;
                        APRegister iDoc = iAdg;
                        if (!(it.Released ?? false) && !(header.IncludeUnreleased ?? false)) continue;
                        if (isPrepayment)
                        {
							if(iDoc.Paying == true)
							{
								res.DocBal += iDoc.DocType != APDocType.Refund ? (it.AdjAmt + it.AdjDiscAmt + it.SignedRGOLAmt) : (-it.AdjAmt - it.SignedRGOLAmt);
								res.CuryDocBal += iDoc.DocType != APDocType.Refund ? (it.CuryAdjdAmt + it.CuryAdjdDiscAmt) : (-it.CuryAdjdAmt);	
								res.RGOLAmt += it.RGOLAmt;
							}
							else
							{
								res.DocBal += -it.AdjAmt;
								res.CuryDocBal += -it.CuryAdjdAmt;
							}

                            if (it.AdjgTranPeriodID != header.FinPeriodID)
                            {
                                res.BegBalance += iDoc.Paying.Value ? (it.AdjAmt + it.AdjDiscAmt + it.SignedRGOLAmt) : (-it.AdjAmt);
                                res.CuryBegBalance -= iDoc.Paying.Value ? (it.CuryAdjdAmt + it.CuryAdjdDiscAmt) : (-it.CuryAdjdAmt);
                            }
                        }
                        else
                        {
							decimal AdjDiscAmt = res.DocType == APDocType.QuickCheck ? 0m : (it.AdjDiscAmt ?? 0m);
							decimal CuryAdjdDiscAmt = res.DocType == APDocType.QuickCheck ? 0m : (it.CuryAdjdDiscAmt ?? 0m);

							res.DocBal -= (it.AdjAmt + AdjDiscAmt + it.SignedRGOLAmt);
							res.CuryDocBal -= (it.CuryAdjdAmt + CuryAdjdDiscAmt);
							res.DiscActTaken += AdjDiscAmt;
                            res.RGOLAmt += it.RGOLAmt;
                            res.WhTaxBal -= (it.AdjWhTaxAmt);
                            res.TaxWheld += (it.AdjWhTaxAmt);

                            res.CuryWhTaxBal -= (it.CuryAdjdWhTaxAmt);
                            res.CuryTaxWheld += (it.CuryAdjdWhTaxAmt);

							res.CuryDiscActTaken += CuryAdjdDiscAmt;
                            if (it.AdjgTranPeriodID != header.FinPeriodID)
                            {
								res.BegBalance -= (it.AdjAmt + AdjDiscAmt + it.AdjWhTaxAmt);
								res.CuryBegBalance -= (it.CuryAdjdAmt + CuryAdjdDiscAmt + it.CuryAdjdWhTaxAmt);
                            }
                        }
                    }
                    //Scan invoices, to which were  payment was applied (for checks)
                    //Quick checks must be excluded - applications for them are already considered in previous part
                    if (res.DocType != APDocType.QuickCheck && res.DocType != APDocType.VoidQuickCheck)
                    {

                        foreach (APAdjust it in selectAdjusted.Select(res.DocType, res.RefNbr))
                        {
                            if (!(it.Released ?? false) && !(header.IncludeUnreleased ?? false)) continue;
                            res.DocBal -= (it.AdjAmt * it.AdjgBalSign);
                            res.CuryDocBal -= (it.CuryAdjgAmt * it.AdjgBalSign);
                            if (it.AdjgTranPeriodID != header.FinPeriodID)
                            {
                                res.BegBalance -= (it.AdjAmt * it.AdjgBalSign) + it.SignedRGOLAmt;
                                res.CuryBegBalance -= (it.CuryAdjgAmt * it.AdjgBalSign);
                            }
                        }
                    }
					
					if (res.DocType == APDocType.VoidCheck)
					{
						res.DocBal = 0m;
						res.CuryDocBal = 0m;
						res.BegBalance = 0m;
						res.CuryBegBalance = 0m;
					}

					if ((res.Voided == true || header.IncludeUnreleased == true) &&
						(res.DocType == APDocType.Check || res.DocType == APDocType.Prepayment))
					{
						APRegister voidingDoc = selectVoiding.Select(res.RefNbr);
						if (voidingDoc != null &&
							(voidingDoc.Released == true || header.IncludeUnreleased == true))
						{
							res.DocBal = 0m;
							res.CuryDocBal = 0m;
							
							if (voidingDoc.TranPeriodID != header.FinPeriodID)
							{
								res.BegBalance = 0m;
								res.CuryBegBalance = 0m;
							}
						}
					}

                    if (res.BegBalance == Decimal.Zero && res.DocBal == Decimal.Zero && !createdInPeriod)
                        continue; //Filter out Documents which were not chage during the period
                }
                else
                {
                    res.CuryDiscActTaken = res.CuryDiscTaken ?? Decimal.Zero;
                    res.DiscActTaken = res.DiscTaken ?? Decimal.Zero;
                }

                anyDoc = true;
				res.RGOLAmt = -res.RGOLAmt;
                if (res.SignAmount.Value.Equals(-1m))
                {
                    InvertValuesSign(res);
                }
                yield return res;
            }
            viewDocument.SetEnabled(anyDoc);
        }

        protected virtual IEnumerable filter()
        {
            PXCache cache = this.Caches[typeof(APDocumentFilter)];
            if (cache != null)
            {
                APDocumentFilter filter = cache.Current as APDocumentFilter;
                if (filter != null)
                {
                    filter.ClearSummary();
                    bool byPeriod = !string.IsNullOrEmpty(filter.FinPeriodID);
                    foreach (APDocumentResult it in this.Documents.Select())
                    {
                        Aggregate(filter, it);
                    }
                    if (filter.VendorID != null)
                    {
                        APVendorBalanceEnq balanceBO = PXGraph.CreateInstance<APVendorBalanceEnq>();
                        APVendorBalanceEnq.APHistoryFilter histFilter = balanceBO.Filter.Current;
                        APVendorBalanceEnq.Copy(histFilter, filter);
                        balanceBO.Filter.Update(histFilter);
                        foreach (APVendorBalanceEnq.APHistoryResult iRes in balanceBO.History.Select())
                        {
                            Aggregate(filter, iRes);
                        }
                    }
                }
            }
            yield return cache.Current;
            cache.IsDirty = false;
        }

		#endregion
		
		#region Ctor +  Overrides
		public APDocumentEnq()
		{
			APSetup setup = this.APSetup.Current;
			Company company = this.Company.Current;
			Documents.Cache.AllowDelete = false;
			Documents.Cache.AllowInsert = false;
			Documents.Cache.AllowUpdate = false;

			PXUIFieldAttribute.SetVisibility<APRegister.finPeriodID>(Documents.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<APRegister.vendorID>(Documents.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<APRegister.vendorLocationID>(Documents.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<APRegister.curyDiscBal>(Documents.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<APRegister.curyOrigDocAmt>(Documents.Cache, null, PXUIVisibility.Visible);
			PXUIFieldAttribute.SetVisibility<APRegister.curyDiscTaken>(Documents.Cache, null, PXUIVisibility.Visible);

			this.actionsFolder.MenuAutoOpen = true;
			this.actionsFolder.AddMenuAction(this.createInvoice);
			this.actionsFolder.AddMenuAction(this.createPayment);
			this.actionsFolder.AddMenuAction(this.payDocument);

			this.reportsFolder.MenuAutoOpen = true;
			this.reportsFolder.AddMenuAction(this.aPBalanceByVendorReport);
			this.reportsFolder.AddMenuAction(this.vendorHistoryReport);
			this.reportsFolder.AddMenuAction(this.aPAgedPastDueReport);
			this.reportsFolder.AddMenuAction(this.aPAgedOutstandingReport);
			this.reportsFolder.AddMenuAction(this.aPRegisterReport);
		}
		
		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region Actions

		public PXCancel<APDocumentFilter> Cancel;
		public PXAction<APDocumentFilter> viewDocument;
		public PXAction<APDocumentFilter> previousPeriod;
		public PXAction<APDocumentFilter> nextPeriod;

		public PXAction<APDocumentFilter> actionsFolder;
		public PXAction<APDocumentFilter> createInvoice;
		public PXAction<APDocumentFilter> createPayment;
		public PXAction<APDocumentFilter> payDocument;

		public PXAction<APDocumentFilter> reportsFolder;
		public PXAction<APDocumentFilter> aPBalanceByVendorReport;
		public PXAction<APDocumentFilter> vendorHistoryReport;
		public PXAction<APDocumentFilter> aPAgedPastDueReport;
		public PXAction<APDocumentFilter> aPAgedOutstandingReport;
		public PXAction<APDocumentFilter> aPRegisterReport;

        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXEditDetailButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry)]
        public virtual IEnumerable ViewDocument(PXAdapter adapter)
        {
            if (this.Documents.Current != null)
            {
                PXRedirectHelper.TryRedirect(Documents.Cache, Documents.Current, "Document", PXRedirectHelper.WindowMode.NewWindow);
            }
            return Filter.Select();
        }

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXPreviousButton]
		public virtual IEnumerable PreviousPeriod(PXAdapter adapter)
		{
			APDocumentFilter filter = Filter.Current as APDocumentFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindPrevPeriod(this, filter.FinPeriodID, true);
			if (nextperiod != null)
			{
				filter.FinPeriodID = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXNextButton]
		public virtual IEnumerable NextPeriod(PXAdapter adapter)
		{
			APDocumentFilter filter = Filter.Current as APDocumentFilter;
			FinPeriod nextperiod = FiscalPeriodUtils.FindNextPeriod(this, filter.FinPeriodID, true);
			if (nextperiod != null)
			{
				filter.FinPeriodID = nextperiod.FinPeriodID;
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = "Actions", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Actionsfolder(PXAdapter adapter)
		{
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.NewInvoice, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable CreateInvoice(PXAdapter adapter)
		{
			if (Filter.Current?.VendorID != null)
			{
				APInvoiceEntry invoiceEntry = PXGraph.CreateInstance<APInvoiceEntry>();
				invoiceEntry.Clear();

				APInvoice newDoc = invoiceEntry.Document.Insert(new APInvoice());
				newDoc.VendorID = Filter.Current.VendorID;

				object oldVendorValue = null;
				object newVendorValue = Filter.Current.VendorID;

				invoiceEntry.Document.Cache.RaiseFieldVerifying<APInvoice.vendorID>(
					newDoc,
					ref newVendorValue);

				invoiceEntry.Document.Cache.RaiseFieldUpdated<APInvoice.vendorID>(
					newDoc,
					oldVendorValue);

				throw new PXRedirectRequiredException(invoiceEntry, nameof(CreateInvoice));
			}

			return Filter.Select();
		}
            
		[PXUIField(DisplayName = Messages.NewPayment, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable CreatePayment(PXAdapter adapter)
		{
			if (Filter.Current?.VendorID != null)
			{
				APPaymentEntry paymentEntry = PXGraph.CreateInstance<APPaymentEntry>();
				paymentEntry.Clear();

				APPayment newDoc = paymentEntry.Document.Insert(new APPayment());
				newDoc.VendorID = Filter.Current.VendorID;

				object oldVendorValue = null;
				object newVendorValue = Filter.Current.VendorID;

				paymentEntry.Document.Cache.RaiseFieldVerifying<APPayment.vendorID>(
					newDoc,
					ref newVendorValue);

				paymentEntry.Document.Cache.RaiseFieldUpdated<APPayment.vendorID>(
					newDoc,
					oldVendorValue);

				throw new PXRedirectRequiredException(paymentEntry, nameof(CreatePayment));
			}

			return Filter.Select();
		}

		[PXUIField(DisplayName = Messages.APPayBill, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton]
		public virtual IEnumerable PayDocument(PXAdapter adapter)
		{
			if (Documents.Current != null)
			{
				if (!(this.Documents.Current.OpenDoc ?? false))
				{
					throw new PXException(Messages.Only_Open_Documents_MayBe_Processed);
				}
								
				bool? isInvoice = this.Documents.Current.Payable;
				if (!isInvoice.HasValue)
					throw new PXException(Messages.UnknownDocumentType);
				if (isInvoice ?? false)
				{
					APInvoice doc = this.FindInvoice(Documents.Current);
					if (doc != null)
					{
						APPaymentEntry graph = PXGraph.CreateInstance<APPaymentEntry>();
						graph.Clear();
						try
						{
							graph.CreatePayment(doc);
						}
						catch (AdjustedNotFoundException)
						{
							APAdjust foundAPAdjust = PXSelect<APAdjust,
								Where<APAdjust.adjdDocType, Equal<Required<APAdjust.adjdDocType>>,
								And<APAdjust.adjdRefNbr, Equal<Required<APAdjust.adjdRefNbr>>,
								And<APAdjust.released, Equal<False>>>>>.
								Select(graph, doc.DocType, doc.RefNbr);
							if (foundAPAdjust != null)
							{
								graph.Clear();
								graph.Document.Current = graph.Document.Search<APPayment.refNbr>(foundAPAdjust.AdjgRefNbr, foundAPAdjust.AdjgDocType);
								throw new PXRedirectRequiredException(graph, "PayInvoice");
							}
						}
						throw new PXRedirectRequiredException(graph, "PayInvoice");
					}
				}
				else
				{
					throw new PXException(Messages.Only_Invoices_MayBe_Payed);
				}
			}
			return Filter.Select();
		}

		[PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
		[PXButton(SpecialType = PXSpecialButtonType.Report)]
		protected virtual IEnumerable Reportsfolder(PXAdapter adapter)
		{
			return adapter.Get();
        }
		
        [PXUIField(DisplayName = Messages.APBalanceByVendorReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable APBalanceByVendorReport(PXAdapter adapter)
        {
            APDocumentFilter filter = Filter.Current;

            if (filter != null)
            {
                Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APDocumentFilter.vendorID>>>>.Select(this);
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(filter.FinPeriodID))
                {
                    parameters["PeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
                }
                parameters["VendorID"] = vendor.AcctCD;
                throw new PXReportRequiredException(parameters, "AP632500", Messages.APBalanceByVendorReport);
            }
            return adapter.Get();
        }

        [PXUIField(DisplayName = Messages.VendorHistoryReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable VendorHistoryReport(PXAdapter adapter)
        {
            APDocumentFilter filter = Filter.Current;
            if (filter != null)
            {
                Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APDocumentFilter.vendorID>>>>.Select(this);
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(filter.FinPeriodID))
                {
                    parameters["FromPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
                    parameters["ToPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
                }
                parameters["VendorID"] = vendor.AcctCD;
                throw new PXReportRequiredException(parameters, "AP652000", Messages.VendorHistoryReport);
            }
            return adapter.Get();
        }

        [PXUIField(DisplayName = Messages.APAgedPastDueReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable APAgedPastDueReport(PXAdapter adapter)
        {
            APDocumentFilter filter = Filter.Current;
            if (filter != null)
            {
                Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APDocumentFilter.vendorID>>>>.Select(this);
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["VendorID"] = vendor.AcctCD;
                throw new PXReportRequiredException(parameters, "AP631000", Messages.APAgedPastDueReport);
            }
            return adapter.Get();
        }

        [PXUIField(DisplayName = Messages.APAgedOutstandingReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable APAgedOutstandingReport(PXAdapter adapter)
        {
            APDocumentFilter filter = Filter.Current;
            if (filter != null)
            {
                Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APDocumentFilter.vendorID>>>>.Select(this);
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["VendorID"] = vendor.AcctCD;
                throw new PXReportRequiredException(parameters, "AP631500", Messages.APAgedOutstandingReport);
            }
            return adapter.Get();
        }

        [PXUIField(DisplayName = Messages.APRegisterReport, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable APRegisterReport(PXAdapter adapter)
        {
            APDocumentFilter filter = Filter.Current;
            if (filter != null)
            {
                Vendor vendor = PXSelect<Vendor, Where<Vendor.bAccountID, Equal<Current<APDocumentFilter.vendorID>>>>.Select(this);
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(filter.FinPeriodID))
                {
                    parameters["StartPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
                    parameters["EndPeriodID"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.FinPeriodID);
                }
                parameters["VendorID"] = vendor.AcctCD;
                throw new PXReportRequiredException(parameters, "AP621500", Messages.APRegisterReport);
            }
            return adapter.Get();
        }
        #endregion

		#region Events Handlers
		public virtual void APDocumentFilter_AccountID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APDocumentFilter header = e.Row as APDocumentFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.AccountID = null;
			}
		}
		public virtual void APDocumentFilter_CuryID_ExceptionHandling(PXCache cache, PXExceptionHandlingEventArgs e)
		{
			APDocumentFilter header = e.Row as APDocumentFilter;
			if (header != null)
			{
				e.Cancel = true;
				header.CuryID = null;
			}
		}
		public virtual void APDocumentFilter_SubID_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			e.Cancel = true;
		}
		public virtual void APDocumentFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;
            
            APDocumentFilter row = (APDocumentFilter)e.Row;
			bool byPeriod   = (row.FinPeriodID != null);
			PXCache docCache = Documents.Cache;
			bool isMCFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
			bool isForeignCurrencySelected = String.IsNullOrEmpty(row.CuryID) == false && (row.CuryID != this.Company.Current.BaseCuryID);
			bool isBaseCurrencySelected = String.IsNullOrEmpty(row.CuryID) == false && (row.CuryID == this.Company.Current.BaseCuryID);

            PXUIFieldAttribute.SetVisible<APDocumentFilter.showAllDocs>(cache, row, !byPeriod);
			PXUIFieldAttribute.SetVisible<APDocumentFilter.byFinancialPeriod>(cache, row, byPeriod);
			PXUIFieldAttribute.SetVisible<APDocumentFilter.curyID>(cache, row, isMCFeatureInstalled);
			PXUIFieldAttribute.SetVisible<APDocumentFilter.curyBalanceSummary>(cache, row, isMCFeatureInstalled && isForeignCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentFilter.curyDifference>(cache, row, isMCFeatureInstalled && isForeignCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentFilter.curyVendorBalance>(cache, row, isMCFeatureInstalled && isForeignCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentFilter.curyVendorDepositsBalance>(cache, row, isMCFeatureInstalled && isForeignCurrencySelected);

			PXUIFieldAttribute.SetVisible<APDocumentResult.curyID>(docCache, null, isMCFeatureInstalled);
			PXUIFieldAttribute.SetVisible<APDocumentResult.rGOLAmt>(docCache, null, isMCFeatureInstalled && !isBaseCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentResult.curyBegBalance>(docCache, null, byPeriod && isMCFeatureInstalled && !isBaseCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentResult.begBalance>(docCache, null, byPeriod);
			PXUIFieldAttribute.SetVisible<APDocumentResult.curyOrigDocAmt>(docCache, null, isMCFeatureInstalled && !isBaseCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentResult.curyDocBal>(docCache, null, isMCFeatureInstalled && !isBaseCurrencySelected);
			PXUIFieldAttribute.SetVisible<APDocumentResult.curyDiscActTaken>(docCache, null, isMCFeatureInstalled && !isBaseCurrencySelected);			
			PXUIFieldAttribute.SetVisible<APDocumentResult.curyTaxWheld>(docCache, null, isMCFeatureInstalled && !isBaseCurrencySelected);
								
			//PXUIFieldAttribute.SetEnabled<APDocumentResult.status>(docCache, null, true);//???
			//PXUIFieldAttribute.SetEnabled<APDocumentResult.curyDocBal>(docCache, null, true);//???

            aPBalanceByVendorReport.SetEnabled(row.VendorID != null);
            vendorHistoryReport.SetEnabled(row.VendorID != null);
            aPAgedPastDueReport.SetEnabled(row.VendorID != null);
            aPAgedOutstandingReport.SetEnabled(row.VendorID != null);
            aPRegisterReport.SetEnabled(row.VendorID != null);
        }
		#endregion	
        
		#region Utility Functions - internal
		protected virtual void Aggregate(APDocumentFilter aDest, APVendorBalanceEnq.APHistoryResult aSrc) 
		{
			aDest.VendorBalance += aSrc.EndBalance ?? Decimal.Zero;
			aDest.VendorDepositsBalance += aSrc.DepositsBalance ?? Decimal.Zero; //Consider sign correctly - now its negative in the summary enquiry too
			aDest.CuryVendorBalance += aSrc.CuryEndBalance ?? Decimal.Zero;
			aDest.CuryVendorDepositsBalance += aSrc.CuryDepositsBalance ?? Decimal.Zero; //Consider sign correctly - now its negative in the summary enquiry too

		}
		protected virtual void Aggregate(APDocumentFilter aDest, APDocumentResult aSrc)
		{
			aDest.BalanceSummary += aSrc.DocBal ?? Decimal.Zero; 
			aDest.CuryBalanceSummary += aSrc.CuryDocBal ?? Decimal.Zero;
		}

		protected virtual APInvoice FindInvoice(APDocumentResult aRes) 
		{
			APInvoice doc = FindInvoice(this,aRes.DocType, aRes.RefNbr);
			return doc;
		}
		protected virtual APPayment FindPayment(APDocumentResult aRes)
		{
			return FindPayment(this, aRes.DocType, aRes.RefNbr);
		}
		protected virtual void InvertValuesSign(APDocumentResult aRes)
		{
			aRes.OrigDocAmt = -aRes.OrigDocAmt;
			aRes.DocBal = -aRes.DocBal;
			aRes.BegBalance = -aRes.BegBalance;
			aRes.DiscActTaken = -aRes.DiscActTaken;
			aRes.DiscTaken = -aRes.DiscTaken;
			aRes.DiscBal = -aRes.DiscBal;
			aRes.OrigDiscAmt = -aRes.OrigDiscAmt;
			aRes.OrigWhTaxAmt = -aRes.OrigWhTaxAmt;
			aRes.WhTaxBal = -aRes.WhTaxBal;
			aRes.TaxWheld = -aRes.TaxWheld;
			aRes.CuryOrigDocAmt = -aRes.CuryOrigDocAmt;
			aRes.CuryDocBal = -aRes.CuryDocBal;
			aRes.CuryBegBalance = -aRes.CuryBegBalance;
			aRes.CuryDiscActTaken = -aRes.CuryDiscActTaken;
			aRes.CuryDiscTaken = -aRes.CuryDiscTaken;
			aRes.CuryOrigDiscAmt = -aRes.CuryOrigDiscAmt;
			aRes.CuryDiscBal = -aRes.CuryDiscBal;
			aRes.CuryWhTaxBal = -aRes.CuryWhTaxBal;
			aRes.CuryOrigWhTaxAmt = -aRes.CuryOrigWhTaxAmt;
			aRes.CuryTaxWheld = -aRes.CuryTaxWheld;
			

		}
	
		#endregion

		#region Utility Functions - public
		public static APInvoice FindInvoice(PXGraph aGraph, string aDocType, string apRefNbr) 
		{
			APInvoice doc = PXSelect<APInvoice,
						Where<APInvoice.docType, Equal<Required<APInvoice.docType>>,
						And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>
						.Select(aGraph, aDocType, apRefNbr);
			return doc;
		}
		public static APPayment FindPayment(PXGraph aGraph, string aDocType, string apRefNbr)
		{
			APPayment doc = PXSelect<APPayment,
						Where<APPayment.docType, Equal<Required<APPayment.docType>>,
						And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>
						.Select(aGraph, aDocType, apRefNbr);
			return doc;
		}
		
		public static bool? IsInvoiceType(string aDocType) 
		{
			return APDocType.Payable(aDocType);			
		}
		#endregion
	}
}
