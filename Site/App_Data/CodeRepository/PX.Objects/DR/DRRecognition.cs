using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using System.Collections;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CM;
using System.Diagnostics;
using PX.Objects.IN;
using PX.Objects.AR;

namespace PX.Objects.DR
{
	[TableAndChartDashboardType]
	public class DRRecognition: PXGraph<DRRecognition>
	{
		public PXCancel<ScheduleRecognitionFilter> Cancel;
		public PXAction<ScheduleRecognitionFilter> viewSchedule;
		public PXFilter<ScheduleRecognitionFilter> Filter;
		[PXFilterable]
		public PXFilteredProcessing<ScheduledTran, ScheduleRecognitionFilter> Items;
		
		protected virtual IEnumerable items()
		{
			ScheduleRecognitionFilter filter = Filter.Current;
			if (filter == null)
			{
				yield break;
			}
			bool found = false;
			foreach (ScheduledTran item in Items.Cache.Inserted)
			{
				found = true;
				yield return item;
			}
			if (found)
				yield break;


			PXSelectBase<DRScheduleTran> select = new PXSelectJoin<DRScheduleTran,
				InnerJoin<DRScheduleDetail, On<DRScheduleTran.scheduleID, Equal<DRScheduleDetail.scheduleID>, And<DRScheduleTran.componentID, Equal<DRScheduleDetail.componentID>>>,
				LeftJoin<InventoryItem, On<DRScheduleTran.componentID, Equal<InventoryItem.inventoryID>>>>,
				Where<DRScheduleTran.recDate, LessEqual<Current<ScheduleRecognitionFilter.recDate>>,
				And<DRScheduleTran.status, Equal<DRScheduleTranStatus.OpenStatus>,
				And<DRScheduleDetail.status, NotEqual<DRScheduleStatus.DraftStatus>>>>,
				OrderBy<Asc<DRScheduleTran.scheduleID, Asc<DRScheduleTran.componentID, Asc<DRScheduleTran.recDate, Asc<DRScheduleTran.lineNbr>>>>>>(this);

			if (!string.IsNullOrEmpty(filter.DeferredCode))
			{
				select.WhereAnd<Where<DRScheduleDetail.defCode, Equal<Current<ScheduleRecognitionFilter.deferredCode>>>>();
			}

			Dictionary<string, string> added = new Dictionary<string, string>();

			foreach (PXResult<DRScheduleTran, DRScheduleDetail, InventoryItem> resultSet in select.Select())
			{
				DRScheduleTran tran = (DRScheduleTran)resultSet;
				DRScheduleDetail schedule = (DRScheduleDetail)resultSet;
				InventoryItem item = (InventoryItem)resultSet;

				string key = string.Format("{0}.{1}", tran.ScheduleID, tran.ComponentID);

				bool doInsert = false;

				if (added.ContainsKey(key))
				{
					string addedFinPeriod = added[key];

					if (tran.FinPeriodID == addedFinPeriod)
					{
						doInsert = true;
					}
				}
				else
				{
					doInsert = true;
					added.Add(key, tran.FinPeriodID);
				}

				if (doInsert)
				{
					ScheduledTran result = new ScheduledTran();
				    result.BranchID = tran.BranchID;
					result.AccountID = tran.AccountID;
					result.Amount = tran.Amount;
					result.ComponentID = tran.ComponentID;
					result.DefCode = schedule.DefCode;
					result.FinPeriodID = tran.FinPeriodID;
					result.LineNbr = tran.LineNbr;
					result.RecDate = tran.RecDate;
					result.ScheduleID = tran.ScheduleID;
					result.SubID = tran.SubID;
					result.ComponentCD = item.InventoryCD;
					result.DocType = DRScheduleDocumentType.BuildDocumentType(schedule.Module, schedule.DocType);
					
					Items.Cache.SetStatus(result, PXEntryStatus.Inserted);
					yield return result;// Items.Insert(result);
				}
			}


			//Virtual Records (CashReceipt):
			
			PXSelectBase<ARInvoice> s = null;

			if (!string.IsNullOrEmpty(filter.DeferredCode))
			{
				s = new PXSelectJoinGroupBy<ARInvoice,
				InnerJoin<ARTran, On<ARTran.tranType, Equal<ARInvoice.docType>,
					And<ARTran.refNbr, Equal<ARInvoice.refNbr>>>,
				InnerJoin<DRDeferredCode, On<ARTran.deferredCode, Equal<DRDeferredCode.deferredCodeID>,
					And<DRDeferredCode.method, Equal<DeferredMethodType.cashReceipt>,
					And<DRDeferredCode.deferredCodeID, Equal<Current<ScheduleRecognitionFilter.deferredCode>>>>>,
				InnerJoin<DRSchedule, On<ARTran.tranType, Equal<DRSchedule.docType>,
					And<ARTran.refNbr, Equal<DRSchedule.refNbr>,
					And<ARTran.lineNbr, Equal<DRSchedule.lineNbr>>>>,
				InnerJoin<DRScheduleDetail, On<DRSchedule.scheduleID, Equal<DRScheduleDetail.scheduleID>>>>>>,
				Where<ARInvoice.released, Equal<True>,
				And<DRScheduleDetail.isOpen, Equal<True>>>,
				Aggregate<GroupBy<ARInvoice.docType, GroupBy<ARInvoice.refNbr>>>>(this);
			}
			else
			{
				s = new PXSelectJoinGroupBy<ARInvoice,
				InnerJoin<ARTran, On<ARTran.tranType, Equal<ARInvoice.docType>,
					And<ARTran.refNbr, Equal<ARInvoice.refNbr>>>,
				InnerJoin<DRDeferredCode, On<ARTran.deferredCode, Equal<DRDeferredCode.deferredCodeID>,
					And<DRDeferredCode.method, Equal<DeferredMethodType.cashReceipt>>>,
				InnerJoin<DRSchedule, On<ARTran.tranType, Equal<DRSchedule.docType>,
					And<ARTran.refNbr, Equal<DRSchedule.refNbr>,
					And<ARTran.lineNbr, Equal<DRSchedule.lineNbr>>>>,
				InnerJoin<DRScheduleDetail, On<DRSchedule.scheduleID, Equal<DRScheduleDetail.scheduleID>>>>>>,
				Where<ARInvoice.released, Equal<True>,
				And<DRScheduleDetail.isOpen, Equal<True>>>,
				Aggregate<GroupBy<ARInvoice.docType, GroupBy<ARInvoice.refNbr>>>>(this);
			}


			foreach (ARInvoice inv in s.Select())
			{
				PXSelectBase<ARTran> trs =
					new PXSelectJoin<ARTran,
						InnerJoin<DRDeferredCode, On<ARTran.deferredCode, Equal<DRDeferredCode.deferredCodeID>,
						And<DRDeferredCode.method, Equal<DeferredMethodType.cashReceipt>>>>,
						Where<ARTran.tranType, Equal<Required<ARTran.tranType>>,
						And<ARTran.refNbr, Equal<Required<ARTran.refNbr>>>>>(this);

				foreach ( PXResult<ARTran, DRDeferredCode> res in trs.Select(inv.DocType, inv.RefNbr) )
				{
					List<ScheduledTran> virtualRecords = new List<ScheduledTran>();
					List<ScheduledTran> virtualVoidedRecords = new List<ScheduledTran>();

					ARTran tr = (ARTran)res;
					DRDeferredCode dc = (DRDeferredCode)res;

					decimal trPart = 0;
					if ( inv.LineTotal.Value != 0 )
						trPart = tr.TranAmt.Value / inv.LineTotal.Value;
					decimal trPartRest = tr.TranAmt.Value;

					InventoryItem invItem = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, tr.InventoryID);
					
					//NOTE: Multiple Components are not supported in CashReceipt Deferred Revenue Recognition.
					DRSchedule schedule = GetScheduleByFID(inv.DocType, inv.RefNbr, tr.LineNbr);
					DRScheduleDetail scheduleDetail = GetScheduleDetailbyID(schedule.ScheduleID, tr.InventoryID != null ? tr.InventoryID : DRScheduleDetail.EmptyComponentID );
					int lineNbr = scheduleDetail.LineCntr ?? 0;
					
					
					PXSelectBase<ARAdjust> ads =
						new PXSelectJoin<ARAdjust,
							LeftJoin<DRScheduleTran, On<ARAdjust.adjgDocType, Equal<DRScheduleTran.adjgDocType>,
								And<ARAdjust.adjgRefNbr, Equal<DRScheduleTran.adjgRefNbr>>>>,
							Where<ARAdjust.adjdDocType, Equal<Required<ARAdjust.adjdDocType>>,
							And<ARAdjust.adjdRefNbr, Equal<Required<ARAdjust.adjdRefNbr>>,
							And<DRScheduleTran.scheduleID, IsNull,
							And<ARAdjust.adjgDocType, NotEqual<ARDocType.creditMemo>>>>>,
							OrderBy<Asc<ARAdjust.adjgDocDate>>>(this);

                    foreach (ARAdjust ad in ads.Select(inv.DocType, inv.RefNbr))
					{
						lineNbr++;
						decimal amtRaw = Math.Min(trPart * ad.AdjAmt.Value, trPartRest);
						trPartRest -= amtRaw;
						decimal amt = PXDBCurrencyAttribute.BaseRound(this, amtRaw);

						ScheduledTran result = new ScheduledTran();

					    result.BranchID = ad.AdjgBranchID;
						result.Amount = amt;
						result.ComponentID = tr.InventoryID;
						result.DefCode = tr.DeferredCode;
						result.FinPeriodID = FinPeriodIDAttribute.PeriodFromDate(this, ad.AdjgDocDate);
						result.LineNbr = lineNbr;
						result.Module = schedule.Module;
						result.RecDate = ad.AdjgDocDate;
						result.ScheduleID = schedule.ScheduleID;
						result.DocType = schedule.DocType;
						result.AdjgDocType = ad.AdjgDocType;
						result.AdjgRefNbr = ad.AdjgRefNbr;
						result.AdjNbr = ad.AdjNbr;
						result.IsVirtual = true;
						result.AccountID = scheduleDetail.AccountID;
						result.SubID = scheduleDetail.SubID;
						result.ComponentCD = invItem == null ? "" : invItem.InventoryCD;
												
						if (ad.Voided == true)
						{
							if (ad.AdjgDocType == ARDocType.VoidPayment && virtualVoidedRecords.Count > 0)
							{
								ScheduledTran tran = virtualVoidedRecords.Where<ScheduledTran>( v => (v.AdjgDocType == ARDocType.Payment && v.AdjgRefNbr == ad.AdjgRefNbr && v.AdjNbr == ad.AdjNbr) ).First<ScheduledTran>();
								if ( tran != null )
									virtualVoidedRecords.Remove(tran);
							}
							else
							{
								virtualVoidedRecords.Add(result);
							}
						}
						else
						{
							
							virtualRecords.Add(result);
						}
					}

					foreach (ScheduledTran v in virtualRecords)
					{
						Items.Cache.SetStatus(v, PXEntryStatus.Inserted);
						yield return v;// Items.Insert(v);
					}

					foreach (ScheduledTran v in virtualVoidedRecords)
					{
						Items.Cache.SetStatus(v, PXEntryStatus.Inserted);
						yield return v;// Items.Insert(v);
					}

				}
			}
			

			Items.Cache.IsDirty = false;
		}

		#region Actions / Buttons

		
		[PXUIField(DisplayName = "", Visible = false)]
		[PXEditDetailButton]
		public virtual IEnumerable ViewSchedule(PXAdapter adapter)
		{
			if (Items.Current != null)
			{
				NavigationHelper.NavigateToDeferralSchedule(this, Items.Current.ScheduleID);
			}
			return adapter.Get();
		} 

		#endregion

				
		public DRRecognition()
		{
			Items.SetSelected<ScheduledTran.selected>();
		}

		#region EventHandlers
		protected virtual void ScheduleRecognitionFilter_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			Items.Cache.Clear();
		}

		protected virtual void ScheduleRecognitionFilter_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			ScheduleRecognitionFilter filter = Filter.Current;
            DateTime? filterDate = filter.RecDate;

            Items.SetProcessDelegate(
                    delegate(List<ScheduledTran> items)
					{
                        RunRecognition(items, filterDate);
					});
		}

		#endregion

		public static void RunRecognition(List<ScheduledTran> items, DateTime? filterDate)
		{
			ScheduleMaint scheduleMaint = PXGraph.CreateInstance<ScheduleMaint>();
			scheduleMaint.Clear();

			// Save virtual records:
			// -
			foreach (ScheduledTran tr in items)
			{
				if (tr.IsVirtual == true)
				{
					scheduleMaint.Document.Current = PXSelect<DRScheduleDetail, 
						Where<DRScheduleDetail.scheduleID, Equal<Required<DRScheduleDetail.scheduleID>>,
						And<DRScheduleDetail.componentID, Equal<Required<DRScheduleDetail.componentID>>>>>
					.Select(scheduleMaint, tr.ScheduleID, tr.ComponentID ?? DRScheduleDetail.EmptyComponentID);
								
					DRScheduleTran tran = new DRScheduleTran();
				    tran.BranchID = tr.BranchID;
					tran.AccountID = tr.AccountID;
					tran.SubID = tr.SubID; 
					tran.AdjgDocType = tr.AdjgDocType;
					tran.AdjgRefNbr = tr.AdjgRefNbr;
					tran.AdjNbr = tr.AdjNbr;
					tran.Amount = tr.Amount;
					tran.ComponentID = tr.ComponentID ?? DRScheduleDetail.EmptyComponentID;
					tran.FinPeriodID = tr.FinPeriodID;
					tran.ScheduleID = tr.ScheduleID;
					tran.RecDate = tr.RecDate;
					tran.Status = DRScheduleTranStatus.Open;
					
					tran = scheduleMaint.OpenTransactions.Insert(tran);
					tr.LineNbr = tran.LineNbr;

					scheduleMaint.RebuildProjections(tran.BranchID);
										
					scheduleMaint.Save.Press();
					byte[] ts = scheduleMaint.TimeStamp;
					scheduleMaint.Clear();
					scheduleMaint.TimeStamp = ts;
				}
			}

			SortedList<string, DRBatch> list = SplitByFinPeriod(items);

			DRProcess process = PXGraph.CreateInstance<DRProcess>();
			process.Clear();
			process.TimeStamp = scheduleMaint.TimeStamp;
			List<Batch> batchlist = process.RunRecognition(list, filterDate);

			PostGraph pg = PXGraph.CreateInstance<PostGraph>();
			//Post Batches if AutoPost

			bool postFailed = false;
			if (pg.AutoPost)
			{
				foreach (Batch batch in batchlist)
				{
					try
					{
						pg.Clear();
						pg.TimeStamp = batch.tstamp;
						pg.PostBatchProc(batch);
					}
					catch (Exception)
					{
						postFailed = true;
					}
				}
				if (postFailed)
				{
					throw new PXException(Messages.AutoPostFailed);
				}
			}

		}

		private static SortedList<string, DRBatch> SplitByFinPeriod(List<ScheduledTran> items)
		{
			SortedList<string, DRBatch> list = new SortedList<string, DRBatch>();

			foreach (ScheduledTran item in items)
			{
				if (list.ContainsKey(item.FinPeriodID))
				{
					list[item.FinPeriodID].Trans.Add(new DRTranKey(item.ScheduleID, item.ComponentID ?? DRScheduleDetail.EmptyComponentID, item.LineNbr));
				}
				else
				{
					list.Add(item.FinPeriodID, new DRBatch(item.FinPeriodID, item.ScheduleID, item.ComponentID ?? DRScheduleDetail.EmptyComponentID, item.LineNbr));
				}
			}

			return list;
		}


		private DRSchedule GetScheduleByFID(string docType, string refNbr, int? lineNbr)
		{
			return PXSelect<DRSchedule,
						Where<DRSchedule.docType, Equal<Required<DRSchedule.docType>>,
						And<DRSchedule.refNbr, Equal<Required<DRSchedule.refNbr>>,
						And<DRSchedule.lineNbr, Equal<Required<DRSchedule.lineNbr>>>>>>.Select(this, docType, refNbr, lineNbr);

		}

		private DRScheduleDetail GetScheduleDetailbyID(int? scheduleID, int? inventoryID)
		{
			return PXSelect<DRScheduleDetail,
							Where<DRScheduleDetail.scheduleID, Equal<Required<DRScheduleDetail.scheduleID>>,
							And<DRScheduleDetail.componentID, Equal<Required<DRScheduleDetail.componentID>>>>>.Select(this, scheduleID, inventoryID);
		}
		
		#region Local Types
		[Serializable]
		public partial class ScheduleRecognitionFilter : IBqlTable
		{
			#region RecDate
			public abstract class recDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _RecDate;
			[PXDBDate()]
			[PXDefault(typeof(AccessInfo.businessDate))]
			[PXUIField(DisplayName = "Recognition Date")]
			public virtual DateTime? RecDate
			{
				get
				{
					return this._RecDate;
				}
				set
				{
					this._RecDate = value;
				}
			}
			#endregion
			#region DeferredCode
			public abstract class deferredCode : PX.Data.IBqlField
			{
			}
			protected String _DeferredCode;
			[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
			[PXUIField(DisplayName = "Deferral Code")]
			[PXSelector(typeof(DRDeferredCode.deferredCodeID))]
			public virtual String DeferredCode
			{
				get
				{
					return this._DeferredCode;
				}
				set
				{
					this._DeferredCode = value;
				}
			}
			#endregion
		}

		[Serializable]
		public partial class ScheduledTran : PX.Data.IBqlTable
		{
			#region ScheduleID
			public abstract class scheduleID : PX.Data.IBqlField
			{
			}
			protected Int32? _ScheduleID;
			[PXDBInt(IsKey = true)]
			[PXDefault()]
			[PXUIField(DisplayName = "Schedule ID", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual Int32? ScheduleID
			{
				get
				{
					return this._ScheduleID;
				}
				set
				{
					this._ScheduleID = value;
				}
			}
			#endregion
			#region ComponentID
			public abstract class componentID : PX.Data.IBqlField
			{
			}
			protected int? _ComponentID;
			[PXUIField(DisplayName = Messages.ComponentID, Visibility = PXUIVisibility.Visible)]
			[PXDBInt(IsKey = true)]
			[PXSelector(typeof(Search2<DRScheduleDetail.componentID, LeftJoin<InventoryItem, On<DRScheduleDetail.componentID, Equal<InventoryItem.inventoryID>>>, Where<DRScheduleDetail.scheduleID, Equal<Current<ScheduledTran.scheduleID>>>>), new Type[] { typeof(InventoryItem.inventoryCD), typeof(InventoryItem.descr) })]
			public virtual int? ComponentID
			{
				get
				{
					return this._ComponentID;
				}
				set
				{
					this._ComponentID = value;
				}
			}
			#endregion
			#region ComponentCD
			public abstract class componentCD : PX.Data.IBqlField
			{
			}
			protected string _ComponentCD;
			[PXUIField(DisplayName = Messages.ComponentID, Visibility = PXUIVisibility.Visible)]
			[PXDBString]
			public virtual string ComponentCD
			{
				get
				{
					return this._ComponentCD;
				}
				set
				{
					this._ComponentCD = value;
				}
			}
			#endregion
			#region LineNbr
			public abstract class lineNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _LineNbr;
			[PXDBInt(IsKey = true)]
			[PXDefault()]
			[PXUIField(DisplayName = Messages.TransactionNumber, Enabled = false)]
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
            #region BranchID
            public abstract class branchID : PX.Data.IBqlField
            {
            }
            protected Int32? _BranchID;
            [Branch()]
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
			#region Module
			public abstract class module : PX.Data.IBqlField
			{
			}
			protected String _Module;
			[PXDBString(2, IsFixed = true)]
			[PXDefault("")]
			[PXUIField(DisplayName = "Module")]
			public virtual String Module
			{
				get
				{
					return this._Module;
				}
				set
				{
					this._Module = value;
				}
			}
			#endregion
			#region RecDate
			public abstract class recDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _RecDate;
			[PXDBDate()]
			[PXDefault()]
			[PXUIField(DisplayName = "Rec. Date")]
			public virtual DateTime? RecDate
			{
				get
				{
					return this._RecDate;
				}
				set
				{
					this._RecDate = value;
				}
			}
			#endregion
			#region Amount
			public abstract class amount : PX.Data.IBqlField
			{
			}
			protected Decimal? _Amount;
			[PXDBBaseCury()]
			[PXDefault(TypeCode.Decimal, "0.0")]
			[PXUIField(DisplayName = "Amount")]
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
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[Account(DisplayName = "Account", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Account.description))]
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
			//[SubAccount(typeof(DRScheduleTran.accountID), DisplayName = "Subaccount", Visibility = PXUIVisibility.Visible, DescriptionField = typeof(Sub.description))]
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
			#region FinPeriodID
			public abstract class finPeriodID : PX.Data.IBqlField
			{
			}
			protected String _FinPeriodID;
			[FinPeriodID()]
			[PXUIField(DisplayName = "Fin. Period", Enabled = false)]
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
			#region DefCode
			public abstract class defCode : PX.Data.IBqlField
			{
			}
			protected String _DefCode;
			[PXDBString(10, IsUnicode = true)]
			[PXDefault("")]
			[PXUIField(DisplayName = "Deferral Code", Enabled = false, Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String DefCode
			{
				get
				{
					return this._DefCode;
				}
				set
				{
					this._DefCode = value;
				}
			}
			#endregion
			#region DocType
			public abstract class docType : PX.Data.IBqlField
			{
			}
			protected String _DocType;
			[PXDBString(3, IsFixed = true, InputMask = "")]
			[PXUIField(DisplayName = Messages.DocumentType)]
			[DRScheduleDocumentType.List]
			public virtual String DocType
			{
				get
				{
					return this._DocType;
				}
				set
				{
					this._DocType = value;
				}
			}
			#endregion
			
			#region AdjgDocType
			public abstract class adjgDocType : PX.Data.IBqlField
			{
			}
			protected String _AdjgDocType;
			[PXDBString(3, IsFixed = true, InputMask = "")]
			public virtual String AdjgDocType
			{
				get
				{
					return this._AdjgDocType;
				}
				set
				{
					this._AdjgDocType = value;
				}
			}
			#endregion
			#region AdjgRefNbr
			public abstract class adjgRefNbr : PX.Data.IBqlField
			{
			}
			protected String _AdjgRefNbr;
			[PXDBString(15, IsUnicode = true)]
			public virtual String AdjgRefNbr
			{
				get
				{
					return this._AdjgRefNbr;
				}
				set
				{
					this._AdjgRefNbr = value;
				}
			}
			#endregion
			#region AdjNbr
			public abstract class adjNbr : PX.Data.IBqlField
			{
			}
			protected Int32? _AdjNbr;
			[PXDBInt()]
			public virtual Int32? AdjNbr
			{
				get
				{
					return this._AdjNbr;
				}
				set
				{
					this._AdjNbr = value;
				}
			}
			#endregion
			#region IsVirtual
			public abstract class isVirtual : IBqlField
			{
			}
			protected bool? _IsVirtual = false;
			[PXBool]
			[PXDefault(false)]
			public bool? IsVirtual
			{
				get
				{
					return _IsVirtual;
				}
				set
				{
					_IsVirtual = value;
				}
			}
			#endregion
				
			#region Selected
			public abstract class selected : IBqlField
			{
			}
			protected bool? _Selected = false;
			[PXBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Selected", Visibility = PXUIVisibility.Visible)]
			public bool? Selected
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

			public static int CompareFinPeriod(ScheduledTran a, ScheduledTran b)
			{
				return a.FinPeriodID.CompareTo(b.FinPeriodID);
			}
		}

		[DebuggerDisplay("{FinPeriod} Trans.Count={Trans.Count}")]
		public class DRBatch
		{
			public string FinPeriod { get; private set; }
			public List<DRTranKey> Trans { get; set; }

			public DRBatch(string finPeriod, int? scheduleID, int? componentID, int? lineNbr)
			{
				FinPeriod = finPeriod;
				Trans = new List<DRTranKey>();
				Trans.Add(new DRTranKey(scheduleID, componentID, lineNbr));
			}
		}

		[DebuggerDisplay("{ScheduleID}.{LineNbr}")]
		public struct DRTranKey
		{
			public int? ScheduleID;
			public int? ComponentID;
			public int? LineNbr;

			public DRTranKey(int? scheduleID, int? componentID, int? lineNbr)
			{
				this.ScheduleID = scheduleID;
				this.ComponentID = componentID;
				this.LineNbr = lineNbr;
			}
		}

		#endregion
	}

	
}
