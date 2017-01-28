using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.FA;
using PX.Objects.IN;
using PX.Objects.CA;
using PX.Objects.GL.Overrides.CloseGraph;
using PX.Objects.PR;

namespace PX.Objects.GL
{
	[PX.Objects.GL.TableAndChartDashboardType]
	public class Closing : PXGraph<Closing>
	{
		#region Selects
		[PXFilterable]
		public PXSelect<FinPeriod> PeriodList;
		public PXCancel<FinPeriod> Cancel;
		public PXAction<FinPeriod> Close;
		public PXAction<FinPeriod> ShowDocuments;
		public PXSelectReadonly<FinPeriod, Where<FinPeriod.finYear, Greater<Optional<FinPeriod.finYear>>>, OrderBy<Asc<FinPeriod.finPeriodID>>> NextFiscalYear;
		public PXSetup<GLSetup> GLSetup;
		public PXSelect<APSetup> APSetup;
		public PXSelect<ARSetup> ARSetup;
		public PXSelect<INSetup> INSetup;
		public PXSelect<CASetup> CASetup;
        public PXSelect<FASetup> FASetup;
		public PXSelect<PRSetup> PRSetup;
        public string ReportID;
		#endregion

		#region Constructor

		public Closing()
		{
			GLSetup setup = GLSetup.Current;
			APSetup apSetup = APSetup.Select();
			ARSetup arSetup = ARSetup.Select();
			INSetup inSetup = INSetup.Select();
			CASetup caSetup = CASetup.Select();
            FASetup faSetup = FASetup.Select();
			PRSetup prSetup = PRSetup.Select();
            PXCache periodCache = Caches[typeof(FinPeriod)];

			PXUIFieldAttribute.SetEnabled<FinPeriod.finPeriodID>(periodCache, null, false);
			PXUIFieldAttribute.SetEnabled<FinPeriod.descr>(periodCache, null, false);

			PXUIFieldAttribute.SetVisible<FinPeriod.aPClosed>(periodCache, null, apSetup != null);
			PXUIFieldAttribute.SetVisible<FinPeriod.aRClosed>(periodCache, null, arSetup != null);
			PXUIFieldAttribute.SetVisible<FinPeriod.iNClosed>(periodCache, null, inSetup != null);
			PXUIFieldAttribute.SetVisible<FinPeriod.cAClosed>(periodCache, null, caSetup != null);
            PXUIFieldAttribute.SetVisible<FinPeriod.fAClosed>(periodCache, null, faSetup != null);
			PXUIFieldAttribute.SetVisible<FinPeriod.pRClosed>(periodCache, null, prSetup != null);
			
			Caches[typeof(FinPeriod)].AllowInsert = false;
			Caches[typeof(FinPeriod)].AllowDelete = false;

			if(this.GetType() != typeof(Closing))
			{
				ShowDocuments.SetCaption(PXMessages.LocalizeNoPrefix(GL.Messages.ShowDocumentsNonGL));
			}
		}
		#endregion

		#region Executables
		protected virtual IEnumerable periodList()
		{
			string fiscalYear = null;
			foreach (FinPeriod per in PXSelect<FinPeriod, 
												Where<FinPeriod.closed, Equal<False>>>.Select(this))
			{
				if (fiscalYear == null)
				{
					fiscalYear = per.FinYear;
				}
				if (per.FinYear == fiscalYear)
				{
					yield return per;
				}
			}

			PeriodList.Cache.IsDirty = false;
		}
		#endregion

		#region Button Close
		[PXUIField(DisplayName = Messages.ClosePeriods, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable close(PXAdapter adapter)
		{
			APSetup apSetup = APSetup.Select();
			ARSetup arSetup = ARSetup.Select();
			INSetup inSetup = INSetup.Select();
			CASetup caSetup = CASetup.Select();
            FASetup faSetup = FASetup.Select();

			List<FinPeriod> list = new List<FinPeriod>();

			bool YearClosed = true;

			foreach (FinPeriod period in adapter.Get())
			{
				if (period.Selected == true)
				{
					
					if(period.Active != true) 
						throw new FiscalPeriodInactiveException(period.FinPeriodID);	

					if (Accessinfo.ScreenID == "GL.50.30.00")
					{
						if (apSetup != null && period.APClosed != true)
							throw new PXException(Messages.PeriodOpenInAPModule);	
						if (arSetup != null && period.ARClosed != true)
							throw new PXException(Messages.PeriodOpenInARModule);
						if (caSetup != null && period.CAClosed != true)
							throw new PXException(Messages.PeriodOpenInCAModule);
						if (inSetup != null && PXAccess.FeatureInstalled<CS.FeaturesSet.distributionModule>() && period.INClosed != true)
							throw new PXException(Messages.PeriodOpenInINModule);
                        if (faSetup != null && PXAccess.FeatureInstalled<CS.FeaturesSet.fixedAsset>() && period.FAClosed != true)
                            throw new PXException(Messages.PeriodOpenInFAModule);
                    }

					list.Add(period);
				}
				else
				{
					YearClosed = false;
				}
			}

			if (YearClosed)
			{
				if ((FinPeriod) NextFiscalYear.Select(list[list.Count-1].FinYear) == null)
				{
					throw new PXException(Messages.NoPeriodsForNextYear);
				}
			}

			if (list.Count > 0)
			{
				StartClosePeriod(list);
			}
			return adapter.Get();
		}
		#endregion
		#region Buttons Show Documents
		[PXUIField(DisplayName = Messages.ShowDocuments, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        //[PXLookupButton]
        [PXButton]
		public virtual IEnumerable showDocuments(PXAdapter adapter)
		{
			List<FinPeriod> list = new List<FinPeriod>();

			bool YearClosed = true;

			foreach (FinPeriod period in adapter.Get())
			{
				if (period.Selected == true)
				{
					list.Add(period);
				}
				else
				{
					YearClosed = false;
				}
			}

			if (YearClosed)
			{
				if ((FinPeriod) NextFiscalYear.Select(list[list.Count-1].FinYear) == null)
				{
					throw new PXException(Messages.NoPeriodsForNextYear);
				}
			}

			if (list.Count > 0)
			{
				ShowOpenDocuments(this, list); 
			}
			return adapter.Get();
		}
		#endregion

		#region Functions
		protected virtual void StartClosePeriod(List<FinPeriod> list)
		{
			PXLongOperation.StartOperation(this, delegate() { ClosePeriod(list); });
		}

		public static void ClosePeriod(List<FinPeriod> list)
		{
			CloseGraph pg = PXGraph.CreateInstance<CloseGraph>();

			foreach (FinPeriod per in list)
			{
				pg.ClosePeriodProc(per);
			}
		}

		public static void ShowOpenDocuments(Closing pg, List<FinPeriod> list)
		{
			if (list.Count == 0)
			{
				return;
			}

            List<string> periods = list.Select(fp => fp.FinPeriodID).ToList();

            string fromPeriod = periods.Min();
            string toPeriod = periods.Max();

            Dictionary<string, string> d = new Dictionary<string, string>();
            d["FromPeriodID"] = fromPeriod.Substring(4, 2) + fromPeriod.Substring(0, 4);
            d["ToPeriodID"] = toPeriod.Substring(4, 2) + toPeriod.Substring(0, 4);

			pg.RunShowReport(d);
		}

		protected virtual void RunShowReport(Dictionary<string, string> d )
		{
			string ReportID = "GL656000"; 
			if (d.Count > 0)
			{
				throw new PXReportRequiredException(d, ReportID, PXBaseRedirectException.WindowMode.New, "Open GL Documents");
			}
		}
		#endregion

		#region Events
		protected virtual void FinPeriod_Selected_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			//
			bool IsRefreshNeeded = false;
			foreach (FinPeriod per in PeriodList.Select())
			{
				if (((FinPeriod)e.Row).Selected == true)
				{
					if (String.Compare(per.PeriodNbr, ((FinPeriod)e.Row).PeriodNbr) < 0)
					{
						per.Selected = true;
						cache.Update(per);
						IsRefreshNeeded = true;
					}
				}
				else
				{
					if (String.Compare(per.PeriodNbr, ((FinPeriod)e.Row).PeriodNbr) > 0)
					{
						per.Selected = false;
						cache.Update(per);
						IsRefreshNeeded = true;
					}
				}
			}

			if (IsRefreshNeeded)
			{
				PeriodList.View.RequestRefresh();
			}
		}

		protected virtual void FinPeriod_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			FinPeriod per = (FinPeriod) e.Row;
			if (per == null) return;

			PXUIFieldAttribute.SetEnabled<FinPeriod.active>(sender, per, false);

			FinPeriod p = PeriodList.Select();
			ShowDocuments.SetEnabled(p.Selected == true);
		}
		#endregion

		protected virtual void ClosePeriodProc(FinPeriod p)
		{
			
		}
	}

	public class CloseGraph : PXGraph<CloseGraph>
	{
		public Batch Unposted
		{
			get
			{
				return Batch_Unposted.Select();
			}
		}
		public Batch Unreleased
		{
			get
			{
				return Batch_Unreleased.Select();
			}
		}
		public FinPeriod Unclosed
		{
			get
			{
				return FiscalYear_Unclosed.Select();
			}
		}

		public PXSelect<FinPeriod> FiscalPeriod_ID;
		public PXSelectReadonly<Batch, Where<Batch.finPeriodID, Equal<Current<FinPeriod.finPeriodID>>, And<Batch.posted, Equal<False>, And<Batch.released, Equal<True>>>>> Batch_Unposted;
		public PXSelectReadonly<Batch, Where<Batch.finPeriodID, Equal<Current<FinPeriod.finPeriodID>>, And<Batch.released, Equal<False>, And<Batch.scheduled, Equal<False>, And<Batch.voided, Equal<False>>>>>> Batch_Unreleased;
		public PXSelectReadonly<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinPeriod.finYear>>, And<FinPeriod.finPeriodID, NotEqual<Current<FinPeriod.finPeriodID>>, And<FinPeriod.closed, Equal<boolFalse>>>>> FiscalYear_Unclosed;
		//public PXSelect<FinPeriod, Where<FinPeriod.finYear, Equal<Current<FinPeriod.finYear>>, And<FinPeriod.closed, Equal<boolFalse>>>> FiscalYear_Unclosed;
		public PXSelectReadonly<FinPeriod, Where<FinPeriod.finYear, Greater<Optional<FinPeriod.finYear>>>,OrderBy<Asc<FinPeriod.finPeriodID>>> NextFiscalYear;

		public PXSelectGroupBy<AcctHist, 
			Where<AcctHist.finYear, Equal<Optional<FinPeriod.finYear>>, 
				And<Where<AcctHist.balanceType, Equal<LedgerBalanceType.actual>,
				Or<AcctHist.balanceType, Equal<LedgerBalanceType.report>>>>>, 
			Aggregate<GroupBy<AcctHist.accountID, 
				GroupBy<AcctHist.subID, 
				GroupBy<AcctHist.ledgerID, 
                GroupBy<AcctHist.branchID,
				Max<AcctHist.finPeriodID>>>>>>>
			AcctHist_Close;

		public PXSetup<GLSetup> glsetup;

		public void CloseYearProc(FinPeriod p)
		{
			FinPeriod pnext = (FinPeriod) NextFiscalYear.Select(p.FinYear);

			foreach (AcctHist h in AcctHist_Close.Select())
			{
				AcctHist ins = new AcctHist();
				ins.FinPeriodID = pnext.FinPeriodID;
				ins.LedgerID = h.LedgerID;
                ins.BranchID = h.BranchID;
				ins.SubID = h.SubID;
				ins.BalanceType = h.BalanceType;
				ins.CuryID = h.CuryID;
				if (Object.Equals(h.AccountID, glsetup.Current.YtdNetIncAccountID))
				{
					ins.AccountID = glsetup.Current.RetEarnAccountID;
					Caches[typeof(AcctHist)].Insert(ins);
				}
				ins.AccountID = h.AccountID;
				Caches[typeof(AcctHist)].Insert(ins);
			}
			Caches[typeof(AcctHist)].Persist(PXDBOperation.Insert);
			Caches[typeof(AcctHist)].Persisted(false);
		}

		public virtual void ClosePeriodProc(FinPeriod p)
		{
			FiscalPeriod_ID.Current = p;

            PostGraph pg = PXGraph.CreateInstance<PostGraph>();

            foreach (PXResult<Batch, BatchCopy> res in PXSelectJoin<Batch, LeftJoin<BatchCopy, On<BatchCopy.origModule, Equal<Batch.module>, And<BatchCopy.origBatchNbr, Equal<Batch.batchNbr>, And<BatchCopy.autoReverseCopy, Equal<True>>>>>, Where<Batch.finPeriodID, Equal<Required<Batch.finPeriodID>>, And<Batch.autoReverse, Equal<True>, And<Batch.released, Equal<True>, And<BatchCopy.origBatchNbr, IsNull>>>>>.Select(this, p.FinPeriodID))
            {
                pg.Clear();
                Batch copy = pg.ReverseBatchProc((Batch)res);
                pg.ReleaseBatchProc(copy);

                if (glsetup.Current.AutoPostOption == true)
                {
                    pg.PostBatchProc(copy);
                }
            }

			if (Unreleased != null || Unposted != null)
			{
				throw new PXException(Messages.PeriodHasUnpostedBatches);
			}

            using (PXTransactionScope ts = new PXTransactionScope())
            {
				p.Closed = true;
				p.DateLocked = true;
                p.APClosed = true;
                p.ARClosed = true;
                p.INClosed = true;
                p.CAClosed = true;
                p.FAClosed = true;
				Caches[typeof(FinPeriod)].Update(p);

				if (Unclosed == null)
				{
					if ((FinPeriod)NextFiscalYear.Select(p.FinYear) == null)
					{
						throw new PXException(Messages.NoPeriodsForNextYear);
					}

					CloseYearProc(p);
				}

				Caches[typeof(FinPeriod)].Persist(PXDBOperation.Update);
				Caches[typeof(FinPeriod)].Persisted(false);
				ts.Complete();
			}
		}

	}
}

namespace PX.Objects.GL.Overrides.CloseGraph
{
    //Alias
    [Serializable]
    [PXHidden]
	public partial class BatchCopy : Batch
	{
		public new abstract class origBatchNbr : PX.Data.IBqlField {}
		public new abstract class origModule : PX.Data.IBqlField {}
		public new abstract class autoReverseCopy : PX.Data.IBqlField {}
	}

	public class AHAccumAttribute : PostGraph.AHAccumAttribute
	{
		protected override bool PrepareInsert(PXCache sender, object row, PXAccumulatorCollection columns)
		{
			if (!base.PrepareInsert(sender, row, columns))
			{
				return false;
			}
			
			if (((GLHistory)row).AccountID != reacct)
			{
				columns.InsertOnly = true;
				columns.UpdateFuture<GLHistory.finBegBalance>(null);
				columns.UpdateFuture<GLHistory.tranBegBalance>(null);
				columns.UpdateFuture<GLHistory.curyFinBegBalance>(null);
				columns.UpdateFuture<GLHistory.curyTranBegBalance>(null);
				columns.UpdateFuture<GLHistory.finYtdBalance>(null);
				columns.UpdateFuture<GLHistory.tranYtdBalance>(null);
				columns.UpdateFuture<GLHistory.curyFinYtdBalance>(null);
				columns.UpdateFuture<GLHistory.curyTranYtdBalance>(null);
			}

			return true;
		}
	}
    
	[System.SerializableAttribute()]
	[AHAccum]
	public partial class AcctHist : GLHistory
	{
        #region BranchID
        [PXDBInt(IsKey = true)]
        public override Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion
		#region LedgerID
		[PXDBInt(IsKey = true)]
		public override Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion
		#region AccountID
		[PXDBInt(IsKey = true)]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		[PXDBInt(IsKey = true)]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region FinPeriod
		[PXDBString(6, IsFixed = true, IsKey = true)]
		//[PXDefault(typeof(GLTran.finPeriodID))]
		public override String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region BalanceType
		[PXDBString(1, IsFixed = true)]
		public override String BalanceType
		{
			get
			{
				return this._BalanceType;
			}
			set
			{
				this._BalanceType = value;
			}
		}
		#endregion
		#region CuryID
		[PXDBString(5, IsUnicode = true)]
		public override String CuryID
		{
			get
			{
				return this._CuryID;
			}
			set
			{
				this._CuryID = value;
			}
		}
		#endregion
        #region FinYear
        public new abstract class finYear : PX.Data.IBqlField
        {
        }
        [PXDBCalced(typeof(Substring<finPeriodID, CS.int1, CS.int4>), typeof(string))]
        public override string FinYear
        {
            get;
            set;
        }
        #endregion
	}

	[Obsolete("CuryAcctHist is not used. It will be removed in the later versions.")]
	[System.SerializableAttribute()]
	[AHAccum]
    [PXHidden]
	public partial class CuryAcctHist : CuryGLHistory
	{
        #region BranchID
        [PXDBInt(IsKey = true)]
        public override Int32? BranchID
        {
            get
            {
                return this._BranchID;
            }
            set
            {
                this._BranchID = value;
            }
        }
        #endregion
        #region LedgerID
		public new abstract class ledgerID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		public override Int32? LedgerID
		{
			get
			{
				return this._LedgerID;
			}
			set
			{
				this._LedgerID = value;
			}
		}
		#endregion
		#region AccountID
		[PXDBInt(IsKey = true)]
		public override Int32? AccountID
		{
			get
			{
				return this._AccountID;
			}
			set
			{
				this._AccountID = value;
			}
		}
		#endregion
		#region SubID
		[PXDBInt(IsKey = true)]
		public override Int32? SubID
		{
			get
			{
				return this._SubID;
			}
			set
			{
				this._SubID = value;
			}
		}
		#endregion
		#region CuryID
		[PXDBString(5, IsUnicode = true, IsKey = true)]
		public override String CuryID
		{
				get
				{
						return this._CuryID;
				}
				set
				{
						this._CuryID = value;
				}
		}
		#endregion
		#region FinPeriod
		[PXDBString(6, IsFixed = true, IsKey = true)]
		public override String FinPeriodID
		{
			get
			{
				return this._FinPeriodID;
			}
			set
			{
				this._FinPeriodID = value;
			}
		}
		#endregion
		#region BalanceType
		[PXDBString(1, IsFixed = true)]
		public override String BalanceType
		{
			get
			{
				return this._BalanceType;
			}
			set
			{
				this._BalanceType = value;
			}
		}
		#endregion
		#region BaseCuryID
		public new abstract class baseCuryID : PX.Data.IBqlField
		{
		}
		[PXDBString(5, IsUnicode = true)]
		[PXDefault(typeof(Search<Ledger.baseCuryID,Where<Ledger.ledgerID,Equal<Current<CuryAcctHist.ledgerID>>>>))]
		public override String BaseCuryID
		{
			get
			{
				return this._BaseCuryID;
			}
			set
			{
				this._BaseCuryID = value;
			}
		}
		#endregion
        #region FinYear
        public new abstract class finYear : PX.Data.IBqlField
        {
        }
        [PXDBCalced(typeof(Substring<finPeriodID, CS.int1, CS.int4>), typeof(string))]
        public override string FinYear
        {
            get;
            set;
        }
        #endregion
	}
}
