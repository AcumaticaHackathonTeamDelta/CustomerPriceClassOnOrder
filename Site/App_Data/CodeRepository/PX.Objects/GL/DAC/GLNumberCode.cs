namespace PX.Objects.GL
{
	using System;
	using PX.Data;
	using PX.Data.EP;
	using System.Collections;
	using System.Collections.Generic;

	[System.SerializableAttribute()]
	[PXCacheName(Messages.GLNumberCode)]
	public partial class GLNumberCode : PX.Data.IBqlTable
	{
		#region NumberCode
		public abstract class numberCode : PX.Data.IBqlField
		{
		}
		protected String _NumberCode;
		[PXDBString(5, IsUnicode = true, IsKey = true, InputMask = ">aaaaa")]
		[PXDefault()]
		[PXUIField(DisplayName = "Numbering Code", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String NumberCode
		{
			get
			{
				return this._NumberCode;
			}
			set
			{
				this._NumberCode = value;
			}
		}
		#endregion
		#region NumberingID
		public abstract class numberingID : IBqlField { }
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Numbering ID")]
		[PXSelector(typeof(Search<CS.Numbering.numberingID>))]
		public string NumberingID
		{
			get;
			set;
		}
		#endregion
	}
}
