using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.CS
{
    [Serializable]
	[PXCacheName(Messages.NotificationRecipient)]
	public partial class NotificationRecipient : IBqlTable
	{
		#region NotificationID
		public abstract class notificationID : PX.Data.IBqlField
		{
		}
		protected int? _NotificationID;
		[PXDBIdentity(IsKey = true)]
		[PXParent(typeof(Select<NotificationSource,
			Where<NotificationSource.sourceID, Equal<Current<NotificationRecipient.sourceID>>>>))]
		[PXUIField(DisplayName = "NotificationID")]
		public virtual int? NotificationID
		{
			get
			{
				return this._NotificationID;
			}
			set
			{
				this._NotificationID = value;
			}
		}
		#endregion
		#region SetupID
		public abstract class setupID : PX.Data.IBqlField
		{
		}
		protected Guid? _SetupID;
		[PXDBGuid]
		[PXDefault(typeof(NotificationSource.setupID))]
		public virtual Guid? SetupID
		{
			get
			{
				return this._SetupID;
			}
			set
			{
				this._SetupID = value;
			}
		}
		#endregion
		#region SourceID
		public abstract class sourceID : PX.Data.IBqlField
		{
		}
		protected int? _SourceID;
		[PXDBInt()]
		public virtual int? SourceID
		{
			get
			{
				return this._SourceID;
			}
			set
			{
				this._SourceID = value;
			}
		}
		#endregion		
		#region ClassID
		public abstract class classID : PX.Data.IBqlField
		{
		}
		protected string _ClassID;
		[PXDBString(10)]
		[PXDefault(typeof(NotificationSource.classID), PersistingCheck=PXPersistingCheck.Nothing)]
		public virtual string ClassID
		{
			get
			{
				return this._ClassID;
			}
			set
			{
				this._ClassID = value;
			}
		}
		#endregion		
		#region RefNoteID
		public abstract class refNoteID : PX.Data.IBqlField
		{
		}
		protected Guid? _RefNoteID;
		[PXDBDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBGuid]
		public virtual Guid? RefNoteID
		{
			get
			{
				return this._RefNoteID;
			}
			set
			{
				this._RefNoteID = value;
			}
		}
		#endregion
		#region ContactType
		public abstract class contactType : PX.Data.IBqlField
		{
		}
		protected string _ContactType;		
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Contact Type")]		
		public virtual string ContactType
		{
			get
			{
				return this._ContactType;
			}
			set
			{
				this._ContactType = value;
			}
		}
		#endregion
		#region ContactID
		public abstract class contactID : PX.Data.IBqlField
		{
		}
		protected Int32? _ContactID;
		[PXDBInt]
		[PXUIField(DisplayName = "Contact")]	
		public virtual Int32? ContactID
		{
			get
			{
				return this._ContactID;
			}
			set
			{
				this._ContactID = value;
			}
		}
		#endregion					
		#region OriginalContactID
		public abstract class originalContactID : IBqlField { }
		[PXInt]
		[PXUIField(Visible = false)]
		public Int32? OriginalContactID
		{
			get
			{
				return this.ContactID;
			}
		}
		#endregion
		#region Format
		public abstract class format : PX.Data.IBqlField
		{
		}
		protected string _Format;
		[PXDefault(typeof(Search<NotificationSource.format,
			Where<NotificationSource.sourceID, Equal<Current<NotificationRecipient.sourceID>>>>))]
		[PXDBString(255)]
		[PXUIField(DisplayName = "Format")]
		[NotificationFormat.List]
		[PXNotificationFormat(
			typeof(NotificationSource.reportID), 
			typeof(NotificationSource.notificationID),
			typeof(Where<NotificationRecipient.sourceID, Equal<Current<NotificationSource.sourceID>>>))]
		public virtual string Format
		{
			get
			{
				return this._Format;
			}
			set
			{
				this._Format = value;
			}
		}
		#endregion		
		#region Active
		public abstract class active : PX.Data.IBqlField
		{
		}
		protected bool? _Active;
		[PXDBBool()]
		[PXDefault(typeof(Search<NotificationSource.active,
			Where<NotificationSource.sourceID, Equal<Current<NotificationRecipient.sourceID>>>>))]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				this._Active = value;
			}
		}
		#endregion
		#region Hidden
		public abstract class hidden : PX.Data.IBqlField
		{
		}
		protected bool? _Hidden;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Bcc")]
		public virtual bool? Hidden
		{
			get
			{
				return this._Hidden;
			}
			set
			{
				this._Hidden = value;
			}
		}
		#endregion
		#region Email
		public abstract class email : PX.Data.IBqlField
		{
		}
		protected string _Email;
		[PXString()]		
		public virtual string Email
		{
			get
			{
				return this._Email;
			}
			set
			{
				this._Email = value;
			}
		}
		#endregion
		#region OrderID
		public abstract class orderID : PX.Data.IBqlField
		{
		}
		protected int? _OrderID;
		[PXInt()]		
		public virtual int? OrderID
		{
			get
			{
				return this._OrderID;
			}
			set
			{
				this._OrderID = value;
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

	public class NotificationContactType
	{
		public const string Employee = "E";
		public const string Contact = "C";
		public const string Primary = "P";
		public const string Remittance = "R";
		public const string Shipping = "S";
		public const string Billing = "B";

		public class employee : Constant<string> { public employee() : base(Employee) { } }
		public class contact : Constant<string> { public contact() : base(Contact) { } }
		public class primary : Constant<string> { public primary() : base(Primary) { } }
		public class remittance : Constant<string> { public remittance() : base(Remittance) { } }
		public class shipping : Constant<string> { public shipping() : base(Shipping) { } }
		public class billing : Constant<string> { public billing() : base(Billing) { } }

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(new string[] { Primary, Billing, Shipping, Remittance, Employee, Contact },
							 new string[] { CR.Messages.Primary, AR.Messages.Billing, AR.Messages.Shipping, AP.Messages.Remittance, EP.Messages.Employee, CR.Messages.Contact })
			{
			}
		}
	}	
}
