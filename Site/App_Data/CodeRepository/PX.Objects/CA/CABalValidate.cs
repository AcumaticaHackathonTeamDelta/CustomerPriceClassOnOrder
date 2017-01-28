using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.AP;
using PX.Objects.AR;


namespace PX.Objects.CA
{
	[TableAndChartDashboardType]
	public class CABalValidate : PXGraph<CABalValidate>
	{
		public CABalValidate()
		{
			CASetup setup = CASetup.Current;

			CABalValidateList.SetProcessDelegate<CATranEntryLight>(Validate);

			CABalValidateList.SetProcessCaption(Messages.Validate);
			CABalValidateList.SetProcessAllCaption(Messages.ValidateAll);

			PXUIFieldAttribute.SetEnabled<CashAccount.selected>(CABalValidateList.Cache, null, true);
			PXUIFieldAttribute.SetEnabled<CashAccount.cashAccountCD>(CABalValidateList.Cache, null, false);
			PXUIFieldAttribute.SetEnabled<CashAccount.descr>(CABalValidateList.Cache, null, false);
		}

		public PXCancel<CashAccount> Cancel;

		[PXFilterable]
		public PXProcessing<CashAccount, Where<CashAccount.active, Equal<boolTrue>>> CABalValidateList;
		public PXSetup<CASetup> CASetup;

		protected virtual IEnumerable cABalValidateList()
		{
			bool anyFound = false;
			foreach (CashAccount tlist in CABalValidateList.Cache.Inserted)
			{
				anyFound = true;
				yield return tlist;
			}
			if (anyFound)
			{
				yield break;
			}

			foreach (CashAccount cash in PXSelect<CashAccount>.Select(this))
			{
				yield return cash;
			}
			CABalValidateList.Cache.IsDirty = false;
		}

		private static void Validate(CATranEntryLight te, CashAccount tlist)
		{
			if (tlist.Reconcile != true)
			{ 
				te.Clear();
				using (new PXConnectionScope())
				{
					using (PXTransactionScope ts = new PXTransactionScope())
					{
						PXCache adjcache = te.Caches[typeof(CATran)];
						foreach (CATran catran in PXSelect<CATran, Where<CATran.cashAccountID, Equal<Required<CATran.cashAccountID>>>>.Select(te, tlist.CashAccountID))
						{
							if (tlist.Reconcile != true && (catran.Cleared != true || catran.TranDate == null))
							{
								catran.Cleared   = true;
								catran.ClearDate = catran.TranDate;
							}
							te.catrancache.Update(catran);
						}
						te.catrancache.Persist(PXDBOperation.Update);
						ts.Complete(te);
					}
					te.catrancache.Persisted(false);
				}
			}

			te.Clear();

			using (new PXConnectionScope())
			{
				PXCache adjcache = te.Caches[typeof(CAAdj)];

				using (PXTransactionScope ts = new PXTransactionScope())
				{
					foreach (PXResult<CAAdj, CATran> res in PXSelectJoin<CAAdj, LeftJoin<CATran, On<CATran.tranID, Equal<CAAdj.tranID>>>, Where<CAAdj.cashAccountID, Equal<Required<CAAdj.cashAccountID>>, And<CATran.tranID, IsNull>>>.Select(te, tlist.CashAccountID))
					{
						CAAdj caadj = (CAAdj)res;

						adjcache.SetValue<CAAdj.tranID>(caadj, null);
						adjcache.SetValue<CAAdj.cleared>(caadj, false);

						CATran catran = AdjCashTranIDAttribute.DefaultValues<CAAdj.tranID>(adjcache, caadj);

						if (catran != null)
						{
							catran = (CATran)te.catrancache.Insert(catran);
							te.catrancache.PersistInserted(catran);
							long id = Convert.ToInt64(PXDatabase.SelectIdentity());

							adjcache.SetValue<CAAdj.tranID>(caadj, id);
							adjcache.Update(caadj);
						}
					}

					adjcache.Persist(PXDBOperation.Update);

					ts.Complete(te);
				}

				adjcache.Persisted(false);
				te.catrancache.Persisted(false);
			}

			te.Clear();

			using (new PXConnectionScope())
			{
				PXCache transfercache = te.Caches[typeof(CATransfer)];

				using (PXTransactionScope ts = new PXTransactionScope())
				{
					foreach (PXResult<CATransfer, CATran> res in PXSelectJoin<CATransfer, LeftJoin<CATran, On<CATran.tranID, Equal<CATransfer.tranIDIn>>>, Where<CATransfer.inAccountID, Equal<Required<CATransfer.inAccountID>>, And<CATran.tranID, IsNull>>>.Select(te, tlist.CashAccountID))
					{
						CATransfer catransfer = (CATransfer)res;

						transfercache.SetValue<CATransfer.tranIDIn>(catransfer, null);
						transfercache.SetValue<CATransfer.clearedIn>(catransfer, false);
						if (transfercache.GetValue<CATransfer.clearedOut>(catransfer) == null)
						{
							transfercache.SetValue<CATransfer.clearedOut>(catransfer, false);
						}

						CATran catran = TransferCashTranIDAttribute.DefaultValues<CATransfer.tranIDIn>(transfercache, catransfer);

						if (catran != null)
						{
							catran = (CATran)te.catrancache.Insert(catran);
							te.catrancache.PersistInserted(catran);
							long id = Convert.ToInt64(PXDatabase.SelectIdentity());

							transfercache.SetValue<CATransfer.tranIDIn>(catransfer, id);
							transfercache.Update(catransfer);
						}
					}

					foreach (PXResult<CATransfer, CATran> res in PXSelectJoin<CATransfer, LeftJoin<CATran, On<CATran.tranID, Equal<CATransfer.tranIDOut>>>, Where<CATransfer.outAccountID, Equal<Required<CATransfer.outAccountID>>, And<CATran.tranID, IsNull>>>.Select(te, tlist.CashAccountID))
					{
						CATransfer catransfer = (CATransfer)res;

						transfercache.SetValue<CATransfer.tranIDOut>(catransfer, null);
						transfercache.SetValue<CATransfer.clearedOut>(catransfer, false);
						if (transfercache.GetValue<CATransfer.clearedIn>(catransfer) == null)
						{
							transfercache.SetValue<CATransfer.clearedIn>(catransfer, false);
						}

						CATran catran = TransferCashTranIDAttribute.DefaultValues<CATransfer.tranIDOut>(transfercache, catransfer);

						if (catran != null)
						{
							catran = (CATran)te.catrancache.Insert(catran);
							te.catrancache.PersistInserted(catran);
							long id = Convert.ToInt64(PXDatabase.SelectIdentity());

							transfercache.SetValue<CATransfer.tranIDOut>(catransfer, id);
							transfercache.Update(catransfer);
						}
					}

					transfercache.Persist(PXDBOperation.Update);

					ts.Complete(te);
				}

				transfercache.Persisted(false);
				te.catrancache.Persisted(false);
			}

			te.Clear();

			PXDBDefaultAttribute.SetDefaultForUpdate<GLTran.module>(te.Caches[typeof(GLTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<GLTran.batchNbr>(te.Caches[typeof(GLTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<GLTran.ledgerID>(te.Caches[typeof(GLTran)], null, false);
			PXDBDefaultAttribute.SetDefaultForUpdate<GLTran.finPeriodID>(te.Caches[typeof(GLTran)], null, false);

			using (new PXConnectionScope())
			{
				const int rowsPerCycle = 10000;
				bool noMoreTran = false;
				while (!noMoreTran)
				{
					noMoreTran = true;
					using (PXTransactionScope ts = new PXTransactionScope())
					{
						foreach (PXResult<GLTran, Ledger, Batch> res in PXSelectJoin<GLTran, InnerJoin<Ledger, On<Ledger.ledgerID, Equal<GLTran.ledgerID>>,
									InnerJoin<Batch, On<Batch.module, Equal<GLTran.module>, And<Batch.batchNbr, Equal<GLTran.batchNbr>,
										And<Batch.scheduled, Equal<False>, And<Batch.voided, NotEqual<True>>>>>>>,
									Where<GLTran.accountID, Equal<Required<GLTran.accountID>>,
										And<GLTran.subID, Equal<Required<GLTran.subID>>,
										And<GLTran.branchID, Equal<Required<GLTran.branchID>>,
										And<Ledger.balanceType, Equal<LedgerBalanceType.actual>,
										//ignoring CM because DefaultValues always return null for CM
										And<GLTran.module, NotEqual<BatchModule.moduleCM>,
										And<GLTran.cATranID, IsNull>>>>>>>.SelectWindowed(te, 0, rowsPerCycle, tlist.AccountID, tlist.SubID, tlist.BranchID))
						{
							GLTran gltran = (GLTran)res;
							noMoreTran = false;
							CATran catran = GLCashTranIDAttribute.DefaultValues<GLTran.cATranID>(te.gltrancache, gltran);
							if (catran != null)
							{
								long id;
								bool newCATRan = false;
								if (te.catrancache.Locate(catran) == null)
								{
									catran = (CATran)te.catrancache.Insert(catran);
									newCATRan = true;
									te.catrancache.PersistInserted(catran);
									id = Convert.ToInt64(PXDatabase.SelectIdentity());
								}
								else
								{
									catran = (CATran)te.catrancache.Update(catran);
									te.catrancache.PersistUpdated(catran);
									id = catran.TranID.Value;
								}

								gltran.CATranID = id;
								te.gltrancache.Update(gltran);

								if (catran.OrigModule != CAAPARTranType.GLEntry)
								{
									switch (catran.OrigModule)
									{
										case BatchModule.AR:
											ARPayment arPayment = PXSelect<ARPayment, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>,
												And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(te, catran.OrigTranType, catran.OrigRefNbr);
											if (arPayment != null && (arPayment.CATranID == null || newCATRan))
											{
												arPayment.CATranID = id;
												arPayment = (ARPayment)te.Caches[typeof(ARPayment)].Update(arPayment);
												te.Caches[typeof(ARPayment)].PersistUpdated(arPayment);
											}
											break;
										case BatchModule.AP:
											APPayment apPayment = PXSelect<APPayment, Where<APPayment.docType, Equal<Required<APPayment.docType>>,
												And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(te, catran.OrigTranType, catran.OrigRefNbr);
											if (apPayment != null && (apPayment.CATranID == null || newCATRan))
											{
												apPayment.CATranID = id;
												apPayment = (APPayment)te.Caches[typeof(APPayment)].Update(apPayment);
												te.Caches[typeof(APPayment)].PersistUpdated(apPayment);
											}
											break;
									}
								}
							}
						}
						te.gltrancache.ClearQueryCache();
						te.gltrancache.Persist(PXDBOperation.Update);
						te.gltrancache.Clear();
						te.catrancache.Clear();
						te.catrancache.ClearQueryCache();
						te.dailycache.Clear();
						te.Caches[typeof(APPayment)].Clear();
						te.Caches[typeof(ARPayment)].Clear();
						ts.Complete(te);
					}
				}

				PXDatabase.Delete<CADailySummary>(
						new PXDataFieldRestrict("CashAccountID", PXDbType.Int, 4, tlist.CashAccountID, PXComp.EQ)
					);

				foreach (CATran tran in PXSelect<CATran, Where<CATran.cashAccountID, Equal<Required<CATran.cashAccountID>>>>.Select(te, tlist.CashAccountID))
				{
					CADailyAccumulatorAttribute.RowInserted<CATran.tranDate>(te.catrancache, tran);
				}

				te.dailycache.Persist(PXDBOperation.Insert);
				te.dailycache.Persist(PXDBOperation.Update);


				te.gltrancache.Persisted(false);
				te.catrancache.Persisted(false);
				te.dailycache.Persisted(false);
			}
		}
	}
	public class CATranEntryLight : CATranEntry
	{
		[PXDBString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Batch Number", Enabled = false)]
		public override void CATran_BatchNbr_CacheAttached(PXCache sender)
		{
		}
	}

}
