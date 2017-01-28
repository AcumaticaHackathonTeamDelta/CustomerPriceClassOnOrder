using System;
using PX.Data;
using PX.Data.EP;
using PX.SM;
using PX.Objects.CS;

namespace PX.Objects.CR
{
	[PXProjection(typeof(Select2<NotificationRecipient,		
		InnerJoin<NotificationSource,
						On<NotificationSource.sourceID, Equal<NotificationRecipient.sourceID>>>>), 
	Persistent = true)]
	[PXCacheName(Messages.ContactNotification)]
    [Serializable]
	public partial class ContactNotification : NotificationRecipient
	{		
		#region SetupID
		public new abstract class setupID : PX.Data.IBqlField
		{
		}
		[PXDBGuid]
		[PXSelector(typeof(Search<NotificationSetup.setupID>),
			 SubstituteKey = typeof(NotificationSetup.notificationCD))]
		[PXUIField(DisplayName = "Notification ID", Enabled = false)]
		public override Guid? SetupID
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
		#region EntityDescription
		public abstract class entityDescription : PX.Data.IBqlField {}
		[PXString]
		[PXUIField(DisplayName = "Description", Enabled = false)]
		[PXFormula(typeof(EntityDescription<refNoteID>))]
		public virtual string EntityDescription { get; set; }
		#endregion		
		#region SourceClassID
		public abstract class sourceClassID : PX.Data.IBqlField
		{
		}
		protected string _SourceClassID;
		[PXDBString(BqlField = typeof(NotificationRecipient.classID))]		
		public virtual string SourceClassID
		{
			get
			{
				return this._SourceClassID;
			}
			set
			{
				this._SourceClassID = value;
			}
		}
		#endregion	
		#region ClassID
		public new abstract class classID : PX.Data.IBqlField
		{
		}
		[PXString]
		[PXUIField(DisplayName = "Class ID", Enabled = false)]
		public override string ClassID
		{
			[PXDependsOnFields(typeof(refNoteID),typeof(sourceClassID))]
			get
			{
				return this.RefNoteID != null ? null : this._SourceClassID;
			}
		}
		#endregion	
		#region ReportID
		public abstract class reportID : PX.Data.IBqlField
		{
		}
		protected String _ReportID;		
		[PXDBString(8, InputMask = "CC.CC.CC.CC", BqlField = typeof(NotificationSource.reportID))]
		[PXUIField(DisplayName = "Report", Enabled = false)]
		[PXFormula(typeof(Default<NotificationSource.setupID>))]
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
		#region TemplateID
		public abstract class templateID : PX.Data.IBqlField
		{
		}
		protected Int32? _TemplateID;
		[PXDBInt(BqlField = typeof(NotificationSource.notificationID))]
		[PXUIField(DisplayName = "Notification Template", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXSelector(typeof(Search<Notification.notificationID>),
			SubstituteKey = typeof(Notification.name),
			DescriptionField = typeof(Notification.name))]
		public virtual Int32? TemplateID
		{
			get
			{
				return this._TemplateID;
			}
			set
			{
				this._TemplateID = value;
			}
		}
		#endregion
		#region ContactID
		public new abstract class contactID : IBqlField { }
		[PXDBInt]
		[PXUIField(DisplayName = "Contact")]
		[PXParent(typeof(Select<Contact, Where<Contact.contactID, Equal<Current<ContactNotification.contactID>>>>))]
		public override Int32? ContactID
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
		#region Format
		public new abstract class format : PX.Data.IBqlField
		{
		}
		[PXDBString(255)]
		[PXUIField(DisplayName = "Format")]
		[NotificationFormat.List]
		[PXNotificationFormat(typeof(ContactNotification.reportID), typeof(ContactNotification.notificationID))]		
		public override string Format
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
		#region KeySourceID
		public abstract class keySourceID : PX.Data.IBqlField
		{
		}		
		[PXDBInt(BqlField=typeof(NotificationSource.sourceID))]
		[PXExtraKey]
		public virtual int? KeySourceID
		{
			get
			{
				return null;
			}
			set
			{				
			}
		}
		#endregion
		#region KeySetupID
		public abstract class keySetupID : PX.Data.IBqlField
		{
		}		
		[PXDBGuid(BqlField=typeof(NotificationSource.setupID))]
		[PXExtraKey]
		public virtual Guid? KeySetupID
		{
			get
			{
				return null;
			}
			set
			{				
			}
		}
		#endregion

		#region NotificationID

		public new abstract class notificationID : IBqlField { }

		#endregion
	}
}
