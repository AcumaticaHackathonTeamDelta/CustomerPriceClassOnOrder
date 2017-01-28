using System;
using PX.Data;


namespace PX.Objects.BQLConstants
{
	#region Misc
	public class BitOn : Constant<int>
	{
		public BitOn() : base((int)1) { }

		public override object Value
		{
			get
			{
				return true;
			}
		}
	}

	public class BitOff : Constant<int>
	{
		public BitOff() : base((int)0) {}

		public override object Value
		{
			get
			{
				return false;
			}
		}
	}
	public class EmptyString: Constant<string> 
	{
		public EmptyString() : base(string.Empty) { }
	}
	#endregion
	#region Batch Module
	public class moduleGL : Constant<string>
	{
		public moduleGL() : base("GL") { }
	}

	public class moduleAP : Constant<string>
	{
		public moduleAP()
			: base("AP")
		{
		}
	}
	public class moduleAR : Constant<string>
	{
		public moduleAR()
			: base("AR")
		{
		}
	}
	#endregion
	#region Batch Status
	public class statusU : Constant<string>
	{
		public statusU()
			: base("U")
		{
		}
	}

	public class statusB : Constant<string>
	{
		public statusB()
			: base("B")
		{
		}
	}

	public class statusP : Constant<string>
	{
		public statusP()
			: base("P")
		{
		}
	}

	public class statusC : Constant<string>
	{
		public statusC()
			: base("C")
		{
		}
	}

	public class statusH : Constant<string>
	{
		public statusH()
			: base("H")
		{
		}
	}
	public class statusS : Constant<string>
	{
		public statusS()
			: base("S")
		{
		}
	}
	#endregion
	#region Business Account Types
	public class accountVendor : Constant<string>
	{
		public accountVendor()
			: base("VE")
		{
		}
	}
	public class accountCustomer : Constant<string>
	{
		public accountCustomer()
			: base("CU")
		{
		}
	}
	public class accountVendorCustomer : Constant<string>
	{
		public accountVendorCustomer()
			: base("VC")
		{
		}
	}
	public class accountUnknown : Constant<string>
	{
		public accountUnknown()
			: base("UN")
		{
		}
	}
	#endregion
}
