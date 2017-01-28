namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Data.EP;
	using PX.Objects.CS;
	using PX.Objects.CM;
	using System.Collections;
	using System.Collections.Generic;


	/// <summary>
	/// 
	/// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLVoucherBatch)]
	public partial class GLVoucherBatch : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;

		/// <summary>
		/// Indicates whether the record is selected for processing.
		/// </summary>
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
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
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa", IsKey= true)]
		[PXDefault()]
		//[PXSelector(typeof(Search<GLWorkBook.workBookID>))]
		[PXUIField(DisplayName = "Workbook ID", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]
		//[PXSelector(typeof(Search<GLVoucherBatch.voucherBatchNbr, Where<GLVoucherBatch.workBookID, Equal<Current<GLVoucherBatch.workBookID>>>, OrderBy<Desc<GLVoucherBatch.voucherBatchNbr>>>), Filterable = true)]
		[AutoNumber(typeof(Search<GLWorkBook.voucherBatchNumberingID,Where<GLWorkBook.workBookID,Equal<Current<GLVoucherBatch.workBookID>>>>), typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = Messages.VoucherBatchNbr, Visibility = PXUIVisibility.SelectorVisible)]
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
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;

		/// <summary>
		/// Description of the entry code.
		/// </summary>
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
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
		[PXDBString(2,IsFixed = true)]
		[PXDefault(typeof(Search<GLWorkBook.module, Where<GLWorkBook.workBookID,Equal<Current<GLVoucherBatch.workBookID>>>>))]
		//[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.SelectorVisible)]
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
		[PXDBString(3,IsFixed = true)]		
		[PXDefault(typeof(Search<GLWorkBook.docType, Where<GLWorkBook.workBookID,Equal<Current<GLVoucherBatch.workBookID>>>>))]
		//[PXUIField(DisplayName = "Module Tran. Type", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region LineCntr
		public abstract class lineCntr : PX.Data.IBqlField
		{
		}
		protected Int32? _LineCntr;

		/// <summary>
		/// The counter of the document lines, used <i>internally</i> to assign consistent numbers to newly created lines.
		/// It is not recommended to rely on this field to determine the exact count of lines, because it might not reflect the latter under some conditions.
		/// </summary>
		[PXDBInt()]
		[PXDefault(0)]
		public virtual Int32? LineCntr
		{
			get
			{
				return this._LineCntr;
			}
			set
			{
				this._LineCntr = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;

		/// <summary>
		/// Indicates whether the batch has been released.
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
		#region NotReleased
		public abstract class notReleased : PX.Data.IBqlField
		{
		}

		[PXBool()]
		[PXUIField(DisplayName = "Not Released", Visible=false, Visibility=PXUIVisibility.Invisible, Enabled=false)]
		public virtual Boolean? NotReleased
		{
			[PXDependsOnFields(typeof(released))]
			get
			{
				return this._Released==false;
			}
		}
		#endregion
		#region DocCount
		public abstract class docCount : PX.Data.IBqlField
		{
		}
		protected Int32? _DocCount;

		/// <summary>
		/// Represents number of documents included into the batch. 	
		/// </summary>
		[PXDBInt()]
		[PXDefault(0)]
		[PXUIField(DisplayName = "Documents Count",Enabled=false)]
		public virtual Int32? DocCount
		{
			get
			{
				return this._DocCount;
			}
			set
			{
				this._DocCount = value;
			}
		}
		#endregion
		#region BatchTotal
		public abstract class batchTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _BatchTotal;

		/// <summary>
		/// Represents the Batch Total Amount in Base Currency		
		/// </summary>
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Batch Total Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Decimal? BatchTotal
		{
			get
			{
				return this._BatchTotal;
			}
			set
			{
				this._BatchTotal = value;
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
		[PXUIField(DisplayName = "Last Modified", Visibility = PXUIVisibility.Visible, Enabled = false)]
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
	}
}


