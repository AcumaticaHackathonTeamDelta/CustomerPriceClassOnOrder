using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Objects.AR;
using PX.Data;

namespace PX.Objects.RUTROT
{
    [Serializable]
    public partial class ClaimRUTROTFilter : IBqlTable
    {
        #region Action
        public abstract class action : IBqlField { }

        [ClaimActions.List]
        [PXDBString(1)]
        [PXDefault(ClaimActions.Claim)]
        [PXUIField(DisplayName = "Action", Visible = true)]
        public virtual string Action { get; set; }
        #endregion

        #region DeductionType
        public abstract class rUTROTType : IBqlField { }

        [RUTROTTypes.List]
        [PXDefault(RUTROTTypes.RUT)]
        [PXDBString(1)]
        [PXUIField(DisplayName = "Deduction Type", Visible = true)]
        public string RUTROTType { get; set; }
        #endregion
    }

    public class ClaimActions
    {
        public class List : PXStringListAttribute
        {
            public List()
                : base(new string[] { Claim, Export }, new string[] { "Claim", "Export" })
            {
            }
        }

        public const string Export = "E";
        public const string Claim = "C";

        public class export : Constant<string>
        {
            public export() : base(Export) { }
        }

        public class claim : Constant<string>
        {
            public claim() : base(Claim) { }
        }
    }
}
