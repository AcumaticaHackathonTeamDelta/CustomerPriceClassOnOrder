namespace PX.Objects.AR
{
	using System;
	using PX.Data;
	using PX.Objects.GL;
	using PX.Objects.CM;
	
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(ARStatementUpdate))]
	[PXEMailSource]
	[PXCacheName(Messages.Statement)]
	public partial class ARStatement : PX.Data.IBqlTable
	{
        #region BranchID
        public abstract class branchID : PX.Data.IBqlField
        {
        }
        protected Int32? _BranchID;
        [GL.Branch(IsKey = true)]
        public virtual Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXSelector(typeof(CM.Currency.curyID), CacheGlobal = true)]
		[PXUIField(DisplayName = "Currency ID")]
		public virtual String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : PX.Data.IBqlField
		{
		}
		protected Int32? _CustomerID;
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Customer", IsReadOnly = true)]
		public virtual Int32? CustomerID
		{
			get
			{
				return this._CustomerID;
			}
			set
			{
				this._CustomerID = value;
			}
		}
		#endregion
		#region StatementCustomerID
		public abstract class statementCustomerID : IBqlField { }
		[PXDBInt]
		[PXDefault]
		public virtual Int32? StatementCustomerID { get; set; }
		#endregion
		#region StatementDate
		public abstract class statementDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StatementDate;
		[PXDBDate(IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Statement Date")]
		[PXSelector(typeof(Search4<ARStatement.statementDate,Aggregate<GroupBy<ARStatement.statementDate>>>))]
		public virtual DateTime? StatementDate
		{
			get
			{
				return this._StatementDate;
			}
			set
			{
				this._StatementDate = value;
			}
		}
        #endregion
        #region PrevStatementDate
        public abstract class prevStatementDate : PX.Data.IBqlField
        {
        }
        protected DateTime? _PrevStatementDate;
        [PXDBDate]
        [PXUIField(DisplayName = "Last Statement Date")]
        public virtual DateTime? PrevStatementDate
        {
            get
            {
                return this._PrevStatementDate;
            }
            set
            {
                this._PrevStatementDate = value;
            }
        }
        #endregion
		#region StatementCycleId
		public abstract class statementCycleId : PX.Data.IBqlField
		{
		}
		protected String _StatementCycleId;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Statement Cycle ID")]
		[PXSelector(typeof(ARStatementCycle.statementCycleId))]
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
		#region StatementType
		public abstract class statementType : PX.Data.IBqlField
		{
		}
		protected String _StatementType;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Statement Type")]
		public virtual String StatementType
		{
			get
			{
				return this._StatementType;
			}
			set
			{
				this._StatementType = value;
			}
		}
		#endregion
		#region BegBalance
		public abstract class begBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _BegBalance;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Beg. Balance")]
		public virtual Decimal? BegBalance
		{
			get
			{
				return this._BegBalance;
			}
			set
			{
				this._BegBalance = value;
			}
		}
		#endregion
		#region CuryBegBalance
		public abstract class curyBegBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryBegBalance;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Curr. Beg. Balance")]
		public virtual Decimal? CuryBegBalance
		{
			get
			{
				return this._CuryBegBalance;
			}
			set
			{
				this._CuryBegBalance = value;
			}
		}
		#endregion
		#region EndBalance
		public abstract class endBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _EndBalance;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? EndBalance
		{
			get
			{
				return this._EndBalance;
			}
			set
			{
				this._EndBalance = value;
			}
		}
		#endregion
		#region CuryEndBalance
		public abstract class curyEndBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryEndBalance;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		public virtual Decimal? CuryEndBalance
		{
			get
			{
				return this._CuryEndBalance;
			}
			set
			{
				this._CuryEndBalance = value;
			}
		}
		#endregion
		#region AgeBalance00
		public abstract class ageBalance00 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AgeBalance00;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Age00 Balance")]
		public virtual Decimal? AgeBalance00
		{
			get
			{
				return this._AgeBalance00;
			}
			set
			{
				this._AgeBalance00 = value;
			}
		}
		#endregion
		#region CuryAgeBalance00
		public abstract class curyAgeBalance00 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAgeBalance00;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Age00 Balance")]
		public virtual Decimal? CuryAgeBalance00
		{
			get
			{
				return this._CuryAgeBalance00;
			}
			set
			{
				this._CuryAgeBalance00 = value;
			}
		}
		#endregion
		#region AgeBalance01
		public abstract class ageBalance01 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AgeBalance01;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Age01 Balance")]
		public virtual Decimal? AgeBalance01
		{
			get
			{
				return this._AgeBalance01;
			}
			set
			{
				this._AgeBalance01 = value;
			}
		}
		#endregion
		#region CuryAgeBalance01
		public abstract class curyAgeBalance01 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAgeBalance01;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Age01 Balance")]
		public virtual Decimal? CuryAgeBalance01
		{
			get
			{
				return this._CuryAgeBalance01;
			}
			set
			{
				this._CuryAgeBalance01 = value;
			}
		}
		#endregion
		#region AgeBalance02
		public abstract class ageBalance02 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AgeBalance02;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Age02 Balance")]
		public virtual Decimal? AgeBalance02
		{
			get
			{
				return this._AgeBalance02;
			}
			set
			{
				this._AgeBalance02 = value;
			}
		}
		#endregion
		#region CuryAgeBalance02
		public abstract class curyAgeBalance02 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAgeBalance02;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Age02 Balance")]
		public virtual Decimal? CuryAgeBalance02
		{
			get
			{
				return this._CuryAgeBalance02;
			}
			set
			{
				this._CuryAgeBalance02 = value;
			}
		}
		#endregion
		#region AgeBalance03
		public abstract class ageBalance03 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AgeBalance03;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Age03 Balance")]
		public virtual Decimal? AgeBalance03
		{
			get
			{
				return this._AgeBalance03;
			}
			set
			{
				this._AgeBalance03 = value;
			}
		}
		#endregion
		#region CuryAgeBalance03
		public abstract class curyAgeBalance03 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAgeBalance03;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Age03 Balance")]
		public virtual Decimal? CuryAgeBalance03
		{
			get
			{
				return this._CuryAgeBalance03;
			}
			set
			{
				this._CuryAgeBalance03 = value;
			}
		}
		#endregion
		#region AgeBalance04
		public abstract class ageBalance04 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AgeBalance04;
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Age04 Balance")]
		public virtual Decimal? AgeBalance04
		{
			get
			{
				return this._AgeBalance04;
			}
			set
			{
				this._AgeBalance04 = value;
			}
		}
		#endregion
		#region CuryAgeBalance04
		public abstract class curyAgeBalance04 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAgeBalance04;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal,"0.0")]
		[PXUIField(DisplayName = "Cury. Age04 Balance")]
		public virtual Decimal? CuryAgeBalance04
		{
			get
			{
				return this._CuryAgeBalance04;
			}
			set
			{
				this._CuryAgeBalance04 = value;
			}
		}
		#endregion
		#region AgeDays00
		public abstract class ageDays00 : PX.Data.IBqlField
		{
		}
		protected Int16? _AgeDays00;
		[PXDBShort()]
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
		#region AgeDays03
		public abstract class ageDays03 : PX.Data.IBqlField
		{
		}
		protected Int16? _AgeDays03;
		[PXDBShort()]
		public virtual Int16? AgeDays03
		{
			get
			{
				return this._AgeDays03;
			}
			set
			{
				this._AgeDays03 = value;
			}
		}
		#endregion
		#region AgeDays04
		public abstract class ageDays04 : PX.Data.IBqlField
		{
		}
		protected Int16? _AgeDays04;
		[PXDBShort()]
		public virtual Int16? AgeDays04
		{
			get
			{
				return this._AgeDays04;
			}
			set
			{
				this._AgeDays04 = value;
			}
		}
		#endregion
		#region DontPrint
		public abstract class dontPrint : PX.Data.IBqlField
		{
		}
		protected Boolean? _DontPrint;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Don't Print")]
		public virtual Boolean? DontPrint
		{
			get
			{
				return this._DontPrint;
			}
			set
			{
				this._DontPrint = value;
			}
		}
		#endregion
		#region Printed
		public abstract class printed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Printed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Printed")]
		public virtual Boolean? Printed
		{
			get
			{
				return this._Printed;
			}
			set
			{
				this._Printed = value;
			}
		}
        #endregion
        #region PrevPrintedCnt
        public abstract class prevPrintedCnt : IBqlField { }

        [PXDBShort]
        [PXDefault(TypeCode.Int16, "0")]
        [PXUIField(DisplayName = "Previously Printed Count")]
        public virtual Int16? PrevPrintedCnt { get; set; }
        #endregion
        #region DontEmail
        public abstract class dontEmail : PX.Data.IBqlField
		{
		}
		protected Boolean? _DontEmail;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Don't Email")]
		public virtual Boolean? DontEmail
		{
			get
			{
				return this._DontEmail;
			}
			set
			{
				this._DontEmail = value;
			}
		}
		#endregion
		#region Emailed
		public abstract class emailed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Emailed;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Emailed")]
		public virtual Boolean? Emailed
		{
			get
			{
				return this._Emailed;
			}
			set
			{
				this._Emailed = value;
			}
		}
		#endregion
		#region Processed
		public abstract class processed : PX.Data.IBqlField
		{
		}
		[PXBool]
		public virtual Boolean? Processed { get; set; }
		#endregion
        #region PrevEmailedCnt
        public abstract class prevEmailedCnt : IBqlField { }

        [PXDBShort]
        [PXDefault(TypeCode.Int16, "0")]
        [PXUIField(DisplayName = "Previously Emailed Count")]
        public virtual Int16? PrevEmailedCnt { get; set; }
        #endregion
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField
        {
        }
		protected Guid? _NoteID;
        [PXNote(new Type[0], DescriptionField = typeof(ARStatement.statementDate))]
		public virtual Guid? NoteID
        {
            get
            {
                return this._NoteID;
            }
            set
            {
                this._NoteID = value;
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

	public class ARStatementType
	{
		public class balanceFoward : Constant<string>
		{
			public balanceFoward() : base(StatementTypes.CS_BALANCE_BROUGHT_FORWARD) { ;}
		}
		public class openItem: Constant<string>
		{
			public openItem() : base(StatementTypes.CS_OPEN_ITEM) { ;}
		}

	}
}
