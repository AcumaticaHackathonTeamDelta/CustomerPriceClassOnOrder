using PX.Data;

namespace PX.Objects.CA
{
	public class PXModule
	{
		public const string AR = "AR";
		public const string AP = "AP";
		public const string CR = "CR";
		public const string PO = "PO";
		public const string SO = "SO";
		public const string RQ = "RQ";
		public const string PM = "PM";

		public class ar : Constant<string> { public ar() : base(AR) { } }
		public class ap : Constant<string> { public ap() : base(AP) { } }
		public class cr : Constant<string> { public cr() : base(CR) { } }
		public class po : Constant<string> { public po() : base(PO) { } }
		public class so : Constant<string> { public so() : base(SO) { } }
		public class rq : Constant<string> { public rq() : base(RQ) { } }
		public class pm : Constant<string> { public pm() : base(PM) { } }

		public class ar_ : Constant<string> { public ar_() : base(AR + "%") { } }
		public class ap_ : Constant<string> { public ap_() : base(AP + "%") { } }
		public class cr_ : Constant<string> { public cr_() : base(CR + "%") { } }
		public class po_ : Constant<string> { public po_() : base(PO + "%") { } }
		public class so_ : Constant<string> { public so_() : base(SO + "%") { } }
		public class rq_ : Constant<string> { public rq_() : base(RQ + "%") { } }
		public class pm_ : Constant<string> { public pm_() : base(PM + "%") { } }

		public class namespaceAR : Constant<string>
		{
			public namespaceAR() : base("PX.Objects.AR.Reports") { }
		}
		public class namespaceAP : Constant<string>
		{
			public namespaceAP() : base("PX.Objects.AP.Reports") { }
		}
		public class namespacePO : Constant<string>
		{
			public namespacePO() : base("PX.Objects.PO.Reports") { }
		}
		public class namespaceSO : Constant<string>
		{
			public namespaceSO() : base("PX.Objects.SO.Reports") { }
		}
		public class namespaceCR : Constant<string>
		{
			public namespaceCR() : base("PX.Objects.CR.Reports") { }
		}
		public class namespaceRQ : Constant<string>
		{
			public namespaceRQ() : base("PX.Objects.RQ.Reports") { }
		}
	}
}
