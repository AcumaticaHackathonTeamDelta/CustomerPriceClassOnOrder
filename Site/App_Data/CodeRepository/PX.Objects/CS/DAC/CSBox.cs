using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.CS
{
	[System.SerializableAttribute()]
	public partial class CSBox : IBqlTable
	{
		#region BoxID
		public abstract class boxID : PX.Data.IBqlField
		{
		}
		protected String _BoxID;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDefault()]
		[PXUIField(DisplayName = "Box ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String BoxID
		{
			get
			{
				return this._BoxID;
			}
			set
			{
				this._BoxID = value;
			}
		}
		#endregion
		#region MaxWeight
		public abstract class maxWeight : PX.Data.IBqlField
		{
		}
		protected Decimal? _MaxWeight;

		[PXDBDecimal(4, MinValue = 0)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Max. Weight", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? MaxWeight
		{
			get
			{
				return this._MaxWeight;
			}
			set
			{
				this._MaxWeight = value;
			}
		}
		#endregion
        #region BoxWeight
        public abstract class boxWeight : PX.Data.IBqlField
        {
        }
        protected Decimal? _BoxWeight;

        [PXDBDecimal(4, MinValue = 0)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Box Weight", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? BoxWeight
        {
            get
            {
                return this._BoxWeight;
            }
            set
            {
                this._BoxWeight = value;
            }
        }
        #endregion
        #region MaxVolume
        public abstract class maxVolume : PX.Data.IBqlField
        {
        }
        protected Decimal? _MaxVolume;

        [PXDBDecimal(4, MinValue = 0)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Max Volume", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? MaxVolume
        {
            get
            {
                return this._MaxVolume;
            }
            set
            {
                this._MaxVolume = value;
            }
        }
        #endregion
		#region Length
		public abstract class length : PX.Data.IBqlField
		{
		}
		protected int? _Length;
		[PXDBInt(MinValue = 0)]
		[PXUIField(DisplayName = "Length", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Length
		{
			get
			{
				return this._Length;
			}
			set
			{
				this._Length = value;
			}
		}
		#endregion
		#region Width
		public abstract class width : PX.Data.IBqlField
		{
		}
		protected int? _Width;
		[PXDBInt(MinValue=0)]
		[PXUIField(DisplayName = "Width", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Width
		{
			get
			{
				return this._Width;
			}
			set
			{
				this._Width = value;
			}
		}
		#endregion
		#region Height
		public abstract class height : PX.Data.IBqlField
		{
		}
		protected int? _Height;
		[PXDBInt(MinValue = 0)]
		[PXUIField(DisplayName = "Height", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual int? Height
		{
			get
			{
				return this._Height;
			}
			set
			{
				this._Height = value;
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
		#region CarrierBox
		public abstract class carrierBox : PX.Data.IBqlField
		{
		}
		protected String _CarrierBox;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Carrier's Package", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String CarrierBox
		{
			get
			{
				return this._CarrierBox;
			}
			set
			{
				this._CarrierBox = value;
			}
		}
		#endregion
		#region ActiveByDefault
		public abstract class activeByDefault : PX.Data.IBqlField
		{
		}
		protected Boolean? _ActiveByDefault;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Active By Default")]
		public virtual Boolean? ActiveByDefault
		{
			get
			{
				return this._ActiveByDefault;
			}
			set
			{
				this._ActiveByDefault = value;
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

        
	}
}
