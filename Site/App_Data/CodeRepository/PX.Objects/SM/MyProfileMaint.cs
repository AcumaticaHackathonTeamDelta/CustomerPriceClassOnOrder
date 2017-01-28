using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using System;

namespace PX.SM
{
	public class MyProfileMaint : SMAccessPersonalMaint
	{
		#region Selects

		public PXSelect<SMLanguageSearchSettings> LanguageSearchSettings;
		public PXSelect<Contact, Where<Contact.userID, Equal<Optional<Users.pKID>>>> Contact;

		public PXSelectJoin<EMailSyncAccount,
			InnerJoin<BAccount,
				On<BAccount.bAccountID, Equal<EMailSyncAccount.employeeID>>,
			InnerJoin<Contact,
				On<Contact.contactID, Equal<BAccount.defContactID>,
				And<Contact.userID, Equal<Optional<Users.pKID>>>>>>> SyncAccount;

		public PXSelect<EMailAccount,
			Where<EMailAccount.emailAccountID, Equal<Optional<EMailSyncAccount.emailAccountID>>>> EMailAccountsNew;

		#endregion

		#region Data Handlers

		public virtual IEnumerable languageSearchSettings()
		{
			var cache = LanguageSearchSettings.Cache;
			var oldDirtyValue = cache.IsDirty;
			foreach (PXResult<SMLanguageSearchSettings, WikiPageLanguage> record in
				PXSelectJoinGroupBy<SMLanguageSearchSettings,
					RightJoin<WikiPageLanguage, On<WikiPageLanguage.language, Equal<SMLanguageSearchSettings.name>>>,
					Where<SMLanguageSearchSettings.userID, IsNull,
						Or<SMLanguageSearchSettings.userID, Equal<Current<AccessInfo.userID>>>>,
					Aggregate<GroupBy<WikiPageLanguage.language, 
						GroupBy<SMLanguageSearchSettings.userID, 
						GroupBy<SMLanguageSearchSettings.active>>>>>.
					Select(this))
			{
				var langSettings = (SMLanguageSearchSettings)record;
				var pageLanguage = (WikiPageLanguage)record;
				if (langSettings.UserID == null)
				{
					var fieldValues = new OrderedDictionary
					                  	{
					                  		{cache.GetField(typeof(SMLanguageSearchSettings.name)), pageLanguage.Language},
					                  		{cache.GetField(typeof(SMLanguageSearchSettings.userID)), PXAccess.GetUserID()},
					                  		{cache.GetField(typeof(SMLanguageSearchSettings.active)), false}
					                  	};
					cache.Insert(fieldValues);
					langSettings = (SMLanguageSearchSettings)cache.Current;
				}
				yield return langSettings;
			}
			cache.IsDirty = oldDirtyValue;
		}

		#endregion

		#region Event Handlers
		[PXDBString(50)]
		[PXUIField(DisplayName = "Phone", Visibility = PXUIVisibility.SelectorVisible)]
		[PhoneValidation]
		protected virtual void Users_Phone_CacheAttached(PXCache sender) {}

		protected virtual void UserPreferences_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			ResetTimeZone.SetVisible(true);
		}

		protected virtual void UserPreferences_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			if (e.TranStatus == PXTranStatus.Completed)
			{
				if (System.Web.HttpContext.Current != null &&
				    String.Equals(this.UserProfile.Current.Username, PXAccess.GetUserName()))
					// do not throw an exception to prevent breaking of the event-handling chain
					Redirector.Refresh(System.Web.HttpContext.Current);
			}
		}

		#endregion

		#region Public Methods

		public override string GetUserTimeZoneId(string username)
		{
			var result = base.GetUserTimeZoneId(username);
			if (string.IsNullOrEmpty(result))
			{
				var set = PXSelectJoin<CSCalendar,
					InnerJoin<EPEmployee, On<EPEmployee.calendarID, Equal<CSCalendar.calendarID>>,
						InnerJoin<Users, On<Users.pKID, Equal<EPEmployee.userID>>>>,
					Where<Users.username, Equal<Required<Users.username>>>>.
					Select(this, username);
				if (set != null && set.Count > 0) result = ((CSCalendar)set[0][typeof(CSCalendar)]).TimeZone;
			}
			return result;
		}

		protected override string GetDefaultUserTimeZoneId(string username)
		{
			CSCalendar calendar = PXSelectJoin<CSCalendar,
				InnerJoin<EPEmployee, On<EPEmployee.calendarID, Equal<CSCalendar.calendarID>>,
					InnerJoin<Users, On<Users.pKID, Equal<EPEmployee.userID>>>>,
				Where<Users.username, Equal<Required<Users.username>>>>.
				Select(this, username);
			
			if (calendar != null && !string.IsNullOrEmpty(calendar.TimeZone))
				return calendar.TimeZone;

			return base.GetDefaultUserTimeZoneId(username);
		}

		[PXUIField(DisplayName = "Change Email", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public override void changeEmail()
		{
			base.changeEmail();
			foreach (Contact copy in Contact.Select(UserProfile.Current.PKID)
				       .RowCast<Contact>()
				       .Select(contact => (Contact) Contact.Cache.CreateCopy(contact)))
			{
				copy.EMail = UserProfile.Current.Email;
				Contact.Update(copy);
			}

			foreach (EMailSyncAccount syncAccount in SyncAccount.Select(UserProfile.Current.PKID)
					   .RowCast<EMailSyncAccount>()
					   .Select(account => (EMailSyncAccount)SyncAccount.Cache.CreateCopy(account)))
			{
				syncAccount.Address = UserProfile.Current.Email;

				syncAccount.ContactsExportDate = null;
				syncAccount.ContactsImportDate = null;
				syncAccount.EmailsExportDate = null;
				syncAccount.EmailsImportDate = null;
				syncAccount.TasksExportDate = null;
				syncAccount.TasksImportDate = null;
				syncAccount.EventsExportDate = null;
				syncAccount.EventsImportDate = null;

				EMailAccount mailAccount = EMailAccountsNew.Select(syncAccount.EmailAccountID);
				mailAccount.Address = syncAccount.Address;

				EMailAccountsNew.Update(mailAccount);

				SyncAccount.Update(syncAccount);
			}

			Actions.PressSave();
		}

		#endregion
	}
}
