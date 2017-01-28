using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.SM;
using PX.SM;

namespace PX.Objects.CS
{
    [Serializable]
	[PXCacheName(Messages.NotificationSetup)]
	public partial class NotificationSetup : IBqlTable
	{
		#region SetupID
		public abstract class setupID : PX.Data.IBqlField
		{
		}
		protected Guid? _SetupID;
		[PXDBGuid(true)]
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
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected string _Module;
		[PXDBString(2, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Module", Enabled = false)]
		public virtual string Module
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
		#region SourceCD
		public abstract class sourceCD : PX.Data.IBqlField
		{
		}
		protected string _SourceCD;
		[PXDBString(10, IsFixed = true, IsKey = true, InputMask = "")]
		[PXUIField(DisplayName = "Source", Enabled = false)]		
		public virtual string SourceCD
		{
			get
			{
				return this._SourceCD;
			}
			set
			{
				this._SourceCD = value;
			}
		}
		#endregion		
		#region NotificationCD
		public abstract class notificationCD : PX.Data.IBqlField
		{
		}
		protected string _NotificationCD;
		[PXDBString(30, IsKey = true, InputMask="", IsUnicode = true)]
		[PXUIField(DisplayName = "Mailing ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXCheckUnique(typeof(NotificationSetup.module), typeof(NotificationSetup.sourceCD))]
		[PXDefault]
		public virtual string NotificationCD
		{
			get
			{
				return this._NotificationCD;
			}
			set
			{
				this._NotificationCD = value;
			}
		}
		#endregion
		#region EMailAccount
		public abstract class eMailAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _EMailAccountID;
		[PXDBInt]
		[PXUIField(DisplayName = "Default Email Account")]
		[PXSelector(typeof(EMailAccount.emailAccountID), DescriptionField = typeof(EMailAccount.address))]
		public virtual Int32? EMailAccountID { get; set; }
		#endregion
		#region ReportID
		public abstract class reportID : PX.Data.IBqlField
		{
		}
		protected String _ReportID;
		[PXDBString(8, InputMask = "CC.CC.CC.CC")]
		[PXUIField(DisplayName = "Report ID", Visibility=PXUIVisibility.SelectorVisible )]
		public virtual String ReportID
		{
			get
			{
				return this._ReportID;
			}
			set
			{
				this._ReportID = value;
			}
		}
		#endregion
		
		#region NotificationID
		public abstract class notificationID : PX.Data.IBqlField
		{
		}
		protected Int32? _NotificationID;
		[PXDBInt]
		[PXUIField(DisplayName = "Notification Template", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<Notification.notificationID>), 
			SubstituteKey = typeof(Notification.name), 
			DescriptionField = typeof(Notification.name))]
		public virtual Int32? NotificationID
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

		#region Format
		public abstract class format : PX.Data.IBqlField
		{
		}
		protected string _Format;
		[PXDefault(NotificationFormat.Html)]
		[PXDBString(255)]
		[PXUIField(DisplayName = "Format")]
		[NotificationFormat.List]
		[PXNotificationFormat(typeof(NotificationSetup.reportID), typeof(NotificationSetup.notificationID))]
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
		[PXDefault(true)]
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

	public class NotificationFormat
	{
		public const string Html = "H";
		public const string Excel = "E";
		public const string PDF = "P";

		public class ListAttribute : PXStringListAttribute
		{
			public string[] AllowedValues
			{
				get
				{
					return _AllowedValues;
				}
			}
			public string[] AllowedLabels
			{
				get
				{
					return _AllowedLabels;
				}
			}
			public ListAttribute()
				: base(new string[] { Html, Excel, PDF },
							new string[] { "Html", "Excel", "PDF" })
			{
			}
			protected ListAttribute(string[] values, string[] labels)
				: base(values, labels)
			{
			}
		}
		public class ReportListAttribute : ListAttribute
		{
			public ReportListAttribute()
				: base(new string[] { Html, Excel, PDF },
							new string[] { "Html", "Excel", "PDF" })
			{
			}
		}
		public class TemplateListAttribute : ListAttribute
		{
			public TemplateListAttribute()
				: base(new string[] {  Html },
							new string[] { "Html" })
			{
			}
		}

		public static ListAttribute List = new ListAttribute();
		public static ListAttribute ReportList = new ReportListAttribute();
		public static ListAttribute TemplateList = new TemplateListAttribute();
	}			
}
