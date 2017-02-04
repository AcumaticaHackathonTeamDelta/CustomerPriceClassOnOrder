using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.SO;

namespace PX.Objects.DA
{

    [PXTable(typeof(SOOrder.orderType), typeof(SOOrder.orderNbr), IsOptional = true)]
    public class SOOrderDAExtension : PXCacheExtension<SOOrder>
    {
        #region DAPriceClassID
        public abstract class dAPriceClassID : PX.Data.IBqlField
        {
        }
        protected String _DAPriceClassID;
        [PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault(typeof(Search<LocationExtAddress.cPriceClassID, 
            Where<LocationExtAddress.locationBAccountID, Equal<Current<SOOrder.customerID>>>>), 
            PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Customer Price Class", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<ARPriceClass.priceClassID>))]
        [PXFormula(typeof(Default<SOOrder.customerID>))]
        public virtual String DAPriceClassID
        {
            get
            {
                return this._DAPriceClassID;
            }
            set
            {
                this._DAPriceClassID = value;
            }
        }
        #endregion
    }
}
