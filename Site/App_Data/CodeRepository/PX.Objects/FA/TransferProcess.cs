using System;
using System.Collections;
using System.Collections.Generic;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.CS;

namespace PX.Objects.FA
{
	public class TransferProcess : PXGraph<TransferProcess>
	{
		public PXCancel<TransferFilter> Cancel;
		public PXFilter<TransferFilter> Filter;
		[PXFilterable]
		[PX.SM.PXViewDetailsButton(typeof(FixedAsset.assetCD), WindowMode = PXRedirectHelper.WindowMode.NewWindow)]
		public PXFilteredProcessingJoin<FixedAsset, TransferFilter, LeftJoin<FADetails, On<FixedAsset.assetID, Equal<FADetails.assetID>>, LeftJoin<Account, On<FixedAsset.fAAccountID, Equal<Account.accountID>>, LeftJoin<FALocationHistory, On<FALocationHistory.assetID, Equal<FADetails.assetID>, And<FALocationHistory.revisionID, Equal<FADetails.locationRevID>>>>>>> Assets;

		public PXSelect<BAccount> Baccount;
		public PXSelect<Vendor> Vendor;
		public PXSelect<EPEmployee> Employee;
		public PXSelect<FixedAsset> AssetSelect;
		public PXSelect<FADetails> Details;
		public PXSelect<FARegister> Register;
		public PXSelect<FATran> FATransactions;
		public PXSelect<FALocationHistory> Lochist;
 
		public PXSetup<FASetup> fasetup; 

		public TransferProcess()
		{
			FASetup setup = fasetup.Current;

			if (fasetup.Current.UpdateGL != true)
			{
				throw new PXSetupNotEnteredException<FASetup>(Messages.OperationNotWorkInInitMode, PXUIFieldAttribute.GetDisplayName<FASetup.updateGL>(fasetup.Cache));
			}

			PXUIFieldAttribute.SetDisplayName<FADetails.receiptDate>(Caches[typeof(FADetails)], Messages.AcquiredDate);
			PXUIFieldAttribute.SetDisplayName<Account.accountClassID>(Caches[typeof(Account)], Messages.FixedAssetsAccountClass);

			if (fasetup.Current.AutoReleaseTransfer != true)
			{
				Assets.SetProcessCaption(Messages.Prepare);
				Assets.SetProcessAllCaption(Messages.PrepareAll);
			}
		}

		[PXString(6, IsFixed = true)]
		[PXUIField(DisplayName = "Transfer Period", Enabled = false)]
		[PXDBScalar(typeof(Search<FABookBalanceTransfer.finPeriodID, Where<FADetails.assetID, Equal<FABookBalanceTransfer.assetID>>, OrderBy<Desc<FABookBalanceTransfer.updateGL>>>))]
		[FinPeriodIDFormatting]
		protected virtual void FADetails_TransferPeriod_CacheAttached(PXCache sender)
		{ 
		}

		protected virtual void FALocationHistory_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			AssetMaint.LiveUpdateMaskedSubs(this, Assets.Cache, (FALocationHistory)e.Row);
		}

		public static void Transfer(TransferFilter filter, List<FixedAsset> list)
		{
			TransferProcess graph = PXGraph.CreateInstance<TransferProcess>();
			graph.DoTransfer(filter, list);
		}

		protected virtual void DoTransfer(TransferFilter filter, List<FixedAsset> list)
		{
			DocumentList<FARegister> created = new DocumentList<FARegister>(this);
			foreach (FixedAsset asset in list)
			{
				FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectSingleBound(this, new object[]{asset});
				FALocationHistory location = PXSelect<FALocationHistory, Where<FALocationHistory.assetID, Equal<Current<FADetails.assetID>>, And<FALocationHistory.revisionID, Equal<Current<FADetails.locationRevID>>>>>.SelectSingleBound(this, new object[] { det });
				int? destClassID = filter.ClassTo ?? asset.ClassID;
				int? destBranchID = filter.BranchTo ?? location.LocationID;
				string destDeptID = string.IsNullOrEmpty(filter.DepartmentTo) ? location.Department : filter.DepartmentTo;

				if (location.LocationID != destBranchID || location.Department != destDeptID || asset.ClassID != destClassID)
				{
					FADetails copy_det = (FADetails)Details.Cache.CreateCopy(det);
					FALocationHistory copy_loc = (FALocationHistory) Lochist.Cache.CreateCopy(location);
					copy_loc.RevisionID = ++copy_det.LocationRevID;
					copy_loc.LocationID = destBranchID;
					copy_loc.Department = destDeptID;
					copy_loc.PeriodID = filter.PeriodID;
					copy_loc.TransactionDate = filter.TransferDate;
					copy_loc.Reason = filter.Reason;

					TransactionEntry.SegregateRegister(this, (int)destBranchID, FARegister.origin.Transfer, null, filter.TransferDate, "", created);

					Details.Update(copy_det);
					location = Lochist.Insert(copy_loc);

					if (asset.ClassID != destClassID)
					{
						asset.ClassID = destClassID;
						AssetSelect.Cache.Update(asset);
					}

					FARegister reg = Register.Current;
					AssetProcess.TransferAsset(this, asset, location, ref reg);
				}
			}
			if (Register.Current != null && created.Find(Register.Current) == null)
			{
				created.Add(Register.Current);
			}
			Actions.PressSave();
			if (fasetup.Current.AutoReleaseTransfer == true)
			{
				SelectTimeStamp();
				PXLongOperation.StartOperation(this, delegate{ AssetTranRelease.ReleaseDoc(created, false); });
			}
			else if (created.Count > 0)
			{
				AssetTranRelease graph = CreateInstance<AssetTranRelease>();
				AssetTranRelease.ReleaseFilter fltr = (AssetTranRelease.ReleaseFilter)graph.Filter.Cache.CreateCopy(graph.Filter.Current);
				fltr.Origin = FARegister.origin.Transfer;
				graph.Filter.Update(fltr);
				graph.SelectTimeStamp();

				Dictionary<string, string> parameters = new Dictionary<string, string>();

				for (int i = 0; i < created.Count; ++i)
				{
					FARegister reg = created[i];
				reg.Selected = true;
				graph.FADocumentList.Update(reg);
				graph.FADocumentList.Cache.SetStatus(reg, PXEntryStatus.Updated);
				graph.FADocumentList.Cache.IsDirty = false;

					parameters["FARegister.RefNbr" + i] = reg.RefNbr;
				}

				parameters["PeriodFrom"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.PeriodID);
				parameters["PeriodTo"] = FinPeriodIDFormattingAttribute.FormatForDisplay(filter.PeriodID);
				parameters["Mode"] = "U";

				PXReportRequiredException reportex = new PXReportRequiredException(parameters, "FA642000", "Preview");
				throw new PXRedirectWithReportException(graph, reportex, "Release FA Transaction");
			}
		}

		public virtual IEnumerable assets()
		{
			TransferFilter filter = Filter.Current;
			PXSelectBase<FixedAsset> cmd = new PXSelectJoin<FixedAsset, 
				InnerJoin<FADetails, On<FixedAsset.assetID, Equal<FADetails.assetID>>, 
				LeftJoin<Account, On<FixedAsset.fAAccountID, Equal<Account.accountID>>, 
				LeftJoin<FALocationHistory, On<FALocationHistory.assetID, Equal<FADetails.assetID>, And<FALocationHistory.revisionID, Equal<FADetails.locationRevID>>>>>>, 
				Where<FADetails.status, NotEqual<FixedAssetStatus.hold>, And<FADetails.status, NotEqual<FixedAssetStatus.disposed>>>>(this);
			if(filter.PeriodID != null)
			{
				cmd.WhereAnd<Where<FADetails.transferPeriod, IsNull, Or<FADetails.transferPeriod, LessEqual<Current<TransferFilter.periodID>>>>>();
			}
			if(filter.BranchFrom != null)
			{
				cmd.WhereAnd<Where<FixedAsset.branchID, Equal<Current<TransferFilter.branchFrom>>>>();
			}
			if(filter.DepartmentFrom != null)
			{
				cmd.WhereAnd<Where<FALocationHistory.department, Equal<Current<TransferFilter.departmentFrom>>>>();
			}
			if (filter.ClassFrom != null)
			{
				cmd.WhereAnd<Where<FixedAsset.classID, Equal<Current<TransferFilter.classFrom>>>>();
			}

			int startRow = PXView.StartRow;
			int totalRows = 0;
			List<PXFilterRow> newFilters = new List<PXFilterRow>();
			foreach (PXFilterRow f in PXView.Filters)
			{
				if (f.DataField.ToLower() == "status")
				{
					f.DataField = "FADetails__Status";
				}
				newFilters.Add(f);
			}
			List<object> list = cmd.View.Select(PXView.Currents, null, PXView.Searches, PXView.SortColumns, PXView.Descendings, newFilters.ToArray(), ref startRow, PXView.MaximumRows, ref totalRows);
			PXView.StartRow = 0;
			return list;
		}

		protected virtual void FixedAsset_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FixedAsset asset = (FixedAsset)e.Row;

			// AssetID can be <0 when the datasource inserts a temporary record on redirect from selector
			if (asset == null || asset.AssetID < 0) return;

			FADetails det = PXSelect<FADetails, Where<FADetails.assetID, Equal<Current<FixedAsset.assetID>>>>.SelectSingleBound(this, new object[] { asset });
			try
			{
				AssetProcess.ThrowDisabled_Transfer(this, asset, det);
			}
			catch (PXException exc)
			{
				PXUIFieldAttribute.SetEnabled<FixedAsset.selected>(sender, asset, false);
				sender.RaiseExceptionHandling<FixedAsset.selected>(asset, null, new PXSetPropertyException(exc.MessageNoNumber, PXErrorLevel.RowWarning));
			}
			
			if(string.IsNullOrEmpty(det.TransferPeriod))
			{
				PXUIFieldAttribute.SetEnabled<FixedAsset.selected>(sender, asset, false);
				sender.RaiseExceptionHandling<FADetails.transferPeriod>(asset, null, new PXSetPropertyException(Messages.NextPeriodNotGenerated));
			}
		}


		public void TransferFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			TransferFilter filter = (TransferFilter)e.Row;
			if (filter == null) return;

			Assets.SetProcessDelegate(delegate(List<FixedAsset> list) { Transfer(filter, list); });

			Assets.SetProcessEnabled(string.IsNullOrEmpty(filter.PeriodID) == false && (filter.BranchTo != null || filter.DepartmentTo != null));
			Assets.SetProcessAllEnabled(string.IsNullOrEmpty(filter.PeriodID) == false && (filter.BranchTo != null || filter.DepartmentTo != null));
		}

		[Serializable]
		public partial class TransferFilter : IBqlTable
		{
			#region TransferDate
			public abstract class transferDate : IBqlField
			{
			}
			protected DateTime? _TransferDate;
			[PXDBDate]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Transfer Date")]
			public virtual DateTime? TransferDate
			{
				get
				{
					return _TransferDate;
				}
				set
				{
					_TransferDate = value;
				}
			}
			#endregion
			#region PeriodID
			public abstract class periodID : IBqlField
			{
			}
			protected String _PeriodID;
			[PXUIField(DisplayName = "Transfer Period")]
			[FAOpenPeriod(typeof(TransferFilter.transferDate))]
			public virtual String PeriodID
			{
				get
				{
					return _PeriodID;
				}
				set
				{
					_PeriodID = value;
				}
			}
			#endregion
			#region ClassFrom
			public abstract class classFrom : IBqlField
			{
			}
			protected int? _ClassFrom;
			[PXDBInt()]
			[PXSelector(typeof(Search<FAClass.assetID, Where<FAClass.recordType, Equal<FARecordType.classType>>>),
						typeof(FAClass.assetCD), typeof(FAClass.assetTypeID), typeof(FAClass.description), typeof(FAClass.usefulLife),
						SubstituteKey = typeof(FAClass.assetCD),
						DescriptionField = typeof(FAClass.description), CacheGlobal = true)]
			[PXUIField(DisplayName = "Asset Class")]
			public virtual int? ClassFrom
			{
				get
				{
					return _ClassFrom;
				}
				set
				{
					_ClassFrom = value;
				}
			}
			#endregion
			#region BranchFrom
			public abstract class branchFrom : IBqlField
			{
			}
			protected int? _BranchFrom;
			[Branch(null, IsDetail = false)]
			[PXDefault(typeof(Search<Branch.branchID, Where<Branch.branchID, Equal<Current<AccessInfo.branchID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
			public virtual int? BranchFrom
			{
				get
				{
					return _BranchFrom;
				}
				set
				{
					_BranchFrom = value;
				}
			}
			#endregion
			#region DepartmentFrom
			public abstract class departmentFrom : IBqlField
			{
			}
			protected string _DepartmentFrom;
			[PXDBString(10, IsUnicode = true)]
			[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
			[PXUIField(DisplayName = "Department")]
			public virtual string DepartmentFrom
			{
				get
				{
					return _DepartmentFrom;
				}
				set
				{
					_DepartmentFrom = value;
				}
			}
			#endregion
			#region ClassTo
			public abstract class classTo : IBqlField
			{
			}
			protected int? _ClassTo;
			[PXDBInt()]
			[PXSelector(typeof(Search<FAClass.assetID, Where<FAClass.recordType, Equal<FARecordType.classType>>>),
						typeof(FAClass.assetCD), typeof(FAClass.assetTypeID), typeof(FAClass.description), typeof(FAClass.usefulLife),
						SubstituteKey = typeof(FAClass.assetCD),
						DescriptionField = typeof(FAClass.description), CacheGlobal = true)]
			[PXUIField(DisplayName = "Asset Class")]
			public virtual int? ClassTo
			{
				get
				{
					return _ClassTo;
				}
				set
				{
					_ClassTo = value;
				}
			}
			#endregion
			#region BranchTo
			public abstract class branchTo : IBqlField
			{
			}
			protected int? _BranchTo;
			[Branch(null, IsDetail = false)]
			public virtual int? BranchTo
			{
				get
				{
					return _BranchTo;
				}
				set
				{
					_BranchTo = value;
				}
			}
			#endregion
			#region DepartmentTo
			public abstract class departmentTo : IBqlField
			{
			}
			protected string _DepartmentTo;
			[PXDBString(10, IsUnicode = true)]
			[PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
			[PXUIField(DisplayName = "Department")]
			public virtual string DepartmentTo
			{
				get
				{
					return _DepartmentTo;
				}
				set
				{
					_DepartmentTo = value;
				}
			}
			#endregion
			#region Reason
			public abstract class reason : PX.Data.IBqlField
			{
			}
			protected String _Reason;
			[PXDBString(30, IsUnicode = true)]
			[PXUIField(DisplayName = "Reason")]
			public virtual String Reason
			{
				get
				{
					return this._Reason;
				}
				set
				{
					this._Reason = value;
				}
			}
			#endregion
		}
	}
}
