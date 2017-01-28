

namespace PX.Objects.CA
{
	using System;
	using PX.Data;
	using PX.Objects.AR;

	[PXCacheName("Customer Processing Center ID")]
	[System.SerializableAttribute()]
	public partial class CustomerProcessingCenterID : PX.Data.IBqlTable
	{
		//"InstanceID"
		#region InstanceID
		public abstract class instanceID : PX.Data.IBqlField
		{
		}
		protected Int32? _InstanceID;
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? InstanceID
		{
			get
			{
				return this._InstanceID;
			}
			set
			{
				this._InstanceID = value;
			}
		}
		#endregion
		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[Customer(DescriptionField = typeof(Customer.acctName))]
		[PXParent(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<CustomerProcessingCenterID.bAccountID>>>>))]
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
		#region CCProcessingCenterID
		public abstract class cCProcessingCenterID : PX.Data.IBqlField
		{
		}
		protected String _CCProcessingCenterID;
		[PXDBString(10, IsUnicode = true)]
		[PXDefault()]
		[PXParent(typeof(Select<CCProcessingCenter, Where<CCProcessingCenter.processingCenterID, Equal<Current<CustomerProcessingCenterID.cCProcessingCenterID>>>>))]
		[PXUIField(DisplayName = "Proc. Center ID")]
		public virtual String CCProcessingCenterID
		{
			get
			{
				return this._CCProcessingCenterID;
			}
			set
			{
				this._CCProcessingCenterID = value;
			}
		}
		#endregion
		#region CustomerCCPID
		public abstract class customerCCPID : PX.Data.IBqlField
		{
		}
		[PXDBString(1024, IsUnicode = true)]
		[PXDBDefault()]
		[PXUIField(DisplayName = "Customer CCPID")]
		public virtual string CustomerCCPID { get; set; }
		#endregion
	}
}
