namespace PX.Objects.SO
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using PX.Data;
	using PX.Objects.CS;
	using PX.Objects.IN;
	using PX.Objects.AR;
	using PX.Objects.CM;
	using POReceipt = PX.Objects.PO.POReceipt;
	using POReceiptLine = PX.Objects.PO.POReceiptLine;
	using POLineType = PX.Objects.PO.POLineType;


	[Obsolete()]
	public class SOInvoiceOrder : PXGraph<SOInvoiceOrder>
	{
	}

	[PX.Objects.GL.TableAndChartDashboardType]
	public class SOInvoiceShipment : PXGraph<SOInvoiceShipment>
	{
		public PXCancel<SOShipmentFilter> Cancel;
		public PXAction<SOShipmentFilter> viewDocument;
		public PXFilter<SOShipmentFilter> Filter;
		[PXFilterable]
		public PXFilteredProcessing<SOShipment, SOShipmentFilter> Orders;
		public PXSelect<SOShipLine> dummy_select_to_bind_events;
		public PXSetup<SOSetup> sosetup;

		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXEditDetailButton]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			if (Orders.Current != null)
			{
				if (Orders.Current.ShipmentType == INDocType.DropShip)
				{
					PO.POReceiptEntry docgraph = PXGraph.CreateInstance<PO.POReceiptEntry>();
					docgraph.Document.Current = docgraph.Document.Search<POReceipt.receiptNbr>(Orders.Current.ShipmentNbr);

					throw new PXRedirectRequiredException(docgraph, true, PO.Messages.POReceipt) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
				else
				{
					SOShipmentEntry docgraph = PXGraph.CreateInstance<SOShipmentEntry>();
					docgraph.Document.Current = docgraph.Document.Search<SOShipment.shipmentNbr>(Orders.Current.ShipmentNbr);

					throw new PXRedirectRequiredException(docgraph, true, Messages.SOShipment) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
				}
			}
			return adapter.Get();
		}

        public PXSelect<INSite> INSites;
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = Messages.SiteDescr, Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void INSite_Descr_CacheAttached(PXCache sender)
        {
        }

        public PXSelect<Carrier> Carriers;
        [PXMergeAttributes(Method = MergeMethod.Merge)]
        [PXUIField(DisplayName = Messages.CarrierDescr, Visibility = PXUIVisibility.SelectorVisible)]
        protected virtual void Carrier_Description_CacheAttached(PXCache sender)
        {
        }

        public SOInvoiceShipment()
		{
			Orders.SetSelected<SOShipment.selected>();
            object item = sosetup.Current;
		}

		public virtual void SOShipmentFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;

            SOShipmentFilter filter = e.Row as SOShipmentFilter;
			if (filter != null && !String.IsNullOrEmpty(filter.Action))
			{
				Dictionary<string, object> parameters = Filter.Cache.ToDictionary(filter);
				Orders.SetProcessTarget(null, null, null, filter.Action, parameters);
			}
			string actionID = (string)Orders.GetTargetFill(null, null, null, filter.Action, "@actionID");
			int action = 0;
			int.TryParse(actionID, out action);
			
			PXUIFieldAttribute.SetEnabled<SOShipmentFilter.invoiceDate>(sender, filter, action == SOShipmentEntryActionsAttribute.CreateInvoice || action == SOShipmentEntryActionsAttribute.CreateDropshipInvoice);
			PXUIFieldAttribute.SetEnabled<SOShipmentFilter.packagingType>(sender, filter, action != SOShipmentEntryActionsAttribute.CreateDropshipInvoice);
			PXUIFieldAttribute.SetVisible<SOShipmentFilter.showPrinted>(sender, filter, action == SOShipmentEntryActionsAttribute.PrintLabels || action == SOShipmentEntryActionsAttribute.PrintPickList);
			PXUIFieldAttribute.SetDisplayName<SOShipment.shipmentNbr>(Orders.Cache, action == SOShipmentEntryActionsAttribute.CreateDropshipInvoice ? Messages.GetLocal(POReceipt.receiptNbr.DisplayName) : Messages.GetLocal(SOShipment.shipmentNbr.DisplayName));
			PXUIFieldAttribute.SetDisplayName<SOShipment.shipDate>(Orders.Cache, action == SOShipmentEntryActionsAttribute.CreateDropshipInvoice ? Messages.GetLocal(POReceipt.receiptDate.DisplayName) : Messages.GetLocal(SOShipment.shipDate.DisplayName));


			if (sosetup.Current.UseShipDateForInvoiceDate == true)
			{
				sender.RaiseExceptionHandling<SOShipmentFilter.invoiceDate>(filter, null, new PXSetPropertyException(Messages.UseInvoiceDateFromShipmentDateWarning, PXErrorLevel.Warning));
				PXUIFieldAttribute.SetEnabled<SOShipmentFilter.invoiceDate>(sender, filter, false);
			}
		}

		protected bool _ActionChanged = false;

		public virtual void SOShipmentFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			_ActionChanged = !sender.ObjectsEqual<SOShipmentFilter.action>(e.Row, e.OldRow);
			if (_ActionChanged)
				((SOShipmentFilter)e.Row).PackagingType = SOShipmentFilter.Both;
		}
				
		public virtual IEnumerable orders()
		{
			PXUIFieldAttribute.SetDisplayName<SOShipment.customerID>(Caches[typeof(SOShipment)], Messages.CustomerID);

			SOShipmentFilter filter = Filter.Current;
			if (filter.Action == "<SELECT>")
			{
				yield break;
			}

			string actionID = (string)Orders.GetTargetFill(null, null, null, filter.Action, "@actionID");
			int action = 0;
			int.TryParse(actionID, out action);

			if (_ActionChanged)
			{
				Orders.Cache.Clear();
			}

			PXSelectBase cmd;
			
			switch (action)
			{
				case SOShipmentEntryActionsAttribute.CreateInvoice:
					cmd = new PXSelectJoinGroupBy<SOShipment,
						InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>, And<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>>>,
						InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
						InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrderShipment.orderType>>,
						InnerJoin<SOOrderTypeOperation, 
							On<SOOrderTypeOperation.orderType, Equal<SOOrderShipment.orderType>,
							And<SOOrderTypeOperation.operation, Equal<SOOrderShipment.operation>>>,
						InnerJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>,
						InnerJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>,
						LeftJoin<Carrier, On<SOShipment.shipVia, Equal<Carrier.carrierID>>>>>>>>>,
						Where<SOShipment.confirmed, Equal<boolTrue>,
						And<SOOrderType.aRDocType, NotEqual<ARDocType.noUpdate>,
						And<SOOrderShipment.invoiceNbr, IsNull,
                        And2<Match<Customer, Current<AccessInfo.userName>>,
						And<Match<INSite, Current<AccessInfo.userName>>>>>>>,
						Aggregate<GroupBy<SOShipment.shipmentNbr,
						GroupBy<SOShipment.createdByID,
						GroupBy<SOShipment.lastModifiedByID,
						GroupBy<SOShipment.confirmed,
						GroupBy<SOShipment.ownerID,
						GroupBy<SOShipment.released,
						GroupBy<SOShipment.hold,
						GroupBy<SOShipment.resedential,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.groundCollect,
						GroupBy<SOShipment.insurance,
						GroupBy<SOShipment.shippedViaCarrier,
						GroupBy<SOShipment.labelsPrinted>>>>>>>>>>>>>>>(this);
					break;
				case SOShipmentEntryActionsAttribute.PostInvoiceToIN:
					cmd = new PXSelectJoinGroupBy<SOShipment, 
					LeftJoin<Carrier, On<SOShipment.shipVia, Equal<Carrier.carrierID>>,
					InnerJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>,
					InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>, And<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>>>,
					InnerJoin<SOShipLine, On<SOShipLine.shipmentType, Equal<SOOrderShipment.shipmentType>, And<SOShipLine.shipmentNbr, Equal<SOOrderShipment.shipmentNbr>, And<SOShipLine.origOrderType, Equal<SOOrderShipment.orderType>, And<SOShipLine.origOrderNbr, Equal<SOOrderShipment.orderNbr>>>>>,
					InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOShipLine.origOrderType>, And<SOOrder.orderNbr, Equal<SOShipLine.origOrderNbr>>>,
					LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>,
					InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrderShipment.orderType>>>>>>>>>,
					Where<SOShipment.confirmed, Equal<boolTrue>, 
						And<SOOrderShipment.invtRefNbr, IsNull,
						And<SOShipLine.lineType, Equal<SOLineType.inventory>,
						And<SOOrderType.iNDocType, NotEqual<INTranType.noUpdate>,
                        And2<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>,
						And<Match<INSite, Current<AccessInfo.userName>>>>>>>>,
					Aggregate<GroupBy<SOShipment.shipmentNbr,
						GroupBy<SOShipment.createdByID,
						GroupBy<SOShipment.lastModifiedByID,
						GroupBy<SOShipment.confirmed,
						GroupBy<SOShipment.ownerID,
						GroupBy<SOShipment.released,
						GroupBy<SOShipment.hold,
						GroupBy<SOShipment.resedential,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.groundCollect,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.shippedViaCarrier,
						GroupBy<SOShipment.labelsPrinted>>>>>>>>>>>>>>>(this);
					break;
				case SOShipmentEntryActionsAttribute.CreateDropshipInvoice:
					cmd = new PXSelectJoinGroupBy<POReceipt,
					InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentNbr, Equal<POReceipt.receiptNbr>, And<SOOrderShipment.shipmentType, Equal<SOShipmentType.dropShip>>>,
					InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
					InnerJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrderShipment.orderType>>,
					InnerJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>,
					InnerJoin<SOOrderTypeOperation, 
							On<SOOrderTypeOperation.orderType, Equal<SOOrderShipment.orderType>,
							And<SOOrderTypeOperation.operation, Equal<SOOrderShipment.operation>>>>>>>>,
					Where<POReceipt.released, Equal<boolTrue>,
						//And2<Where<POReceiptLine.lineType, Equal<POLineType.goodsForDropShip>, Or<POReceiptLine.lineType, Equal<POLineType.nonStockForDropShip>>>,
						And<SOOrderType.aRDocType, NotEqual<ARDocType.noUpdate>,
						And<SOOrderShipment.invoiceNbr, IsNull,
                        And<Match<Customer, Current<AccessInfo.userName>>>>>>,
					Aggregate<GroupBy<POReceipt.receiptNbr,
						GroupBy<POReceipt.createdByID,
						GroupBy<POReceipt.lastModifiedByID,
						GroupBy<POReceipt.released,
						GroupBy<POReceipt.ownerID,
						GroupBy<POReceipt.hold>>>>>>>>(this);
					break;
				case SOShipmentEntryActionsAttribute.CancelReturn:
					cmd = new PXSelectJoinGroupBy<SOShipment, InnerJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>,
						LeftJoin<Carrier, On<SOShipment.shipVia, Equal<Carrier.carrierID>>,
						InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>, And<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>>>,
						InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
						InnerJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>>>>,
					Where<SOShipment.labelsPrinted, Equal<False>,
                    And2<Match<Customer, Current<AccessInfo.userName>>,
                    And<Match<INSite, Current<AccessInfo.userName>>>>>,
					Aggregate<GroupBy<SOShipment.shipmentNbr,
						GroupBy<SOShipment.createdByID,
						GroupBy<SOShipment.lastModifiedByID,
						GroupBy<SOShipment.confirmed,
						GroupBy<SOShipment.ownerID,
						GroupBy<SOShipment.released,
						GroupBy<SOShipment.hold,
						GroupBy<SOShipment.resedential,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.groundCollect,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.shippedViaCarrier,
						GroupBy<SOShipment.labelsPrinted>>>>>>>>>>>>>>>(this);
					break;
				default:
					cmd = new PXSelectJoinGroupBy<SOShipment,
						LeftJoin<Carrier, On<SOShipment.shipVia, Equal<Carrier.carrierID>>,
						InnerJoin<INSite, On<INSite.siteID, Equal<SOShipment.siteID>>,
						InnerJoin<SOOrderShipment, On<SOOrderShipment.shipmentType, Equal<SOShipment.shipmentType>, And<SOOrderShipment.shipmentNbr, Equal<SOShipment.shipmentNbr>>>,
						InnerJoin<SOOrder, On<SOOrder.orderType, Equal<SOOrderShipment.orderType>, And<SOOrder.orderNbr, Equal<SOOrderShipment.orderNbr>>>,
						LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>>>>>>>,
					Where2<Match<INSite, Current<AccessInfo.userName>>,
					And<Where<Customer.bAccountID, IsNull, Or<Match<Customer, Current<AccessInfo.userName>>>>>>,
					Aggregate<GroupBy<SOShipment.shipmentNbr,
						GroupBy<SOShipment.createdByID,
						GroupBy<SOShipment.lastModifiedByID,
						GroupBy<SOShipment.confirmed,
						GroupBy<SOShipment.ownerID,
						GroupBy<SOShipment.released,
						GroupBy<SOShipment.hold,
						GroupBy<SOShipment.resedential,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.groundCollect,
						GroupBy<SOShipment.saturdayDelivery,
						GroupBy<SOShipment.shippedViaCarrier,
						GroupBy<SOShipment.labelsPrinted>>>>>>>>>>>>>>>(this);
					break;
			}

			if (typeof(PXSelectBase<SOShipment>).IsAssignableFrom(cmd.GetType()))
			{
				((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.shipDate, LessEqual<Current<SOShipmentFilter.endDate>>>>();

				if (filter.SiteID != null)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.siteID, Equal<Current<SOShipmentFilter.siteID>>>>();
				}

				if (!string.IsNullOrEmpty(filter.CarrierPluginID))
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<Carrier.carrierPluginID, Equal<Current<SOShipmentFilter.carrierPluginID>>>>();
				}

				if (!string.IsNullOrEmpty(filter.ShipVia))
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.shipVia, Equal<Current<SOShipmentFilter.shipVia>>>>();
				}

				if (filter.StartDate != null)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.shipDate, GreaterEqual<Current<SOShipmentFilter.startDate>>>>();
				}

				if (filter.CustomerID != null)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.customerID, Equal<Current<SOShipmentFilter.customerID>>>>();
				}

				if ( action == SOShipmentEntryActionsAttribute.PrintLabels && filter.ShowPrinted == false)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.labelsPrinted, Equal<False>>>();
				}

				if (action == SOShipmentEntryActionsAttribute.PrintPickList && filter.ShowPrinted == false)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOShipment.pickListPrinted, Equal<False>>>();
				}
								
				if (filter.PackagingType == SOShipmentFilter.Manual)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOOrder.isManualPackage, Equal<True>>>();
				}
				else if (filter.PackagingType == SOShipmentFilter.Auto)
				{
					((PXSelectBase<SOShipment>)cmd).WhereAnd<Where<SOOrder.isManualPackage, NotEqual<True>>>();
				}

				int startRow = PXView.StartRow;
				int totalRows = 0;

				List<PXFilterRow> newFilters = new List<PXFilterRow>();
				foreach (PXFilterRow f in PXView.Filters)
				{
					if (string.Compare(f.DataField, "CustomerOrderNbr", StringComparison.OrdinalIgnoreCase) == 0)
					{
						f.DataField = "SOOrder__CustomerOrderNbr";
					}
					newFilters.Add(f);
				}

				foreach (object res in ((PXSelectBase<SOShipment>)cmd).View.Select(null, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, newFilters.ToArray(), ref startRow, PXView.MaximumRows, ref totalRows))
				{
					SOShipment order = PXResult.Unwrap<SOShipment>(res);
					SOOrder so = PXResult.Unwrap<SOOrder>(res);

					if (order.BilledOrderCntr + order.UnbilledOrderCntr + order.ReleasedOrderCntr == 1)
					{
						order.CustomerOrderNbr = so.CustomerOrderNbr;
					}

					SOShipment cached = (SOShipment)Orders.Cache.Locate(order);
					if (cached != null)
						order.Selected = cached.Selected;
					yield return order;
				}
				PXView.StartRow = 0;
			}

			if (typeof(PXSelectBase<POReceipt>).IsAssignableFrom(cmd.GetType()))
			{
				((PXSelectBase<POReceipt>)cmd).WhereAnd<Where<POReceipt.receiptDate, LessEqual<Current<SOShipmentFilter.endDate>>>>();

				/*
				if (filter.SiteID != null)
				{
					((PXSelectBase<POReceipt>)cmd).WhereAnd<Where<POReceipt.siteID, Equal<Current<SOShipmentFilter.siteID>>>>();
				}

				if (!string.IsNullOrEmpty(filter.ShipVia))
				{
					((PXSelectBase<POReceipt>)cmd).WhereAnd<Where<POReceipt.shipVia, Equal<Current<SOShipmentFilter.shipVia>>>>();
				}
				*/

				if (filter.StartDate != null)
				{
					((PXSelectBase<POReceipt>)cmd).WhereAnd<Where<POReceipt.receiptDate, GreaterEqual<Current<SOShipmentFilter.startDate>>>>();
				}

				foreach (PXResult<POReceipt, SOOrderShipment, SOOrder> res in ((PXSelectBase<POReceipt>)cmd).Select())
				{
					SOShipment order = res;
					SOShipment cached = (SOShipment)Orders.Cache.Locate(order);

					if (cached == null)
						Orders.Cache.SetStatus(order, PXEntryStatus.Held);
					else 
						order.Selected = cached.Selected;
					yield return order;
				}
			}
			Orders.Cache.IsDirty = false;
		}
	}

    [Serializable]
	public partial class SOShipmentFilter : IBqlTable
	{
		#region Action
		public abstract class action : PX.Data.IBqlField
		{
		}
		protected string _Action;
		[PXAutomationMenu]
		public virtual string Action
		{
			get
			{
				return this._Action;
			}
			set
			{
				this._Action = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[IN.Site(DisplayName = "Warehouse", DescriptionField = typeof(INSite.descr))]
		public virtual Int32? SiteID
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
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Start Date", Visibility = PXUIVisibility.SelectorVisible, Required = false)]
		[PXDefault()]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "End Date", Visibility = PXUIVisibility.SelectorVisible)]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				this._EndDate = value;
			}
		}
		#endregion
		#region CarrierPluginID
		public abstract class carrierPluginID : PX.Data.IBqlField
		{
		}
		protected String _CarrierPluginID;
		[PXDBString(15, IsUnicode = true, InputMask = ">aaaaaaaaaaaaaaa")]
		[PXUIField(DisplayName = "Carrier", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<CarrierPlugin.carrierPluginID>))]
		public virtual String CarrierPluginID
		{
			get
			{
				return this._CarrierPluginID;
			}
			set
			{
				this._CarrierPluginID = value;
			}
		}
		#endregion
		#region ShipVia
		public abstract class shipVia : PX.Data.IBqlField
		{
		}
		protected String _ShipVia;
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Ship Via")]
		[PXSelector(typeof(Search<Carrier.carrierID>), DescriptionField = typeof(Carrier.description), CacheGlobal = true)]
		public virtual String ShipVia
		{
			get
			{
				return this._ShipVia;
			}
			set
			{
				this._ShipVia = value;
			}
		}
		#endregion
		#region CustomerID
		public abstract class customerID : IBqlField
		{
		}
		protected int? _CustomerID;
		[PXUIField(DisplayName = "Customer")]
		[Customer(DescriptionField = typeof(Customer.acctName))]
		public virtual int? CustomerID
		{
			get
			{
				return _CustomerID;
			}
			set
			{
				_CustomerID = value;
			}
		}
		#endregion
		#region InvoiceDate
		public abstract class invoiceDate : IBqlField
		{
		}
		protected DateTime? _InvoiceDate;
		[PXDBDate]
		[PXUIField(DisplayName = "Invoice Date")]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? InvoiceDate
		{
			get
			{
				return _InvoiceDate;
			}
			set
			{
				_InvoiceDate = value;
			}
		}
		#endregion
		#region PackagingType
		public abstract class packagingType : PX.Data.IBqlField
		{
		}

		public const string Auto = "A";
		public const string Manual = "M";
		public const string Both = "B";
		protected String _PackagingType;
		[PXDBString(1, IsFixed = true)]
		[PXDefault("B")]
		[PXStringList(
				new string[] { Both, Auto, Manual},
				new string[] { Messages.PackagingType_Both, Messages.PackagingType_Auto, Messages.PackagingType_Manual })]
		[PXUIField(DisplayName = "Packaging Type")]
		public virtual String PackagingType
		{
			get
			{
				return this._PackagingType;
			}
			set
			{
				this._PackagingType = value;
			}
		}
		#endregion
		#region ShowPrinted
		public abstract class showPrinted : PX.Data.IBqlField
		{
		}
		protected bool? _ShowPrinted;
		[PXBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Show Printed")]
		public virtual bool? ShowPrinted
		{
			get
			{
				return this._ShowPrinted;
			}
			set
			{
				this._ShowPrinted = value;
			}
		}
		#endregion

	}
}
