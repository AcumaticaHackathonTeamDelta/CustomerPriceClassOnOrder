namespace PX.Objects.AR
{
	using System;
	using PX.Data;

	/// <summary>
	/// Header of Dunning Letter
	/// </summary>
	[System.SerializableAttribute()]
    [PXPrimaryGraph(typeof(ARDunningLetterUpdate))] //for notification
    [PXEMailSource]
	[PXCacheName(Messages.DunningLetter)]
    public partial class ARDunningLetter : PX.Data.IBqlTable
	{
		#region DunningLetterID
		public abstract class dunningLetterID : PX.Data.IBqlField
		{
		}
		protected Int32? _DunningLetterID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Dunning Letter ID", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(ARDunningLetter.dunningLetterID), 
			new Type[]
			{
				typeof(ARDunningLetter.dunningLetterID),
				typeof(ARDunningLetter.branchID_Branch_branchCD),
				typeof(ARDunningLetter.bAccountID_Customer_acctCD),
				typeof(ARDunningLetter.dunningLetterDate),
				typeof(ARDunningLetter.dunningLetterLevel),
				typeof(ARDunningLetter.deadline)
			})]
		public virtual Int32? DunningLetterID
		{
			get
			{
				return this._DunningLetterID;
			}
			set
			{
				this._DunningLetterID = value;
			}
		}
		#endregion
		#region BranchID
        public abstract class branchID : PX.Data.IBqlField
		{
		}
		public abstract class branchID_Branch_branchCD : PX.Data.IBqlField
		{
		}
        protected Int32? _BranchID;
		[PXDBInt()]
		[PXDefault()]
        [PXUIField(DisplayName = "Branch")]
		[PXSelector(typeof(Search<GL.Branch.branchID, Where<GL.Branch.branchID, Equal<Current<ARDunningLetter.branchID>>>>), DescriptionField = typeof(GL.Branch.branchCD), ValidateValue = false)]
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
        #region BAccountID
        public abstract class bAccountID : PX.Data.IBqlField
        {
        }
		public abstract class bAccountID_Customer_acctCD : PX.Data.IBqlField
		{
		}
        protected Int32? _BAccountID;
        [PXDBInt()]
        [PXDefault()]
		[PXUIField(DisplayName = "Customer", IsReadOnly=true)]
		[PXSelector(typeof(Search<Customer.bAccountID, Where<Customer.bAccountID, Equal<Current<ARDunningLetter.bAccountID>>>>), DescriptionField = typeof(Customer.acctCD), ValidateValue = false)]
		public virtual Int32? BAccountID
        {
            get
            {
                return this._BAccountID;
            }
            set
            {
                this._BAccountID = value;
            }
        }
        #endregion
        #region DunningLetterDate
		public abstract class dunningLetterDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DunningLetterDate;
		[PXDBDate()]
		[PXDefault(TypeCode.DateTime, "01/01/1900")]
		[PXUIField(DisplayName = "Dunning Letter Date", IsReadOnly=true)]
		public virtual DateTime? DunningLetterDate
		{
			get
			{
				return this._DunningLetterDate;
			}
			set
			{
				this._DunningLetterDate = value;
			}
		}
		#endregion
        #region Deadline
        public abstract class deadline : PX.Data.IBqlField
        {
        }
        protected DateTime? _Deadline;
        [PXDBDate()]
        [PXDefault(TypeCode.DateTime, "01/01/1900")]
		[PXUIField(DisplayName = "Deadline")]
        public virtual DateTime? Deadline
        {
            get
            {
                return this._Deadline;
            }
            set
            {
                this._Deadline = value;
            }
        }
        #endregion
        #region DunningLetterLevel
        public abstract class dunningLetterLevel : PX.Data.IBqlField
		{
		}
		protected Int32? _DunningLetterLevel;
		[PXDBInt()]
		[PXDefault()]
        [PXUIField(DisplayName = Messages.DunningLetterLevel, IsReadOnly = true)]
        public virtual Int32? DunningLetterLevel
		{
			get
			{
                return this._DunningLetterLevel;
			}
			set
			{
                this._DunningLetterLevel = value;
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
        #region Released
        public abstract class released : PX.Data.IBqlField
        {
        }
        protected Boolean? _Released;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? Released
        {
            get
            {
                return this._Released;
            }
            set
            {
                this._Released = value;
            }
        }
        #endregion
        #region Voided
        public abstract class voided : PX.Data.IBqlField
        {
        }
        protected Boolean? _Voided;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? Voided
        {
            get
            {
                return this._Voided;
            }
            set
            {
                this._Voided = value;
            }
        }
        #endregion
        #region Status
        public abstract class status : PX.Data.IBqlField
        {
        }
        [PXString(1, IsFixed = true)]
        [PXDefault("D")]
        [PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXStringList(new string[] { "D", "R", "V" },
                new string[] { Messages.Draft, Messages.Released, Messages.Voided })]
        public virtual String Status
        {
            get
            {
                if (Voided == true)
                    return "V";
                if (Released == true)
                    return "R";
                return "D";
            }
            set { }
        }
        #endregion
        #region DetailsCount
        public abstract class detailsCount : PX.Data.IBqlField
        {
        }
        protected Int32? _DetailsCount;
        [PXInt]
        [PXUIField(DisplayName="Number of Documents")]
        public virtual Int32? DetailsCount
        {
            get
            {
                return this._DetailsCount;
            }
            set
            {
                this._DetailsCount = value;
            }
        }
        #endregion
        #region FeeDocType
        public abstract class feeDocType : PX.Data.IBqlField
        {
        }
        protected String _FeeDocType;
        [PXDBString(3, IsFixed = true)]
        [ARDocType.List()]
        [PXUIField(DisplayName = "Fee Type")]
        public virtual String FeeDocType
        {
            get
            {
                return this._FeeDocType;
            }
            set
            {
                this._FeeDocType = value;
            }
        }
        #endregion
        #region FeeRefNbr
        public abstract class feeRefNbr : PX.Data.IBqlField
        {
        }
        protected String _FeeRefNbr;
        [PXDBString(15, IsUnicode = true)]
        [PXSelector(typeof(Search<ARInvoice.refNbr, Where<ARInvoice.docType, Equal<Current<ARDunningLetter.feeDocType>>, And<ARInvoice.refNbr, Equal<Current<ARDunningLetter.feeRefNbr>>>>>),ValidateValue=false)]
        [PXUIField(DisplayName = "Fee Reference Nbr.", Enabled=false)]
        public virtual String FeeRefNbr
        {
            get
            {
                return this._FeeRefNbr;
            }
            set
            {
                this._FeeRefNbr = value;
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
        #region NoteID
        public abstract class noteID : PX.Data.IBqlField
        {
        }
		protected Guid? _NoteID;
        [PXNote(new Type[0])]
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

        #region Consolidated
        public abstract class consolidated : PX.Data.IBqlField
        {
        }
        protected Boolean? _Consolidated;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? Consolidated
        {
            get
            {
                return this._Consolidated;
            }
            set
            {
                this._Consolidated = value;
            }
        }
        #endregion

        #region LastLevel
        public abstract class lastLevel : PX.Data.IBqlField
        {
        }
        protected Boolean? _LastLevel;
        [PXDBBool()]
        [PXDefault(false)]
        public virtual Boolean? LastLevel
        {
            get
            {
                return this._LastLevel;
            }
            set
            {
                this._LastLevel = value;
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
    }

}
