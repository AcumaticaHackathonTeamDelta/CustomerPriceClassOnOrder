using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.CR
{
	[Serializable]
	[PXHidden]
	public partial class CRGrams : IBqlTable
	{
		#region GramID
		public abstract class gramID : IBqlField { }

		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Gram ID", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? GramID { get; set; }
		#endregion

		#region EntityType
		public abstract class validationType : IBqlField { }

		[PXDBString(2)]
		[PXUIField(DisplayName = "Entity Type")]
		[PXDefault(ValidationTypesAttribute.LeadContact)]
		[ValidationTypes]
		public virtual String ValidationType { get; set; }
		#endregion

		#region EntityID
		public abstract class entityID : IBqlField { }

		[PXDBInt]
		[PXUIField(DisplayName = "Entity ID")]
		public virtual int? EntityID { get; set; }
		#endregion

		#region FieldName
		public abstract class fieldName : IBqlField { }

		[PXDBString(60)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Field Name", Visibility = PXUIVisibility.Visible)]
		public virtual String FieldName { get; set; }
		#endregion

		#region FieldValue
		public abstract class fieldValue : IBqlField { }

		[PXDBString(60)]
		[PXDefault("")]
		[PXUIField(DisplayName = "Field Value", Visibility = PXUIVisibility.Visible)]
		public virtual String FieldValue { get; set; }
		#endregion

		#region Score
		public abstract class score : IBqlField { }

		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "1")]
		[PXUIField(DisplayName = "Score")]
		public virtual decimal? Score { get; set; }

		#endregion
	}

	[Serializable]
    [PXHidden]
	public partial class CRGrams2 : CRGrams
	{
		public new abstract class gramID : IBqlField { }

		public new abstract class validationType : IBqlField { }

		public new abstract class entityID : IBqlField { }

		public new abstract class fieldName : IBqlField { }

		public new abstract class fieldValue : IBqlField { }
	}
}
