using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Data.EP;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.IN;

namespace PX.Objects.PM
{
	[PXPrimaryGraph(typeof(RegisterEntry))]
	[Serializable]
	public partial class PMRegister : IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
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
		
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXDBString(2, IsKey = true, IsFixed = true)]
		[PXDefault(BatchModule.PM)]
		[PXUIField(DisplayName = "Module", Visibility = PXUIVisibility.SelectorVisible)]
		[BatchModule.PMListAttribute()]
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
		#region RefNbr
		public abstract class refNbr : PX.Data.IBqlField
		{
			public const int Length = 15;
		}
		protected String _RefNbr;
		[PXDBString(PMRegister.refNbr.Length, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXSelector(typeof(Search<PMRegister.refNbr, Where<PMRegister.module, Equal<Current<PMRegister.module>>>, OrderBy<Desc<PMRegister.refNbr>>>), Filterable = true)]
		[PXUIField(DisplayName = "Ref. Number", Visibility = PXUIVisibility.SelectorVisible)]
		[AutoNumber(typeof(Search<PMSetup.tranNumbering>), typeof(AccessInfo.businessDate))]
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
		#region Date
		public abstract class date : PX.Data.IBqlField
		{
		}
		protected DateTime? _Date;
		[PXDBDate]
		[PXDefault(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Transaction Date", Visibility = PXUIVisibility.Visible)]
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        [PXFieldDescription]
        public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
        #region Status
        public abstract class status : IBqlField
        {
            #region List
            public class ListAttribute : PXStringListAttribute
            {
                public ListAttribute()
                    : base(
                new string[] { Hold, Balanced, Released },
                new string[] { Messages.Hold, Messages.Balanced, Messages.Released }) { }
            }

            public const string Hold = "H";
            public const string Balanced = "B";
            public const string Released = "R";

            public class hold : Constant<string>
            {
                public hold() : base(Hold) { ;}
            }

            public class balanced : Constant<string>
            {
                public balanced() : base(Balanced) { ;}
            }

            public class released : Constant<string>
            {
                public released() : base(Released) { ;}
            }
            #endregion
        }
        protected String _Status;
        [PXDBString(1, IsFixed = true)]
        [PXDefault(status.Hold)]
        [status.List()]
        [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        public virtual String Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }
        #endregion
        #region Released
		public abstract class released : PX.Data.IBqlField
		{
		}
		protected Boolean? _Released;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName="Released", Enabled=false)]
		public virtual Boolean? Released
		{
			get
			{
				return this._Released;
			}
			set
			{
				_Released = value;
                SetStatus();
            }
		}
		#endregion
        #region Hold
        public abstract class hold : PX.Data.IBqlField
        {
        }
        protected Boolean? _Hold;
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "On Hold")]
        public virtual Boolean? Hold
        {
            get
            {
                return _Hold;
            }
            set
            {
                _Hold = value;
                SetStatus();
            }
        }
        #endregion
		#region IsAllocation
		public abstract class isAllocation : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsAllocation;
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? IsAllocation
		{
			get
			{
				return this._IsAllocation;
			}
			set
			{
				_IsAllocation = value;
			}
		}
		#endregion
        #region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(PMRegister.refNbr.Length, IsUnicode = true)]
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion
		#region OrigDocType
		public abstract class origDocType : PX.Data.IBqlField
		{
		}
		protected String _OrigDocType;
		[PXDBString(2, IsFixed = true)]
		[PMOrigDocType.List()]
		[PXUIField(DisplayName = "Orig. Doc. Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String OrigDocType
		{
			get
			{
				return this._OrigDocType;
			}
			set
			{
				this._OrigDocType = value;
			}
		}
		#endregion
		#region OrigDocNbr
		public abstract class origDocNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigDocNbr;
		[PXDBString()]
		[PXUIField(DisplayName = "Orig. Doc. Nbr.", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String OrigDocNbr
		{
			get
			{
				return this._OrigDocNbr;
			}
			set
			{
				this._OrigDocNbr = value;
			}
		}
		#endregion

		#region QtyTotal
		public abstract class qtyTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyTotal;
		[PXQuantity]
		[PXUIField(DisplayName = "Total Qty.", Enabled = false)]
		public virtual Decimal? QtyTotal
		{
			get
			{
				return this._QtyTotal;
			}
			set
			{
				this._QtyTotal = value;
			}
		}
		#endregion
		#region BillableQtyTotal
		public abstract class billableQtyTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _BillableQtyTotal;
		[PXQuantity]
		[PXUIField(DisplayName = "Total Billable Qty.", Enabled = false)]
		public virtual Decimal? BillableQtyTotal
		{
			get
			{
				return this._BillableQtyTotal;
			}
			set
			{
				this._BillableQtyTotal = value;
			}
		}
		#endregion
		#region AmtTotal
		public abstract class amtTotal : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmtTotal;
		[PXQuantity]
		[PXUIField(DisplayName = "Total Amount", Enabled = false)]
		public virtual Decimal? AmtTotal
		{
			get
			{
				return this._AmtTotal;
			}
			set
			{
				this._AmtTotal = value;
			}
		}
		#endregion
		
		#region System Columns
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
        [PXNote]
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
		#endregion
        #region Methods
        private void SetStatus()
        {
            if (_Hold ?? false)
            {
                _Status = status.Hold;
            }
            else if (! (_Released ?? false))
            {
                _Status = status.Balanced;
            }
            else if (_Released == true)
            {
                _Status = status.Released;
            }
        }
        #endregion

	}

	public static class PMOrigDocType
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
                new string[] { Allocation, Timecard, Case, ExpenseClaim, EquipmentTimecard, AllocationReversal, Reversal, CreditMemo, UnbilledRemainder },
				new string[] { Messages.Allocation, Messages.Timecard, Messages.Case, Messages.ExpenseClaim, Messages.EquipmentTimecard, Messages.AllocationReversal, Messages.Reversal, Messages.CreditMemo, Messages.UnbilledRemainder }) { ; }
		}
		public const string Allocation = "AL";
		public const string Timecard = "TC";
		public const string Case = "CS";
		public const string ExpenseClaim = "EC";
		public const string EquipmentTimecard = "ET";
		public const string AllocationReversal = "AR";
		public const string Reversal = "RV";
		public const string CreditMemo = "CR";
		public const string UnbilledRemainder = "UR";

		public class timeCard:Constant<String>
		{
			public timeCard() : base(Timecard)
			{
			}
		}

	}
}
