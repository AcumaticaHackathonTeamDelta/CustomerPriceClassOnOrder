using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.CT;
using System.Collections;
using PX.Objects.IN;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.CR;
using System.Diagnostics;

namespace PX.Objects.PM
{

	#region Project Selectors

	/// <summary>
	/// Selector Attribute that displays all Projects including Templates.
	/// Selector also has <see cref="WarnIfCompleted"/> property for field verification.
	/// This Attribute also contains static Util methods.
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible, FieldClass = DimensionName)]
	[PXAttributeFamily(typeof(AcctSubAttribute))]
	public class ProjectAttribute : AcctSubAttribute, IPXFieldVerifyingSubscriber
	{
		public const string DimensionName = "PROJECT";


		public ProjectAttribute()
		{
			WarnIfCompleted = true;

			Type SearchType =
					BqlCommand.Compose(
					typeof(Search<,>),
					typeof(PMProject.contractID),
					typeof(Where<,,>),
					typeof(PMProject.baseType),
					typeof(Equal<PMProject.ProjectBaseType>),
					typeof(And<Match<Current<AccessInfo.userName>>>)
					);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, SearchType, typeof(PMProject.contractCD),
				typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);

			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		/// <summary>
		/// Creates an instance of ProjectAttribute.
		/// </summary>
		/// <param name="where">BQL Where 
		/// </param>
		public ProjectAttribute(Type where)
		{
			WarnIfCompleted = true;

			Type SearchType =
					BqlCommand.Compose(
					typeof(Search<,>),
					typeof(PMProject.contractID),
					typeof(Where<,,>),
					typeof(PMProject.baseType),
					typeof(Equal<PMProject.ProjectBaseType>),
					typeof(And2<,>),
					typeof(Match<Current<AccessInfo.userName>>),
					typeof(And<>),
					where);


			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, SearchType, typeof(PMProject.contractCD),
				typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		/// <summary>
		/// If True a Warning will be shown if the Project selected is Completed.
		/// Default = True
		/// </summary>
		///
		public bool WarnIfCompleted { get; set; }

		/// <summary>
		/// Composes VisibleInModule Type to be used in BQL
		/// </summary>
		/// <param name="Module">Module</param>
		public static Type ComposeVisibleIn(string Module)
		{
			Type visibleInModule;
			switch (Module)
			{
				case BatchModule.GL:
					visibleInModule = typeof(PMProject.visibleInGL);
					break;
				case BatchModule.AP:
					visibleInModule = typeof(PMProject.visibleInAP);
					break;
				case BatchModule.AR:
					visibleInModule = typeof(PMProject.visibleInAR);
					break;
				case BatchModule.SO:
					visibleInModule = typeof(PMProject.visibleInSO);
					break;
				case BatchModule.PO:
					visibleInModule = typeof(PMProject.visibleInPO);
					break;
				case BatchModule.IN:
					visibleInModule = typeof(PMProject.visibleInIN);
					break;
				case BatchModule.CA:
					visibleInModule = typeof(PMProject.visibleInCA);
					break;
				case BatchModule.EP:
					visibleInModule = typeof(PMProject.visibleInEP);
					break;
				case BatchModule.CR:
					visibleInModule = typeof(PMProject.visibleInCR);
					break;
				default:
					throw new ArgumentOutOfRangeException("Module", Module, Messages.ProjectAttributeNotSupport);
			}

			return visibleInModule;
		}

		/// <summary>
		/// Returns True if the given module is integrated with PM.
		/// </summary>
		/// <remarks>Always returns True if Module is null or an empty string.</remarks>
		public static bool IsPMVisible(PXGraph graph, string module)
		{
			if (string.IsNullOrEmpty(module))
				return true;

            if (!PXAccess.FeatureInstalled<FeaturesSet.projectModule>())
                return false;

			if (graph == null)
				throw new ArgumentNullException("graph");

			PM.PMSetup setup = PXSelect<PM.PMSetup>.Select(graph);
			if (setup == null)
			{
				return false;
			}
			else
			{
				if (setup.IsActive != true)
					return false;
				else
				{
					switch (module)
					{
						case BatchModule.AR: return setup.VisibleInAR == true;
						case BatchModule.AP: return setup.VisibleInAP == true;
						case BatchModule.EP: return setup.VisibleInEP == true;
						case BatchModule.GL: return setup.VisibleInGL == true;
						case BatchModule.IN: return setup.VisibleInIN == true;
						case BatchModule.PO: return setup.VisibleInPO == true;
						case BatchModule.SO: return setup.VisibleInSO == true;
						case BatchModule.CA: return setup.VisibleInCA == true;
						case BatchModule.CR: return setup.VisibleInCR == true;

						default: return false;
					}
				}
			}
		}


		#region IPXFieldVerifyingSubscriber Members

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMProject project = PXSelect<PMProject>.Search<PMProject.contractID>(sender.Graph, e.NewValue);
			if (project != null)
			{
				if (WarnIfCompleted && project.IsCompleted == true)
				{
					sender.RaiseExceptionHandling(FieldName, e.Row, e.NewValue,
                        new PXSetPropertyException(Warnings.ProjectIsCompleted, PXErrorLevel.Warning));
				}
			}
		}

		#endregion

	}

	/// <summary>
	/// Attribute for ProjectCD (ContractCD) field. Aggregates PXFieldAttribute, PXUIFieldAttribute and DimensionSelector without any restriction.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionName)]
	public class ProjectRawAttribute : AcctSubAttribute
	{
		public ProjectRawAttribute()
			: base()
		{
			PXDimensionAttribute attr = new PXDimensionAttribute(ProjectAttribute.DimensionName);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}


	/// <summary>
	/// Project Selector that displays only Active and Not Completed Projects
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionName)]
	public class ActiveProjectAttribute : AcctSubAttribute
	{
		public ActiveProjectAttribute()
		{
			Type SearchType =
					BqlCommand.Compose(
					typeof(Search<,>),
					typeof(PMProject.contractID),
					typeof(Where<,,>),
					typeof(PMProject.baseType),
					typeof(Equal<PMProject.ProjectBaseType>),
					typeof(And<,,>),
					typeof(PMProject.isActive),
					typeof(Equal<>),
					typeof(True),
                    typeof(And<,,>),
                    typeof(PMProject.isTemplate),
                    typeof(Equal<>),
                    typeof(False),
					typeof(And<,,>),
					typeof(PMProject.isCompleted),
					typeof(Equal<>),
					typeof(False),
					typeof(And<Match<Current<AccessInfo.userName>>>)
					);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, SearchType, typeof(PMProject.contractCD),
				typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ActiveProjectAttribute(Type Where)
		{
			if (Where == null)
				throw new ArgumentNullException("Where");

			Type SearchType =
					BqlCommand.Compose(
					typeof(Search<,>),
					typeof(PMProject.contractID),
					typeof(Where<,,>),
					typeof(PMProject.baseType),
					typeof(Equal<PMProject.ProjectBaseType>),
					typeof(And<,,>),
					typeof(PMProject.isActive),
					typeof(Equal<>),
					typeof(True),
                    typeof(And<,,>),
                    typeof(PMProject.isTemplate),
                    typeof(Equal<>),
                    typeof(False),
					typeof(And<,,>),
					typeof(PMProject.isCompleted),
					typeof(Equal<>),
					typeof(False),
					typeof(And2<,>),
					typeof(Match<Current<AccessInfo.userName>>),
					typeof(And<>),
					Where);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, SearchType, typeof(PMProject.contractCD),
				typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	/// <summary>
	/// Project Selector that displays only Active Projects for a given Module. Project Field is visible and mandatory 
	/// only if the Current module (XX) is integrated into PM. i.e. PMSetup.VisibleInXX = True.
	/// If Customer field is supplied then only those Projects are shown that are specific for the given Customer.
	/// ShowContracts option specifies whether to show Contracts in the selector or just Projects.
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionName)]
	public class ActiveProjectForModuleAttribute : AcctSubAttribute, IPXFieldVerifyingSubscriber
	{
		protected readonly string module;
		protected readonly bool dontFilterByCustomer;
		protected readonly bool showContracts;

		public Type AccountFieldType { get; set; }

		protected readonly Type Customer;
		protected Type Module;



		public ActiveProjectForModuleAttribute(string module) : this(module, null, false, false)
		{
		}

		public ActiveProjectForModuleAttribute(string module, bool showContracts) : this(module, null, showContracts, false)
		{
		}

		public ActiveProjectForModuleAttribute(string module, Type customer) : this(module, customer, false, false)
		{
		}

		public ActiveProjectForModuleAttribute(string module, Type customer, bool showContracts, bool dontFilterByCustomer)
		{
			if (string.IsNullOrEmpty(module))
				throw new ArgumentNullException("module");

			this.Customer = customer;
			this.module = module;
			this.dontFilterByCustomer = dontFilterByCustomer;
			this.showContracts = showContracts;

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName,
				CreateCommand(module, customer, showContracts, dontFilterByCustomer, false), typeof (PMProject.contractCD),
				typeof (PMProject.contractCD), typeof (PMProject.description), typeof (PMProject.status));
			select.DescriptionField = typeof (PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ActiveProjectForModuleAttribute(Type Module, Type customer, bool aShowContracts)
		{
			dontFilterByCustomer = true;
			this.Customer = customer;
			this.Module = Module;
			this.showContracts = aShowContracts;
			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName,
				CreateCommand(Module, customer, this.showContracts, true, false), typeof (PMProject.contractCD),
				typeof (PMProject.contractCD), typeof (PMProject.description), typeof (PMProject.status));
			select.DescriptionField = typeof (PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.RowSelected.AddHandler(sender.GetItemType(), OnRowSelectedHandler);
			if (AccountFieldType != null)
				sender.Graph.FieldUpdated.AddHandler(sender.GetItemType(), AccountFieldType.Name, AccountFieldUpdated);

			Visible =
				Enabled =
					this.showContracts
						? ProjectAttribute.IsPMVisible(sender.Graph, module) ||
						  PXAccess.FeatureInstalled<FeaturesSet.contractManagement>()
						: ProjectAttribute.IsPMVisible(sender.Graph, module);
		}

		private void OnRowSelectedHandler(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (AccountFieldType != null && !showContracts)
			{
				Account account =
					PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sender.Graph,
						sender.GetValue(e.Row, AccountFieldType.Name));
				if (account != null)
				{
					PXUIFieldAttribute.SetEnabled(sender, e.Row, FieldName, account.AccountGroupID != null);
				}
			}
		}


		protected internal static Type CreateCommand(string module, Type customer, bool showContracts,
			bool dontFilterByCustomer, bool isSelectCommand)
		{
			Type visibleInModule = ProjectAttribute.ComposeVisibleIn(module);
			List<Type> command = new List<Type>();

			if (isSelectCommand)
				command.AddRange(new[] {typeof (Select<,>), typeof (PMProject)});
			else
				command.AddRange(new[] {typeof (Search<,>), typeof (PMProject.contractID)});

			command.AddRange(
				new[]
				{

					typeof (Where<,,>),
					typeof (PMProject.nonProject),
					typeof (Equal<True>),
					typeof (Or<>),
					typeof (Where<,,>),
					typeof (PMProject.isActive),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,,>),
					typeof (PMProject.isTemplate),
					typeof (Equal<>),
					typeof (False),
					typeof (And<,,>),
					typeof (PMProject.isCompleted),
					typeof (Equal<>),
					typeof (False),
				}
				);

			if (customer != null && !dontFilterByCustomer)
			{
				command.AddRange(
					new[]
					{
						typeof (And2<,>),
						typeof (Where<,,>),
						typeof (PMProject.customerID),
						typeof (Equal<>),
						typeof (Current<>),
						customer,
						typeof (Or<,>),
						typeof (Current<>),
						customer,
						typeof (IsNull),
					});
			}
			command.Add(typeof (And<>));
			if (showContracts)
				command.Add(typeof (Where2<,>));

			command.AddRange(
				new[]
				{
					typeof (Where<,,>),
					typeof (PMProject.baseType),
					typeof (Equal<PMProject.ProjectBaseType>),
					typeof (And<,,>),
					visibleInModule,
					typeof (Equal<True>),
					typeof (And<Match<Current<AccessInfo.userName>>>)
				});

			if (showContracts)
				command.AddRange(
					new[]
					{
						typeof (Or<,>),
						typeof (PMProject.baseType),
						typeof (Equal<PMProject.ContractBaseType>)
					});

			return BqlCommand.Compose(command.ToArray());
		}

		protected internal static Type CreateCommand(Type Module, Type customer, bool showContracts, bool dontFilterByCustomer,
			bool isSelectCommand)
		{
			List<Type> command = new List<Type>();

			if (isSelectCommand)
				command.AddRange(new[] {typeof (Select<,>), typeof (PMProject)});
			else
				command.AddRange(new[] {typeof (Search<,>), typeof (PMProject.contractID)});

			command.AddRange(
				new[]
				{

					typeof (Where<,,>),
					typeof (PMProject.nonProject),
					typeof (Equal<True>),
					typeof (Or<>),
					typeof (Where<,,>),
					typeof (PMProject.isActive),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,,>),
					typeof (PMProject.isTemplate),
					typeof (Equal<>),
					typeof (False),
					typeof (And<,,>),
					typeof (PMProject.isCompleted),
					typeof (Equal<>),
					typeof (False),
					typeof (And2<,>),
					typeof (Match<Current<AccessInfo.userName>>),
				}
				);

			if (customer != null)
			{
				command.AddRange(
					new[]
					{
						typeof (And2<,>),
						typeof (Where<,,>),
						typeof (PMProject.customerID),
						typeof (Equal<>),
						typeof (Current<>),
						customer,
						typeof (Or<,,>),
						typeof (Current<>),
						customer,
						typeof (IsNull),
						typeof (Or<,>),
						typeof (Current<>),
						Module,
						typeof (NotEqual<GL.BatchModule.moduleAR>),
					});
			}
			command.Add(typeof (And2<,>));
			command.AddRange(
				new[]
				{
					typeof (Where<,,>),
					typeof (PMProject.visibleInGL),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,,>),
					typeof (Optional<>),
					Module,
					typeof (Equal<>),
					typeof (GL.BatchModule.moduleGL),
					typeof (Or<,,>),
					typeof (PMProject.visibleInAR),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,,>),
					typeof (Optional<>),
					Module,
					typeof (Equal<>),
					typeof (GL.BatchModule.moduleAR),
					typeof (Or<,,>),
					typeof (PMProject.visibleInAP),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,,>),
					typeof (Optional<>),
					Module,
					typeof (Equal<>),
					typeof (GL.BatchModule.moduleAP),
					typeof (Or<,,>),
					typeof (PMProject.visibleInCA),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,,>),
					typeof (Optional<>),
					Module,
					typeof (Equal<>),
					typeof (GL.BatchModule.moduleCA),
					typeof (Or<,,>),
					typeof (PMProject.visibleInIN),
					typeof (Equal<>),
					typeof (True),
					typeof (And<,>),
					typeof (Optional<>),
					Module,
					typeof (Equal<>),
					typeof (GL.BatchModule.moduleIN)
				});

			if (showContracts)
			{
				command.AddRange(
					new[]
					{
						typeof (And<>),
						typeof (Where<,,>),
						typeof (PMProject.baseType),
						typeof (Equal<PMProject.ProjectBaseType>),
						typeof (Or<,>),
						typeof (PMProject.baseType),
						typeof (Equal<PMProject.ContractBaseType>)
					});

			}
			else
			{
				command.AddRange(
					new[]
					{
						typeof (And<,>),
						typeof (PMProject.baseType),
						typeof (Equal<PMProject.ProjectBaseType>)
					});

			}
			return BqlCommand.Compose(command.ToArray());
		}

		protected virtual void AccountFieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object newVlaue = sender.GetValue(e.Row, _FieldName);
			try
			{
				sender.RaiseFieldVerifying(_FieldName, e.Row, ref newVlaue);
			}
			catch (PXSetPropertyException err)
			{
				sender.RaiseExceptionHandling(_FieldName, e.Row, newVlaue, err);
			}
		}

		#region IPXFieldVerifyingSubscriber Members

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			PMProject project = PXSelect<PMProject>.Search<PMProject.contractID>(sender.Graph, e.NewValue);
			if (project != null && project.NonProject != true)
			{
				if (project.BaseType == PMProject.ProjectBaseType.Project)
				{
					if (project != null && project.NonProject != true && project.BaseType == PMProject.ProjectBaseType.Project &&
					    AccountFieldType != null)
					{
						Account account =
							PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sender.Graph,
								sender.GetValue(e.Row, AccountFieldType.Name));
						if (account != null && account.AccountGroupID == null)
						{
							var newRow = sender.CreateCopy(e.Row);
							sender.SetValue(newRow, _FieldName, e.NewValue);
							e.NewValue = sender.GetStateExt(newRow, _FieldName);
							throw new PXSetPropertyException(PM.Messages.NoAccountGroup, PXErrorLevel.Error, account.AccountCD);
						}
					}
				}


				bool isApplicable = false;
				string currentModule = module;
				if (Module != null)
				{
					currentModule = (string) sender.GetValue(e.Row, sender.GetField(Module));
				}
				if (currentModule == BatchModule.AR || currentModule == BatchModule.SO || currentModule == BatchModule.CR)
					isApplicable = true;
				if (dontFilterByCustomer && isApplicable)
				{
					if (project != null && project.NonProject != true)
					{
						int? customerID = (int?) sender.GetValue(e.Row, Customer.Name);

						if (customerID != project.CustomerID)
						{
							sender.RaiseExceptionHandling(FieldName, e.Row, e.NewValue,
								new PXSetPropertyException(Warnings.ProjectCustomerDontMatchTheDocument, PXErrorLevel.Warning));
						}
					}
				}
			}
		}

		#endregion
	}


	/// <summary>
	/// Project Selector that displays only Active Projects and Active Contracts for the given Customer.
	/// If Customer is null all Active Projects and Contracts are displayed.
	/// </summary>
    [PXDBInt()]
		[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible)]
		[PXRestrictor(typeof(Where<PMProject.isActive, Equal<True>>), Messages.InactiveContract, typeof(PMProject.contractCD))]
		[PXRestrictor(typeof(Where<PMProject.isCompleted, Equal<False>>), Messages.CompleteContract, typeof(PMProject.contractCD))]
		[PXRestrictor(typeof(Where<PMProject.isTemplate, Equal<False>>), Messages.TemplateContract, typeof(PMProject.contractCD))]
    public class ActiveProjectOrContractAttribute : AcctSubAttribute
    {

		public ActiveProjectOrContractAttribute():this(null)
		{
		}

		public ActiveProjectOrContractAttribute(Type customer)
		{

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, CreateCommand(customer), typeof(PMProject.contractCD),
			typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;

		}

		protected internal static Type CreateCommand(Type customer)
		{
			List<Type> command = new List<Type>();


			if (customer == null)
			{
				command.AddRange(
				new[]
					{
						typeof (Search<>),
						typeof (PMProject.contractID),						
					}
			   );
			}
			else
			{
				command.AddRange(
					new[]
						{
							typeof (Search2<,,>),
							typeof (PMProject.contractID),
							typeof(LeftJoin<,>),
							typeof(BAccountR),
							typeof(On<,>),
							typeof(BAccountR.bAccountID),
							typeof(Equal<>),
							typeof(Current<>),
							customer,
							typeof (Where<,,>),
							typeof (PMProject.nonProject),
							typeof (Equal<True>),
							typeof (Or<>),							
							typeof (Where<,,>),
							typeof (PMProject.customerID),
							typeof (Equal<>),
							typeof (Current<>),
							customer,
							typeof (Or<,,>),
							typeof (Current<>),
							customer,
							typeof (IsNull),
							typeof (Or<,>),
							typeof (BAccountR.type),
							typeof (Equal<>),
							typeof (BAccountType.vendorType),
					});
			}

			return BqlCommand.Compose(command.ToArray());
		}
	}


	/// <summary>
	/// Displays only Active Projects. Project Field is visible only if the PO module is integrated into PM. i.e. PMSetup.VisibleInPO = True.
	/// If Visible, Project field is mandatory only for non-stock type of lines (Non-stock, service, freight)
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionName)]
	public class POActiveProjectAttribute : AcctSubAttribute, IPXRowPersistingSubscriber
	{
		protected Type lineType;
		public POActiveProjectAttribute(Type lineType)
		{
			if (lineType == null)
				throw new ArgumentNullException();

			this.lineType = lineType;

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, ActiveProjectForModuleAttribute.CreateCommand(BatchModule.PO, null, false, false, false), typeof(PMProject.contractCD),
			typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);
			select.ValidComboRequired = true;
			select.CacheGlobal = true;
			
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		#region IPXRowPersistingSubscriber Members

		public void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (ProjectAttribute.IsPMVisible(sender.Graph, BatchModule.PO) &&
				 IsRequired(sender, e.Row))
			{
				int? projectID = (int?)sender.GetValue(e.Row, FieldOrdinal);

				if (projectID == null)
				{
					if (sender.RaiseExceptionHandling(FieldName, e.Row, null, new PXSetPropertyException(Data.ErrorMessages.FieldIsEmpty, FieldName)))
					{
						throw new PXRowPersistingException(FieldName, null, Data.ErrorMessages.FieldIsEmpty, FieldName);
					}
				}
			}
		}

		#endregion

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			Visible = Enabled = ProjectAttribute.IsPMVisible(sender.Graph, BatchModule.PO);
		}

		protected virtual bool IsRequired(PXCache sender, object row)
		{
			if (row == null)
				return false;

			string poLineType = (string)sender.GetValue(row, lineType.Name);
			return IsRequired(poLineType);
		}

		public static bool IsRequired(string poLineType)
		{
			switch (poLineType)
			{
				case PX.Objects.PO.POLineType.NonStock:
				case PX.Objects.PO.POLineType.Freight:
				case PX.Objects.PO.POLineType.Service:
					return true;

				default:
					return false;
			}
		}

	}


	#region EPTimeCardActiveProjectAttribute
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible)]
	public class EPTimeCardProjectAttribute : AcctSubAttribute
	{
		public EPTimeCardProjectAttribute()
		{
			Type searchType = typeof(Search2<PMProject.contractID,
			LeftJoin<EPEmployeeContract, On<EPEmployeeContract.contractID, Equal<PMProject.contractID>, And<EPEmployeeContract.employeeID, Equal<Current<EPTimeCard.employeeID>>>>>,
			Where<PMProject.isTemplate, Equal<False>,
			And<PMProject.isCancelled, NotEqual<True>,
			And<PMProject.baseType, Equal<PMProject.ProjectBaseType>,
			And2<Where<PMProject.restrictToEmployeeList, Equal<False>, Or<EPEmployeeContract.employeeID, IsNotNull>>,
			And2<Match<Current<AccessInfo.userName>>,
			And<Where<PMProject.visibleInEP, Equal<True>, Or<PMProject.nonProject, Equal<True>>>>>>>>>,
			OrderBy<Desc<PMProject.contractCD>>>);


			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectAttribute.DimensionName, searchType, typeof(PMProject.contractCD),
			typeof(PMProject.contractCD), typeof(PMProject.description), typeof(PMProject.customerID), typeof(PMProject.locationID), typeof(PMProject.status));
			select.DescriptionField = typeof(PMProject.description);
			select.ValidComboRequired = true;
			
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			Visible = Enabled = ProjectAttribute.IsPMVisible(sender.Graph, BatchModule.EP);
		}
	}

	#endregion 

	
	#region EPEquipmentActiveProjectAttribute
	[PXDBInt()]
	[PXUIField(DisplayName = "Project", Visibility = PXUIVisibility.Visible)]
	public class EPEquipmentActiveProjectAttribute : AcctSubAttribute, IPXFieldVerifyingSubscriber
	{
		public EPEquipmentActiveProjectAttribute()
		{
			Type searchType = typeof(Search<PMProject.contractID,
			Where<PMProject.isTemplate, Equal<False>,
			And<PMProject.isActive, Equal<True>,
            And<PMProject.isCompleted, Equal<False>, 
			And<PMProject.baseType, Equal<PMProject.ProjectBaseType>,
			And<Match<Current<AccessInfo.userName>>>>>>>,
			OrderBy<Desc<PMProject.contractCD>>>);


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
			Visible = Enabled = ProjectAttribute.IsPMVisible(sender.Graph, BatchModule.EP);
		}

		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
	    {
            if (e.Row != null && e.NewValue != null)
            {
                PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(sender.Graph, e.NewValue);
                if (project != null && project.RestrictToResourceList == true)
                {
                    PMProject allowedForProject = PXSelectJoin<PMProject, LeftJoin<EPEquipmentRate,
                                                            On<EPEquipmentRate.projectID, Equal<PMProject.contractID>,
                                                            And<EPEquipmentRate.equipmentID, Equal<Current<EPEquipmentTimeCard.equipmentID>>>>>,
                                                            Where<EPEquipmentRate.projectID, Equal<Required<PMProject.contractID>>>>.Select(sender.Graph, project.ContractID);
                    if (allowedForProject == null)
                        throw new PXSetPropertyException(EP.Messages.ProjectIsNotAvailableForEquipment);
                }
            }
	    }
	}

	#endregion 

	#endregion

	#region Project Task Selectors
	/// <summary>
	/// Displays all Tasks for the given Project. Task is mandatory if Project is set.
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Project Task", Visibility = PXUIVisibility.Visible)]
	public class ProjectTaskAttribute : AcctSubAttribute, IPXRowPersistingSubscriber, IPXFieldSelectingSubscriber
	{
		public const string DimensionName = "PROTASK";
		Type projectIDField;
		string module;

		/// <summary>
		/// If True allows TaskID to be null if ProjectID is a Contract.
		/// </summary>
		///
		public bool AllowNullIfContract { get; set; }

		public bool AllowNull { get; set; }

		public ProjectTaskAttribute(Type projectID)
		{
			if (projectID == null)
				throw new ArgumentNullException("projectID");

			projectIDField = projectID;

			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,>),
				typeof(PMTask.taskID),
				typeof(Where<,>),
				typeof(PMTask.projectID),
				typeof(Equal<>),
				typeof(Current<>),
				projectID);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(PMTask.taskCD),
				typeof(PMTask.taskCD), typeof(PMTask.description), typeof(PMTask.status));
			select.DescriptionField = typeof(PMTask.description);
			select.ValidComboRequired = true;
						
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ProjectTaskAttribute(Type projectID, string Module)
		{
			if (projectID == null)
				throw new ArgumentNullException("projectID");

			if (string.IsNullOrEmpty(Module))
				throw new ArgumentNullException("Module");

			projectIDField = projectID;
			module = Module;

			Type visibleInModule;
			switch (Module)
			{
				case BatchModule.GL:
					visibleInModule = typeof(PMTask.visibleInGL);
					break;
				case BatchModule.AP:
					visibleInModule = typeof(PMTask.visibleInAP);
					break;
				case BatchModule.AR:
					visibleInModule = typeof(PMTask.visibleInAR);
					break;
				case BatchModule.SO:
					visibleInModule = typeof(PMTask.visibleInSO);
					break;
				case BatchModule.PO:
					visibleInModule = typeof(PMTask.visibleInPO);
					break;
				case BatchModule.IN:
					visibleInModule = typeof(PMTask.visibleInIN);
					break;
				case BatchModule.CA:
					visibleInModule = typeof(PMTask.visibleInCA);
					break;
				case BatchModule.EP:
					visibleInModule = typeof(PMTask.visibleInEP);
					break;
				case BatchModule.CR:
					visibleInModule = typeof(PMTask.visibleInCR);
					break;
				default:
					throw new ArgumentOutOfRangeException("Module", Module, Messages.ProjectTaskAttributeNotSupport);
			}

			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,>),
				typeof(PMTask.taskID),
				typeof(Where<,,>),
				visibleInModule,
				typeof(Equal<True>),
				typeof(And<,>),
				typeof(PMTask.projectID),
				typeof(Equal<>),
				typeof(Current<>),
				projectID);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(PMTask.taskCD),
				typeof(PMTask.taskCD), typeof(PMTask.description), typeof(PMTask.status));
			select.DescriptionField = typeof(PMTask.description);
			select.ValidComboRequired = true;
			
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		#region IPXRowPersistingSubscriber Members

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			//rule: Task is mandatory only if Project is not an X project.

			int? projectID = (int?)sender.GetValue(e.Row, projectIDField.Name);
			int? taskID = (int?)sender.GetValue(e.Row, FieldOrdinal);

			if (projectID != null && !ProjectDefaultAttribute.IsNonProject(sender.Graph, projectID) && taskID == null)
			{
				if (AllowNull) return;

				if (AllowNullIfContract)
				{
					//projectID may be contract and task is not required.
					PMProject project = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(sender.Graph, projectID);
					if (project.BaseType == PMProject.ContractBaseType.Contract)
					{
						return;
					}
				}

				if (sender.RaiseExceptionHandling(FieldName, e.Row, null, new PXSetPropertyException(Data.ErrorMessages.FieldIsEmpty, FieldName)))
				{
					throw new PXRowPersistingException(FieldName, null, Data.ErrorMessages.FieldIsEmpty, FieldName);
				}

			}
		}

		#endregion

		#region IPXFieldSelectingSubscriber Members

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			PXFieldState ss = e.ReturnState as PXFieldState;
			if (ss != null)
			{
				PMProject project = null;
				if (e.Row != null)
				{
					project = PXSelectorAttribute.Select(sender, e.Row, projectIDField.Name) as PMProject;

					if (project == null)
					{
						//Can be that Selector Attribute is not defined for the given field. Fallback to slower method:
						int? projectID = (int?)sender.GetValue(e.Row, projectIDField.Name);
						project = PXSelect<PMProject>.Search<PMProject.contractID>(sender.Graph, projectID);

					}
				}

                ss.Enabled = (project != null && project.BaseType == PMProject.ProjectBaseType.Project && project.NonProject != true && sender.GetValue(e.Row, projectIDField.Name) != null) || sender.Graph.IsImport;
				ss.Visible = ProjectAttribute.IsPMVisible(sender.Graph, module);
			}
		}
		#endregion

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.SetAltered(_FieldName, true);
			sender.Graph.FieldUpdated.AddHandler(sender.GetItemType(), projectIDField.Name, OnProjectUpdated);
		}
		
		protected virtual void OnProjectUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object taskCD = (sender.GetValuePending(e.Row, _FieldName) as string) ?? (sender.GetValueExt(e.Row, _FieldName) as PXSegmentedState).Value;

			if (taskCD != null && taskCD != PXCache.NotSetValue)
			{
				try
				{
					object projectID = sender.GetValue(e.Row, projectIDField.Name);
				    PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(sender.Graph, projectID, taskCD);

                    if (task != null && !ProjectDefaultAttribute.IsNonProject(sender.Graph, (int?)projectID))
                        sender.SetValueExt(e.Row, _FieldName, task.TaskID);
                    else
                    {
                        object pendingValue = null;
                        if (ProjectDefaultAttribute.IsNonProject(sender.Graph, (int?)projectID))
                        {
                            object projectCD = sender.GetValuePending(e.Row, projectIDField.Name);
                            if (projectCD != null && projectCD != PXCache.NotSetValue)
                            {
                                PMProject project = PXSelect<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>.Select(sender.Graph, projectCD);
                                task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(sender.Graph, project.ContractID, taskCD);
                                if (task != null)
                                {
                                    pendingValue = task.TaskCD;
                                }
                            }
                        }
                        sender.SetValuePending(e.Row, _FieldName, pendingValue);
                    }
				}
				catch (PXException)
				{
					sender.SetValuePending(e.Row, _FieldName, null);
				}
			}
		}

	}

	
	/// <summary>
	/// Attribute for TaskCD field. Aggregates PXFieldAttribute, PXUIFieldAttribute and DimensionSelector without any restriction.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Project Task", Visibility = PXUIVisibility.Visible)]
	public class ProjectTaskRawAttribute : AcctSubAttribute
	{
		public ProjectTaskRawAttribute()
			: base()
		{
			PXDimensionAttribute attr = new PXDimensionAttribute(ProjectTaskAttribute.DimensionName);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}


	/// <summary>
	/// Task Selector that displays all active Tasks for the given Project. Task Field is Disabled if a Non-Project is selected; otherwise mandatory.
	/// Task Selector always work in pair with Project Selector. When the Project Selector displays a valid Project Task field becomes mandatory.
	/// When This selector is used in pair with <see cref="ActiveProjectOrContractAttribute"/> and a Contract is selected Task is no longer mandatory.
	/// If Completed Task is selected an error will be displayed - Completed Task cannot be used in DataEntry.
	/// </summary>
	/// 
	[PXDBInt()]
	[PXUIField(DisplayName = "Project Task", Visibility = PXUIVisibility.Visible, FieldClass = ProjectAttribute.DimensionName)]
    [PXRestrictor(typeof(Where<PMTask.isActive, Equal<True>, And<PMTask.isCancelled, NotEqual<True>>>), Messages.InactiveTask, typeof(PMTask.taskCD))]
	public class ActiveProjectTaskAttribute : AcctSubAttribute, IPXFieldSelectingSubscriber, IPXFieldVerifyingSubscriber
	{
		readonly Type projectIDField;
		readonly Type needTaskValidationField;
	    readonly string module;

		protected int _ModuleRestrictorAttrIndex = -1;
		protected Type _visibleInModule;

		public bool AllowCompleted { get; set; }
				
		public ActiveProjectTaskAttribute(Type projectID)
		{
			if (projectID == null)
				throw new ArgumentNullException("projectID");

			projectIDField = projectID;

			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,>),
				typeof(PMTask.taskID),
				typeof(Where<,>),
				typeof(PMTask.projectID),
				typeof(Equal<>),
				typeof(Optional<>),
				projectID
				);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectTaskAttribute.DimensionName, SearchType, typeof(PMTask.taskCD),
				typeof(PMTask.taskCD), typeof(PMTask.description), typeof(PMTask.status));
			select.DescriptionField = typeof(PMTask.description);
			select.ValidComboRequired = true;
						
			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public ActiveProjectTaskAttribute(Type projectID, string Module): this(projectID)
		{
			if (string.IsNullOrEmpty(Module))
				throw new ArgumentNullException("Module");

			module = Module;

			switch (Module)
			{
				case BatchModule.GL:
					_visibleInModule = typeof(PMTask.visibleInGL);
					break;
				case BatchModule.AP:
					_visibleInModule = typeof(PMTask.visibleInAP);
					break;
				case BatchModule.AR:
					_visibleInModule = typeof(PMTask.visibleInAR);
					break;
				case BatchModule.SO:
					_visibleInModule = typeof(PMTask.visibleInSO);
					break;
				case BatchModule.PO:
					_visibleInModule = typeof(PMTask.visibleInPO);
					break;
				case BatchModule.IN:
					_visibleInModule = typeof(PMTask.visibleInIN);
					break;
				case BatchModule.CA:
					_visibleInModule = typeof(PMTask.visibleInCA);
					break;
				case BatchModule.EP:
					_visibleInModule = typeof(PMTask.visibleInEP);
					break;
				case BatchModule.CR:
					_visibleInModule = typeof(PMTask.visibleInCR);
					break;
				default:
					throw new ArgumentOutOfRangeException("Module", Module, Messages.ProjectTaskAttributeNotSupport);
			}
            _Attributes.Add(new PXRestrictorAttribute(BqlCommand.Compose(typeof(Where<,>), _visibleInModule, typeof(Equal<True>)), PXMessages.LocalizeFormatNoPrefixNLA(Messages.TaskInvisibleInModule, "{0}", PXMessages.LocalizeNoPrefix(module)), typeof(PMTask.taskCD)));
			_ModuleRestrictorAttrIndex = _Attributes.Count - 1;
		}

		public ActiveProjectTaskAttribute(Type projectID, Type Module, Type NeedTaskValidation): this(projectID)
		{
			if (Module == null)
				throw new ArgumentNullException("Module");
			if (NeedTaskValidation == null)
				throw new ArgumentNullException("NeedTaskValidation");

			needTaskValidationField = NeedTaskValidation;
			
            Type VisibleType =
				BqlCommand.Compose(
				typeof(Where<,,>),
				typeof(PMTask.visibleInGL),
				typeof(Equal<>),
				typeof(True),
				typeof(And<,,>),
				typeof(Optional<>),
				Module,
				typeof(Equal<>),
				typeof(GL.BatchModule.moduleGL),
				typeof(Or<,,>),
				typeof(PMTask.visibleInAR),
				typeof(Equal<>),
				typeof(True),
				typeof(And<,,>),
				typeof(Optional<>),
				Module,
				typeof(Equal<>),
				typeof(GL.BatchModule.moduleAR),
				typeof(Or<,,>),
				typeof(PMTask.visibleInAP),
				typeof(Equal<>),
				typeof(True),
				typeof(And<,,>),
				typeof(Optional<>),
				Module,
				typeof(Equal<>),
				typeof(GL.BatchModule.moduleAP),
				typeof(Or<,,>),
				typeof(PMTask.visibleInCA),
				typeof(Equal<>),
				typeof(True),
				typeof(And<,,>),
				typeof(Optional<>),
				Module,
				typeof(Equal<>),
				typeof(GL.BatchModule.moduleCA),
				typeof(Or<,,>),
				typeof(PMTask.visibleInIN),
				typeof(Equal<>),
				typeof(True),
				typeof(And<,>),
				typeof(Optional<>),
				Module,
				typeof(Equal<>),
				typeof(GL.BatchModule.moduleIN)
				);

            _Attributes.Add(new PXRestrictorAttribute(typeof(Where<PMTask.isCompleted, NotEqual<True>>), Messages.CompletedTask, typeof(PMTask.taskCD)));
            _Attributes.Add(new PXRestrictorAttribute(VisibleType, Messages.InvisibleTask, typeof(PMTask.taskCD)));

		}

		#region IPXRowPersistingSubscriber Members

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			//rule: Task is mandatory only if Project is not an X project.
			int? taskID = (int?)sender.GetValue(e.Row, FieldOrdinal);
			Boolean? needsTaskValidation = true;
			if(this.needTaskValidationField != null )
				needsTaskValidation = (Boolean?)sender.GetValue(e.Row, this.needTaskValidationField.Name);

			int? projectID = (int?)sender.GetValue(e.Row, projectIDField.Name);
			PMProject project = PXSelect<PMProject>.Search<PMProject.contractID>(sender.Graph, projectID);

			if (project != null && project.NonProject != true && taskID == null && project.BaseType == PMProject.ProjectBaseType.Project && (needsTaskValidation == true))
			{
				if (sender.RaiseExceptionHandling(FieldName, e.Row, null, new PXSetPropertyException(Data.ErrorMessages.FieldIsEmpty, FieldName)))
				{
					throw new PXRowPersistingException(FieldName, null, Data.ErrorMessages.FieldIsEmpty, FieldName);
				}
			}

		}

		#endregion

		#region IPXFieldVerifyingSubscriber Members
		public virtual void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.NewValue != null && e.NewValue is Int32)
			{
				PMTask task = PXSelect<PMTask>.Search<PMTask.taskID>(sender.Graph, e.NewValue);
				if (task != null)
				{
					if (!AllowCompleted && task.IsCompleted == true)
					{
						var ex = new PXSetPropertyException(PM.Messages.ProjectTaskIsCompleted);
						ex.ErrorValue = task.TaskCD;
						throw ex;
					}
				}
			}
		}
		#endregion

		#region IPXFieldSelectingSubscriber Members

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
            PXFieldState ss = e.ReturnState as PXFieldState;
			Boolean? needsTaskValidation = true;
            if (ss != null  )
            {
				ss.Visible = ProjectAttribute.IsPMVisible(sender.Graph, module);
				if (e.Row != null)
				{
					PMProject rec = (PMProject)PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.SelectSingleBound(sender.Graph, null, sender.GetValue(e.Row, projectIDField.Name));

					if (this.needTaskValidationField != null)
						needsTaskValidation = (Boolean?)sender.GetValue(e.Row, this.needTaskValidationField.Name);
					ss.Enabled = (needsTaskValidation == true) && rec != null && rec.NonProject != true && rec.BaseType == PMProject.ProjectBaseType.Project;
				}				
            }
		}
		#endregion

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.SetAltered(_FieldName, true);
			sender.Graph.FieldUpdated.AddHandler(sender.GetItemType(),projectIDField.Name, OnProjectUpdated);
			sender.Graph.RowPersisting.AddHandler(sender.GetItemType(), RowPersisting);
			if (_ModuleRestrictorAttrIndex != -1 && !string.IsNullOrEmpty(module))
			{
				PXRestrictorAttribute restrictor = (PXRestrictorAttribute) _Attributes[_ModuleRestrictorAttrIndex];
				restrictor.Message = PXMessages.LocalizeFormat(Messages.TaskInvisibleInModule, "{0}", PXUIFieldAttribute.GetDisplayName(sender.Graph.Caches[typeof(PMTask)], _visibleInModule.Name));
			}
		}

		protected virtual void OnProjectUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{     
			object taskCD = (sender.GetValuePending(e.Row, _FieldName) as string) ?? (sender.GetValueExt(e.Row, _FieldName) as PXSegmentedState).Value;

			if(taskCD != null && taskCD != PXCache.NotSetValue)
			{
				try
				{
					object projectID = sender.GetValue(e.Row, projectIDField.Name);
					PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(sender.Graph, projectID, taskCD);

					if (task != null && !ProjectDefaultAttribute.IsNonProject(sender.Graph, (int?) projectID))
						sender.SetValueExt(e.Row, _FieldName, task.TaskID);
					else
					{
						object pendingValue = null;
						if ( ProjectDefaultAttribute.IsNonProject(sender.Graph, (int?) projectID))
						{
							object projectCD = sender.GetValuePending(e.Row, projectIDField.Name);
                            if (projectCD != null && projectCD != PXCache.NotSetValue)
                            {
                                PMProject project = PXSelect<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>.Select(sender.Graph, projectCD);
                                if (project != null)
                                {
                                    task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(sender.Graph, project.ContractID, taskCD);
                                    if (task != null)
                                    {
                                        pendingValue = task.TaskCD;
                                    }
                                }
                            }
						}

						sender.SetValuePending(e.Row, _FieldName, pendingValue);
					}
				}
				catch(PXException)
				{
					sender.SetValuePending(e.Row, _FieldName, null);
				}
			}
		}
	}
	
	/// <summary>
	/// Task Selector that displays all active Tasks for the given Project. Task Field is Disabled if a Non-Project is selected; otherwise mandatory.
	/// Task Selector always work in pair with Project Selector. When the Project Selector displays a valid Project Task field becomes mandatory.
	/// If Completed Task is selected an error will be displayed - Completed Task cannot be used in DataEntry.
	/// 
	/// Task is mandatory only if the Freight amount is greater then zero.
	/// </summary>
	/// 
	public class SOFreightDetailTask : ActiveProjectTaskAttribute
	{
		protected Type curyTotalFreightAmtField;
		public SOFreightDetailTask(Type projectID, Type curyTotalFreightAmt):base(projectID, BatchModule.SO)
		{
			this.curyTotalFreightAmtField = curyTotalFreightAmt;
		}

		public override void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			decimal? amt = (decimal?)sender.GetValue(e.Row, curyTotalFreightAmtField.Name);

			if ( amt > 0 )
				base.RowPersisting(sender, e);
		}	
	}


	[PXDBInt()]
	[PXUIField(DisplayName = "Project Task", Visibility = PXUIVisibility.Visible)]
	// Restrictor is not applicable cause Verifying must be switch on/off in runtime (implemented in graph)
	//[PXRestrictor(typeof(Where<PMTask.isActive, Equal<True>, And<PMTask.isCancelled, NotEqual<True>>>), Messages.InactiveTask, typeof(PMTask.taskCD))]
	public class EPTimecardProjectTaskAttribute : AcctSubAttribute, IPXFieldSelectingSubscriber //, IPXFieldVerifyingSubscriber - verifying is done in graph
	{
		readonly Type projectIDField;
		readonly string module;

		public Type	SkipValidationField{ get; set; }

		public EPTimecardProjectTaskAttribute(Type projectID)
		{
			if (projectID == null)
				throw new ArgumentNullException("projectID");

			projectIDField = projectID;

			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,>),
				typeof(PMTask.taskID),
				typeof(Where<,>),
				typeof(PMTask.projectID),
				typeof(Equal<>),
				typeof(Optional<>),
				projectID
				);

			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(ProjectTaskAttribute.DimensionName, SearchType, typeof(PMTask.taskCD),
				typeof(PMTask.taskCD), typeof(PMTask.description), typeof(PMTask.status));
			select.DescriptionField = typeof(PMTask.description);
			select.ValidComboRequired = true;

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public EPTimecardProjectTaskAttribute(Type projectID, string Module)
			: this(projectID)
		{
			if (string.IsNullOrEmpty(Module))
				throw new ArgumentNullException("Module");

			module = Module;

			Type visibleInModule;
			switch (Module)
			{
				case BatchModule.GL:
					visibleInModule = typeof(PMTask.visibleInGL);
					break;
				case BatchModule.AP:
					visibleInModule = typeof(PMTask.visibleInAP);
					break;
				case BatchModule.AR:
					visibleInModule = typeof(PMTask.visibleInAR);
					break;
				case BatchModule.SO:
					visibleInModule = typeof(PMTask.visibleInSO);
					break;
				case BatchModule.PO:
					visibleInModule = typeof(PMTask.visibleInPO);
					break;
				case BatchModule.IN:
					visibleInModule = typeof(PMTask.visibleInIN);
					break;
				case BatchModule.CA:
					visibleInModule = typeof(PMTask.visibleInCA);
					break;
				case BatchModule.EP:
					visibleInModule = typeof(PMTask.visibleInEP);
					break;
				case BatchModule.CR:
					visibleInModule = typeof(PMTask.visibleInCR);
					break;
				default:
					throw new ArgumentOutOfRangeException("Module", Module, Messages.ProjectTaskAttributeNotSupport);
			}

			_Attributes.Add(new PXRestrictorAttribute(BqlCommand.Compose(typeof(Where<,>), visibleInModule, typeof(Equal<True>)), PXMessages.LocalizeFormatNoPrefixNLA(Messages.TaskInvisibleInModule, "{0}", PXMessages.LocalizeNoPrefix(module)), typeof(PMTask.taskCD)));
		}
		
		#region IPXRowPersistingSubscriber Members

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			//rule: Task is mandatory only if Project is not an X project.
			int? taskID = (int?)sender.GetValue(e.Row, FieldOrdinal);

			int? projectID = (int?)sender.GetValue(e.Row, projectIDField.Name);
			PMProject contract = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.Select(sender.Graph, projectID);

			if (contract != null && contract.NonProject != true && taskID == null && contract.BaseType == PMProject.ProjectBaseType.Project )
			{
				if (sender.RaiseExceptionHandling(FieldName, e.Row, null, new PXSetPropertyException(Data.ErrorMessages.FieldIsEmpty, FieldName)))
				{
					throw new PXRowPersistingException(FieldName, null, Data.ErrorMessages.FieldIsEmpty, FieldName);
				}
			}

		}

		#endregion

		#region IPXFieldSelectingSubscriber Members

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			PXFieldState ss = e.ReturnState as PXFieldState;
			if (ss != null)
			{
				ss.Visible = ProjectAttribute.IsPMVisible(sender.Graph, module);
				if (e.Row != null)
				{
					PMProject rec = (PMProject)PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.SelectSingleBound(sender.Graph, null, sender.GetValue(e.Row, projectIDField.Name));

					ss.Enabled = rec != null && rec.NonProject != true && rec.BaseType == PMProject.ProjectBaseType.Project;
				}
			}
		}
		#endregion

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.SetAltered(_FieldName, true);
			sender.Graph.FieldUpdated.AddHandler(sender.GetItemType(), projectIDField.Name, OnProjectUpdated);
			sender.Graph.RowPersisting.AddHandler(sender.GetItemType(), RowPersisting);
		}
		protected virtual void OnProjectUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			object taskCD = (sender.GetValuePending(e.Row, _FieldName) as string) ?? (sender.GetValueExt(e.Row, _FieldName) as PXSegmentedState).Value;

			if (taskCD != null && taskCD != PXCache.NotSetValue)
			{
				try
				{
					object projectID = sender.GetValue(e.Row, projectIDField.Name);

					PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(sender.Graph, projectID, taskCD);

					if (task != null && !ProjectDefaultAttribute.IsNonProject(sender.Graph, (int?)projectID))
						sender.SetValueExt(e.Row, _FieldName, task.TaskID);
					else
						sender.SetValuePending(e.Row, _FieldName, null);
				}
				catch (PXException)
				{
					sender.SetValuePending(e.Row, _FieldName, null);
				}
			}
		}
	}
	#endregion


	/// <summary>
	/// Defaults ProjectID to the Non-Project if the module supplied is either null or not not intergated with Project Management i.e. PMSetup.VisibleInXX = False.
	/// When Search is supplied ProjectID is defaulted with the value returned by that search.
	/// Selector also contains static Util methods.
	/// </summary>
	public class ProjectDefaultAttribute : PXDefaultAttribute
	{
		protected readonly string module;
		public Type AccountType {get; set;}

        /// <summary>
        /// Forces user to explicitly set the Project irrespective of the AccountType settings.
        /// </summary>
        public bool ForceProjectExplicitly { get; set; }

	    public ProjectDefaultAttribute()
			:this(null)
		{			
		}

		public ProjectDefaultAttribute(string module)
		{
			this.module = module;
		}
		
		public ProjectDefaultAttribute(string module, Type search) : this(module, search, null) { }
		public ProjectDefaultAttribute(string module, Type search, Type account)
			:base(search)
		{
			this.module = module;
			this.AccountType = account;
		}

		public override void FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			base.FieldDefaulting(sender, e);
			if (e.NewValue == null)
			{
				if (IsImporting(sender,e.Row) || IsDefaultNonProject(sender, e.Row) || !IsAccountGroupSpecified(sender, e.Row))
				{
					PMProject prj = PXSelect<PMProject, Where<PMProject.nonProject, Equal<True>>>.SelectSingleBound(sender.Graph, null, null);
					e.NewValue = prj != null ? prj.ContractCD : null;
				}
			}
			else
			{
				if (IsAccountGroupSpecified(sender, e.Row))
					e.NewValue = null;
			}

		}

		protected virtual bool IsDefaultNonProject(PXCache sender, object row)
		{
			return module == null || !ProjectAttribute.IsPMVisible(sender.Graph, module);
		}

		protected  virtual bool IsImporting(PXCache sender, object row)
		{
			return sender.GetValuePending(row, PXImportAttribute.ImportFlag) != null;
		}

		/// <summary>
		/// When Account has no AccountGroup associated with it the only valid value for the Project is a Non-Project.
		/// </summary>
		protected bool IsAccountGroupSpecified(PXCache sender, object row)
		{
		    if (ForceProjectExplicitly)
		        return true;

			if (AccountType == null)
				return false;
			else
			{
				object accountID = sender.GetValue(row, AccountType.Name);

				if ( accountID == null )
				{
					return false;
				}
				else
				{
					Account account = PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(sender.Graph, accountID);
					if ( account == null )
						return false;
					else
						return account.AccountGroupID != null;
				}
			}			
		}


		/// <summary>
		/// Returns the Non-Project ID.
		/// Non-Project is stored in the table as a row with <see cref="PMProject.nonProject"/>=1.
		/// </summary>
		public static int? NonProject(PXGraph graph)
		{
			PMProject prj = PXSelect<PMProject, Where<PMProject.nonProject, Equal<True>>>.SelectSingleBound(graph, null, null);
			return prj != null ? prj.ContractID : null;
		}

		/// <summary>
		/// Returns the Npn-Project CD.
		/// Non-Project is stored in the table as a row with <see cref="PMProject.nonProject"/>=1.
		/// </summary>
		public static string NonProjectCD(PXGraph graph)
        {
			PMProject prj = PXSelect<PMProject, Where<PMProject.nonProject, Equal<True>>>.SelectSingleBound(graph, null, null);
            return prj != null ? prj.ContractCD : null;
        }	


		/// <summary>
		/// Returns true if the given ID is a Non-Project ID; oterwise false.
		/// </summary>
		public static bool IsNonProject(PXGraph graph, int? projectID)
		{
			PMProject rec =
				PXSelect<PMProject,
					Where<PMProject.nonProject, Equal<True>,
						And<PMProject.contractID, Equal<Required<PMProject.contractID>>>>>
						.SelectSingleBound(graph, null, projectID);
			return rec != null;
		}

		public static bool IsProject(PXGraph graph, int? projectID)
		{
			if (projectID == null)
				return false;

			PMProject rec = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.SelectWindowed(graph, 0, 1, projectID);

			if (rec.BaseType != PMProject.ProjectBaseType.Project)
				return false;

			if (rec.NonProject == true)
				return false;

			return true;
		}

		public static bool IsBillableProject(PXGraph graph, int? projectID)
		{
			if (projectID == null)
				return false;

			PMProject rec = PXSelect<PMProject, Where<PMProject.contractID, Equal<Required<PMProject.contractID>>>>.SelectWindowed(graph, 0, 1, projectID);

			if (rec.BaseType != PMProject.ProjectBaseType.Project)
				return false;

			if (rec.NonProject == true)
				return false;

			return rec.CustomerID != null;
		}
	}

	/// <summary>
	/// Project Default Attribute specific for PO Module. Defaulting of ProjectID field in PO depends on the LineType.
	/// If Line type is of type Non-Stock, Freight or Service ProjectID is defaulted depending on the setting in PMSetup (same as <see cref="ProjectDefaultAttribute"/>). 
	/// For all other type of lines Project is defaulted with Non-Project.
	/// </summary>
	public class POProjectDefaultAttribute : ProjectDefaultAttribute
	{
		protected readonly Type lineType;
		
		public POProjectDefaultAttribute(Type lineType)
			:base(BatchModule.PO)
		{
			this.lineType = lineType;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			sender.Graph.FieldUpdated.AddHandler(lineType, lineType.Name,
				(cache,e)=>
			 {
			  bool isDefault = IsDefaultNonProject(cache, e.Row);
				if (!isDefault && 
					IsNonProject(cache.Graph, (int?)cache.GetValuePending(e.Row,_FieldName)))
					cache.SetValuePending(e.Row, _FieldName, null);

				if(isDefault && cache.GetValuePending(e.Row, _FieldName) == null)
					 cache.SetDefaultExt(e.Row, _FieldName);
			});
		}

		protected override bool IsDefaultNonProject(PXCache sender, object row)
		{
			string poLineType = (string)sender.GetValue(row, lineType.Name);	
			switch (poLineType)
			{
				case PO.POLineType.NonStock:
				case PO.POLineType.Freight:
				case PO.POLineType.Service:
                    return base.IsDefaultNonProject(sender, row);

				default:
					return true;
			}			
		}

	}

    /// <summary>
    /// Project Default Attribute specific for GL Module. Defaulting of ProjectID field in GL depends on the Ledger type. 
    /// Budget and Report Ledgers do not require Project and hense it is always defaulted with Non-Project for these ledgers.
    /// </summary>
    public class GLProjectDefaultAttribute : ProjectDefaultAttribute
    {
        protected readonly Type ledgerType;
    	public GLProjectDefaultAttribute(Type ledgerType)
            : base(BatchModule.GL)
        {
            this.ledgerType = ledgerType;
		}

        public override void CacheAttached(PXCache sender)
        {
            base.CacheAttached(sender);
            sender.Graph.FieldUpdated.AddHandler(ledgerType, ledgerType.Name,
                (cache, e) =>
                {
                    bool isDefault = IsDefaultNonProject(cache, e.Row);
                    if (!isDefault &&
                        IsNonProject(cache.Graph, (int?)cache.GetValuePending(e.Row, _FieldName)))
                        cache.SetValuePending(e.Row, _FieldName, null);
                    if (isDefault && cache.GetValuePending(e.Row, _FieldName) == null)
                        cache.SetDefaultExt(e.Row, _FieldName);
                });
		}

        protected override bool IsDefaultNonProject(PXCache sender, object row)
        {
            object ledgerID = sender.GetValue(row, ledgerType.Name);
            Ledger ledger = PXSelect<Ledger, Where<Ledger.ledgerID, Equal<Required<Ledger.ledgerID>>>>.Select(sender.Graph, ledgerID);

			if (ledger != null && (ledger.BalanceType == LedgerBalanceType.Report || ledger.BalanceType == LedgerBalanceType.Budget))
			{
				return true;
			}
			else
				return base.IsDefaultNonProject(sender, row);

        }



    }

	/// <summary>
	/// Displays all AccountGroups sorted by SortOrder.
	/// </summary>
	[PXDBInt()]
	[PXUIField(DisplayName = "Account Group", Visibility = PXUIVisibility.Visible)]
	public class AccountGroupAttribute : AcctSubAttribute
	{
		public const string DimensionName = "ACCGROUP";
		protected Type showGLAccountGroups;
			

		public AccountGroupAttribute():this(typeof(Where<PMAccountGroup.groupID, IsNotNull>))
		{	
		}

		public AccountGroupAttribute(Type WhereType)
		{
			Type SearchType =
				BqlCommand.Compose(
				typeof(Search<,,>),
				typeof(PMAccountGroup.groupID),
				WhereType,
				typeof(OrderBy<Asc<PMAccountGroup.sortOrder>>)
				);
			
			PXDimensionSelectorAttribute select = new PXDimensionSelectorAttribute(DimensionName, SearchType, typeof(PMAccountGroup.groupCD),
				typeof(PMAccountGroup.groupCD), typeof(PMAccountGroup.description), typeof(PMAccountGroup.type), typeof(PMAccountGroup.isActive));
			select.DescriptionField = typeof(PMAccountGroup.description);

			_Attributes.Add(select);
			_SelAttrIndex = _Attributes.Count - 1;
		}

	}


	/// <summary>
	/// Base attribute for AccountGroupCD field. Aggregates PXFieldAttribute, PXUIFieldAttribute and DimensionSelector without any restriction.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Account Group", Visibility = PXUIVisibility.Visible)]
	public class AccountGroupRawAttribute : AcctSubAttribute
	{
		public AccountGroupRawAttribute()
			: base()
		{
			PXDimensionAttribute attr = new PXDimensionAttribute(AccountGroupAttribute.DimensionName);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	/// <summary>
	/// Inventory Selector that allows to specify an OTHER InventoryID saving 0 in the table.
	/// </summary>  
    [Serializable]
	public class PMInventorySelectorAttribute : PXSelectorAttribute
    {
        public PMInventorySelectorAttribute(Type search):base(search)
        {
        }

        [Serializable]
        [PXHidden]
        public partial class Cogs : Account
        {
            #region AccountID
            public new abstract class accountID : PX.Data.IBqlField
            {
            }
            #endregion
            #region Type
            public new abstract class type : PX.Data.IBqlField
            {
            }
            #endregion
            #region AccountGroupID
            public new abstract class accountGroupID : PX.Data.IBqlField
            {
            }
            #endregion
        }

        [Serializable]
        [PXHidden]
        public partial class Exp : Account
        {
            #region AccountID
            public new abstract class accountID : PX.Data.IBqlField
            {
            }
            #endregion
            #region Type
            public new abstract class type : PX.Data.IBqlField
            {
            }
            #endregion
            #region AccountGroupID
            public new abstract class accountGroupID : PX.Data.IBqlField
            {
            }
            #endregion
        }

        [Serializable]
        [PXHidden]
        public partial class Sale : Account
        {
            #region AccountID
            public new abstract class accountID : PX.Data.IBqlField
            {
            }
            #endregion
            #region Type
            public new abstract class type : PX.Data.IBqlField
            {
            }
            #endregion
            #region AccountGroupID
            public new abstract class accountGroupID : PX.Data.IBqlField
            {
            }
            #endregion
        }

        private const string EmptyComponentCD = "<OTHER>";
        public const int EmptyInventoryID = 0;
                       
        public override void FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (object.Equals(PMProjectStatus.EmptyInventoryID, e.NewValue))
                return;

            base.FieldVerifying(sender, e);
        }

        public override void SubstituteKeyFieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (e.NewValue == null || object.Equals(EmptyComponentCD, e.NewValue))
            {
                e.NewValue = PMProjectStatus.EmptyInventoryID;
            }
            else
                base.SubstituteKeyFieldUpdating(sender, e);
        }

        public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            base.FieldSelecting(sender, e);
            if (e.ReturnValue == null || object.Equals(PMProjectStatus.EmptyInventoryID, e.ReturnValue) || object.Equals(PMProjectStatus.EmptyInventoryID.ToString(), e.ReturnValue))
            {
                e.ReturnValue = PXMessages.LocalizeNoPrefix(EmptyComponentCD);
            }
        }
    }

   	/// <summary>
	/// Same as INUnit with support for InventoryID=0 
	/// </summary>
	[PXDBString(6, IsUnicode = true, InputMask = ">aaaaaa")]
	[PXUIField(DisplayName = "UOM", Visibility = PXUIVisibility.Visible)]
	public class PMUnitAttribute : AcctSub2Attribute
	{		
		public PMUnitAttribute(Type InventoryType)
			: base()
		{
			Type searchType = BqlCommand.Compose(
				typeof(Search4<,,>),
				typeof(INUnit.fromUnit),
				typeof(Where<,,>),
				typeof(INUnit.unitType),
				typeof(Equal<INUnitType.inventoryItem>),
				typeof(And<,,>),
				typeof(INUnit.inventoryID),
				typeof(Equal<>),
				typeof(Optional<>),
				InventoryType,
				typeof(Or<,,>),
				typeof(INUnit.unitType),
				typeof(Equal<INUnitType.global>),
				typeof(And<,>),
				typeof(Optional<>),
				InventoryType,
				typeof(Equal<>),
				typeof(Zero),
				typeof(Aggregate<GroupBy<INUnit.fromUnit>>));

			PXSelectorAttribute attr = new PXSelectorAttribute(searchType);
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}
	}

	/// <summary>
	/// Attribute for Subaccount field. Aggregates PXFieldAttribute, PXUIFieldAttribute and DimensionSelector without any restriction.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount Mask", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
	public sealed class PMSubAccountMaskAttribute : AcctSubAttribute
	{
		private const string _DimensionName = "SUBACCOUNT";
		private const string _MaskName = "PMSETUP";
		public PMSubAccountMaskAttribute()
			: base()
		{
			PXDimensionMaskAttribute attr = new PXDimensionMaskAttribute(_DimensionName, _MaskName, PMAcctSubDefault.MaskSource, new PMAcctSubDefault.SubListAttribute().AllowedValues, new PMAcctSubDefault.SubListAttribute().AllowedLabels);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields)
			where Field : IBqlField
		{
			try
			{
				return PXDimensionMaskAttribute.MakeSub<Field>(graph, mask, new PMAcctSubDefault.SubListAttribute().AllowedValues, 0, sources);
			}
			catch (PXMaskArgumentException ex)
			{
				PXCache cache = graph.Caches[BqlCommand.GetItemType(fields[ex.SourceIdx])];
				string fieldName = fields[ex.SourceIdx].Name;
				throw new PXMaskArgumentException(new PMAcctSubDefault.SubListAttribute().AllowedLabels[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
			}

			
		}
	}

	
	public class PMAcctSubDefault
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

		public class SubListAttribute : CustomListAttribute
		{
			public SubListAttribute()
				: base(new string[] { MaskSource, AllocationStep, ProjectDefault, TaskDefault }, new string[] { Messages.MaskSource, Messages.AllocationStep, Messages.ProjectDefault, Messages.TaskDefault })
			{
			}
		}

		public const string MaskSource = "S";
		public const string AllocationStep = "A";
		public const string ProjectDefault = "P";
		public const string TaskDefault = "T";
	}

	/// <summary>
	/// Attribute for Subaccount field. Aggregates PXFieldAttribute, PXUIFieldAttribute and DimensionSelector without any restriction.
	/// Used in PM Billing to create a subaccount mask.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount Mask", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
	public sealed class PMBillSubAccountMaskAttribute : AcctSubAttribute
	{
		private const string _DimensionName = "SUBACCOUNT";
		private const string _MaskName = "PMBILL";
		public PMBillSubAccountMaskAttribute()
			: base()
		{
			PXDimensionMaskAttribute attr = new PXDimensionMaskAttribute(_DimensionName, _MaskName, AcctSubDefault.BillingRule, new AcctSubDefault.BillingSubListAttribute().AllowedValues, new AcctSubDefault.BillingSubListAttribute().AllowedLabels);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields)
			where Field : IBqlField
		{
			try
			{
				return PXDimensionMaskAttribute.MakeSub<Field>(graph, mask, new AcctSubDefault.BillingSubListAttribute().AllowedValues, 0, sources);
			}
			catch (PXMaskArgumentException ex)
			{
				PXCache cache = graph.Caches[BqlCommand.GetItemType(fields[ex.SourceIdx])];
				string fieldName = fields[ex.SourceIdx].Name;
				throw new PXMaskArgumentException(new AcctSubDefault.BillingSubListAttribute().AllowedLabels[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
			}
		}
	}

	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Subaccount Mask", Visibility = PXUIVisibility.Visible)]
	public sealed class PMRecurentBillSubAccountMaskAttribute : AcctSubAttribute
	{
		private string _DimensionName = "SUBACCOUNT";
		private string _MaskName = "PMRECBILL";
		public PMRecurentBillSubAccountMaskAttribute()
			: base()
		{
			PXDimensionMaskAttribute attr = new PXDimensionMaskAttribute(_DimensionName, _MaskName, AcctSubDefault.RecurentBilling, new AcctSubDefault.RecurentBillingSubListAttribute().AllowedValues, new AcctSubDefault.RecurentBillingSubListAttribute().AllowedLabels);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields)
			where Field : IBqlField
		{
			try
			{
				return PXDimensionMaskAttribute.MakeSub<Field>(graph, mask, new AcctSubDefault.RecurentBillingSubListAttribute().AllowedValues, 0, sources);
			}
			catch (PXMaskArgumentException ex)
			{
				PXCache cache = graph.Caches[fields[ex.SourceIdx].DeclaringType];
				string fieldName = fields[ex.SourceIdx].Name;
				throw new PXMaskArgumentException(new AcctSubDefault.RecurentBillingSubListAttribute().AllowedLabels[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
			}
		}
	}
	
	
	/// <summary>
	/// Attribute for Subaccount field. Aggregates PXFieldAttribute, PXUIFieldAttribute and DimensionSelector without any restriction.
	/// Used in PM Billing to create a subaccount mask for Expenses.
	/// </summary>
	[PXDBString(30, IsUnicode = true, InputMask = "")]
	[PXUIField(DisplayName = "Combine Expense Sub. From", Visibility = PXUIVisibility.Visible, FieldClass = _DimensionName)]
	public sealed class SubAccountMaskAttribute : AcctSubAttribute
	{
		private const string _DimensionName = "SUBACCOUNT";
		private const string _MaskName = "PMSETUP";
		public SubAccountMaskAttribute()
			: base()
		{
			PXDimensionMaskAttribute attr = new PXDimensionMaskAttribute(_DimensionName, _MaskName, AcctSubDefault.Inventory, new AcctSubDefault.SubListAttribute().AllowedValues, new AcctSubDefault.SubListAttribute().AllowedLabels);
			attr.ValidComboRequired = false;
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
		}

		public static string MakeSub<Field>(PXGraph graph, string mask, object[] sources, Type[] fields)
			where Field : IBqlField
		{
			try
			{
				return PXDimensionMaskAttribute.MakeSub<Field>(graph, mask, new AcctSubDefault.SubListAttribute().AllowedValues, 0, sources);
			}
			catch (PXMaskArgumentException ex)
			{
				PXCache cache = graph.Caches[BqlCommand.GetItemType(fields[ex.SourceIdx])];
				string fieldName = fields[ex.SourceIdx].Name;
				throw new PXMaskArgumentException(new AcctSubDefault.SubListAttribute().AllowedLabels[ex.SourceIdx], PXUIFieldAttribute.GetDisplayName(cache, fieldName));
			}


		}
	}

	public class AcctSubDefault
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

		public class SubListAttribute : CustomListAttribute
		{
			public SubListAttribute()
				: base(new string[] { Inventory, Project, Task, Employee }, new string[] { Messages.AccountSource_InventoryItem, Messages.AccountSource_Project, Messages.AccountSource_Task, Messages.AccountSource_Resource })
			{
			}
		}

		public class BillingSubListAttribute : CustomListAttribute
		{
			public BillingSubListAttribute()
				: base(new string[] { Source, BillingRule, Project, Task, Employee }, new string[] { Messages.MaskSource, Messages.AccountSource_BillingRule, Messages.AccountSource_Project, Messages.AccountSource_Task, Messages.AccountSource_Resource })
			{
			}
		}

		public class RecurentBillingSubListAttribute : CustomListAttribute
		{
			public RecurentBillingSubListAttribute()
				: base(new string[] { RecurentBilling, Project, Task }, new string[] { Messages.AccountSource_RecurentBillingItem, Messages.AccountSource_Project, Messages.AccountSource_Task })
			{
			}
		}

		public const string Source = "S";
		public const string BillingRule = "B";
		public const string RecurentBilling = "B";
		public const string Inventory = "I";
		public const string Project = "P";
		public const string Task = "T";
		public const string Employee = "E";
	}


	public class ProjectStatusAccumAttribute : PXAccumulatorAttribute
	{
		public ProjectStatusAccumAttribute()
		{
			base._SingleRecord = true;
		}
		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}

			PMProjectStatusAccum item = (PMProjectStatusAccum)row;

			columns.Update<PMProjectStatusAccum.actualQty>(item.ActualQty, PXDataFieldAssign.AssignBehavior.Summarize);
			columns.Update<PMProjectStatusAccum.actualAmount>(item.ActualAmount, PXDataFieldAssign.AssignBehavior.Summarize);
						
			return true;
		}
	}
	
	[PXDBInt]
	[PXUIField(DisplayName = "Account", Visibility = PXUIVisibility.Visible)]
	public class PMAccountAttribute : AcctSubAttribute
	{		
		public PMAccountAttribute()
		{
			/*
			 
			SEARCH Account.accountID
			WHERE 
			Account.active = True
			AND 
			(
			  (
				( Current_PMAccountGroup.type = AccountType.asset OR Current_PMAccountGroup.type = AccountType.liability )
				AND 
				( Account.type = AccountType.asset OR Account.type = AccountType.liability )
			  )
			  OR
			  (
				( Current_PMAccountGroup.type = AccountType.expense OR Current_PMAccountGroup.type = AccountType.income )
				AND 
				( Account.type = AccountType.expense OR Account.type = AccountType.income )
			  )
			)
			 
			 */

			Type SearchType = typeof(Search<Account.accountID,
				Where2<Match<Current<AccessInfo.userName>>,
				And<Account.active, Equal<boolTrue>,
				And2<
					Where2<Where<Current<PMAccountGroup.type>, Equal<AccountType.asset>,
							Or<Current<PMAccountGroup.type>, Equal<AccountType.liability>>>,
						And<Where<Account.type, Equal<AccountType.asset>, 
							Or<Account.type, Equal<AccountType.liability>>>>>,
					Or2<Where<Current<PMAccountGroup.type>, Equal<AccountType.expense>,
							Or<Current<PMAccountGroup.type>, Equal<AccountType.income>>>,
						And<Where<Account.type, Equal<AccountType.expense>,
							Or<Account.type, Equal<AccountType.income>>>>>>>>>);
			
			PXDimensionSelectorAttribute attr = new PXDimensionSelectorAttribute(AccountAttribute.DimensionName, SearchType, typeof(Account.accountCD));
			attr.CacheGlobal = true;
			attr.DescriptionField = typeof(Account.description);
			_Attributes.Add(attr);
			_SelAttrIndex = _Attributes.Count - 1;
			this.Filterable = true;
		}
	}

    public class PMDetailAccumAttribute : PXAccumulatorAttribute
    {
        public PMDetailAccumAttribute()
        {
            base._SingleRecord = true;
        }
        protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
        {
            if (!base.PrepareInsert(sender, row, columns))
            {
                return false;
            }

            columns.UpdateOnly = true;

            PMDetailAcum item = (PMDetailAcum)row;
            columns.Update<PMDetailAcum.used>(item.Used, PXDataFieldAssign.AssignBehavior.Summarize);
            columns.Update<PMDetailAcum.usedTotal>(item.UsedTotal, PXDataFieldAssign.AssignBehavior.Summarize);

            //columns.Update<PMDetailAcum.contractID>(item.ContractID, PXDataFieldAssign.AssignBehavior.Initialize);
            //columns.Update<PMDetailAcum.inventoryID>(item.InventoryID, PXDataFieldAssign.AssignBehavior.Initialize);

            return true;
        }
    }

	public static class GroupTypes
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Project, Task, AccountGroup, Equipment },
				new string[] { Messages.GroupTypes_Project, Messages.GroupTypes_Task, Messages.GroupTypes_AccountGroup, Messages.GroupTypes_Equipment }) { ; }
		}
		public const string Project = "PROJECT";
		public const string Task = "TASK";
		public const string AccountGroup = "ACCGROUP";
		public const string Transaction = "PROTRAN";
		public const string Equipment = "EQUIPMENT";

		public class ProjectType : Constant<string>
		{
			public ProjectType() : base(GroupTypes.Project) { ;}
		}

		public class TaskType : Constant<string>
		{
			public TaskType() : base(GroupTypes.Task) { ;}
		}

		public class AccountGroupType : Constant<string>
		{
			public AccountGroupType() : base(GroupTypes.AccountGroup) { ;}
		}
		public class TransactionType : Constant<string>
		{
			public TransactionType() : base(GroupTypes.Transaction) { ;}
		}
		public class EquipmentType : Constant<string>
		{
			public EquipmentType() : base(GroupTypes.Equipment) { ;}
		}
	}

	
	[Serializable]
    [DebuggerDisplay("{LineNbr} - [{AccountGroupID},{ProjectID},{ProjectTaskID},{InventoryID}] = {Amount}")]
	public partial class PMProjectStatusEx : PX.Data.IBqlTable, IPMProjectStatus
	{
        #region LineNbr
        public abstract class lineNbr : PX.Data.IBqlField
        {
        }
        protected Int32? _LineNbr;
        [PXDBInt(IsKey = true)]
        public virtual Int32? LineNbr
        {
            get
            {
                return this._LineNbr;
            }
            set
            {
                this._LineNbr = value;
            }
        }
        #endregion

		#region ProjectID
		public abstract class projectID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectID;
		[PXDefault]
		[PXDBInt()]
		public virtual Int32? ProjectID
		{
			get
			{
				return this._ProjectID;
			}
			set
			{
				this._ProjectID = value;
			}
		}
		#endregion
		#region ProjectTaskID
		public abstract class projectTaskID : PX.Data.IBqlField
		{
		}
		protected Int32? _ProjectTaskID;
		[PXDefault]
        [PXDBInt()]
		[PXSelector(typeof(Search<PMTask.taskID>))]
		public virtual Int32? ProjectTaskID
		{
			get
			{
				return this._ProjectTaskID;
			}
			set
			{
				this._ProjectTaskID = value;
			}
		}
		#endregion
		#region AccountGroupID
		public abstract class accountGroupID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountGroupID;
		[PXDefault]
        [PXDBInt()]
		public virtual Int32? AccountGroupID
		{
			get
			{
				return this._AccountGroupID;
			}
			set
			{
				this._AccountGroupID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[PXDefault]
        [PXDBInt()]
		public virtual Int32? InventoryID
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
                
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion
		#region Qty
		public abstract class qty : PX.Data.IBqlField
		{
		}
		protected Decimal? _Qty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Budgeted Qty.")]
		[PXUIVerify(typeof(Where<Current2<qty>, Greater<decimal0>, Or<isProduction, Equal<False>, Or<Selector<PMProjectStatusEx.projectTaskID, PMTask.completedPctMethod>, NotEqual<PMCompletedPctMethod.byQuantity>>>>), PXErrorLevel.Warning, Messages.ValueMustBeGreaterThanZero, true)]
		public virtual Decimal? Qty
		{
			get
			{
				return this._Qty;
			}
			set
			{
				this._Qty = value;
			}
		}
		#endregion
		#region UOM
		public abstract class uOM : PX.Data.IBqlField
		{
		}
		protected String _UOM;
		[PMUnit(typeof(PMProjectStatusEx.inventoryID))]
		public virtual String UOM
		{
			get
			{
				return this._UOM;
			}
			set
			{
				this._UOM = value;
			}
		}
		#endregion
		#region Rate
		public abstract class rate : PX.Data.IBqlField
		{
		}
		protected Decimal? _Rate;
		[PXDBPriceCost]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Rate")]
		public virtual Decimal? Rate
		{
			get
			{
				return this._Rate;
			}
			set
			{
				this._Rate = value;
			}
		}
		#endregion
		#region Amount
		public abstract class amount : PX.Data.IBqlField
		{
		}
		protected Decimal? _Amount;
		[PXDBBaseCury]
		[PXFormula(typeof(Mult<PMProjectStatusEx.qty, PMProjectStatusEx.rate>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Budgeted Amount")]
		[PXUIVerify(typeof(Where<Current2<amount>, Greater<decimal0>, Or<isProduction, Equal<False>, Or<Selector<PMProjectStatusEx.projectTaskID, PMTask.completedPctMethod>, NotEqual<PMCompletedPctMethod.byAmount>>>>), PXErrorLevel.Warning, Messages.ValueMustBeGreaterThanZero, true)]
		public virtual Decimal? Amount
		{
			get
			{
				return this._Amount;
			}
			set
			{
				this._Amount = value;
			}
		}
		#endregion
		#region RevisedQty
		public abstract class revisedQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _RevisedQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Revised Qty.")]
		[PXUIVerify(typeof(Where<Current2<revisedQty>, Greater<decimal0>, Or<isProduction, Equal<False>, Or<Selector<projectTaskID, PMTask.completedPctMethod>, NotEqual<PMCompletedPctMethod.byQuantity>, Or<Selector<projectTaskID, PMTask.status>, Equal<ProjectTaskStatus.planned>>>>>), PXErrorLevel.Warning, Messages.ValueMustBeGreaterThanZero, true)]
		public virtual Decimal? RevisedQty
		{
			get
			{
				return this._RevisedQty;
			}
			set
			{
				this._RevisedQty = value;
			}
		}
		#endregion
		#region RevisedAmount
		public abstract class revisedAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _RevisedAmount;
		[PXDBBaseCury]
		[PXFormula(typeof(Mult<PMProjectStatusEx.revisedQty, PMProjectStatusEx.rate>))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Revised Amount")]
		[PXUIVerify(typeof(Where<Current2<revisedAmount>, Greater<decimal0>, Or<isProduction, Equal<False>, Or<Selector<projectTaskID, PMTask.completedPctMethod>, NotEqual<PMCompletedPctMethod.byAmount>, Or<Selector<projectTaskID, PMTask.status>, Equal<ProjectTaskStatus.planned>>>>>), PXErrorLevel.Warning, Messages.ValueMustBeGreaterThanZero, true)]
		public virtual Decimal? RevisedAmount
		{
			get
			{
				return this._RevisedAmount;
			}
			set
			{
				this._RevisedAmount = value;
			}
		}
		#endregion
		#region ActualQty
		public abstract class actualQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _ActualQty;
		[PXDBQuantity]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Actual Qty.", Enabled = false)]
		public virtual Decimal? ActualQty
		{
			get
			{
				return this._ActualQty;
			}
			set
			{
				this._ActualQty = value;
			}
		}
		#endregion
		#region ActualAmount
		public abstract class actualAmount : PX.Data.IBqlField
		{
		}
		protected Decimal? _ActualAmount;
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Actual Amount", Enabled = false)]
		public virtual Decimal? ActualAmount
		{
			get
			{
				return this._ActualAmount;
			}
			set
			{
				this._ActualAmount = value;
			}
		}
		#endregion
		#region IsProduction
		public abstract class isProduction : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsProduction;
		[PXDefault(false)]
		[PXDBBool()]
		[PXUIField(DisplayName = "Production")]
		public virtual Boolean? IsProduction
		{
			get
			{
				return this._IsProduction;
			}
			set
			{
				this._IsProduction = value;
			}
		}
		#endregion

		#region Performance
		public abstract class performance : PX.Data.IBqlField
		{
		}
		protected Decimal? _Performance;
		[PXDBDecimal(2)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Performance (%)", Enabled = false)]
		public virtual Decimal? Performance
		{
			get
			{
				if (_RevisedAmount != 0)
					return (_ActualAmount / _RevisedAmount) * 100;
				else
					return 0;
			}
		}
		#endregion

		#region TaskStatus
		public abstract class taskStatus : PX.Data.IBqlField
		{
		}
		protected String _TaskStatus;
		[PXDBString(1, IsFixed = true)]
		[ProjectTaskStatus.List()]
		[PXUIField(DisplayName = "Status", Enabled = false)]
		public virtual String TaskStatus
		{
			get
			{
				return this._TaskStatus;
			}
			set
			{
				this._TaskStatus = value;
			}
		}
		#endregion
        #region Type
        public abstract class type : PX.Data.IBqlField
        {
        }
        protected string _Type;
        [PXDBString(1)]
        [PMAccountType.List()]
        [PXUIField(DisplayName = "Type", Enabled=false)]
        public virtual string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }
        #endregion
        #region SortOrder
        public abstract class sortOrder : PX.Data.IBqlField
        {
        }
        protected Int16? _SortOrder;
        [PXDBShort()]
        [PXUIField(DisplayName = "Sort Order")]
        public virtual Int16? SortOrder
        {
            get
            {
                return this._SortOrder;
            }
            set
            {
                this._SortOrder = value;
            }
        }
        #endregion
	}

    public class ProjectStatusSelect<T, Where, OrderBy> : ProjectStatusSelectBase<T, Where, OrderBy>
		where T : PMProjectStatusEx, new()
		where Where : IBqlWhere, new()
        where OrderBy : IBqlOrderBy, new()
	
	{		
     	public ProjectStatusSelect(PXGraph graph)
			: base(graph)
     	{
            var projectStatusCache = _Graph.Caches[typeof(PMProjectStatus)];
            _Graph.Views.Caches.Add(typeof(PMProjectStatus));

            _Graph.RowDeleted.AddHandler<T>(RowDeleted);
            _Graph.RowPersisting.AddHandler<T>(RowPersisting);
            _Graph.FieldUpdated.AddHandler(typeof(T), typeof(PMProjectStatus.projectTaskID).Name, ProjectTaskID_FieldUpdated);
		}

		public ProjectStatusSelect(PXGraph graph, Delegate handle)
			: base(graph, handle)
		{
            var projectStatusCache = _Graph.Caches[typeof(PMProjectStatus)];
            _Graph.Views.Caches.Add(typeof(PMProjectStatus));

			_Graph.RowDeleted.AddHandler<T>(RowDeleted);
			_Graph.RowPersisting.AddHandler<T>(RowPersisting);
			_Graph.FieldUpdated.AddHandler(typeof(T), typeof(PMProjectStatus.projectTaskID).Name, ProjectTaskID_FieldUpdated);
		}
		
		#region Event Handlers
		
		protected virtual void RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
            T row = e.Row as T;
            if (row != null)
            {
                DeleteProjectStatus(row);
                Cache.Remove(row);
            }
		}

		protected virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			T row = e.Row as T;

			Debug.Print("PMProjectStatusEx_RowPersisting {0}", row.ProjectTaskID);

            if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				if (!ProjectStatusExists(row))
				{
					InsertProjectStatus(row);
				}
				else
				{
					if (!UpdateProjectStatus(row))
					{
						throw new PXException(Messages.ValidationFailed);
					}
				}
			}

			foreach (PMProjectStatus item in _Graph.Caches[typeof(PMProjectStatus)].Deleted)
			{
				if (item.AccountGroupID == row.AccountGroupID &&
					item.ProjectID == row.ProjectID &&
					item.ProjectTaskID == row.ProjectTaskID &&
					item.InventoryID == row.InventoryID)
				{
					_Graph.Caches[typeof(PMProjectStatus)].PersistDeleted(item);
				}
			}

			foreach (PMProjectStatus item in _Graph.Caches[typeof(PMProjectStatus)].Inserted)
			{
				if (item.AccountGroupID == row.AccountGroupID &&
					item.ProjectID == row.ProjectID &&
					item.ProjectTaskID == row.ProjectTaskID &&
					item.InventoryID == row.InventoryID)
				{
					_Graph.Caches[typeof(PMProjectStatus)].PersistInserted(item);
				}
			}

			foreach (PMProjectStatus item in _Graph.Caches[typeof(PMProjectStatus)].Updated)
			{
				if (item.AccountGroupID == row.AccountGroupID &&
					item.ProjectID == row.ProjectID &&
					item.ProjectTaskID == row.ProjectTaskID &&
					item.InventoryID == row.InventoryID)
				{
					_Graph.Caches[typeof(PMProjectStatus)].PersistUpdated(item);
				}
			}

			//sender.Remove(row);
			e.Cancel = true;
		}

		protected virtual void ProjectTaskID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			T row = e.Row as T;
			if (row != null)
			{
				PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
							And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, row.ProjectID, row.ProjectTaskID);

				if (task != null)
				{
					row.TaskStatus = task.Status;
				}
			}
		}

		#endregion

		protected virtual void DeleteProjectStatus(T item)
		{
			foreach (PMProjectStatus row in GetProjectStatus(item))
			{
				_Graph.Caches[typeof(PMProjectStatus)].Delete(row);
			}
		}

		protected virtual void InsertProjectStatus(T item)
		{
			//distibute proportionaly amoung active periods that fit the Task [PlannedStartDate, PlannedEndDate] range:

			PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, item.ProjectID, item.ProjectTaskID);
			
			if (task != null)
			{
				List<string> periods = GetPeriods(item, task);

				decimal qtyRaw = item.Qty.GetValueOrDefault() / periods.Count;
				decimal revisedQtyRaw = item.RevisedQty.GetValueOrDefault() / periods.Count;
				decimal amtRaw = item.Amount.GetValueOrDefault() / periods.Count;
				decimal revisedAmtRaw = item.RevisedAmount.GetValueOrDefault() / periods.Count;

				decimal qty = PXDBQuantityAttribute.Round(DistibuteRound(qtyRaw, periods.Count));
				decimal revisedQty = PXDBQuantityAttribute.Round(DistibuteRound(revisedQtyRaw, periods.Count));
				decimal amt = PXCurrencyAttribute.BaseRound(this._Graph, DistibuteRound(amtRaw, periods.Count));
				decimal revisedAmt = PXCurrencyAttribute.BaseRound(this._Graph, DistibuteRound(revisedAmtRaw, periods.Count));

				decimal qtyLast = item.Qty.GetValueOrDefault();
				decimal revisedQtyLast = item.RevisedQty.GetValueOrDefault();
				decimal amtLast = item.Amount.GetValueOrDefault();
				decimal revisedAmtLast = item.RevisedAmount.GetValueOrDefault();

				//1..n-1:
				for (int i = 0; i < periods.Count - 1; i++)
				{
                    PMProjectStatus ps = new PMProjectStatus(); //(PMProjectStatus)_Graph.Caches[typeof(PMProjectStatus)].Insert();
					Transform(ps, item, periods[i], qty, amt, revisedQty, revisedAmt);
					_Graph.Caches[typeof(PMProjectStatus)].Update(ps);
					
					qtyLast -= qty;
					revisedQtyLast -= revisedQty;
					amtLast -= amt;
					revisedAmtLast -= revisedAmt;
				}

				//n:
                PMProjectStatus psLast = new PMProjectStatus(); //(PMProjectStatus)_Graph.Caches[typeof(PMProjectStatus)].Insert();
				Transform(psLast, item, periods[periods.Count - 1], qtyLast, amtLast, revisedQtyLast, revisedAmtLast);
				_Graph.Caches[typeof(PMProjectStatus)].Update(psLast);
			}
		}

		protected virtual bool UpdateProjectStatus(T item)
		{
			List<PMProjectStatus> list = GetProjectStatus(item);
			PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, item.ProjectID, item.ProjectTaskID);

			if (task == null)
			{
				throw new PXException(Messages.TaskNotFound, item.ProjectID, item.ProjectTaskID);
			}

			decimal oldQty = 0, oldAmt = 0, oldRevisedQty = 0, oldRevisedAmt = 0;

			foreach (PMProjectStatus ps in list)
			{
				oldQty += ps.Qty.GetValueOrDefault();
				oldAmt += ps.Amount.GetValueOrDefault();
				oldRevisedQty += ps.RevisedQty.GetValueOrDefault();
				oldRevisedAmt += ps.RevisedAmount.GetValueOrDefault();
			}

			decimal deltaQty = item.Qty.GetValueOrDefault() - oldQty;
			decimal deltaAmt = item.Amount.GetValueOrDefault() - oldAmt;
			decimal deltaRevisedQty = item.RevisedQty.GetValueOrDefault() - oldRevisedQty;
			decimal deltaRevisedAmt = item.RevisedAmount.GetValueOrDefault() - oldRevisedAmt;

			decimal qtyLast = item.Qty.GetValueOrDefault();
			decimal revisedQtyLast = item.RevisedQty.GetValueOrDefault();
			decimal amtLast = item.Amount.GetValueOrDefault();
			decimal revisedAmtLast = item.RevisedAmount.GetValueOrDefault();

			//delta must be distibuted proportionaly 

			for (int i = 0; i < list.Count - 1; i++)
			{
				decimal qtyRaw = 0;
				if (oldQty > 0)
					qtyRaw = list[i].Qty.GetValueOrDefault() + (list[i].Qty.GetValueOrDefault() * deltaQty / oldQty);

				decimal revisedQtyRaw = 0;
				if (oldRevisedQty > 0)
					revisedQtyRaw = list[i].RevisedQty.GetValueOrDefault() + (list[i].RevisedQty.GetValueOrDefault() * deltaRevisedQty / oldRevisedQty);

				decimal amtRaw = 0;
				if (oldAmt > 0)
					amtRaw = list[i].Amount.GetValueOrDefault() + (list[i].Amount.GetValueOrDefault() * deltaAmt / oldAmt);

				decimal revisedAmtRaw = 0;
				if (oldRevisedAmt > 0)
					revisedAmtRaw = list[i].RevisedAmount.GetValueOrDefault() + (list[i].RevisedAmount.GetValueOrDefault() * deltaRevisedAmt / oldRevisedAmt);


				decimal qty = PXDBQuantityAttribute.Round(DistibuteRound(qtyRaw, list.Count - i));
				decimal revisedQty = PXDBQuantityAttribute.Round(DistibuteRound(revisedQtyRaw, list.Count - i));
				decimal amt = PXCurrencyAttribute.BaseRound(this._Graph, DistibuteRound(amtRaw, list.Count - i));
				decimal revisedAmt = PXCurrencyAttribute.BaseRound(this._Graph, DistibuteRound(revisedAmtRaw, list.Count - i));

				PMProjectStatus copy = PXCache<PMProjectStatus>.CreateCopy(list[i]);

				if (task.Status == ProjectTaskStatus.Planned)
				{
					copy.Qty = qty;
					copy.Amount = amt;
					copy.RevisedQty = revisedQty;
					copy.RevisedAmount = revisedAmt;
                }
				else
				{
					copy.Qty = qty;
					copy.Amount = amt;
					copy.RevisedQty = revisedQty;
					copy.RevisedAmount = revisedAmt;
				}

				copy.UOM = item.UOM;
				copy.Rate = item.Rate;
				copy.Description = item.Description;
				copy.IsProduction = item.IsProduction;
				_Graph.Caches[typeof(PMProjectStatus)].Update(copy);

				qtyLast -= qty;
				revisedQtyLast -= revisedQty;
				amtLast -= amt;
				revisedAmtLast -= revisedAmt;
			}

			PMProjectStatus copyLast = PXCache<PMProjectStatus>.CreateCopy(list[list.Count - 1]);

			if (task.Status == ProjectTaskStatus.Planned)
			{
				copyLast.Qty = qtyLast;
				copyLast.Amount = amtLast;
				copyLast.RevisedQty = revisedQtyLast;
				copyLast.RevisedAmount = revisedAmtLast;
			}
			else
			{
				copyLast.Qty = qtyLast;
				copyLast.Amount = amtLast;
				copyLast.RevisedQty = revisedQtyLast;
				copyLast.RevisedAmount = revisedAmtLast;
			}

			copyLast.UOM = item.UOM;
			copyLast.Rate = item.Rate;
			copyLast.Description = item.Description;
			copyLast.IsProduction = item.IsProduction;
			_Graph.Caches[typeof(PMProjectStatus)].Update(copyLast);

			return true;
		}

		private decimal DistibuteRound(decimal value, int rowCount)
		{
			int valDigit = (int)Math.Log10((double)value);
			int rowDigit = (int)Math.Log10((double)rowCount);
			int roundIndex = rowDigit - valDigit + 1;
			if (rowCount > 1 && value > 0)
			{
				if (roundIndex >= 0)
				{
					return Math.Round(value, roundIndex);
				}
				else
				{
					return Math.Round(value / (decimal)Math.Pow(10, -roundIndex)) * (decimal)Math.Pow(10, -roundIndex);
				}
			}
			else
			{
				return value;
			}
		}

		private void Transform(PMProjectStatus ps, T item, string periodID, decimal qty, decimal amt, decimal revisedQty, decimal revisedAmt)
		{
			ps.PeriodID = periodID;
			ps.ProjectID = item.ProjectID;
			ps.ProjectTaskID = item.ProjectTaskID;
			ps.AccountGroupID = item.AccountGroupID;
			ps.InventoryID = item.InventoryID;
			ps.Description = item.Description;
			ps.UOM = item.UOM;
			ps.Rate = item.Rate;
			ps.IsProduction = item.IsProduction;
			ps.Qty = qty;
			ps.Amount = amt;
			ps.RevisedQty = revisedQty;
			ps.RevisedAmount = revisedAmt;
		}

		private List<string> GetPeriods(T item, PMTask task)
		{
			List<string> list = new List<string>();

			DateTime? startDate = null;
			DateTime? endDate = null;

			if (task.Status == ProjectTaskStatus.Planned)
			{
				startDate = task.PlannedStartDate;
				endDate = task.PlannedEndDate;
			}
			else if (task.Status == ProjectTaskStatus.Active)
			{
				startDate = task.StartDate;
				endDate = task.PlannedEndDate;
			}

			if (startDate != null && endDate != null)
			{
				string startPeriod = FinPeriodIDAttribute.PeriodFromDate(this._Graph, startDate);
				string endPeriod = FinPeriodIDAttribute.PeriodFromDate(this._Graph, endDate);

				PXSelectBase<FinPeriod> select = new PXSelect<FinPeriod,
					Where<FinPeriod.startDate, NotEqual<FinPeriod.endDate>,
					And<FinPeriod.closed, Equal<False>,
					And<FinPeriod.finPeriodID, Between<Required<FinPeriod.finPeriodID>, Required<FinPeriod.finPeriodID>>>>>,
					OrderBy<Asc<FinPeriod.finPeriodID>>>(this._Graph);

				foreach (FinPeriod fp in select.Select(startPeriod, endPeriod))
				{
					list.Add(fp.FinPeriodID);
				}
			}
			else
			{
				if (endDate != null)
				{
					list.Add(FinPeriodIDAttribute.PeriodFromDate(this._Graph, startDate));
				}
				else if (startDate != null)
				{
					list.Add(FinPeriodIDAttribute.PeriodFromDate(this._Graph, startDate));
				}
			}

			if (list.Count == 0)
			{
				list.Add(FinPeriodIDAttribute.PeriodFromDate(this._Graph, this._Graph.Accessinfo.BusinessDate));
			}

			return list;
		}
				
		private bool ProjectStatusExists(T row)
		{
			PXSelectBase<PMProjectStatus> select = new PXSelect<PMProjectStatus,
				Where<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
				And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
				And<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>,
				And<PMProjectStatus.inventoryID, Equal<Required<PMProjectStatus.inventoryID>>>>>>>(this._Graph);

			PMProjectStatus ps = select.SelectWindowed(0, 1, row.ProjectID, row.ProjectTaskID, row.AccountGroupID, row.InventoryID);

			return ps != null;

		}

        private List<PMProjectStatus> GetProjectStatus(T row)
        {
            PXSelectBase<PMProjectStatus> select = new PXSelect<PMProjectStatus,
                Where<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
                And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
                And<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>,
                And<PMProjectStatus.inventoryID, Equal<Required<PMProjectStatus.inventoryID>>>>>>>(this._Graph);

            PXResultset<PMProjectStatus> resultset = select.Select(row.ProjectID, row.ProjectTaskID, row.AccountGroupID, row.InventoryID);

            List<PMProjectStatus> list = new List<PMProjectStatus>(resultset.Count);
            foreach (PMProjectStatus item in resultset)
            {
                list.Add(item);
            }

            return list;
        }

		private List<PMProjectStatus> GetProjectStatusInActivePeriods(T row)
		{
			PXSelectBase<PMProjectStatus> select = new PXSelectJoin<PMProjectStatus,
				InnerJoin<FinPeriod, On<PMProjectStatus.periodID, Equal<FinPeriod.finPeriodID>>>,
				Where<PMProjectStatus.projectID, Equal<Required<PMProjectStatus.projectID>>,
				And<PMProjectStatus.projectTaskID, Equal<Required<PMProjectStatus.projectTaskID>>,
				And<PMProjectStatus.accountGroupID, Equal<Required<PMProjectStatus.accountGroupID>>,
				And<PMProjectStatus.inventoryID, Equal<Required<PMProjectStatus.inventoryID>>,
				And<FinPeriod.active, Equal<True>,
				And<FinPeriod.closed, Equal<False>>>>>>>>(this._Graph);

			PXResultset<PMProjectStatus> resultset = select.Select(row.ProjectID, row.ProjectTaskID, row.AccountGroupID, row.InventoryID);

			List<PMProjectStatus> list = new List<PMProjectStatus>(resultset.Count);
			foreach (PXResult<PMProjectStatus, FinPeriod> res in resultset)
			{
				PMProjectStatus item = (PMProjectStatus)res;
				FinPeriod fp = (FinPeriod)res;

				list.Add(item);
			}

			return list;
		}

	}

	public class ProjectStatusSelectBase<T, Where, OrderBy> : PXSelect<T, Where, OrderBy>
		where T : class, IBqlTable, IPMProjectStatus, new() 
		where Where : IBqlWhere, new()
        where OrderBy : IBqlOrderBy, new()
	{
		protected PXSetup<Company> CompanySetup;

		public ProjectStatusSelectBase(PXGraph graph, Delegate handle)
			: base(graph, handle)
		{
			CompanySetup = new PXSetup<Company>(_Graph);

			_Graph.RowDeleting.AddHandler<T>(RowDeleting);
			_Graph.RowUpdated.AddHandler<T>(RowUpdated);
			_Graph.RowInserted.AddHandler<T>(RowInserted);
			_Graph.FieldUpdated.AddHandler(typeof(T), typeof(PMProjectStatus.inventoryID).Name, InventoryID_FieldUpdated);
			_Graph.FieldDefaulting.AddHandler(typeof(T), typeof(PMProjectStatus.rate).Name, Rate_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler(typeof(T), typeof(PMProjectStatus.description).Name, Description_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler(typeof(T), typeof(PMProjectStatus.uOM).Name, UOM_FieldDefaulting);
			_Graph.FieldUpdated.AddHandler(typeof(T), typeof(PMProjectStatus.uOM).Name, UOM_FieldUpdated);

			_Graph.RowInserted.AddHandler<PMProjectStatus>(_RowInserted);
			_Graph.RowUpdated.AddHandler<PMProjectStatus>(_RowUpdated);
			_Graph.RowDeleted.AddHandler<PMProjectStatus>(_RowDeleted);
		}

		private void _RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			AddHistory((PMProjectStatus)e.Row);
		}
		private void _RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			SubtractHistory(((PMProjectStatus)e.OldRow));
			AddHistory((PMProjectStatus)e.Row);
		}
		private void _RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			SubtractHistory((PMProjectStatus)e.Row);
		}


		public ProjectStatusSelectBase(PXGraph graph):base(graph)
		{
			CompanySetup = new PXSetup<Company>(_Graph);

			_Graph.RowDeleting.AddHandler<T>(RowDeleting);
			_Graph.RowUpdated.AddHandler<T>(RowUpdated);
			_Graph.RowInserted.AddHandler<T>(RowInserted);
			_Graph.FieldUpdated.AddHandler(typeof(T), typeof(PMProjectStatus.inventoryID).Name, InventoryID_FieldUpdated);
			_Graph.FieldDefaulting.AddHandler(typeof(T), typeof(PMProjectStatus.rate).Name, Rate_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler(typeof(T), typeof(PMProjectStatus.description).Name, Description_FieldDefaulting);
			_Graph.FieldDefaulting.AddHandler(typeof(T), typeof(PMProjectStatus.uOM).Name, UOM_FieldDefaulting);
			_Graph.FieldUpdated.AddHandler(typeof(T), typeof(PMProjectStatus.uOM).Name, UOM_FieldUpdated);

			_Graph.RowInserted.AddHandler<PMProjectStatus>(_RowInserted);
			_Graph.RowUpdated.AddHandler<PMProjectStatus>(_RowUpdated);
			_Graph.RowDeleted.AddHandler<PMProjectStatus>(_RowDeleted);
		}

		#region Event Handlers
				
		protected virtual void RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			T row = e.Row as T;
			if (row != null)
			{
				if (row.ActualQty > 0 || row.ActualAmount > 0)
				{
					throw new PXException(Messages.HasRollupData);
				}

				if (sender.GetStatus(row) != PXEntryStatus.Inserted && sender.GetStatus(row) != PXEntryStatus.InsertedDeleted)
				{
					PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
								And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, row.ProjectID, row.ProjectTaskID);
					
					if (task != null && task.IsActive == true && task.IsCancelled == false)
					{
						throw new PXException(Messages.OnlyPlannedCanbeDeleted);
					}
				}
			}
		}

		protected virtual void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			T row = e.Row as T;
			if (row != null)
			{
				PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
							And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, row.ProjectID, row.ProjectTaskID);

				if (task != null && task.Status == ProjectTaskStatus.Planned)
				{
					row.RevisedQty = row.Qty;
					row.RevisedAmount = row.Amount;
				}
			}
		}

		protected virtual void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			T row = e.Row as T;
			T oldRow = e.OldRow as T;
			if (row != null && oldRow != null && (row.Qty != oldRow.Qty || row.Amount != oldRow.Amount))
			{
				PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
							And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, row.ProjectID, row.ProjectTaskID);

				if (task != null && task.Status == ProjectTaskStatus.Planned)
				{
					row.RevisedQty = row.Qty;
					row.RevisedAmount = row.Amount;
				}
			}
		}
						
		protected virtual void InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row != null)
			{
				sender.SetDefaultExt(e.Row, typeof(PMProjectStatus.uOM).Name);
				sender.SetDefaultExt(e.Row, typeof(PMProjectStatus.description).Name);
			}
		}

		protected virtual void Rate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			e.NewValue = GetRate(sender, (T)e.Row);
		}

		protected virtual void Description_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			T row = e.Row as T;
			if (row != null && row.InventoryID != null && row.InventoryID != PMProjectStatus.EmptyInventoryID)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this._Graph, row.InventoryID);
				if (item != null)
				{
					e.NewValue = item.Descr;
				}
			}
		}

		protected virtual void UOM_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			T row = e.Row as T;
			if (row != null && row.InventoryID != null && row.InventoryID != PMProjectStatus.EmptyInventoryID)
			{
				InventoryItem item = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this._Graph, row.InventoryID);
				if (item != null)
				{
					e.NewValue = item.BaseUnit;
				}
			}

			sender.SetDefaultExt(e.Row, typeof(PMProjectStatus.rate).Name);
		}

		protected virtual void UOM_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row != null)
			{
				sender.SetDefaultExt(e.Row, typeof(PMProjectStatus.rate).Name);
			}
		}

		#endregion
		
		protected virtual decimal GetRate(PXCache sender, T row)
		{
			if (row == null)
				return 0;

			if (row != null && row.UOM != null)
			{
				PMAccountGroup accountGroup = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(this._Graph, row.AccountGroupID);
				if (accountGroup != null)
				{
					if (accountGroup.Type == GL.AccountType.Expense)
					{
						//Cost
						PXResult<InventoryItem, INItemCost> r = (PXResult<InventoryItem,INItemCost>)PXSelectJoin<InventoryItem,
							LeftJoin<INItemCost, On<INItemCost.inventoryID, Equal<InventoryItem.inventoryID>>>,
							Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this._Graph, row.InventoryID);
						InventoryItem item = r;
						INItemCost cost = r;
						if (item != null)
						{
							if (item.StkItem == true)
							{
								return INUnitAttribute.ConvertToBase(sender, item.InventoryID, row.UOM, cost.LastCost ?? 0, INPrecision.UNITCOST);
							}
							else
							{
								return INUnitAttribute.ConvertToBase(sender, item.InventoryID, row.UOM, item.StdCost ?? 0, INPrecision.UNITCOST);
							}
						}
					}
					else if (accountGroup.Type == GL.AccountType.Income)
					{
						//Sales Price
						PMTask task = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>,
							And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, row.ProjectID, row.ProjectTaskID);

												
						if (task != null)
						{
							Location customerLoc = PXSelect<Location,
								Where<Location.bAccountID, Equal<Required<Location.bAccountID>>,
								And<Location.locationID, Equal<Required<Location.locationID>>>>>.Select(this._Graph, task.CustomerID, task.LocationID);

							if (customerLoc != null)
							{
								PX.Objects.CM.CurrencyInfo ci = new PX.Objects.CM.CurrencyInfo();
								ci.CuryID = CompanySetup.Current.BaseCuryID;//Accessinfo.BaseCuryID;
								ci.BaseCuryID = CompanySetup.Current.BaseCuryID;//Accessinfo.BaseCuryID;

                                return PX.Objects.AR.ARSalesPriceMaint.CalculateSalesPrice(sender, customerLoc.CPriceClassID, row.InventoryID.Value, ci, row.UOM, _Graph.Accessinfo.BusinessDate.Value, true) ?? 0;
							}
						}
					}

				}


			}

			return 0;
		}

		protected virtual void AddHistory(PMProjectStatus status)
		{
			if (status.PeriodID == null) return;
			
			/*			 
			 * ProjectStatusEx that is used in the graph is a virtual Dac and do not go throught the Persisting/Persisteed event handlers.
			 * In case when Project / ProjectTask is Inserted (not in DB yet) the status.ProjectID and status.PorjectTaskID is a -ve value.
			 * This -ve value is later resolved when ProjectStatus is persisted correctly, but for the History tables real/persisted keys are required.
			 * Selecting PMTask with -ve keys returns entity with real keys.
			 */

			int? realProjectID = status.ProjectID;
			int? realProjectTaskID = status.ProjectTaskID;
			if (realProjectTaskID < 0)
			{
				PMTask realTask = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, status.ProjectID, status.ProjectTaskID);
				if (realTask != null)
				{
					realProjectTaskID = realTask.TaskID;
					realProjectID = realTask.ProjectID;
				}
			}

			PMHistory2Accum hist2 = new PMHistory2Accum();
			hist2.ProjectID = realProjectID;
			hist2.ProjectTaskID = realProjectTaskID;
			hist2.AccountGroupID = status.AccountGroupID;
			hist2.InventoryID = status.InventoryID ?? PMProjectStatus.EmptyInventoryID;
			hist2.PeriodID = status.PeriodID;

			hist2 = (PMHistory2Accum)_Graph.Caches[typeof(PMHistory2Accum)].Insert(hist2);
			hist2.PTDBudgetAmount += status.Amount;
			hist2.PTDBudgetQty += status.Qty;
			hist2.BudgetAmount += status.Amount;
			hist2.BudgetQty += status.Qty;
			hist2.PTDRevisedAmount += status.RevisedAmount;
			hist2.PTDRevisedQty += status.RevisedQty;
			hist2.RevisedAmount += status.RevisedAmount;
			hist2.RevisedQty += status.RevisedQty;

			Debug.Print("Add History {0}: {1} = {2}", status.PeriodID, status.Amount, hist2.BudgetAmount);
		}

		protected virtual void SubtractHistory(PMProjectStatus status)
		{
			if (status.PeriodID == null) return;

			int? realProjectID = status.ProjectID;
			int? realProjectTaskID = status.ProjectTaskID;
			if (realProjectTaskID < 0)
			{
				PMTask realTask = PXSelect<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskID, Equal<Required<PMTask.taskID>>>>>.Select(this._Graph, status.ProjectID, status.ProjectTaskID);
				if (realTask != null)
				{
					realProjectTaskID = realTask.TaskID;
					realProjectID = realTask.ProjectID;
				}
			}

			PMHistory2Accum hist2 = new PMHistory2Accum();
			hist2.ProjectID = realProjectID;
			hist2.ProjectTaskID = realProjectTaskID;
			hist2.AccountGroupID = status.AccountGroupID;
			hist2.InventoryID = status.InventoryID ?? PMProjectStatus.EmptyInventoryID;
			hist2.PeriodID = status.PeriodID;

			hist2 = (PMHistory2Accum)_Graph.Caches[typeof(PMHistory2Accum)].Insert(hist2);
			hist2.PTDBudgetAmount -= status.Amount;
			hist2.PTDBudgetQty -= status.Qty;
			hist2.BudgetAmount -= status.Amount;
			hist2.BudgetQty -= status.Qty;
			hist2.PTDRevisedAmount -= status.RevisedAmount;
			hist2.PTDRevisedQty -= status.RevisedQty;
			hist2.RevisedAmount -= status.RevisedAmount;
			hist2.RevisedQty -= status.RevisedQty;

			Debug.Print("Subtract History {0}: {1} = {2}", status.PeriodID, status.Amount, hist2.BudgetAmount);
		}

	}

	#region projectType
	public sealed class projectType : Constant<string>
	{
		public projectType()
			: base(typeof(PMProject).FullName)
		{
		}
	}
	#endregion


}
