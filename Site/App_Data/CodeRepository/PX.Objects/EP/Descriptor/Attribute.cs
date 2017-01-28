using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Compilation;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.TM;
using PX.Objects.TX;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.SM;
using PX.Objects.PM;
using PX.Objects.AP;

namespace PX.TM
{
	#region PXOwnerSelectorAttribute

	using Objects.EP;
	/// <summary>
	/// Allows show employees for specified work group.
	/// </summary>
	/// <example>
	/// [PXOwnerSelector(typeof(MyDac.myField)]
	/// </example>
	public class PXOwnerSelectorAttribute : PXAggregateAttribute
	{
        #region SelectorAttribute

	    public class OwnerSubstituteSelectorAttribute : PXSelectorAttribute
	    {
	        public OwnerSubstituteSelectorAttribute(Type type, params Type[] fieldList)
	            : base(type, fieldList)
	        {	            
	        }

	        public override void SubstituteKeyFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
	        {
	            Guid value;
	            if (e.NewValue != null && !GUID.TryParse(e.NewValue.ToString(), out value))
	            {	                
	                base.SubstituteKeyFieldUpdating(sender, e);
	            }
	        }

	        public override void SubstituteKeyFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
	        {
	            base.SubstituteKeyFieldSelecting(sender,e);
	        }





		    public override void DescriptionFieldSelecting(PXCache sender, PXFieldSelectingEventArgs e, string alias)
		    {
					var origValue = sender.GetValue(e.Row, _FieldName);
			    base.DescriptionFieldSelecting(sender, e, alias);										
					if (origValue != null && e.ReturnValue == null)
					{
						using (var del = new PXReadDeletedScope())
						{
							Users user = PXSelect<Users, Where<Users.pKID, Equal<Required<Users.pKID>>>>.SelectSingleBound(sender.Graph, new object[] { }, origValue);							
							e.ReturnValue = (user != null)
								? user.DisplayName
								: origValue;
							
							e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(string), false, true, null, null, null, null, alias
								, null, null, PXLocalizer.Localize(Messages.OwnerNotFound, typeof(Messages).FullName), PXErrorLevel.Warning , false, null, null, PXUIVisibility.Visible, null, null, null);							
						}
					}			    
		    }
	    }
        #endregion
		#region State
		protected readonly int _SelAttrIndex;
		protected Type _workgroupType;
		#endregion

		#region DAC
		[PXProjection(typeof(Select<CREmployee,Where<CREmployee.userID, IsNotNull>>), Persistent = false)]
		[CRCacheIndependentPrimaryGraph(
			typeof(EmployeeMaint),
			typeof(Select<PX.Objects.EP.EPEmployee,
				Where<PX.Objects.EP.EPEmployee.bAccountID, Equal<Current<EPEmployee.bAccountID>>>>))]
		[CRCacheIndependentPrimaryGraph(
			typeof(AccessUsers),
			typeof(Select<Users,
				Where<Current<EPEmployee.bAccountID>, IsNull, And<Users.pKID, Equal<Current<EPEmployee.pKID>>>>>))]
        [CRCacheIndependentPrimaryGraph(
			typeof(EmployeeMaint),
			typeof(Where<Current<EPEmployee.bAccountID>, IsNull>))]
		[PXHidden]
		public class EPEmployee : IBqlTable
		{
			#region PKID
			public abstract class pKID : PX.Data.IBqlField
			{
			}
			protected Guid? _PKID;
			[PXDBGuidMaintainDeleted(BqlField = typeof(CREmployee.userID))]
			[PXDefault]
			[PXUIField(Visibility = PXUIVisibility.Invisible)]
			public virtual Guid? PKID
			{
				get
				{
					return this._PKID;
				}
				set
				{
					this._PKID = value;
				}
			}
			#endregion
			#region BAccountID
			public abstract class bAccountID : PX.Data.IBqlField
			{
			}
			protected Int32? _BAccountID;
			[PXDBIdentity(BqlTable = typeof(CREmployee))]
			[PXUIField(Visible = false, Visibility = PXUIVisibility.Invisible)]
			public virtual Int32? BAccountID
			{
				get
				{
					return this._BAccountID;
				}
				set
				{
					this._BAccountID = value;
				}
			}
			#endregion
			#region AcctCD
			public abstract class acctCD : PX.Data.IBqlField
			{
			}
			protected String _AcctCD;
			[PXDBString(30, IsUnicode = true, InputMask = "", BqlTable = typeof(CREmployee), IsKey = true)]
			[PXUIField(DisplayName = "Employee ID", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String AcctCD
			{
				get
				{
					return this._AcctCD;
				}
				set
				{
					this._AcctCD = value;
				}
			}
			#endregion
			#region AcctName
			public abstract class acctName : PX.Data.IBqlField
			{
			}
			protected String _AcctName;
			[PXDBString(60, IsUnicode = true, BqlTable = typeof(CREmployee))]
			[PXUIField(DisplayName = "Employee Name")]
			public virtual String AcctName
			{
				get
				{
					return this._AcctName;
				}
				set
				{
					this._AcctName = value;
				}
			}
			#endregion						
			#region DepartmentID
			public abstract class departmentID : PX.Data.IBqlField
			{
			}
			protected String _DepartmentID;
			[PXDBString(30, IsUnicode = true, BqlTable = typeof(CREmployee))]
			[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
			[PXUIField(DisplayName = "Department", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String DepartmentID
			{
				get
				{
					return this._DepartmentID;
				}
				set
				{
					this._DepartmentID = value;
				}
			}
			#endregion						
			#region Status
			public abstract class status : PX.Data.IBqlField{}
			protected String _Status;
			[PXDBString(1, IsFixed = true, BqlField = typeof(CREmployee.status))]
			[PXUIField(DisplayName = "Status")]
			[BAccount.status.List()]
			[PXDefault(BAccount.status.Active)]
			public virtual String Status
			{
				get
				{
					return this._Status;
				}
				set
				{
					this._Status = value;
				}
			}
			#endregion			
			#region DefContactID
			public abstract class defContactID : IBqlField { }
            [PXDBInt(BqlField = typeof(CREmployee.defContactID))]
			[PXUIField(DisplayName = "Default Contact")]
			[PXSelector(typeof(Search<Contact.contactID, Where<Contact.bAccountID, Equal<Current<EPEmployee.parentBAccountID>>>>))]
			public virtual Int32? DefContactID
			{
				get;
				set;
			}
			#endregion            
			#region ParentBAccountID
			public abstract class parentBAccountID : IBqlField { }
            [PXDBInt(BqlField = typeof(CREmployee.parentBAccountID))]
			[PXUIField(DisplayName = "Branch")]
			public virtual Int32? ParentBAccountID
			{
				get;
				set;
			}
			#endregion
		}
		#endregion

		public PXOwnerSelectorAttribute():this(null)
		{
			
		}
		public PXOwnerSelectorAttribute(Type workgroupType)
			: this(workgroupType,null)
		{
		}

		protected PXOwnerSelectorAttribute(Type workgroupType, Type search, bool validateValue = true, bool inquiryMode = false)
		{
			PXSelectorAttribute selector;
            _Attributes.Add(selector = new OwnerSubstituteSelectorAttribute(search ?? CreateSelect(workgroupType),
            typeof(EPEmployee.acctName), typeof(EPEmployee.acctCD), 
			typeof(EPEmployee.departmentID)));
			_SelAttrIndex = _Attributes.Count - 1;

			selector.DescriptionField = typeof(EPEmployee.acctName);
			selector.SubstituteKey = typeof(EPEmployee.acctCD);
			selector.ValidateValue = validateValue;
		    selector.CacheGlobal = true;
			_workgroupType = workgroupType;

            if (!inquiryMode)
            {
                _Attributes.Add(new PXRestrictorAttribute(typeof(Where<EPEmployee.status, IsNull, Or<EPEmployee.status, NotEqual<BAccount.status.inactive>>>), Objects.EP.Messages.InactiveEpmloyee, typeof(EPEmployee.acctCD), typeof(EPEmployee.status)));                
            }
        }		
		
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			if (_workgroupType != null)
			{
				sender.Graph.RowUpdated.AddHandler(sender.GetItemType(), RowUpdated);
			}			
		}

		private static Type CreateSelect(Type workgroupType)
		{
			if( workgroupType == null )
				return typeof(Search<EPEmployee.pKID,Where<EPEmployee.acctCD, IsNotNull>>);

			return BqlCommand.Compose(
							typeof(Search2<,,>), typeof(EPEmployee.pKID),
							typeof(LeftJoin<,>), typeof(EPCompanyTreeMember),
							typeof(On<,,>), typeof(EPCompanyTreeMember.userID), typeof(Equal<EPEmployee.pKID>),
							typeof(And<,>), typeof(EPCompanyTreeMember.workGroupID), typeof(Equal<>), typeof(Optional<>), workgroupType,							
							typeof(Where<,,>),
							typeof(Optional<>), workgroupType, typeof(IsNull),
							typeof(Or<EPCompanyTreeMember.userID, IsNotNull>)
							);			
		}

		protected virtual void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (e.Row != null)
			{
				int? WorkGroupID = (int?)sender.GetValue(e.Row, _workgroupType.Name);
				int? oldWorkGroupID = (int?)sender.GetValue(e.OldRow, _workgroupType.Name);
				Guid? OwnerID = (Guid?)sender.GetValue(e.Row, _FieldName);

				if (WorkGroupID != oldWorkGroupID && !BelongsToWorkGroup(sender.Graph, WorkGroupID, OwnerID))
				{
                    sender.SetValue(e.Row, _FieldName, OwnerWorkGroup(sender.Graph, WorkGroupID));
				}
			}
		}
		public static bool BelongsToWorkGroup(PXGraph graph, int? WorkGroupID, Guid? OwnerID)
		{
			if (WorkGroupID == null && OwnerID != null || OwnerID == null) return true;

			return PXSelect<EPCompanyTreeMember,
					Where<EPCompanyTreeMember.workGroupID, Equal<Required<EPCompanyTreeMember.workGroupID>>,
					And<EPCompanyTreeMember.userID, Equal<Required<EPCompanyTreeMember.userID>>>>>
					.Select(graph, WorkGroupID, OwnerID).Count > 0;
		}

		public static Guid? OwnerWorkGroup(PXGraph graph, int? WorkGroupID)
		{
			EPCompanyTreeMember member = PXSelect<EPCompanyTreeMember,
				Where<EPCompanyTreeMember.workGroupID, Equal<Required<EPCompanyTreeMember.workGroupID>>,
					And<EPCompanyTreeMember.isOwner, Equal<Required<EPCompanyTreeMember.isOwner>>>>>
				.Select(graph, WorkGroupID, 1);
			return member != null ? member.UserID : null;
		}

        public static int? DefaultWorkgroup(PXGraph graph, Guid? userID)
	    {
            PXSelectBase<EPCompanyTreeMember> cmd = new PXSelectJoin<EPCompanyTreeMember,
                  InnerJoin<EPCompanyTreeH, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTreeH.workGroupID>>>,
                  Where<EPCompanyTreeMember.userID, Equal<Required<EPCompanyTreeMember.userID>>>>(graph);
            EPCompanyTreeMember m = cmd.SelectSingle(userID ?? graph.Accessinfo.UserID);
	        return m != null ? m.WorkGroupID : null;
	    }
	}

	#endregion
	
	#region PXSubordinateOwnerSelectorAttribute

	/// <summary>
	/// Allows show employees which are subordinated or coworkers for current logined employee.
	/// </summary>
	/// <example>
	/// [PXSubordinateOwnerSelector]
	/// </example>
	public class PXSubordinateOwnerSelectorAttribute : PXOwnerSelectorAttribute
	{
		public PXSubordinateOwnerSelectorAttribute()
			: base(null, typeof(Search5<EPEmployee.pKID,
				LeftJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.userID, Equal<EPEmployee.pKID>>>,
				Where<EPEmployee.pKID, Equal<Current<AccessInfo.userID>>,
				   Or<EPCompanyTreeMember.workGroupID, Owned<Current<AccessInfo.userID>>>>,
				Aggregate<GroupBy<EPEmployee.pKID>>>), false, true)
		{			
		}
	}
	#endregion
}

namespace PX.Objects.EP
{
	#region PXSubordinateSelectorAttribute
	/// <summary>
	/// Allow show employees which are subordinated of coworkers for logined employee.
	/// You can specify additional filter for EPEmployee records.
	/// It's a 'BIZACCT' dimension selector.
	/// </summary>
	/// <example>
	/// [PXSubordinateSelector]
	/// </example>
	public class PXSubordinateSelectorAttribute : PXAggregateAttribute
	{		
		public Type DescriptionField
		{
			get
			{
				return this.GetAttribute<PXSelectorAttribute>().DescriptionField;
			}
			set
			{
				this.GetAttribute<PXSelectorAttribute>().DescriptionField = value;
			}
		}

		public Type SubstituteKey
		{
			get
			{
				return this.GetAttribute<PXSelectorAttribute>().SubstituteKey;
			}
			set
			{
				this.GetAttribute<PXSelectorAttribute>().SubstituteKey = value;
			}
		}

        public PXSubordinateSelectorAttribute(Type where)
            :this("BIZACCT", GetCommand(where), true, true)
		{			
		}

        protected PXSubordinateSelectorAttribute(string DimensionName, Type seach, bool validCombo, bool restrictInactiveEmployee)
	    {
            PXDimensionAttribute attr = new PXDimensionAttribute(DimensionName);
            attr.ValidComboRequired = validCombo;
            _Attributes.Add(attr);

            PXSelectorAttribute selattr = new PXSelectorAttribute(seach,
                typeof(CREmployee.acctCD),
                typeof(CREmployee.bAccountID), typeof(CREmployee.acctName),
                typeof(CREmployee.departmentID));
            selattr.SubstituteKey = typeof(CREmployee.acctCD);
            selattr.DescriptionField = typeof(CREmployee.acctName);

            _Attributes.Add(selattr);
            if (restrictInactiveEmployee)
                _Attributes.Add(new PXRestrictorAttribute(typeof(Where<CREmployee.status, NotEqual<BAccount.status.inactive>>), Objects.EP.Messages.InactiveEpmloyee, typeof(CREmployee.acctCD), typeof(CREmployee.status)));

        }

        public PXSubordinateSelectorAttribute()
			: this(null)
		{
		}

        private static Type GetCommand(Type where)
        {
            var whereType = typeof(Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>,
                        Or<EPCompanyTreeMember.workGroupID, Owned<Current<AccessInfo.userID>>>>);
            if (where != null)
                whereType = BqlCommand.Compose(typeof(Where2<,>),
                    typeof(Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>,
                        Or<EPCompanyTreeMember.workGroupID, Owned<Current<AccessInfo.userID>>>>),
                    typeof(And<>), where);
            return BqlCommand.Compose(typeof(Search5<,,,>), typeof(CREmployee.bAccountID),
                typeof(LeftJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.userID, Equal<CREmployee.userID>>>),
                whereType,
                typeof(Aggregate<GroupBy<CREmployee.acctCD>>));
        }

		
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			sender.Graph.FieldUpdating.RemoveHandler(sender.GetItemType(), _FieldName, this.GetAttribute<PXSelectorAttribute>().SubstituteKeyFieldUpdating);
			sender.Graph.FieldUpdating.AddHandler(sender.GetItemType(), _FieldName, FieldUpdating);
		}

		protected virtual void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			CREmployee employee = PXSelect<CREmployee, Where<CREmployee.acctCD, Equal<Required<CREmployee.acctCD>>>>
				.SelectWindowed(sender.Graph, 0, 1, e.NewValue);
			if (employee != null)
			{
				e.NewValue = employee.BAccountID;
				e.Cancel = true;
			}
			else
			{
				PXFieldUpdating fu = this.GetAttribute<PXDimensionAttribute>().FieldUpdating;
				fu(sender, e);
				e.Cancel = false;

				fu = this.GetAttribute<PXSelectorAttribute>().SubstituteKeyFieldUpdating;
				fu(sender, e);
				
			}
		}

		public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
		{
			if (typeof(ISubscriber) != typeof(IPXFieldUpdatingSubscriber) && 
				  typeof(ISubscriber) != typeof(IPXRowPersistingSubscriber) &&
					typeof(ISubscriber) != typeof(IPXFieldDefaultingSubscriber))
			{
				base.GetSubscriber<ISubscriber>(subscribers);
			}
		}

        public static bool IsSubordinated(PXGraph graph, object employeeId)
        {
            var command = BqlCommand.CreateInstance(GetCommand(null));
            var filter = new[] { new PXFilterRow
							{
								DataField = typeof(CREmployee.bAccountID).Name,
								Condition = PXCondition.EQ,
								Value = employeeId
							}};
            var view = new PXView(graph, true, command);
            int startRow = 0;
            int totalRows = 0;
            var res = view.Select(null, null, null, null, null, filter, ref startRow, 1, ref totalRows);
            return res.Return(_ => _.Count(), 0) > 0;
        }
	}

	#endregion

	#region PXEPEmployeeSelectorAttribute

	/// <summary>
	/// Dimension selector for EPEmployee.
	/// </summary>
	/// <example>
	/// [PXEPEmployeeSelector]
	/// </example>
	public class PXEPEmployeeSelectorAttribute : PXDimensionSelectorAttribute
	{
		public PXEPEmployeeSelectorAttribute()
			: base(
				"EMPLOYEE",
            typeof(Search<CR.Standalone.EPEmployee.bAccountID>),
            typeof(CR.Standalone.EPEmployee.acctCD),
				typeof(CR.Standalone.EPEmployee.bAccountID), 
				typeof(CR.Standalone.EPEmployee.acctCD), 
				typeof(CR.Standalone.EPEmployee.acctName),
            typeof(CR.Standalone.EPEmployee.departmentID))
		{
            DescriptionField = typeof(CR.Standalone.EPEmployee.acctName);
		}
	}

	#endregion

	public class PXWorkgroupSelectorAttribute : PXSelectorAttribute
	{
		public PXWorkgroupSelectorAttribute()
			:this(null)
		{
			
		}
		public PXWorkgroupSelectorAttribute(Type rootWorkgroupID)
			: base(rootWorkgroupID == null
							? typeof(Search3<EPCompanyTree.workGroupID, 
								 LeftJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>, And<EPCompanyTreeMember.isOwner, Equal<True>>>,
                               LeftJoin<CREmployee, On<CREmployee.userID, Equal<EPCompanyTreeMember.userID>>>>, 
								 OrderBy<Asc<EPCompanyTree.description, Asc<EPCompanyTree.workGroupID>>>>)
			       	: BqlCommand.Compose(
			       		typeof (Search2<,,,>), typeof (EPCompanyTree.workGroupID),
								typeof(InnerJoin<EPCompanyTreeH, On<EPCompanyTreeH.workGroupID, Equal<EPCompanyTree.workGroupID>>,
											 LeftJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.workGroupID, Equal<EPCompanyTree.workGroupID>, And<EPCompanyTreeMember.isOwner, Equal<True>>>,
                                             LeftJoin<CREmployee, On<CREmployee.userID, Equal<EPCompanyTreeMember.userID>>>>>),
								typeof(Where2<,>),
									typeof(Where<,,>), typeof(Current<>), rootWorkgroupID, typeof(IsNotNull), typeof(And<,>), typeof(EPCompanyTreeH.parentWGID), typeof(Equal<>), typeof(Current<>), rootWorkgroupID,
								typeof(Or<>),
									typeof(Where<,,>), typeof(Current<>), rootWorkgroupID, typeof(IsNull), typeof(And<,>), typeof(EPCompanyTreeH.parentWGID), typeof(Equal<>), typeof(EPCompanyTreeH.workGroupID),
			       		typeof (OrderBy<
			       			Asc<EPCompanyTree.description,
                                    Asc<EPCompanyTree.workGroupID>>>)), typeof(EPCompanyTree.description), typeof(CREmployee.acctCD), typeof(CREmployee.acctName))
		{
			SubstituteKey = typeof (EPCompanyTree.description);
		}
	}

	#region EPExpenceClaimSelectorAttribute

	/// <summary>
	/// Allow show expence claim records.
	/// </summary>
	/// <example>
	/// [EPExpenceClaimSelector]
	/// </example>
	public class EPExpenceClaimSelectorAttribute : PXSelectorAttribute
	{
		public EPExpenceClaimSelectorAttribute()
			: base(typeof(Search2<EPExpenseClaim.refNbr,
					InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPExpenseClaim.employeeID>>>,
					Where<EPExpenseClaim.createdByID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, Equal<Current<AccessInfo.userID>>, 
						 Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
						 Or<EPExpenseClaim.noteID, Approver<Current<AccessInfo.userID>>,
						 Or<EPExpenseClaim.employeeID, WingmanUser<Current<AccessInfo.userID>>>>>>>, OrderBy<Desc<EPExpenseClaim.refNbr>>>)
				, typeof(EPExpenseClaim.docDate)
				, typeof(EPExpenseClaim.refNbr)
				, typeof(EPExpenseClaim.status)
				, typeof(EPExpenseClaim.docDesc)
				, typeof(EPExpenseClaim.curyDocBal)
				, typeof(EPExpenseClaim.curyID)
				, typeof(EPEmployee.acctName)
				, typeof(EPExpenseClaim.departmentID)
				)
		{
		}
	}

	#endregion

	#region EPExpenceReceiptSelectorAttribute

	/// <summary>
	/// Allow show expence receipt records.
	/// </summary>
	/// <example>
	/// [EPExpenceReceiptSelector]
	/// </example>
	public class EPExpenceReceiptSelectorAttribute : PXSelectorAttribute
	{
		public EPExpenceReceiptSelectorAttribute()
			: base(typeof(Search2<EPExpenseClaimDetails.claimDetailID,
					InnerJoin<EPEmployee, On<EPEmployee.bAccountID, Equal<EPExpenseClaimDetails.employeeID>>>,
					Where<EPExpenseClaimDetails.createdByID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, Equal<Current<AccessInfo.userID>>,
						 Or<EPEmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
						 Or<EPExpenseClaimDetails.employeeID, WingmanUser<Current<AccessInfo.userID>>,
                         Or<EPExpenseClaimDetails.noteID, Approver<Current<AccessInfo.userID>>>>>>>,
                    OrderBy<Desc<EPExpenseClaimDetails.claimDetailID>>>)
					, typeof(EPExpenseClaimDetails.claimDetailID)
					, typeof(EPExpenseClaimDetails.expenseDate)
					, typeof(EPExpenseClaimDetails.curyTranAmt)
					, typeof(EPExpenseClaimDetails.curyID)
					, typeof(EPExpenseClaimDetails.employeeID)
					, typeof(EPExpenseClaimDetails.refNbr)
                    , typeof(EPExpenseClaimDetails.status)
				)
		{
		}
	}

	#endregion

	#region EPTaxAttribute

	public class EPTaxAttribute : TaxAttribute
	{
		public EPTaxAttribute(Type ParentType, Type TaxType, Type TaxSumType)
			: base(ParentType, TaxType, TaxSumType)
		{
			this.CuryDocBal = typeof(EPExpenseClaim.curyDocBal);
			this.CuryLineTotal = typeof(EPExpenseClaim.curyLineTotal);
			this.DocDate = typeof(EPExpenseClaim.docDate);
			this.CuryTranAmt = typeof(EPExpenseClaimDetails.claimCuryTranAmt);
		}

		protected override List<object> SelectTaxes<Where>(PXGraph graph, object row, PXTaxCheck taxchk, params object[] parameters)
		{
			Dictionary<string, PXResult<Tax, TaxRev>> tail = new Dictionary<string, PXResult<Tax, TaxRev>>();
			object[] currents = new object[] { row, ((ExpenseClaimEntry)graph).ExpenseClaim.Current };
			foreach (PXResult<Tax, TaxRev> record in PXSelectReadonly2<Tax,
				LeftJoin<TaxRev, On<TaxRev.taxID, Equal<Tax.taxID>,
					And<TaxRev.outdated, Equal<boolFalse>,
					And2<Where<TaxRev.taxType, Equal<TaxType.purchase>, And<Tax.reverseTax, Equal<boolFalse>,
						Or<TaxRev.taxType, Equal<TaxType.sales>, And<Tax.reverseTax, Equal<boolTrue>,
						Or<Tax.taxType, Equal<CSTaxType.use>, 
						Or<Tax.taxType, Equal<CSTaxType.withholding>>>>>>>,
					And<Current<EPExpenseClaim.docDate>, Between<TaxRev.startDate, TaxRev.endDate>>>>>>,
				Where>
				.SelectMultiBound(graph, currents, parameters))
			{
				tail[((Tax)record).TaxID] = record;
			}
			List<object> ret = new List<object>();
			switch (taxchk)
			{
				case PXTaxCheck.Line:
					foreach (EPTax record in PXSelect<EPTax,
						Where<EPTax.refNbr, Equal<Current<EPExpenseClaim.refNbr>>,
							And<EPTax.claimDetailID, Equal<Current<EPExpenseClaimDetails.claimDetailID>>>>>
						.SelectMultiBound(graph, currents))
					{
						PXResult<Tax, TaxRev> line;
						if (tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& String.Compare(((Tax)(PXResult<EPTax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<EPTax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				case PXTaxCheck.RecalcLine:
					foreach (EPTax record in PXSelect<EPTax,
						Where<EPTax.refNbr, Equal<Current<EPExpenseClaim.refNbr>>,
							And<EPTax.claimDetailID, Less<intMax>>>>
						.SelectMultiBound(graph, currents))
					{
						PXResult<Tax, TaxRev> line;
						if (tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& ((EPTax)(PXResult<EPTax, Tax, TaxRev>)ret[idx - 1]).ClaimDetailID == record.ClaimDetailID
								&& String.Compare(((Tax)(PXResult<EPTax, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<EPTax, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				case PXTaxCheck.RecalcTotals:
					foreach (EPTaxTran record in PXSelect<EPTaxTran,
						Where<EPTaxTran.refNbr, Equal<Current<EPExpenseClaim.refNbr>>>>
						.SelectMultiBound(graph, currents))
					{
						PXResult<Tax, TaxRev> line;
						if (record.TaxID != null && tail.TryGetValue(record.TaxID, out line))
						{
							int idx;
							for (idx = ret.Count;
								(idx > 0)
								&& String.Compare(((Tax)(PXResult<EPTaxTran, Tax, TaxRev>)ret[idx - 1]).TaxCalcLevel, ((Tax)line).TaxCalcLevel) > 0;
								idx--) ;
							ret.Insert(idx, new PXResult<EPTaxTran, Tax, TaxRev>(record, (Tax)line, (TaxRev)line));
						}
					}
					return ret;
				default:
					return ret;
			}
		}

		protected void ParentChanged(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			EPExpenseClaimDetails row = (EPExpenseClaimDetails)e.Row;
			if (row != null)
			{
				if (row.RefNbr == null)
				{
					ClearTaxes(sender, row);
				}
				else
				{
					EPExpenseClaimDetails oldRow = (EPExpenseClaimDetails) sender.CreateCopy(row);
					oldRow.TaxCategoryID = null;
					RowUpdated(sender, new PXRowUpdatedEventArgs(row, oldRow, e.ExternalCall));
				}
			}
		}

		public override void CacheAttached(PXCache sender)
		{
			if (sender.Graph is ExpenseClaimEntry)
			{
				sender.Graph.FieldUpdated.AddHandler<EPExpenseClaimDetails.refNbr>(ParentChanged);
				base.CacheAttached(sender);
			}
			else
			{
				this.TaxCalc = TaxCalc.NoCalc;
			}
		}
	}

	#endregion

	#region EPSetupSelect
	[Obsolete("Will be removed in 7.0 version")]
	public class EPSetupSelect : PXSetupOptional<EPSetup>
	{
		public EPSetupSelect(PXGraph graph)
			: base(graph)
		{

		}
	}

	#endregion

	#region EPAcctSubDefault

	public class EPAcctSubDefault
	{
        public class CustomListAttribute : PXStringListAttribute
		{
            public string[] AllowedValues
			{
                get
			{
                    return _AllowedValues;
			}
		}

            public string[] AllowedLabels
            {
                get
		{
                    return _AllowedLabels;
		}
            }

            public CustomListAttribute(string[] AllowedValues, string[] AllowedLabels)
                : base(AllowedValues, AllowedLabels)
		{
			}
        }

        /// <summary>
        /// Specialized version of the string list attribute which represents <br/>
        /// the list of the possible sources of the segments for the sub-account <br/>
        /// defaulting in the AP transactions. <br/>
        /// </summary>
		public class ClassListAttribute : CustomListAttribute
			{
            public ClassListAttribute()
                : base(new string[] { MaskEmployee, MaskItem, MaskCompany, MaskProject, MaskTask, MaskLocation }, new string[] { Messages.MaskEmployee, Messages.MaskItem, Messages.MaskCompany, PM.Messages.Project, Messages.Task, !PXAccess.FeatureInstalled<FeaturesSet.accountLocations>() ? AR.Messages.MaskCustomer : AR.Messages.MaskLocation })
            {
			}

            public override void CacheAttached(PXCache sender)
            {
                _AllowedValues = new[] { MaskEmployee, MaskItem, MaskCompany, MaskProject, MaskTask, MaskLocation };
                _AllowedLabels = new[] { Messages.MaskEmployee, Messages.MaskItem, Messages.MaskCompany, PM.Messages.Project, Messages.Task, !PXAccess.FeatureInstalled<FeaturesSet.accountLocations>() ? AR.Messages.MaskCustomer : AR.Messages.MaskLocation };
                _NeutralAllowedLabels = _AllowedLabels;

                base.CacheAttached(sender);
		}
        }
		public const string MaskEmployee = "E";
		public const string MaskItem = "I";
		public const string MaskCompany = "C";
		public const string MaskProject = "P";
		public const string MaskTask = "T";
		public const string MaskLocation = "L";
	}


	#endregion


	#region SubAccountMaskAttribute

	/// <summary>
	/// Determine mask for sub accounts used in EP module.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount Mask", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
    [EPAcctSubDefault.ClassList]
	public sealed class SubAccountMaskAttribute : AcctSubAttribute
	{
		private const string _DimensionName = "SUBACCOUNT";
		private const string _MaskName = "EPSETUPSALE";
		public SubAccountMaskAttribute()
			: base()
		{
            PXDimensionMaskAttribute attr = new PXDimensionMaskAttribute(_DimensionName, _MaskName, EPAcctSubDefault.MaskEmployee, new EPAcctSubDefault.ClassListAttribute().AllowedValues, new EPAcctSubDefault.ClassListAttribute().AllowedLabels);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}

        public override void GetSubscriber<ISubscriber>(List<ISubscriber> subscribers)
        {
            base.GetSubscriber<ISubscriber>(subscribers);
            subscribers.Remove(_Attributes.OfType<EPAcctSubDefault.ClassListAttribute>().FirstOrDefault() as ISubscriber);
        }

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            var stringlist = (EPAcctSubDefault.ClassListAttribute)_Attributes.First(x => x.GetType() == typeof(EPAcctSubDefault.ClassListAttribute));
            var dimensionmaskattribute = (PXDimensionMaskAttribute)_Attributes.First(x => x.GetType() == typeof(PXDimensionMaskAttribute));
            dimensionmaskattribute.SynchronizeLabels(stringlist.AllowedValues, stringlist.AllowedLabels);
        }

		public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields)
			where Field : IBqlField 
		{
			try
			{
				return PXDimensionMaskAttribute.MakeSub<Field>(graph, mask, new EPAcctSubDefault.ClassListAttribute().AllowedValues, 0, sources);
			}
			catch (PXMaskArgumentException ex)
			{
				PXCache cache = graph.Caches[BqlCommand.GetItemType(fields[ex.SourceIdx])];
				string fieldName = fields[ex.SourceIdx].Name;
				throw new PXMaskArgumentException(new EPAcctSubDefault.ClassListAttribute().AllowedValues[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
			}
		}
	}

	#endregion

	#region EPMessageType

	public sealed class EPMessageType
	{
		/// <summary>
		/// List of attendee message types.
		/// </summary>
		/// <example>
		/// [EPMessageTypeList]
		/// </example>
		public sealed class EPMessageTypeListAttribute : PXIntListAttribute
		{
			public EPMessageTypeListAttribute()
				: base(new int[] { Invitation, CancelInvitation },
						 new string[] { Messages.Invitation, Messages.CancelInvitation })
			{

			}
		}

		public const int Invitation = 1;
		public const int CancelInvitation = 2;

		public class invitation : Constant<int>
		{
			public invitation() : base(Invitation) { }
		}

		public class cancelInvitation : Constant<int>
		{
			public cancelInvitation() : base(CancelInvitation) { }
		}
	}

	#endregion

	#region EPNotificationTemplateAttribute
	[Obsolete("Will be removed in 7.0 version")]
	public sealed class EPNotificationTemplateAttribute : PXEventSubscriberAttribute, IPXRowSelectedSubscriber
	{
		private readonly Type _wikiIDBqlField;
		private readonly Type _pageIDBqlField;
		private string _wikiIDField;
		private string _pageIDField;

		public EPNotificationTemplateAttribute(Type wikiIdBqlField, Type pageIdBqlField)
		{
			_wikiIDBqlField = wikiIdBqlField;
			_pageIDBqlField = pageIdBqlField;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_wikiIDField = sender.GetField(_wikiIDBqlField);
			_pageIDField = sender.GetField(_pageIDBqlField);
		}

		public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			object wikiID = sender.GetValue(e.Row, _wikiIDField);
			object pageID = sender.GetValue(e.Row, _pageIDField);

			object newvalue = null;
			if (wikiID != null && pageID != null)
			{
				PXResultset<WikiDescriptor> wikiSet = PXSelect<WikiDescriptor>.Search<WikiDescriptor.pageID>(sender.Graph, wikiID);
				if (wikiSet != null && wikiSet.Count > 0)
				{
					PXResultset<WikiPageSimple> pageSet = PXSelect<WikiPageSimple>.
						Search<WikiPageSimple.pageID, WikiPageSimple.wikiID>(sender.Graph, pageID, wikiID);
					if (pageSet != null && pageSet.Count > 0)
					{
						PXSiteMapNode nodeWiki = PXSiteMap.WikiProvider.FindSiteMapNodeFromKey(((WikiDescriptor)wikiSet[0]).PageID.Value);
						PXSiteMapNode nodePage = PXSiteMap.WikiProvider.FindSiteMapNodeFromKey(((WikiPageSimple)pageSet[0]).PageID.Value);
						if (pageSet != null && pageSet.Count > 0)
							newvalue = string.Format("{0}/{1}", nodeWiki.Title, nodePage.Title);
					}
				}
			}
			sender.SetValue(e.Row, _FieldOrdinal, newvalue);
		}
	}

	#endregion

	#region EPWikiPageSelectorAttribute
	/// <summary>
	/// Allow show articles of certain wiki.
	/// </summary>
	/// <example>
	/// [EPWikiPageSelector]
	/// </example>
	public sealed class EPWikiPageSelectorAttribute : PXCustomSelectorAttribute
	{
		private readonly Type _wiki;

		public EPWikiPageSelectorAttribute() : this(null) { }

		public EPWikiPageSelectorAttribute(Type wiki) :
			base(typeof(WikiPageSimple.pageID))
		{
			_wiki = wiki;
			_ViewName = GenerateViewName();
		}

		protected override string GenerateViewName()
		{
			return string.Concat(base.GenerateViewName(), "_", _wiki == null ? null : _wiki.Name);
		}

		public IEnumerable GetRecords([PXDBGuid] Guid? wikiId)
		{
			if (wikiId != null || _wiki != null && BqlCommand.GetItemType(_wiki) != null)
			{
				var wikiCache = _Graph.Caches[BqlCommand.GetItemType(_wiki)];
				var id = wikiId;
				if (id == null && _wiki != null)
				{
					var idValue = wikiCache.GetValue(wikiCache.Current, _wiki.Name);
					if (idValue != null) id = GUID.CreateGuid(idValue.ToString());
				}
				if (id != null)
					foreach (WikiPageSimple page in
						PXSelect<WikiPageSimple, Where<WikiPageSimple.wikiID, Equal<Required<WikiPageSimple.wikiID>>>>.
							Select(_Graph, id))
					{
						if (PXSiteMap.WikiProvider.GetAccessRights(page.PageID.Value) >= PXWikiRights.Select)
						{
							PXSiteMapNode node = PXSiteMap.WikiProvider.FindSiteMapNodeFromKey(page.PageID.Value);
							page.Title = (node != null && !string.IsNullOrEmpty(node.Title)) ? node.Title : page.Name;
							yield return page;
						}
					}
			}
		}
	}

	#endregion

	#region EPAllDaySupportDateAttribute
	public class EPAllDaySupportDateTimeAttribute : PXDBDateAndTimeAttribute
	{
		public Type AllDayField { get; set; }

		public override void Time_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			SetPreserveTime(sender, e.Row);
			base.Time_FieldSelecting(sender, e);
		}

		public override void CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			SetPreserveTime(sender, e.Row);
			base.CommandPreparing(sender, e);
		}

	    public override void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
	    {
            SetPreserveTime(sender, e.Row);
	        base.RowSelecting(sender, e);
	    }

	    private void SetPreserveTime(PXCache sender, object row)
		{
			if (AllDayField != null) PreserveTime = (bool?)sender.GetValue(row, sender.GetField(AllDayField)) != true;
		}
	}
	#endregion

	#region EPStartDateAttribute
	public class EPStartDateAttribute : EPAllDaySupportDateTimeAttribute
	{
		private string _DisplayFieldName;

		public EPStartDateAttribute()
		{
			_PreserveTime = true;
			base.UseTimeZone = true;
			base.WithoutDisplayNames = true;
		}

		public string DisplayName { get; set; }

        public bool IgnoreRequireTimeOnActivity { get; set; }

		public override bool UseTimeZone
		{
			get { return base.UseTimeZone; }
			set { }
		}

		public override void CacheAttached(PXCache sender)
		{
			if (!typeof(CRActivity).IsAssignableFrom(sender.GetItemType()))
				throw new ArgumentException(Messages.CRActivityIsExpected);

			_DisplayFieldName = _FieldName + "_Display";
			sender.Fields.Add(_DisplayFieldName);
			sender.Graph.FieldSelecting.AddHandler(sender.GetItemType(), _DisplayFieldName, _DisplayFieldName_FieldSelecting);
			base.CacheAttached(sender);
		}

		private void _DisplayFieldName_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs args)
		{
			var info = string.Format("{0:g}", sender.GetValue(args.Row, _FieldOrdinal));
			string localDispName = DisplayName;
			if (!CultureInfo.InvariantCulture.Equals(System.Threading.Thread.CurrentThread.CurrentCulture))
				localDispName = PXLocalizer.Localize(DisplayName, sender.GetItemType().FullName);

			args.ReturnState = PXFieldState.CreateInstance(info, typeof(string), null, null, null, null, null,
						null, _DisplayFieldName, null, localDispName, null, PXErrorLevel.Undefined, false,
						true, true, PXUIVisibility.Visible, null, null, null);
		}

		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			this.InputMask = this.DisplayMask = RequireTimeOnActivity(sender) ? "g" : "d";
			base.FieldSelecting(sender, e);
		}

		public override void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
		{
			EPSetup setup = sender.Graph.Caches[typeof(EPSetup)].Current as EPSetup ?? new PXSetupSelect<EPSetup>(sender.Graph).SelectSingle();

            if (e.NewValue != null  && (setup == null || setup.RequireTimes != true))
			{
                if (!IgnoreRequireTimeOnActivity)
			    {
			        DateTime date = (DateTime) e.NewValue;
			        e.NewValue = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
			    }
			}
			base.FieldUpdating(sender, e);
		}		

		protected virtual bool RequireTimeOnActivity(PXCache sender)
		{
			EPSetup setup = null;
			try
			{
				setup = sender.Graph.Caches[typeof(EPSetup)].Current as EPSetup ?? new PXSetupSelect<EPSetup>(sender.Graph).SelectSingle();
			}
			catch {/* SKIP */}			
			return (setup != null ? setup.RequireTimes : null ) ?? false;			
		}
	}

	#endregion
   
	#region EPEndDayAttribute
	public sealed class EPEndDateAttribute : EPAllDaySupportDateTimeAttribute, IPXRowUpdatedSubscriber
	{
		private readonly Type ActivityClass;
		private readonly Type StartDate;
		private readonly Type TimeSpent;

		public EPEndDateAttribute(Type activityClass, Type startDate, Type timeSpent = null)
		{
			this.ActivityClass = activityClass;
			this.StartDate = startDate;
			this.TimeSpent = timeSpent;
			this.InputMask = "g";
			this.PreserveTime = true;
			this.WithoutDisplayNames = true;
		}

		public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			this.InputMask = this.DisplayMask = RequireTimeOnActivity(sender) ? "g" : "d";
			base.FieldSelecting(sender, e);
		}

		private bool RequireTimeOnActivity(PXCache sender)
		{
			EPSetup setup = null;
			try
			{
				setup = sender.Graph.Caches[typeof(EPSetup)].Current as EPSetup ?? new PXSetup<EPSetup>(sender.Graph).SelectSingle();
			}
			catch {/* SKIP */}
			return (setup != null ? setup.RequireTimes : null) ?? false;
		}

		#region IPXRowUpdatingSubscriber Members
		public void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			var side = (int?)sender.GetValue(e.Row, ActivityClass.Name);
			var startDate = (DateTime?)sender.GetValue(e.Row, StartDate.Name);			
			var endDate = (DateTime?)sender.GetValue(e.Row, _FieldName);
			var timeSpent = TimeSpent != null ? (int?)sender.GetValue(e.Row, TimeSpent.Name) : null;

		    if (side != null && startDate != null)
			{
				DateTime? value = endDate;
				switch ((int) side)
				{
					case CRActivityClass.Task:
						if (startDate > endDate)
							value = (DateTime) startDate;
						break;
					case CRActivityClass.Event:
						if (startDate > endDate || endDate == null)
							value = ((DateTime) startDate).AddMinutes(30);
						else if (Object.Equals(sender.GetValue(e.OldRow, _FieldName), endDate))
						{
							var oldStartDate = (DateTime?) sender.GetValue(e.OldRow, StartDate.Name);
							if (oldStartDate != null)
								value = ((DateTime) endDate).AddTicks(((DateTime) startDate - (DateTime) oldStartDate).Ticks);
						}
						break;
					default:
						value = timeSpent != null ? (DateTime?) ((DateTime) startDate).AddMinutes((int) timeSpent) : null;
						break;
				}
                sender.SetValue(e.Row, _FieldName, value);
			}			
		}
		#endregion
	}
	#endregion

	#region EPAllDayAttribute
	public sealed class EPAllDayAttribute  : PXDBBoolAttribute, IPXRowUpdatedSubscriber, IPXRowSelectedSubscriber
	{
		private readonly Type StartDate;
		private readonly Type EndDate;
		public EPAllDayAttribute(Type startDate, Type endDate)
		{			
			this.StartDate = startDate;
			this.EndDate = endDate;
		}

		#region IPXRowUpdatedSubscriber Members
		public void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			bool? oldAllDay = (bool?)sender.GetValue(e.OldRow, _FieldName);
			bool? allDay = (bool?)sender.GetValue(e.Row, _FieldName);
			DateTime? startDate = (DateTime?)sender.GetValue(e.Row, StartDate.Name);
			DateTime? endDate = (DateTime?)sender.GetValue(e.Row, EndDate.Name);
			DateTime? newStartDate = startDate;
			DateTime? newEndDate = endDate;
			if (allDay == true && startDate != null)
			{				
				if (endDate == null || ((DateTime)endDate).Date <= ((DateTime)startDate).Date)
					newEndDate = ((DateTime) startDate).AddDays(1D);

				newStartDate = ((DateTime) startDate).Date;
				newEndDate = ((DateTime) newEndDate).Date;
			}
			
			if (allDay != true && allDay != oldAllDay)
			{
				newStartDate = ((DateTime)startDate).Date;
				newEndDate = ((DateTime)newEndDate).Date;
			}

			if (startDate != newStartDate || endDate != newEndDate)
			{
				sender.SetValue(e.Row, StartDate.Name, newStartDate);
				sender.SetValue(e.Row, EndDate.Name, newEndDate);
			}
		}
		#endregion

		#region IPXRowSelectedSubscriber Members

		public void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			bool? allDay = (bool?)sender.GetValue(e.Row, _FieldName);
			PXFieldState startState = sender.GetStateExt(e.Row, typeof(CRActivity.startDate).Name + PXDBDateAndTimeAttribute.DATE_FIELD_POSTFIX) as PXFieldState;
			PXFieldState endState = sender.GetStateExt(e.Row, typeof(CRActivity.endDate).Name + PXDBDateAndTimeAttribute.DATE_FIELD_POSTFIX) as PXFieldState;
			if(startState != null)
				PXDBDateAndTimeAttribute.SetTimeEnabled<CRActivity.startDate>(sender, e.Row, startState.Enabled && allDay != true );
			if (endState != null)
				PXDBDateAndTimeAttribute.SetTimeEnabled<CRActivity.endDate>(sender, e.Row, endState.Enabled &&  allDay != true );
	}

	#endregion
	}
	#endregion

	#region PXInvitationStatusAttribute

	/// <summary>
	/// List of invitation statuses.
	/// </summary>
	/// <example>
	/// [PXInvitationStatus]
	/// </example>
	public class PXInvitationStatusAttribute : PXIntListAttribute
	{
		public const int NOTINVITED = 0;
		public const int INVITED = 1;
		public const int ACCEPTED = 2;
		public const int REJECTED = 3;
		public const int RESCHEDULED = 4;
		public const int CANCELED = 5;

		public PXInvitationStatusAttribute()
			: base(
				new[] { NOTINVITED, INVITED, ACCEPTED, REJECTED, RESCHEDULED, CANCELED },
				new[] { Messages.InvitationNotInvited,
                  Messages.InvitationInvited, 
				  Messages.InvitationAccepted,
				  Messages.InvitationRejected,
				  Messages.InvitationRescheduled,
				  Messages.InvitationCanceled })
		{
		}
	}

	#endregion

	#region EPDependNoteList

	public class EPDependNoteList<Table, FRefNoteID, ParentTable> : PXSelect<Table>
		where Table : class, IBqlTable, new()
		where ParentTable : class, IBqlTable
		where FRefNoteID : class, IBqlField
	{
		protected PXView _History;
		
		public EPDependNoteList(PXGraph graph)
			: base(graph)
		{
			PXDBDefaultAttribute.SetSourceType<FRefNoteID>(graph.Caches[typeof(Table)], SourceNoteID);
			this.View = new PXView(graph, false, BqlCommand.CreateInstance(BqlCommand.Compose(typeof(Select<,>), typeof(Table), ComposeWhere)));
			Init(graph);
		}
		public EPDependNoteList(PXGraph graph, Delegate handler)
			: base(graph, handler)
		{			
			Init(graph);
		}

		protected Type SourceNoteID
		{
			get { return typeof(ParentTable).GetNestedType(EntityHelper.GetNoteField(typeof(ParentTable))); }
		}
		protected Type RefNoteID
		{
			get { return typeof (FRefNoteID); }
		}

		protected Type ComposeWhere
		{
			get { return BqlCommand.Compose(typeof (Where<,>), RefNoteID, typeof (Equal<>), typeof (Current<>), SourceNoteID); }
		}

		private void Init(PXGraph graph)
		{
			graph.RowInserted.AddHandler(BqlCommand.GetItemType(SourceNoteID), Source_RowInserted);
			graph.RowDeleted.AddHandler(BqlCommand.GetItemType(SourceNoteID), Source_RowDeleted);

			if(!graph.Views.Caches.Contains(typeof(Note)))
				graph.Views.Caches.Add(typeof(Note));

			PXCache source = graph.Caches[typeof(Table)];
			_History = CreateView(graph, BqlCommand.Compose(
				typeof(Select<,>), typeof(Table),
				typeof(Where<,>), RefNoteID,
				typeof(Equal<>), typeof(Required<>), SourceNoteID));		
		}

		protected virtual PXView CreateView(PXGraph graph, Type command)
		{
			PXView view = new PXView(graph, false, BqlCommand.CreateInstance(command));
			graph.Views.Add(Guid.NewGuid().ToString(), view);
			return view;
		}

		protected virtual void Source_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			Guid? NoteID = (Guid?)sender.GetValue(e.Row, SourceNoteID.Name);
			foreach (Table item in this._History.SelectMulti(NoteID))
				this.Cache.Delete(item);
		}

		protected virtual void Source_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
            GetSourceNoteID(e.Row);
		}

		protected Guid? GetSourceNoteID(object source)
		{
			PXCache cache = this._Graph.Caches[source.GetType()];
			var noteCache = _Graph.Caches[typeof(Note)];
			var oldDirty = noteCache.IsDirty;
			var res = (Guid?)PXNoteAttribute.GetNoteID(cache, source, SourceNoteID.Name);
			noteCache.IsDirty = oldDirty;
			return res;
		}

		protected Guid? GetRefNoteID(object source)
		{
			PXCache cache = this._Graph.Caches[source.GetType()];
			return (Guid?)cache.GetValue(source, RefNoteID.Name);
		}
	}
	#endregion

	#region EPApprovalList

    public class ApprovalMap
    {
        public readonly int ID;
        public readonly int? NotificationID;

        public ApprovalMap(int assignmentMapID, int? notificationID)
        {
            ID = assignmentMapID;
            NotificationID = notificationID;
        }
    }

	public class EPApprovalList<SourceAssign> : EPDependNoteList<EPApproval, EPApproval.refNoteID, SourceAssign>
		where SourceAssign : class, IAssign,  IBqlTable
	{
		#region State
		protected PXView _Pending;
		protected PXView _Find;
        protected PXView _FindOwner;
		protected PXView _Rejected;
		protected PXAction _Activity;
		#endregion

		#region Ctor
		public EPApprovalList(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
			Initialize(graph);
		}

		public EPApprovalList(PXGraph graph)
			: base(graph)
		{
			Initialize(graph);
		}

		private void Initialize(PXGraph graph)
		{
			PXCache source = graph.Caches[typeof(EPApproval)];
			
			_Activity = graph.Actions["RegisterActivity"];


			this.View = CreateView(graph, BqlCommand.Compose(
				typeof (Select2<,,>), typeof (EPApproval),
				typeof (LeftJoin<ApproverEmployee, On<ApproverEmployee.userID, Equal<EPApproval.ownerID>>,
					LeftJoin<ApprovedByEmployee, On<ApprovedByEmployee.userID, Equal<EPApproval.approvedByID>>>>),
				typeof(Where<,>), typeof(EPApproval.refNoteID), typeof(Equal<>), typeof(Current<>), SourceNoteID));

			_Pending = CreateView(graph, BqlCommand.Compose(
											typeof(Select<,>), typeof(EPApproval),
											typeof(Where<,,>), source.GetBqlField(typeof(EPApproval.refNoteID).Name),
											typeof(Equal<>), typeof(Required<>), SourceNoteID,
											typeof(And<,>), source.GetBqlField(typeof(EPApproval.status).Name),
											typeof(Equal<>), typeof(EPApprovalStatus.pending)));

			_Find = CreateView(graph, BqlCommand.Compose(
										typeof(Select<,>), typeof(EPApproval),
										typeof(Where<,,>), source.GetBqlField(typeof(EPApproval.refNoteID).Name),
										typeof(Equal<>), typeof(Required<>), SourceNoteID,
										typeof(And<,,>), source.GetBqlField(typeof(EPApproval.assignmentMapID).Name),
										typeof(Equal<>), typeof(Required<>), source.GetBqlField(typeof(EPApproval.assignmentMapID).Name),
										typeof(And<,>), source.GetBqlField(typeof(EPApproval.workgroupID).Name),
										typeof(Equal<>), typeof(Required<>), source.GetBqlField(typeof(EPApproval.workgroupID).Name)));

            _FindOwner = CreateView(graph, BqlCommand.Compose(
                                        typeof(Select<,>), typeof(EPApproval),
                                        typeof(Where<,,>), source.GetBqlField(typeof(EPApproval.refNoteID).Name),
                                        typeof(Equal<>), typeof(Required<>), SourceNoteID,
                                        typeof(And<,,>), source.GetBqlField(typeof(EPApproval.assignmentMapID).Name),
                                        typeof(Equal<>), typeof(Required<>), source.GetBqlField(typeof(EPApproval.assignmentMapID).Name),
                                        typeof(And<,,>), source.GetBqlField(typeof(EPApproval.workgroupID).Name), typeof(IsNull),
                                        typeof(And<,>), source.GetBqlField(typeof(EPApproval.ownerID).Name),
                                        typeof(Equal<>), typeof(Required<>), source.GetBqlField(typeof(EPApproval.ownerID).Name)));

			_Rejected = CreateView(graph, BqlCommand.Compose(
										typeof(Select<,>), typeof(EPApproval),
										typeof(Where<,,>), source.GetBqlField(typeof(EPApproval.refNoteID).Name),
										typeof(Equal<>), typeof(Required<>), SourceNoteID,
										typeof(And<,>), source.GetBqlField(typeof(EPApproval.status).Name),
										typeof(Equal<>), typeof(EPApprovalStatus.rejected)));

            graph.RowPersisted.AddHandler<SourceAssign>(OnPersisted);
		}


		#endregion

		#region Implementation
        public bool Assign(SourceAssign source, IEnumerable<ApprovalMap> maps)
		{
            if (source == null) return false;            
			this.Reset(source);
            bool result = false;
            foreach(ApprovalMap map in maps)
                if (Assign(source, null, map.ID, map.NotificationID))
                    result = true;
            return result;
        }	

        public bool Assign(SourceAssign source, int? map, int? notification/* = null*/)
        {
            this.Reset(source);
            return Assign(source, null, map, notification);
		}

		public virtual void Reset(SourceAssign source)
		{
			foreach (EPApproval item in this._History.SelectMulti(GetSourceNoteID(source)))
				this.Cache.Delete(item);
		}

		public virtual bool Approve(SourceAssign source)
		{
			EPApproval item = (EPApproval)this._Rejected.SelectSingle(GetSourceNoteID(source));
			if (item != null) throw new PXException(Messages.CannotApproveRejectedItem);
			if (UpdateApproval(source, EPApprovalStatus.Approved))
			{
				this.RegisterActivity(source, PO.Messages.Approved);
				return true;
			}
			return false;
		}

		public virtual bool Reject(SourceAssign source)
		{
			if(UpdateApproval(source, EPApprovalStatus.Rejected))
			{
				this.RegisterActivity(source, PO.Messages.Rejected);
				return true;
			}
			return false;
		}

		public virtual void ClearPendingApproval(SourceAssign source)
		{
			foreach (EPApproval item in this._Pending.SelectMulti(GetSourceNoteID(source)))
			{
				this.Cache.Delete(item);
			}
		}

		public virtual bool IsApprover(SourceAssign source)
		{
			bool result = true;
			foreach (EPApproval item in this._Pending.SelectMulti(GetSourceNoteID(source)))
			{
				result = false;
				if (ValidateAccess(item.WorkgroupID, item.OwnerID))
					return true;
			}
			return result;
		}

		public virtual bool IsApproved(SourceAssign source)
		{
			var sourceNoteId = GetSourceNoteID(source);
			EPApproval item = (EPApproval)this._Pending.SelectSingle(sourceNoteId);
			if (item == null)
			{
				item = (EPApproval)this._Rejected.SelectSingle(sourceNoteId);
				return item == null;
			}
			return false;
		}

		private bool UpdateApproval(SourceAssign source, string status)
		{
			bool result = false;
			var stillPending = new List<EPApproval>();
			foreach (EPApproval item in this._Pending.SelectMulti(GetSourceNoteID(source)))
			{
				if (ValidateAccess(item.WorkgroupID, item.OwnerID))
				{
					item.ApprovedByID = PXAccess.GetUserID();
					item.ApproveDate = DateTime.Now;
					item.Status = status;
					if (this.Cache.Update(item) != null)
					{
						result = true;
						if (status == EPApprovalStatus.Approved)
							Assign(source, item, item.AssignmentMapID, item.NotificationID);
					}
				}
				if (item.Status == EPApprovalStatus.Pending)
					stillPending.Add(item);
			}
			if (result && status == EPApprovalStatus.Rejected)
			{
				stillPending.ForEach(item => this.Cache.Delete(item));
			}
			return result;
		}

		public bool ValidateAccess(SourceAssign source)
		{
			foreach (EPApproval item in this._Pending.SelectMulti(GetSourceNoteID(source)))
				if (ValidateAccess(item.WorkgroupID, item.OwnerID))
					return true;
			return false;
		}

        private bool Assign(SourceAssign source, EPApproval aproved, int? map, int? notification)
        {
            if (map == null) return false;
            PXCache sourceCache = this.Cache.Graph.Caches[typeof(SourceAssign)];
            PXCache cache = this.Cache.Graph.Caches[BqlCommand.GetItemType(SourceNoteID)];

            using (new CRAssigmentScope(source))
            {
                source.WorkgroupID = aproved == null ? null : aproved.WorkgroupID;
                source.OwnerID = aproved == null ? null : aproved.OwnerID;


                if (!DoAssignApprover(source, map.Value)) return false;

                EPApproval exists = (EPApproval)
                    (source.WorkgroupID == null
                        ? this._FindOwner.SelectSingle(GetSourceNoteID(source), map, source.OwnerID)
                        : this._Find.SelectSingle(GetSourceNoteID(source), map, source.WorkgroupID));

                if (exists != null) return false;

                if (sourceCache.GetStatus(source) == PXEntryStatus.Notchanged)
                    sourceCache.SetStatus(source, PXEntryStatus.Updated);

                EPApproval item = new EPApproval();
                item.RefNoteID = GetSourceNoteID(source);
                item.WorkgroupID = source.WorkgroupID;
                item.OwnerID = source.OwnerID;
                item.Status = EPApprovalStatus.Pending;
                item.AssignmentMapID = map;
                item.NotificationID = notification;
                item = (EPApproval)this.Cache.Insert(item);
                if (aproved != null && item != null)
                {
                    if (ValidateAccess(item.WorkgroupID, item.OwnerID))
                    {
                        item.ApprovedByID = PXAccess.GetUserID();
                        item.ApproveDate = DateTime.Now;
                        item.Status = EPApprovalStatus.Approved;
                        if (this.Cache.Update(item) != null)
                            Assign(source, item, item.AssignmentMapID, item.NotificationID);
                    }
                }
                return true;
            }
        }

	    protected virtual bool DoAssignApprover(SourceAssign source, int map)
	    {
            var helper = new EPAssignmentProcessHelper<SourceAssign>(this._Graph);
	        return helper.Assign(source, map) && (source.WorkgroupID != null || source.OwnerID != null);	            
	    }

		public virtual bool ValidateAccess(int? workgroup, Guid? ownerID)
		{
			if (workgroup == null && ownerID == null) return true;

			if (PXAccess.GetUserID() == ownerID) return true;			

			EPCompanyTree wg =
				PXSelect<EPCompanyTree,
					Where<EPCompanyTree.workGroupID, Equal<Required<EPCompanyTree.workGroupID>>,
						And<EPCompanyTree.workGroupID, Owned<Current<AccessInfo.userID>>>>>.SelectWindowed(this._Graph, 0, 1, workgroup);
			return wg != null;
		}

		protected void RegisterActivity(object item, string summary)
		{
			if(_Activity != null)
			{
				PXAdapter adapter = new PXAdapter(new DummyView(this._Graph, new Select<SourceAssign>(), new List<object>{item}));
				adapter.Parameters = new object[] {summary};
				foreach (object r in _Activity.Press(adapter))
				{					
				}
			}
		}
		protected virtual void OnPersisted(PXCache cache, PXRowPersistedEventArgs e)
		{
			if(e.TranStatus == PXTranStatus.Completed && e.Operation != PXDBOperation.Delete)
			{
			   foreach (EPApproval approval in this._Pending.Cache.Inserted)
				{
			        if (approval.NotificationID != null && approval.Status == EPApprovalStatus.Pending /*&& approval.OwnerID != PXAccess.GetUserID()*/)
						{
							try
							{
								this._Pending.Cache.Current = approval;
								var sender = TemplateNotificationGenerator.Create(this._Graph, e.Row, approval.NotificationID.Value);
								sender.LinkToEntity = false;
								sender.MassProcessMode = false;
								sender.Send();
							}
							catch (Exception ex)
							{
								this._Pending.Cache.RaiseExceptionHandling<EPApproval.status>(approval, approval.Status,
									new PXSetPropertyException(Messages.ApprovalNotificationError, PXErrorLevel.Warning, ex.Message));																
							}                     
			        }				    
			    }
			    
			}
		}

		private sealed class DummyView : PXView
		{
			private readonly List<object>_records;
			internal DummyView(PXGraph graph, BqlCommand command, List<object> records)
				: base(graph, true, command)
			{
				_records = records;
			}
			public override List<object> Select(object[] currents, object[] parameters, object[] searches, string[] sortcolumns, bool[] descendings, PXFilterRow[] filters, ref int startRow, int maximumRows, ref int totalRows)
			{
				return _records;
			}
		}
		#endregion
	}	
	#endregion

	#region EPApprovalActionList
	public class EPApprovalAction<SourceAssign, Approved, Rejected> : EPApprovalList<SourceAssign>
		where SourceAssign : class, IAssign,  IBqlTable
		where Approved : class, IBqlField
		where Rejected : class, IBqlField
	{
		#region Ctor
		public EPApprovalAction(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
			Initialize(graph);
		}

		public EPApprovalAction(PXGraph graph)
			: base(graph)
		{
			Initialize(graph);
		}
		private void Initialize(PXGraph graph)
		{
			AddAction(graph, "Approve", Approve);
			AddAction(graph, "Reject" , Reject);		
		}
		#endregion

		public virtual void SetEnabledActions(PXCache sender, SourceAssign row, bool enable)
		{
			if (enable && !IsApprover(row))
				enable = false;

			sender.Graph.Actions["Approve"].SetEnabled(enable);
            sender.Graph.Actions["Reject"].SetEnabled(enable);
		}
		protected virtual void AddAction(PXGraph graph, string name, PXButtonDelegate handler)
		{
			graph.Actions[name] =
				(PXAction)Activator.CreateInstance(
				typeof(PXNamedAction<>).MakeGenericType(
				new Type[] { BqlCommand.GetItemType(SourceNoteID) }),
				new object[] { graph, name, handler }
				);
		}	

		[PXUIField(DisplayName = Messages.Approve)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Complete)]
		protected virtual IEnumerable Approve(PXAdapter adapter)
		{
			PXCache cache = _Graph.Caches[typeof (SourceAssign)];
			foreach (SourceAssign item in adapter.Get<SourceAssign>())
			{
			    if (!this.Approve(item))
                    throw new PXSetPropertyException(PO.Messages.NotApprover);                    
			    
				cache.SetValue<Approved>(item, IsApproved(item));
				yield return cache.Update(item);
			}
		}

		[PXUIField(DisplayName = Messages.Reject)]
		[PXButton(ImageKey = PX.Web.UI.Sprite.Main.Remove)]
		protected virtual IEnumerable Reject(PXAdapter adapter)
		{
			PXCache cache = _Graph.Caches[typeof (SourceAssign)];
			foreach (SourceAssign item in adapter.Get<SourceAssign>())
			{

                if (this.Reject(item))                
                    cache.SetValue<Rejected>(item, true);                                    
                else
                    throw new PXSetPropertyException(PO.Messages.NotApprover);
					
				cache.Update(item);
				yield return cache.Update(item);
			}
		}
	}
	#endregion

    #region EPApprovalAutomation

    interface IAssignedMap
    {
        int? AssignmentMapID { get; set; }
        int? AssignmentNotificationID { get; set; }
        Boolean? IsActive { get; }
    }

	public class EPApprovalAutomation<SourceAssign, Approved, Rejected, Hold, SetupApproval> : EPApprovalList<SourceAssign>
		where SourceAssign : class, IAssign, IBqlTable
		where Approved : class, IBqlField
		where Rejected : class, IBqlField
		where Hold : class, IBqlField
		where SetupApproval : class, IBqlTable, new()
	{
		#region Ctor
		public EPApprovalAutomation(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
			Initialize(graph);
		}

		public EPApprovalAutomation(PXGraph graph)
			: base(graph)
		{
			Initialize(graph);
		}

		private void Initialize(PXGraph graph)
		{
			if (PXSetup<SetupApproval>.Select(graph, null) == null)
			{	
				//Note that the view must be declared before the EPApprovalAutomation
				throw new PXException(PXMessages.LocalizeFormatNoPrefixNLA(EP.Messages.ViewMustBeDeclared, typeof(SetupApproval).Name));
			}
			
			graph.FieldVerifying.AddHandler(BqlCommand.GetItemType(typeof(Approved)), typeof(Approved).Name, Approved_FieldVerifying);
			graph.FieldVerifying.AddHandler(BqlCommand.GetItemType(typeof(Rejected)), typeof(Rejected).Name, Rejected_FieldVerifying);
			graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(typeof(Approved)), typeof(Approved).Name, Approved_FieldUpdated);
			graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(typeof(Rejected)), typeof(Rejected).Name, Rejected_FieldUpdated);
			graph.FieldDefaulting.AddHandler(BqlCommand.GetItemType(typeof(Hold)), typeof(Hold).Name, Hold_FieldDefaulting);
			graph.FieldDefaulting.AddHandler(BqlCommand.GetItemType(typeof(Approved)), typeof(Approved).Name, Approved_FieldDefaulting);
            graph.Initialized += InitLastEvents;
        }

        private void InitLastEvents(PXGraph graph)
        {
            graph.RowUpdated.AddHandler<SourceAssign>(RowUpdated);
        }
		#endregion
		#region Implementation
		public virtual List<ApprovalMap> GetAssignedMaps(SourceAssign doc, PXCache cache)
		{
			PXResultset<SetupApproval> setups = PXSetup<SetupApproval>.SelectMultiBound(cache.Graph, new object[] { doc });
			
			int count = setups.Count;
			var list = new List<ApprovalMap>();
			for (int i = 0; i < count; i++)
			{
				SetupApproval setup = (SetupApproval)setups[i];
				IAssignedMap map = (IAssignedMap)setup;
				if (map.IsActive == true && map.AssignmentMapID != null)
				{
					list.Add( new ApprovalMap(map.AssignmentMapID.Value, map.AssignmentNotificationID));
				}
			}
		    return list;
		}
		protected virtual void Approved_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			if ((bool?)e.NewValue == true && !this.IsApprover((SourceAssign)e.Row))
			{
				if (cache
					.GetAttributesReadonly<Approved>(e.Row)
					.OfType<PXUIFieldAttribute>()
					.Any(attribute => attribute.Visible))
				{
					throw new PXSetPropertyException(Common.Messages.NotApprover);
			}
				else
				{
					throw new PXException(Common.Messages.NotApprover);
		}
			}
		}
		protected virtual void Rejected_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if ((bool?)e.NewValue == true && !this.IsApprover((SourceAssign)e.Row))
			{
				if (sender
					.GetAttributesReadonly<Approved>(e.Row)
					.OfType<PXUIFieldAttribute>()
					.Any(attribute => attribute.Visible))
				{
					PXUIFieldAttribute.SetError<Approved>(sender, e.Row, Common.Messages.NotApprover);
					throw new PXSetPropertyException(Common.Messages.NotApprover);
			}
				else
				{
					throw new PXException(Common.Messages.NotApprover);
		}
			}
		}
		protected virtual void Approved_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			SourceAssign doc = (SourceAssign)e.Row;
			PXFieldState state = (PXFieldState)cache.GetStateExt<Approved>(doc);
			if (e.Row != null &&
			(bool?)(cache.GetValue<Approved>(doc)) == true && (bool?)e.OldValue != true)
			{
				cache.SetValue<Approved>(doc, false);
				if (this.Approve(doc))
				{
					cache.SetValue<Approved>(doc, this.IsApproved(doc));
				}
			}
		}
		protected virtual void Rejected_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			SourceAssign doc = (SourceAssign)e.Row;
			if (!((bool?)cache.GetValue<Rejected>(doc) == true && this.Reject(doc)))
				cache.SetValue<Rejected>(doc, false);
		}
		protected virtual void RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			SourceAssign doc = (SourceAssign)e.Row;
            SourceAssign olddoc = (SourceAssign)e.OldRow;
            if (olddoc == null || doc==null)
                return;
            bool? approved = (bool?)cache.GetValue<Approved>(doc);
			bool? hold = (bool?)cache.GetValue<Hold>(doc);
			bool? oldValue = (bool?)cache.GetValue<Hold>(olddoc);

			if (oldValue != null)
			{
				if (hold == true)
				{
					if (oldValue == false)
					{
						this.ClearPendingApproval(doc);
						cache.SetDefaultExt<Approved>(doc);
						cache.SetDefaultExt<Rejected>(doc);
					}
				}
				else if (oldValue == true && approved != true)
				{
					if (!Assign(doc, GetAssignedMaps(doc, cache)))
					{
						cache.SetValue<Approved>(doc, true);
					}
				}
			}
		}
		protected virtual void Hold_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			SourceAssign doc = (SourceAssign)e.Row;
			if (GetAssignedMaps(doc, cache).Any())
			{
				cache.SetValue<Hold>(doc, true);
			}
		}
		protected virtual void Approved_FieldDefaulting(PXCache cache, PXFieldDefaultingEventArgs e)
		{
			SourceAssign doc = (SourceAssign)e.Row;
			bool? hold = (bool?)cache.GetValue<Hold>(doc);
			if (hold != true && !GetAssignedMaps(doc, cache).Any()) 
			{
				cache.SetValue<Approved>(doc, true);
			}
		}
		#endregion
	}
	#endregion

    #region EPApprovalEnabledFlagAttribute

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EPRequireApprovalAttribute : PXDBBoolAttribute
    {
      
        public override void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {
            base.RowSelecting(sender, e);

            if (!PXAccess.FeatureInstalled<FeaturesSet.approvalWorkflow>())
            {
                sender.SetValue(e.Row, _FieldOrdinal, false);
            }
        }
    }
    

    #endregion

    #region EPAssignmentMapSelector
    public class EPAssignmentMapSelectorAttribute : PXCustomSelectorAttribute
    {
        public EPAssignmentMapSelectorAttribute()
            : base(typeof(EPAssignmentMap.assignmentMapID))
        {
            DescriptionField = typeof(EPAssignmentMap.name);
        }

        public IEnumerable GetRecords()
        {
            foreach (EPAssignmentMap map in PXSelect<EPAssignmentMap>.Select(_Graph))
            {
                if (map.GraphType != null)
                {
                    var node = PXSiteMap.Provider.FindSiteMapNode(PXBuildManager.GetType(map.GraphType,false));
                    if (node != null)
                    {
                        yield return map;
                    }
                }
                else
                {
                    if (map.EntityType != null)
                    {
                        Type entityType = PXBuildManager.GetType(map.EntityType, false);
                        if (entityType != null)
                        {
                            Type graphType = EntityHelper.GetPrimaryGraphType(_Graph, entityType);

                            if (graphType != null)
                            {
                                var node = PXSiteMap.Provider.FindSiteMapNode(graphType);
                                if (node != null)
                                {
                                    yield return map;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region DayOfWeekAttribute

    /// <summary>
	/// List days of week.
	/// </summary>
	/// <example>
	/// [DayOfWeek]
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class DayOfWeekAttribute : PXIntListAttribute
	{
		public DayOfWeekAttribute()
			: base(new[] { (int)DayOfWeek.Sunday, 
			               (int)DayOfWeek.Monday, 
			               (int)DayOfWeek.Tuesday, 
			               (int)DayOfWeek.Wednesday, 
			               (int)DayOfWeek.Thursday, 
			               (int)DayOfWeek.Friday, 
			               (int)DayOfWeek.Saturday },
			       new[] { GetDayName(DayOfWeek.Sunday), 
			               GetDayName(DayOfWeek.Monday), 
			               GetDayName(DayOfWeek.Tuesday), 
			               GetDayName(DayOfWeek.Wednesday), 
			               GetDayName(DayOfWeek.Thursday), 
			               GetDayName(DayOfWeek.Friday), 
			               GetDayName(DayOfWeek.Saturday) })
		{
		}

		private static string GetDayName(DayOfWeek day)
		{
			return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(day);
		}
	}

	#endregion

	#region WorkTimeRemindDateAttribute

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class WorkTimeRemindDateAttribute : PXRemindDateAttribute
	{
		private readonly Type _activityEmployeeBqlField;
		private int _activityEmployeeFieldOrigin;
		private PXGraph _graph;

		public WorkTimeRemindDateAttribute(Type isReminderOnBqlField, Type startDateBqlField, Type activityEmployeeBqlField) 
			: base(isReminderOnBqlField, startDateBqlField)
		{
			_activityEmployeeBqlField = activityEmployeeBqlField;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			_activityEmployeeFieldOrigin = sender.GetFieldOrdinal(sender.GetField(_activityEmployeeBqlField));
			_graph = sender.Graph;
		}

		protected override DateTime? CalcCorrectValue(PXCache sender, object row)
		{
			_reversedRemindAt = 0;
			DateTime? result = base.CalcCorrectValue(sender, row);
			if (result != null && object.Equals(true, sender.GetValue(row, _isReminderOnFieldOrigin)))
			{
				var searchedCalendar = PXSelectJoin<CSCalendar,
					InnerJoin<EPEmployee, On<EPEmployee.calendarID, Equal<CSCalendar.calendarID>>>,
					Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>.
					Select(_graph, sender.GetValue(row, _activityEmployeeFieldOrigin));
				if (searchedCalendar != null && searchedCalendar.Count != 0)
				{
					var calendar = (CSCalendar)searchedCalendar[0][typeof(CSCalendar)];
					var calendarTimeZone = string.IsNullOrEmpty(calendar.TimeZone) ? 
						PXTimeZoneInfo.Invariant :
						PXTimeZoneInfo.FindSystemTimeZoneById(calendar.TimeZone);
					var addTicks = 0L;
					switch (result.Value.DayOfWeek)
					{
						case DayOfWeek.Sunday:
							if (calendar.SunWorkDay == true) addTicks = calendar.SunStartTime.Value.TimeOfDay.Ticks;
							break;
						case DayOfWeek.Monday:
							if (calendar.MonWorkDay == true) addTicks = calendar.MonStartTime.Value.TimeOfDay.Ticks;
							break;
						case DayOfWeek.Tuesday:
							if (calendar.TueWorkDay == true) addTicks = calendar.TueStartTime.Value.TimeOfDay.Ticks;
							break;
						case DayOfWeek.Wednesday:
							if (calendar.WedWorkDay == true) addTicks = calendar.WedStartTime.Value.TimeOfDay.Ticks;
							break;
						case DayOfWeek.Thursday:
							if (calendar.ThuWorkDay == true) addTicks = calendar.ThuStartTime.Value.TimeOfDay.Ticks;
							break;
						case DayOfWeek.Friday:
							if (calendar.FriWorkDay == true) addTicks = calendar.FriStartTime.Value.TimeOfDay.Ticks;
							break;
						case DayOfWeek.Saturday:
							if (calendar.SatWorkDay == true) addTicks = calendar.SatStartTime.Value.TimeOfDay.Ticks;
							break;
					}
					var utcTime = result.Value.Date.AddTicks(addTicks - calendarTimeZone.BaseUtcOffset.Ticks);
					result = PXTimeZoneInfo.ConvertTimeFromUtc(utcTime, LocaleInfo.GetTimeZone());
				}
			}
			return result;
		}
	}

	#endregion

	#region EmployeeRawAttribute

	/// <summary>
	/// 'EMPLOYEE' dimension selector.
	/// </summary>
	/// <example>
	/// [EmployeeRaw]
	/// </example>
	public class EmployeeRawAttribute : AcctSubAttribute
	{
		#region EmployeeLogin

		[Serializable]
		[PXBreakInheritance]
        [PXHidden]
		public sealed class EmployeeLogin : Users
		{
			#region PKID

			public new abstract class pKID : IBqlField { }

			#endregion

			#region Username

			public new abstract class username : IBqlField { }

			[PXDBString(64, IsKey = true, IsUnicode = true)]
			[PXUIField(DisplayName = "Username", Visible = false)]
			public override string Username
			{
				get
				{
					return base.Username;
				}
				set
				{
					base.Username = value;
				}
			}

			#endregion

		}

		#endregion

		public const string DimensionName = "EMPLOYEE";

		public EmployeeRawAttribute()
		{
			Type searchType = typeof(Search2<EPEmployee.acctCD, 
				LeftJoin<EmployeeLogin, On<EmployeeLogin.pKID, Equal<EPEmployee.userID>>,
				InnerJoin<GL.Branch, On<GL.Branch.bAccountID, Equal<EPEmployee.parentBAccountID>>,
				LeftJoin<EPEmployeePosition, On<EPEmployeePosition.employeeID, Equal<EPEmployee.bAccountID>, And<EPEmployeePosition.isActive, Equal<True>>>>>>,
				Where<MatchWithBranch<GL.Branch.branchID>>>);

			PXDimensionSelectorAttribute attr;
			_Attributes.Add(attr = new PXDimensionSelectorAttribute(DimensionName, searchType, typeof(EPEmployee.acctCD),
									typeof(EPEmployee.bAccountID), typeof(EPEmployee.acctCD), typeof(EPEmployee.acctName),
									typeof(EPEmployee.status), typeof(EPEmployeePosition.positionID), typeof(EPEmployee.departmentID),
									typeof(EPEmployee.defLocationID), typeof(EmployeeLogin.username), typeof(EmployeeLogin.displayName)));
			attr.DescriptionField = typeof(EPEmployee.acctName);
			_SelAttrIndex = _Attributes.Count - 1;
			this.Filterable = true;
		}
	}

	#endregion

	#region EPViewStatusAttribute

	public class EPViewStatusAttribute : PXIntListAttribute
	{
		public const int NOTVIEWED = 0;
		public const int VIEWED = 1;

		public sealed class NotViewed : Constant<int>
		{
			public NotViewed() : base(NOTVIEWED) { }
		}

		public sealed class Viewed : Constant<int>
		{
			public Viewed() : base(VIEWED) { }
		}

		public EPViewStatusAttribute() : base(
			new[] { NOTVIEWED, VIEWED },
            new[] { Messages.EntityIsNotViewed, Messages.EntityIsViewed })
        { }
	}

	#endregion

	#region CurrentEmployeeByDefaultAttribute

	/// <summary>
	/// Allow determine current logined employee on 'field default' event.
	/// </summary>
	/// <example>
	/// [CurrentEmployeeByDefault]
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class CurrentEmployeeByDefaultAttribute : PXDefaultAttribute
	{
		public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var employee = EmployeeMaint.GetCurrentEmployee(sender.Graph);
			if (employee != null && employee.TimeCardRequired == true)
				e.NewValue = employee.BAccountID;
		}
	}

	#endregion

	#region PXWeekSelectorAttribute

	/// <summary>
	/// Allow select weeks.</br>
	/// Shows start and end date of week.
	/// </summary>
	/// <example>
	/// [PXWeekSelector]
	/// </example>
    [Serializable]
	public class PXWeekSelectorAttribute : PXCustomSelectorAttribute, IPXFieldDefaultingSubscriber
	{
		[PXVirtual]
        [Serializable]
        [PXHidden]
		public partial class EPWeek : IBqlTable
		{
			#region WeekID

			public abstract class weekID : IBqlField { }

			[PXDBInt(IsKey = true)]
			[PXUIField(Visible = false)]
			public virtual Int32? WeekID { get; set; }

			#endregion

			#region FullNumber

			public abstract class fullNumber : IBqlField { }

			private String _fullNumber;
			[PXString]
			[PXUIField(DisplayName = "Week")]
			public virtual String FullNumber
			{
				get
				{
					Initialize();
					return _fullNumber;
				}
			}

			#endregion

			#region Number

			public abstract class number : IBqlField { }

			private Int32? _number;
			[PXInt]
			[PXUIField(DisplayName = "Number", Visible = false)]
			public virtual Int32? Number
			{
				get
				{
					Initialize();
					return _number;
				}
			}

			#endregion

			#region StartDate

			public abstract class startDate : IBqlField { }

			private DateTime? _startDate;
			[PXDate]
			[PXUIField(DisplayName = "Start")]
			public virtual DateTime? StartDate
			{
				get
				{
					Initialize();
					return _startDate;
				}
			}

			#endregion

			#region EndDate

			public abstract class endDate : IBqlField { }

			private DateTime? _endDate;
			[PXDate]
			[PXUIField(DisplayName = "End", Visibility = PXUIVisibility.Visible)]
			public virtual DateTime? EndDate
			{
				get
				{
					Initialize();
					return _endDate;
				}
			}

			#endregion

			#region Description

			public abstract class description : IBqlField { }

			[PXString]
			[PXUIField(DisplayName = "Description")]
			public virtual String Description
			{
				[PXDependsOnFields(typeof(fullNumber),typeof(startDate),typeof(endDate))]
				get
				{
                   
				    return string.Format("{0} ({1:MM/dd} - {2:MM/dd})", FullNumber, StartDate, EndDate);
				}
			}

			#endregion

			#region ShortDescription

			public abstract class shortDescription : IBqlField { }

			[PXString]
			[PXUIField(DisplayName = "Description", Visible = false)]
			public virtual String ShortDescription
			{
                [PXDependsOnFields(typeof(fullNumber), typeof(startDate), typeof(endDate))]
				get
				{
                    return string.Format("{0} ({1:MM/dd} - {2:MM/dd})",FullNumber , StartDate, EndDate);
				}
			}

			#endregion

			private void Initialize()
			{
				if (_number != null && _fullNumber != null && 
					_startDate != null && _endDate != null || WeekID == null)
				{
					return;
				}

				var weekId = (int)WeekID;
				var year = weekId / 100;
				var week = weekId % 100;
				_number = week;
				_fullNumber = string.Format("{0}-{1:00}", year, week);

				var startDate = PX.Data.EP.PXDateTimeInfo.GetWeekStart(year, week);
				_startDate = startDate;
				_endDate = startDate.AddDays(7).AddTicks(-1L);
			}
		}

		private readonly Type _startDateBqlField;
		private readonly Type _timeSpentBqlField;

		private readonly bool _limited;
		private int _startDateOrdinal;
		private int _timeSpentOrdinal;

		public PXWeekSelectorAttribute()
			: base(typeof(EPWeek.weekID), 
				typeof(EPWeek.number), typeof(EPWeek.startDate), typeof(EPWeek.endDate))
		{
			
		}


		public PXWeekSelectorAttribute(Type type, Type[] fieldList)
			: base(type, fieldList)
		{
			
		}

		public PXWeekSelectorAttribute(Type startDateBqlField, Type timeSpentBqlField)
			: this()
		{
			_startDateBqlField = startDateBqlField;
			_timeSpentBqlField = timeSpentBqlField;
			_limited = true;
		}

		public override void CacheAttached(PXCache sender)
		{
            base.EmitColumnForDescriptionField(sender);

			base.CacheAttached(sender);

			if (_limited)
			{
				_startDateOrdinal = sender.GetFieldOrdinal(sender.GetField(_startDateBqlField));
				_timeSpentOrdinal = sender.GetFieldOrdinal(sender.GetField(_timeSpentBqlField));
			}
		}

        public override void DescriptionFieldCommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
        {
            if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Select &&
            (e.Operation & PXDBOperation.Option) == PXDBOperation.External &&
            (e.Value == null || e.Value is string))
            {
                e.Cancel = true;
                e.FieldName = BqlCommand.Null;
                e.DataLength = null;
                e.DataType = PXDbType.Unspecified;
                e.DataValue = null;
            }
        }

        protected IEnumerable GetRecords()
		{
			if (!_limited) return GetAllRecords();

			var cache = _Graph.Caches[_CacheType];
			var startDate = cache.GetValue(cache.Current, _startDateOrdinal);
			return GetRecordsByDate((DateTime?)startDate);
		}

		protected virtual IEnumerable GetAllRecords()
		{
			var pageSize = PXView.MaximumRows;
			if (pageSize < 1) pageSize = 52;

			var startIndex = PXView.StartRow;
			/*var pageNumber = pageSize / */
			PXView.StartRow = 0;

			int year = -1;
			int week = -1;
			if (PXView.Searches != null && PXView.Searches.Length > 0)
				for (int i = 0; i < PXView.Searches.Length; i++)
				{
					var name = PXView.SortColumns[i].With(_ => _.ToLower());
					if (string.IsNullOrEmpty(name)) continue;

					var searchValue = PXView.Searches[i];
					if (searchValue == null) continue;

					if (typeof(EPWeek.weekID).Name.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						var weekId = searchValue is int ? (int)searchValue : int.Parse((string)searchValue);
						year = weekId / 100;
						week = weekId % 100;
						break;
					}

					var str = searchValue as string;
					if (str != null && typeof(EPWeek.fullNumber).Name.Equals(name, StringComparison.OrdinalIgnoreCase) && str.Length > 6)
					{
						year = int.Parse(str.Substring(0, 4));
						week = int.Parse(str.Substring(5, 2));
						break;
					}
				}

			if ((year == -1 || week == -1) && PXView.Filters != null && PXView.Filters.Length > 0)
			{
				var condArr = new[]
								{
									PXCondition.EQ, PXCondition.BETWEEN, PXCondition.GE, PXCondition.IN,
									PXCondition.LE, PXCondition.LLIKE, PXCondition.LIKE, PXCondition.RLIKE
								};
				foreach (PXFilterRow item in PXView.Filters)
				{
					var name = item.DataField;
					if (string.IsNullOrEmpty(name)) continue;
					var searchValue = item.Value;
					if (searchValue == null) continue;
					var condition = item.Condition;
					if (Array.IndexOf(condArr, condition) < 0) continue;

					if (typeof(EPWeek.weekID).Name.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						var weekId = searchValue is int ? (int)searchValue : int.Parse((string)searchValue);
						year = weekId / 100;
						week = weekId % 100;
						break;
					}

					var str = searchValue as string;
					if (str != null && typeof(EPWeek.fullNumber).Name.Equals(name, StringComparison.OrdinalIgnoreCase) && str.Length > 6)
					{
						year = int.Parse(str.Substring(0, 4));
						week = int.Parse(str.Substring(5, 2));
						break;
					}
				}
			}

			if (year == -1 || week == -1)
			{
				var toDay = PXTimeZoneInfo.Now.Date;
				year = toDay.Year;
				week = PX.Data.EP.PXDateTimeInfo.GetWeekNumber(toDay);
			}

			var currentDate = PX.Data.EP.PXDateTimeInfo.GetWeekStart(year, week).AddDays(startIndex * 7);
			if (currentDate.Year < 1901) yield break;

			var currentWeek = PX.Data.EP.PXDateTimeInfo.GetWeekNumber(currentDate);
			int currentYear = currentDate.Year;

			if (currentWeek >52 )
			{
				if (currentDate.Day > 28)
				{
					currentYear++;
					currentWeek = 1;
				}
			}

			
			for (int i = 0; i < pageSize; i++)
			{
				yield return new EPWeek { WeekID = currentYear * 100 + currentWeek };

				currentDate = currentDate.AddDays(7D);
				if (currentWeek < 52) 
					currentWeek++;
				else
				{
					currentWeek = PX.Data.EP.PXDateTimeInfo.GetWeekNumber(currentDate);
					if (currentWeek == 1)
					{
						currentYear++;
					}
				}

				if (currentDate.Year > 9998) yield break;
			}
		}

		public static IEnumerable GetRecordsByDate(DateTime? startDate)
		{
			if (startDate == null) yield break;

			var date = (DateTime)startDate;
			var dateWeek = PX.Data.EP.PXDateTimeInfo.GetWeekNumber(date);
			var utcDate = PXTimeZoneInfo.ConvertTimeToUtc(date, LocaleInfo.GetTimeZone());
			var utcDateWeek = PX.Data.EP.PXDateTimeInfo.GetWeekNumber(utcDate);
			if (dateWeek != utcDateWeek)
			{
				if (date > utcDate)
				{
					yield return new EPWeek { WeekID = utcDate.Year * 100 + utcDateWeek };
					yield return new EPWeek { WeekID = date.Year * 100 + dateWeek };
				}
				else
				{
					yield return new EPWeek { WeekID = date.Year * 100 + dateWeek };
					yield return new EPWeek { WeekID = utcDate.Year * 100 + utcDateWeek };
				}
			}
			else
				yield return new EPWeek { WeekID = date.Year * 100 + dateWeek };
		}

		public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (_limited)
			{
				var oldCurrent = sender.Current;
				sender.Current = e.Row;
				base.FieldVerifying(sender, e);
				sender.Current = oldCurrent;
			}
		}

		public void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (!_limited) return;

			var startDate = sender.GetValue(e.Row, _startDateOrdinal);
			if (startDate != null)
			{
				var date = (DateTime)startDate;
				e.NewValue = GetWeekID(date);
			}
		}

		public static int GetWeekID(DateTime date)
		{
			int year = date.Year;
			int week = PX.Data.EP.PXDateTimeInfo.GetWeekNumber(date.Date);

			if (week >= 52 && date.Month == 1)
			{
				year--;
			}
			
			if (week == 1 && date.Month == 12)
			{
				year++;
			}
			
			return year * 100 + week;
		}

		public static DateTime GetWeekStartDate(int weekId)
		{
			var year = weekId / 100;
			var weekNumber = weekId % 100;
			return PX.Data.EP.PXDateTimeInfo.GetWeekStart(year, weekNumber);
		}
	}

	#endregion

	#region PXWeekSelector2Attribute

	/// <summary>
	/// Allow select weeks.</br>
	/// Shows start and end date of week, fixed diapason only.
	/// </summary>
	/// <example>
	/// [PXWeekSelector2]
	/// </example>
	[Serializable]
	public class PXWeekSelector2Attribute : PXWeekSelectorAttribute
	{
		public class FullWeekList
		{

			private class Definition : IPrefetchable
			{
				private DateTime _stratDay = new DateTime(2005, 1, 1);
				private const int weekListCount = 800;
				private List<EPWeekRaw> _weeks = new List<EPWeekRaw>(weekListCount);

				public void Prefetch()
				{
					DateTime curentDay = _stratDay;
					for (int i = 0; i < weekListCount; i++)
					{
						int year;
						if (curentDay.AddDays(-3).Year < curentDay.Year && PX.Data.EP.PXDateTimeInfo.GetWeekNumber(curentDay) > 1)
							year = curentDay.AddYears(-1).Year;
						else
							year = curentDay.Year;

						EPWeek epWeek = new EPWeek { WeekID = year * 100 + PX.Data.EP.PXDateTimeInfo.GetWeekNumber(curentDay) };
						EPWeekRaw rawWeek = EPWeekRaw.ToEPWeekRaw(epWeek);
						_weeks.Add(rawWeek);
						curentDay = curentDay.AddDays(7);
					}
				}

				public static Definition Get()
				{
					return PXDatabase.GetSlot<Definition>(typeof(FullWeekList).Name, typeof(EPSetup));
				}

				public List<EPWeekRaw> Weeks
				{
					get { return _weeks; }
				}
			}

			public static List<EPWeekRaw> Weeks()
			{
				var def = Definition.Get();
				if (def == null) return new List<EPWeekRaw> { };
				return def.Weeks;
			}

		}

		public PXWeekSelector2Attribute() : base(typeof (EPWeekRaw.weekID),
		                                         new Type[]
													{
														typeof (EPWeekRaw.fullNumber),
														typeof (EPWeekRaw.startDate),
														typeof (EPWeekRaw.endDate)
													}
												)
		{
		}

		protected override IEnumerable GetAllRecords()
		{
			if (IsCustomWeek(_Graph))
			{
				var res = (PXResultset<EPCustomWeek>) PXSelect<EPCustomWeek>.Select(_Graph, null);
				return res.Select(_ => EPWeekRaw.ToEPWeekRaw(_)).ToList();
			}
			else
				return FullWeekList.Weeks();
		}

		public static bool IsCustomWeek(PXGraph graph)
		{
			EPSetup setup;
			if (graph.Caches.ContainsKey(typeof(EPSetup)) && graph.Caches[typeof(EPSetup)].Current != null)
				setup = graph.Caches[typeof(EPSetup)].Current as EPSetup;
			else
				setup = PXSetup<EPSetup>.SelectSingleBound(graph, null);
			return setup != null && setup.CustomWeek == true;
		}

		private static EPCustomWeek GetCustomWeek(PXGraph graph, DateTime date)
		{
			EPCustomWeek customWeek = PXSelect<EPCustomWeek, Where<Required<EPCustomWeek.startDate>, Between<EPCustomWeek.startDate, EPCustomWeek.endDate>>>.SelectSingleBound(graph, null, date);
			if (customWeek == null)
				throw new PXException(Messages.CustomWeekNotFoundByDate, date);
			return customWeek;
		}

		private static EPCustomWeek GetCustomWeek(PXGraph graph, int weekID)
		{
			EPCustomWeek customWeek = PXSelect<EPCustomWeek, Where<EPCustomWeek.weekID, Equal<Required<EPCustomWeek.weekID>>>>.SelectSingleBound(graph, null, weekID);
			if (customWeek == null)
				throw new PXException(Messages.CustomWeekNotFound);
			return customWeek;
		}

        private static EPWeekRaw GetStandartWeek(PXGraph graph, DateTime date)
        {
            EPWeek epWeek = new EPWeek { WeekID = date.Year * 100 + PX.Data.EP.PXDateTimeInfo.GetWeekNumber(date) };
            return EPWeekRaw.ToEPWeekRaw(epWeek);
        }

		public static int GetWeekID(PXGraph graph, DateTime date)
		{
			if (IsCustomWeek(graph))
				return GetCustomWeek(graph, date).WeekID.Value;
			else
				return PXWeekSelectorAttribute.GetWeekID(date);
		}

		public static int GetNextWeekID(PXGraph graph, int weekID)
		{
			if (IsCustomWeek(graph))
				return GetWeekID(graph, GetCustomWeek(graph, weekID).EndDate.Value.AddDays(1d));
			else
                return PXWeekSelectorAttribute.GetWeekID(PXWeekSelectorAttribute.GetWeekStartDate(weekID).AddDays(7d));
		}

		public static int GetNextWeekID(PXGraph graph, DateTime date)
		{
			if (IsCustomWeek(graph))
				return GetWeekID(graph, GetCustomWeek(graph, date).EndDate.Value.AddDays(1d));
			else
				return PXWeekSelectorAttribute.GetWeekID(date.AddDays(7));
		}

        public static DateTime GetWeekStartDate(PXGraph graph, int weekId)
		{
			if (IsCustomWeek(graph))
				return GetCustomWeek(graph, weekId).StartDate.Value;
			else
                return PXWeekSelectorAttribute.GetWeekStartDate(weekId);
		}

        public static DateTime GetWeekEndDate(PXGraph graph, int weekId)
        {
            if (IsCustomWeek(graph))
                return GetCustomWeek(graph, weekId).EndDate.Value;
            else
                return PXWeekSelectorAttribute.GetWeekStartDate(weekId).AddDays(6d);
        }


        public class WeekInfo
        {
            private Dictionary<DayOfWeek, DayInfo> _days = new Dictionary<DayOfWeek, DayInfo>();

            public DayInfo Mon { get { return GetDayInfo(DayOfWeek.Monday); } }
            public DayInfo Tue { get { return GetDayInfo(DayOfWeek.Tuesday); } }
            public DayInfo Wed { get { return GetDayInfo(DayOfWeek.Wednesday); } }
            public DayInfo Thu { get { return GetDayInfo(DayOfWeek.Thursday); } }
            public DayInfo Fri { get { return GetDayInfo(DayOfWeek.Friday); } }
            public DayInfo Sat { get { return GetDayInfo(DayOfWeek.Saturday); } }
            public DayInfo Sun { get { return GetDayInfo(DayOfWeek.Sunday); } }

            private DayInfo GetDayInfo(DayOfWeek mDay)
            {
                if (_days.ContainsKey(mDay))
                    return _days[mDay];
                else
                    return new DayInfo(null);
            }

            public void AddDayInfo(DateTime date)
            {
                _days[date.DayOfWeek] = new DayInfo(date);
            }

	        public Dictionary<DayOfWeek, DayInfo> Days 
			{
				get { return _days; }
	        }

	        public bool IsValid(DateTime date)
            {
                foreach (DayInfo info in _days.Values)
                {
                    if (info.Enabled && info.Date.Value.Date == date.Date.Date)
                        return true;
                }

                return false;
            }
        }

        public class DayInfo
        {
            public DayInfo(DateTime? date)
            {
                _date = date;
            }

            private DateTime? _date;
            public DateTime? Date { get { return _date; } }
            public bool Enabled { get { return (_date != null); } }
        }

        public static WeekInfo GetWeekInfo(PXGraph graph, int weekId)
        {
            WeekInfo ret = new WeekInfo();
            for (DateTime date = GetWeekStartDate(graph, weekId); date <= GetWeekEndDate(graph, weekId); date = date.AddDays(1))
            {
                ret.AddDayInfo(date);
            }
            return ret;
        }

	}

	#endregion

	#region EPActivityDefaultWeekAttribute
	public class EPActivityDefaultWeekAttribute : PXDefaultAttribute
	{
		private readonly Type dateField;

		public EPActivityDefaultWeekAttribute(Type _dateField): base()
		{
			dateField = _dateField;
			this.PersistingCheck = PXPersistingCheck.Nothing;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			if (!typeof(PMTimeActivity).IsAssignableFrom(sender.GetItemType()) && !typeof(CRPMTimeActivity).IsAssignableFrom(sender.GetItemType()))
				throw new PXArgumentException(EP.Messages.IncorrectUsingAttribute, new object[] {GetType().Name, typeof(PMTimeActivity).Name});
		}

		public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			var field = sender.Graph.Caches[sender.GetItemType()].GetBqlField(dateField.Name);
			
			var dateValue = sender.GetValue(e.Row, field.Name);
			
			if (e.Row != null && dateValue != null)
			{
				if (PXWeekSelector2Attribute.IsCustomWeek(sender.Graph))
				{
					try
					{
						e.NewValue = PXWeekSelector2Attribute.GetWeekID(sender.Graph, (DateTime)dateValue);
		}
					catch (PXException exception)
					{
						sender.RaiseExceptionHandling(FieldName, e.Row, true, exception);
					}
				}
				else
					e.NewValue = PXWeekSelector2Attribute.GetWeekID(sender.Graph, (DateTime)dateValue);
			}
			else
				e.NewValue = null;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			var field = sender.Graph.Caches[sender.GetItemType()].GetBqlField(dateField.Name);

			var dateValue = sender.GetValue(e.Row, field.Name);
			var weekValue = sender.GetValue(e.Row, FieldName);

			if (e.Row != null && dateValue != null && weekValue == null && PXWeekSelector2Attribute.IsCustomWeek(sender.Graph))
			{
				try
				{
					PXWeekSelector2Attribute.GetWeekID(sender.Graph, (DateTime)dateValue);
				}
				catch (PXException exception)
				{
					sender.RaiseExceptionHandling(FieldName, e.Row, true, exception);
				}
			}

			base.RowPersisting(sender, e);
		}

	}
	#endregion


	#region EPActivityProjectDefaultAttribute
	public class EPActivityProjectDefaultAttribute : PM.ProjectDefaultAttribute
	{
		private readonly Type isBillableField;

		public EPActivityProjectDefaultAttribute(Type _isBillableField = null)
			: base(BatchModule.EP, typeof(Search<PM.PMProject.contractID, Where<PM.PMProject.nonProject, Equal<True>>>))
		{
			if (_isBillableField != null)
				isBillableField = _isBillableField;
		}

		public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			bool? isBillable = null;

			if (isBillableField != null)
				isBillable = (bool?)sender.GetValue(e.Row, isBillableField.Name);
			
			if (e.Row == null || isBillable != true || !PM.ProjectAttribute.IsPMVisible(sender.Graph, BatchModule.EP)) 
				base.FieldDefaulting(sender, e);
		}

	}
	#endregion

	#region EPActiveProjectAttribute
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible)]
	[PXAttributeFamily(typeof(AcctSubAttribute))]
	public class EPProjectAttribute : AcctSubAttribute
	{
		protected Type OwnerFieldType; 
		public EPProjectAttribute(Type ownerFieldType)
		{
			this.OwnerFieldType = ownerFieldType;

			Type searchType = BuildSearchClause();

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, searchType, typeof(PMProject.contractCD),
			typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));

			select.DescriptionField = typeof(PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			/*Visible =*/
			Enabled = ProjectAttribute.IsPMVisible(sender.Graph, BatchModule.EP);
		}

		protected virtual Type BuildSearchClause()
		{
			Type SearchType =
				BqlCommand.Compose(
				typeof(Search2<,,,>),
				typeof(PMProject.contractID),
				BuildJoinClause(),
				BuildWhereClause(),
				BuildOrderByClause());

			return SearchType;
		}

		protected virtual Type BuildJoinClause()
		{
			Type staticJoinType = typeof(LeftJoin<EPEmployeeContract, On<EPEmployeeContract.contractID, Equal<PMProject.contractID>, And<EPEmployeeContract.employeeID, Equal<EPEmployee.bAccountID>>>>);

			Type JoinType =
				BqlCommand.Compose(
				typeof(LeftJoin<,,>),
				typeof(EPEmployee),
				typeof(On<,>),
				typeof(EPEmployee.userID),
				typeof(Equal<>),
				typeof(Current2<>),
				OwnerFieldType,
				staticJoinType);

			return JoinType;
		}

		protected virtual Type BuildWhereClause()
		{
			return typeof(Where<PMProject.isTemplate, Equal<False>,
				And<PMProject.isCancelled, NotEqual<True>,
				And<PMProject.isCompleted, NotEqual<True>,
				And<PMProject.baseType, Equal<PMProject.ProjectBaseType>,
				And2<Where<PMProject.restrictToEmployeeList, Equal<False>, Or<EPEmployeeContract.employeeID, IsNotNull>>,
				And2<Match<Current<AccessInfo.userName>>,
				And<Where<PMProject.visibleInEP, Equal<True>, Or<PMProject.nonProject, Equal<True>>>>>>>>>>);
		}

		protected virtual Type BuildOrderByClause()
		{
			return typeof(OrderBy<Desc<PMProject.contractCD>>);
		}
	}

	#endregion

	#region RateTypesAttribute
	public class RateTypesAttribute : PXStringListAttribute
	{
		public const string Hourly = "H";
		public const string Salary = "S";
		public const string SalaryWithExemption = "E";

		public RateTypesAttribute()
			: base(
			new[] { Hourly, Salary, SalaryWithExemption },
            new[] { Messages.Hourly, Messages.Salary, Messages.SalaryWithExemption })
        { }

		public class hourly : Constant<string>
		{
			public hourly() : base(Hourly) { }
		}

		public class salary : Constant<string>
		{
			public salary() : base(Salary) { }
		}

		public class salaryWithExemption : Constant<string>
		{
			public salaryWithExemption() : base(SalaryWithExemption) { }
		}
	}
	#endregion

	#region FilterHeaderDescriptionAttribute

	public sealed class FilterHeaderDescriptionAttribute : PXDACDescriptionAttribute
	{
		public FilterHeaderDescriptionAttribute()
			: base(typeof(FilterHeader), new PXPrimaryGraphAttribute(typeof(PX.Objects.CS.CSFilterMaint)))
		{ }
	}

	#endregion

	#region PXDBTimeSpanAttribute
	[Obsolete("Will be removed in 7.0 version")]
	public class PXDBTimeSpan2Attribute : PXDBIntAttribute
    {
        #region State
        protected string _InputMask = "HH:mm";
        protected string _DisplayMask = "HH:mm";
        protected new DateTime? _MinValue;
        protected new DateTime? _MaxValue;

        public string InputMask
        {
            get
            {
                return _InputMask;
            }
            set
            {
                _InputMask = value;
            }
        }
        public string DisplayMask
        {
            get
            {
                return _DisplayMask;
            }
            set
            {
                _DisplayMask = value;
            }
        }
        public new string MinValue
        {
            get
            {
                if (_MinValue != null)
                {
                    return _MinValue.ToString();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    _MinValue = DateTime.Parse(value);
                }
                else
                {
                    _MinValue = null;
                }
            }
        }
        public new string MaxValue
        {
            get
            {
                if (_MaxValue != null)
                {
                    return _MaxValue.ToString();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    _MaxValue = DateTime.Parse(value);
                }
                else
                {
                    _MaxValue = null;
                }
            }
        }

        public const string Zero = "00:00";
        public sealed class zero : Constant<string> { public zero() : base(Zero) { } }
        #endregion

        #region Ctor
        public PXDBTimeSpan2Attribute()
        {
        }
        #endregion

        #region Initialization
        #endregion

        #region Implementation
        public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            if (_AttributeLevel == PXAttributeLevel.Item || e.IsAltered)
            {
                e.ReturnState = PXDateState.CreateInstance(e.ReturnState, _FieldName, _IsKey, null, _InputMask, _DisplayMask, _MinValue, _MaxValue);
            }

            if (e.ReturnValue != null && (e.ReturnValue is int || e.ReturnValue is int?))
            {
                int minutes = (int) e.ReturnValue;

                TimeSpan span = new TimeSpan(0, 0, Math.Abs(minutes), 0);

                //if (minutes < 0)
                //{
                //    e.ReturnValue 
                //}
                //else
                //{
                    
                //}
                e.ReturnValue = new DateTime(1900, 1, 1).Add(span);
                //e.ReturnValue = new DateTime(1900, 1, 1, span.Hours, span.Minutes, 0);
            }
        }

        public override void FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null || e.NewValue is int)
            {
            }
            else if (e.NewValue is string)
            {
                DateTime val;
                if (DateTime.TryParse((string)e.NewValue, sender.Graph.Culture, DateTimeStyles.None, out val))
                {
                    TimeSpan span = new TimeSpan(val.Hour, val.Minute, 0);
                    e.NewValue = (int)span.TotalMinutes;
                }
                else
                {
                    e.NewValue = null;
                }
            }
            else if (e.NewValue is DateTime)
            {
                DateTime val = (DateTime)e.NewValue;
                TimeSpan span = new TimeSpan(val.Hour, val.Minute, 0);
                e.NewValue = (int)span.TotalMinutes;
            }
        }
        #endregion

        public static DateTime FromMinutes(int minutes)
        {
            TimeSpan span = new TimeSpan(0, 0, minutes, 0);
            return new DateTime(1900, 1, 1).Add(span);
        }
    }
    #endregion

	/// <summary>
	/// Time is displayed and modified in the timezone of the user/Employee.
	/// </summary>
	public class EPDBDateAndTimeAttribute : PXDBDateAndTimeAttribute
	{
		protected Type typeUserID;
		protected PXTimeZoneInfo timezone;
		
		public EPDBDateAndTimeAttribute(Type userID)
		{
			this.typeUserID = userID;
		}
		
		public override void CommandPreparing(PXCache sender, PXCommandPreparingEventArgs e)
		{
			InitTimeZone(sender, e.Row);

			base.CommandPreparing(sender, e);
		}
		public override void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			using (new PXConnectionScope())
			{
				InitTimeZone(sender, e.Row);
			}

			base.RowSelecting(sender, e);
		}

		protected virtual void InitTimeZone(PXCache sender, object row)
		{
			if (row == null) return;

			Guid? userID = (Guid?)sender.GetValue(row, sender.GetField(typeUserID));
			if (userID != null)
			{
				UserPreferences pref = PXSelect<UserPreferences, Where<UserPreferences.userID, Equal<Required<UserPreferences.userID>>>>.Select(sender.Graph, userID);
				if (pref != null && !string.IsNullOrEmpty(pref.TimeZone))
				{
					timezone = PXTimeZoneInfo.FindSystemTimeZoneById(pref.TimeZone);
					return;
				}
				EPEmployee employee = PXSelect<EPEmployee, Where<EPEmployee.userID, Equal<Required<EPEmployee.userID>>>>.Select(sender.Graph, userID);
				if (employee != null && employee.CalendarID != null)
				{
					CSCalendar cal = PXSelect<CSCalendar, Where<CSCalendar.calendarID, Equal<Required<CSCalendar.calendarID>>>>.Select(sender.Graph, employee.CalendarID);
					if (cal != null && !string.IsNullOrEmpty(cal.TimeZone))
					{
						timezone = PXTimeZoneInfo.FindSystemTimeZoneById(cal.TimeZone);
					}
				}
			}
		}

		protected override PXTimeZoneInfo GetTimeZone()
		{
			if (timezone != null)
				return timezone;

			return base.GetTimeZone();
		}
	}

	#region PXSubordinateAndWingmenSelectorAttribute
	public class PXSubordinateAndWingmenSelectorAttribute : PXSubordinateSelectorAttribute
	{

		public PXSubordinateAndWingmenSelectorAttribute(Type where)
            :base("BIZACCT",GetCommand(where),true, true)
		{			
		}

		public PXSubordinateAndWingmenSelectorAttribute()
			: this(null)
		{
		}

		private static Type GetCommand(Type where)
		{
			var whereType = typeof(Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>,
                        Or<CREmployee.userID, OwnedUser<Current<AccessInfo.userID>>,
                        Or<CREmployee.bAccountID, WingmanUser<Current<AccessInfo.userID>>>
						>>);
			if (where != null)
				whereType = BqlCommand.Compose(typeof(Where2<,>),
                    typeof(Where<CREmployee.userID, Equal<Current<AccessInfo.userID>>,
                        Or<CREmployee.userID, OwnedUser<Current<AccessInfo.userID>>>>),
					typeof(And<>), where);
            return BqlCommand.Compose(typeof(Search5<,,,>), typeof(CREmployee.bAccountID),
                typeof(LeftJoin<Users, On<Users.pKID, Equal<CREmployee.userID>>>),
				whereType,
                typeof(Aggregate<GroupBy<CREmployee.acctCD>>));
		}

	}
	#endregion

	#region PXSubordinateAndWingmenOwnerSelectorAttribute

	/// <summary>
	/// Allows show employees which are subordinated or wingman for current logined employee.
	/// </summary>
	/// <example>
	/// [PXSubordinateAndWingmenOwnerSelector]
	/// </example>
	public class PXSubordinateAndWingmenOwnerSelectorAttribute : PXOwnerSelectorAttribute
	{
		public PXSubordinateAndWingmenOwnerSelectorAttribute()
			: base(null, typeof(Search5<EPEmployee.pKID,
				LeftJoin<Users, On<Users.pKID, Equal<EPEmployee.pKID>>>,
				Where<EPEmployee.pKID, Equal<Current<AccessInfo.userID>>,
						Or<EPEmployee.pKID, OwnedUser<Current<AccessInfo.userID>>,
						Or<EPEmployee.bAccountID, WingmanUser<Current<AccessInfo.userID>>>
						>>,
				Aggregate<GroupBy<EPEmployee.pKID>>>), false)
		{
		}
	}
	#endregion

    #region BusinessDateDefaultAttribute

    /// <summary>
    /// Allow determine current business date with time part on 'field default' event.
    /// </summary>
    /// <example>
    /// [BusinessDateTimeDefault]
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class BusinessDateTimeDefaultAttribute : PXDefaultAttribute
	{
		public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
		    e.NewValue = sender.Graph.Accessinfo.BusinessDate?.AddMinutes(PXTimeZoneInfo.Now.Minute + PXTimeZoneInfo.Now.Hour*60);
}
	}

	#endregion
}
