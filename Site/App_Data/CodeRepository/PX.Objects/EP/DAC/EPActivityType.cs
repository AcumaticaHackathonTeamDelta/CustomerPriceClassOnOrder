using System;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CR;
using PX.SM;

namespace PX.Objects.EP
{
	[PXCacheName(Messages.ActivityType)]
	[PXPrimaryGraph(typeof(CRActivitySetupMaint))]
	[Serializable]
	public partial class EPActivityType : IBqlTable, PX.Data.EP.ActivityService.IActivityType
	{
		#region Type

		public abstract class type : IBqlField { }

		[PXDBString(5, IsFixed = true, IsKey = true, InputMask = ">AAAAA")]
		[PXDefault]
		[PXUIField(DisplayName = "Type ID")]
		public virtual String Type { get; set; }

		#endregion

		#region Description
		public abstract class description : IBqlField {}
		[PXDBString(64, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		[PXFieldDescription]
		[PXDefault]
		public virtual string Description { get; set; }
		#endregion

		#region Active
		public abstract class active : IBqlField { }
		[PXDBBool]
		[PXDefault(true)]
		[PXUIField(DisplayName = "Active")]
		public virtual bool? Active { get; set; }
		#endregion

		#region IsDefault
		public abstract class isDefault : IBqlField {}
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "System Default")]
		public virtual bool? IsDefault { get; set; }
		#endregion

		#region ImageUrl

		public abstract class imageUrl : IBqlField { }

		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Image")]
		[PXIconsList]
		public virtual String ImageUrl { get; set; }

		#endregion

		#region Application
		public abstract class application : IBqlField { }
		[PXInt]
		[PXUIField(DisplayName = "Application")]
		[PXActivityApplication]
		[PXDefault(PXActivityApplicationAttribute.Backend)]
		[PXDBCalced(typeof(Switch<Case<Where<EPActivityType.isInternal, Equal<True>>, PXActivityApplicationAttribute.backend>, PXActivityApplicationAttribute.portal>), typeof(int))]
		public virtual int? Application { get; set; }
		#endregion

		#region IsInternal
		public abstract class isInternal : IBqlField { }
		[PXDBBool]				
		[PXDefault(true)]
		[PXFormula(typeof(Switch<Case<Where<EPActivityType.application, Equal<PXActivityApplicationAttribute.backend>>, True>,False>)) ]
		public virtual bool? IsInternal { get; set; }
		#endregion

		#region PrivateByDefault
		public abstract class privateByDefault : IBqlField { }
		[PXDBBool]		
		[PXFormula(typeof(Switch<Case<Where<EPActivityType.application, Equal<PXActivityApplicationAttribute.portal>>, False>, EPActivityType.privateByDefault>))]
		[PXUIEnabled(typeof(EPActivityType.isInternal))]
		[PXUIField(DisplayName = "Internal")]
		public virtual Boolean? PrivateByDefault { get; set; }
		#endregion

		#region RequireTimeByDefault


		public abstract class requireTimeByDefault : IBqlField { }
		[PXDefault(false,PersistingCheck = PXPersistingCheck.Nothing)]
		[PXDBBool]
		[PXUIField(DisplayName = "Track Time")]
		[PXFormula(typeof(Switch<Case<Where<EPActivityType.application, Equal<PXActivityApplicationAttribute.portal>>, False>, EPActivityType.requireTimeByDefault>))]
		[PXUIEnabled(typeof(EPActivityType.isInternal))]
		public virtual Boolean? RequireTimeByDefault { get; set; }

		#endregion

		#region Incoming
		public abstract class incoming : IBqlField { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Incoming")]
		public virtual bool? Incoming { get; set; }
		#endregion

		#region Outgoing
		public abstract class outgoing : IBqlField { }
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Outgoing")]
		public virtual bool? Outgoing { get; set; }
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
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTimeUtc]
		[PXUIField(DisplayName = "Last Activity", Enabled = false)]
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
