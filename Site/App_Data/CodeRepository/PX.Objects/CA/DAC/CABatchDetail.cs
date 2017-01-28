using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CR;
using PX.TM;



namespace PX.Objects.CA
{
	[PXCacheName(Messages.CABatchDetail)]
    [Serializable]
	public partial class CABatchDetail : IBqlTable
	{
		#region BatchNbr
		public abstract class batchNbr : PX.Data.IBqlField
		{
		}
		protected String _BatchNbr;
		[PXDBString(15, IsKey = true, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
		[PXDBDefault(typeof(CABatch.batchNbr))]		
		[PXParent(typeof(Select<CABatch,Where<CABatch.batchNbr,Equal<Current<CABatchDetail.batchNbr>>>>))]
		
		public virtual String BatchNbr
		{
			get
			{
				return this._BatchNbr;
			}
			set
			{
				this._BatchNbr = value;
			}
		}
		#endregion
		#region OrigModule
		public abstract class origModule : PX.Data.IBqlField
		{
		}
		protected String _OrigModule;
		[PXDBString(2, IsFixed = true,IsKey=true)]
		[PXDefault(GL.BatchModule.AP)]
		[PXStringList(new string[] { GL.BatchModule.AP, GL.BatchModule.AR }, new string[] { "AP", "AR" })]
		[PXUIField(DisplayName = "Module", Enabled = false)]
		public virtual String OrigModule
		{
			get
			{
				return this._OrigModule;
			}
			set
			{
				this._OrigModule = value;
			}
		}
		#endregion
		#region OrigDocType
		public abstract class origDocType : PX.Data.IBqlField
		{
		}
		protected String _OrigDocType;
		[PXDBString(3, IsFixed = true,IsKey=true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Doc.Type")]		
		public virtual String OrigDocType
		{
			get
			{
				return this._OrigDocType;
			}
			set
			{
				this._OrigDocType = value;
			}
		}
		#endregion
		#region OrigRefNbr
		public abstract class origRefNbr : PX.Data.IBqlField
		{
		}
		protected String _OrigRefNbr;
		[PXDBString(15, IsUnicode = true,IsKey=true)]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]		
		[PXUIField(DisplayName = "Reference Nbr.")]
		public virtual String OrigRefNbr
		{
			get
			{
				return this._OrigRefNbr;
			}
			set
			{
				this._OrigRefNbr = value;
			}
		}
		#endregion

		public virtual void Copy(AP.APPayment payment) 
		{
			this.OrigRefNbr = payment.RefNbr;
			this.OrigDocType = payment.DocType;
			this.OrigModule = GL.BatchModule.AP;
		}
		public virtual void Copy(AR.ARPayment payment)
		{
			this.OrigRefNbr = payment.RefNbr;
			this.OrigDocType = payment.DocType;
			this.OrigModule = GL.BatchModule.AR;
		}
	
	}
}
