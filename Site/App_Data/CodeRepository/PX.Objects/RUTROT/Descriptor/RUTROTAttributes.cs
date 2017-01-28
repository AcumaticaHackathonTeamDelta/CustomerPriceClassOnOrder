using PX.Data;
using PX.Objects.GL;
using PX.Objects.AR;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PX.Objects.RUTROT
{
    public class RUTROTTypes
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(new string[] { RUT, ROT }, new string[] { RUTROTMessages.RUTType, RUTROTMessages.ROTType }) { ; }
        }
        public const string RUT = "U";
        public const string ROT = "O";

        public class rut : Constant<string>
        {
            public rut() : base(RUT) { }
        }

        public class rot : Constant<string>
        {
            public rot() : base(ROT) { }
        }
    }

    public class RUTROTItemTypes
    {
        public class ListAttribute : PXStringListAttribute
        {
            public ListAttribute()
                : base(new string[] { Service, MaterialCost, OtherCost }, new string[] { RUTROTMessages.Service, RUTROTMessages.MaterialCost, RUTROTMessages.OtherCost}) { ; }
        }
        public const string Service = "S";
        public const string MaterialCost = "M";
        public const string OtherCost = "O";

		public class service : Constant<string>
		{
			public service() : base(Service) { }
		}
		public class materialCost : Constant<string>
		{
			public materialCost() : base(MaterialCost) { }
		}
		public class otherCost : Constant<string>
		{
			public otherCost() : base(OtherCost) { }
		}
    }   
}
