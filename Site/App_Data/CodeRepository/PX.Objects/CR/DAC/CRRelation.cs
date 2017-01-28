using System;
using PX.Data;

namespace PX.Objects.CR
{
	[Serializable]
	[PXCacheName(Messages.Relations)]
	public partial class CRRelation : IBqlTable
	{
		#region RelationID

		public abstract class relationID : IBqlField { }

		[PXDBIdentity(IsKey = true)]
		[PXUIField(Visible = false)]
		public virtual Int32? RelationID { get; set; }

		#endregion

		#region RefNoteID

		public abstract class refNoteID : IBqlField { }

		[PXParent(typeof(Select<Contact, Where<Contact.noteID, Equal<Current<CRRelation.refNoteID>>>>))]
		[PXParent(typeof(Select<BAccount, Where<BAccount.noteID, Equal<Current<CRRelation.refNoteID>>>>))]
		[PXDBGuid]
		[PXDBDefault(null)]
		public virtual Guid? RefNoteID { get; set; }

		#endregion

		#region EntityID

		public abstract class entityID : IBqlField { }

		[PXDBInt]
		[PXSelector(typeof(Search<BAccount.bAccountID,
						Where<BAccount.type, Equal<BAccountType.prospectType>,
							Or<BAccount.type, Equal<BAccountType.customerType>,
							Or<BAccount.type, Equal<BAccountType.vendorType>,
							Or<BAccount.type, Equal<BAccountType.combinedType>,
							Or<BAccount.type, Equal<BAccountType.employeeType>>>>>>>),
			new[] { typeof(BAccount.acctCD), typeof(BAccount.acctName), typeof(BAccount.type), 
				typeof(BAccount.parentBAccountID), typeof(BAccount.acctReferenceNbr) },
			DescriptionField = typeof(BAccount.acctName), Filterable = true)]
		[PXUIField(DisplayName = "Account/Employee")]
		public virtual Int32? EntityID { get; set; }

		#endregion

		#region EntityCD

		public abstract class entityCD : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Account/Employee", Enabled = false)]
		public virtual String EntityCD { get; set; }

		#endregion

		#region Role

		public abstract class role : IBqlField { }

		[PXDBString(2)]
		[PXUIField(DisplayName = "Role")]
		[PXDefault]
		[PXStringListAttribute]
		public virtual String Role { get; set; }

		#endregion

		#region ContactID

		public abstract class contactID : IBqlField
		{
		}

		[PXDBInt]
		[PXUIField(DisplayName = "Contact")]
		[PXSelector(typeof(Search<Contact.contactID,
			Where2<
				Where<Contact.contactType, Equal<ContactTypesAttribute.person>,
					Or<Contact.contactType, Equal<ContactTypesAttribute.lead>>>,
				And<
				Where<Current<CRRelation.entityID>, IsNull,
					Or<Contact.bAccountID, Equal<Current<CRRelation.entityID>>>>>>>),
			Filterable = true,
			DescriptionField = typeof(Contact.displayName))]
		public virtual Int32? ContactID { get; set; }

		#endregion

		#region AddToCC

		public abstract class addToCC : IBqlField { }

		[PXDBBool]
		[PXUIField(DisplayName = "Add to CC")]
		public virtual Boolean? AddToCC { get; set; }

		#endregion

		#region Name

		public abstract class name : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Name", Enabled = false)]
		public virtual String Name { get; set; }

		#endregion

		#region ContactName

		public abstract class contactName : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Contact", Enabled = false)]
		public virtual String ContactName { get; set; }

		#endregion

		#region Email

		public abstract class email : IBqlField { }

		[PXString]
		[PXUIField(DisplayName = "Email", Enabled = false)]
		public virtual String Email { get; set; }

		#endregion
	}
}
