using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.EP;
using PX.TM;

namespace PX.Objects.PO
{
	[System.SerializableAttribute()]
	public partial class POPrintOrderFilter : IBqlTable
	{
		#region CurrentOwnerID
		public abstract class currentOwnerID : PX.Data.IBqlField
		{
		}

		[PXDBGuid]
		[CR.CRCurrentOwnerID]
		public virtual Guid? CurrentOwnerID { get; set; }
		#endregion
		#region OwnerID
		public abstract class ownerID : PX.Data.IBqlField
		{
		}
		protected Guid? _OwnerID;
		[PXDBGuid]
		[PXUIField(DisplayName = "Assigned To")]
		[PX.TM.PXSubordinateOwnerSelector]
		public virtual Guid? OwnerID
		{
			get
			{
				return (_MyOwner == true) ? CurrentOwnerID : _OwnerID;
			}
			set
			{
				_OwnerID = value;
			}
		}
		#endregion
		#region MyOwner
		public abstract class myOwner : PX.Data.IBqlField
		{
		}
		protected Boolean? _MyOwner;
		[PXDBBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Me")]
		public virtual Boolean? MyOwner
		{
			get
			{
				return _MyOwner;
			}
			set
			{
				_MyOwner = value;
			}
		}
		#endregion
		#region WorkGroupID
		public abstract class workGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _WorkGroupID;
		[PXDBInt]
		[PXUIField(DisplayName = "Workgroup")]
		[PXSelector(typeof(Search<EPCompanyTree.workGroupID,
			Where<EPCompanyTree.workGroupID, Owned<Current<AccessInfo.userID>>>>),
		 SubstituteKey = typeof(EPCompanyTree.description))]
		public virtual Int32? WorkGroupID
		{
			get
			{
				return (_MyWorkGroup == true) ? null : _WorkGroupID;
			}
			set
			{
				_WorkGroupID = value;
			}
		}
		#endregion
		#region MyWorkGroup
		public abstract class myWorkGroup : PX.Data.IBqlField
		{
		}
		protected Boolean? _MyWorkGroup;
		[PXDefault(false)]
		[PXDBBool]
		[PXUIField(DisplayName = "My", Visibility = PXUIVisibility.Visible)]
		public virtual Boolean? MyWorkGroup
		{
			get
			{
				return _MyWorkGroup;
			}
			set
			{
				_MyWorkGroup = value;
			}
		}
		#endregion
		#region FilterSet
		public abstract class filterSet : PX.Data.IBqlField
		{
		}
		[PXDefault(false)]
		[PXDBBool]
        public virtual Boolean? FilterSet
		{
			get
			{
				return
					this.OwnerID != null ||
					this.WorkGroupID != null ||
					this.MyWorkGroup == true;
			}
		}
		#endregion
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
	}

	public class actionEmpty : Constant<string>
	{
		public actionEmpty():base("<SELECT>"){}
		public override object  Value
		{
			get 
			{ 
				 return "<SELECT>";
			}
		}
	}

	[PX.Objects.GL.TableAndChartDashboardType]
	public class POPrintOrder : PXGraph<POPrintOrder>
	{
		public PXFilter<POPrintOrderFilter> Filter;

        public PXSelect<Vendor> vendors;
        public PXSelect<EPEmployee> employees;

        [Serializable]
		[PXProjection(typeof(Select5<POOrder, 
			LeftJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<POOrder.employeeID>>>,			
			Where2<
						Where<CurrentValue<POPrintOrderFilter.ownerID>, IsNull,
						   Or<CurrentValue<POPrintOrderFilter.ownerID>, Equal<EPEmployee.userID>>>,
					And2<
						Where<CurrentValue<POPrintOrderFilter.workGroupID>, IsNull,
						   Or<CurrentValue<POPrintOrderFilter.workGroupID>, Equal<POOrder.ownerWorkgroupID>>>,
					And2<
						Where<CurrentValue<POPrintOrderFilter.myWorkGroup>, Equal<CS.boolFalse>,
							 Or<POOrder.ownerWorkgroupID, InMember<CurrentValue<POPrintOrderFilter.currentOwnerID>>>>,
					And<
						Where<POOrder.ownerWorkgroupID, IsNull,
							 Or<POOrder.ownerWorkgroupID, Owned<CurrentValue<POPrintOrderFilter.currentOwnerID>>>>>>>>,
					Aggregate<GroupBy<POOrder.orderNbr, 			              
			              GroupBy<POOrder.hold,
										GroupBy<POOrder.approved,
										GroupBy<POOrder.emailed,
										GroupBy<POOrder.dontEmail,
										GroupBy<POOrder.cancelled,
										GroupBy<POOrder.receipt,
										GroupBy<POOrder.isOpenTaxValid,
										GroupBy<POOrder.isTaxValid,
										GroupBy<POOrder.ownerWorkgroupID,
										GroupBy<POOrder.createdByID,
										GroupBy<POOrder.lastModifiedByID,
										GroupBy<POOrder.dontPrint,
										GroupBy<POOrder.noteID,
										GroupBy<POOrder.printed>>>>>>>>>>>>>>>>>))]
		public partial class POPrintOrderOwned : POOrder
		{			
		}

		public PXCancel<POPrintOrderFilter> Cancel;
		public PXAction<POPrintOrderFilter> details;
		[PXFilterable]
		public PXFilteredProcessingJoin<POPrintOrderOwned, POPrintOrderFilter,
			LeftJoin<Vendor, On<Vendor.bAccountID, Equal<POPrintOrderOwned.vendorID>>,
 	        LeftJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<POPrintOrderOwned.employeeID>>>>,
			Where<Current<POPrintOrderFilter.action>, NotEqual<actionEmpty>>> Records;

		public PXSetup<EPSetup> EPSetup;

		public POPrintOrder()
		{
			PXUIFieldAttribute.SetRequired<POPrintOrderOwned.orderDate>(this.Records.Cache, false);
			PXUIFieldAttribute.SetRequired<POPrintOrderOwned.curyID>(this.Records.Cache, false);
			PXUIFieldAttribute.SetEnabled(Records.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<POPrintOrderOwned.selected>(Records.Cache, null, true);

			Records.SetSelected<POPrintOrderOwned.selected>();
			Records.SetProcessCaption(IN.Messages.Process);
			Records.SetProcessAllCaption(IN.Messages.ProcessAll);
			//Records.SetProcessDelegate(PrintOrders);			
			this.Records.Cache.AllowInsert = false;
			this.Records.Cache.AllowDelete = false;			
		}

		public virtual IEnumerable records(PXAdapter adapter)
			{
			Type where = PX.TM.OwnedFilter.ProjectionAttribute.ComposeWhere(
				typeof(POPrintOrderFilter),
				typeof(POPrintOrderOwned.workgroupID),
				typeof(POPrintOrderOwned.ownerID));

			Type printWhere =
				typeof(Where<POPrintOrderOwned.hold, Equal<False>,
								 And<POPrintOrderOwned.dontPrint, Equal<False>,
								 And<POPrintOrderOwned.printed, NotEqual<True>>>>);
			Type emailWhere =
				typeof(Where<POPrintOrderOwned.hold, Equal<False>,
								 And<POPrintOrderOwned.dontEmail, Equal<False>,
								 And<POPrintOrderOwned.emailed, NotEqual<True>>>>);
			
			Type action =
				Filter.Current.Action == "<SELECT>"
				? typeof(Where<CS.boolTrue, Equal<CS.boolFalse>>)
				: Filter.Current.Action.Contains("Email")
				? emailWhere :
				printWhere;

			Type select =
				BqlCommand.Compose(
					typeof(Select2<,,>), typeof(POPrintOrderOwned),
					typeof(InnerJoin<Vendor, On<Vendor.bAccountID, Equal<POPrintOrderOwned.vendorID>>, LeftJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<POPrintOrderOwned.employeeID>>>>),
					typeof(Where<>), action);

			PXView view = new PXView(this, false, BqlCommand.CreateInstance(select));
			return view.SelectMulti();
		}
			
		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXEditDetailButton]
		public virtual IEnumerable Details(PXAdapter adapter)
		{
			if (Records.Current != null && Filter.Current != null)
			{
				POOrderEntry graph = PXGraph.CreateInstance<POOrderEntry>();
				graph.Document.Current = Records.Current;
				throw new PXRedirectRequiredException(graph, true, Messages.ViewPOOrder) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
			}
			return adapter.Get();
		}

		protected virtual void POPrintOrderFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			POPrintOrderFilter o = (POPrintOrderFilter)e.Row;
			PXUIFieldAttribute.SetEnabled<POPrintOrderFilter.ownerID>(sender, o, o == null || o.MyOwner == false);
			PXUIFieldAttribute.SetEnabled<POPrintOrderFilter.workGroupID>(sender, o, o == null || o.MyWorkGroup == false);

			if (o != null && !String.IsNullOrEmpty(o.Action))
			{
				Records.SetProcessTarget(null,null, null, o.Action);
			}
		}
		
		protected virtual void POPrintOrderOwned_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			sender.IsDirty = false;
		}
	}
}
