using System;

namespace PX.Data.Wiki.Parser
{
	[PX.Common.PXLocalizable]
	public static class Messages
	{
		public const string Edit = "Edit";
		public const string Close = "Close";
		public const string GetLink = "Get Link";
		public const string ttipGetLink = "Get link to this article.";
		public const string Print = "Print";
		public const string ttipPrint = "Print this article.";
		public const string WikiText = "Wiki Text";
		public const string ttipWikiText = "View wiki mark-up.";
		public const string Export = "Export";
		public const string PlainText = "Plain Text";
		public const string Word = "Word";
        public const string DITA = "DITA";
	}
}

namespace PX.Data.Search
{
	[PX.Common.PXLocalizable]
	public static class Messages
	{
		public const string scrSearch = "Search";
		public const string scrEntitySearch = "Search In Entities";
		public const string scrWikiSearch = "Search In Help";
		public const string scrFileSearch = "Search In Files";
		public const string scrNoteSearch = "Search In Notes";
		public const string scrFormSearch = "Search In Forms";
		public const string scrKB = "Search In Knowledge Base";
		public const string ttipHelpSearch = "Search for Wiki articles.";
		public const string ttipFileSearch = "Files search.";
		public const string ttipBAccountSearch = "Vendors/Customers search";
		public const string ttipEntitySearch = "Entities search";
		public const string ttipNoteSearch = "Notes search";
		public const string SpecifySearchRequest = "Enter your search request.";
		public const string SearchReplaceEmpty = "The \"Replace with :\" field is empty. Click OK to replace matching text with empty values or click Cancel to perform search without replace.";
		public const string WikiArticles = "All Wikis";
		public const string Files = "Files";
		public const string Notes = "Notes";
		public const string KB = "Knowledge Base";
		public const string BAccount = "Vendors/Customers";
		public const string Entity = "Entities";
		public const string NothingFound = "No results found for";
		public const string AllEntities = "All Entities";
		public const string SelectedEntities = "Selected...";
		public const string EntityName = "Entity Name";
		public const string EntityType = "Entity Type";
		public const string SearchIn = "Search In:";
		public const string Preferences = "Preferences";
		public const string Advanced = "Advanced";
		public const string SearchTips = "Search tips";
		public const string CheckSpelling = "Make sure that all words are spelled correctly.";
		public const string SimplifyQuery = "Consider simplifying complex or wordy queries.";
		public const string TryOtherWords = "Try other words.";
		public const string TryFullTextSearch = "You may want to try full-text search; click the Full-text search check box and repeat your request. ";
		public const string SearchResults = "Results <b>{0}-{1}</b> of <b>{2}</b> for: <b><i>{3}</i></b> ({4} seconds)";
		public const string SearchResultsShort = "Results for: <b><i>{0}</i></b> ({1} seconds)";
		public const string SearchIndexIsEmpty = "Full-Text Entity Index must be build before using the search. Please rebuild the index and try again.";
		public const string Open = "Open";
		public const string OpenInNewTab = "Open in new tab";
        public const string IndexDisabled = "Warning! The Full-Text Search feature is not enabled in the database. Search by a phrase or a list of keywords might not work. You can contact the database administrator to enable the feature.";

    }
}

namespace PX.AscxControlsMessages
{
	[PX.Common.PXLocalizable]
	public static class LoginScreen
	{
		public const string PleaseChangePassword = "You are required to change password. Please, enter your new password below.";
		public const string InvalidRecoveryAnswer = "Invalid recovery answer.";
		public const string NewPasswordMustDiffer = "New password must differ from the old one.";
		public const string PasswordNotConfirmed = "The password entered doesn't match confirmation.";
		public const string PasswordBlank = "New password can not be blank.";
		public const string EmailEmpty = "Please, enter your e-mail address.";
		public const string InvalidQueryString = "Invalid query string parameters, e-mail could not be sent.";
		public const string PasswordSent = "The e-mail containing instructions on further actions is sent to your address.";
		public const string InvalidLogin = "Please, enter your valid login.";
		public const string DbNotAccessible = "Database could not be accessed";
		public const string YouAreNotLoggedIn = "You are not currently logged in.";
		public const string PleaseSelectCompany = "Please select company.";
		public const string IncorrectLoginSymbols = "Invalid credentials. Please try again.";
	}

	[PX.Common.PXLocalizable]
	public static class SearchBox
	{
		public const string TypeYourQueryHere = "Type your query here";
		public const string SearchSystem = "Search in system.";
	}

	[PX.Common.PXLocalizable]
	public static class PageTitle
	{
		public const string Reminder = "Reminder";
		public const string ttipReminder = "Shows Tasks And Events";
		public const string UserID = "Username";
		public const string ScreenID = "Screen ID";
		public const string TimeZone = "Time Zone";
		public const string TargetFrame = "Target Frame :";
		public const string Title = "Title :";
		public const string Shortcut = "Shortcut :";
		public const string MaxLines = "Max. Lines :";
		public const string MaxPoints = "Max. Points :";
		public const string ChartType = "Chart type";
		public const string Line = "Line";
		public const string Bar = "Bar";
		public const string Pie = "Pie";
		public const string Version = "Version {0}";
		public const string Copyright2 = "Acumatica and Acumatica Logos are trademarks of Acumatica, Inc.<br />All rights reserved.";
		public const string InstallationID = "Installation ID:<br /> {0}";
		public const string Customization = "Customization: ";
		public const string Updates = "New version of Acumatica is available.";


	}

	[PX.Common.PXLocalizable]
	public static class TasksAndEventsPanel
	{
		public const string Tasks = "Tasks";
		public const string Events = "Events";
        public const string Emails = "Emails";
	}
}