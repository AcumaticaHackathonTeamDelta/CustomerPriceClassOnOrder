namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Data.EP;
	using PX.Objects.CS;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLVoucher)]
	public partial class GLVoucher: PX.Data.IBqlTable
	{
		#region WorkBookID
		public abstract class workBookID : PX.Data.IBqlField
		{
		}
		protected String _WorkBookID;

		/// <summary>
		/// !REV!
		/// The unique voucher code for the selected <see cref="Module"/> and <see cref="TranType">Type</see> of the document.
		/// The code is selected by user on the Journal Vouchers (GL.30.40.00) screen (<see cref="GLWorkBookID.WorkBookID"/> field)
		/// when entering the lines of the batch and determines the module and type of the document or transaction
		/// to be created from the corresponding line of the document batch.
		/// Identifies the record of this DAC associated with a <see cref="GLWorkBookID">line</see> of a <see cref="GLDocBatch">document batch</see>.
		/// Only one code can be created for any combination of <see cref="Module"/> and <see cref="TranType">Document/Transaction Type</see>.
		/// </summary>
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa", IsKey = true)]
		[PXDBDefault(typeof(GLVoucherBatch))]		
		[PXUIField(DisplayName = "Workbook ID", Visibility = PXUIVisibility.Visible, Visible=false)]
		public virtual String WorkBookID
		{
			get
			{
				return this._WorkBookID;
			}
			set
			{
				this._WorkBookID = value;
			}
		}
		#endregion
		#region VoucherBatchNbr
		public abstract class voucherBatchNbr : PX.Data.IBqlField
		{
		}
		protected String _VoucherBatchNbr;

		/// <summary>
		/// !REV!
		/// The unique voucher code for the selected <see cref="Module"/> and <see cref="TranType">Type</see> of the document.
		/// The code is selected by user on the Journal Vouchers (GL.30.40.00) screen (<see cref="GLVoucherBatchNbr.VoucherBatchNbr"/> field)
		/// when entering the lines of the batch and determines the module and type of the document or transaction
		/// to be created from the corresponding line of the document batch.
		/// Identifies the record of this DAC associated with a <see cref="GLVoucherBatchNbr">line</see> of a <see cref="GLDocBatch">document batch</see>.
		/// Only one code can be created for any combination of <see cref="Module"/> and <see cref="TranType">Document/Transaction Type</see>.
		/// </summary>
		[PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC",IsKey=true)]		
		[PXDBDefault(typeof(GLVoucherBatch))]
		[PXParent(typeof(Select<GLVoucherBatch, Where<GLVoucherBatch.workBookID, Equal<Current<GLVoucher.workBookID>>,
								And<GLVoucherBatch.voucherBatchNbr, Equal<Current<GLVoucher.voucherBatchNbr>>>>>))]       
		
		[PXUIField(DisplayName = Messages.VoucherBatchNbr, Visibility = PXUIVisibility.SelectorVisible, Visible=false)]
		public virtual String VoucherBatchNbr
		{
			get
			{
				return this._VoucherBatchNbr;
			}
			set
			{
				this._VoucherBatchNbr = value;
			}
		}
		#endregion

		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineNbr;

		/// <summary>
		/// Key field.
		/// The number of the document/transaction line inside the <see cref="GLDocBatch">document batch</see>.
		/// </summary>
		/// <value>
		/// Note that the sequence of line numbers of the transactions belonging to a single document may include gaps.
		/// </value>
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, Enabled = false)]
		[PXLineNbr(typeof(GLVoucherBatch.lineCntr))]
		//[PXLineNbr(typeof(Search<GLVoucherBatch.lineCntr,Where<GLVoucherBatch.workBookID, Equal<Current<GLVoucher.workBookID>>,
		//						And<GLVoucherBatch.voucherBatchNbr,Equal<Current<GLVoucher.voucherBatchNbr>>>>>))]
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
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;

		/// <summary>
		/// The code of the module where a document or transaction will be generated according to this entry code.
		/// </summary>
		/// <value>
		/// Allowed values are: <c>"GL"</c>, <c>"AP"</c>, <c>"AR"</c> and <c>"CA"</c>.
		/// </value>
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault(typeof(Search<GLWorkBook.module, Where<GLWorkBook.workBookID, Equal<Current<GLVoucherBatch.workBookID>>>>))]
		[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.SelectorVisible)]
		//[VoucherModule.List()]
		//[PXFieldDescription]
		public virtual String Module
		{
			get
			{
				return this._Module;
			}
			set
			{
				this._Module = value;
			}
		}
		#endregion
		#region DocType
		public abstract class docType : PX.Data.IBqlField
		{
		}
		protected String _DocType;

		/// <summary>
		/// The type of the document or transaction generated according to the code.
		/// </summary>
		/// <value>
		/// Allowed values set depends on the selected <see cref="Module"/>.
		/// </value>
		[PXDBString(3, IsKey = true, IsFixed = true)]
		[PXDefault(typeof(Search<GLWorkBook.docType, Where<GLWorkBook.workBookID, Equal<Current<GLVoucherBatch.workBookID>>>>))]
		[PXUIField(DisplayName = "Module Tran. Type", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
		}
		protected String _RefNbr;

		/// <summary>
		/// Reference number of the document or transaction created from the line.
		/// </summary>
		/// <value>
		/// For the lines defining GL transactions the field corresponds to the <see cref="Batch.BatchNbr"/> field.
		/// For the lines used to create documents in AP and AR modules the field corresponds to the 
		/// <see cref="PX.Objects.AP.APRegister.RefNbr"/> and <see cref="PX.Objects.AR.ARRegister.RefNbr"/> fields, respectively.
		/// For the lines defining CA transactions the field corresponds to the <see cref="CAAdj.AdjRefNbr"/> field.
		/// </value>
		/// [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXFormula(null, typeof(CountCalc<GLVoucherBatch.docCount>))]		
		[PXDBString(15, IsUnicode = true)]		
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Ref. Number", Visible = true)]
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
		#region RefNoteID
		public abstract class refNoteID : IBqlField { }
		[PXDBGuid()]
		public virtual Guid? RefNoteID { get; set; }
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
	}
}
