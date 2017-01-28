using PX.Data;
using PX.Objects.CS;
using System;
using PX.TM;
using PX.SM;

namespace PX.Objects.CR
{
	[Serializable]
	[PXCacheName(Messages.DuplicateValidationRules)]
	public partial class CRValidationRules : IBqlTable
	{
		#region ValidationType

		public abstract class validationType : IBqlField {}

		[PXDBString(2, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Validation Type")]
		[PXDefault(ValidationTypesAttribute.LeadContact)]
		[ValidationTypes]
		public virtual String ValidationType { get; set; }

		#endregion
		#region MatchingField

		public abstract class matchingField : IBqlField {}

		[PXDBString(60, IsKey = true)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Matching Field", Visibility = PXUIVisibility.Visible)]
		public virtual String MatchingField { get; set; }

		#endregion
		#region ScoreWieght

		public abstract class scoreWeight : IBqlField {}

		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "1")]
		[PXUIField(DisplayName = "Score Weight")]
		public virtual decimal? ScoreWeight { get; set; }

		#endregion
		#region TransformationRule

		public abstract class transformationRule : IBqlField { }

		[PXDBString(2)]
		[PXUIField(DisplayName = "Transformation Rule")]
		[PXDefault(TransformationRulesAttribute.None)]
		[TransformationRules]
		public virtual String TransformationRule { get; set; }

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
		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote]
		public virtual Guid? NoteID { get; set; }
		#endregion
	}

	[Serializable]
	public partial class LeadContactValidationRules : CRValidationRules
	{
		#region ValidationType
		public new abstract class validationType : IBqlField {}
		[PXDBString(2, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Validation Type")]
		[PXDefault(ValidationTypesAttribute.LeadContact)]
		public override String ValidationType { get; set; }
		#endregion
	}

	[Serializable]
	public partial class LeadAccountValidationRules : CRValidationRules
	{
		#region ValidationType
		public new abstract class validationType : IBqlField { }
		[PXDBString(2, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Validation Type")]
		[PXDefault(ValidationTypesAttribute.LeadAccount)]
		public override String ValidationType { get; set; }
		#endregion
	}

	[Serializable]
	public partial class AccountValidationRules : CRValidationRules
	{
		#region ValidationType
		public new abstract class validationType : IBqlField { }
		[PXDBString(2, IsFixed = true, IsKey = true)]
		[PXUIField(DisplayName = "Validation Type")]
		[PXDefault(ValidationTypesAttribute.Account)]
		public override String ValidationType { get; set; }
		#endregion
	}

}
