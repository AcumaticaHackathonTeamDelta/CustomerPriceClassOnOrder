namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	using PX.Objects.TX;
	using PX.Objects.CS;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(INMovementClassMaint))]
	public partial class INMovementClass : PX.Data.IBqlTable
	{
		#region MovementClassID
		public abstract class movementClassID : PX.Data.IBqlField
		{
		}
		protected String _MovementClassID;
		[PXDefault()]
        [PXDBString(1, IsKey = true)]
		[PXUIField(DisplayName = "Movement Class ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String MovementClassID
		{
			get
			{
				return this._MovementClassID;
			}
			set
			{
				this._MovementClassID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
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
		#region CountsPerYear
        public abstract class countsPerYear : PX.Data.IBqlField
        {
        }
        protected Int16? _CountsPerYear;
        [PXDBShort(MinValue = 0)]
        [PXUIField(DisplayName = "Counts Per Year", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Int16? CountsPerYear
        {
            get
            {
                return this._CountsPerYear;
            }
            set
            {
                this._CountsPerYear = value;
            }
        }
        #endregion
        #region MaxTurnoverPct
		public abstract class maxTurnoverPct : PX.Data.IBqlField
        {
        }
		protected Decimal? _MaxTurnoverPct;
        [PXDBDecimal(2, MinValue = 0, MaxValue = 1000000000)]
        [PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Turnover %")]

		public virtual Decimal? MaxTurnoverPct
        {
            get
            {
				return this._MaxTurnoverPct;
            }
            set
            {
				this._MaxTurnoverPct = value;
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
