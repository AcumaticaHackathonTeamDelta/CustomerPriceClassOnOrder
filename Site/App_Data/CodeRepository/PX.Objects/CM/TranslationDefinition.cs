using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Objects.BQLConstants;
using PX.Objects.GL;
using PX.Objects.CS;

namespace PX.Objects.CM
{    
    [Serializable]
	public class TranslationDefinitionMaint : PXGraph<TranslationDefinitionMaint, TranslDef>
    {
		#region Aliases
        [Serializable]
        [PXHidden]
		public partial class AccountFrom : Account
		{
			#region AccountID
			public new class accountID : PX.Data.IBqlField
			{
			}
			#endregion
			#region AccountCD
			public new class accountCD : PX.Data.IBqlField
			{
			}
			#endregion
		}
        [Serializable]
        [PXHidden]
		public partial class AccountTo : Account
		{
			#region AccountID
			public new class accountID : PX.Data.IBqlField
			{
			}
			#endregion
			#region AccountCD
			public new class accountCD : PX.Data.IBqlField
			{
			}
			#endregion
		}
        [Serializable]
        [PXHidden]
		public partial class SubFrom : Sub
		{
			#region SubID
			public new class subID : PX.Data.IBqlField
			{
			}
			#endregion
			#region SubCD
			public new class subCD : PX.Data.IBqlField
			{
			}
			#endregion
		}
        [Serializable]
        [PXHidden]
		public partial class SubTo : Sub
		{
			#region SubID
			public new class subID : PX.Data.IBqlField
			{
			}
			#endregion
			#region SubCD
			public new class subCD : PX.Data.IBqlField
			{
			}
			#endregion
		}
		#endregion

        public PXSelect<TranslDef> TranslDefRecords;
		public PXSelect<TranslDefDet, Where<TranslDefDet.translDefId, Equal<Current<TranslDef.translDefId>>>, OrderBy<Asc<TranslDefDet.accountIdFrom, Asc<TranslDefDet.subIdFrom>>>> TranslDefDetailsRecords;
		public PXSetup<CMSetup> CMSetup;
		public PXSetup<GLSetup> gLSetup;

		public TranslationDefinitionMaint()
		{
			var setup = CMSetup.Current;

			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>() != true) 
				throw new Exception(Messages.MultiCurrencyNotActivated);
		}
		
		#region Functions
		public virtual bool CheckDetail(PXCache cache, TranslDefDet newRow, bool active, Int32 destLedgerId, TranslDef def, Exception e)
		{
			bool ret = true;

			if (newRow.AccountIdFrom == null)
			{
				cache.RaiseExceptionHandling<TranslDefDet.accountIdFrom>(
					newRow, 
					null, 
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(TranslDefDet.accountIdFrom).Name));
				ret = false;
			}

			if (newRow.AccountIdTo == null)
			{
				cache.RaiseExceptionHandling<TranslDefDet.accountIdTo>(
					newRow, 
					null, 
					new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(TranslDefDet.accountIdTo).Name));
				ret = false;
			}
			
			if ((newRow.AccountIdFrom == newRow.AccountIdTo) &&
				(newRow.SubIdFrom == null || newRow.SubIdTo == null) && (newRow.SubIdFrom != null || newRow.SubIdTo != null))
			{
				if (newRow.SubIdFrom == null)
				{
					throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(TranslDefDet.subIdFrom).Name);
				}

				if (newRow.SubIdTo == null)
				{
					throw new PXSetPropertyException(ErrorMessages.FieldIsEmpty, typeof(TranslDefDet.subIdTo).Name);
				}
			}
			
			string AccountFromCD = (string)TranslDefDetailsRecords.GetValueExt<TranslDefDet.accountIdFrom>(newRow);
			string SubFromCD     = (string)TranslDefDetailsRecords.GetValueExt<TranslDefDet.subIdFrom>(newRow);
			string AccountToCD	 = (string)TranslDefDetailsRecords.GetValueExt<TranslDefDet.accountIdTo>(newRow);
			string SubToCD	     = (string)TranslDefDetailsRecords.GetValueExt<TranslDefDet.subIdTo>(newRow);

			if(	 ( (newRow.AccountIdFrom == newRow.AccountIdTo) &&
				   (String.Compare(SubFromCD, SubToCD) == 1))	  ||
				 //( (newRow.AccountIdFrom == newRow.AccountIdTo) && (newRow.SubIdFrom == newRow.SubIdTo) && (newRow.SubIdFrom != null) && (newRow.SubIdTo != null)) ||
				 ( String.Compare(AccountFromCD, AccountToCD) == 1))
			{
				throw new PXSetPropertyException(Messages.NotValidCombination);
			}

			if (ret == true && newRow.LineNbr != null && active == true)
			{
				foreach(PXResult<TranslDefDet, AccountFrom, AccountTo, SubFrom, SubTo> existingDet in 
							PXSelectJoin<TranslDefDet, InnerJoin<AccountFrom, On<TranslDefDet.accountIdFrom, Equal<AccountFrom.accountID>>, 
									InnerJoin<AccountTo, On<TranslDefDet.accountIdTo, Equal<AccountTo.accountID>>,
									LeftJoin<SubFrom, On<TranslDefDet.subIdFrom, Equal<SubFrom.subID>>, 
									LeftJoin<SubTo, On<TranslDefDet.subIdTo, Equal<SubTo.subID>>>>>>,
										Where<TranslDefDet.translDefId, Equal<Required<TranslDefDet.translDefId>>,
										  And<TranslDefDet.lineNbr, NotEqual<Required<TranslDefDet.lineNbr>>, 
										  And<	Where<Required<AccountFrom.accountCD>,   LessEqual<AccountTo.accountCD>,
												  And<AccountFrom.accountCD, LessEqual<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, NotEqual<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 NotEqual<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, NotEqual<Required<AccountTo.accountCD>>,
												  And<AccountTo.accountCD,	 NotEqual<Required<AccountFrom.accountCD>>,
												  And<AccountFrom.accountCD, NotEqual<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, NotEqual<Required<AccountTo.accountCD>>,
												   Or<AccountFrom.accountCD, Equal<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 Equal<Required<AccountFrom.accountCD>>,
												  And<Required<AccountFrom.accountCD>, NotEqual<Required<AccountTo.accountCD>>,
												  And2<Where<Required<SubFrom.subCD>, IsNull,
														 Or<Required<SubFrom.subCD>, IsNotNull,
														 And<Required<SubFrom.subCD>, LessEqual<SubTo.subCD>>>>,
												   Or<Required<AccountFrom.accountCD>, Equal<AccountFrom.accountCD>,
												  And<Required<AccountTo.accountCD>,   Equal<AccountFrom.accountCD>,
												  And<AccountFrom.accountCD,		   NotEqual<AccountTo.accountCD>,
												  And2<Where<SubFrom.subCD, IsNull,
														 Or<SubFrom.subCD, IsNotNull,
														 And<SubFrom.subCD, LessEqual<Required<SubTo.subCD>>>>>,
												   Or<AccountTo.accountCD,   Equal<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Equal<Required<AccountTo.accountCD>>,
												  And<Required<AccountFrom.accountCD>, NotEqual<Required<AccountTo.accountCD>>,
												  And2<Where<Required<SubTo.subCD>, IsNull,
														 Or<Required<SubTo.subCD>, IsNotNull,
														 And<SubFrom.subCD, LessEqual<Required<SubTo.subCD>>>>>,
												   Or<Required<AccountTo.accountCD>,   Equal<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, Equal<AccountTo.accountCD>,
												  And<AccountFrom.accountCD, NotEqual<AccountTo.accountCD>,
												  And2<Where<SubTo.subCD, IsNull,
														 Or<SubTo.subCD, IsNotNull,			
														 And<Required<SubFrom.subCD>, LessEqual<SubTo.subCD>>>>,
												   Or<AccountFrom.accountCD, Equal<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 Greater<AccountFrom.accountCD>,
												  And<Required<AccountTo.accountCD>,   Greater<Required<AccountFrom.accountCD>>,
												   Or<Required<AccountFrom.accountCD>, Equal<AccountFrom.accountCD>,
												  And<Required<AccountTo.accountCD>,   Greater<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 Greater<AccountFrom.accountCD>,
												   Or<AccountTo.accountCD, Equal<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Less<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, Less<Required<AccountTo.accountCD>>,
												   Or<Required<AccountTo.accountCD>, Equal<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, Less<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Less<AccountTo.accountCD>,

												   Or<AccountFrom.accountCD,  Equal<Required<AccountTo.accountCD>>,
												  And2<Where<SubFrom.subCD, IsNull,
														 Or<Required<SubTo.subCD>, IsNull,
														 Or<SubFrom.subCD, IsNotNull,
														 And<Required<SubTo.subCD>, IsNotNull,			
														 And<Required<SubTo.subCD>, GreaterEqual<SubFrom.subCD>>>>>>,
												   Or<Required<AccountFrom.accountCD>,  Equal<AccountTo.accountCD>,
												  And2<Where<Required<SubFrom.subCD>, IsNull,
														 Or<SubTo.subCD, IsNull,
														 Or<Required<SubFrom.subCD>, IsNotNull,
														 And<SubTo.subCD, IsNotNull,			
														 And<SubTo.subCD, GreaterEqual<Required<SubFrom.subCD>>>>>>>,

												   Or<AccountTo.accountCD,    Equal<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Equal<Required<AccountTo.accountCD>>,
												  And<Required<AccountFrom.accountCD>, Equal<Required<AccountTo.accountCD>>,
											      And<Where<SubFrom.subCD, IsNotNull,
														And<Required<SubFrom.subCD>, IsNotNull,
														And<Required<SubFrom.subCD>, LessEqual<SubTo.subCD>,
														And<SubFrom.subCD, LessEqual<Required<SubTo.subCD>>,
														 Or<SubFrom.subCD, IsNull,
														 Or<Required<SubFrom.subCD>, IsNull>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
											  .Select(cache.Graph, 
													  newRow.TranslDefId,
													  newRow.LineNbr, 
													  AccountFromCD, 
													  AccountToCD,
													  AccountFromCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD,
													  AccountFromCD,
													  AccountToCD,
													  AccountFromCD,
													  AccountFromCD,
													  AccountFromCD, 
													  AccountToCD,
													  SubFromCD,
													  SubFromCD,
													  SubFromCD,
													  AccountFromCD,
													  AccountToCD,
													  SubToCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD, 
													  AccountToCD,
													  SubToCD,
													  SubToCD,
													  SubToCD,
													  AccountToCD,
													  AccountFromCD,
													  SubFromCD,
													  AccountFromCD,
													  AccountToCD,   
													  AccountFromCD,
													  AccountFromCD,
													  AccountToCD,   
													  AccountFromCD,
													  AccountToCD,
													  AccountFromCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD, 
													  AccountToCD,
													  AccountToCD,
													  SubToCD,
													  SubToCD,			
													  SubToCD,
													  AccountFromCD,
													  SubFromCD,
													  SubFromCD,
													  SubFromCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD, 
													  AccountToCD,
													  SubFromCD,
													  SubFromCD,
													  SubToCD,
													  SubFromCD))
												  
				{
					TranslDefDet existingDefinitionDetail = existingDet;
					AccountFrom accountFrom = existingDet;
					AccountTo accountTo = existingDet;
					SubFrom subAccountFrom = existingDet;
					SubTo subAccountTo = existingDet;

					if (existingDefinitionDetail != null)
					{
						cache.RaiseExceptionHandling<TranslDefDet.accountIdFrom>(
							newRow, 
							newRow.AccountIdFrom,
							new PXSetPropertyException(
								Messages.RangeIntersectsWithRangeOnTheExistingDefinition,
								PXErrorLevel.RowError,
								existingDefinitionDetail.TranslDefId,
								accountFrom.AccountCD,
								subAccountFrom.SubCD == null ? string.Empty : $"/{subAccountFrom.SubCD}",
								accountTo.AccountCD,
								subAccountTo.SubCD == null ? string.Empty : $"/{subAccountTo.SubCD}"));

						if (e != null) throw new PXSetPropertyException(e.Message);
					}
				}
				
				foreach(PXResult<TranslDefDet, TranslDef, AccountFrom, AccountTo, SubFrom, SubTo> existingDetInOthers 
						in PXSelectJoin<TranslDefDet, InnerJoin<TranslDef, On<TranslDefDet.translDefId, Equal<TranslDef.translDefId>>, 
										InnerJoin<AccountFrom, On<TranslDefDet.accountIdFrom, Equal<AccountFrom.accountID>>, 
										InnerJoin<AccountTo, On<TranslDefDet.accountIdTo, Equal<AccountTo.accountID>>,
										LeftJoin<SubFrom, On<TranslDefDet.subIdFrom, Equal<SubFrom.subID>>, 
										LeftJoin<SubTo, On<TranslDefDet.subIdTo, Equal<SubTo.subID>>>>>>>,
											Where<TranslDef.destLedgerId, Equal<Required<TranslDef.destLedgerId>>, 
											  And<TranslDef.active, Equal<boolTrue>, 
											  And<TranslDef.translDefId, NotEqual<Required<TranslDefDet.translDefId>>,
											  And<	Where<Required<AccountFrom.accountCD>,   LessEqual<AccountTo.accountCD>,
												  And<AccountFrom.accountCD, LessEqual<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, NotEqual<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 NotEqual<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, NotEqual<Required<AccountTo.accountCD>>,
												  And<AccountTo.accountCD,	 NotEqual<Required<AccountFrom.accountCD>>,
												  And<AccountFrom.accountCD, NotEqual<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, NotEqual<Required<AccountTo.accountCD>>,
												   Or<AccountFrom.accountCD, Equal<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 Equal<Required<AccountFrom.accountCD>>,
												  And<Required<AccountFrom.accountCD>, NotEqual<Required<AccountTo.accountCD>>,
												  And2<Where<Required<SubFrom.subCD>, IsNull,
														 Or<Required<SubFrom.subCD>, IsNotNull,
														 And<Required<SubFrom.subCD>, LessEqual<SubTo.subCD>>>>,
												   Or<Required<AccountFrom.accountCD>, Equal<AccountFrom.accountCD>,
												  And<Required<AccountTo.accountCD>,   Equal<AccountFrom.accountCD>,
												  And<AccountFrom.accountCD,		   NotEqual<AccountTo.accountCD>,
												  And2<Where<SubFrom.subCD, IsNull,
														 Or<SubFrom.subCD, IsNotNull,
														 And<SubFrom.subCD, LessEqual<Required<SubTo.subCD>>>>>,
												   Or<AccountTo.accountCD,   Equal<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Equal<Required<AccountTo.accountCD>>,
												  And<Required<AccountFrom.accountCD>, NotEqual<Required<AccountTo.accountCD>>,
												  And2<Where<Required<SubTo.subCD>, IsNull,
														 Or<Required<SubTo.subCD>, IsNotNull,
														 And<SubFrom.subCD, LessEqual<Required<SubTo.subCD>>>>>,
												   Or<Required<AccountTo.accountCD>,   Equal<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, Equal<AccountTo.accountCD>,
												  And<AccountFrom.accountCD, NotEqual<AccountTo.accountCD>,
												  And2<Where<SubTo.subCD, IsNull,
														 Or<SubTo.subCD, IsNotNull,			
														 And<Required<SubFrom.subCD>, LessEqual<SubTo.subCD>>>>,
												   Or<AccountFrom.accountCD, Equal<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 Greater<AccountFrom.accountCD>,
												  And<Required<AccountTo.accountCD>,   Greater<Required<AccountFrom.accountCD>>,
												   Or<Required<AccountFrom.accountCD>, Equal<AccountFrom.accountCD>,
												  And<Required<AccountTo.accountCD>,   Greater<Required<AccountFrom.accountCD>>,
												  And<AccountTo.accountCD,	 Greater<AccountFrom.accountCD>,
												   Or<AccountTo.accountCD, Equal<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Less<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, Less<Required<AccountTo.accountCD>>,
												   Or<Required<AccountTo.accountCD>, Equal<AccountTo.accountCD>,
												  And<Required<AccountFrom.accountCD>, Less<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Less<AccountTo.accountCD>,

												   Or<AccountFrom.accountCD,  Equal<Required<AccountTo.accountCD>>,
												  And2<Where<SubFrom.subCD, IsNull,
														 Or<Required<SubTo.subCD>, IsNull,
														 Or<SubFrom.subCD, IsNotNull,
														 And<Required<SubTo.subCD>, IsNotNull,			
														 And<Required<SubTo.subCD>, GreaterEqual<SubFrom.subCD>>>>>>,
												   Or<Required<AccountFrom.accountCD>,  Equal<AccountTo.accountCD>,
												  And2<Where<Required<SubFrom.subCD>, IsNull,
														 Or<SubTo.subCD, IsNull,
														 Or<Required<SubFrom.subCD>, IsNotNull,
														 And<SubTo.subCD, IsNotNull,			
														 And<SubTo.subCD, GreaterEqual<Required<SubFrom.subCD>>>>>>>,

												   Or<AccountTo.accountCD,    Equal<Required<AccountTo.accountCD>>,
												  And<AccountFrom.accountCD, Equal<Required<AccountTo.accountCD>>,
												  And<Required<AccountFrom.accountCD>, Equal<Required<AccountTo.accountCD>>,
												  And<Where<SubFrom.subCD, IsNotNull,
														And<Required<SubFrom.subCD>, IsNotNull,
														And<Required<SubFrom.subCD>, LessEqual<SubTo.subCD>,
														And<SubFrom.subCD, LessEqual<Required<SubTo.subCD>>,
														 Or<SubFrom.subCD, IsNull,
														 Or<Required<SubFrom.subCD>, IsNull>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
											  .Select(cache.Graph, 
													  destLedgerId, 
													  newRow.TranslDefId,  
													  AccountFromCD, 
													  AccountToCD,
													  AccountFromCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD,
													  AccountFromCD,
													  AccountToCD,
													  AccountFromCD,
													  AccountFromCD,
													  AccountFromCD, 
													  AccountToCD,
													  SubFromCD,
													  SubFromCD,
													  SubFromCD,
													  AccountFromCD,
													  AccountToCD,
													  SubToCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD, 
													  AccountToCD,
													  SubToCD,
													  SubToCD,
													  SubToCD,
													  AccountToCD,
													  AccountFromCD,
													  SubFromCD,
													  AccountFromCD,
													  AccountToCD,   
													  AccountFromCD,
													  AccountFromCD,
													  AccountToCD,   
													  AccountFromCD,
													  AccountToCD,
													  AccountFromCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD, 
													  AccountToCD,
													  AccountToCD,
													  SubToCD,
													  SubToCD,			
													  SubToCD,
													  AccountFromCD,
													  SubFromCD,
													  SubFromCD,
													  SubFromCD,
													  AccountToCD,
													  AccountToCD,
													  AccountFromCD, 
													  AccountToCD,
													  SubFromCD,
													  SubFromCD,
													  SubToCD,
													  SubFromCD))
												  
				{
					TranslDefDet existingDefinitionDetail = existingDetInOthers;
					TranslDef existingDefinition = existingDetInOthers;
					AccountFrom accountFrom = existingDetInOthers;
					AccountTo accountTo = existingDetInOthers;
					SubFrom subAccountFrom = existingDetInOthers;
					SubTo subAccountTo = existingDetInOthers;

					if (existingDefinitionDetail != null && 
						existingDefinition != null && 
						(def.BranchID == null || existingDefinition.BranchID == null || existingDefinition.BranchID == def.BranchID))
					{
						cache.RaiseExceptionHandling<TranslDefDet.accountIdFrom>(
							newRow, 
							newRow.AccountIdFrom,
							new PXSetPropertyException(
								Messages.RangeIntersectsWithRangeOnTheExistingDefinition,
								PXErrorLevel.RowError,
								existingDefinitionDetail.TranslDefId,
								accountFrom.AccountCD,
								subAccountFrom.SubCD == null ? string.Empty : $"/{subAccountFrom.SubCD}",
								accountTo.AccountCD,
								subAccountTo.SubCD == null ? string.Empty : $"/{subAccountTo.SubCD}"));

						if (e != null) throw new PXSetPropertyException(e.Message);
					}
				}
			}
			if (ret == true)
			{
				GLSetup glsetup = gLSetup.Current; 
				if (glsetup != null && glsetup.YtdNetIncAccountID != null)
				{
					string YtdNetIncAccountCD	 = (string)gLSetup.GetValueExt<GLSetup.ytdNetIncAccountID>(glsetup);
					int resFrom = String.Compare(AccountFromCD, YtdNetIncAccountCD); 	
					int resTo   = String.Compare(AccountToCD,   YtdNetIncAccountCD);
					if(	(resFrom == 0 || resFrom == -1) && (resTo == 1 || resTo == 0))
					{
						cache.RaiseExceptionHandling<TranslDefDet.accountIdFrom>(newRow, AccountFromCD, new PXSetPropertyException(Messages.YTDNetIncomeAccountWillBeExclude, PXErrorLevel.RowWarning));
					}
				}
			}
			else
			{
				if (e != null)
					throw new PXSetPropertyException(e.Message);
			}
			return ret;
		}
		#endregion

		#region TranslDefDet Events

		/*
        protected virtual void TranslDefDet_CalcMode_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            TranslDefDet row = (TranslDefDet)e.Row;

            if (row != null)
            {
                if (row.CalcMode == 2)
                {
                    row.RateTypeId = TranslationSetup.Current.AvgRateTypeId; 
                }
                else
                {
                    row.RateTypeId = TranslationSetup.Current.EffRateTypeId;
                }
            }
        }
		*/

		protected virtual void TranslDefDet_RowUpdating(PXCache cache, PXRowUpdatingEventArgs e)
        {
            TranslDefDet newRow = (TranslDefDet)e.NewRow;
			TranslDef		def = (TranslDef) TranslDefRecords.Current;
            if (newRow == null || def == null) return;
			CheckDetail(cache, newRow, def.Active == true, (Int32)def.DestLedgerId, def, null);
		}

		protected virtual void TranslDefDet_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete)
			{
				TranslDefDet newRow = (TranslDefDet)e.Row;
				TranslDef def = TranslDefRecords.Current;
				if (newRow == null || def == null) return;
				CheckDetail(cache, newRow, def.Active == true, (Int32)def.DestLedgerId, def, null);
			}
		}

		protected virtual void TranslDefDet_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
        {
			TranslDefDet newRow = (TranslDefDet)e.Row;
			TranslDef def = TranslDefRecords.Current;
			if (newRow == null || def == null) return;
			CheckDetail(cache, newRow, def.Active == true, (Int32)TranslDefRecords.Current.DestLedgerId, def, null);
		}
		 
		#endregion

		#region TranslDef Events
		protected virtual void TranslDef_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Delete)
			{
				TranslDef tDef = (TranslDef)e.Row;

				TranslationHistory hist = PXSelect<TranslationHistory,
													 Where<TranslationHistory.translDefId, Equal<Required<TranslationHistory.translDefId>>>>.
													 Select(this, tDef.TranslDefId);
				if ((hist != null) && (hist.ReferenceNbr != null))
				{
					e.Cancel = true;
					throw new PXException(PX.Objects.CM.Messages.TranslDefIsAlreadyUsed, tDef.TranslDefId);
				}
			}
		}

		protected virtual void TranslDef_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row != null)
			{
				TranslDef row = (TranslDef)e.Row;

				if (row.SourceLedgerId != null)
				{
					Ledger sLedger = PXSelectReadonly<Ledger, Where<Ledger.ledgerID, Equal<Required<Ledger.ledgerID>>>>.Select(this, row.SourceLedgerId);
					row.SourceCuryID = sLedger?.BaseCuryID;
				}

				if (row.DestLedgerId != null)
				{
					Ledger dLedger = PXSelectReadonly<Ledger, Where<Ledger.ledgerID, Equal<Required<Ledger.ledgerID>>>>.Select(this, row.DestLedgerId);
					row.DestCuryID = dLedger?.BaseCuryID;
				}
			}
		}
		
		protected virtual void TranslDef_Active_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
		{
            PXBoolAttribute.ConvertValue(e);
			TranslDef row = (TranslDef)e.Row;
			if (row == null) return;
			if ((row != null) && (row.TranslDefId != null) && ((bool)e.NewValue == true))
			{
				foreach (TranslDefDet td in TranslDefDetailsRecords.Select())					
				{
					CheckDetail(Caches[typeof(TranslDefDet)], td, (bool)e.NewValue == true, (Int32)row.DestLedgerId, row,
						new Exception(Messages.TranslationDefinitionCanNotBeActive));
				}
	   		}
		}
		protected virtual void TranslDef_DestLedgerId_FieldUpdating(PXCache cache, PXFieldUpdatingEventArgs e)
		{
			TranslDef row = (TranslDef)e.Row;
			if (row == null) return;
			if ((row != null) && (row.TranslDefId != null) && (e.NewValue != null))
			{
				Ledger ledger = (Ledger)PXSelect<Ledger, Where<Ledger.ledgerCD, Equal<Required<Ledger.ledgerCD>>>>.Select(this, e.NewValue);
				if (ledger != null && (row.Active == true))
				{
					foreach (TranslDefDet td in TranslDefDetailsRecords.Select())
					{
						CheckDetail(Caches[typeof(TranslDefDet)], td, (row.Active == true), (int)ledger.LedgerID, row,
							new Exception(Messages.TranslationDestinationLedegrIDCanNotBeChanged));
						
					}
				}
			}
		}
		protected virtual void TranslDef_BranchID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			TranslDef row = (TranslDef)e.Row;
			if (row == null) return;
			if (row.TranslDefId != null && (bool)row.Active == true)
			{
				row.Active = false;
			}
		} 
		#endregion
	}
}
