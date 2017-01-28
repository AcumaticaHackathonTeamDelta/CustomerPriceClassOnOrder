namespace PX.Objects.IN
{
	using System;
	using PX.Data;
	
	[System.SerializableAttribute()]
	[PXPrimaryGraph(typeof(INLotSerClassMaint))]
	[PXCacheName(Messages.LotSerClass)]
	public partial class INLotSerClass : PX.Data.IBqlTable, ILotSerNumVal
	{
		private const string DfltLotSerialClass = "DEFAULT";
		public class dfltLotSerialClass : Constant<string>
		{
			public dfltLotSerialClass()
				: base(DfltLotSerialClass)
			{
			}
		}

		public static string GetDefaultLotSerClass(PXGraph graph)
		{
			INLotSerClass lotSerClass = PXSelect<INLotSerClass, Where<INLotSerClass.lotSerTrack, Equal<INLotSerTrack.notNumbered>>>.Select(graph);
			if (lotSerClass == null)
			{
				PXCache cache = graph.Caches<INLotSerClass>();
				INLotSerClass lotser = (INLotSerClass) cache.CreateInstance();
				lotser.LotSerClassID = DfltLotSerialClass;
				lotser.LotSerTrack = INLotSerTrack.NotNumbered;
				cache.Insert(lotser);
				return lotser.LotSerClassID;
			}
			else
			{
				return lotSerClass.LotSerClassID;
			}
		}

		#region LotSerClassID
		public abstract class lotSerClassID : PX.Data.IBqlField
		{
		}
		protected String _LotSerClassID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName="Class ID", Visibility=PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INLotSerClass.lotSerClassID>))]
		[PX.Data.EP.PXFieldDescription]
		public virtual String LotSerClassID
		{
			get
			{
				return this._LotSerClassID;
			}
			set
			{
				this._LotSerClassID = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PX.Data.EP.PXFieldDescription]
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
		#region LotSerTrack
		public abstract class lotSerTrack : PX.Data.IBqlField
		{
		}
		protected String _LotSerTrack;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INLotSerTrack.NotNumbered)]
		[PXUIField(DisplayName = "Tracking Method", Visibility = PXUIVisibility.SelectorVisible)]
		[INLotSerTrack.List()]
		public virtual String LotSerTrack
		{
			get
			{
				return this._LotSerTrack;
			}
			set
			{
				this._LotSerTrack = value;
			}
		}
		#endregion
		#region LotSerAssign
		public abstract class lotSerAssign : PX.Data.IBqlField
		{
		}
		protected String _LotSerAssign;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INLotSerAssign.WhenReceived)]
		[PXUIField(DisplayName = "Assignment Method", Visibility = PXUIVisibility.SelectorVisible)]
		[INLotSerAssign.List()]
		public virtual String LotSerAssign
		{
			get
			{
				return this._LotSerAssign;
			}
			set
			{
				this._LotSerAssign = value;
			}
		}
		#endregion
		#region LotSerIssueMethod
		public abstract class lotSerIssueMethod : PX.Data.IBqlField
		{
		}
		protected String _LotSerIssueMethod;
		[PXDBString(1, IsFixed = true)]
		[PXDefault(INLotSerIssueMethod.FIFO)]
		[PXUIField(DisplayName = "Issue Method", Visibility = PXUIVisibility.SelectorVisible)]
		[INLotSerIssueMethod.List()]
		public virtual String LotSerIssueMethod
		{
			get
			{
				return this._LotSerIssueMethod;
			}
			set
			{
				this._LotSerIssueMethod = value;
			}
		}
		#endregion
		#region LotSerNumShared
		public abstract class lotSerNumShared : PX.Data.IBqlField
		{
		}
		protected Boolean? _LotSerNumShared;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Share Auto-Incremental Value Between All Class Items")]
		public virtual Boolean? LotSerNumShared
		{
			get
			{
				return this._LotSerNumShared;
			}
			set
			{
				this._LotSerNumShared = value;
			}
		}
		#endregion
		#region LotSerNumVal
		public abstract class lotSerNumVal : PX.Data.IBqlField
		{
		}
		protected String _LotSerNumVal;
		[PXDBString(30, InputMask = "999999999999999999999999999999")]
		[PXUIField(DisplayName="Auto-Incremental Value")]
		public virtual String LotSerNumVal
		{
			get
			{
				return this._LotSerNumVal;
			}
			set
			{
				this._LotSerNumVal = value;
			}
		}
		#endregion
		#region LotSerFormatStr
		public abstract class lotSerFormatStr : PX.Data.IBqlField
		{
		}
		protected String _LotSerFormatStr;
		[PXDBString(60)]
		public virtual String LotSerFormatStr
		{
			get
			{
				return this._LotSerFormatStr;
			}
			set
			{
				this._LotSerFormatStr = value;
			}
		}
		#endregion
		#region LotSerTrackExpiration
		public abstract class lotSerTrackExpiration : PX.Data.IBqlField
		{
		}
		protected Boolean? _LotSerTrackExpiration;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Track Expiration Date")]
		public virtual Boolean? LotSerTrackExpiration
		{
			get
			{
				return this._LotSerTrackExpiration;
			}
			set
			{
				this._LotSerTrackExpiration = value;
			}
		}
		#endregion
		#region AutoNextNbr
		public abstract class autoNextNbr : IBqlField
		{
		}
		protected Boolean? _AutoNextNbr;
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Auto-Generate Next Number")]
		public virtual Boolean? AutoNextNbr
		{
			get
			{
				return _AutoNextNbr;
			}
			set
			{
				_AutoNextNbr = value;
			}
		}
		#endregion
		#region AutoSerialMaxCount
		public abstract class autoSerialMaxCount : PX.Data.IBqlField
		{
		}
		protected int? _AutoSerialMaxCount;
		[PXDBInt()]
		[PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Max. Auto-Generate Numbers")]
		public virtual int? AutoSerialMaxCount
		{
			get
			{
				return this._AutoSerialMaxCount;
			}
			set
			{
				this._AutoSerialMaxCount = value;
			}
		}
		#endregion
		#region RequiredForDropship
		public abstract class requiredForDropship : IBqlField
		{
		}
		protected Boolean? _RequiredForDropship;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Required for Drop-ship")]
		public virtual Boolean? RequiredForDropship
		{
			get
			{
				return _RequiredForDropship;
			}
			set
			{
				_RequiredForDropship = value;
			}
		}
		#endregion

		/// <summary>
		/// Lot/Serial number is not assigned automatically and requires user interaction.
		/// </summary>
		public bool IsUnassigned
		{
			get
			{
				if (string.IsNullOrEmpty(_LotSerTrack) || _LotSerTrack.Equals(INLotSerTrack.NotNumbered, StringComparison.InvariantCultureIgnoreCase))
					return false;

				return   _LotSerAssign == INLotSerAssign.WhenReceived && LotSerIssueMethod == INLotSerIssueMethod.UserEnterable ||
				       _LotSerAssign == INLotSerAssign.WhenUsed && _AutoNextNbr != true;
			}
		}

		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote(DescriptionField = typeof(INLotSerClass.lotSerClassID),
			Selector = typeof(INLotSerClass.lotSerClassID))]
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

	public class INLotSerAssign
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { WhenReceived, WhenUsed },
					new string[] { Messages.WhenReceived, Messages.WhenUsed }) { ; }
		}

		public const string WhenReceived = "R";
		public const string WhenUsed = "U";

		public class whenReceived : Constant<string>
		{
			public whenReceived() : base(WhenReceived) { ;}
		}

		public class whenUsed : Constant<string>
		{
			public whenUsed() : base(WhenUsed) { ;}
		}
	}

	public class INLotSerTrack
	{
		[Flags]
		public enum Mode
		{
			None   = 0,
			Create = 1,
			Issue  = 2,
			Manual = 4
		}

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { NotNumbered, LotNumbered, SerialNumbered },
					new string[] { Messages.NotNumbered, Messages.LotNumbered, Messages.SerialNumbered }) { ; }
		}

		public const string NotNumbered = "N";
		public const string LotNumbered = "L";
		public const string SerialNumbered = "S";

		public class notNumbered : Constant<string>
		{
			public notNumbered() : base(NotNumbered) { ;}
		}

		public class lotNumbered : Constant<string>
		{
			public lotNumbered() : base(LotNumbered) { ;}
		}

		public class serialNumbered : Constant<string>
		{
			public serialNumbered() : base(SerialNumbered) { ;}
		}				
	}
	
	public class INLotSerIssueMethod
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
					new string[] { FIFO, LIFO, Sequential, Expiration, UserEnterable },
					new string[] { Messages.FIFO, Messages.LIFO, Messages.Sequential, Messages.Expiration, Messages.UserEnterable }) { ; }
		}

		public const string FIFO = "F";
		public const string LIFO = "L";
		public const string Sequential = "S";
		public const string Expiration = "E";
		public const string UserEnterable = "U";

		public class fIFO : Constant<string>
		{
			public fIFO() : base(FIFO) { ;}
		}

		public class lIFO : Constant<string>
		{
			public lIFO() : base(LIFO) { ;}
		}

		public class sequential : Constant<string>
		{
			public sequential() : base(Sequential) { ;}
		}

		public class expiration : Constant<string>
		{
			public expiration() : base(Expiration) { ;}
		}

		public class userEnterable : Constant<string>
		{
			public userEnterable() : base(UserEnterable) { ;}
		}
	}
}
