using PX.Data.EP;

namespace PX.Objects.AR
{
	using System;
	using PX.Data;

	public static class PrepareOnType 
	{
		public const string FixedDayOfMonth = "F";
		public const string EndOfMonth = "E";
		public const string Custom = "C";
		public class ListAttribute : PXStringListAttribute 
		{
			public ListAttribute():
				base(
					new string[]{FixedDayOfMonth,EndOfMonth,Custom},
					new string[]{"Fixed Day of Month","End of Month","Custom"}
					){}
		}
	}
	[PXCacheName(Messages.StatementCycle)]
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ARStatementMaint))]
	public partial class ARStatementCycle : PX.Data.IBqlTable
	{
		#region Selected
		public abstract class selected : IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public bool? Selected
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
		#region NextStatementDate
		public abstract class nextStmtDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _NextStmtDate;
		[PXDate()]
		[PXUIField(DisplayName = "Next Statement Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? NextStmtDate
		{
			get
			{
				return this._NextStmtDate;
			}
			set
			{
				this._NextStmtDate = value;
			}
		}
		#endregion
		#region StatementCycleId
		public abstract class statementCycleId : PX.Data.IBqlField
		{
		}
		protected String _StatementCycleId;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Cycle ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(ARStatementCycle.statementCycleId))]
		[PXFieldDescription]
		public virtual String StatementCycleId
		{
			get
			{
				return this._StatementCycleId;
			}
			set
			{
				this._StatementCycleId = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = " Description",Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault()]
		[PXFieldDescription]
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
		#region AgeDays00
		public abstract class ageDays00 : PX.Data.IBqlField
		{
		}
		protected Int16? _AgeDays00;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Age Days 1", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int16? AgeDays00
		{
			get
			{
				return this._AgeDays00;
			}
			set
			{
				this._AgeDays00 = value;
			}
		}
		#endregion
		#region AgeDays01
		public abstract class ageDays01 : PX.Data.IBqlField
		{
		}
		protected Int16? _AgeDays01;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Age Days 2", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int16? AgeDays01
		{
			get
			{
				return this._AgeDays01;
			}
			set
			{
				this._AgeDays01 = value;
			}
		}
		#endregion
		#region AgeDays02
		public abstract class ageDays02 : PX.Data.IBqlField
		{
		}
		protected Int16? _AgeDays02;
		[PXDBShort()]
		[PXDefault((short)0)]
		[PXUIField(DisplayName = "Age Days 3", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int16? AgeDays02
		{
			get
			{
				return this._AgeDays02;
			}
			set
			{
				this._AgeDays02 = value;
			}
		}
		#endregion
		#region AgeMsg00
		public abstract class ageMsg00 : PX.Data.IBqlField
		{
		}
		protected String _AgeMsg00;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Age Message 1", Visibility = PXUIVisibility.Visible)]
		public virtual String AgeMsg00
		{
			get
			{
				return this._AgeMsg00;
			}
			set
			{
				this._AgeMsg00 = value;
			}
		}
		#endregion
		#region AgeMsg01
		public abstract class ageMsg01 : PX.Data.IBqlField
		{
		}
		protected String _AgeMsg01;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Age Message 2", Visibility = PXUIVisibility.Visible)]
		public virtual String AgeMsg01
		{
			get
			{
				return this._AgeMsg01;
			}
			set
			{
				this._AgeMsg01 = value;
			}
		}
		#endregion
		#region AgeMsg02
		public abstract class ageMsg02 : PX.Data.IBqlField
		{
		}
		protected String _AgeMsg02;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Age Message 3", Visibility = PXUIVisibility.Visible)]
		public virtual String AgeMsg02
		{
			get
			{
				return this._AgeMsg02;
			}
			set
			{
				this._AgeMsg02 = value;
			}
		}
		#endregion
		#region AgeMsg03
		public abstract class ageMsg03 : PX.Data.IBqlField
		{
		}
		protected String _AgeMsg03;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Age Message 4", Visibility = PXUIVisibility.Visible)]
		public virtual String AgeMsg03
		{
			get
			{
				return this._AgeMsg03;
			}
			set
			{
				this._AgeMsg03 = value;
			}
		}
		#endregion
		#region LastAgeDate
		public abstract class lastAgeDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastAgeDate;
		[PXDBDate()]
		public virtual DateTime? LastAgeDate
		{
			get
			{
				return this._LastAgeDate;
			}
			set
			{
				this._LastAgeDate = value;
			}
		}
		#endregion
		#region LastFinChrgDate
		public abstract class lastFinChrgDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastFinChrgDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Finance Charge Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? LastFinChrgDate
		{
			get
			{
				return this._LastFinChrgDate;
			}
			set
			{
				this._LastFinChrgDate = value;
			}
		}
		#endregion
		#region LastStmtDate
		public abstract class lastStmtDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastStmtDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Last Statement Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? LastStmtDate
		{
			get
			{
				return this._LastStmtDate;
			}
			set
			{
				this._LastStmtDate = value;
			}
		}
		#endregion
		#region PrepareOn
		public abstract class prepareOn : PX.Data.IBqlField
		{
		}
		protected String _PrepareOn;
		[PXDBString(1, IsFixed = true)]
		[PrepareOnType.List()]
		[PXDefault(PrepareOnType.EndOfMonth)]
		[PXUIField(DisplayName = "Prepare On")]
		public virtual String PrepareOn
		{
			get
			{
				return this._PrepareOn;
			}
			set
			{
				this._PrepareOn = value;
			}
		}
		#endregion
		#region Day00
		public abstract class day00 : PX.Data.IBqlField
		{
		}
		protected Int16? _Day00;
		[PXDBShort()]
		[PXUIField(DisplayName = "Day Of Month")]
		public virtual Int16? Day00
		{
			get
			{
				return this._Day00;
			}
			set
			{
				this._Day00 = value;
			}
		}
		#endregion
		#region Day01
		public abstract class day01 : PX.Data.IBqlField
		{
		}
		protected Int16? _Day01;
		[PXDBShort()]
		[PXUIField(DisplayName = "Day Of Month 1")]
		public virtual Int16? Day01
		{
			get
			{
				return this._Day01;
			}
			set
			{
				this._Day01 = value;
			}
		}
		#endregion
		#region FinChargeApply
		public abstract class finChargeApply : PX.Data.IBqlField
		{
		}
		protected Boolean? _FinChargeApply;
		[PXDBBool()]
		[PXDefault(false, typeof(Search<CustomerClass.finChargeApply, Where<CustomerClass.customerClassID, Equal<Current<Customer.customerClassID>>>>))]
		[PXUIField(DisplayName = "Apply Overdue Charges")]
		public virtual Boolean? FinChargeApply
		{
			get
			{
				return this._FinChargeApply;
			}
			set
			{
				this._FinChargeApply = value;
			}
		}
		#endregion
		#region FinChargeID
		public abstract class finChargeID : PX.Data.IBqlField
		{
		}
		protected String _FinChargeID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Overdue Charge ID")]
		[PXSelector(typeof(ARFinCharge.finChargeID), DescriptionField = typeof(ARFinCharge.finChargeDesc))]
		public virtual String FinChargeID
		{
			get
			{
				return this._FinChargeID;
			}
			set
			{
				this._FinChargeID = value;
			}
		}
		#endregion
		#region RequirePaymentApplication
		public abstract class requirePaymentApplication : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequirePaymentApplication;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Require Payment Application Before Statement Processing")]
		public virtual Boolean? RequirePaymentApplication
		{
			get
			{
				return this._RequirePaymentApplication;
			}
			set
			{
				this._RequirePaymentApplication = value;
			}
		}
		#endregion
		#region RequireFinChargeProcessing
		public abstract class requireFinChargeProcessing : PX.Data.IBqlField
		{
		}
		protected Boolean? _RequireFinChargeProcessing;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Require Overdue Charges Calculation Before Statement")]
		public virtual Boolean? RequireFinChargeProcessing
		{
			get
			{
				return this._RequireFinChargeProcessing;
			}
			set
			{
				this._RequireFinChargeProcessing = value;
			}
		}
		#endregion
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField { }
		[PXNote(DescriptionField = typeof(ARStatementCycle.statementCycleId))]
		public virtual Guid? NoteID { get; set; }
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
