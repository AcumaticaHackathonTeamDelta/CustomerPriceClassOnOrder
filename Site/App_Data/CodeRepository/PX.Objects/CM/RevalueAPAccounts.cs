using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AP;
using PX.Objects.CS;
using PX.Objects.AP.Overrides.APDocumentRelease;

namespace PX.Objects.CM
{
	[TableAndChartDashboardType]
	public class RevalueAPAccounts : PXGraph<RevalueAPAccounts>
	{
		public PXCancel<RevalueFilter> Cancel;
		public PXFilter<RevalueFilter> Filter;
		[PXFilterable]
		public PXFilteredProcessing<RevaluedAPHistory, RevalueFilter, Where<boolTrue, Equal<boolTrue>>, OrderBy<Asc<RevaluedAPHistory.accountID, Asc<RevaluedAPHistory.subID, Asc<RevaluedAPHistory.vendorID>>>>> APAccountList;
		public PXSelect<CurrencyInfo> currencyinfo;
		public PXSetup<APSetup> apsetup;
		public PXSetup<CMSetup> cmsetup;
		public PXSetup<Company> company;

		public RevalueAPAccounts()
		{
			var curCMsetup = cmsetup.Current;
			var curAPsetup = apsetup.Current;
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>() != true) 
				throw new Exception(Messages.MultiCurrencyNotActivated);
			APAccountList.SetProcessCaption(Messages.Revalue);
			APAccountList.SetProcessAllVisible(false);

			PXUIFieldAttribute.SetEnabled<RevaluedAPHistory.finPtdRevalued>(APAccountList.Cache, null, true);
		}

		protected virtual void RevalueFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			RevalueFilter filter = (RevalueFilter)e.Row;
			if (filter != null)
			{
				APAccountList.SetProcessDelegate(
					delegate(List<RevaluedAPHistory> list)
					{
						Revalue(filter, list);
					}
				);
			}
		}

		protected virtual void RevalueFilter_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (((RevalueFilter)e.Row).CuryEffDate != null)
			{
				((RevalueFilter)e.Row).CuryEffDate = ((DateTime)((RevalueFilter)e.Row).CuryEffDate).AddDays(-1);
			}
		}

		protected virtual void RevalueFilter_FinPeriodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<RevalueFilter.curyEffDate>(e.Row);

			if (((RevalueFilter)e.Row).CuryEffDate != null)
			{
				((RevalueFilter)e.Row).CuryEffDate = ((DateTime)((RevalueFilter)e.Row).CuryEffDate).AddDays(-1);
			}

			APAccountList.Cache.Clear();
		}

		protected virtual void RevalueFilter_CuryEffDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APAccountList.Cache.Clear();
		}

		protected virtual void RevalueFilter_CuryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			APAccountList.Cache.Clear();
		}

		protected virtual void RevalueFilter_TotalRevalued_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (e.Row != null)
			{
				decimal val = 0m;
				foreach (RevaluedAPHistory res in APAccountList.Cache.Updated)
				{
					if ((bool)res.Selected)
					{
						val += (decimal)res.FinPtdRevalued;
					}
				}

				if (val == 0)
				{
					TimeSpan timespan;
					Exception ex;
					if (PXLongOperation.GetStatus(UID, out timespan, out ex) == PXLongRunStatus.Completed)
					{
						Filter.Cache.RaiseExceptionHandling<RevalueFilter.totalRevalued>(e.Row, val,
							new PXSetPropertyException(Messages.NoRevaluationEntryWasMade, PXErrorLevel.Warning));
					}
				}
				else
				{
					e.ReturnValue = val;
					e.Cancel = true;
				}
			}
		}

		public virtual IEnumerable apaccountlist()
		{
			foreach (PXResult<APHistoryByPeriod, RevaluedAPHistory, Vendor, APHistoryLastRevaluation> res in 
								PXSelectJoin<APHistoryByPeriod, 
									InnerJoin<RevaluedAPHistory, 
										On<RevaluedAPHistory.vendorID, Equal<APHistoryByPeriod.vendorID>, 
											And<RevaluedAPHistory.branchID,Equal<APHistoryByPeriod.branchID>, 
											And<RevaluedAPHistory.accountID,Equal<APHistoryByPeriod.accountID>, 
											And<RevaluedAPHistory.subID,Equal<APHistoryByPeriod.subID>, 
											And<RevaluedAPHistory.curyID, Equal<APHistoryByPeriod.curyID>, 
											And<RevaluedAPHistory.finPeriodID,Equal<APHistoryByPeriod.lastActivityPeriod>>>>>>>,
									InnerJoin<Vendor, 
										On<Vendor.bAccountID, Equal<APHistoryByPeriod.vendorID>>,
									LeftJoin<APHistoryLastRevaluation,
										On<APHistoryByPeriod.vendorID, Equal<APHistoryLastRevaluation.vendorID>,
											And<APHistoryByPeriod.branchID, Equal<APHistoryLastRevaluation.branchID>,
											And<APHistoryByPeriod.accountID, Equal<APHistoryLastRevaluation.accountID>,
											And<APHistoryByPeriod.subID, Equal<APHistoryLastRevaluation.subID>,
											And<APHistoryByPeriod.curyID, Equal<APHistoryLastRevaluation.curyID>>>>>>>>>,
									Where<APHistoryByPeriod.curyID, Equal<Current<RevalueFilter.curyID>>, 
										And<APHistoryByPeriod.finPeriodID, Equal<Current<RevalueFilter.finPeriodID>>,
										And<Where<RevaluedAPHistory.curyFinYtdBalance, NotEqual<decimal0>, 
													Or<RevaluedAPHistory.finPtdRevalued, NotEqual<decimal0>,
													Or<RevaluedAPHistory.curyFinYtdDeposits, NotEqual<decimal0>>>>>>>>
									.Select(this))
			{
				APHistoryByPeriod histbyper = res;
				APHistoryLastRevaluation lastRevaluationPeriod = res;
				RevaluedAPHistory hist = PXCache<RevaluedAPHistory>.CreateCopy(res);
				RevaluedAPHistory existing;

				if ((existing = APAccountList.Locate(hist)) != null)
				{
					yield return existing;
					continue;
				}
				else
				{
					APAccountList.Cache.SetStatus(hist, PXEntryStatus.Held);
				}

				hist.CuryRateTypeID = cmsetup.Current.APRateTypeReval ?? ((Vendor)res).CuryRateTypeID;

				if (string.IsNullOrEmpty(hist.CuryRateTypeID))
				{
					APAccountList.Cache.RaiseExceptionHandling<RevaluedGLHistory.curyRateTypeID>(hist, null, new PXSetPropertyException(Messages.RateTypeNotFound));
				}
				else
				{
					CurrencyRate curyrate = PXSelect<CurrencyRate,
						Where<CurrencyRate.fromCuryID, Equal<Current<RevalueFilter.curyID>>,
						And<CurrencyRate.toCuryID, Equal<Current<Company.baseCuryID>>,
						And<CurrencyRate.curyRateType, Equal<Required<Vendor.curyRateTypeID>>,
						And<CurrencyRate.curyEffDate, LessEqual<Current<RevalueFilter.curyEffDate>>>>>>,
						OrderBy<Desc<CurrencyRate.curyEffDate>>>.Select(this, hist.CuryRateTypeID);

					if (curyrate == null || curyrate.CuryMultDiv == null)
					{
						hist.CuryMultDiv = "M";
						hist.CuryRate = 1m;
						hist.RateReciprocal = 1m;
						hist.CuryEffDate = Filter.Current.CuryEffDate;
						APAccountList.Cache.RaiseExceptionHandling<RevaluedAPHistory.curyRate>(hist, 1m, new PXSetPropertyException(Messages.RateNotFound, PXErrorLevel.RowWarning));
					}
					else
					{
						hist.CuryRate = curyrate.CuryRate;
						hist.RateReciprocal = curyrate.RateReciprocal;
						hist.CuryEffDate = curyrate.CuryEffDate;
						hist.CuryMultDiv = curyrate.CuryMultDiv;
					}

					CurrencyInfo info = new CurrencyInfo();
					info.BaseCuryID = company.Current.BaseCuryID;
					info.CuryID = hist.CuryID;
					info.CuryMultDiv = hist.CuryMultDiv;
					info.CuryRate = hist.CuryRate;
					info.RecipRate = hist.RateReciprocal;

					hist.CuryFinYtdBalance -= hist.CuryFinYtdDeposits;
					hist.FinYtdBalance -= hist.FinYtdDeposits;

					decimal baseval;
					PXCurrencyAttribute.CuryConvBase(currencyinfo.Cache, info, (decimal)hist.CuryFinYtdBalance, out baseval);
					hist.FinYtdRevalued = baseval;
					hist.FinPrevRevalued = string.Equals(histbyper.FinPeriodID, histbyper.LastActivityPeriod) ? hist.FinPtdRevalued : 0m;
					hist.FinPtdRevalued = hist.FinYtdRevalued - hist.FinPrevRevalued - hist.FinYtdBalance;
					hist.LastRevaluedFinPeriodID = lastRevaluationPeriod?.LastActivityPeriod;
				}
				yield return hist;
			}
		}

		public static void Revalue(RevalueFilter filter, List<RevaluedAPHistory> list)
		{
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			PostGraph pg = PXGraph.CreateInstance<PostGraph>();
			PXCache cache = je.Caches[typeof(CuryAPHist)];
			PXCache basecache = je.Caches[typeof(APHist)];
			je.Views.Caches.Add(typeof(CuryAPHist));
			je.Views.Caches.Add(typeof(APHist));
            
            string extRefNbrNumbering = je.CMSetup.Current.ExtRefNbrNumberingID;
            if (string.IsNullOrEmpty(extRefNbrNumbering) == false)
            {
                RevaluationRefNbrHelper helper = new RevaluationRefNbrHelper(extRefNbrNumbering);
				helper.Subscribe(je);
            }

			DocumentList<Batch> created = new DocumentList<Batch>(je);

			Currency currency = PXSelect<Currency, Where<Currency.curyID, Equal<Required<Currency.curyID>>>>.Select(je, filter.CuryID);

			using (PXTransactionScope ts = new PXTransactionScope())
			{

				foreach (RevaluedAPHistory hist in list)
				{
					if (hist.FinPtdRevalued == 0m)
					{
						continue;
					}

                    if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
					{
						je.Save.Press();

						if (created.Find(je.BatchModule.Current) == null)
						{
							created.Add(je.BatchModule.Current);
						}
					}

					Batch cmbatch = created.Find<Batch.branchID>(hist.BranchID) ?? new Batch();
					if (cmbatch.BatchNbr == null)
					{
						je.Clear();

						CurrencyInfo info = new CurrencyInfo();
						info.CuryID = hist.CuryID;
						info.CuryEffDate = hist.CuryEffDate;
						info.BaseCalc = false;
						info = je.currencyinfo.Insert(info) ?? info;

						cmbatch = new Batch();
						cmbatch.BranchID = hist.BranchID;
						cmbatch.Module = "CM";
						cmbatch.Status = "U";
						cmbatch.AutoReverse = true;
						cmbatch.Released = true;
						cmbatch.Hold = false;
						cmbatch.DateEntered = filter.CuryEffDate;
						cmbatch.FinPeriodID = filter.FinPeriodID;
						cmbatch.TranPeriodID = filter.FinPeriodID;
						cmbatch.CuryID = hist.CuryID;
						cmbatch.CuryInfoID = info.CuryInfoID;
						cmbatch.DebitTotal = 0m;
						cmbatch.CreditTotal = 0m;
						cmbatch.Description = filter.Description;
						je.BatchModule.Insert(cmbatch);

						CurrencyInfo b_info = je.currencyinfo.Select();
						if (b_info != null)
						{
							b_info.CuryID = hist.CuryID;
							b_info.CuryEffDate = hist.CuryEffDate;
							b_info.CuryRateTypeID = hist.CuryRateTypeID;
							b_info.CuryRate = hist.CuryRate;
							b_info.RecipRate = hist.RateReciprocal;
							b_info.CuryMultDiv = hist.CuryMultDiv;
							je.currencyinfo.Update(b_info);
						}
					}
					else
					{
						if (!je.BatchModule.Cache.ObjectsEqual(je.BatchModule.Current, cmbatch))
						{
							je.Clear();
						}

						je.BatchModule.Current = je.BatchModule.Search<Batch.batchNbr>(cmbatch.BatchNbr, cmbatch.Module);
					}

					{
						GLTran tran = new GLTran();
						tran.SummPost = false;
						tran.AccountID = currency.APProvAcctID ?? hist.AccountID;
						tran.SubID = currency.APProvSubID ?? hist.SubID;
						tran.CuryDebitAmt = 0m;
						tran.CuryCreditAmt = 0m;

						tran.DebitAmt = (hist.FinPtdRevalued < 0m) ? -1m * hist.FinPtdRevalued : 0m;
						tran.CreditAmt = (hist.FinPtdRevalued < 0m) ? 0m : hist.FinPtdRevalued;

						tran.TranType = "REV";
						tran.TranClass = AccountType.Liability;
						tran.RefNbr = string.Empty;
						tran.TranDesc = filter.Description;
						tran.TranPeriodID = filter.FinPeriodID;
						tran.FinPeriodID = filter.FinPeriodID;
						tran.TranDate = filter.CuryEffDate;
						tran.CuryInfoID = null;
						tran.Released = true;
						tran.ReferenceID = hist.VendorID;

						je.GLTranModuleBatNbr.Insert(tran);
					}


					foreach (GLTran tran in je.GLTranModuleBatNbr.SearchAll<Asc<GLTran.tranClass>>(new object [] { GLTran.tranClass.UnrealizedAndRevaluationGOL }))
					{
						je.GLTranModuleBatNbr.Delete(tran);
					}

                    VendorClass vendclass = PXSelectReadonly<VendorClass, Where<VendorClass.vendorClassID, Equal<Required<VendorClass.vendorClassID>>>>.Select(je, hist.VendorClassID);

                    if (vendclass == null)
                    {
                        vendclass = new VendorClass();
                    }

                    if (vendclass.UnrealizedGainAcctID == null)
                    {
                        vendclass.UnrealizedGainSubID = null;
                    }

                    if (vendclass.UnrealizedLossAcctID == null)
                    {
                        vendclass.UnrealizedLossSubID = null;
                    }

					{
						GLTran tran = new GLTran();
						tran.SummPost = true;
						tran.ZeroPost = false;
						tran.CuryDebitAmt = 0m;
						tran.CuryCreditAmt = 0m;

						if (je.BatchModule.Current.DebitTotal > je.BatchModule.Current.CreditTotal)
						{
                            tran.AccountID = vendclass.UnrealizedGainAcctID ?? currency.UnrealizedGainAcctID;
                            tran.SubID = vendclass.UnrealizedGainSubID ?? GainLossSubAccountMaskAttribute.GetSubID<Currency.unrealizedGainSubID>(je, hist.BranchID, currency);
							tran.DebitAmt = 0m;
							tran.CreditAmt = (je.BatchModule.Current.DebitTotal - je.BatchModule.Current.CreditTotal);
						}
						else
						{
                            tran.AccountID = vendclass.UnrealizedLossAcctID ?? currency.UnrealizedLossAcctID;
                            tran.SubID = vendclass.UnrealizedLossSubID ?? GainLossSubAccountMaskAttribute.GetSubID<Currency.unrealizedLossSubID>(je, hist.BranchID, currency);
							tran.DebitAmt = (je.BatchModule.Current.CreditTotal - je.BatchModule.Current.DebitTotal);
							tran.CreditAmt = 0m;
						}

						tran.TranType = "REV";
						tran.TranClass = GLTran.tranClass.UnrealizedAndRevaluationGOL;
						tran.RefNbr = string.Empty;
						tran.TranDesc = filter.Description;
						tran.Released = true;
						tran.ReferenceID = null;

						je.GLTranModuleBatNbr.Insert(tran);
					}

					{
						CuryAPHist aphist = new CuryAPHist();
						aphist.BranchID = hist.BranchID;
						aphist.AccountID = hist.AccountID;
						aphist.SubID = hist.SubID;
						aphist.FinPeriodID = filter.FinPeriodID;
						aphist.VendorID = hist.VendorID;
						aphist.CuryID = hist.CuryID;

						aphist = (CuryAPHist) cache.Insert(aphist);
						aphist.FinPtdRevalued += hist.FinPtdRevalued;
					}

					{
						APHist aphist = new APHist();
						aphist.BranchID = hist.BranchID;
						aphist.AccountID = hist.AccountID;
						aphist.SubID = hist.SubID;
						aphist.FinPeriodID = filter.FinPeriodID;
						aphist.VendorID = hist.VendorID;

						aphist = (APHist)basecache.Insert(aphist);
						aphist.FinPtdRevalued += hist.FinPtdRevalued;
					}
				}

				if (je.GLTranModuleBatNbr.Cache.IsInsertedUpdatedDeleted)
				{
					je.Save.Press();

					if (created.Find(je.BatchModule.Current) == null)
					{
						created.Add(je.BatchModule.Current);
					}
				}

				ts.Complete();
			}

			CMSetup cmsetup = PXSelect<CMSetup>.Select(je);

			for (int i = 0; i < created.Count; i++)
			{
				if (cmsetup.AutoPostOption == true)
				{
					pg.Clear();
					pg.PostBatchProc(created[i]);
				}
			}

            if (created.Count > 0)
            {
                je.BatchModule.Current = created[created.Count - 1];
                throw new PXRedirectRequiredException(je, "Preview");
            }
		}
	}

    [Serializable]
	[PXBreakInheritance()]
	public partial class RevaluedAPHistory : CuryAPHistory
	{
		#region Selected
		public abstract class selected : PX.Data.IBqlField
		{
		}
		protected bool? _Selected = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Selected")]
		public virtual bool? Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
			}
		}
		#endregion
		#region CuryRateTypeID
		public abstract class curyRateTypeID : PX.Data.IBqlField
		{
		}
		protected String _CuryRateTypeID;
		[PXString(6, IsUnicode = true)]
		[PXUIField(DisplayName = "Currency Rate Type")]
		[PXSelector(typeof(CurrencyRateType.curyRateTypeID))]
		public virtual String CuryRateTypeID
		{
			get
			{
				return this._CuryRateTypeID;
			}
			set
			{
				this._CuryRateTypeID = value;
			}
		}
		#endregion
		#region CuryRate
		public abstract class curyRate : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryRate = 1m;
		[PXDecimal(6)]
		[PXUIField(DisplayName = "Currency Rate")]
		public virtual Decimal? CuryRate
		{
			get
			{
				return this._CuryRate;
			}
			set
			{
				this._CuryRate = value;
			}
		}
		#endregion
		#region RateReciprocal
		public abstract class rateReciprocal : PX.Data.IBqlField
		{
		}
		protected Decimal? _RateReciprocal;

		[PXDecimal(8)]
		public virtual Decimal? RateReciprocal
		{
			get
			{
				return this._RateReciprocal;
			}
			set
			{
				this._RateReciprocal = value;
			}
		}
		#endregion
		#region CuryEffDate
		public abstract class curyEffDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _CuryEffDate;

		[PXDate()]
		public virtual DateTime? CuryEffDate
		{
			get
			{
				return this._CuryEffDate;
			}
			set
			{
				this._CuryEffDate = value;
			}
		}
		#endregion
		#region CuryMultDiv
		public abstract class curyMultDiv : PX.Data.IBqlField
		{
		}
		protected String _CuryMultDiv = "M";
		[PXString(1, IsFixed = true)]
		public virtual String CuryMultDiv
		{
			get
			{
				return this._CuryMultDiv;
			}
			set
			{
				this._CuryMultDiv = value;
			}
		}
		#endregion
		#region AccountID
		public new abstract class accountID : PX.Data.IBqlField
		{
		}
		[Account(IsKey = true, DescriptionField=typeof(Account.description))]
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
		public new abstract class subID : PX.Data.IBqlField
		{
		}
		[SubAccount(IsKey = true, DescriptionField=typeof(Sub.description))]
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
		#region FinPeriodID
		public new abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		#endregion
		#region BranchID
		public new abstract class branchID : PX.Data.IBqlField
		{
		}
		[Branch(IsKey = true, IsDetail = true)]
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
		#region VendorID
		public new abstract class vendorID : PX.Data.IBqlField
		{
		}
		[Vendor(IsKey = true, DescriptionField=typeof(Vendor.acctName))]
		public override Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
        #region VendorClassID
        public abstract class vendorClassID : PX.Data.IBqlField
        {
        }
        protected String _VendorClassID;
        [PXString(10, IsUnicode = true)]
        [PXSelector(typeof(VendorClass.vendorClassID), DescriptionField = typeof(VendorClass.descr), CacheGlobal = true)]
        [PXUIField(DisplayName = "Vendor Class", Enabled = false)]
        public virtual String VendorClassID
        {
            get
            {
                return this._VendorClassID;
            }
            set
            {
                this._VendorClassID = value;
            }
        }
        #endregion
        #region CuryID
		public new abstract class curyID : PX.Data.IBqlField
		{
		}
		#endregion
		#region CuryFinYtdBalance
		public new abstract class curyFinYtdBalance : PX.Data.IBqlField
		{
		}
		[PXDBCury(typeof(RevaluedAPHistory.curyID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Foreign Currency Balance", Enabled = false)]
		public override Decimal? CuryFinYtdBalance
		{
			get
			{
				return this._CuryFinYtdBalance;
			}
			set
			{
				this._CuryFinYtdBalance = value;
			}
		}
		#endregion
		#region FinYtdBalance
		public new abstract class finYtdBalance : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName="Original Balance", Enabled=false)]
		public override Decimal? FinYtdBalance
		{
			get
			{
				return this._FinYtdBalance;
			}
			set
			{
				this._FinYtdBalance = value;
			}
		}
		#endregion
		#region CuryFinYtdDeposits
		public new abstract class curyFinYtdDeposits : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinYtdDeposits
		public new abstract class finYtdDeposits : PX.Data.IBqlField
		{
		}
		#endregion
		#region FinPrevRevalued
		public abstract class finPrevRevalued : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinPrevRevalued;
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "PTD gain or loss", Enabled = false)]
		public virtual Decimal? FinPrevRevalued
		{
			get
			{
				return this._FinPrevRevalued;
			}
			set
			{
				this._FinPrevRevalued = value;
			}
		}
		#endregion
		#region FinYtdRevalued
		public abstract class finYtdRevalued : PX.Data.IBqlField
		{
		}
		protected Decimal? _FinYtdRevalued;
		[PXBaseCury()]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Revalued Balance", Enabled = false)]
		[PXFormula(typeof(Add<Add<RevaluedAPHistory.finYtdBalance, RevaluedAPHistory.finPrevRevalued>, RevaluedAPHistory.finPtdRevalued>))]
		public virtual Decimal? FinYtdRevalued
		{
			get
			{
				return this._FinYtdRevalued;
			}
			set
			{
				this._FinYtdRevalued = value;
			}
		}
		#endregion
		#region FinPtdRevalued
		public new abstract class finPtdRevalued : PX.Data.IBqlField
		{
		}
		[PXDBBaseCury]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName="Difference", Enabled=true)]
		public override Decimal? FinPtdRevalued
		{
			get
			{
				return this._FinPtdRevalued;
			}
			set
			{
				this._FinPtdRevalued = value;
			}
		}
		#endregion
		#region LastRevaluedFinPeriodID
		public abstract class lastRevaluedFinPeriodID : PX.Data.IBqlField
		{
		}

		[PXUIField(DisplayName = "Last Revaluation Period")]
		[FinPeriodID(IsDBField = false)]
		public virtual String LastRevaluedFinPeriodID { get; set; }
		#endregion
	}

	[System.SerializableAttribute()]
	public partial class RevalueFilter : PX.Data.IBqlTable
	{
		#region BusinessDate
		public abstract class businessDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _BusinessDate;
		[PXDBDate()]
		[PXDefault(typeof(AccessInfo.businessDate))]
		public virtual DateTime? BusinessDate
		{
			get
			{
				return this._BusinessDate;
			}
			set
			{
				this._BusinessDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[ClosedPeriod(typeof(RevalueFilter.businessDate))]
		[PXUIField(DisplayName = "Fin. Period", Required = true)]
		public virtual String FinPeriodID
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
		#region CuryEffDate
		public abstract class curyEffDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _CuryEffDate;
		[PXDBDate()]
		[PXDefault(typeof(Search<FinPeriod.endDate,Where<FinPeriod.finPeriodID,Equal<Current<RevalueFilter.finPeriodID>>>>))]
		[PXUIField(DisplayName = "Currency Effective Date", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? CuryEffDate
		{
			get
			{
				return this._CuryEffDate;
			}
			set
			{
				this._CuryEffDate = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask=">LLLLL")]
		[PXDefault()]
		[PXUIField(DisplayName = "Currency")]
		[PXSelector(typeof(Search2<Currency.curyID, InnerJoin<Company, On<Currency.curyID, NotEqual<Company.baseCuryID>>>>))]
		public virtual String CuryID
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
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(60, IsUnicode=true)]
		[PXDefault()]
		[PXUIField(DisplayName = "Description")]
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
		#region TotalRevalued
		public abstract class totalRevalued : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalRevalued;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBBaseCury()]
		[PXUIField(DisplayName = "Revaluation Total", Enabled = false)]
		public virtual Decimal? TotalRevalued
		{
			get
			{
				return this._TotalRevalued;
			}
			set
			{
				this._TotalRevalued = value;
			}
		}
		#endregion
	}

	[Serializable]
	[PXProjection(typeof(Select4<CuryAPHistory,
		Where<CuryAPHistory.finPtdRevalued, NotEqual<decimal0>>,
		Aggregate<
			GroupBy<CuryAPHistory.branchID,
			GroupBy<CuryAPHistory.vendorID,
			GroupBy<CuryAPHistory.accountID,
			GroupBy<CuryAPHistory.subID,
			GroupBy<CuryAPHistory.curyID,
			Max<CuryAPHistory.finPeriodID>>>>>>>>))]
	public partial class APHistoryLastRevaluation : PX.Data.IBqlTable
	{
		#region BranchID
		public abstract class branchID : PX.Data.IBqlField
		{
		}
		protected Int32? _BranchID;
		[Branch(IsKey = true, BqlField = typeof(CuryAPHistory.branchID))]
		public virtual Int32? BranchID
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
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;
		[Vendor(IsKey = true, BqlField = typeof(CuryAPHistory.vendorID))]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region AccountID
		public abstract class accountID : PX.Data.IBqlField
		{
		}
		protected Int32? _AccountID;
		[Account(IsKey = true, BqlField = typeof(CuryAPHistory.accountID))]
		public virtual Int32? AccountID
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
		public abstract class subID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubID;
		[SubAccount(IsKey = true, BqlField = typeof(CuryAPHistory.subID))]
		public virtual Int32? SubID
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
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, IsKey = true, InputMask = ">LLLLL", BqlField = typeof(CuryAPHistory.curyID))]
		public virtual String CuryID
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
		#region LastActivityPeriod
		public abstract class lastActivityPeriod : PX.Data.IBqlField
		{
		}
		protected String _LastActivityPeriod;
		[GL.FinPeriodID(BqlField = typeof(CuryAPHistory.finPeriodID))]
		public virtual String LastActivityPeriod
		{
			get
			{
				return this._LastActivityPeriod;
			}
			set
			{
				this._LastActivityPeriod = value;
			}
		}
		#endregion
	}
}
