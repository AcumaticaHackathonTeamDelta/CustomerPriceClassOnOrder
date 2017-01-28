using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CA;
using PX.Objects.CS;

namespace PX.Objects.AP
{
	[TableAndChartDashboardType]
	public class APReleaseChecks : PXGraph<APReleaseChecks>
	{
		public PXFilter<ReleaseChecksFilter> Filter;
		public PXCancel<ReleaseChecksFilter> Cancel;
		public PXAction<ReleaseChecksFilter> ViewDocument;
		public ToggleCurrency<ReleaseChecksFilter> CurrencyView;
		[PXFilterable]
		public PXFilteredProcessing<APPayment, ReleaseChecksFilter, Where<boolTrue, Equal<boolTrue>>, OrderBy<Desc<APPayment.selected>>> APPaymentList;

		public PXSelect<CurrencyInfo> currencyinfo;
		public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>> CurrencyInfo_CuryInfoID;

		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXEditDetailButton]
		public virtual IEnumerable viewDocument(PXAdapter adapter)
		{
			if (this.APPaymentList.Current != null)
			{
				PXRedirectHelper.TryRedirect(
					this.APPaymentList.Cache,
					this.APPaymentList.Current,
					"View Document",
					PXRedirectHelper.WindowMode.NewWindow);
			}

			return adapter.Get();
		}
		#region Actions for assign access rights
		public PXAction<ReleaseChecksFilter> Release;
		[PXProcessButton]
		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		public IEnumerable release(PXAdapter a)
		{
			return a.Get();
		}
		public PXAction<ReleaseChecksFilter> Reprint;
		[PXProcessButton]
		[PXUIField(DisplayName = "Reprint", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		public IEnumerable reprint(PXAdapter a)
		{
			return a.Get();
		}
		public PXAction<ReleaseChecksFilter> VoidReprint;
		[PXProcessButton]
		[PXUIField(DisplayName = "Void and Reprint", MapEnableRights = PXCacheRights.Delete, MapViewRights = PXCacheRights.Delete)]
		public IEnumerable voidReprint(PXAdapter a)
		{
			return a.Get();
		}
		#endregion

		#region Setups
		public PXSetup<APSetup> APSetup;
		public PXSetup<GL.Company> Company;

		public PXSetup<PaymentMethodAccount, Where<PaymentMethodAccount.cashAccountID, Equal<Optional<ReleaseChecksFilter.payAccountID>>, And<PaymentMethodAccount.paymentMethodID, Equal<Optional<ReleaseChecksFilter.payTypeID>>>>> cashaccountdetail;
		#endregion

		public virtual void SetPreloaded(APPrintChecks graph)
		{
			ReleaseChecksFilter filter_copy = PXCache<ReleaseChecksFilter>.CreateCopy(this.Filter.Current);
			filter_copy.PayAccountID = graph.Filter.Current.PayAccountID;
			filter_copy.PayTypeID = graph.Filter.Current.PayTypeID;
			filter_copy.CuryID = graph.Filter.Current.CuryID;
			this.Filter.Cache.Update(filter_copy);

			foreach (APPayment seldoc in graph.APPaymentList.Cache.Updated)
			{
				seldoc.Passed = true;
				this.APPaymentList.Cache.Update(seldoc);
				this.APPaymentList.Cache.SetStatus(seldoc, PXEntryStatus.Updated);
			}
			this.APPaymentList.Cache.IsDirty = false;

			this.TimeStamp = graph.TimeStamp;
		}

		public APReleaseChecks()
		{
			APSetup setup = APSetup.Current;
			PXUIFieldAttribute.SetEnabled(APPaymentList.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<APPayment.selected>(APPaymentList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<APPayment.printCheck>(APPaymentList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<APPayment.extRefNbr>(APPaymentList.Cache, null, true);

			PXUIFieldAttribute.SetDisplayName<APPayment.printCheck>(APPaymentList.Cache, Messages.ReprintCaption);

			PXUIFieldAttribute.SetVisible<APPayment.printCheck>(APPaymentList.Cache, null, false);
			PXUIFieldAttribute.SetVisibility<APPayment.printCheck>(APPaymentList.Cache, null, PXUIVisibility.Invisible);
		}

		bool cleared;
		public override void Clear()
		{
			cleared = true;
			base.Clear();
		}

		protected virtual IEnumerable appaymentlist()
		{
			if (cleared)
			{
				foreach (APPayment doc in APPaymentList.Cache.Updated)
				{
					doc.Passed = false;
				}
			}

			return PXSelectJoin<APPayment,
				InnerJoinSingleTable<Vendor, On<Vendor.bAccountID, Equal<APPayment.vendorID>>>,
				Where<APPayment.released, Equal<False>,
					And<APPayment.printed, Equal<True>,
					And<APPayment.docType, NotEqual<APDocType.prepayment>, 
					And<APPayment.docType, NotEqual<APDocType.refund>,
					And<APPayment.cashAccountID, Equal<Current<ReleaseChecksFilter.payAccountID>>,
					And<APPayment.paymentMethodID, Equal<Current<ReleaseChecksFilter.payTypeID>>,
					And<Match<Vendor, Current<AccessInfo.userName>>>>>>>>>>
				.Select(this).RowCast<APPayment>();
		}

		public static void ReleasePayments(List<APPayment> list, string Action)
		{
			APReleaseChecks graph = CreateInstance<APReleaseChecks>();
			APPaymentEntry pe = CreateInstance<APPaymentEntry>();
			bool failed = false;
			bool successed = false;

			List<APRegister> docs = new List<APRegister>(list.Count);

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Passed == true)
				{
					graph.TimeStamp = pe.TimeStamp = list[i].tstamp;
				}

				switch (Action)
				{
					case "R":
						list[i].Printed = true;
						break;
					case "D":
					case "V":
						list[i].ExtRefNbr = null;
						list[i].Printed = false;
						break;
					default:
						continue;
				}

				if ((bool)list[i].Printed)
				{
					try
					{
						APPrintChecks.AssignNumbersWithNoAdditionalProcessing(pe, list[i]);
						pe.Save.Press();

						object[] persisted = PXTimeStampScope.GetPersisted(pe.Document.Cache, pe.Document.Current);
						if (persisted == null || persisted.Length == 0)
						{
							//preserve timestamp which will be @@dbts after last record committed to db on previous Persist().
							//otherwise another process updated APAdjust.
							docs.Add(list[i]);
						}
						else
						{
							if (list[i].Passed == true)
							{
								pe.Document.Current.Passed = true;
							}
							docs.Add(pe.Document.Current);
						}
						successed = true;
					}
					catch (Exception e)
					{
						PXProcessing<APPayment>.SetError(i, e);
						docs.Add(null);
						failed = true;
					}
				}
				else
				{
					try
					{
						list[i].Hold = true;
						// TODO: Need to rework. Use legal CRUD methods of caches!
						graph.APPaymentList.Cache.PersistUpdated(list[i]);
						graph.APPaymentList.Cache.Persisted(false);

						graph.TimeStamp = PXDatabase.SelectTimeStamp();

						// delete check numbers only if Reprint (not Void and Reprint)
						PaymentMethod pm = pe.paymenttype.Select(list[i].PaymentMethodID);
						if (pm.APPrintChecks == true && Action == "D")
						{
							APPayment doc = list[i];
							new HashSet<string>(pe.Adjustments_Raw.Select(doc.DocType, doc.RefNbr, doc.LineCntr)
								.RowCast<APAdjust>()
								.Select(adj => adj.StubNbr))
								.ForEach(nbr => PaymentRefAttribute.DeleteCheck((int) doc.CashAccountID, doc.PaymentMethodID, nbr));

							// sync PaymentMethodAccount.APLastRefNbr with actual last CashAccountCheck number
							PaymentMethodAccount det = graph.cashaccountdetail.SelectSingle(list[i].CashAccountID, list[i].PaymentMethodID);
							PaymentRefAttribute.LastCashAccountCheckSelect.Clear(graph);
							CashAccountCheck cacheck = PaymentRefAttribute.LastCashAccountCheckSelect.SelectSingleBound(graph, new object[] { det });
							det.APLastRefNbr = cacheck == null ? null : cacheck.CheckNbr;
							graph.cashaccountdetail.Cache.PersistUpdated(det);
							graph.cashaccountdetail.Cache.Persisted(false);
						}
						// END TODO
						if (string.IsNullOrEmpty(list[i].ExtRefNbr))
						{
							//try to get next number
							graph.APPaymentList.Cache.SetDefaultExt<APPayment.extRefNbr>(list[i]);
						}
					}
					catch (Exception e)
					{
						PXProcessing<APPayment>.SetError(i, e);
					}
					docs.Add(null);
				}
			}
			if (successed)
			{
				APDocumentRelease.ReleaseDoc(docs, true);
			}

			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
		}

		protected virtual void ReleaseChecksFilter_PayAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Filter.Cache.SetDefaultExt<ReleaseChecksFilter.curyID>(e.Row);
			APPaymentList.Cache.Clear();
		}

		protected virtual void ReleaseChecksFilter_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			//redefault to release action when saved values are populated in filter
			if (((ReleaseChecksFilter)e.OldRow).PayAccountID == null && ((ReleaseChecksFilter)e.OldRow).PayTypeID == null)
			{
				((ReleaseChecksFilter)e.Row).Action = "R";
			}
		}

		protected virtual void ReleaseChecksFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetVisible<ReleaseChecksFilter.curyID>(sender, null, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			if (e.Row == null) return;

			PaymentMethod paymentType = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Current<ReleaseChecksFilter.payTypeID>>>>.Select(this);
			var actions = new List<string>();
			var actionNames = new List<string>();
			AddAvailableAction(this.Release, "R", actions, actionNames, e.Row);

			if (paymentType != null && paymentType.PrintOrExport == true)
			{
				AddAvailableAction(this.Reprint, "D", actions, actionNames, e.Row);
				AddAvailableAction(this.VoidReprint, "V", actions, actionNames, e.Row);
			}

			PXStringListAttribute.SetList<ReleaseChecksFilter.action>(Filter.Cache, null,
							 actions.ToArray(),
							 actionNames.ToArray());


			var row = e.Row as ReleaseChecksFilter;
			if (row == null) return;

			if (actions.Count > 0)
			{
				if (row.Action == null || !actions.Contains(row.Action))
					row.Action = actions[0];
			}
			else
				row.Action = null;

			string action = row.Action;
			this.APPaymentList.SetProcessEnabled(action != null);
			this.APPaymentList.SetProcessAllEnabled(action != null);

			APPaymentList.SetProcessDelegate(list => ReleasePayments(list, action));
		}
		private void AddAvailableAction(PXAction action, string shortcut, List<string> actions, List<string> actionNames, object row)
		{
			PXButtonState state = action.GetState(row) as PXButtonState;
			if (state == null || !state.Enabled) return;
			actions.Add(shortcut);
			actionNames.Add(state.DisplayName);
		}

		protected virtual void ReleaseChecksFilter_PayTypeID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			sender.SetDefaultExt<ReleaseChecksFilter.payAccountID>(e.Row);
		}


		protected virtual void APPayment_PrintCheck_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if ((bool)((APPayment)e.Row).Printed == false)
			{
				((APPayment)e.Row).Selected = true;
			}
		}
	}
	[Serializable]
	public partial class ReleaseChecksFilter : PX.Data.IBqlTable
	{
		#region PayTypeID
		public abstract class payTypeID : PX.Data.IBqlField
		{
		}
		protected String _PayTypeID;
		[PXDefault()]
		[PXDBString(10, IsUnicode = true)]
		[PXUIField(DisplayName = "Payment Method", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<PaymentMethod.paymentMethodID,
						  Where<PaymentMethod.useForAP, Equal<True>>>))]		
		public virtual String PayTypeID
		{
			get
			{
				return this._PayTypeID;
			}
			set
			{
				this._PayTypeID = value;
			}
		}
		#endregion
		#region PayAccountID
		public abstract class payAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _PayAccountID;
		[CashAccount(null, typeof(Search2<CashAccount.cashAccountID,
						   InnerJoin<PaymentMethodAccount,
							   On<PaymentMethodAccount.cashAccountID, Equal<CashAccount.cashAccountID>>>,
						   Where2<Match<Current<AccessInfo.userName>>,
						   And<CashAccount.clearingAccount, Equal<False>,
						   And<PaymentMethodAccount.paymentMethodID, Equal<Current<ReleaseChecksFilter.payTypeID>>,
						   And<PaymentMethodAccount.useForAP, Equal<True>>>>>>), Visibility = PXUIVisibility.Visible)]
		[PXDefault(typeof(Search2<PaymentMethodAccount.cashAccountID,
							InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<PaymentMethodAccount.cashAccountID>>>,
										Where<PaymentMethodAccount.paymentMethodID, Equal<Current<ReleaseChecksFilter.payTypeID>>,
											And<PaymentMethodAccount.useForAP, Equal<True>,
											And<PaymentMethodAccount.aPIsDefault, Equal<True>,
											And<CashAccount.branchID, Equal<Current<AccessInfo.branchID>>>>>>>))]
		public virtual Int32? PayAccountID
		{
			get
			{
				return this._PayAccountID;
			}
			set
			{
				this._PayAccountID = value;
			}
		}
		#endregion
		
		#region Action
		public abstract class action : PX.Data.IBqlField
		{
		}
		protected string _Action;
		[PXDBString(10)]
		[PXUIField(DisplayName = "Action")]
		[PXStringList(new string[] { "R" }, new string[] { Messages.ReleaseChecks })]
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
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PXDefault(typeof(Search<CashAccount.curyID, Where<CashAccount.cashAccountID, Equal<Current<ReleaseChecksFilter.payAccountID>>>>))]
		[PXSelector(typeof(Currency.curyID))]
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
		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(ModuleCode = "AP")]
		public virtual Int64? CuryInfoID
		{
			get
			{
				return this._CuryInfoID;
			}
			set
			{
				this._CuryInfoID = value;
			}
		}
		#endregion
		#region CashBalance
		public abstract class cashBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _CashBalance;
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXDBCury(typeof(ReleaseChecksFilter.curyID))]
		[PXUIField(DisplayName = "Available Balance", Enabled = false)]
		[CashBalance(typeof(ReleaseChecksFilter.payAccountID))]
		public virtual Decimal? CashBalance
		{
			get
			{
				return this._CashBalance;
			}
			set
			{
				this._CashBalance = value;
			}
		}
		#endregion
		#region PayFinPeriodID
		public abstract class payFinPeriodID : PX.Data.IBqlField
		{
		}
		protected string _PayFinPeriodID;
		[FinPeriodID(typeof(AccessInfo.businessDate))]
		[PXUIField(DisplayName = "Post Period", Visibility = PXUIVisibility.Visible)]
		public virtual String PayFinPeriodID
		{
			get
			{
				return this._PayFinPeriodID;
			}
			set
			{
				this._PayFinPeriodID = value;
			}
		}
		#endregion
		#region GLBalance
		public abstract class gLBalance : PX.Data.IBqlField
		{
		}
		protected Decimal? _GLBalance;

		[PXDefault(TypeCode.Decimal, "0.0")]
		//[PXDBDecimal()]
		[PXDBCury(typeof(ReleaseChecksFilter.curyID))]
		[PXUIField(DisplayName = "GL Balance", Enabled = false)]
		[GLBalance(typeof(ReleaseChecksFilter.payAccountID), typeof(ReleaseChecksFilter.payFinPeriodID))]
		public virtual Decimal? GLBalance
		{
			get
			{
				return this._GLBalance;
			}
			set
			{
				this._GLBalance = value;
			}
		}
		#endregion
	}
}
