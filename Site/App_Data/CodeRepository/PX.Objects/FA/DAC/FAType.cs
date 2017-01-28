using System;
using PX.Data;

namespace PX.Objects.FA
{
	[Serializable]
	[PXPrimaryGraph(typeof(AssetTypeMaint))]
	[PXCacheName(Messages.FAType)]
	public partial class FAType : IBqlTable
	{
		#region AssetTypeID
		public abstract class assetTypeID : PX.Data.IBqlField
		{
		}
		protected string _AssetTypeID;
		[PXDBString(10, IsUnicode = true, IsKey = true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Asset Type ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<FAType.assetTypeID>))]
		public virtual string AssetTypeID
		{
			get
			{
				return _AssetTypeID;
			}
			set
			{
				_AssetTypeID = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : IBqlField
		{
		}
		protected string _Description;
		[PXDBString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual string Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}
		#endregion	
		#region IsTangible
		public abstract class isTangible : IBqlField
		{
		}
		protected bool? _IsTangible;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Tangible", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual bool? IsTangible
		{
			get
			{
				return _IsTangible;
			}
			set
			{
				_IsTangible = value;
			}
		}
		#endregion
		#region Depreciable
		public abstract class depreciable : IBqlField { }
		protected bool? _Depreciable;
		[PXDBBool()]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Depreciate", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual bool? Depreciable
		{
			get
			{
				return _Depreciable;
			}
			set
			{
				_Depreciable = value;
			}
		}
		#endregion
	}
}
