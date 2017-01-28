using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CS;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Api;
using PX.SM;
using PX.Data.EP;
using PX.Objects.CA.BankStatementHelpers;

namespace PX.Objects.CA
{
    [Serializable]
	public class CABankStatementEntry : PXGraph<CABankStatementEntry, CABankStatement>, PXImportAttribute.IPXPrepareItems
	{

		#region Buttons Definition		
		
		#region Button ViewDoc
		public PXAction<CABankStatement> viewDoc;
		[PXUIField(DisplayName = Messages.ViewDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewDoc(PXAdapter adapter)
		{
			CATran tran = this.DetailMatches.Current;
			if(tran != null)
		    {
				CATran.Redirect(this.DetailMatches.Cache, tran);
			}
			return adapter.Get();
		}
		#endregion

		#region Button ViewDetailsDoc
		public PXAction<CABankStatement> viewDetailsDoc;
		[PXUIField(DisplayName = Messages.ViewDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewDetailsDoc(PXAdapter adapter)
		{
			CABankStatementDetail row = this.Details.Current;
			if (row != null && row.CATranID!=null)
			{
				CATran tran = PXSelect<CATran, Where<CATran.cashAccountID, Equal<Required<CATran.cashAccountID>>,
								And<CATran.tranID, Equal<Required<CATran.tranID>>>>>.Select(this, row.CashAccountID, row.CATranID);
				CATran.Redirect(this.DetailMatches.Cache, tran);
			}
			return adapter.Get();
		}
		#endregion
		#region Button Upload File
		public PXSelect<CABankStatement> NewRevisionPanel;
		public PXAction<CABankStatement> uploadFile;
		[PXUIField(DisplayName = Messages.UploadFile, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]		
		[PXButton()]
		public virtual IEnumerable UploadFile(PXAdapter adapter)
		{
            bool doImport = true;
			if (this.casetup.Current.ImportToSingleAccount == true)
			{
				CABankStatement row = this.BankStatement.Current;
				if (row == null || this.BankStatement.Current.CashAccountID == null)
				{
					throw new PXException(Messages.CashAccountMustBeSelectedToImportStatement);
				}
				else
				{
					CashAccount acct = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, row.CashAccountID);
					if (acct != null && string.IsNullOrEmpty(acct.StatementImportTypeName))
					{
						throw new PXException(Messages.StatementImportServiceMustBeConfiguredForTheCashAccount);
					}
				}
			}
			else 
			{
				if (string.IsNullOrEmpty(this.casetup.Current.StatementImportTypeName))
				{
					throw new PXException(Messages.StatementImportServiceMustBeConfiguredInTheCASetup);
				}
			}

            if (this.BankStatement.Current != null && this.IsDirty == true)
            {
                if (this.casetup.Current.ImportToSingleAccount != true)
                {
                    if (this.BankStatement.Ask(Messages.ImportConfirmationTitle, Messages.UnsavedDataInThisScreenWillBeLostConfirmation, MessageButtons.YesNo) != WebDialogResult.Yes)
                    {
                        doImport = false;
                    }
                }
                else 
                {
                    doImport = true;
                }
            }

			if (doImport)
			{
				if (this.NewRevisionPanel.AskExt() == WebDialogResult.OK) 
				{
					const string PanelSessionKey = "ImportStatementFile";
					PX.SM.FileInfo info = PXContext.SessionTyped<PXSessionStatePXData>().FileInfo[PanelSessionKey] as PX.SM.FileInfo;
					System.Web.HttpContext.Current.Session.Remove(PanelSessionKey);
					this.ImportStatement(info);
					CABankStatement newRow = this.BankStatement.Current;
					List<CABankStatement> result = new List<CABankStatement>();
					result.Add(newRow);
					return result;
				}				
			}
			return adapter.Get();
		}

		#endregion

		#region Import
		public PXAction<CABankStatement> import;
		[PXUIField(DisplayName = Messages.ImportStatement, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update, Enabled = true)]
		[PXProcessButton]
		public virtual IEnumerable Import(PXAdapter adapter)
		{
			if (PXLongOperation.Exists(UID))
			{
				throw new ApplicationException(GL.Messages.PrevOperationNotCompleteYet);
			}

			CABankStatement doc = this.BankStatement.Current;
			if (doc != null && doc.Released == true && doc.Hold == false)
			{
				string importScenatioName = "Import Bank Statement MOM";
				SYMapping map = PXSelect<SYMapping, Where<SYMapping.name, Equal<Required<SYMapping.name>>>>.Select(this, importScenatioName);
				
				/*if (map!= null && map.MappingID!= null)
				{
					string defaultFileName = 
					PXLongOperation.StartOperation(this, delegate()
					{

						PX.Api.SYImportProcess.RunScenario(map.Name,
							new PX.Api.PXSYParameter(ExportProviderParams.FileName, defaultFileName),
							new PX.Api.PXSYParameter(ExportProviderParams.BatchNbr, doc.BatchNbr)
							//,
							//new PX.Api.PXSYParameter(ExportProviderParams.BatchSequenceStartingNbr, "0000")
							);
					});
				}
				else
				{
					throw new PXException(Messages.CABatchExportProviderIsNotConfigured);
				}*/
			}

			return adapter.Get();
		}
		
		#endregion
		#region AutoMatch
		public PXAction<CABankStatement> autoMatch;
		[PXUIField(DisplayName = "Auto Match", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXProcessButton]
		public virtual IEnumerable AutoMatch(PXAdapter adapter)
		{
			DoMatch();			
			return adapter.Get();
		}
		#endregion
		
		#region MatchTrans
		public PXAction<CABankStatement> matchTrans;

		[PXUIField(DisplayName = Messages.MatchSelected, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable MatchTrans(PXAdapter adapter)
		{
			CATranExt tran = this.DetailMatches.Current;
			CABankStatementDetail source = this.Details.Current;
			if (tran != null && source!=null)
			{
				
				if (source.CATranID.HasValue && source.CATranID != tran.TranID) 
				{
					throw new PXException(Messages.StatementDetailIsAlreadyMatched); 
				}
				else
				{
					source.CATranID = tran.TranID;
					source.DocumentMatched = true;
					this.Details.Update(source);
				}
			}
			return adapter.Get();
		}
		#endregion
		#region Clear Match
		public PXAction<CABankStatement> clearMatch;

		[PXUIField(DisplayName = Messages.ClearMatch, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXLookupButton]
		public virtual IEnumerable ClearMatch(PXAdapter adapter)
		{
			CABankStatementDetail source = this.Details.Current;
			if (source!=null)
			{
				source.CATranID = null;
				source.DocumentMatched = false;
				this.Details.Update(source);
			}
			return adapter.Get();
		}
		#endregion

		#region Clear All Matches
		public PXAction<CABankStatement> clearAllMatches;

		[PXUIField(DisplayName = "Clear All", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
		[PXProcessButton]
		public virtual IEnumerable ClearAllMatches(PXAdapter adapter)
		{ 
			foreach (CABankStatementDetail it in this.Details.Select())
			{
				if (it != null && it.CATranID!= null)
				{
					it.CATranID = null;
					it.DocumentMatched = false;
					this.Details.Update(it);
				}
			}
			return adapter.Get();
		}
		#endregion

		#region CreateAllDocuments
		public PXAction<CABankStatement> createAllDocuments;

		[PXUIField(DisplayName = Messages.CreateAllDocuments, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton]
		public virtual IEnumerable CreateAllDocuments(PXAdapter adapter)
		{
			CABankStatement source = this.BankStatement.Current;
			if (source != null && source.Released == false)
			{
				foreach (CABankStatementDetail iDet in this.Details.Select()) 
				{
					if (iDet.CreateDocument == true && iDet.CATranID == null) 
					{
						CreateDocumentProc(iDet,false);
					}
				}
				this.Save.Press();
			}
			return adapter.Get();
		}
		#endregion
		#region CreateDocument
		public PXAction<CABankStatement> createDocument;

		[PXUIField(DisplayName =Messages.CreateDocument, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton]
		public virtual IEnumerable CreateDocument(PXAdapter adapter)
		{
			CABankStatementDetail source = this.Details.Current;
			CABankStatement doc = this.BankStatement.Current;
			if (source != null 
				&& source.CATranID == null && (source.CreateDocument == true))
			{
				source.CashAccountID = doc.CashAccountID;
				this.CreateDocumentProc(source,false);
			}
			return adapter.Get();
		}
		#endregion

		#region Release
		public PXAction<CABankStatement> release;

		[PXUIField(DisplayName = Messages.Release, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton]
		public virtual IEnumerable Release(PXAdapter adapter)
		{
			
			PXCache cache = this.BankStatement.Cache;
			List<CABankStatement> list = new List<CABankStatement>();
			foreach (CABankStatement doc in adapter.Get<CABankStatement>())
			{
				if (doc.CuryReconciledBalanceDiff != Decimal.Zero || doc.CuryDetailsBalanceDiff != Decimal.Zero
					|| doc.CuryCreditsTotal != doc.CuryReconciledCredits || doc.CuryDebitsTotal != doc.CuryReconciledDebits
					|| doc.CountCredit != doc.ReconciledCountCredit || doc.CountDebit != doc.ReconciledCountDebit)
				{
					
					throw new PXException(Messages.DocumentIsUnbalancedItCanNotBeReleased);
				}
				if (!(bool)doc.Hold)
				{
					cache.Update(doc);
					list.Add(doc);						
				}				
			}
			if (list.Count == 0)
			{
				throw new PXException(Messages.DocumentOnHoldCanNotBeReleased);
			}
			this.Save.Press();
            PXLongOperation.ClearStatus(this.UID);
			PXLongOperation.StartOperation(this, delegate() { CABankStatementEntry.ReleaseDoc(list); });
			return list;			
		}
		#endregion
		#region MatchSettingsPanel
		public PXAction<CABankStatement> matchSettingsPanel;

		[PXUIField(DisplayName = Messages.MatchSettings, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXButton()]
		public virtual IEnumerable MatchSettingsPanel(PXAdapter adapter)
		{
			return adapter.Get();
		}
		#endregion

		#region ValidateMatches
		public PXAction<CABankStatement> validateMatches;
		[PXUIField(DisplayName = "Validate Matches", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
		[PXProcessButton()]
		public virtual IEnumerable ValidateMatches(PXAdapter adapter)
		{
			CABankStatement doc = this.BankStatement.Current;
			if( doc!= null && doc.Released == false)
			{
				PXCache cache = this.Details.Cache;
				PXCache matchesCache = this.DetailMatches.Cache;
				int problemsCount = 0;
				foreach (CABankStatementDetail iDet in this.Details.Select())
				{
					if (iDet.CATranID.HasValue) 
					{
						CATranExt tran = PXSelect<CATranExt, Where<CATranExt.tranID, Equal<Required<CATranExt.tranID>>>>.Select(this, iDet.CATranID);
						if (tran != null)
						{
							Decimal res = CompareRefNbr(iDet, tran, false);
							if (res < Decimal.One)
							{
								problemsCount++;
								cache.RaiseExceptionHandling<CABankStatementDetail.extRefNbr>(iDet, iDet.ExtRefNbr, new PXSetPropertyException( Messages.RowMatchesCATranWithDifferentExtRefNbr, PXErrorLevel.RowWarning));
								matchesCache.RaiseExceptionHandling<CATranExt.extRefNbr>(tran, iDet.ExtRefNbr, new PXSetPropertyException(Messages.RowMatchesCATranWithDifferentExtRefNbr, PXErrorLevel.RowWarning));
							}
							else 
							{
								cache.RaiseExceptionHandling<CABankStatementDetail.extRefNbr>(iDet, iDet.ExtRefNbr, null);
							}
						}
						else 
						{
							cache.RaiseExceptionHandling<CABankStatementDetail.documentMatched>(iDet, true, new PXSetPropertyException(Messages.MatchingCATransIsNotValid, PXErrorLevel.RowError));
						}
					}
				}
				if (problemsCount > 0)
					throw new PXException(Messages.RowsWithSuspiciousMatchingWereDetected, problemsCount);
			}
			return adapter.Get();
		}
		#endregion
		#endregion

		#region Ctor + Selects
		//public ToggleCurrency<CABankStatement> CurrencyView;

		public PXSelect<CABankStatement, Where<CABankStatement.cashAccountID, Equal<Optional<CABankStatement.cashAccountID>>>,OrderBy<Asc<CABankStatement.cashAccountID, Desc<CABankStatement.endBalanceDate>>>> BankStatement;

        public PXSelectJoin<CABankStatementDetail,
                        LeftJoin<CATran, On<CATran.cashAccountID, Equal<CABankStatementDetail.cashAccountID>,
                            And<CATran.tranID, Equal<CABankStatementDetail.cATranID>>>>,
                            Where<CABankStatementDetail.refNbr, Equal<Current<CABankStatement.refNbr>>>,
                                OrderBy<Asc<CABankStatementDetail.tranDate>>> PaymentData;

        [PXCopyPasteHiddenView]        
        public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>> CurrencyInfo_CuryInfoID;


        [PXCopyPasteHiddenView]
        public PXSelectJoin<CAApplicationStatementDetail,
                        LeftJoin<CATran, On<CATran.cashAccountID, Equal<CAApplicationStatementDetail.cashAccountID>,
                            And<CATran.tranID, Equal<CAApplicationStatementDetail.cATranID>>>>,
                            Where<CAApplicationStatementDetail.refNbr, Equal<Current<CABankStatement.refNbr>>,
                                And<CAApplicationStatementDetail.createDocument,Equal<True>>>,
                                OrderBy<Asc<CAApplicationStatementDetail.tranDate>>> ApplicationDetails;

        public PXSelect<CABankStatementAdjustment, Where<CABankStatementAdjustment.refNbr, Equal<Current<CABankStatement.refNbr>>,
            And<CABankStatementAdjustment.lineNbr,
            Equal<Current<CAApplicationStatementDetail.lineNbr>>>>> Adjustments;

		[PXFilterable]
		[PXImport(typeof(CABankStatement))]
		public PXSelectJoin<CABankStatementDetail, 
						LeftJoin<CATran,On<CATran.cashAccountID,Equal<CABankStatementDetail.cashAccountID>,
							And<CATran.tranID, Equal<CABankStatementDetail.cATranID>>>>,
							Where<CABankStatementDetail.refNbr,Equal<Current<CABankStatement.refNbr>>>,
								OrderBy<Asc<CABankStatementDetail.tranDate>>> Details;

        [PXCopyPasteHiddenView]
        [PXFilterable]
        public PXSelectJoin<CABankStatementDetail,
                        LeftJoin<CATran, On<CATran.cashAccountID, Equal<CABankStatementDetail.cashAccountID>,
                            And<CATran.tranID, Equal<CABankStatementDetail.cATranID>>>>,
                            Where<CABankStatementDetail.refNbr, Equal<Current<CABankStatement.refNbr>>>,
                                OrderBy<Asc<CABankStatementDetail.tranDate>>> DetailsForBalance;
 
		public PXSelectJoin<CATranExt, LeftJoin<BAccountR, On<CATranExt.referenceID, Equal<BAccountR.bAccountID>>>, 
					Where<CATranExt.tranID, Equal<Optional<CABankStatementDetail.cATranID>>>> DetailMatches;
		
		//public PXSelect<CurrencyInfo> currencyinfo;
        public PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Optional<CABankStatementDetail.curyInfoID>>>> currencyinfo;
		//public PXSetup<CashAccount, Where<CashAccount.reconcile, Equal<boolTrue>>> cashaccount;
        public PXSetup<CashAccount, Where<CashAccount.accountID, Equal<Optional<CABankStatement.cashAccountID>>>> cashaccount;
		public PXSetup<CASetup> casetup;
		public PXFilter<MatchSettings> matchSettings;

		public PXSelectReadonly<CashAccount, Where<CashAccount.extRefNbr, Equal<Optional<CashAccount.extRefNbr>>>> cashAccountByExtRef;
		public PXSelect<APPayment, Where<APPayment.docType,IsNull>> apPayment;
		public PXSelect<ARPayment, Where<ARPayment.docType, IsNull>> arPayment;
		public PXSelect<CADeposit, Where<CADeposit.tranType,IsNull>> caDeposit;
		public PXSelect<CAAdj, Where<CAAdj.adjRefNbr,IsNull>> caAdjustment;
		public PXSelect<CATransfer, Where<CATransfer.transferNbr, IsNull>> caTransfer;
		
		public CABankStatementEntry()
		{
			CASetup setup = casetup.Current; 

			PXCache cache = this.BankStatement.Cache;
			PXDBCurrencyAttribute.SetBaseCalc<CABankStatement.curyReconciledBalanceDiff>(cache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CABankStatement.curyReconciledCredits>(cache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CABankStatement.curyReconciledDebits>(cache, null, false);
			//PXDBCurrencyAttribute.SetBaseCalc<CABankStatement.curyReconciledTurnover>(reconCache, null, false);
			PXUIFieldAttribute.SetVisible<CABankStatement.curyID>(cache, null, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());

			PXCache detailCache = this.Details.Cache;
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.tranEntryDate>(detailCache, null, false);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.extTranID>(detailCache, null, false);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.curyID>(detailCache, null, false);
			bool showAmountAsDrCr = true;
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.drCr>(detailCache, null, !showAmountAsDrCr);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.curyTranAmt>(detailCache, null, !showAmountAsDrCr);

			PXUIFieldAttribute.SetVisible<CABankStatementDetail.curyDebitAmt>(detailCache, null, showAmountAsDrCr);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.curyCreditAmt>(detailCache, null, showAmountAsDrCr);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.payeeMatched>(detailCache, null, false);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.documentMatched>(detailCache, null, true);

			this.DetailMatches.Cache.AllowInsert = false;
			this.DetailMatches.Cache.AllowDelete = false;
			this.DetailMatches.Cache.AllowUpdate = false;

		}		
        #endregion


		#region Implement

		public virtual IEnumerable detailMatches()
		{
			CABankStatementDetail detail = this.Details.Current;
			if (detail == null) return null;
			List<CATranExt> res = new List<CATranExt>(1);
			if (detail.CATranID == null)
			{
				return this.FindDetailMatches(detail, this.matchSettings.Current, false);
			}
			else
			{
				PXResult<CATranExt,BAccountR> tranRes =(PXResult<CATranExt,BAccountR>) PXSelectReadonly2<CATranExt, LeftJoin<BAccountR, On<BAccountR.bAccountID,
														Equal<CATranExt.referenceID>>>, Where<CATranExt.tranID, Equal<Required<CATranExt.tranID>>>>.Select(this, detail.CATranID);
				CATranExt tran = tranRes;
				BAccount payee = tranRes;
				if (tran != null)
				{
					if (payee != null && payee.BAccountID != null)
						tran.ReferenceName = payee.AcctName;
					tran.MatchRelevance = EvaluateMatching(detail, tran, this.matchSettings.Current);
					tran.IsMatched = true;
					res.Add(tran);
				}
				else 
				{
					//this.Details.Cache.RaiseExceptionHandling<CABankStatementDetail.extRefNbr>(detail,detail.ExtRefNbr, new PXSetPropertyException("This line is matched to deleted Document",PXErrorLevel.Warning));
				}
			}
			return res;
		}
	
		#endregion

		#region Functions
		public virtual void DoMatch()
		{
			PXCache cache = this.Details.Cache;
			DoMatch(cache, this.Details.Select().RowCast<CABankStatementDetail>());
		} 

		public virtual void DoMatch(PXCache aUpdateCache, IEnumerable aRows)
		{
			Dictionary<long, List<CABankStmtDetailDocRef>> cross = new Dictionary<long, List<CABankStmtDetailDocRef>>();
			Dictionary<int, CABankStatementDetail> rows = new Dictionary<int, CABankStatementDetail>();
			int rowCount= 0;
			IMatchSettings setting = this.matchSettings.Current;
			const int daysTreshold = 30;
			bool useInternalCache = false;
			DateTime? minDate = null;
			DateTime? maxDate = null;

			if (((setting.DisbursementTranDaysAfter ?? 0) + (setting.DisbursementTranDaysBefore ?? 0)) > daysTreshold
				|| ((setting.ReceiptTranDaysAfter ?? 0) + (setting.ReceiptTranDaysBefore ?? 0)) > daysTreshold)
			{
				useInternalCache = true;
				foreach (object en in aRows)
				{					
					CABankStatementDetail iDet = en as CABankStatementDetail;
					if (setting.MatchInSelection == true && iDet.Selected != true) continue; 
					if (!minDate.HasValue || minDate.Value > iDet.TranDate.Value)
						minDate = iDet.TranDate;
					if (!maxDate.HasValue || maxDate.Value < iDet.TranDate.Value)
						maxDate = iDet.TranDate;
				}
			}
			foreach (object en in aRows) 
			{
				rowCount++;
				CABankStatementDetail iDet = en as CABankStatementDetail;
				if (iDet.CATranID.HasValue) continue; //Skip already matched records
				if (setting.MatchInSelection == true && iDet.Selected != true) continue; 
				if (!rows.ContainsKey(iDet.LineNbr.Value)) 
				{
					rows.Add(iDet.LineNbr.Value, iDet);
				}
				List<CATranExt> list= null;
				if(useInternalCache)
					list = (List<CATranExt>)this.FindDetailMatches(iDet, this.matchSettings.Current, minDate.Value, maxDate.Value);
				else
					list = (List<CATranExt>) this.FindDetailMatches(iDet,this.matchSettings.Current, true);
				int count = 0;
				CATranExt[] bestMatches = { null, null };				
				foreach (CATranExt iTran in list) 
				{
					if (bestMatches[0] == null || bestMatches[0].MatchRelevance < iTran.MatchRelevance)
					{						 
						bestMatches[1] = bestMatches[0];
						bestMatches[0] = iTran;
					}
					else 
					{
						if (bestMatches[1] == null || bestMatches[1].MatchRelevance < iTran.MatchRelevance)
							bestMatches[1] = iTran;
					}
					count++;
 				}
				if (bestMatches[0] != null 
					&& (( bestMatches[1] == null || (bestMatches[0].MatchRelevance - bestMatches[1].MatchRelevance) > 0.2m)|| bestMatches[0].MatchRelevance >0.75m)) 
				{
					CATranExt matchCandidate = bestMatches[0];
					CABankStmtDetailDocRef xref = new CABankStmtDetailDocRef();
					xref.Copy(iDet);
					xref.Copy(matchCandidate);
					xref.MatchRelevance = matchCandidate.MatchRelevance;
					if (cross.ContainsKey(matchCandidate.TranID.Value) == false)
					{
						cross.Add(matchCandidate.TranID.Value, new List<CABankStmtDetailDocRef>()); 
					}
					cross[matchCandidate.TranID.Value].Add(xref);					
				}
			}

			Dictionary<Int32, CABankStatementDetail> spareDetails = new Dictionary<Int32, CABankStatementDetail>();
			int updateCount = 0;
			int totalCount = 0;
			foreach(KeyValuePair<long,List<CABankStmtDetailDocRef>> iCandidate in cross)
			{ 
				CABankStmtDetailDocRef bestMatch = null;
				updateCount++;
				foreach(CABankStmtDetailDocRef iRef in iCandidate.Value)
				{
					totalCount++;
					if (bestMatch == null || bestMatch.MatchRelevance < iRef.MatchRelevance) 
					{
						bestMatch = iRef;
					} 
				}
				if (bestMatch != null && bestMatch.LineNbr!= null)  
				{
					CABankStatementDetail detail;
					if (!rows.TryGetValue(bestMatch.LineNbr.Value, out detail))
					{
						detail = this.Details.Search<CABankStatementDetail.lineNbr>(bestMatch.LineNbr.Value);
						rows.Add(detail.LineNbr.Value, detail);
					}
					 
					if (detail != null && detail.CATranID == null) 
					{
						detail.CATranID = bestMatch.CATranID;
						detail.DocumentMatched = true;
						aUpdateCache.Update(detail);
						//Clear CA transaction from the storage dictionary - it's not available anymore.
						if (this.caTransDict != null) 
						{
							List<CATranExt> storage;
							if (this.caTransDict.TryGetValue(detail.CuryTranAmt.Value, out storage) && storage!=null)
							{
								storage.RemoveAll(i=>(i.TranID == bestMatch.CATranID));
							}
						}
					}
					spareDetails.Remove(bestMatch.LineNbr.Value);
					foreach (CABankStmtDetailDocRef iMatch in iCandidate.Value) 
					{
						if (Object.ReferenceEquals(iMatch, bestMatch)) continue;
						spareDetails[iMatch.LineNbr.Value] = null;
					}
				}				
			}
			cross.Clear();
            List<CABankStatementDetail> spareDetails1 = new List<CABankStatementDetail>(spareDetails.Keys.Count);            
			foreach (KeyValuePair<Int32,CABankStatementDetail> iDet in spareDetails) 
			{
				CABankStatementDetail detail;
				if (!rows.TryGetValue(iDet.Key, out detail))
				{
					detail = this.Details.Search<CABankStatementDetail.lineNbr>(iDet.Key);
					rows.Add(detail.LineNbr.Value, detail);
				}
                //CABankStatementDetail detail = this.Details.Search<CABankStatementDetail.lineNbr>(iDet.Key);
                if(detail!=null)
                    spareDetails1.Add(detail);
			}
			if(spareDetails1.Count >0)
				DoMatch(aUpdateCache, spareDetails1);
		}

		public virtual IEnumerable FindDetailMatches(CABankStatementDetail aDetail, IMatchSettings aSettings, bool skipLowRelevance)
		{
			return StatementMatching.FindDetailMatches(this, Details.Cache, aDetail, aSettings, skipLowRelevance);
				}

//	    public static IEnumerable FindDetailMatches(CABankStatementDetail aDetail, IMatchSettings aSettings, DateTime minDate, DateTime maxDate)
//	    {
//		    
//	    }

		public virtual IEnumerable FindDetailMatches(CABankStatementDetail aDetail, IMatchSettings aSettings, DateTime minDate, DateTime maxDate)
		{
            List<CATranExt> matchList = new List<CATranExt>();
			bool hasBaccount = aDetail.PayeeBAccountID.HasValue;
			bool hasLocation = aDetail.PayeeLocationID.HasValue;			
            if (!aDetail.TranEntryDate.HasValue && !aDetail.TranDate.HasValue) return matchList;			
			Pair<DateTime, DateTime> tranDateRange = StatementMatching.GetDateRangeForMatch(aDetail, aSettings);			
			const decimal relevanceTreshhold = 0.2m;
			if(this.caTransDict == null)
				this.LoadTranDictionary(aSettings, minDate, maxDate);

			List<CATranExt> storage;
			if (!this.caTransDict.TryGetValue(aDetail.CuryTranAmt.Value, out storage))
			{
				return matchList;
			}
			
			string CuryID = aDetail.CuryID; //Need to reconsider.			
			foreach (CATranExt iRes in storage)
			{
				CATranExt iTran = iRes;				
				if (hasBaccount && iTran.ReferenceID != aDetail.PayeeBAccountID) continue;
				if (iTran.TranDate < tranDateRange.first || iTran.TranDate > tranDateRange.second) continue;
				iTran.MatchRelevance = EvaluateMatching(aDetail, iTran, aSettings);
				iTran.IsMatched = (iTran.TranID == aDetail.CATranID);
				if ((iTran.IsMatched == false && iTran.MatchRelevance < relevanceTreshhold))
					continue;				
				matchList.Add(iTran);
			}
			return matchList;
		}

		protected Dictionary<Decimal,List<CATranExt>> caTransDict = null;

		protected virtual void LoadTranDictionary(IMatchSettings aSettings, DateTime minDate, DateTime maxDate) 
		{
			CABankStatement stmt = this.BankStatement.Current;
			if (stmt == null) return;
			this.caTransDict = new Dictionary<decimal, List<CATranExt>>();
			DateTime disbDateFrom = minDate.AddDays(-(aSettings.DisbursementTranDaysBefore ?? 0));
			DateTime disbDateto = maxDate.AddDays((aSettings.DisbursementTranDaysAfter ?? 0));
			DateTime receiptDateFrom = minDate.AddDays(-(aSettings.ReceiptTranDaysBefore ?? 0));
			DateTime receiptDateto = maxDate.AddDays((aSettings.ReceiptTranDaysAfter ?? 0));

			foreach (PXResult<CATranExt, BAccountR, CABankStatementDetail> iRes in PXSelectReadonly2<CATranExt, LeftJoin<BAccountR, On<BAccountR.bAccountID,
														Equal<CATranExt.referenceID>>,
														LeftJoin<CABankStatementDetail, On<CABankStatementDetail.cATranID,Equal<CATran.tranID>>>>,
														Where<CATranExt.cashAccountID, Equal<Current<CABankStatement.cashAccountID>>,
															And<CABankStatementDetail.refNbr,IsNull,
															And<CATranExt.tranDate, Between<Required<CATran.tranDate>, Required<CATran.tranDate>>,
															And<CATranExt.drCr, Equal<Required<CATran.drCr>>>>>>>.
															Select(this, disbDateFrom, disbDateto, CADrCr.CACredit))
			{
				CATranExt iTran = iRes;
				BAccountR iBaccount = iRes;
				List<CATranExt> storage;
				if (!this.caTransDict.TryGetValue(iTran.CuryTranAmt.Value, out storage)) 
				{
					storage = new List<CATranExt>();
					this.caTransDict.Add(iTran.CuryTranAmt.Value, storage);
				}
				CABankStatementDetail sourceRow= null;
				//Need additional search for rows updated in the current graph
				foreach (CABankStatementDetail iDetail in this.Details.Cache.Updated)
				{
					if (iDetail.CATranID == iTran.TranID)
					{
						sourceRow = iDetail;
						break;
					}
				}
				if (sourceRow != null) continue;
				iTran.ReferenceName = iBaccount.AcctName;
				storage.Add(iTran);
			}

			foreach (PXResult<CATranExt, BAccountR, CABankStatementDetail> iRes in PXSelectReadonly2<CATranExt, LeftJoin<BAccountR, On<BAccountR.bAccountID,
														Equal<CATranExt.referenceID>>,
														LeftJoin<CABankStatementDetail, On<CABankStatementDetail.cATranID, Equal<CATran.tranID>>>>,
														Where<CATranExt.cashAccountID, Equal<Current<CABankStatement.cashAccountID>>,
															And<CABankStatementDetail.refNbr, IsNull,
															And<CATranExt.tranDate, Between<Required<CATran.tranDate>, Required<CATran.tranDate>>,
															And<CATranExt.drCr, Equal<Required<CATran.drCr>>>>>>>.
															Select(this, receiptDateFrom, receiptDateto, CADrCr.CADebit))
			{
				CATranExt iTran = iRes;
				BAccountR iBaccount = iRes;
				List<CATranExt> storage;
				if (!this.caTransDict.TryGetValue(iTran.CuryTranAmt.Value, out storage))
				{
					storage = new List<CATranExt>();
					this.caTransDict.Add(iTran.CuryTranAmt.Value, storage);
				}
				//Need additional search for rows updated in the current graph
				CABankStatementDetail sourceRow = null;
				foreach (CABankStatementDetail iDetail in this.Details.Cache.Updated)
				{
					if (iDetail.CATranID == iTran.TranID)
					{
						sourceRow = iDetail;
						break;
					}
				}
				if (sourceRow != null) continue;
				iTran.ReferenceName = iBaccount.AcctName;
				storage.Add(iTran);
			}
		}

	    public virtual decimal EvaluateMatching(CABankStatementDetail aDetail, CATran aTran, IMatchSettings aSettings)
					{
			return StatementMatching.EvaluateMatching(this, aDetail, aTran, aSettings);
		}

		public virtual decimal CompareDate(CABankStatementDetail aDetail, CATran aTran, Double meanValue, Double sigma)
		{
			return StatementMatching.CompareDate(aDetail, aTran, meanValue, sigma);
		}

		public virtual decimal CompareRefNbr(CABankStatementDetail aDetail, CATran aTran, bool looseCompare)
			{
			return StatementMatching.CompareRefNbr(this, aDetail, aTran, looseCompare);
			}

		public virtual decimal ComparePayee(CABankStatementDetail aDetail, CATran aTran)
		{
			return StatementMatching.ComparePayee(this, aDetail, aTran);
		}

		//Returns Degree of matching between two strings - as a percentage of matching characters 
		//in the correstponding positions of the aStr1 and aStr2
		//Return value - decimal between 0 and 1.
		public virtual decimal EvaluateMatching(string aStr1, string aStr2, bool aCaseSensitive) 
		{
			return StatementMatching.EvaluateMatching(aStr1, aStr2, aCaseSensitive);
		}

		public virtual decimal EvaluateTideMatching(string aStr1, string aStr2, bool aCaseSensitive)
		{
			return StatementMatching.EvaluateTideMatching(aStr1, aStr2, aCaseSensitive);
			}

		public static void SetDocTypeList(PXCache cache, CABankStatementDetail Row)
				{
            CABankStatementDetail detail = Row;
			
            List<string> AllowedValues = new List<string>();
            List<string> AllowedLabels = new List<string>();

            if (detail.OrigModule == GL.BatchModule.AP)
            {
                if (detail.DocType == APDocType.Refund)
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, APDocType.DebitAdj);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null, new string[] { APDocType.DebitAdj, APDocType.Prepayment }, new string[] { AP.Messages.DebitAdj, AP.Messages.Prepayment });
                }
                else if (detail.DocType == APDocType.Prepayment)
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, APDocType.Invoice);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null, new string[] { APDocType.Invoice, APDocType.CreditAdj }, new string[] { AP.Messages.Invoice, AP.Messages.CreditAdj });
                }
                else if (detail.DocType == APDocType.Check)
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, APDocType.Invoice);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null, new string[] { APDocType.Invoice, APDocType.DebitAdj, APDocType.CreditAdj, APDocType.Prepayment }, new string[] { AP.Messages.Invoice, AP.Messages.DebitAdj, AP.Messages.CreditAdj, AP.Messages.Prepayment });
                }
                else
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, APDocType.Invoice);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null, new string[] { APDocType.Invoice, APDocType.CreditAdj, APDocType.Prepayment }, new string[] { AP.Messages.Invoice, AP.Messages.CreditAdj, AP.Messages.Prepayment });
                }
            }
            else if (detail.OrigModule == GL.BatchModule.AR)
            {

                if (detail.DocType == ARDocType.Refund)
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, ARDocType.CreditMemo);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null, new string[] { ARDocType.CreditMemo, ARDocType.Payment, ARDocType.Prepayment }, new string[] { AR.Messages.CreditMemo, AR.Messages.Payment, AR.Messages.Prepayment });
                }
                else if (detail.DocType == ARDocType.Payment || detail.DocType == ARDocType.VoidPayment)
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, ARDocType.Invoice);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null,
                        new string[] { ARDocType.Invoice, ARDocType.DebitMemo, ARDocType.CreditMemo, ARDocType.FinCharge, ARDocType.SmallCreditWO },
                        new string[] { AR.Messages.Invoice, AR.Messages.DebitMemo, AR.Messages.CreditMemo, AR.Messages.FinCharge, AR.Messages.SmallCreditWO });
                }
                else
                {
					PXDefaultAttribute.SetDefault<CABankStatementAdjustment.adjdDocType>(cache, ARDocType.Invoice);
					PXStringListAttribute.SetList<CABankStatementAdjustment.adjdDocType>(cache, null,
                        new string[] { ARDocType.Invoice, ARDocType.DebitMemo, ARDocType.FinCharge, ARDocType.SmallCreditWO },
                        new string[] { AR.Messages.Invoice, AR.Messages.DebitMemo, AR.Messages.FinCharge, AR.Messages.SmallCreditWO });
                }
            }
        }

        public virtual void RecalcApplAmounts(PXCache sender, CAApplicationStatementDetail row)
        {
            using (new PXConnectionScope())
            {
                internalCall = true;
                try
                {
                    PXFormulaAttribute.CalcAggregate<CABankStatementAdjustment.curyAdjgAmt>(Adjustments.Cache, row, true);                    
                }
                finally
                {
                    internalCall = false;
                }
                sender.RaiseFieldUpdated<CAApplicationStatementDetail.curyApplAmt>(row, null);
                PXDBCurrencyAttribute.CalcBaseValues<CAApplicationStatementDetail.curyApplAmt>(sender, row);
                PXDBCurrencyAttribute.CalcBaseValues<CAApplicationStatementDetail.curyUnappliedBal>(sender, row);
            }
        }



		#endregion

		#region Import-Related functions
		public virtual void ImportStatement(PX.SM.FileInfo aFileInfo)
		{
			bool isFormatRecognized = false;
			IStatementReader reader = this.CreateReader();			
			if (reader !=null && reader.IsValidInput(aFileInfo.BinData))
			{				
				reader.Read(aFileInfo.BinData);
				List<CABankStatement> imported;
				reader.ExportTo(aFileInfo, this, out imported);
				if (imported != null)
				{
					CABankStatement last = (imported != null && imported.Count > 0) ? imported[imported.Count-1] : null; 
					if (this.BankStatement.Current == null || (last != null 
						&& (this.BankStatement.Current.CashAccountID!= last.CashAccountID || this.BankStatement.Current.RefNbr != last.RefNbr)))
					{
						this.BankStatement.Current = this.BankStatement.Search<CABankStatement.cashAccountID, CABankStatement.refNbr>(last.CashAccountID, last.RefNbr);
						throw new PXRedirectRequiredException(this, "Navigate to the uploaded record");
					}
				}
				isFormatRecognized = true;
			}
			if (!isFormatRecognized)
			{
				throw new PXException(Messages.UploadFileHasUnrecognizedBankStatementFormat);
			}
		}
		protected virtual IStatementReader CreateReader() 
		{
			IStatementReader processor = null;
			bool importToSingleAccount = this.casetup.Current.ImportToSingleAccount??false;
			string typeName = this.casetup.Current.StatementImportTypeName;
			if (importToSingleAccount)
			{
				CashAccount acct = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Optional<CABankStatement.cashAccountID>>>>.Select(this);
				if (acct != null)
					typeName = acct.StatementImportTypeName;
			}			

			if (string.IsNullOrEmpty(typeName)) 
				return processor;
			try
			{
				Type processorType = System.Web.Compilation.PXBuildManager.GetType(typeName, true);
				processor = (IStatementReader)Activator.CreateInstance(processorType);
			}
			catch (Exception e)
			{
				throw new PXException(e, Messages.StatementServiceReaderCreationError, typeName);
			}
			return processor;
		}

        public bool IsAlreadyImported(int? aCashAccountID, string aExtTranID, out string aRefNbr)
        {
            aRefNbr = null;          
            CABankStatementDetail detail = PXSelectReadonly<CABankStatementDetail,
                                               Where<CABankStatementDetail.cashAccountID, Equal<Required<CABankStatementDetail.cashAccountID>>,
                                                And<CABankStatementDetail.extTranID, Equal<Required<CABankStatementDetail.extTranID>>>>>.Select(this, aCashAccountID, aExtTranID);
            if (detail != null)
                aRefNbr = detail.RefNbr;
            return (detail != null);
        }

		#endregion

		#region Events
		#region CABankStatement Events

		protected virtual void CABankStatement_EndBalanceDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CABankStatement row = (CABankStatement)e.Row;
			if (row.Hold != false && row.StatementDate.HasValue)
				e.NewValue = row.StatementDate;
		}

		protected virtual void CABankStatement_CuryEndBalance_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CABankStatement row = (CABankStatement)e.Row;
			if (row != null && row.Hold != false)
			{
				e.NewValue = (row.CuryBegBalance ?? Decimal.Zero);
			}
		}

		protected virtual void CABankStatement_CashAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CABankStatement header = (CABankStatement)e.Row;
			sender.SetDefaultExt<CABankStatement.curyID>(e.Row);
			sender.SetDefaultExt<CABankStatement.startBalanceDate>(e.Row);
			sender.SetDefaultExt<CABankStatement.endBalanceDate>(e.Row);
			sender.SetDefaultExt<CABankStatement.curyBegBalance>(e.Row);
			sender.SetDefaultExt<CABankStatement.curyEndBalance>(e.Row);
		}

		protected virtual void CABankStatement_StatementDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CABankStatement row = (CABankStatement)e.Row;
			if (row != null && row.Hold == true)
			{
				sender.SetDefaultExt<CABankStatement.startBalanceDate>(e.Row);
				sender.SetDefaultExt<CABankStatement.endBalanceDate>(e.Row);
				sender.SetDefaultExt<CABankStatement.curyBegBalance>(e.Row);
				sender.SetDefaultExt<CABankStatement.curyEndBalance>(e.Row);
			}
		}

		protected virtual void CABankStatement_Hold_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			CABankStatement row = (CABankStatement)e.Row;
			if ((bool)e.NewValue != true)
			{
				bool hasError = false;
				if (row.CuryDetailsBalanceDiff != null && (Decimal)row.CuryDetailsBalanceDiff != 0)
				{
				    sender.RaiseExceptionHandling<CABankStatement.curyEndBalance>(row, row.CuryEndBalance, new PXSetPropertyException(Messages.StatementIsOutOfBalanceEndBalanceDoesNotMatchDetailsTotal));
					hasError = true;
				}
				
				if (row.CuryCreditsTotal != row.CuryReconciledCredits || row.CountCredit != row.ReconciledCountCredit)
				{
					sender.RaiseExceptionHandling<CABankStatement.curyReconciledCredits>(row, row.CuryReconciledCredits, new PXSetPropertyException(Messages.StatementIsOutOfBalanceThereAreUnmatchedDetails));
					hasError = true;					
				}

				if (row.CuryDebitsTotal != row.CuryReconciledDebits || row.CountDebit != row.ReconciledCountDebit)
				{
					sender.RaiseExceptionHandling<CABankStatement.curyReconciledDebits>(row, row.CuryReconciledDebits, new PXSetPropertyException(Messages.StatementIsOutOfBalanceThereAreUnmatchedDetails));
					hasError = true;
				}

				if (!row.StartBalanceDate.HasValue)
				{
					sender.RaiseExceptionHandling<CABankStatement.startBalanceDate>(row, row.StartBalanceDate, new PXSetPropertyException(Messages.StatementStartBalanceDateIsRequired));
					hasError = true;
				}

				if (!row.EndBalanceDate.HasValue)
				{
					sender.RaiseExceptionHandling<CABankStatement.endBalanceDate>(row, row.EndBalanceDate, new PXSetPropertyException(Messages.StatementEndBalanceDateIsRequired));
					hasError=true;
				}

				if (row.EndBalanceDate < row.StartBalanceDate) 
				{
					sender.RaiseExceptionHandling<CABankStatement.endBalanceDate>(row, row.EndBalanceDate, new PXSetPropertyException(Messages.StatementEndDateMustBeGreaterThenStartDate));
					hasError = true;
				}
				if(hasError)
				{
					e.NewValue = row.Hold;
					e.Cancel = true;
				}
				
			}
		}

		protected virtual void CABankStatement_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;
			bool importToSingleAccount = (this.casetup.Current.ImportToSingleAccount ?? false); 
            CABankStatement row = (CABankStatement)e.Row;
			bool isReleased = (row.Released == true);
			bool isHold = (row.Hold == true);
			bool isBalanced = !isHold && !isReleased;
			bool hasKeys = row.CashAccountID.HasValue;
			bool showSelection = false;
			
			if ((bool)row.Released)
			{
				PXUIFieldAttribute.SetEnabled(sender, null, false);				
				PXUIFieldAttribute.SetEnabled<CABankStatement.cashAccountID>(sender, row, true);
				PXUIFieldAttribute.SetEnabled<CABankStatement.refNbr>(sender, row, true);
				PXUIFieldAttribute.SetVisible<CABankStatement.hold>(sender, row, false);
				PXUIFieldAttribute.SetVisible<CABankStatement.released>(sender, row, true);
				sender.AllowDelete = false;
				sender.AllowUpdate = true;
				this.Details.Cache.AllowDelete = false;
				this.Details.Cache.AllowUpdate = false;
				this.Details.Cache.AllowInsert = false;
				this.matchSettingsPanel.SetEnabled(false);
			}
			else
			{
				showSelection = (this.matchSettings.Current.MatchInSelection == true);
				sender.AllowDelete = hasKeys;
				sender.AllowUpdate = true;
				this.Details.Cache.AllowDelete = isHold && hasKeys;
				this.Details.Cache.AllowUpdate = isHold && hasKeys;
				this.Details.Cache.AllowInsert = isHold && hasKeys;				
				if (hasKeys)
				{
					PXUIFieldAttribute.SetEnabled(sender, null, true);
					PXUIFieldAttribute.SetEnabled<CABankStatement.startBalanceDate>(sender, row, isHold);
					PXUIFieldAttribute.SetEnabled<CABankStatement.endBalanceDate>(sender, row, isHold);
					PXUIFieldAttribute.SetEnabled<CABankStatement.statementDate>(sender, row, isHold);
					PXUIFieldAttribute.SetEnabled<CABankStatement.curyBegBalance>(sender, row, isHold);
					PXUIFieldAttribute.SetEnabled<CABankStatement.curyEndBalance>(sender, row, isHold);
					PXUIFieldAttribute.SetVisible<CABankStatement.hold>(sender, row, true);
					PXUIFieldAttribute.SetVisible<CABankStatement.released>(sender, row, false);
				}
				else 
				{
					PXUIFieldAttribute.SetEnabled(sender, null, false);
					PXUIFieldAttribute.SetEnabled<CABankStatement.cashAccountID>(sender, row, true);
					PXUIFieldAttribute.SetEnabled<CABankStatement.refNbr>(sender, row, true);				
				}
			}

			PXUIFieldAttribute.SetVisible<MatchSettings.matchInSelection>(this.matchSettings.Cache, null, (this.casetup.Current.MatchInSelection == true));
			PXUIFieldAttribute.SetEnabled<CABankStatementDetail.selected>(this.Details.Cache, null, showSelection);
			PXUIFieldAttribute.SetVisible<CABankStatementDetail.selected>(this.Details.Cache, null, showSelection);
			if (importToSingleAccount)
			{
				PXEntryStatus status =  sender.GetStatus(e.Row);
				this.uploadFile.SetEnabled(isHold && hasKeys && status == PXEntryStatus.Inserted);
			}
			else 
			{
				//this.import.SetEnabled(isHold);
			}
			this.createAllDocuments.SetEnabled(isHold && hasKeys);
			this.createDocument.SetEnabled(isHold && hasKeys);
			this.matchTrans.SetEnabled(isHold && hasKeys);
			this.clearMatch.SetEnabled(isHold && hasKeys);
			this.autoMatch.SetEnabled(isHold && hasKeys);
			this.release.SetEnabled(isBalanced);
			this.validateMatches.SetEnabled((isReleased != true) && hasKeys);
			this.matchSettingsPanel.SetVisible(isHold && hasKeys);
			this.matchSettingsPanel.SetEnabled(isHold && hasKeys);
	
			PXUIFieldAttribute.SetEnabled<CABankStatement.status>(sender, row, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.curyID>(sender, row, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.countCredit>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.countDebit>(sender, null, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.reconciledCountCredit>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.reconciledCountDebit>(sender, null, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.curyCreditsTotal>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.curyDebitsTotal>(sender, null, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.curyReconciledDebits>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.curyReconciledCredits>(sender, null, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.curyDetailsEndBalance>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.curyDetailsBalanceDiff>(sender, null, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.curyReconciledEndBalance>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.curyReconciledBalanceDiff>(sender, null, false);

			PXUIFieldAttribute.SetEnabled<CABankStatement.bankStatementFormat>(sender, null, false);
			PXUIFieldAttribute.SetEnabled<CABankStatement.formatVerisionNbr>(sender, null, false);
			
			
			if (row.Released != true)
			{
				if (row.CuryDetailsBalanceDiff != Decimal.Zero)
				{
                    sender.RaiseExceptionHandling<CABankStatement.curyEndBalance>(row, row.CuryEndBalance, new PXSetPropertyException(Messages.StatementIsOutOfBalanceEndBalanceDoesNotMatchDetailsTotal, (bool)row.Hold ? PXErrorLevel.Warning: PXErrorLevel.Error));
                    sender.RaiseExceptionHandling<CABankStatement.curyDetailsBalanceDiff>(row, row.CuryDetailsBalanceDiff, new PXSetPropertyException(Messages.StatementIsOutOfBalanceEndBalanceDoesNotMatchDetailsTotal, (bool)row.Hold ? PXErrorLevel.Warning : PXErrorLevel.Error));
				}
				else
				{
					sender.RaiseExceptionHandling<CABankStatement.curyEndBalance>(row, row.CuryEndBalance, null);
					sender.RaiseExceptionHandling<CABankStatement.curyDetailsBalanceDiff>(row, row.CuryDetailsBalanceDiff, null);
				}
				PXDefaultAttribute.SetPersistingCheck<CABankStatement.startBalanceDate>(sender,row,isBalanced?PXPersistingCheck.NullOrBlank: PXPersistingCheck.Nothing);
				PXUIFieldAttribute.SetRequired<CABankStatement.startBalanceDate>(sender, isBalanced);
				PXDefaultAttribute.SetPersistingCheck<CABankStatement.endBalanceDate>(sender,row,isBalanced?PXPersistingCheck.NullOrBlank: PXPersistingCheck.Nothing);
				PXUIFieldAttribute.SetRequired<CABankStatement.endBalanceDate>(sender, isBalanced);				
			}

		}

		protected virtual void CABankStatement_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CABankStatement row = (CABankStatement)e.Row;
			if (row!= null &&  row.Released == false)
			{
				if (row.Hold != true )
				{
					if (row.StartBalanceDate > row.EndBalanceDate) 
					{
						sender.RaiseExceptionHandling<CABankStatement.endBalanceDate>(e.Row, row.EndBalanceDate, new PXSetPropertyException(Messages.StatementEndDateMustBeGreaterThenStartDate, PXErrorLevel.RowError));
					} 					
				}
			}
		}

		#endregion

		#region CABankStatementDetail Events
		protected virtual void CABankStatementDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			CABankStatement doc = this.BankStatement.Current;
			if (doc != null && row != null && doc.Released == false)
			{
				bool isNotMatched = (row.DocumentMatched == false);
                bool hasApplications = row.LineCntr.HasValue && row.LineCntr > 0;
				PXUIFieldAttribute.SetEnabled(sender, e.Row, (isNotMatched && hasApplications== false));
                if (isNotMatched && (hasApplications == false))
				{					
					bool createDocument = row.CreateDocument.Value;
					bool isAP = (row.OrigModule == GL.BatchModule.AP);
					bool isAR = (row.OrigModule == GL.BatchModule.AR);
					bool isCA = (row.OrigModule == GL.BatchModule.CA);
                    bool isPMInstanceRequired = false;
                    if (isAR && !String.IsNullOrEmpty(row.PaymentMethodID) ) 
                    {
                        PaymentMethod pm = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.Select(this, row.PaymentMethodID);
                        isPMInstanceRequired = (pm.IsAccountNumberRequired == true); 
                    }

					PXUIFieldAttribute.SetEnabled<CABankStatementDetail.origModule>(sender, e.Row, createDocument);
					PXUIFieldAttribute.SetEnabled<CABankStatementDetail.paymentMethodID>(sender, e.Row, createDocument && (isAP||isAR));
                    PXUIFieldAttribute.SetEnabled<CABankStatementDetail.pMInstanceID>(sender, e.Row, createDocument && isAR && isPMInstanceRequired);
					//PXUIFieldAttribute.SetEnabled<CABankStatementDetail.paymentMethodID>(sender, e.Row, createDocument && isAP);
					PXUIFieldAttribute.SetEnabled<CABankStatementDetail.payeeBAccountID>(sender, e.Row, createDocument && (isAP || isAR));
					PXUIFieldAttribute.SetEnabled<CABankStatementDetail.payeeLocationID>(sender, e.Row, createDocument && (isAP || isAR));
					PXUIFieldAttribute.SetEnabled<CABankStatementDetail.entryTypeID>(sender, e.Row, createDocument && isCA);
                    //PXDefaultAttribute.SetPersistingCheck<CABankStatementDetail.paymentMethodID>(sender,e.Row, createDocument && is
					//PXUIFieldAttribute.SetEnabled<CABankStatementDetail.finPeriodID>(sender, e.Row, createDocument);
				}
			}            
			PXUIFieldAttribute.SetEnabled<CABankStatementDetail.documentMatched>(sender, e.Row, false);
		}

		protected virtual void CABankStatementDetail_OrigModule_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			if (row!= null && row.CreateDocument ==true)
			{
				if (row.DrCr == CADrCr.CACredit)
					e.NewValue = GL.BatchModule.AP;
				else if (row.DrCr == CADrCr.CADebit)
					e.NewValue = GL.BatchModule.AR;
			}
			else
			{ 
				e.NewValue = null;
			}
			e.Cancel = true;
		}

		protected virtual void CABankStatementDetail_OrigModule_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			if (row != null)
			{
				sender.SetDefaultExt<CABankStatementDetail.payeeBAccountID>(e.Row);
				sender.SetDefaultExt<CABankStatementDetail.entryTypeID>(e.Row);				
			}
		}
				
		protected virtual void CABankStatementDetail_CreateDocument_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			if (row != null)
			{
				sender.SetDefaultExt<CABankStatementDetail.origModule>(e.Row);
			}
		}

		protected virtual void CABankStatementDetail_PayeeBAccountID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			if (row != null)
			{
				sender.SetDefaultExt<CABankStatementDetail.payeeLocationID>(e.Row);
				sender.SetDefaultExt<CABankStatementDetail.paymentMethodID>(e.Row);
				sender.SetDefaultExt<CABankStatementDetail.pMInstanceID>(e.Row);
			}
		}

		protected virtual void CABankStatementDetail_EntryTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			if (row.OrigModule == GL.BatchModule.CA)
			{
				string entryTypeID = this.casetup.Current.UnknownPaymentEntryTypeID;
				CAEntryType entryType =  PXSelect<CAEntryType,Where<CAEntryType.entryTypeId,Equal<Required<CAEntryType.entryTypeId>>>>.Select(this, entryTypeID);
				e.NewValue = (entryType != null && entryType.DrCr == row.DrCr)? entryTypeID : null;				
			}
			else
			{
				e.NewValue = null;
			}
			e.Cancel = true;
		}

        protected virtual void CABankStatementDetail_PaymentMethodID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CABankStatementDetail row = (CABankStatementDetail)e.Row;
            if (row != null)
            {
                sender.SetDefaultExt<CABankStatementDetail.pMInstanceID>(e.Row);
            }
        }

        protected virtual void CABankStatementDetail_TranDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CABankStatementDetail row = (CABankStatementDetail)e.Row;
            if (row != null)
            {
                if (row.CATranID.HasValue == false)
                {
                    CurrencyInfoAttribute.SetEffectiveDate<CABankStatementDetail.tranDate>(sender, e);                    
                }
            }
        }
	
		protected virtual void CABankStatementDetail_TranDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			CABankStatement doc = this.BankStatement.Current;
			if (row != null && doc != null) 
			{
				e.NewValue = doc.TranMaxDate.HasValue ? doc.TranMaxDate : doc.StartBalanceDate;
				e.Cancel = true;
			}
			
		}

		protected virtual void CABankStatementDetail_ExtTranID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{ 
			CABankStatementDetail row = (CABankStatementDetail) e.Row;
			if (row != null)
			{
				Dictionary<long, CAMessage> listMessages = PXLongOperation.GetCustomInfo(this.UID) as Dictionary<long, CAMessage>;
				TimeSpan timespan;
				Exception ex;
				PXLongRunStatus status = PXLongOperation.GetStatus(this.UID, out timespan, out ex);
				if ((status == PXLongRunStatus.Aborted || status == PXLongRunStatus.Completed)
							&& listMessages != null)
				{
					CAMessage message = null;
					if (listMessages.ContainsKey(row.CATranID.Value))
						message = listMessages[row.CATranID.Value];
					if (message != null)
					{
						string fieldName = typeof(CABankStatementDetail.extTranID).Name;

						e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
									null, null, message.Message, message.ErrorLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
						e.IsAltered = true;
					}
				}
			}
		}
        private bool _isCacheSync = false;

		protected virtual void CABankStatementDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			CABankStatementDetail row = (CABankStatementDetail)e.Row;
			CABankStatementDetail oldRow = (CABankStatementDetail)e.OldRow;
			if (row != null && row.DrCr != oldRow.DrCr)
			{
				if(row.DocumentMatched == false && row.CreateDocument == true && row.OrigModule == GL.BatchModule.CA)
					sender.SetDefaultExt<CABankStatementDetail.entryTypeID>(e.Row);
			}
            if (this._isCacheSync == false)
            {
                CAApplicationStatementDetail detailRow = (CAApplicationStatementDetail)this.ApplicationDetails.Search<CAApplicationStatementDetail.refNbr, CAApplicationStatementDetail.lineNbr>(row.RefNbr, row.LineNbr);
                if (detailRow != null)
                {
                    if (row.CreateDocument == true)
                    {
                        sender.RestoreCopy(detailRow, row);
                        this.ApplicationDetails.Update(detailRow);
                    }
                    else
                    {
                        this.ApplicationDetails.Delete(detailRow);
                    }
                }
                else 
                {
                    if (row.CreateDocument == true) 
                    {
                        CAApplicationStatementDetail detailRow1 = new CAApplicationStatementDetail();
                        sender.RestoreCopy(detailRow1, row);
                        this.ApplicationDetails.Insert(detailRow1);                        
                    }
                }

                if (row.TranDate != oldRow.TranDate) 
                {                    
                    CABankStatement parent = this.BankStatement.Current;
                    if (parent != null)
                    {
                        if (!parent.TranMaxDate.HasValue || parent.TranMaxDate < row.TranDate)
                        {
                            parent.TranMaxDate = row.TranDate;
                            this.BankStatement.Update(parent);                           
                        }
                        else if (oldRow.TranDate.HasValue && parent.TranMaxDate == oldRow.TranDate) 
                        {
                            CABankStatementDetail latest = PXSelect<CABankStatementDetail, Where<CABankStatementDetail.cashAccountID, Equal<Required<CABankStatementDetail.cashAccountID>>,
                                             And<CABankStatementDetail.refNbr, Equal<Required<CABankStatementDetail.refNbr>>>>, OrderBy<Desc<CABankStatementDetail.tranDate>>>.Select(this, parent.CashAccountID, parent.RefNbr);
                            parent.TranMaxDate = (latest != null ? latest.TranDate : null);
                            this.BankStatement.Update(parent);                       
                        }                        
                        
                    }
                }
            }
		}
        protected virtual void CABankStatementDetail_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            CABankStatementDetail row = (CABankStatementDetail)e.Row;
            if (row != null)
            {
                if (row.CATranID.HasValue == false)
                {
                    PXFieldUpdatedEventArgs args = new PXFieldUpdatedEventArgs(e.Row, false, false);
                    CurrencyInfoAttribute.SetEffectiveDate<CABankStatementDetail.tranDate>(sender, args);
                }
                if (row.CreateDocument == true)
                {
                    if (this._isCacheSync == false)
                    {
                        CAApplicationStatementDetail detailRow = new CAApplicationStatementDetail();
                        if (detailRow != null)
                        {
                            sender.RestoreCopy(detailRow, row);
                            this.ApplicationDetails.Insert(detailRow);
                        }
                    }
                }
                CABankStatement parent = this.BankStatement.Current;
                if (parent != null && row.TranDate.HasValue) 
                {
                    if (parent.TranMaxDate == null || parent.TranMaxDate < row.TranDate.Value) 
                    {
                        parent.TranMaxDate = row.TranDate;
                        this.BankStatement.Update(parent);
                    }
                } 
            }
        }
        protected virtual void CABankStatementDetail_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            CABankStatementDetail row = (CABankStatementDetail)e.Row;
            if (row != null)
            {
                if (this._isCacheSync == false)
                {
                    CAApplicationStatementDetail detailRow = (CAApplicationStatementDetail)this.ApplicationDetails.Search<CAApplicationStatementDetail.refNbr, CAApplicationStatementDetail.lineNbr>(row.RefNbr, row.LineNbr);
                    if (detailRow != null)
                    {
                        this.ApplicationDetails.Delete(detailRow);
                    }
                    if (row.CuryInfoID != null) 
                    {
                        CurrencyInfo info = this.currencyinfo.Select(row.CuryInfoID);
                        if (info != null)
                            this.currencyinfo.Delete(info);
                    }

                    CABankStatement parent = this.BankStatement.Current;
                    if (parent != null && BankStatement.Cache.GetStatus(parent) != PXEntryStatus.Deleted && parent.TranMaxDate.HasValue)
                    {
                        if(parent.TranMaxDate == row.TranDate.Value)
                        {
                            CABankStatementDetail latest = PXSelect<CABankStatementDetail, Where<CABankStatementDetail.cashAccountID, Equal<Required<CABankStatementDetail.cashAccountID>>,
                                And<CABankStatementDetail.refNbr, Equal<Required<CABankStatementDetail.refNbr>>>>, OrderBy<Desc<CABankStatementDetail.tranDate>>>.Select(this, parent.CashAccountID, parent.RefNbr);
                            parent.TranMaxDate = (latest !=null? latest.TranDate: null);
                            this.BankStatement.Update(parent);
                        }
                    }
                }
            }
        }    

		protected virtual void CADeposit_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			var row = e.Row as CADeposit;
			if (row == null)
				return;

			bool requireExtRefNbr = casetup.Current.RequireExtRefNbr == true;
			PXUIFieldAttribute.SetRequired<CADeposit.extRefNbr>(cache, requireExtRefNbr);
			PXDefaultAttribute.SetPersistingCheck<CADeposit.extRefNbr>(cache, row, requireExtRefNbr ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
		}

#if false
        protected virtual void CABankStatementDetail_PaymentMethodID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            CABankStatementDetail row = (CABankStatementDetail)e.Row;
            if (row != null && row.OrigModule == GL.BatchModule.AP && row.CashAccountID.HasValue)
            {
                PaymentMethod type = null;
                type = PXSelectJoin<PaymentMethod,
                                    InnerJoin<PaymentMethodAccount, On<PaymentMethodAccount.paymentMethodID, Equal<PaymentMethod.paymentMethodID>,
                                    And<PaymentMethodAccount.useForAP, Equal<True>>>>,
                                    Where<PaymentMethodAccount.cashAccountID, Equal<Required<CABankStatementDetail.cashAccountID>>>,
                                        OrderBy<Desc<PaymentMethodAccount.aPIsDefault>>>.Select(this, row.CashAccountID);

                e.NewValue = (type != null ? type.PaymentMethodID : null);
            }
            else
            {
                if (row != null && row.OrigModule == GL.BatchModule.AR && row.PMInstanceID.HasValue)
                {
                    CustomerPaymentMethod cpm = PXSelect<CustomerPaymentMethod, Where<CustomerPaymentMethod.pMInstanceID, Equal<Required<CABankStatementDetail.pMInstanceID>>>>.Select(this, row.PMInstanceID);
                    e.NewValue = cpm.PaymentMethodID;
                }
                else
                {
                    e.NewValue = null;
                }
            }
            e.Cancel = true;
        } 
#endif
		#endregion

        #region CABankStatementAdjustment Events

        protected virtual void CABankStatementAdjustment_AdjdRefNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            CABankStatementAdjustment row = (CABankStatementAdjustment)e.Row;
            CABankStatementDetail parent = PXSelect<CABankStatementDetail, Where<CABankStatementDetail.refNbr, Equal<Required<CABankStatementDetail.refNbr>>,
                                                And<CABankStatementDetail.lineNbr, Equal<Required<CABankStatementDetail.lineNbr>>>>>.Select(this, row.RefNbr, row.LineNbr);
            if (parent.DocumentMatched == true) 
            {
                e.Cancel = true;
            }
        }
        protected virtual void CABankStatementAdjustment_AdjdRefNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CABankStatementAdjustment adj = (CABankStatementAdjustment)e.Row;
	        CAApplicationStatementDetail currentDetail = PXSelect<CAApplicationStatementDetail,
		        Where<CAApplicationStatementDetail.refNbr, Equal<Required<CAApplicationStatementDetail.refNbr>>,
			        And<CAApplicationStatementDetail.lineNbr, Equal<Required<CAApplicationStatementDetail.lineNbr>>>>>.Select(this, adj.RefNbr, adj.LineNbr);
	        ApplicationDetails.Current = currentDetail;
			adj.AdjgCuryInfoID = ApplicationDetails.Current.CuryInfoID;
            adj.AdjgDocDate = ApplicationDetails.Current.TranDate;
            adj.Released = false;
            adj.CuryDocBal = null;
            adj.CuryDiscBal = null;
            adj.CuryWhTaxBal = null;
            adj.CuryAdjgAmt = null;

            if (ApplicationDetails.Current.OrigModule == GL.BatchModule.AP)
            {
	            StatementApplicationBalances.PopulateAdjustmentFieldsAP(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, adj);
                    sender.SetDefaultExt<CABankStatementAdjustment.adjdTranPeriodID>(e.Row);
            }
            else if (ApplicationDetails.Current.OrigModule == GL.BatchModule.AR)
            {
				StatementApplicationBalances.PopulateAdjustmentFieldsAR(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, adj);
                    sender.SetDefaultExt<CABankStatementAdjustment.adjdTranPeriodID>(e.Row);
                }
        }

        protected virtual void CABankStatementAdjustment_AdjdCuryRate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if ((decimal)e.NewValue <= 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_GT, ((int)0).ToString());
            }
        }

        protected virtual void CABankStatementAdjustment_AdjdCuryRate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            CABankStatementAdjustment adj = (CABankStatementAdjustment)e.Row;

            CurrencyInfo pay_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<CABankStatementAdjustment.adjgCuryInfoID>>>>.SelectSingleBound(this, new object[] { e.Row });
            CurrencyInfo vouch_info = PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<CABankStatementAdjustment.adjdCuryInfoID>>>>.SelectSingleBound(this, new object[] { e.Row });

            if (string.Equals(pay_info.CuryID, vouch_info.CuryID) && adj.AdjdCuryRate != 1m)
            {
                adj.AdjdCuryRate = 1m;
                vouch_info.SetCuryEffDate(currencyinfo.Cache, ApplicationDetails.Current.TranDate);
            }
            else if (string.Equals(vouch_info.CuryID, vouch_info.BaseCuryID))
            {
                adj.AdjdCuryRate = pay_info.CuryMultDiv == "M" ? 1 / pay_info.CuryRate : pay_info.CuryRate;
            }
            else
            {
                vouch_info.CuryRate = Math.Round((decimal)adj.AdjdCuryRate * (pay_info.CuryMultDiv == "M" ? (decimal)pay_info.CuryRate : 1m / (decimal)pay_info.CuryRate), 8, MidpointRounding.AwayFromZero);
                vouch_info.RecipRate = Math.Round((pay_info.CuryMultDiv == "M" ? 1m / (decimal)pay_info.CuryRate : (decimal)pay_info.CuryRate) / (decimal)adj.AdjdCuryRate, 8, MidpointRounding.AwayFromZero);
                vouch_info.CuryMultDiv = "M";
            }

            if (Caches[typeof(CurrencyInfo)].GetStatus(vouch_info) == PXEntryStatus.Notchanged)
            {
                Caches[typeof(CurrencyInfo)].SetStatus(vouch_info, PXEntryStatus.Updated);
            }
			StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, true);
        }

        protected virtual void CABankStatementAdjustment_CuryAdjgAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            CABankStatementAdjustment adj = (CABankStatementAdjustment)e.Row;

            if (adj.CuryDocBal == null || adj.CuryDiscBal == null || adj.CuryWhTaxBal == null) 
            {
				StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, false);
            }

            if (adj.CuryDocBal == null)
            {
                throw new PXSetPropertyException<CABankStatementAdjustment.adjdRefNbr>(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<CABankStatementAdjustment.adjdRefNbr>(sender));
            }

            if (adj.VoidAdjNbr == null && (decimal)e.NewValue < 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_GE, ((int)0).ToString());
            }

            if (adj.VoidAdjNbr != null && (decimal)e.NewValue > 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_LE, ((int)0).ToString());
            }

            if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgAmt - (decimal)e.NewValue < 0)
            {
                throw new PXSetPropertyException(AP.Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgAmt).ToString());
            }
        }

        protected virtual void CABankStatementAdjustment_CuryAdjgAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            if (e.OldValue != null && ((CABankStatementAdjustment)e.Row).CuryDocBal == 0m && ((CABankStatementAdjustment)e.Row).CuryAdjgAmt < (decimal)e.OldValue)
            {
                ((CABankStatementAdjustment)e.Row).CuryAdjgDiscAmt = 0m;
            }
			StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, true);
        }

        protected virtual void CABankStatementAdjustment_CuryAdjgDiscAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            CABankStatementAdjustment adj = (CABankStatementAdjustment)e.Row;

            if (adj.CuryDocBal == null || adj.CuryDiscBal == null || adj.CuryWhTaxBal == null)
            {
				StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, false);
            }

            if (adj.CuryDocBal == null || adj.CuryDiscBal == null)
            {
                throw new PXSetPropertyException<CABankStatementAdjustment.adjdRefNbr>(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<CABankStatementAdjustment.adjdRefNbr>(sender));
            }

            if (adj.VoidAdjNbr == null && (decimal)e.NewValue < 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_GE, ((int)0).ToString());
            }

            if (adj.VoidAdjNbr != null && (decimal)e.NewValue > 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_LE, ((int)0).ToString());
            }

            if ((decimal)adj.CuryDiscBal + (decimal)adj.CuryAdjgDiscAmt - (decimal)e.NewValue < 0)
            {
                throw new PXSetPropertyException(AP.Messages.Entry_LE, ((decimal)adj.CuryDiscBal + (decimal)adj.CuryAdjgDiscAmt).ToString());
            }

            if (adj.CuryAdjgAmt != null && (sender.GetValuePending<CABankStatementAdjustment.curyAdjgAmt>(e.Row) == PXCache.NotSetValue || (Decimal?)sender.GetValuePending<CABankStatementAdjustment.curyAdjgAmt>(e.Row) == adj.CuryAdjgAmt))
            {
                if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgDiscAmt - (decimal)e.NewValue < 0)
                {
                    throw new PXSetPropertyException(AP.Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgDiscAmt).ToString());
                }
            }
        }

        protected virtual void CABankStatementAdjustment_CuryAdjgDiscAmt_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
			StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, true);
        }

        protected virtual void CABankStatementAdjustment_CuryAdjgWhTaxAmt_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            CABankStatementAdjustment adj = (CABankStatementAdjustment)e.Row;

            if (adj.CuryDocBal == null || adj.CuryDiscBal == null || adj.CuryWhTaxBal == null)
            {
				StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, false);
            }

            if (adj.CuryDocBal == null || adj.CuryWhTaxBal == null)
            {
                throw new PXSetPropertyException<CABankStatementAdjustment.adjdRefNbr>(ErrorMessages.FieldIsEmpty, PXUIFieldAttribute.GetDisplayName<CABankStatementAdjustment.adjdRefNbr>(sender));
            }

            if (adj.VoidAdjNbr == null && (decimal)e.NewValue < 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_GE, ((int)0).ToString());
            }

            if (adj.VoidAdjNbr != null && (decimal)e.NewValue > 0m)
            {
                throw new PXSetPropertyException(CS.Messages.Entry_LE, ((int)0).ToString());
            }

            if ((decimal)adj.CuryWhTaxBal + (decimal)adj.CuryAdjgWhTaxAmt - (decimal)e.NewValue < 0)
            {
                throw new PXSetPropertyException(AP.Messages.Entry_LE, ((decimal)adj.CuryWhTaxBal + (decimal)adj.CuryAdjgWhTaxAmt).ToString());
            }

            if (adj.CuryAdjgAmt != null && (sender.GetValuePending<CABankStatementAdjustment.curyAdjgAmt>(e.Row) == PXCache.NotSetValue || (Decimal?)sender.GetValuePending<CABankStatementAdjustment.curyAdjgAmt>(e.Row) == adj.CuryAdjgAmt))
            {
                if ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgWhTaxAmt - (decimal)e.NewValue < 0)
                {
                    throw new PXSetPropertyException(AP.Messages.Entry_LE, ((decimal)adj.CuryDocBal + (decimal)adj.CuryAdjgWhTaxAmt).ToString());
                }
            }
        }
        private bool internalCall = false;

        protected virtual void CABankStatementAdjustment_CuryDocBal_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (!internalCall)
            {
                if (e.Row != null && ((CABankStatementAdjustment)e.Row).AdjdCuryInfoID != null && ((CABankStatementAdjustment)e.Row).CuryDocBal == null && sender.GetStatus(e.Row) != PXEntryStatus.Deleted)
                {
					StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, false);
                }
                if (e.Row != null)
                {
                    e.NewValue = ((CABankStatementAdjustment)e.Row).CuryDocBal;
                }                
            }
            e.Cancel = true;
        }

        protected virtual void CABankStatementAdjustment_CuryDiscBal_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (!internalCall)
            {
                if (e.Row != null && ((CABankStatementAdjustment)e.Row).AdjdCuryInfoID != null && ((CABankStatementAdjustment)e.Row).CuryDiscBal == null && sender.GetStatus(e.Row) != PXEntryStatus.Deleted)
                {
					StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, false);
                }
                if (e.Row != null)
                {
                    e.NewValue = ((CABankStatementAdjustment)e.Row).CuryDiscBal;
                }
            }
            e.Cancel = true;
        }

        protected virtual void CABankStatementAdjustment_CuryWhTaxBal_FieldUpdating(PXCache sender, PXFieldUpdatingEventArgs e)
        {
            if (!internalCall)
            {

                if (e.Row != null && ((CABankStatementAdjustment)e.Row).AdjdCuryInfoID != null && ((CABankStatementAdjustment)e.Row).CuryWhTaxBal == null && sender.GetStatus(e.Row) != PXEntryStatus.Deleted)
                {
					StatementApplicationBalances.UpdateBalance(this, CurrencyInfo_CuryInfoID, ApplicationDetails.Current, (CABankStatementAdjustment)e.Row, false);
                }
                if (e.Row != null)
                {
                    e.NewValue = ((CABankStatementAdjustment)e.Row).CuryWhTaxBal;
                }
            }

            e.Cancel = true;
        }

        protected virtual void CABankStatementAdjustment_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        {
            CABankStatementAdjustment row = (CABankStatementAdjustment)e.Row;
            if (row != null)
            { 
                foreach (CABankStatementAdjustment adjustmentRecord in this.Adjustments.Select())
                {
                    if (row.AdjdRefNbr != null && adjustmentRecord.AdjdRefNbr == row.AdjdRefNbr && adjustmentRecord.AdjdDocType == row.AdjdDocType)
                    {
                        PXEntryStatus status = this.Adjustments.Cache.GetStatus(adjustmentRecord);
                        if (!(status == PXEntryStatus.InsertedDeleted || status == PXEntryStatus.Deleted))
                        {
                            sender.RaiseExceptionHandling<CABankStatementAdjustment.adjdRefNbr>(e.Row, null, new PXException(Messages.DuplicatedKeyForRow));
                            e.Cancel = true;
                            break;
                        }
                    }
                }
            }
        }

        protected virtual void CABankStatementAdjustment_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            CABankStatementAdjustment row = (CABankStatementAdjustment)e.Row;
            //if (row != null)
            //{
            //    foreach (CABankStatementAdjustment adjustmentRecord in this.Adjustments.Select())
            //    {
            //        if (row.AdjdRefNbr != null && adjustmentRecord.AdjdRefNbr == row.AdjdRefNbr && adjustmentRecord.AdjdDocType == row.AdjdDocType)
            //        {
            //            PXEntryStatus status = this.Adjustments.Cache.GetStatus(adjustmentRecord);
            //            if (!(status == PXEntryStatus.InsertedDeleted || status == PXEntryStatus.Deleted))
            //            {
            //                sender.RaiseExceptionHandling<CABankStatementAdjustment.adjdRefNbr>(e.Row, null, new PXException(Messages.DuplicatedKeyForRow));
            //                e.Cancel = true;
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        #endregion

        #region CAApplicationStatementDetail Events
        protected virtual void CAApplicationStatementDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            CAApplicationStatementDetail row = (CAApplicationStatementDetail)e.Row;
			CABankStatement doc = this.BankStatement.Current;
            if (doc != null && row != null && doc.Released == false)  
            {
				SetDocTypeList(Adjustments.Cache, row);
                if (row.OrigModule == GL.BatchModule.AR)
                {
                    PXUIFieldAttribute.SetEnabled<CABankStatementAdjustment.adjdCuryRate>(Adjustments.Cache, null, false);
                }
                else
                {
                    PXUIFieldAttribute.SetEnabled<CABankStatementAdjustment.adjdCuryRate>(Adjustments.Cache, null, true);
                }
            }
            if (row != null)
            {
                if (row.CATranID.HasValue || !String.IsNullOrEmpty(row.InvoiceInfo))
                {
                    this.Adjustments.Cache.AllowInsert = false;
                    this.Adjustments.Cache.AllowUpdate = false;
                    this.Adjustments.Cache.AllowDelete = false;
                }
                else
                {
                    this.Adjustments.Cache.AllowInsert = true;
                    this.Adjustments.Cache.AllowUpdate = true;
                    this.Adjustments.Cache.AllowDelete = true;
                }
            }
            else
            {
                this.Adjustments.Cache.AllowInsert = false;
                this.Adjustments.Cache.AllowUpdate = false;
            }
        }
        protected virtual void CAApplicationStatementDetail_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {
			if (e.Row != null)
			{
				CAApplicationStatementDetail row = (CAApplicationStatementDetail)e.Row;
				if (row != null && row.CuryApplAmt == null)
				{
					RecalcApplAmounts(sender, row);
				}
			}
        }
        protected virtual void CAApplicationStatementDetail_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            //CAApplicationStatementDetail row = (CAApplicationStatementDetail)e.Row;
            //CAApplicationStatementDetail oldRow = (CAApplicationStatementDetail)e.OldRow;
            //if (row != null && row.LineCntr != oldRow.LineCntr)
            //{
            //    CABankStatementDetail src = (CABankStatementDetail)this.Details.Search<CABankStatementDetail.refNbr, CABankStatementDetail.lineNbr>(row.RefNbr, row.LineNbr);
            //    if (src != null)
            //    {
            //        CABankStatementDetail copy = (CABankStatementDetail)this.Details.Cache.CreateCopy(src);
            //        copy.LineCntr = row.LineCntr;
            //        try
            //        {
            //            this._isCacheSync = true;
            //            this.Details.Update(copy);
            //        }
            //        finally
            //        {
            //            this._isCacheSync = false;
            //        }
            //    }
            //}
        }
        protected virtual void CAApplicationStatementDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion

        #region CurrencyInfo
        protected virtual void CurrencyInfo_CuryID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if ((cashaccount.Current != null) && !string.IsNullOrEmpty(cashaccount.Current.CuryID))
				{
					e.NewValue = cashaccount.Current.CuryID;
					e.Cancel = true;
				}
			}
		}

        protected virtual void CurrencyInfo_CuryRateTypeID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				if (cashaccount.Current != null && !string.IsNullOrEmpty(cashaccount.Current.CuryRateTypeID))
				{
					e.NewValue = cashaccount.Current.CuryRateTypeID;
					e.Cancel = true;
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (this.ApplicationDetails.Current != null)
			{
                e.NewValue = this.ApplicationDetails.Current.TranDate;
				e.Cancel = true;
			}
		}

        protected virtual void CurrencyInfo_RowInserting(PXCache sender, PXRowInsertingEventArgs e) 
        {
            CurrencyInfo row = e.Row as CurrencyInfo;
        }

        protected virtual void CurrencyInfo_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            CurrencyInfo row = e.Row as CurrencyInfo;
        }
	
        #endregion
        #endregion

        #region Release-Related Functions
        public static void ReleaseDoc(List<CABankStatement> list)
		{
			foreach (CABankStatement iDoc in list)
			{
				CABankStatementEntry graph = PXGraph.CreateInstance<CABankStatementEntry>();
				CABankStatement row = graph.BankStatement.Search<CABankStatement.refNbr>(iDoc.RefNbr, iDoc.CashAccountID);
                CASetup casetup = graph.casetup.Current; 
				graph.BankStatement.Current = row;
				graph.DetailMatches.Cache.AllowUpdate = true;
				if (row.Hold == true)
					throw new PXException(Messages.DocumentOnHoldCanNotBeReleased);
                List<CATran> needsRelease = new List<CATran>(); 
				foreach (PXResult<CABankStatementDetail, CATran> iDet in PXSelectReadonly2<CABankStatementDetail, 
						    LeftJoin<CATran,On<CATran.cashAccountID,Equal<CABankStatementDetail.cashAccountID>,
							    And<CATran.tranID, Equal<CABankStatementDetail.cATranID>>>>,
							        Where<CABankStatementDetail.refNbr,Equal<Required<CABankStatement.refNbr>>>>.Select(graph, row.RefNbr))
				{
					CABankStatementDetail detail = iDet;
					CATran catran = iDet;
					if (detail.CATranID == null)
						throw new PXException(Messages.StatementCanNotBeReleasedThereAreUnmatchedDetails);

					if (catran == null || catran.TranID == null)
						throw new PXException(Messages.StatementCanNotBeReleasedSomeDetailsMatchedDeletedDocument);

                    if (catran.Released != true)
                    {
                         catran.Selected = true;
                         needsRelease.Add(catran);                        
                    }					
				}				
                if (needsRelease.Count > 0) 
                {
                    CATrxRelease.GroupReleaseTransaction(needsRelease, casetup.ReleaseAP.Value, casetup.ReleaseAR.Value, true);
                }
				
				foreach (PXResult<CABankStatementDetail, CATran> iDet in PXSelectReadonly2<CABankStatementDetail,
							InnerJoin<CATran, On<CATran.cashAccountID, Equal<CABankStatementDetail.cashAccountID>,
								And<CATran.tranID, Equal<CABankStatementDetail.cATranID>>>>,
									Where<CABankStatementDetail.refNbr, Equal<Required<CABankStatement.refNbr>>>>.Select(graph, row.RefNbr))
				{
                    CABankStatementDetail statementTran = iDet;
					CATran catran = iDet;					
					if (catran.Released == true && catran.Cleared == false)
					{
						graph.UpdateSourceDoc(catran, statementTran);
					}
				}
				CABankStatement copy = (CABankStatement)graph.BankStatement.Cache.CreateCopy(row);
				copy.Released = true;
				graph.BankStatement.Update(copy);
				graph.Save.Press();
			}
		}

		protected virtual void UpdateSourceDoc(CATran aTran, CABankStatementDetail statementTran)
		{
		    UpdateSourceDoc(this, aTran, statementTran.TranDate);
	    }

		public static void UpdateSourceDoc(PXGraph graph, CATran aTran, DateTime? clearDate)
		{
			aTran.Cleared = true;
			aTran.ClearDate = clearDate ?? aTran.TranDate;
			graph.Caches[typeof(CATran)].Update(aTran);

			switch (aTran.OrigModule) 
			{
				case GL.BatchModule.AP:
                    if (aTran.OrigTranType != CAAPARTranType.GLEntry)
					{
						APPayment payment = PXSelect<APPayment, Where<APPayment.docType, Equal<Required<APPayment.docType>>,
											And<APPayment.refNbr, Equal<Required<APPayment.refNbr>>>>>.Select(graph, aTran.OrigTranType, aTran.OrigRefNbr);
						payment.ClearDate = aTran.ClearDate;
						payment.Cleared = aTran.Cleared;
						graph.Caches[typeof(APPayment)].Update(payment);
					}
					break;
				case GL.BatchModule.AR:
                    if (aTran.OrigTranType != CAAPARTranType.GLEntry)
                    {
						ARPayment payment = PXSelect<ARPayment, Where<ARPayment.docType, Equal<Required<ARPayment.docType>>,
											And<ARPayment.refNbr, Equal<Required<ARPayment.refNbr>>>>>.Select(graph, aTran.OrigTranType, aTran.OrigRefNbr);
						payment.ClearDate = aTran.ClearDate;
						payment.Cleared = aTran.Cleared;
						graph.Caches[typeof(ARPayment)].Update(payment);
					}
					break;
				case GL.BatchModule.CA:
					{
						if (aTran.OrigTranType == CA.CATranType.CADeposit)
						{
							CATran notClearedTran = PXSelect<CATran, Where<CATran.origModule, Equal<CATran.origModule>,
																And<CATran.origTranType, Equal<CATran.origTranType>,
																And<CATran.origRefNbr, Equal<CATran.origRefNbr>,
																And<CATran.cleared, Equal<False>,
																And<CATran.tranID, NotEqual<CATran.tranID>>>>>>>.Select(graph, aTran.OrigModule, aTran.OrigTranType, aTran.OrigRefNbr, aTran.TranID);
							if(notClearedTran == null)
							{
								CADeposit payment = PXSelect<CADeposit, Where<CADeposit.tranType, Equal<Required<CADeposit.tranType>>,
											And<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>>>>.Select(graph, aTran.OrigTranType, aTran.OrigRefNbr);

								payment.ClearDate = aTran.ClearDate;
								payment.Cleared = aTran.Cleared;
								graph.Caches[typeof(CADeposit)].Update(payment);
							}
						}

						if (aTran.OrigTranType == CA.CATranType.CAAdjustment)
						{
							CATran notClearedTran = PXSelect<CATran, Where<CATran.origModule, Equal<CATran.origModule>,
																And<CATran.origTranType, Equal<CATran.origTranType>,
																And<CATran.origRefNbr, Equal<CATran.origRefNbr>,
																And<CATran.cleared, Equal<False>,
																And<CATran.tranID, NotEqual<CATran.tranID>>>>>>>.Select(graph, aTran.OrigModule, aTran.OrigTranType, aTran.OrigRefNbr, aTran.TranID);
							if (notClearedTran == null)
							{
								CAAdj payment = PXSelect<CAAdj, Where<CAAdj.adjTranType, Equal<Required<CAAdj.adjTranType>>,
												And<CAAdj.adjRefNbr, Equal<Required<CAAdj.adjRefNbr>>>>>.Select(graph, aTran.OrigTranType, aTran.OrigRefNbr);

								payment.ClearDate = aTran.ClearDate;
								payment.Cleared = aTran.Cleared;
								graph.Caches[typeof(CAAdj)].Update(payment);
							}
						}

						if (aTran.OrigTranType == CA.CATranType.CATransferIn || aTran.OrigTranType == CA.CATranType.CATransferOut)
						{
							CATransfer payment = PXSelect<CATransfer, Where<CATransfer.transferNbr, Equal<Required<CATransfer.transferNbr>>>>.Select(graph, aTran.OrigRefNbr);

							if (payment.TranIDIn == aTran.TranID)
							{
								payment.ClearDateIn = aTran.ClearDate;
								payment.ClearedIn = aTran.Cleared;
							}

							if (payment.TranIDOut == aTran.TranID)
							{
								payment.ClearDateOut = aTran.ClearDate;
								payment.ClearedOut = aTran.Cleared;
							}
							graph.Caches[typeof(CATransfer)].Update(payment);
						}
                        if (aTran.OrigTranType == CA.CATranType.CATransferExp)
                        {
                            CAAdj adj = PXSelect<CAAdj, Where<CAAdj.tranID, Equal<Required<CATran.tranID>>,And<CAAdj.cleared,NotEqual<True>>>>.Select(graph, aTran.TranID);
                            adj.Cleared = aTran.Cleared;
                            adj.ClearDate = aTran.ClearDate;
                            graph.Caches[typeof(CAAdj)].Update(adj);
                        }
					}
					break;
			}
		} 
		#endregion

		#region Create Document Functions
		protected virtual void CreateDocumentProc(CABankStatementDetail aRow, bool doPersist)
		{
			CATran result = null;
			PXCache sender = this.Details.Cache;
			if (aRow.CATranID != null)
			{
				sender.RaiseExceptionHandling<CABankStatementDetail.createDocument>(aRow, aRow.PayeeBAccountID, new PXSetPropertyException(Messages.DocumentIsAlreadyCreatedForThisDetail));
				return;
			}
			bool hasError = false;
			if (aRow.BAccountID == null && aRow.OrigModule != GL.BatchModule.CA)
			{
				sender.RaiseExceptionHandling<CABankStatementDetail.payeeBAccountID>(aRow, aRow.PayeeBAccountID, new PXSetPropertyException(Messages.PayeeIsRequiredToCreateDocument));
				hasError = true;
			}

			if (aRow.LocationID == null && aRow.OrigModule != GL.BatchModule.CA)
			{
				sender.RaiseExceptionHandling<CABankStatementDetail.payeeBAccountID>(aRow, aRow.PayeeLocationID, new PXSetPropertyException(Messages.PayeeLocationIsRequiredToCreateDocument));
				hasError = true;
			}

			if (aRow.OrigModule == GL.BatchModule.AR)                
			{
                if(string.IsNullOrEmpty(aRow.PaymentMethodID))
                {
                    sender.RaiseExceptionHandling<CABankStatementDetail.paymentMethodID>(aRow, aRow.PMInstanceID, new PXSetPropertyException(Messages.PaymentMethodIsRequiredToCreateDocument));
				    hasError = true;
                }
                if (!hasError && aRow.PMInstanceID == null)
                {
                    PaymentMethod pm = PXSelect<PaymentMethod, Where<PaymentMethod.paymentMethodID, Equal<Required<PaymentMethod.paymentMethodID>>>>.Select(this, aRow.PaymentMethodID);
                    if (pm.IsAccountNumberRequired == true)
                    {
                        sender.RaiseExceptionHandling<CABankStatementDetail.pMInstanceID>(aRow, aRow.PMInstanceID, new PXSetPropertyException(Messages.PaymentMethodIsRequiredToCreateDocument));
                        hasError = true;
                    }
                }
			}

			if (aRow.OrigModule == GL.BatchModule.AP && string.IsNullOrEmpty(aRow.PaymentMethodID))
			{
				sender.RaiseExceptionHandling<CABankStatementDetail.paymentMethodID>(aRow, aRow.PaymentMethodID, new PXSetPropertyException(Messages.PaymentMethodIsRequiredToCreateDocument));
				hasError = true;
			}

            List<ICADocAdjust> adjustments = new List<ICADocAdjust>();
			if (IsAPInvoiceSearchNeeded(aRow)) 
			{
				PXResult<APInvoice, APAdjust, APPayment> invResult = FindAPInvoiceByInvoiceInfo(aRow);
				if (invResult != null)
				{
					APInvoice invoiceToApply = invResult;
					APAdjust unreleasedAjustment = invResult;
					APPayment invoicePrepayment = invResult;
					if (invoiceToApply.Released == false && invoiceToApply.Prebooked == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.APPaymentApplicationInvoiceIsNotReleased, aRow.InvoiceInfo));
						hasError = true;
					}

					if (invoiceToApply.OpenDoc == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.APPaymentApplicationInvoiceIsClosed, aRow.InvoiceInfo));
						hasError = true;
					}

					if (invoiceToApply.DocDate > aRow.TranDate)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.APPaymentApplicationInvoiceDateIsGreaterThenPaymentDate, aRow.InvoiceInfo));
						hasError = true;
					}

					if (unreleasedAjustment != null && string.IsNullOrEmpty(unreleasedAjustment.AdjgRefNbr) == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.APPaymentApplicationInvoiceUnrealeasedApplicationExist, aRow.InvoiceInfo));
						hasError = true;
					}

					if (aRow.DrCr == CADrCr.CACredit && invoicePrepayment != null && string.IsNullOrEmpty(invoicePrepayment.RefNbr) == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.APPaymentApplicationInvoiceIsPartOfPrepaymentOrDebitAdjustment, aRow.InvoiceInfo));
						hasError = true;
					}

					if (!hasError)
					{						
						CABankStatementAdjustment adj = new CABankStatementAdjustment();
						adj.AdjdDocType = invoiceToApply.DocType;
						adj.AdjdRefNbr = invoiceToApply.RefNbr;
						adj.RefNbr = aRow.RefNbr;
						adj.LineNbr = aRow.LineNbr;
						adj = Adjustments.Insert(adj);
						AdjustInfo adjInfo = new AdjustInfo();
						adjInfo.AdjdRefNbr = adj.AdjdRefNbr;
						adjInfo.AdjdDocType = adj.AdjdDocType;
						adjInfo.CuryAdjgAmount = adj.CuryAdjgAmt;
						adjInfo.CuryAdjgDiscAmt = adj.CuryAdjgDiscAmt;
						adjInfo.CuryAdjgWhTaxAmt = adj.CuryAdjgWhTaxAmt;
						adjInfo.AdjdCuryRate = adj.AdjdCuryRate;
						adjustments.Add(adjInfo);
						Adjustments.Delete(adj);
					}
				}
				else
				{
					sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.APPaymentApplicationInvoiceIsNotFound, aRow.InvoiceInfo, PXErrorLevel.RowWarning));
				}
			}

			if (IsARInvoiceSearchNeeded(aRow))
			{
				PXResult<ARInvoice, ARAdjust> invResult = FindARInvoiceByInvoiceInfo(aRow);
				if (invResult != null)
				{
					ARInvoice invoiceToApply = invResult;
					ARAdjust unreleasedAjustment = invResult;
					if (invoiceToApply.Released == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.ARPaymentApplicationInvoiceIsNotReleased, aRow.InvoiceInfo));
						hasError = true;
					}

					if (invoiceToApply.OpenDoc == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.ARPaymentApplicationInvoiceIsClosed, aRow.InvoiceInfo));
						hasError = true;
					}

					if (invoiceToApply.DocDate > aRow.TranDate)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.ARPaymentApplicationInvoiceDateIsGreaterThenPaymentDate, aRow.InvoiceInfo));
						hasError = true;
					}

					if (unreleasedAjustment != null && string.IsNullOrEmpty(unreleasedAjustment.AdjgRefNbr) == false)
					{
						sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.ARPaymentApplicationInvoiceUnrealeasedApplicationExist, aRow.InvoiceInfo));
						hasError = true;
					}

					if (!hasError)
					{						
						AdjustInfo adj = new AdjustInfo();
						adj.Copy(invoiceToApply);
						adjustments.Add(adj);
					}
				}
				else
				{
					sender.RaiseExceptionHandling<CABankStatementDetail.invoiceInfo>(aRow, aRow.InvoiceInfo, new PXSetPropertyException(Messages.ARPaymentApplicationInvoiceIsNotFound, aRow.InvoiceInfo, PXErrorLevel.RowWarning));
				}
			}

            foreach (CABankStatementAdjustment adj in PXSelect<CABankStatementAdjustment,
                                                                    Where<CABankStatementAdjustment.refNbr, Equal<Required<CABankStatement.refNbr>>,
                                                                    And<CABankStatementAdjustment.lineNbr, Equal<Required<CABankStatementDetail.lineNbr>>>>>.Select(this, aRow.RefNbr, aRow.LineNbr))
            {
                AdjustInfo adjInfo = new AdjustInfo();
                adjInfo.AdjdRefNbr = adj.AdjdRefNbr;
                adjInfo.AdjdDocType = adj.AdjdDocType;
                adjInfo.CuryAdjgAmount = adj.CuryAdjgAmt;
                adjInfo.CuryAdjgDiscAmt = adj.CuryAdjgDiscAmt;
                adjInfo.CuryAdjgWhTaxAmt = adj.CuryAdjgWhTaxAmt;
                adjInfo.AdjdCuryRate = adj.AdjdCuryRate;
                adjustments.Add(adjInfo);
            }


			if (hasError)
				return;

			CABankStatement doc = this.BankStatement.Current;
			if (aRow.CashAccountID == null)
				aRow.CashAccountID = doc.CashAccountID;

			if (aRow.OrigModule == GL.BatchModule.AR)
			{
				bool OnHold = (this.casetup.Current.ReleaseAR == false);
				PaymentReclassifyProcess.CheckARTransaction(aRow);
				result = PaymentReclassifyProcess.AddARTransaction(aRow, null, adjustments, OnHold);
			}

			if (aRow.OrigModule == GL.BatchModule.AP)
			{
				bool OnHold = (this.casetup.Current.ReleaseAP == false);
				PaymentReclassifyProcess.CheckAPTransaction(aRow);
				result = PaymentReclassifyProcess.AddAPTransaction(aRow, null, adjustments, OnHold);
			}

			if (aRow.OrigModule == GL.BatchModule.CA)
			{
				CheckCATransaction(aRow, casetup.Current);
				result = AddCATransaction(this, aRow, null, null, false);
			}

			if (result != null)
			{
				CABankStatementDetail copy = (CABankStatementDetail)sender.CreateCopy(aRow);
				copy.CATranID = result.TranID;
				copy.DocumentMatched = true;
				aRow = (CABankStatementDetail)sender.Update(copy);
                
			}

			if (doPersist)
				this.Save.Press();
		}
	
		protected virtual bool IsARInvoiceSearchNeeded(CABankStatementDetail aRow) 
		{
			return (aRow.OrigModule == GL.BatchModule.AR && String.IsNullOrEmpty(aRow.InvoiceInfo) == false);
		}
		
		protected virtual bool IsAPInvoiceSearchNeeded(CABankStatementDetail aRow)
		{
			return (aRow.OrigModule == GL.BatchModule.AP && String.IsNullOrEmpty(aRow.InvoiceInfo) == false); 
		}
		/// <summary>
		/// Searches in database AR invoices, based on the the information in CABankStatementDetail record.
		/// The field used for the search are  - BAccountID and InvoiceInfo. First it is searching a invoice by it RefNbr, 
		/// then (if not found) - by invoiceNbr. 
		/// </summary>
		/// <param name="aRow">parameters for the search. The field used for the search are  - BAccountID and InvoiceInfo.</param>
		///	<returns>Returns null if nothing is found and PXResult<ARInvoice,ARAdjust> in the case of success.
		///		ARAdjust record represents unreleased adjustment (payment), applied to this Invoice
		///	</returns>
		protected virtual PXResult<ARInvoice, ARAdjust> FindARInvoiceByInvoiceInfo(CABankStatementDetail aRow) 
		{
			PXResult<ARInvoice,ARAdjust> invResult = (PXResult<ARInvoice, ARAdjust>)PXSelectJoin<ARInvoice,
											   LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
												  And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
												  And<ARAdjust.released, Equal<boolFalse>>>>>,
										  Where<ARInvoice.docType, Equal<AR.ARInvoiceType.invoice>,
										  And<ARInvoice.customerID, Equal<Required<ARInvoice.customerID>>,
										  And<ARInvoice.refNbr, Equal<Required<ARInvoice.refNbr>>>>>>.Select(this, aRow.BAccountID, aRow.InvoiceInfo);


			if (invResult == null)
			{
				invResult = (PXResult<ARInvoice, ARAdjust>)PXSelectJoin<ARInvoice,
								   LeftJoin<ARAdjust, On<ARAdjust.adjdDocType, Equal<ARInvoice.docType>,
											   And<ARAdjust.adjdRefNbr, Equal<ARInvoice.refNbr>,
											   And<ARAdjust.released, Equal<boolFalse>>>>>,											   
								   Where<ARInvoice.docType, Equal<AR.ARInvoiceType.invoice>,
									And<ARInvoice.customerID, Equal<Required<ARInvoice.customerID>>,
									And<ARInvoice.invoiceNbr, Equal<Required<ARInvoice.invoiceNbr>>>>>>.Select(this, aRow.BAccountID, aRow.InvoiceInfo);

			}
			return invResult; 			
		}

		/// <summary>
		/// Searches in database AR invoices, based on the the information in the CABankStatementDetail record.
		/// The field used for the search are  - BAccountID and InvoiceInfo. First it is searching a invoice by it RefNbr, 
		/// then (if not found) - by invoiceNbr. 
		/// </summary>
		/// <param name="aRow">Parameters for the search. The field used for the search are  - BAccountID and InvoiceInfo.</param>
		/// <returns>Returns null if nothing is found and PXResult<APInvoice,APAdjust,APPayment> in the case of success.
		/// APAdjust record represents unreleased adjustment (payment), applied to this APInvoice</returns>
		protected virtual PXResult<APInvoice, APAdjust, APPayment> FindAPInvoiceByInvoiceInfo(CABankStatementDetail aRow)
		{

			PXResult<APInvoice, APAdjust, APPayment> invResult = (PXResult<APInvoice, APAdjust, APPayment>)PXSelectJoin<APInvoice,
													 LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APInvoice.docType>,
														And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>,
														And<APAdjust.released, Equal<boolFalse>>>>,
													LeftJoin<APPayment, On<APPayment.docType, Equal<APInvoice.docType>,
														And<APPayment.refNbr, Equal<APInvoice.refNbr>,
														And<Where<APPayment.docType, Equal<APDocType.prepayment>,
															Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>,
												Where<APInvoice.docType, Equal<AP.APInvoiceType.invoice>,
												And<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,
												And<APInvoice.refNbr, Equal<Required<APInvoice.refNbr>>>>>>.Select(this, aRow.BAccountID, aRow.InvoiceInfo);


			if (invResult == null)
			{
				invResult = (PXResult<APInvoice, APAdjust, APPayment>)PXSelectJoin<APInvoice,
								   LeftJoin<APAdjust, On<APAdjust.adjdDocType, Equal<APInvoice.docType>,
											   And<APAdjust.adjdRefNbr, Equal<APInvoice.refNbr>,
											   And<APAdjust.released, Equal<boolFalse>>>>,
										   LeftJoin<APPayment, On<APPayment.docType, Equal<APInvoice.docType>,
											   And<APPayment.refNbr, Equal<APInvoice.refNbr>,
											   And<Where<APPayment.docType, Equal<APDocType.prepayment>,
												   Or<APPayment.docType, Equal<APDocType.debitAdj>>>>>>>>,
								   Where<APInvoice.docType, Equal<AP.APInvoiceType.invoice>,
									And<APInvoice.vendorID, Equal<Required<APInvoice.vendorID>>,
									And<APInvoice.invoiceNbr, Equal<Required<APInvoice.invoiceNbr>>>>>>.Select(this, aRow.BAccountID, aRow.InvoiceInfo);

			}
			return invResult;
		}

        public static void CheckCATransaction(ICADocSource parameters, CASetup setup)
        {
            if (parameters.OrigModule == GL.BatchModule.CA)
            {
                if (parameters.CashAccountID == null)
                {
                    throw new PXRowPersistingException(typeof(AddTrxFilter.cashAccountID).Name, null, ErrorMessages.FieldIsEmpty, typeof(AddTrxFilter.cashAccountID).Name);
                }

                if (string.IsNullOrEmpty(parameters.EntryTypeID))
                {
                    throw new PXRowPersistingException(typeof(AddTrxFilter.entryTypeID).Name, null, ErrorMessages.FieldIsEmpty, typeof(AddTrxFilter.entryTypeID).Name);
                }

                if (string.IsNullOrEmpty(parameters.ExtRefNbr) && setup.RequireExtRefNbr == true)
                {
                    throw new PXRowPersistingException(typeof(AddTrxFilter.extRefNbr).Name, null, ErrorMessages.FieldIsEmpty, typeof(AddTrxFilter.extRefNbr).Name);
                }
            }
        }

        public static CATran AddCATransaction(PXGraph graph, ICADocSource parameters, CurrencyInfo aCuryInfo, IEnumerable<CASplit> splits, bool IsTransferExpense)
		{
			if (parameters.OrigModule == GL.BatchModule.CA)
			{
                CATranEntry te = PXGraph.CreateInstance<CATranEntry>();
				CashAccount cashacct = (CashAccount)PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(graph, parameters.CashAccountID);

				CurrencyInfo refInfo = aCuryInfo;
				if (refInfo == null)
				{
					refInfo = (CurrencyInfo)PXSelectReadonly<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(te, parameters.CuryInfoID);
				}
				if (refInfo != null)
				{
					foreach (CurrencyInfo info in PXSelect<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Current<CAAdj.curyInfoID>>>>.Select(te))
					{
						CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(refInfo);
						new_info.CuryInfoID = info.CuryInfoID;
						te.currencyinfo.Cache.Update(new_info);
					}
				}
				else if ((cashacct != null) && (cashacct.CuryRateTypeID != null))
				{
					refInfo = new CurrencyInfo();
					refInfo.CuryID = cashacct.CuryID;
					refInfo.CuryRateTypeID = cashacct.CuryRateTypeID;
					refInfo = te.currencyinfo.Insert(refInfo);
				}

				CAAdj adj = new CAAdj();
				adj.AdjTranType = (IsTransferExpense ? CATranType.CATransferExp : CATranType.CAAdjustment);
				if (IsTransferExpense)
				{
					adj.TransferNbr = (graph as CashTransferEntry).Transfer.Current.TransferNbr;
				}
				adj.CashAccountID = parameters.CashAccountID;
				adj.CuryID = parameters.CuryID;
				adj.CuryInfoID = refInfo.CuryInfoID;
				adj.DrCr = parameters.DrCr;
				adj.ExtRefNbr = parameters.ExtRefNbr;
				adj.Released = false;
				adj.Cleared = parameters.Cleared;
				adj.TranDate = parameters.TranDate;
				adj.TranDesc = parameters.TranDesc;
				adj.EntryTypeID = parameters.EntryTypeID;
				adj.CuryControlAmt = parameters.CuryOrigDocAmt;
                adj.NoteID = parameters.NoteID;
				adj.Hold = false;
				adj.TaxZoneID = null;
				adj = te.CAAdjRecords.Insert(adj);
				adj.TaxCalcMode = AP.VendorClass.taxCalcMode.TaxSetting;
				adj = te.CAAdjRecords.Update(adj);
                if (splits == null)
                {
                    CASplit split = new CASplit();
                    split.AdjTranType = adj.AdjTranType;
                    //split.AccountID = parameters.AccountID;
                    split.CuryInfoID = refInfo.CuryInfoID;
                    split.Qty = (decimal)1.0;
                    split.CuryUnitPrice = parameters.CuryOrigDocAmt;
                    split.CuryTranAmt = parameters.CuryOrigDocAmt;
                    split.TranDesc = parameters.TranDesc;
                    //split.SubID = parameters.SubID;
                    te.CASplitRecords.Insert(split);
                }
                else
                {
                    foreach(CASplit split in splits)
                    {
                        split.AdjTranType = adj.AdjTranType;
                        split.AdjRefNbr = adj.RefNbr;
                        te.CASplitRecords.Insert(split);
                    }
                }
				adj.CuryTaxAmt = adj.CuryTaxTotal;
				adj = te.CAAdjRecords.Update(adj);
			    te.hold.Press();
				te.Save.Press();
				adj = (CAAdj)te.Caches[typeof(CAAdj)].Current;
				return (CATran)PXSelect<CATran, Where<CATran.tranID, Equal<Required<CAAdj.tranID>>>>.Select(te, adj.TranID);
			}
			return null;
		}
		
		#endregion

		#region IPXPrepareItems Members

        private const string keyFieldName = "RefNbr";
		public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
            
			if (string.Compare(viewName, "Details", true) == 0)
			{
                if (values.Contains(keyFieldName)) values[keyFieldName] = this.BankStatement.Current.RefNbr;
                else values.Add(keyFieldName, this.BankStatement.Current.RefNbr);
				
			}
			return true;
		}

		public bool RowImporting(string viewName, object row)
		{
			return row == null;
		}

		public bool RowImported(string viewName, object row, object oldRow)
		{
			return oldRow == null;
		}

		public void PrepareItems(string viewName, IEnumerable items)
		{
			//throw new NotImplementedException();
		}

		#endregion
	}

	[Serializable]
	public class CAApplicationStatementDetail : CABankStatementDetail
	{
		#region CashAccountID
		public new abstract class cashAccountID : PX.Data.IBqlField { }
		#endregion
		#region RefNbr
		public new abstract class refNbr : PX.Data.IBqlField { }
		#endregion
		#region LineNbr
		public new abstract class lineNbr : PX.Data.IBqlField { }
		#endregion
		#region ExtRefNbr
		public new abstract class extRefNbr : PX.Data.IBqlField { }
		#endregion
		#region CuryInfoID
		public new abstract class curyInfoID : PX.Data.IBqlField
		{
		}

		[PXDBLong()]
		public override Int64? CuryInfoID
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
		#region TranDate
		public new abstract class tranDate : PX.Data.IBqlField { }
		#endregion
		#region TranCode
		public new abstract class tranCode : PX.Data.IBqlField { }
		#endregion
		#region TranDesc
		public new abstract class tranDesc : PX.Data.IBqlField { }
		#endregion
		#region OrigModule
		public new abstract class origModule : PX.Data.IBqlField { }
		#endregion
		#region PayeeBAccountID
		public new abstract class payeeBAccountID : PX.Data.IBqlField { }
		#endregion
		#region PayeeName
		public new abstract class payeeName : PX.Data.IBqlField { }
		#endregion
		#region PayeeLocationID
		public new abstract class payeeLocationID : PX.Data.IBqlField { }
		#endregion
		#region PaymentMethodID
		public new abstract class paymentMethodID : PX.Data.IBqlField { }
		#endregion
		#region PMInstanceID
		public new abstract class pMInstanceID : PX.Data.IBqlField { }
		#endregion
		#region InvoiceInfo
		public new abstract class invoiceInfo : PX.Data.IBqlField { }
		#endregion
		#region CuryTotalAmt
		public new abstract class curyTotalAmount : PX.Data.IBqlField { }
		#endregion
		#region CuryApplAmt
		public new abstract class curyApplAmt : PX.Data.IBqlField { }
		#endregion
		#region CuryUnappliedBal
		public new abstract class curyUnappliedBal : PX.Data.IBqlField { }
		#endregion
		#region DocType
		public new abstract class docType : PX.Data.IBqlField { }
		#endregion
		#region LineCntr
		public new abstract class lineCntr : PX.Data.IBqlField { }
		#endregion
		#region CATranID
		public new abstract class cATranID : PX.Data.IBqlField { }
		#endregion

		#region CuryDebitAmt
		public new abstract class curyDebitAmt : PX.Data.IBqlField
		{
		}

		[PXDecimal()]
		[PXUIField(DisplayName = "Receipt")]
		public override Decimal? CuryDebitAmt
		{
			[PXDependsOnFields(typeof(drCr), typeof(curyTranAmt))]
			get
			{
				return (this._DrCr == CADrCr.CADebit) ? this._CuryTranAmt : Decimal.Zero;
			}
			set
			{
				if (value != 0m)
				{
					this._CuryTranAmt = value;
					this._DrCr = CADrCr.CADebit;
				}
				else if (this._DrCr == CADrCr.CADebit)
				{
					this._CuryTranAmt = 0m;
				}
			}
		}
		#endregion
		#region CuryCreditAmt
		public new abstract class curyCreditAmt : PX.Data.IBqlField
		{
		}

		[PXDecimal()]
		[PXUIField(DisplayName = "Disbursement")]
		public override Decimal? CuryCreditAmt
		{
			[PXDependsOnFields(typeof(drCr), typeof(curyTranAmt))]
			get
			{
				return (this._DrCr == CADrCr.CACredit) ? -this._CuryTranAmt : Decimal.Zero;
			}
			set
			{
				if (value != 0m)
				{
					this._CuryTranAmt = -value;
					this._DrCr = CADrCr.CACredit;
				}
				else if (this._DrCr == CADrCr.CACredit)
				{
					this._CuryTranAmt = 0m;
				}
			}
		}
		#endregion

		#region CuryReconciledDebit
		public new abstract class curyReconciledDebit : PX.Data.IBqlField
		{
		}
		[PXDecimal()]
		public override Decimal? CuryReconciledDebit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(curyDebitAmt))]
			get
			{
				return (this._DocumentMatched == true ? this.CuryDebitAmt : Decimal.Zero);
			}
			set
			{
			}
		}
		#endregion
		#region CuryReconciledCredit
		public new abstract class curyReconciledCredit : PX.Data.IBqlField
		{
		}
		[PXDecimal()]
		public override Decimal? CuryReconciledCredit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(curyCreditAmt))]
			get
			{
				return (this.DocumentMatched == true) ? this.CuryCreditAmt : Decimal.Zero;
			}
			set
			{
			}
		}
		#endregion

		#region CountDebit
		public new abstract class countDebit : PX.Data.IBqlField
		{
		}
		[PXInt()]
		public override Int32? CountDebit
		{
			[PXDependsOnFields(typeof(drCr))]
			get
			{
				return (this._DrCr == CADrCr.CADebit) ? (int)1 : (int)0;
			}
			set
			{
			}
		}
		#endregion
		#region CountCredit
		public new abstract class countCredit : PX.Data.IBqlField
		{
		}
		[PXInt()]
		public override Int32? CountCredit
		{
			[PXDependsOnFields(typeof(drCr))]
			get
			{
				return (this._DrCr == CADrCr.CACredit ? (int)1 : (int)0);
			}
			set
			{
			}
		}
		#endregion

		#region ReconciledCountDebit
		public new abstract class reconciledCountDebit : PX.Data.IBqlField
		{
		}
		[PXInt()]
		public override Int32? ReconciledCountDebit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(countDebit))]
			get
			{
				return (this.DocumentMatched == true) ? this.CountDebit : 0;
			}
			set
			{
			}
		}
		#endregion
		#region ReconciledCountCredit
		public new abstract class reconciledCountCredit : PX.Data.IBqlField
		{
		}
		[PXInt()]
		public override Int32? ReconciledCountCredit
		{
			[PXDependsOnFields(typeof(documentMatched), typeof(countCredit))]
			get
			{
				return (this.DocumentMatched == true) ? this.CountCredit : 0;
			}
			set
			{
			}
		}
		#endregion

	}

}
