using System;
using System.Collections.Generic;
using System.Text;
using PX.Common;
using PX.Data;

namespace PX.Objects.CS
{
    public static class RoundingType
    {
        public const string Currency = "N";
        public const string Mathematical = "R";
        public const string Ceil = "C";
        public const string Floor = "F";
    }

	public class int0 : Constant<int>
	{
		public int0()
			: base((int)0)
		{
		}
	}

	public class int1 : Constant<int>
	{
		public int1()
			: base((int)1)
		{
		}
	}

	public class int2 : Constant<int>
	{
		public int2() : base(2) {}
	}

    public class int4 : Constant<int>
    {
        public int4()
            : base((int)4)
        {
        }
    }
		public class int15 : Constant<int>
		{
			public int15()
				: base(15)
			{
			}
		}
	public class short0 : Constant<short>
	{
		public short0()
			: base((short)0)
		{
		}
	}

	public class shortMinus1 : Constant<short>
	{
		public shortMinus1()
			: base((short)-1)
		{
		}
	}

	public class short1 : Constant<short>
	{
		public short1()
			: base((short)1)
		{
		}
	}

	public class short2 : Constant<short>
	{
		public short2()
			: base((short)2)
		{
		}
	}

	public class decimal0 : Constant<decimal>
	{
		public decimal0()
			: base(0m)
		{
		}
	}

	public class decimal1 : Constant<decimal>
	{
		public decimal1()
			: base(1m)
		{
		}
	}
	public class decimal_1 : Constant<decimal>
	{
		public decimal_1()
			: base(-1m)
		{
		}
	}

	public class decimal100 : Constant<decimal>
	{
		public decimal100() : base(100m) { ;}
	}

	public class decimal365 : Constant<decimal>
	{
		public decimal365() : base(365m) { ;}
	}

	public class decimalMax : Constant<decimal>
	{
		public decimalMax() : base((decimal)int.MaxValue) { ;}
	}

	public class string0 : Constant<string>
	{
		public string0()
			: base("0")
		{
		}
	}

	public class string1 : Constant<string>
	{
		public string1()
			: base("1")
		{
		}
	}

	public class stringA : Constant<string>
	{
		public stringA()
			: base("A")
		{
		}
	}

	public class stringO : Constant<string>
	{
		public stringO()
			: base("O")
		{
		}
	}

	/// <summary>
	/// This constant type is deprecated and is only preserved for 
	/// compatibility purposes. Please use <see cref="False"/> instead.
	/// </summary>
    public class boolFalse : Constant<short>
	{
		public boolFalse()
			: base(0)
		{
		}
		public override object Value
		{
			get
			{
				return false;
			}
		}
	}

	/// <summary>
	/// This constant type is deprecated and is only preserved for 
	/// compatibility purposes. Please use <see cref="True"/> instead.
	/// </summary>
	public class boolTrue : Constant<short>
	{
		public boolTrue()
			: base(1)
		{
		}
		public override object Value
		{
			get
			{
				return true;
			}
		}
	}

	public class intMax : Constant<int>
	{
		public intMax()
			: base(int.MaxValue)
		{
		}
	}



	public class int32000 : Constant<int>
	{
		public int32000()
			: base(32000)
		{
		}
	}

	public class shortMax : Constant<short>
	{
		public shortMax()
			: base(short.MaxValue)
		{
		}
	}

	public sealed class segmentValueType : Constant<string>
	{
		public segmentValueType()
			: base(typeof(SegmentValue).FullName)
		{
		}
	}

	public sealed class TimeZoneNow : IBqlCreator, IBqlOperand
	{		
		public void Parse(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, BqlCommand.Selection selection)
		{			
		}

		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			value = PXTimeZoneInfo.Now;
		}
	}

	public sealed class Quotes : Constant<string>
	{
		public Quotes() : base( "\"")
		{
		}
	}
	public sealed class OpenBracket : Constant<string>
	{
		public OpenBracket()
			: base("(")
		{
		}
	}
	public sealed class CloseBracket : Constant<string>
	{
		public CloseBracket()
			: base(")")
		{
		}
	}
}
