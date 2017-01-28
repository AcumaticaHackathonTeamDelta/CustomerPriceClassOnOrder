using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.RQ
{
	[System.SerializableAttribute()]	
	public partial class RQRequisitionOrder : PX.Data.IBqlTable
	{		
		#region ReqNbr
		public abstract class reqNbr : PX.Data.IBqlField
		{
		}
		protected String _ReqNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault(typeof(RQRequisition.reqNbr))]
		[PXParent(typeof(Select<RQRequisition, Where<RQRequisition.reqNbr, Equal<Current<RQRequisitionOrder.reqNbr>>>>))]
		public virtual String ReqNbr
		{
			get
			{
				return this._ReqNbr;
			}
			set
			{
				this._ReqNbr = value;
			}
		}
		#endregion
		#region OrderCategory
		public abstract class orderCategory : PX.Data.IBqlField
		{
		}
		protected String _OrderCategory;
		[PXDBString(2, IsKey = true, IsFixed = true, InputMask = ">aa")]
		[PXDefault()]
		public virtual String OrderCategory
		{
			get
			{
				return this._OrderCategory;
			}
			set
			{
				this._OrderCategory = value;
			}
		}
		#endregion
		#region OrderType
		public abstract class orderType : PX.Data.IBqlField
		{
		}
		protected String _OrderType;
		[PXDBString(2, IsKey = true, IsFixed = true, InputMask = ">aa")]
		[PXDefault()]		
		public virtual String OrderType
		{
			get
			{
				return this._OrderType;
			}
			set
			{
				this._OrderType = value;
			}
		}
		#endregion
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		protected String _OrderNbr;
		[PXDBString(15, IsUnicode = true, IsKey = true, InputMask = "")]
		[PXDefault()]				
		public virtual String OrderNbr
		{
			get
			{
				return this._OrderNbr;
			}
			set
			{
				this._OrderNbr = value;
			}
		}
		#endregion
	}

	public class RQOrderCategory
	{
		public const string PO = "PO";
		public const string SO = "SO";

		public class po : Constant<string> { public po() : base(PO) { } }
		public class so : Constant<string> { public so() : base(SO) { } }
	}
}
