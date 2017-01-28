using System;

using PX.Data;

using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.FA;
using PX.Objects.PO;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects.DR;
using PX.Objects.PM;

namespace PX.Objects.AP
{
		
    /// <summary>
    /// Represents the individual lines of the Accounts Payable documents.
    /// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.APTran)]
	public partial class APTran : IBqlTable, DR.Descriptor.IDocumentLine
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;

        /// <summary>
        /// Indicates whether the record is selected for mass processing or not.
        /// </summary>
		[PXBool]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.GL.Branch">Branch</see>, to which the transaction belongs.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.GL.Branch.BranchID">Branch.BranchID</see> field.
        /// </value>
		[Branch(typeof(APRegister.branchID))]
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
		#region TranType
		public abstract class tranType : PX.Data.IBqlField
		{
		}
		protected String _TranType;

		/// <summary>
		/// [key] The type of the transaction.
		/// </summary>
		/// <value>
		/// The field is determined by the type of the parent <see cref="APRegister">document</see>.
		/// For the list of possible values see <see cref="APRegister.DocType"/>.
		/// </value>
		[APDocType.List()]
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDBDefault(typeof(APRegister.docType))]
		[PXUIField(DisplayName="Tran. Type",Visibility=PXUIVisibility.Visible,Visible=false)]
		public virtual String TranType
		{
			get
			{
				return this._TranType;
			}
			set
			{
				this._TranType = value;
			}
		}
		#endregion
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;

        /// <summary>
        /// [key] Reference number of the parent <see cref="APRegister">document</see>.
        /// </summary>
		[PXDBString(15, IsUnicode = true, IsKey = true)]
		[PXDBDefault(typeof(APRegister.refNbr))]
		[PXUIField(DisplayName = "Reference Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXParent(typeof(Select<APRegister, Where<APRegister.docType, Equal<Current<APTran.tranType>>, And<APRegister.refNbr, Equal<Current<APTran.refNbr>>>>>))]
		public virtual String RefNbr
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
		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;

        /// <summary>
        /// [key] The number of the transaction line in the document.
        /// </summary>
        /// <value>
        /// Note that the sequence of line numbers of the transactions belonging to a single document may include gaps.
        /// </value>
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false)]
		[PXLineNbr(typeof(APRegister.lineCntr))]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int32? _TranID;

        /// <summary>
        /// Internal unique identifier of the transaction line.
        /// </summary>
        /// <value>
        /// The value is an auto-generated database identity.
        /// </value>
		[PXDBIdentity()]
		public virtual Int32? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
			}
		}
		#endregion

		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;

        /// <summary>
        /// Identifier of the <see cref="Vendor"/>, whom the parent document belongs.
        /// </summary>
		[Vendor()]
		[PXDBDefault(typeof(APRegister.vendorID))]
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
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;

        /// <summary>
        /// Number of the <see cref="Batch"/>, generated for the transaction on release.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Batch.BatchNbr">Batch.BatchNbr</see> field.
        /// </value>
		[PXDBString(15, IsUnicode = true)]
		[PXDefault("")]
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region LineType
		public abstract class lineType : PX.Data.IBqlField
		{
		}
		protected String _LineType;

        /// <summary>
        /// The type of the transaction line. This field is used to distinguish Discount lines from other ones.
        /// </summary>
        /// <value>
        /// Equals <c>"DS"</c> for discounts and empty string for common lines.
        /// </value>
		[PXDBString(2, IsFixed = true)]
		[PXDefault("")]
		public virtual String LineType
		{
			get
			{
				return this._LineType;
			}
			set
			{
				this._LineType = value;
			}
		}
		#endregion
		#region POOrderType
		public abstract class pOOrderType : PX.Data.IBqlField
		{
		}
		protected String _POOrderType;

        /// <summary>
        /// The type of the corresponding <see cref="PX.Objects.PO.POOrder">PO Order</see>.
        /// Together with <see cref="PONbr"/> and <see cref="POLineNbr"/> links APTrans to the PO Orders and their lines.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.POOrder.OrderType">POOrder.OrderType</see> field.
        /// See its description for the list of allowed values.
        /// </value>
		[PXDBString(2, IsFixed = true)]
		[PO.POOrderType.List()]
		[PXUIField(DisplayName = "PO Type", Enabled = false, IsReadOnly=true)]
		public virtual String POOrderType
		{
			get
			{
				return this._POOrderType;
			}
			set
			{
				this._POOrderType = value;
			}
		}
		#endregion
		#region PONbr
		public abstract class pONbr : PX.Data.IBqlField
		{
		}
		protected String _PONbr;

        /// <summary>
        /// The reference number of the corresponding <see cref="PX.Objects.PO.POOrder">PO Order</see>.
        /// Together with <see cref="POOrderType"/> and <see cref="POLineNbr"/> links APTrans to the PO Orders and their lines.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.POOrder.OrderNbr">POOrder.OrderNbr</see> field.
        /// </value>
		[PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PO Number", Enabled = false, IsReadOnly = true)]
		[PXSelector(typeof(Search<POOrder.orderNbr, Where<POOrder.orderType, Equal<Optional<APTran.pOOrderType>>>>))]
		public virtual String PONbr
		{
			get
			{
				return this._PONbr;
			}
			set
			{
				this._PONbr = value;
			}
		}
		#endregion
		#region POLineNbr
		public abstract class pOLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _POLineNbr;

        /// <summary>
        /// The line number of the corresponding <see cref="PX.Objects.PO.POLine">PO Line</see>.
        /// Together with <see cref="POOrderType"/> and <see cref="PONbr"/> links AP transactions to the PO Orders and their lines.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.POLine.LineNbr">POLine.LineNbr</see> field.
        /// </value>
		[PXDBInt()]
		[PXParent(typeof(Select<PO.POLineAP, Where<PO.POLineAP.orderType, Equal<Current<APTran.pOOrderType>>,
													And<PO.POLineAP.orderNbr, Equal<Current<APTran.pONbr>>,
													And<PO.POLineAP.lineNbr, Equal<Current<APTran.pOLineNbr>>,
													And<Current<APTran.receiptNbr>,IsNull>>>>>))]
		[PXUIField(DisplayName = "PO Line", Enabled = false, IsReadOnly = true, Visible = false)]
		public virtual Int32? POLineNbr
		{
			get
			{
				return this._POLineNbr;
			}
			set
			{
				this._POLineNbr = value;
			}
		}
		#endregion
        #region ReceiptType
        public abstract class receiptType : PX.Data.IBqlField
        {
        }
        protected String _ReceiptType;
        [PXDBString(2, IsFixed = true)]
        public virtual String ReceiptType
        {
            get
            {
                return this._ReceiptType;
            }
            set
            {
                this._ReceiptType = value;
            }
        }
        #endregion
		#region ReceiptNbr
		public abstract class receiptNbr : PX.Data.IBqlField
		{
		}
		protected String _ReceiptNbr;

        /// <summary>
        /// The reference number of the corresponding <see cref="PX.Objects.PO.POReceipt">PO Receipt</see>.
        /// Together with <see cref="ReceiptLineNbr"/> field links AP transactions to PO Receipts and their lines.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.POReceipt.ReceiptNbr">POReceipt.ReceiptNbr</see> field.
        /// </value>
		[PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "PO Receipt Nbr.", Enabled = false, IsReadOnly = true)]
		[PXSelector(typeof(Search<POReceipt.receiptNbr>))]
		public virtual String ReceiptNbr
		{
			get
			{
				return this._ReceiptNbr;
			}
			set
			{
				this._ReceiptNbr = value;
			}
		}
		#endregion
		#region ReceiptLineNbr
		public abstract class receiptLineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _ReceiptLineNbr;

        /// <summary>
        /// The number of the corresponding line in the related <see cref="PX.Objects.PO.POReceipt">PO Receipt</see>.
        /// Together with <see cref="ReceiptNbr"/> field links AP transactions to PO Receipts and their lines.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.POReceiptLine.LineNbr">POReceiptLine.LineNbr</see> field.
        /// </value>
		[PXDBInt()]
		[PXParent(typeof(Select<PO.POReceiptLineR, Where<PO.POReceiptLineR.receiptNbr, Equal<Current<APTran.receiptNbr>>,
														And<PO.POReceiptLineR.lineNbr, Equal<Current<APTran.receiptLineNbr>>>>>))]
		[PXUIField(DisplayName = "PO Receipt Line", Enabled = false, IsReadOnly = true, Visible = false)]
		public virtual Int32? ReceiptLineNbr
		{
			get
			{
				return this._ReceiptLineNbr;
			}
			set
			{
				this._ReceiptLineNbr = value;
			}
		}
		#endregion
		#region LCTranID
		public abstract class lCTranID : PX.Data.IBqlField
		{
		}
		protected Int32? _LCTranID;

        /// <summary>
        /// Identifier of the related <see cref="PX.Objects.PO.LandedCostTran">landed cost transaction</see>.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.LandedCostTran.LCTranID">LandedCostTran.LCTranID</see> field.
        /// </value>
		[PXDBInt()]
		//[PXDBLiteDefault(typeof(APLandedCostTran.lCTranID),PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(Enabled = false, Visible = false)]
		public virtual Int32? LCTranID
		{
			get
			{
				return this._LCTranID;
			}
			set
			{
				this._LCTranID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;

		//[NonStockItem(Filterable = true)]
        /// <summary>
        /// Identifier of the <see cref="PX.Objects.IN.InventoryItem">inventory item</see> associated with the transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.IN.InventoryItem.InventoryID">InventoryItem.InventoryID</see> field.
        /// </value>
		[APTranInventoryItem(Filterable = true)]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;

        /// <summary>
        /// Identifier of the <see cref="Account">expense account</see> to be updated by the transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Account.AccountID">Account.AccountID</see> field. Defaults to the 
        /// <see cref="InventoryItem.COGSAcctID">Cost of Goods Sold account</see> associated with the inventory item.
        /// </value>
		[Account(typeof(APTran.branchID), DisplayName = "Account", Visibility = PXUIVisibility.Visible, Filterable=false, DescriptionField = typeof(Account.description))]
		[PXDefault(typeof(Search<InventoryItem.cOGSAcctID, Where<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>))]
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
		#region SubID
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected int? _SubID;

        /// <summary>
        /// Identifier of the <see cref="Sub">Subaccount</see> associated with the transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="Sub.SubID">Sub.SubID</see> field. Defaults to the 
        /// <see cref="InventoryItem.COGSSubID">Cost of Goods Sold subaccount</see> associated with the inventory item.
        /// </value>
		[SubAccount(typeof(APTran.accountID), typeof(APTran.branchID), true, DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, Filterable = true)]
		[PXDefault(typeof(Search<InventoryItem.cOGSSubID, Where<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>))]
		public virtual int? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;

        /// <summary>
        /// The <see cref="PX.Objects.PM.PMProject">project</see> with which the item is associated or the non-project code if the item is not intended for any project.
        /// The field is relevant only if the <see cref="PX.Objects.CS.FeaturesSet.ProjectModule">Project Module</see> is enabled.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PM.PMProject.ProjectID">PMProject.ProjectID</see> field.
        /// Defaults to the <see cref="PX.Objects.CR.Location.VDefProjectID">default project of the vendor</see>.
        /// </value>
		[ProjectDefault(BatchModule.AP, typeof(Search<Location.vDefProjectID, Where<Location.bAccountID, Equal<Current<APInvoice.vendorID>>, And<Location.locationID, Equal<Current<APInvoice.vendorLocationID>>>>>), typeof(APTran.accountID))]
		[APActiveProjectAttibute()]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region TaskID
		public abstract class taskID : PX.Data.IBqlField
		{
		}
		protected Int32? _TaskID;

        /// <summary>
        /// Identifier of the particular <see cref="PX.Objects.PM.PMTask">task</see> associated with the transaction. The task belongs to the <see cref="ProjectID">selected project</see> 
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PM.PMTask.TaskID">PMTask.TaskID</see> field.
        /// </value>
		[ActiveProjectTask(typeof(APTran.projectID), BatchModule.AP, DisplayName = "Project Task")]
		public virtual Int32? TaskID
		{
			get
			{
				return this._TaskID;
			}
			set
			{
				this._TaskID = value;
			}
		}
		#endregion
		#region Box1099
		public abstract class box1099 : PX.Data.IBqlField
		{
		}
		protected Int16? _Box1099;

        /// <summary>
        /// Identifier of the <see cref="AP1099Box">1099 Box</see> associated with the line.
        /// </summary>
        /// <value>
        /// Defaults to the 1099 Box associated with the <see cref="APTran.AccountID">expense account</see> or with the <see cref="APTran.VendorID">vendor</see>.
        /// </value>
		[PXDBShort()]
		[PXIntList(new int[] { 0 }, new string[] { "undefined" })]
		[PXDefault(typeof(Coalesce<Search<Account.box1099,Where<Account.accountID,Equal<Current<APTran.accountID>>>>, Search<Vendor.box1099, Where<Vendor.bAccountID, Equal<Current<APTran.vendorID>>>>>), PersistingCheck=PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName="1099 Box", Visibility=PXUIVisibility.Visible)]
		public virtual Int16? Box1099
		{
			get
			{
				return this._Box1099;
			}
			set
			{
				this._Box1099 = value;
			}
		}
		#endregion
        #region TaxID
        public abstract class taxID : PX.Data.IBqlField
        {
        }
        protected String _TaxID;

        /// <summary>
        /// The identifier of the <see cref="PX.Objects.TX.Tax">tax</see> associated with the line.
        /// </summary>
        [PXDBString(Tax.taxID.Length, IsUnicode = true)]
        [PXUIField(DisplayName = "Tax ID", Visible = false)]
		[PXSelector(typeof(Tax.taxID), DescriptionField = typeof(Tax.descr))]
        public virtual String TaxID
        {
            get
            {
                return this._TaxID;
            }
            set
            {
                this._TaxID = value;
            }
        }
        #endregion
		#region DeferredCode
		public abstract class deferredCode : PX.Data.IBqlField
		{
		}
		protected String _DeferredCode;

        /// <summary>
        /// The field holds one of the deferral codes defined in the system if the bill represents deferred expense.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="InventoryItem.DeferredCode">deferral code associated with the inventory item</see> 
        /// with "Expense" <see cref="DRDeferredCode.AccountType">account type</see>.
        /// </value>
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Deferral Code")]
		[PXSelector(typeof(Search<DRDeferredCode.deferredCodeID, 
			Where<DRDeferredCode.accountType, Equal<DeferredAccountType.expense>,
			And<DRDeferredCode.method, NotEqual<DeferredMethodType.cashReceipt>,
			Or<DRDeferredCode.multiDeliverableArrangement, Equal<True>>>>>))]
		[PXDefault(typeof(Search2<InventoryItem.deferredCode, InnerJoin<DRDeferredCode, On<DRDeferredCode.deferredCodeID, Equal<InventoryItem.deferredCode>>>, Where<DRDeferredCode.accountType, Equal<DeferredAccountType.expense>, And<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String DeferredCode
		{
			get
			{
				return this._DeferredCode;
			}
			set
			{
				this._DeferredCode = value;
			}
		}
		#endregion
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;

        /// <summary>
        /// Identifier of the <see cref="PX.Objects.CM.CurrencyInfo">CurrencyInfo</see> object associated with the transaction.
        /// </summary>
        /// <value>
        /// Generated automatically. Corresponds to the <see cref="PX.Objects.CM.CurrencyInfo.CurrencyInfoID"/> field.
        /// </value>
		[PXDBLong()]
		[CurrencyInfo(typeof(APRegister.curyInfoID))]
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
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDBInt()]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;

        /// <summary>
        /// The <see cref="PX.Objects.IN.INUnit">unit of measure</see> for the transaction.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.IN.INUnit.FromUnit">INUnit.FromUnit</see> field.
        /// </value>
		[PXDefault(typeof(Search<InventoryItem.purchaseUnit, Where<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		[INUnit(typeof(APTran.inventoryID))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;

        /// <summary>
        /// The quantity of the items or services associated with the line delivered by the vendor.
        /// </summary>
		[PXDBQuantity()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUnboundFormula(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.qty>, Minus<APTran.qty>>), typeof(SumCalc<PO.POReceiptLineR.signedVoucheredQty>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.qty>, Minus<APTran.qty>>), typeof(SumCalc<PO.POLineAP.voucheredQty>))]
		[PXUnboundFormula(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.qty>, Minus<APTran.qty>>), typeof(SumCalc<PO.POLineAP.receivedQty>))]		
		[PXUIField(DisplayName = "Quantity", Visibility = PXUIVisibility.Visible)]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region CuryUnitCost
		public abstract class curyUnitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryUnitCost;

        /// <summary>
        /// The unit cost of the item or service received from the vendor and associated with the line.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXDBCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(APTran.curyInfoID), typeof(APTran.unitCost))]
		[PXUIField(DisplayName = "Unit Cost", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryUnitCost
		{
			get
			{
				return this._CuryUnitCost;
			}
			set
			{
				this._CuryUnitCost = value;
			}
		}
		#endregion
		#region UnitCost
		public abstract class unitCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _UnitCost;

        /// <summary>
        /// /// <summary>
        /// The unit cost of the item or service received from the vendor and associated with the line.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
        /// </summary>
		[PXDBPriceCost()]
		[PXDefault(typeof(Search<INItemCost.lastCost, Where<INItemCost.inventoryID, Equal<Current<APTran.inventoryID>>>>))]
		public virtual Decimal? UnitCost
		{
			get
			{
				return this._UnitCost;
			}
			set
			{
				this._UnitCost = value;
			}
		}
		#endregion
        #region CuryLineAmt
        public abstract class curyLineAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryLineAmt;

        /// <summary>
        /// The extended cost of the item or service associated with the line, which is the unit price multiplied by the quantity.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
        [PXDBCurrency(typeof(APTran.curyInfoID), typeof(APTran.lineAmt))]
        [PXUIField(DisplayName = "Ext. Cost")]
        [PXFormula(typeof(Mult<APTran.qty, APTran.curyUnitCost>))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryLineAmt
        {
            get
            {
                return this._CuryLineAmt;
            }
            set
            {
                this._CuryLineAmt = value;
            }
        }
        #endregion
        #region LineAmt
        public abstract class lineAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _LineAmt;

        /// <summary>
        /// The extended cost of the item or service associated with the line, which is the unit price multiplied by the quantity.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
        [PXDBDecimal(4)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? LineAmt
        {
            get
            {
                return this._LineAmt;
            }
            set
            {
                this._LineAmt = value;
            }
        }
        #endregion
		#region CuryDiscAmt
		public abstract class curyDiscAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryDiscAmt;

		/// <summary>
		/// The amount of the line-level discount that has been applied manually or automatically.
		/// This field is relevant only if the <see cref="PX.Objects.CS.FeaturesSet.VendorDiscounts">Vendor Discounts</see> feature is enabled.
		/// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
		/// </summary>
		[PXDBCurrency(typeof(APTran.curyInfoID), typeof(APTran.discAmt))]
		[PXUIField(DisplayName = "Discount Amount")]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryDiscAmt
		{
			get
			{
				return this._CuryDiscAmt;
			}
			set
			{
				this._CuryDiscAmt = value;
			}
		}
		#endregion
		#region DiscAmt
		public abstract class discAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _DiscAmt;

		/// <summary>
		/// The amount of the line-level discount that has been applied manually or automatically.
		/// This field is relevant only if the <see cref="PX.Objects.CS.FeaturesSet.VendorDiscounts">Vendor Discounts</see> feature is enabled.
		/// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
		/// </summary>
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? DiscAmt
		{
			get
			{
				return this._DiscAmt;
			}
			set
			{
				this._DiscAmt = value;
			}
		}
		#endregion
        #region CuryDiscCost
        public abstract class curyDiscCost : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryDiscCost;

        /// <summary>
        /// The unit cost of the item or service associated with the line after discount.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
        [PXDBPriceCostCalced(typeof(Switch<Case<Where<APTran.qty, Equal<decimal0>>, decimal0>, Div<Sub<APTran.curyTranAmt, APTran.curyDiscAmt>, APTran.qty>>), typeof(Decimal))]
        [PXFormula(typeof(Div<Sub<APTran.curyTranAmt, APTran.curyDiscAmt>, NullIf<APTran.qty, decimal0>>))]
        [PXCurrency(typeof(Search<CommonSetup.decPlPrcCst>), typeof(APTran.curyInfoID), typeof(APTran.discCost))]
        [PXUIField(DisplayName = "Disc. Unit Cost", Enabled = false, Visible = false)]
        public virtual Decimal? CuryDiscCost
        {
            get
            {
                return this._CuryDiscCost;
            }
            set
            {
                this._CuryDiscCost = value;
            }
        }
        #endregion
        #region DiscCost
        public abstract class discCost : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscCost;

        /// <summary>
        /// The unit cost of the item or service associated with the line after discount.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
        [PXDBPriceCostCalced(typeof(Switch<Case<Where<APTran.qty, Equal<decimal0>>, decimal0>, Div<APTran.lineAmt, APTran.qty>>), typeof(Decimal))]
        [PXFormula(typeof(Div<Row<APTran.lineAmt, APTran.curyLineAmt>, NullIf<APTran.qty, decimal0>>))]
        public virtual Decimal? DiscCost
        {
            get
            {
                return this._DiscCost;
            }
            set
            {
                this._DiscCost = value;
            }
        }
        #endregion
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;

        /// <summary>
        /// The total amount for the specified quantity of items or services of this type (after discount has been taken),
        /// or the amount of debit adjustment or prepayment.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXDBCurrency(typeof(APTran.curyInfoID), typeof(APTran.tranAmt))]
        [PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXFormula(typeof(Sub<APTran.curyLineAmt, APTran.curyDiscAmt>))]
        [PXUnboundFormula(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.curyTranAmt>, Minus<APTran.curyTranAmt>>), typeof(SumCalc<PO.POReceiptLineR.signedCuryVoucheredCost>))]
        [PXUnboundFormula(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.curyTranAmt>, Minus<APTran.curyTranAmt>>), typeof(SumCalc<PO.POLineAP.curyVoucheredCost>))]
        [PXUnboundFormula(typeof(Switch<Case<Where<APTran.drCr, Equal<DrCr.debit>>, APTran.curyTranAmt>, Minus<APTran.curyTranAmt>>), typeof(SumCalc<PO.POLineAP.curyReceivedCost>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
			}
		}
		#endregion
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;

        /// <summary>
        /// The total amount for the specified quantity of items or services of this type (after discount has been taken),
        /// or the amount of debit adjustment or prepayment.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Amount", Enabled = false)]
		public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
		#endregion
		#region CuryTaxableAmt
		public abstract class curyTaxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTaxableAmt;
        
        /// <summary>
        /// The line amount that is subject to tax.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXDBCurrency(typeof(APTran.curyInfoID), typeof(APTran.taxableAmt))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Net Amount", Enabled = false)]
        public virtual Decimal? CuryTaxableAmt
		{
			get
			{
				return this._CuryTaxableAmt;
			}
			set
			{
				this._CuryTaxableAmt = value;
			}
		}
		#endregion
		#region TaxableAmt
		public abstract class taxableAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TaxableAmt;

        /// <summary>
        /// The line amount that is subject to tax.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? TaxableAmt
		{
			get
			{
				return this._TaxableAmt;
			}
			set
			{
				this._TaxableAmt = value;
			}
		}
		#endregion
        #region CuryTaxAmt
        public abstract class curyTaxAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _CuryTaxAmt;

        /// <summary>
        /// The amount of tax (VAT) associated with the line.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
        [PXDBCurrency(typeof(APTran.curyInfoID), typeof(APTran.taxAmt))]
        [PXUIField(DisplayName = "VAT", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? CuryTaxAmt
        {
            get
            {
                return this._CuryTaxAmt;
            }
            set
            {
                this._CuryTaxAmt = value;
            }
        }
        #endregion
        #region TaxAmt
        public abstract class taxAmt : PX.Data.IBqlField
        {
        }
        protected Decimal? _TaxAmt;

        /// <summary>
        /// The amount of tax (VAT) associated with the line.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
        [PXDBBaseCury()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? TaxAmt
        {
            get
            {
                return this._TaxAmt;
            }
            set
            {
                this._TaxAmt = value;
            }
        }
		#endregion
		#region ExpenseAmt
		public abstract class expenseAmt : IBqlField
		{
		}
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]		
		public virtual Decimal? ExpenseAmt
		{
			get; set;
		}
		#endregion
		#region CuryExpenseAmt
		public abstract class curyExpenseAmt : IBqlField
		{
		}
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0", PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Decimal? CuryExpenseAmt
		{
			get; set;
		}
		#endregion
		#region ManualPrice
		public abstract class manualPrice : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualPrice;
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Price", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? ManualPrice
        {
            get
            {
                return this._ManualPrice;
            }
            set
            {
                this._ManualPrice = value;
            }
        }
        #endregion
        #region ManualDisc
        public abstract class manualDisc : PX.Data.IBqlField
        {
        }
        protected Boolean? _ManualDisc;

        /// <summary>
        /// When set to <c>true</c> indicates that the discount is applied to the line manually. 
        /// In this case user may enter either the <see cref="DiscPct">discount percent</see>, or the
        /// <see cref="DiscAmt">discount amount</see> or a predefined <see cref="DiscountID">discount code</see>.
        /// This field is relevant only if the <see cref="PX.Objects.CS.FeaturesSet.VendorDiscounts">Vendor Discounts</see> feature is enabled.
        /// </summary>
        [AR.ManualDiscountMode(typeof(APTran.curyDiscAmt), typeof(APTran.curyTranAmt), typeof(APTran.discPct), typeof(APTran.freezeManualDisc))]
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Manual Discount", Visibility = PXUIVisibility.Visible)]
        public virtual Boolean? ManualDisc
        {
            get
            {
                return this._ManualDisc;
            }
            set
            {
                this._ManualDisc = value;
            }
        }
        #endregion
        #region FreezeManualDisc
        public abstract class freezeManualDisc : PX.Data.IBqlField
        {
        }
        protected Boolean? _FreezeManualDisc;
        [PXBool()]
        public virtual Boolean? FreezeManualDisc
        {
            get
            {
                return this._FreezeManualDisc;
            }
            set
            {
                this._FreezeManualDisc = value;
            }
        }
        #endregion
        #region GroupDiscountRate
        public abstract class groupDiscountRate : PX.Data.IBqlField
        {
        }
        protected Decimal? _GroupDiscountRate;

        /// <summary>
        /// The effective rate of the group-level discount associated with the line.
        /// </summary>
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "1.0")]
        public virtual Decimal? GroupDiscountRate
        {
            get
            {
                return this._GroupDiscountRate;
            }
            set
            {
                this._GroupDiscountRate = value;
            }
        }
        #endregion
        #region DocumentDiscountRate
        public abstract class documentDiscountRate : PX.Data.IBqlField
        {
        }
        protected Decimal? _DocumentDiscountRate;

        /// <summary>
        /// The effective rate of the document-level discount associated with the line.
        /// </summary>
        [PXDBDecimal(6)]
        [PXDefault(TypeCode.Decimal, "1.0")]
        public virtual Decimal? DocumentDiscountRate
        {
            get
            {
                return this._DocumentDiscountRate;
            }
            set
            {
                this._DocumentDiscountRate = value;
            }
        }
        #endregion
		#region TranClass
		public abstract class tranClass : PX.Data.IBqlField
		{
		}
		protected String _TranClass;
		[PXDBString(1, IsFixed = true)]
		[PXDefault("")]
		public virtual String TranClass
		{
			get
			{
				return this._TranClass;
			}
			set
			{
				this._TranClass = value;
			}
		}
		#endregion
		#region DrCr
		public abstract class drCr : PX.Data.IBqlField
		{
		}
		protected String _DrCr;

        /// <summary>
        /// Indicates whether the line is of debit or credit type.
        /// </summary>
        /// <value>
        /// Is set to the parent's <see cref="APInvoice.DrCr"/> by default.
        /// </value>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(typeof(APInvoice.drCr))]
		public virtual String DrCr
		{
			get
			{
				return this._DrCr;
			}
			set
			{
				this._DrCr = value;
			}
		}
		#endregion
		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;

        /// <summary>
        /// The date of the transaction.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="APRegister.DocDate">date of the parent document</see>.
        /// </value>
		[PXDBDate()]
		[PXDBDefault(typeof(APRegister.docDate))]
		[PXUIField(DisplayName = "Date")]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;

        /// <summary>
        /// <see cref="PX.Objects.GL.FinPeriod">Financial period</see>, which the line is associated with.
        /// </summary>
        /// <value>
        /// Defaults to the <see cref="APRegister.FinPeriodID">document's financial period</see>.
        /// </value>
		[GL.FinPeriodID()]
		[PXDBDefault(typeof(APRegister.finPeriodID))]
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
		#region TranDesc
		public abstract class tranDesc : PX.Data.IBqlField
		{
		}
		protected String _TranDesc;

        /// <summary>
        /// The description text for the transaction.
        /// </summary>
		[PXDBString(256, IsUnicode = true)]
		[PXUIField(DisplayName="Transaction Descr.", Visibility=PXUIVisibility.Visible)]
		public virtual String TranDesc
		{
			get
			{
				return this._TranDesc;
			}
			set
			{
				this._TranDesc = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;

        /// <summary>
        /// Indicates whether the line is released or not.
        /// </summary>
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released")]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				this._Released = value;
			}
		}
		#endregion

		#region POPPVAmt
		public abstract class pOPPVAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _POPPVAmt;

        /// <summary>
        /// Purchase price variance amount associated with the line.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
        /// <seealso cref="PX.Objects.PO.POReceiptLineR1.BillPPVAmt"/>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "PO PPV Amount")]
		public virtual Decimal? POPPVAmt
		{
			get
			{
				return this._POPPVAmt;
			}
			set
			{
				this._POPPVAmt = value;
			}
		}
		#endregion
		#region PPVDocType
		public abstract class pPVDocType : PX.Data.IBqlField
		{
		}
		protected String _PPVDocType;
		[PXDBString(3, IsFixed = true)]
		[PXUIField(DisplayName = "PPV Doc. Type", Enabled = false, IsReadOnly = true, Visible = false)]
		[IN.INDocType.List()]
		public virtual String PPVDocType
		{
			get
			{
				return this._PPVDocType;
			}
			set
			{
				this._PPVDocType = value;
			}
		}
		#endregion
		#region PPVRefNbr
		public abstract class pPVRefNbr : PX.Data.IBqlField
		{
		}
		protected String _PPVRefNbr;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "PPV Ref. Nbr.", Enabled = false, IsReadOnly = true)]
		[PXSelector(typeof(Search<IN.INRegister.refNbr, Where<IN.INRegister.docType, Equal<Optional<APTran.pPVDocType>>>, OrderBy<Desc<IN.INRegister.refNbr>>>), Filterable = true)]
		public virtual String PPVRefNbr
		{
			get
			{
				return this._PPVRefNbr;
			}
			set
			{
				this._PPVRefNbr = value;
			}
		}
		#endregion

        #region NonBillable
        public abstract class nonBillable : PX.Data.IBqlField
        {
        }
        protected Boolean? _NonBillable;

        /// <summary>
        /// When set to <c>true</c> indicates that the document line is not billable in the project.
        /// The field is relevant only in case <see cref="PX.Objects.CS.FeaturesSet.ProjectModule">Project module</see> is enabled.
        /// </summary>
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Non Billable", FieldClass = ProjectAttribute.DimensionName)]
        public virtual Boolean? NonBillable
        {
            get
            {
                return this._NonBillable;
            }
            set
            {
                this._NonBillable = value;
            }
        }
        #endregion
		#region DefScheduleID
		public abstract class defScheduleID : IBqlField
		{
		}
		protected int? _DefScheduleID;

        /// <summary>
        /// A read-only field that shows the identifier of the <see cref="PX.Objects.DR.DRSchedule">schedule</see>
        /// automatically assigned to the bill based on the deferral code.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.DR.DRSchedule.ScheduleID">DRSchedule.ScheduleID</see> field.
        /// </value>
		[PXDBInt]
		[PXUIField(DisplayName = "Original Deferral Schedule")]
		[PXSelector(typeof(Search<DR.DRSchedule.scheduleID, 
			Where<DR.DRSchedule.bAccountID, Equal<Current<APInvoice.vendorID>>,
			And <DR.DRSchedule.docType, Equal<APInvoiceType.invoice>,
			Or<DR.DRSchedule.docType, Equal<APInvoiceType.creditAdj>>>>>))]
		public virtual int? DefScheduleID
		{
			get
			{
				return this._DefScheduleID;
			}
			set
			{
				this._DefScheduleID = value;
			}
		}
		#endregion
		#region Date
		public abstract class date : PX.Data.IBqlField
		{
		}
		protected DateTime? _Date;
		/// <summary>
        /// Expense date. When an <see cref="PX.Objects.EP.EPExpenseClaim">expense claim</see> is released this field is set to
        /// the <see cref="PX.Objects.EP.EPExpenseClaimDetails.ExpenseDate">expense date</see> in the resulting AP transactions. 
		/// </summary>
		[PXDBDate()]
		[PXUIField(DisplayName="Date", Visible=false)]
		public virtual DateTime? Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				this._Date = value;
			}
		}
		#endregion

        #region LandedCostCodeID
        public abstract class landedCostCodeID : PX.Data.IBqlField
        {
        }
        protected String _LandedCostCodeID;

        /// <summary>
        /// The <see cref="PX.Objects.PO.LandedCostCode">landed cost code</see> used to describe the specific landed costs incurred for the line.
        /// This code is one of the codes associated with the vendor.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Objects.PO.LandedCostCode.LandedCostCodeID">LandedCostCode.LandedCostCodeID</see> field.
        /// </value>
        [PXDBString(15, IsUnicode = true, IsFixed = false)]
        [PXUIField(DisplayName = "Landed Cost Code")]
        [PXSelector(typeof(Search<PO.LandedCostCode.landedCostCodeID>))]
        public virtual String LandedCostCodeID
        {
            get
            {
                return this._LandedCostCodeID;
            }
            set
            {
                this._LandedCostCodeID = value;
            }
        }
        #endregion

        #region DiscPct
        public abstract class discPct : PX.Data.IBqlField
        {
        }
        protected Decimal? _DiscPct;

        /// <summary>
        /// The percent of the line-level discount, that has been applied manually or automatically.
        /// This field is relevant only if the <see cref="PX.Objects.CS.FeaturesSet.VendorDiscounts">Vendor Discounts</see> feature is enabled.
        /// </summary>
        [PXDBDecimal(6, MinValue = -100, MaxValue = 100)]
        [PXUIField(DisplayName = "Discount Percent")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual Decimal? DiscPct
        {
            get
            {
                return this._DiscPct;
            }
            set
            {
                this._DiscPct = value;
            }
        }
        #endregion
        #region DiscountID
        public abstract class discountID : PX.Data.IBqlField
        {
        }
        protected String _DiscountID;

        /// <summary>
        /// The code of the <see cref="APDiscount">discount</see> that has been applied to this line.
        /// This field is relevant only if the <see cref="PX.Objects.CS.FeaturesSet.VendorDiscounts">Vendor Discounts</see> feature is enabled.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="APDiscount.DiscountID"/> field.
        /// </value>
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search<APDiscount.discountID, Where<APDiscount.bAccountID, Equal<Current<APTran.vendorID>>, And<APDiscount.type, Equal<AR.DiscountType.LineDiscount>>>>))]
        [PXUIField(DisplayName = "Discount Code", Visible = true, Enabled = true)]
        public virtual String DiscountID
        {
            get
            {
                return this._DiscountID;
            }
            set
            {
                this._DiscountID = value;
            }
        }
        #endregion
        #region DiscountSequenceID
        public abstract class discountSequenceID : PX.Data.IBqlField
        {
        }
        protected String _DiscountSequenceID;

        /// <summary>
        /// The identifier of the discount sequence applied to the line.
        /// </summary>
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Discount Sequence", Visible = false, Enabled = false)]
        public virtual String DiscountSequenceID
        {
            get
            {
                return this._DiscountSequenceID;
            }
            set
            {
                this._DiscountSequenceID = value;
            }
        }
        #endregion
		#region TaxCategoryID
		public abstract class taxCategoryID : PX.Data.IBqlField
		{
		}
		protected String _TaxCategoryID;

        /// <summary>
        /// Identifier of the <see cref="TaxCategory">tax category</see> associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="TaxCategory.TaxCategoryID"/> field.
        /// Defaults to the <see cref="InventoryItem.TaxCategoryID">tax category associated with the line item</see>.
        /// </value>
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Tax Category", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(TaxCategory.taxCategoryID), DescriptionField = typeof(TaxCategory.descr))]
		[PXRestrictor(typeof(Where<TaxCategory.active, Equal<True>>), TX.Messages.InactiveTaxCategory, typeof(TaxCategory.taxCategoryID))]
		[PXDefault(typeof(Search<InventoryItem.taxCategoryID,
			Where<InventoryItem.inventoryID, Equal<Current<APTran.inventoryID>>>>),
			PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String TaxCategoryID
		{
			get
			{
				return this._TaxCategoryID;
			}
			set
			{
				this._TaxCategoryID = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;

        /// <summary>
        /// Identifier of the <see cref="PX.Data.Note">Note</see> object, associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="PX.Data.Note.NoteID">Note.NoteID</see> field. 
        /// </value>
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion

		#region ClassID
		public abstract class classID : IBqlField
		{
		}
		protected Int32? _ClassID;

        /// <summary>
        /// The class of asset associated with the line.
        /// </summary>
        /// <value>
        /// Corresponds to the <see cref="FixedAsset.AssetID"/> field.
        /// </value>
		[PXInt]
		[PXSelector(typeof(Search2<FixedAsset.assetID,
			LeftJoin<FABookSettings, On<FixedAsset.assetID, Equal<FABookSettings.assetID>>,
			LeftJoin<FABook, On<FABookSettings.bookID, Equal<FABook.bookID>>>>,
			Where<FixedAsset.recordType, Equal<FARecordType.classType>,
			And<FABook.updateGL, Equal<True>>>>),
					SubstituteKey = typeof(FixedAsset.assetCD),
					DescriptionField = typeof(FixedAsset.description))]
		[PXUIField(DisplayName = "Asset Class", Visibility = PXUIVisibility.Visible)]
		public virtual Int32? ClassID
		{
			get
			{
				return _ClassID;
			}
			set
			{
				_ClassID = value;
			}
		}
		#endregion
		#region Custodian
		public abstract class custodian : IBqlField
		{
		}
		protected Guid? _Custodian;

        /// <summary>
        /// The <see cref="EPEmployee">employee</see> responsible for the line.
        /// </summary>
		[PXGuid]
		[PXSelector(typeof(EPEmployee.userID), SubstituteKey = typeof(EPEmployee.acctCD), DescriptionField = typeof(EPEmployee.acctName))]
		[PXUIField(DisplayName = "Custodian")]
		public virtual Guid? Custodian
		{
			get
			{
				return _Custodian;
			}
			set
			{
				_Custodian = value;
			}
		}
		#endregion
		#region EmployeeID
        public abstract class employeeID : PX.Data.IBqlField
        {
        }
        protected Int32? _EmployeeID;

		/// <summary>
		/// Identifier of the <see cref="EP.EPEmployee">Employee</see> who created the document line.
		/// </summary>
		/// <value>
		/// Corresponds to the <see cref="EP.EPEmployee.BAccountID">EPEmployee.BAccountID</see> field.
		/// This field is not visible to user and cannot be changed by them.
		/// </value>
		[PXDBInt]
		[PXDefault(typeof(Search<EP.EPEmployee.bAccountID, Where<EP.EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual Int32? EmployeeID
		{
			get
			{
				return this._EmployeeID;
			}
			set
			{
				this._EmployeeID = value;
			}
		}
        #endregion

        /// <summary>
        /// Read-only field showing the sign of the line. Calculated based on the <see cref="DrCr"/> field.
        /// </summary>
        /// <value>
        /// <c>1</c> for debit lines and <c>-1</c> for credit ones.
        /// </value>
		protected decimal Sign
		{
			get { return this.DrCr == GL.DrCr.Debit ? Decimal.One : Decimal.MinusOne; }
		}

		#region SignedQty
		public abstract class signedQty : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only field showing the line quantity multiplied by the line sign.
        /// Based on the <see cref="Qty"/> and <see cref="Sign"/> fields.
        /// </summary>
		[PXQuantity()]
		public virtual Decimal? SignedQty
		{
			[PXDependsOnFields(typeof(qty))]
			get
			{
				return this._Qty * this.Sign;
			}
		}
		#endregion
		#region SignedCuryTranAmt
		public abstract class signedCuryTranAmt : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only field showing the signed line amount.
        /// Based on the <see cref="CuryTranAmt"/> and <see cref="Sign"/> fields.
        /// (Presented in the currency of the document, see <see cref="APRegister.CuryID"/>)
        /// </summary>
		[PXDecimal()]
		public virtual Decimal? SignedCuryTranAmt
		{
			[PXDependsOnFields(typeof(curyTranAmt))]
			get
			{
				return (this._CuryTranAmt * this.Sign);
			}
		}
		#endregion
		#region SignedTranAmt
		public abstract class signedTranAmt : PX.Data.IBqlField
		{
		}

        /// <summary>
        /// Read-only field showing the signed line amount.
        /// Based on the <see cref="CuryTranAmt"/> and <see cref="Sign"/> fields.
        /// (Presented in the base currency of the company, see <see cref="Company.BaseCuryID"/>)
        /// </summary>
		[PXBaseCury()]
		public virtual Decimal? SignedTranAmt
		{
			[PXDependsOnFields(typeof(tranAmt))]
			get
			{
				return (this._TranAmt* this.Sign);
			}
			
		}
		#endregion

		#region DR Interface Implementation

	    string DR.Descriptor.IDocumentLine.Module => BatchModule.AP;

	    #endregion
	}

    /// <summary>
    /// Provides a selector for the Inventory Items, which may be put into the AP Invoice line <br/>
    /// The list is filtered by the user access rights and Inventory Item status - inactive <br/>
    /// and marked to delete items are not shown. If the Purchase order or PO Receipt is specified <br/>
    /// - all the items are shown, restriction is made in other place. <br/>
    /// May be used only on DAC derived from APTran. <br/>
    /// <example>
    /// [APTranInventoryItem(Filterable = true)]
    /// </example>
    /// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Inventory ID", Visibility = PXUIVisibility.Visible)]
	[PXRestrictor(typeof(Where<InventoryItem.stkItem, Equal<False>, Or<Current<APTran.pONbr>, IsNotNull, Or<Current<APTran.receiptNbr>, IsNotNull, Or<Current<APTran.tranType>, Equal<APInvoiceType.invoice>>>>>), Messages.CannotStockItemInAPBillDirectly)]
    [PXRestrictor(typeof(Where<Current<APTran.pONbr>, IsNotNull, Or<InventoryItem.stkItem, NotEqual<False>, Or<InventoryItem.kitItem, NotEqual<True>>>>), Messages.CannotAddNonStockKitInAPBillDirectly)]
	public class APTranInventoryItemAttribute : InventoryAttribute
	{
		public APTranInventoryItemAttribute()
			: base(typeof(Search<InventoryItem.inventoryID, Where<Match<Current<AccessInfo.userName>>>>), typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr))
		{
		}
	}
}
