using PX.Data;

namespace PX.Objects.CR
{
	public class CRActivityClass : PXIntListAttribute
	{
		//NOTE: don't use 5 and 3. These numbers were used in old version. Or look at sql update script carefully.
		public const int Task = 0;
		public const int Event = 1;
		public const int Activity = 2;
		public const int Email = 4;
		public const int EmailRouting = -2;
		public const int OldEmails = -3;

		public CRActivityClass()
			: base(
				new[]
				{
					Task,
					Event,
					Activity,
					Email,
					EmailRouting,
					OldEmails
				},
				new[]
				{
					Data.EP.Messages.TaskClassInfo,
					Data.EP.Messages.EventClassInfo,
					Messages.ActivityClassInfo,
					Messages.EmailClassInfo,
					Messages.EmailResponse,
					Messages.EmailClassInfo
				})
		{
		}

		public class task : Constant<int>
		{
			public task() : base(Task) { }
		}

		public class events : Constant<int>
		{
			public events() : base(Event) { }
		}

		public class activity : Constant<int>
		{
			public activity() : base(Activity) { }
		}

		public class email : Constant<int>
		{
			public email() : base(Email) { }
		}

		public class emailRouting : Constant<int>
		{
			public emailRouting() : base(EmailRouting) { }
		}

		public class oldEmails : Constant<int>
		{
			public oldEmails() : base(OldEmails) { }
		}
	}
}
