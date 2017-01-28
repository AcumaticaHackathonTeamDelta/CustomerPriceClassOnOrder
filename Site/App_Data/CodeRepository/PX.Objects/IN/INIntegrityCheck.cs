using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.IN.Overrides.INDocumentRelease;
using PX.Objects.CS;
using Company = PX.Objects.GL.Company;
using SOOrderType = PX.Objects.SO.SOOrderType;
using SOShipLineSplit = PX.Objects.SO.SOShipLineSplit;
using POReceiptLineSplit = PX.Objects.PO.POReceiptLineSplit;
using PX.Objects.GL;
using PX.Objects.SO;

namespace PX.Objects.IN
{
    [Serializable]
    [PXHidden]
	public partial class ReadOnlySiteStatus : INSiteStatus
	{
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SubItemID
		public new abstract class subItemID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region SiteID
		public new abstract class siteID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
	}

    [Serializable]
    [PXHidden]
	public partial class ReadOnlyLocationStatus : INLocationStatus
	{
		#region InventoryID
		public new abstract class inventoryID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SubItemID
		public new abstract class subItemID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region SiteID
		public new abstract class siteID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region LocationID
		public new abstract class locationID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public override Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
		#region QtyOnHand
		public new abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		#endregion
	}

    [Serializable]
    public partial class INItemSiteSummary : INItemSite 
    {
        public new abstract class inventoryID : IBqlField { }
        public new abstract class siteID : IBqlField { }
    }

	[PX.Objects.GL.TableAndChartDashboardType]
	public class INIntegrityCheck : PXGraph<INIntegrityCheck>
	{
		public PXCancel<INSiteFilter> Cancel;
		public PXFilter<INSiteFilter> Filter;
		[PXFilterable]
        public PXFilteredProcessingJoin<InventoryItem,
			INSiteFilter,
            LeftJoin<INSiteStatusSummary, On<INSiteStatusSummary.inventoryID, Equal<InventoryItem.inventoryID>,
				And<INSiteStatusSummary.siteID, Equal<Current<INSiteFilter.siteID>>>>>,
			Where<True, Equal<True>>,
            OrderBy<Asc<InventoryItem.inventoryCD>>>
			INItemList;
		public PXSetup<INSetup> insetup;
		public PXSelect<INItemSite> itemsite;
		public PXSelect<INSiteStatus> sitestatus_s;
		public PXSelect<SiteStatus> sitestatus;
		public PXSelect<LocationStatus> locationstatus;
		public PXSelect<LotSerialStatus> lotserialstatus;
        public PXSelect<ItemLotSerial> itemlotserial;
		public PXSelect<SiteLotSerial> sitelotserial;
		public PXSelect<INItemPlan> initemplan;

		public PXSelect<ItemSiteHist> itemsitehist;
        public PXSelect<ItemSiteHistD> itemsitehistd;
        public PXSelect<ItemCostHist> itemcosthist;
		public PXSelect<ItemSalesHistD> itemsalehistd;
		public PXSelect<ItemCustSalesStats> itemcustsalesstats;

		public INIntegrityCheck()
		{
			INSetup record = insetup.Current;

			INItemList.SetProcessCaption(Messages.Process);
			INItemList.SetProcessAllCaption(Messages.ProcessAll);

			PXDBDefaultAttribute.SetDefaultForUpdate<INTranSplit.refNbr>(this.Caches[typeof(INTranSplit)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<INTranSplit.tranDate>(this.Caches[typeof(INTranSplit)], null, false);
		}

        protected IEnumerable initemlist()
        {
            if (Filter.Current.SiteID != null)
            {
                return null;
            }
            return new List<object>();
        }

		protected virtual void INSiteFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;

            INSiteFilter filter = (INSiteFilter)e.Row;

            INItemList.SetProcessDelegate<INIntegrityCheck>(delegate(INIntegrityCheck graph, InventoryItem item)
			{
				graph.Clear(PXClearOption.PreserveTimeStamp);
                graph.IntegrityCheckProc(new INItemSiteSummary { InventoryID = item.InventoryID, SiteID = filter != null ? filter.SiteID : null }, filter != null && filter.RebuildHistory == true ? filter.FinPeriodID : null, filter.ReplanBackorders == true);
			});
			PXUIFieldAttribute.SetEnabled<INSiteFilter.finPeriodID>(sender, null, filter.RebuildHistory == true);
		}

        protected virtual void SiteStatus_NegAvailQty_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            e.NewValue = true;
            e.Cancel = true;
        }

        public virtual void IntegrityCheckProc(INItemSiteSummary itemsite, string minPeriod, bool replanBackorders)
        {
            using (PXConnectionScope cs = new PXConnectionScope())
            {
                using (PXTransactionScope ts = new PXTransactionScope())
                {
                    foreach (INItemPlan p in PXSelectReadonly2<INItemPlan, LeftJoin<Note, On<Note.noteID, Equal<INItemPlan.refNoteID>>>, Where<INItemPlan.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>, And<INItemPlan.siteID, Equal<Current<INItemSiteSummary.siteID>>, And<Note.noteID, IsNull>>>>.SelectMultiBound(this, new object[] { itemsite }))
                    {
                        PXDatabase.Delete<INItemPlan>(new PXDataFieldRestrict("PlanID", PXDbType.BigInt, 8, p.PlanID, PXComp.EQ));
                    }

                    foreach (INItemPlan p in PXSelectReadonly2<INItemPlan,
                        InnerJoin<INRegister, On<INRegister.noteID, Equal<INItemPlan.refNoteID>, And<INRegister.siteID, Equal<INItemPlan.siteID>>>>,
                        Where<INRegister.docType, Equal<INDocType.transfer>,
                            And<INRegister.released, Equal<boolTrue>,
                            And<INItemPlan.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>,
                            And<INItemPlan.siteID, Equal<Current<INItemSiteSummary.siteID>>>>>>>.SelectMultiBound(this, new object[] { itemsite }))
                    {
                        PXDatabase.Delete<INItemPlan>(new PXDataFieldRestrict("PlanID", PXDbType.BigInt, 8, p.PlanID, PXComp.EQ));
                    }

                    foreach (PXResult<INTranSplit, INRegister, INSite, INItemSite> res in PXSelectJoin<INTranSplit,
                        InnerJoin<INRegister, On<INRegister.docType, Equal<INTranSplit.docType>, And<INRegister.refNbr, Equal<INTranSplit.refNbr>>>,
                        InnerJoin<INSite, On<INSite.siteID, Equal<INRegister.toSiteID>>,
                        LeftJoin<INItemSite, On<INItemSite.inventoryID, Equal<INTranSplit.inventoryID>, And<INItemSite.siteID, Equal<INRegister.toSiteID>>>,
                        LeftJoin<INTran, On<INTran.origTranType, Equal<INTranSplit.tranType>, And<INTran.origRefNbr, Equal<INTranSplit.refNbr>, And<INTran.origLineNbr, Equal<INTranSplit.lineNbr>>>>,
                        LeftJoin<INItemPlan, On<INItemPlan.planID, Equal<INTranSplit.planID>>>>>>>,
                        Where<INRegister.docType, Equal<INDocType.transfer>,
                            And2<Where<INRegister.released, Equal<boolTrue>, And<INTranSplit.released, Equal<boolTrue>,
                                Or<INRegister.released, Equal<boolFalse>>>>,
                            And<INTranSplit.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>,
                            And<INTranSplit.siteID, Equal<Current<INItemSiteSummary.siteID>>,
                            And<INTranSplit.invtMult, Equal<shortMinus1>,
                            And<INItemPlan.planID, IsNull,
                            And<INTran.refNbr, IsNull>>>>>>>>.SelectMultiBound(this, new object[] { itemsite }))
                    {
                        INTranSplit split = res;
                        INRegister doc = res;

                        if(split.TransferType ==INTransferType.OneStep && doc.Released==true)
                        {
                            if (doc.TransferType == INTransferType.OneStep)
                            {
                                doc.TransferType = INTransferType.TwoStep;
                                Caches[typeof(INRegister)].Update(doc);
                            }
                            split.TransferType = INTransferType.TwoStep;
                        }
                        INItemPlan plan = INItemPlanIDAttribute.DefaultValues(this.Caches[typeof(INTranSplit)], res);
                        if (plan.LocationID == null)
                        {
                            plan.LocationID = ((INItemSite)res).DfltReceiptLocationID ?? ((INSite)res).ReceiptLocationID;
                        }

                        plan = (INItemPlan)this.Caches[typeof(INItemPlan)].Insert(plan);

                        split.PlanID = plan.PlanID;
                        Caches[typeof(INTranSplit)].SetStatus(split, PXEntryStatus.Updated);
                    }
                    

                    PXDatabase.Update<INSiteStatus>(
							new PXDataFieldRestrict<INSiteStatus.inventoryID>(PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
							new PXDataFieldRestrict<INSiteStatus.siteID>(PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
							new PXDataFieldAssign<INSiteStatus.qtyAvail>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyHardAvail>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyNotAvail>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyINIssues>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyINReceipts>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyInTransit>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyINAssemblySupply>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyINAssemblyDemand>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyINReplaned>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPOPrepared>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPOOrders>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPOReceipts>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySOPrepared>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySOBooked>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySOShipped>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySOShipping>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySOBackOrdered>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySOFixed>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPOFixedOrders>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPOFixedPrepared>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPOFixedReceipts>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtySODropShip>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPODropShipOrders>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPODropShipPrepared>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INSiteStatus.qtyPODropShipReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INSiteStatus.qtyInTransitToSO>(PXDbType.Decimal, 0m)
                        );

                    PXDatabase.Update<INLocationStatus>(
                            new PXDataFieldRestrict<INLocationStatus.inventoryID>(PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict<INLocationStatus.siteID>(PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldAssign<INLocationStatus.qtyAvail>(PXDbType.DirectExpression, "QtyOnHand"),
                            new PXDataFieldAssign<INLocationStatus.qtyHardAvail>(PXDbType.DirectExpression, "QtyOnHand"),
                            new PXDataFieldAssign<INLocationStatus.qtyINIssues>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyINReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyInTransit>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyINAssemblySupply>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyINAssemblyDemand>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPOPrepared>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPOOrders>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPOReceipts>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INLocationStatus.qtySOPrepared>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INLocationStatus.qtySOBooked>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtySOShipped>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtySOShipping>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtySOBackOrdered>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtySOFixed>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPOFixedOrders>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPOFixedPrepared>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPOFixedReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtySODropShip>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPODropShipOrders>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPODropShipPrepared>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyPODropShipReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLocationStatus.qtyInTransitToSO>(PXDbType.Decimal, 0m)
                        );

                    PXDatabase.Update<INLotSerialStatus>(
                            new PXDataFieldRestrict<INLotSerialStatus.inventoryID>(PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
							new PXDataFieldRestrict<INLotSerialStatus.siteID>(PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
							new PXDataFieldAssign<INLotSerialStatus.qtyAvail>(PXDbType.DirectExpression, "QtyOnHand"),
							new PXDataFieldAssign<INLotSerialStatus.qtyHardAvail>(PXDbType.DirectExpression, "QtyOnHand"),
                            new PXDataFieldAssign<INLotSerialStatus.qtyINIssues>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyINReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyInTransit>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyINAssemblySupply>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyINAssemblyDemand>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPOPrepared>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPOOrders>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPOReceipts>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INLotSerialStatus.qtySOPrepared>(PXDbType.Decimal, 0m),
							new PXDataFieldAssign<INLotSerialStatus.qtySOBooked>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtySOShipped>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtySOShipping>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtySOBackOrdered>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtySOFixed>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPOFixedOrders>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPOFixedPrepared>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPOFixedReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtySODropShip>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPODropShipOrders>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPODropShipPrepared>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyPODropShipReceipts>(PXDbType.Decimal, 0m),
                            new PXDataFieldAssign<INLotSerialStatus.qtyInTransitToSO>(PXDbType.Decimal, 0m)

                        );

                    PXDatabase.Update<INItemLotSerial>(
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldAssign("QtyAvail", PXDbType.DirectExpression, "QtyOnHand"),
                            new PXDataFieldAssign("QtyHardAvail", PXDbType.DirectExpression, "QtyOnHand"),
                            new PXDataFieldAssign("QtyINTransit", PXDbType.Decimal, 0m)
                        );

					PXDatabase.Update<INSiteLotSerial>(
							new PXDataFieldRestrict<INSiteLotSerial.inventoryID>(PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
							new PXDataFieldRestrict<INSiteLotSerial.siteID>(PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
							new PXDataFieldAssign<INSiteLotSerial.qtyAvail>(PXDbType.DirectExpression, "QtyOnHand"),
							new PXDataFieldAssign<INSiteLotSerial.qtyHardAvail>(PXDbType.DirectExpression, "QtyOnHand"),
							new PXDataFieldAssign<INSiteLotSerial.qtyInTransit>(PXDbType.Decimal, 0m)
						);


                    foreach (PXResult<ReadOnlyLocationStatus, INLocation> res in PXSelectJoinGroupBy<ReadOnlyLocationStatus, InnerJoin<INLocation, On<INLocation.locationID, Equal<ReadOnlyLocationStatus.locationID>>>, Where<ReadOnlyLocationStatus.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>, And<ReadOnlyLocationStatus.siteID, Equal<Current<INItemSiteSummary.siteID>>>>, Aggregate<GroupBy<ReadOnlyLocationStatus.inventoryID, GroupBy<ReadOnlyLocationStatus.siteID, GroupBy<ReadOnlyLocationStatus.subItemID, GroupBy<INLocation.inclQtyAvail, Sum<ReadOnlyLocationStatus.qtyOnHand>>>>>>>.SelectMultiBound(this, new object[] { itemsite }))
                    {
                        SiteStatus status = new SiteStatus();
                        status.InventoryID = ((ReadOnlyLocationStatus)res).InventoryID;
                        status.SubItemID = ((ReadOnlyLocationStatus)res).SubItemID;
                        status.SiteID = ((ReadOnlyLocationStatus)res).SiteID;
                        status = (SiteStatus)sitestatus.Cache.Insert(status);

                        if (((INLocation)res).InclQtyAvail == true)
                        {
                            status.QtyAvail += ((ReadOnlyLocationStatus)res).QtyOnHand;
                            status.QtyHardAvail += ((ReadOnlyLocationStatus)res).QtyOnHand;
                        }
                        else
                        {
                            status.QtyNotAvail += ((ReadOnlyLocationStatus)res).QtyOnHand;
                        }
                    }

                    INPlanType plan60 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan60>>>.Select(this);
					INPlanType plan61 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan61>>>.Select(this);
                    INPlanType plan70 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan70>>>.Select(this);
                    INPlanType plan74 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan74>>>.Select(this);
                    INPlanType plan76 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan76>>>.Select(this);
                    INPlanType plan42 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan42>>>.Select(this);
                    INPlanType plan44 = PXSelect<INPlanType, Where<INPlanType.planType, Equal<INPlanConstants.plan44>>>.Select(this);

                    foreach (PXResult<INItemPlan, INPlanType, SOShipLineSplit, POReceiptLineSplit> res in PXSelectJoin<INItemPlan,
                        InnerJoin<INPlanType, On<INPlanType.planType, Equal<INItemPlan.planType>>,
                        LeftJoin<SOShipLineSplit, On<SOShipLineSplit.planID, Equal<INItemPlan.planID>>,
                        LeftJoin<POReceiptLineSplit, On<POReceiptLineSplit.planID, Equal<INItemPlan.planID>>>>>,
                        Where<INItemPlan.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>, And<INItemPlan.siteID, Equal<Current<INItemSiteSummary.siteID>>>>>.SelectMultiBound(this, new object[] { itemsite }))
                    {
                        INItemPlan plan = (INItemPlan)res;
                        INPlanType plantype = (INPlanType)res;
                        INPlanType locplantype;
                        SOShipLineSplit sosplit = (SOShipLineSplit)res;
                        POReceiptLineSplit posplit = (POReceiptLineSplit)res;

                        if (plan.InventoryID != null &&
                                plan.SubItemID != null &&
                                plan.SiteID != null)
                        {
                            switch (plan.PlanType)
                            {
                                case INPlanConstants.Plan61:
								case INPlanConstants.Plan63:
									locplantype = plantype;

									if (sosplit.ShipmentNbr != null)
									{
										SOOrderType ordetype = PXSelect<SOOrderType, Where<SOOrderType.orderType, Equal<Current<SOShipLineSplit.origOrderType>>>>.SelectSingleBound(this, new object[] { sosplit });

										if (plan.OrigPlanType == null)
										{
											plan.OrigPlanType = ordetype.OrderPlanType;
										}

										if (plan.OrigPlanType == INPlanConstants.Plan60 && sosplit.IsComponentItem != true)
										{
											plantype = plantype - plan60;
										}

										if ((plan.OrigPlanType == INPlanConstants.Plan61 || plan.OrigPlanType == INPlanConstants.Plan63) && sosplit.IsComponentItem != true)
										{
											plantype = plantype - plan61;
										}
									}

                                    break;
                                case INPlanConstants.Plan71:
                                case INPlanConstants.Plan72:
									locplantype = plantype;
                                    if (posplit.ReceiptNbr == null)
                                    {
                                        PXDatabase.Delete<INItemPlan>(new PXDataFieldRestrict("PlanID", PXDbType.BigInt, 8, plan.PlanID, PXComp.EQ));
                                        continue;
                                    }
									if (posplit.PONbr != null)
                                    {
										plantype = plantype - plan70;
                                    }
                                    break;
                                case INPlanConstants.Plan77:
									locplantype = plantype;
									if (posplit.ReceiptNbr != null && posplit.PONbr != null)
                                    {
										plantype = plantype - plan76;
                                    }

                                    break;
                                case INPlanConstants.Plan75:
									locplantype = plantype;
									if (posplit.ReceiptNbr != null && posplit.PONbr != null)
									{
										plantype = plantype - plan74;
									}
									break;
                                case INPlanConstants.Plan43:
                                case INPlanConstants.Plan45:
                                    if (plan.OrigPlanType == INPlanConstants.Plan44)
                                    {
                                        plantype = plantype - plan44;
                                    }

                                    if (plan.OrigPlanType == INPlanConstants.Plan42)
                                    {
                                        plantype = plantype - plan42;
                                    }
                                    locplantype = plantype;
                                    break;

                                default:
                                    locplantype = plantype;
                                    break;
                            }

                            if (plan.LocationID != null)
                            {
                                LocationStatus item = INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<LocationStatus>(this, plan, locplantype, true);
                                INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<SiteStatus>(this, plan, plantype, (bool)item.InclQtyAvail);
                                if (!string.IsNullOrEmpty(plan.LotSerialNbr))
                                {
                                    INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<LotSerialStatus>(this, plan, locplantype, true);
                                    INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<ItemLotSerial>(this, plan, locplantype, true);
									INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<SiteLotSerial>(this, plan, locplantype, true);
                                }
                            }
                            else
                            {
                                INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<SiteStatus>(this, plan, plantype, true);
								if (!string.IsNullOrEmpty(plan.LotSerialNbr))
								{
									//TODO: check if LotSerialNbr was allocated on OrigPlanType
									INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<ItemLotSerial>(this, plan, plantype, true);
									INItemPlanIDAttribute.UpdateAllocatedQuantitiesBase<SiteLotSerial>(this, plan, plantype, true);
								}
                            }
                        }
                    }
	                if (replanBackorders)
	                {
		                INReleaseProcess.ReplanBackOrders(this);
						initemplan.Cache.Persist(PXDBOperation.Insert);
						initemplan.Cache.Persist(PXDBOperation.Update);
	                }

                    Caches[typeof(INTranSplit)].Persist(PXDBOperation.Update);

                    sitestatus.Cache.Persist(PXDBOperation.Insert);
                    sitestatus.Cache.Persist(PXDBOperation.Update);

                    locationstatus.Cache.Persist(PXDBOperation.Insert);
                    locationstatus.Cache.Persist(PXDBOperation.Update);

                    lotserialstatus.Cache.Persist(PXDBOperation.Insert);
                    lotserialstatus.Cache.Persist(PXDBOperation.Update);

                    itemlotserial.Cache.Persist(PXDBOperation.Insert);
                    itemlotserial.Cache.Persist(PXDBOperation.Update);

					sitelotserial.Cache.Persist(PXDBOperation.Insert);
					sitelotserial.Cache.Persist(PXDBOperation.Update);

                    if (minPeriod != null)
                    {
                        FinPeriod period =
                            PXSelect<FinPeriod,
                                Where<FinPeriod.finPeriodID, Equal<Required<FinPeriod.finPeriodID>>>>
                                .SelectWindowed(this, 0, 1, minPeriod);
                        if (period == null) return;
                        DateTime startDate = (DateTime)period.StartDate;

                        PXDatabase.Delete<INItemCostHist>(
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict("CostSiteID", PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldRestrict("FinPeriodID", PXDbType.Char, 6, minPeriod, PXComp.GE)
                            );

                        PXDatabase.Delete<INItemSalesHistD>(
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict("SiteID", PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldRestrict("QtyPlanSales", PXDbType.Decimal, 0m),
                            new PXDataFieldRestrict("SDate", PXDbType.DateTime, 8, startDate, PXComp.GE)

                            );
                        PXDatabase.Delete<INItemCustSalesStats>(
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict("SiteID", PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldRestrict("LastDate", PXDbType.DateTime, 8, startDate, PXComp.GE));

                        PXDatabase.Update<INItemSalesHistD>(
                            new PXDataFieldAssign("QtyIssues", PXDbType.Decimal, 0m),
                            new PXDataFieldAssign("QtyExcluded", PXDbType.Decimal, 0m),
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict("SiteID", PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldRestrict("SDate", PXDbType.DateTime, 8, startDate, PXComp.GE)
                            );

                        foreach (INLocation loc in PXSelectReadonly2<INLocation, InnerJoin<INItemCostHist, On<INItemCostHist.costSiteID, Equal<INLocation.locationID>>>, Where<INLocation.siteID, Equal<Current<INItemSiteSummary.siteID>>, And<INItemCostHist.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>>>>.SelectMultiBound(this, new object[] { itemsite }))
                        {
                            PXDatabase.Delete<INItemCostHist>(
                                new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                                new PXDataFieldRestrict("CostSiteID", PXDbType.Int, 4, loc.LocationID, PXComp.EQ),
                                new PXDataFieldRestrict("FinPeriodID", PXDbType.Char, 6, minPeriod, PXComp.GE)
                                );
                        }

                        PXDatabase.Delete<INItemSiteHist>(
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict("SiteID", PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldRestrict("FinPeriodID", PXDbType.Char, 6, minPeriod, PXComp.GE)
                            );

                        PXDatabase.Delete<INItemSiteHistD>(
                            new PXDataFieldRestrict("InventoryID", PXDbType.Int, 4, itemsite.InventoryID, PXComp.EQ),
                            new PXDataFieldRestrict("SiteID", PXDbType.Int, 4, itemsite.SiteID, PXComp.EQ),
                            new PXDataFieldRestrict("SDate", PXDbType.DateTime, 8, startDate, PXComp.GE)
                            );

                        INTran prev_tran = null;
                        foreach (PXResult<INTran, INTranSplit> res in PXSelectReadonly2<INTran, InnerJoin<INTranSplit, On<INTranSplit.tranType, Equal<INTran.tranType>, And<INTranSplit.refNbr, Equal<INTran.refNbr>, And<INTranSplit.lineNbr, Equal<INTran.lineNbr>>>>>, Where<INTran.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>, And<INTran.siteID, Equal<Current<INItemSiteSummary.siteID>>, And<INTran.finPeriodID, GreaterEqual<Required<INTran.finPeriodID>>, And<INTran.released, Equal<boolTrue>>>>>, OrderBy<Asc<INTran.tranType, Asc<INTran.refNbr, Asc<INTran.lineNbr>>>>>.SelectMultiBound(this, new object[] { itemsite }, minPeriod))
                        {
                            INTran tran = res;
                            INTranSplit split = res;

                            if (!Caches[typeof(INTran)].ObjectsEqual(prev_tran, tran))
                            {
                                INReleaseProcess.UpdateSalesHistD(this, tran);
                                INReleaseProcess.UpdateCustSalesStats(this, tran);

                                prev_tran = tran;
                            }

                            if (split.BaseQty != 0m)
                            {
                                INReleaseProcess.UpdateSiteHist(this, res, split);
                                INReleaseProcess.UpdateSiteHistD(this, split);
                            }
                        }

                        foreach (PXResult<INTran, INTranCost> res in PXSelectReadonly2<INTran, InnerJoin<INTranCost, On<INTranCost.tranType, Equal<INTran.tranType>, And<INTranCost.refNbr, Equal<INTran.refNbr>, And<INTranCost.lineNbr, Equal<INTran.lineNbr>>>>>, Where<INTran.inventoryID, Equal<Current<INItemSiteSummary.inventoryID>>, And<INTran.siteID, Equal<Current<INItemSiteSummary.siteID>>, And<INTranCost.finPeriodID, GreaterEqual<Required<INTran.finPeriodID>>, And<INTran.released, Equal<boolTrue>>>>>>.SelectMultiBound(this, new object[] { itemsite }, minPeriod))
                        {
                            INReleaseProcess.UpdateCostHist(this, (INTranCost)res, (INTran)res);
                        }

                        itemcosthist.Cache.Persist(PXDBOperation.Insert);
                        itemcosthist.Cache.Persist(PXDBOperation.Update);

                        itemsitehist.Cache.Persist(PXDBOperation.Insert);
                        itemsitehist.Cache.Persist(PXDBOperation.Update);

                        itemsitehistd.Cache.Persist(PXDBOperation.Insert);
                        itemsitehistd.Cache.Persist(PXDBOperation.Update);

                        itemsalehistd.Cache.Persist(PXDBOperation.Insert);
                        itemsalehistd.Cache.Persist(PXDBOperation.Update);

                        itemcustsalesstats.Cache.Persist(PXDBOperation.Insert);
                        itemcustsalesstats.Cache.Persist(PXDBOperation.Update);
                    }

                    ts.Complete();
                }

                sitestatus.Cache.Persisted(false);
                locationstatus.Cache.Persisted(false);
                lotserialstatus.Cache.Persisted(false);

                itemcosthist.Cache.Persisted(false);
                itemsitehist.Cache.Persisted(false);
                itemsitehistd.Cache.Persisted(false);
            }
        }
    }
}
