using System;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Objects.BQLConstants;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.TX;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.IN;

namespace PX.Objects.CA
{
	[System.SerializableAttribute()]
	public partial class CARegister : PX.Data.IBqlTable
	{
		#region TranID
		public abstract class tranID : PX.Data.IBqlField
		{
		}
		protected Int64? _TranID;
		[PXLong(IsKey = true)]
		[PXUIField(DisplayName = "Transaction Num.")]
		public virtual Int64? TranID
		{
			get
			{
				return this._TranID;
			}
			set
			{
				this._TranID = value;
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
		[PXUIField(DisplayName = "Selected")]
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
		#region Hold
		public abstract class hold : IBqlField
		{
		}
		protected bool? _Hold = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Hold")]
		public bool? Hold
		{
			get
			{
				return _Hold;
			}
			set
			{
				_Hold = value;
			}
		}
		#endregion
		#region Released
		public abstract class released : IBqlField
		{
		}
		protected bool? _Released = false;
		[PXBool]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Released")]
		public bool? Released
		{
			get
			{
				return _Released;
			}
			set
			{
				_Released = value;
			}
		}
		#endregion
		#region Module
		public abstract class module : PX.Data.IBqlField
		{
		}
		protected String _Module;
		[PXString(3)]
		[GL.BatchModule.List()]
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
        #region TranType
        public abstract class tranType : PX.Data.IBqlField
        {
        }
        protected String _TranType;
        [PXString(3)]
        [CAAPARTranType.ListByModule(typeof(module))]
        [PXUIField(DisplayName = "Transaction Type")]
        public virtual String TranType
        {
            get
            {
                return this._TranType;
            }
            set
            {
                this._TranType = value;
            }
        }
        #endregion
		#region ReferenceNbr
		public abstract class referenceNbr : PX.Data.IBqlField
		{
		}
		protected String _ReferenceNbr;
		[PXString(15, IsUnicode = true)]
		[PXUIField(DisplayName = "Transaction Number", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String ReferenceNbr
		{
			get
			{
				return this._ReferenceNbr;
			}
			set
			{
				this._ReferenceNbr = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.Visible)]
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
		#region DocDate
		public abstract class docDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _DocDate;
		[PXDate()]
		[PXUIField(DisplayName = "Doc. Date")]
		public virtual DateTime? DocDate
		{
			get
			{
				return this._DocDate;
			}
			set
			{
				this._DocDate = value;
			}
		}
		#endregion
		#region FinPeriodID
		public abstract class finPeriodID : PX.Data.IBqlField
		{
		}
		protected String _FinPeriodID;
		[FinPeriodID()]
		[PXUIField(DisplayName = "Fin. Period", Visibility = PXUIVisibility.SelectorVisible)]
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
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
			}
		}
		#endregion
		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[PXDefault()]
		[GL.CashAccount(DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr))]
		public virtual Int32? CashAccountID
		{
			get
			{
				return this._CashAccountID;
			}
			set
			{
				this._CashAccountID = value;
			}
		}
		#endregion
		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Enabled = false)]
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
		#region TranAmt
		public abstract class tranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _TranAmt;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Tran. Amount", Enabled = false)]
		public virtual Decimal? TranAmt
		{
			get
			{
				return this._TranAmt;
			}
			set
			{
				this._TranAmt = value;
			}
		}
		#endregion
		#region CuryTranAmt
		public abstract class curyTranAmt : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryTranAmt;
		[PXDBCury(typeof(CARecon.curyID))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Enabled = false)]
		public virtual Decimal? CuryTranAmt
		{
			get
			{
				return this._CuryTranAmt;
			}
			set
			{
				this._CuryTranAmt = value;
			}
		}
		#endregion
	}

	public abstract class InfoMessage 
	{
        public InfoMessage(PXErrorLevel aLevel, string aMessage) 
        {
            this._ErrorLevel = aLevel;
            this._Message = aMessage;
        } 

		#region PXErrorLevel
		protected PXErrorLevel _ErrorLevel;
	    public virtual PXErrorLevel ErrorLevel
		{
			get
			{
				return this._ErrorLevel;
			}
			set
			{
				this._ErrorLevel = value;
			}
		}
		#endregion
		#region Message
		
		protected String _Message;
		public virtual String Message
		{
			get
			{
				return this._Message;
			}
			set
			{
				this._Message = value;
			}
		}
		#endregion
	}

	public class CAMessage : InfoMessage
	{
        public CAMessage(long aKey, PXErrorLevel aLevel, string aMessage):base(aLevel, aMessage) 
        {
            this._Key = aKey;            
        } 
		#region Key
		protected Int64 _Key;
		public virtual Int64 Key
		{
			get
			{
				return this._Key;
			}
			set
			{
				this._Key = value;
			}
		}
		#endregion      
	}

	public class CAReconMessage : InfoMessage
	{
        public CAReconMessage(int aCashAccountID, string aReconNbr, PXErrorLevel aLevel, string aMessage): 
                base(aLevel,aMessage) 
        {
            this._KeyCashAccount = aCashAccountID;
            this._KeyReconNbr = aReconNbr;            
        }

		#region KeyCashAccount
		public abstract class keyCashAccount : PX.Data.IBqlField
		{
		}
		protected Int32 _KeyCashAccount;
		public virtual Int32 KeyCashAccount
		{
			get
			{
				return this._KeyCashAccount;
			}
			set
			{
				this._KeyCashAccount = value;
			}
		}
		#endregion
		#region KeyReconNbr
		protected String _KeyReconNbr;
		public virtual String KeyReconNbr
		{
			get
			{
				return this._KeyReconNbr;
			}
			set
			{
				this._KeyReconNbr = value;
			}
		}
		#endregion        
	}

	[System.SerializableAttribute()]
	[PXPrimaryGraph(new Type[] {
					typeof(CATranEntry),
					typeof(CashTransferEntry)},
						new Type[] {
					typeof(Select<CAAdj, Where<CAAdj.tranID, Equal<Current<CATran.tranID>>>>),
					typeof(Select<CATransfer, Where<CATransfer.tranIDIn, Equal<Current<CATran.tranID>>, 
							Or<CATransfer.tranIDOut, Equal<Current<CATran.tranID>>>>>) 
					})]
	[TableAndChartDashboardType]    
	public class CATrxRelease : PXGraph<CATrxRelease>
	{
		/// <summary>
		/// CashAccount override - SQL Alias
		/// </summary>
        [Serializable]
        [PXHidden]
		public class CashAccount1: CashAccount 
		{
			public new abstract class cashAccountID : PX.Data.IBqlField
			{
			}
		}
		public CATrxRelease()
		{
			CASetup setup = cASetup.Current;
			CARegisterList.SetProcessDelegate(delegate(List<CARegister> list) 
            {
                GroupRelease(list, true); 
            });
            CARegisterList.SetProcessCaption(Messages.Release);
            CARegisterList.SetProcessAllCaption(Messages.ReleaseAll);
			
		}
		#region Buttons
		#region Cancel
		[PXUIField(DisplayName = ActionsMessages.Cancel, MapEnableRights = PXCacheRights.Select)]
		[PXCancelButton]
		protected virtual IEnumerable Cancel(PXAdapter adapter)
		{
			CARegisterList.Cache.Clear();
			TimeStamp = null;
			PXLongOperation.ClearStatus(this.UID);
			return adapter.Get();
		}
		#endregion

		#region viewCATrax
		[PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXEditDetailButton]
		public virtual IEnumerable ViewCATrx(PXAdapter adapter)
		{
			CARegister register = CARegisterList.Current;
			if (register != null)
			{
				switch (register.TranType)
				{
					case (CAAPARTranType.CAAdjustment):
						CATranEntry graphTranEntry = PXGraph.CreateInstance<CATranEntry>();
						graphTranEntry.Clear();
						CARegisterList.Cache.IsDirty = false;
						graphTranEntry.CAAdjRecords.Current = PXSelect<CAAdj, Where<CAAdj.adjRefNbr, Equal<Required<CATran.origRefNbr>>>>
							.Select(this, register.ReferenceNbr); // !!!
                        throw new PXRedirectRequiredException(graphTranEntry, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
                            //break;
                    case (CAAPARTranType.CAVoidDeposit):
                    case (CAAPARTranType.CADeposit):
                        CADepositEntry graphDepositEntry = PXGraph.CreateInstance<CADepositEntry>();
                        graphDepositEntry.Clear();
                        CARegisterList.Cache.IsDirty = false;
                        graphDepositEntry.Document.Current = PXSelect<CADeposit, Where<CADeposit.refNbr, Equal<Required<CATran.origRefNbr>>,And<CADeposit.tranType, Equal<Required<CATran.origTranType>>>>>
                            .Select(this, register.ReferenceNbr, register.TranType);
                        throw new PXRedirectRequiredException(graphDepositEntry, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					//break;
					case (CAAPARTranType.CATransfer):
						CashTransferEntry graphTransferEntry = PXGraph.CreateInstance<CashTransferEntry>();
						graphTransferEntry.Clear();
						CARegisterList.Cache.IsDirty = false;
						graphTransferEntry.Transfer.Current = PXSelect<CATransfer, Where<CATransfer.transferNbr, Equal<Required<CATransfer.transferNbr>>>>
							.Select(this, register.ReferenceNbr);
						throw new PXRedirectRequiredException(graphTransferEntry, true, "Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
					//break;
				}
			}
			return CARegisterList.Select();
		}
		#endregion

		#endregion

		#region selectStatements
        public PXAction<CARegister> cancel;
        public PXAction<CARegister> viewCATrx;
		[PXFilterable]
		public PXProcessing<CARegister> CARegisterList;
		public PXSetup<CASetup> cASetup;
		#endregion

		#region Function
		protected virtual IEnumerable caregisterlist()
		{
			bool anyFound = false;       
			foreach (CARegister tlist in CARegisterList.Cache.Inserted)
			{
				anyFound = true;				
				yield return tlist;
			}

			if (anyFound)
			{
				yield break;
			}
            foreach (CADeposit deposit in PXSelectJoin<CADeposit, InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CADeposit.cashAccountID>, And<Match<CashAccount, Current<AccessInfo.userName>>>>>, 
                Where<CADeposit.released, Equal<boolFalse>, And<CADeposit.hold, Equal<boolFalse>,
                And<Where<CADeposit.tranType, Equal<CATranType.cADeposit>, Or<CADeposit.tranType, Equal<CATranType.cAVoidDeposit>>>>>>>.Select(this))
            {
                if (deposit.TranID != null)
                    yield return CARegisterList.Cache.Insert(CARegister(deposit));
            }
			foreach (CAAdj adj in PXSelectJoin<CAAdj, InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CAAdj.cashAccountID>, And<Match<CashAccount,Current<AccessInfo.userName>>>>>, Where<CAAdj.released, Equal<boolFalse>, And<CAAdj.status, Equal<CATransferStatus.balanced>, And<CAAdj.adjTranType, Equal<CATranType.cAAdjustment>>>>>.Select(this))
			{
				if (adj.TranID != null)
					yield return CARegisterList.Cache.Insert(CARegister(adj));
			}

			foreach (CATransfer trf in PXSelectJoin<CATransfer,
											InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CATransfer.inAccountID>, And<Match<CashAccount, Current<AccessInfo.userName>>>>,
											InnerJoin<CashAccount1, On<CashAccount1.cashAccountID, Equal<CATransfer.outAccountID>, And<Match<CashAccount1, Current<AccessInfo.userName>>>>>>,
											Where<CATransfer.released, Equal<boolFalse>, And<CATransfer.hold, Equal<boolFalse>>>>.Select(this))
			{
				foreach (CATran tran in PXSelect<CATran, Where<CATran.released, Equal<boolFalse>,
																													 And<CATran.hold, Equal<boolFalse>,
																													 And<Where<CATran.tranID, Equal<Required<CATransfer.tranIDIn>>,
																																	Or<CATran.tranID, Equal<Required<CATransfer.tranIDOut>>>>>>>>.Select(this, trf.TranIDIn, trf.TranIDOut))
				{
					yield return CARegisterList.Cache.Insert(CARegister(trf, tran));
				}
			}

			CARegisterList.Cache.IsDirty = false;
		}

        protected virtual void CARegister_TranID_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            CARegister row = (CARegister)e.Row;
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
                    
                    if (listMessages.ContainsKey(row.TranID.Value))
                        message = listMessages[row.TranID.Value];
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

		public static void GroupReleaseTransaction(List<CATran> tranList, bool allowAP, bool allowAR, bool updateInfo)
		{
			Dictionary<long,CAMessage> listMessages = new Dictionary<long,CAMessage>();
			if (updateInfo == true)
				PXLongOperation.SetCustomInfo(listMessages);
			List<CARegister> caRegisterList = new List<CARegister>();
            bool allPassed = true;
            PXGraph searchGraph = null;
			for (int i = 0; i < tranList.Count; i++)
            {
                CATran tran = tranList[i];
                try
                {
                    if (tran.Released == true)
                    {
                        continue;
                    }
                    if (tran.Hold == true)
                    {
                        throw new PXException(Messages.DocumentStatusInvalid);
                    }
                    switch (tran.OrigModule)
                    {
                        case GL.BatchModule.GL:
                            throw new PXException(Messages.ThisDocTypeNotAvailableForRelease);
                        case GL.BatchModule.AP:
                            if (allowAP != true)
                            {
                                throw new PXException(Messages.APDocumentsCanNotBeReleasedFromCAModule);
                            }
                            else
                            {
                                CATrxRelease.ReleaseCATran(tran, ref searchGraph);
                            }
                            break;
                        case GL.BatchModule.AR:
                            if (allowAR != true)
                            {
                                throw new PXException(Messages.ARDocumentsCanNotBeReleasedFromCAModule);
                            }
                            else
                            {
                                CATrxRelease.ReleaseCATran(tran, ref searchGraph);
                            }
                            break;
                        case GL.BatchModule.CA:
                            CATrxRelease.ReleaseCATran(tran, ref searchGraph);
                            break;
                        default:
                            throw new Exception(Messages.ThisDocTypeNotAvailableForRelease);
                    }
                    if (updateInfo == true)
                        listMessages.Add(tran.TranID.Value, new CAMessage(tran.TranID.Value, PXErrorLevel.RowInfo, ActionsMessages.RecordProcessed));
                }
                catch (Exception ex)
                {
                    allPassed = false;
                    if (updateInfo == true)
                        listMessages.Add(tran.TranID.Value, new CAMessage(tran.TranID.Value, PXErrorLevel.RowError, ex.Message));
                }
            }
			if (!allPassed)
			{
				throw new PXException(Messages.OneOrMoreItemsAreNotReleased);
			}			
		}

        public static void ReleaseCATran(CATran aTran, ref PXGraph aGraph) 
        {
            ReleaseCATran(aTran, ref aGraph, null);
        }
		public static void ReleaseCATran(CATran aTran, ref PXGraph aGraph, List<Batch> externalPostList)
		{
			int i = 0;
			if (aTran != null)
			{
				if (aGraph == null)
					aGraph = PXGraph.CreateInstance<CATranEntry>();
				PXGraph caGraph = aGraph;
				switch (aTran.OrigModule)
				{
					case GL.BatchModule.AP:
						List<APRegister> apList = new List<APRegister>();
						APRegister apReg = PXSelect<APRegister,
															Where<APRegister.docType, Equal<Required<APRegister.docType>>,
																And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>.
																Select(caGraph, aTran.OrigTranType, aTran.OrigRefNbr);
						if (apReg != null)
						{
							if (apReg.Hold == false)
							{
								if (apReg.Released == false)
								{
									apList.Add(apReg);
									APDocumentRelease.ReleaseDoc(apList, false, externalPostList);
								}
							}
							else
							{
								throw new PXException(Messages.DocumentStatusInvalid);
							}
						}
						else
							throw new Exception(Messages.DocNotFound);
						break;

					case GL.BatchModule.AR:
						List<ARRegister> arList = new List<ARRegister>();
						ARRegister arReg = PXSelect<ARRegister,
														Where<ARRegister.docType, Equal<Required<ARRegister.docType>>,
															And<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>>>>.
															Select(caGraph, aTran.OrigTranType, aTran.OrigRefNbr);
						if (arReg != null)
						{
							if (arReg.Hold == false)
							{
								if (arReg.Released == false)
								{
									arList.Add(arReg);
									ARDocumentRelease.ReleaseDoc(arList, false, externalPostList);
								}
							}
							else
							{
								throw new PXException(Messages.DocumentStatusInvalid);
							}
						}
						else
							throw new Exception(Messages.DocNotFound);
						break;

					case GL.BatchModule.CA:
						switch (aTran.OrigTranType)
						{
							case CAAPARTranType.CAAdjustment:
								CAAdj docAdj = PXSelect<CAAdj, Where<CAAdj.adjRefNbr, Equal<Required<CAAdj.adjRefNbr>>, And<CAAdj.adjTranType, Equal<Required<CAAdj.adjTranType>>>>>.Select(caGraph, aTran.OrigRefNbr, aTran.OrigTranType);
								if (docAdj != null)
								{
									if (docAdj.Hold == false)
									{
										if (docAdj.Released == false)
										{
											ReleaseDoc<CAAdj>(docAdj, i, externalPostList);
										}
									}
									else
									{
										throw new PXException(Messages.DocumentStatusInvalid);
									}
								}
								else
									throw new Exception(Messages.DocNotFound);
								break;
							case CAAPARTranType.CATransferIn:
							case CAAPARTranType.CATransferOut:
							case CAAPARTranType.CATransferExp:

								CATransfer docTransfer = PXSelect<CATransfer, Where<CATransfer.transferNbr, Equal<Required<CATransfer.transferNbr>>>>.Select(caGraph, aTran.OrigRefNbr);
								if (docTransfer != null)
								{
									if (docTransfer.Hold == false)
									{
										if (docTransfer.Released == false)
										{
											ReleaseDoc<CATransfer>(docTransfer, i, externalPostList);
										}
									}
									else
									{
										throw new PXException(Messages.DocumentStatusInvalid);
									}
								}
								else
									throw new Exception(Messages.DocNotFound);
								break;
							case CAAPARTranType.CADeposit:
							case CAAPARTranType.CAVoidDeposit:
								CADeposit docDeposit = PXSelect<CADeposit, Where<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>>>.Select(caGraph, aTran.OrigRefNbr);
								if (docDeposit != null)
								{
									if (docDeposit.Hold == false)
									{
										if (docDeposit.Released == false)
										{
											CADepositEntry.ReleaseDoc(docDeposit, externalPostList);
										}
									}
									else
									{
										throw new PXException(Messages.DocumentStatusInvalid);
									}
								}
								else
									throw new Exception(Messages.DocNotFound);
								break;
							default:
								throw new Exception(Messages.DocNotFound);
						}
						break;
					default:
						throw new Exception(Messages.ThisDocTypeNotAvailableForRelease);
				}
			}
		}

		public static void GroupRelease(List<CARegister> list, bool updateInfo)
		{			
            Dictionary<long, CAMessage> listMessages = new Dictionary<long, CAMessage>();
			if (updateInfo == true)
                PXLongOperation.SetCustomInfo(listMessages);


            CAReleaseProcess rgForCA = PXGraph.CreateInstance<CAReleaseProcess>();
            JournalEntry jeForCA = PXGraph.CreateInstance<JournalEntry>();
            jeForCA.FieldVerifying.AddHandler<GLTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
            jeForCA.FieldVerifying.AddHandler<GLTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			jeForCA.RowInserting.AddHandler<GLTran>((sender, e) =>
			{
				((GLTran)e.Row).ZeroPost = ((GLTran)e.Row).ZeroPost ?? true;
			});

            Dictionary<string, List<long>> caBatchMapping = CreateBatchMappingDictionary();
			HashSet<int> batchbind = new HashSet<int>();

			Exception exception = null;
			for (int i = 0; i < list.Count; i++)
			{
				CARegister caRegisterItem = list[i];
                if (caRegisterItem.TranID == null)
                {
                    throw new PXException(Messages.ErrorsProcessingEmptyLines);
                }
				if (caRegisterItem != null)
				{
					try
					{
						if (caRegisterItem.Released == false)
						{
							if ((bool)caRegisterItem.Hold)
							{
								throw new Exception(Messages.HoldDocCanNotBeRelease);
							}
							else
							{
								switch (caRegisterItem.Module)
								{
									case GL.BatchModule.AP:
										List<APRegister> apList = new List<APRegister>();
										APRegister apReg = PXSelect<APRegister,
																		Where<APRegister.docType, Equal<Required<APRegister.docType>>,
																			And<APRegister.refNbr, Equal<Required<APRegister.refNbr>>>>>.
																			Select(jeForCA, caRegisterItem.TranType, caRegisterItem.ReferenceNbr);
										if (apReg != null)
										{
											apList.Add(apReg);
											APDocumentRelease.ReleaseDoc(apList, false);			
										}
										else
											throw new Exception(Messages.TransactionNotComplete);
										break;
									case GL.BatchModule.AR:
										List<ARRegister> arList = new List<ARRegister>();
										ARRegister arReg = PXSelect<ARRegister,
																		Where<ARRegister.docType, Equal<Required<ARRegister.docType>>,
																			And<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>>>>.
																			Select(jeForCA, caRegisterItem.TranType, caRegisterItem.ReferenceNbr);
										if (arReg != null)
										{
											arList.Add(arReg);
											ARDocumentRelease.ReleaseDoc(arList, false);											
										}
										else
											throw new Exception(Messages.TransactionNotComplete);
										break;
									case GL.BatchModule.CA:
										switch (caRegisterItem.TranType)
										{
											case CAAPARTranType.CAAdjustment:
												CAAdj docAdj = PXSelect<CAAdj, 
													Where<CAAdj.adjRefNbr, Equal<Required<CAAdj.adjRefNbr>>, 
													And<CAAdj.adjTranType, Equal<Required<CAAdj.adjTranType>>>>>.Select(jeForCA, caRegisterItem.ReferenceNbr, caRegisterItem.TranType);
												if (docAdj != null)
												{
                                                    ReleaseAndRecordCADoc(jeForCA, rgForCA, docAdj, caRegisterItem.TranID.Value, caBatchMapping);
												}
												else
													throw new Exception(Messages.DocNotFound);
												break;
											case CAAPARTranType.CATransfer:
												CATransfer docTransfer = PXSelect<CATransfer, 
													Where<CATransfer.transferNbr, Equal<Required<CATransfer.transferNbr>>>>.Select(jeForCA, caRegisterItem.ReferenceNbr);
												if (docTransfer != null)
												{
                                                    ReleaseAndRecordCADoc(jeForCA, rgForCA, docTransfer, caRegisterItem.TranID.Value, caBatchMapping);
												}
												else
													throw new Exception(Messages.DocNotFound);
												break;

                                            case CAAPARTranType.CADeposit:
                                            case CAAPARTranType.CAVoidDeposit:
												CADeposit docDeposit = PXSelect<CADeposit, 
													Where<CADeposit.tranType, Equal<Required<CADeposit.tranType>>, 
													And<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>>>>.Select(jeForCA, caRegisterItem.TranType, caRegisterItem.ReferenceNbr);
                                                if (docDeposit != null)
                                                {
                                                    CADepositEntry.ReleaseDoc(docDeposit);
                                                }
                                                else
                                                    throw new Exception(Messages.DocNotFound);
                                                break;
											default:
												throw new Exception(Messages.DocNotFound);
										}
										break;
									default:
										throw new Exception(Messages.DocNotFound);
								}                                
								int k;
								if ((k = jeForCA.created.IndexOf(jeForCA.BatchModule.Current)) >= 0)
								{
									batchbind.Add(k);
								}
                                if (updateInfo == true && (caRegisterItem.Module != GL.BatchModule.CA || rgForCA.AutoPost == false))
                                {
                                    listMessages.Add(caRegisterItem.TranID.Value, new CAMessage(caRegisterItem.TranID.Value, PXErrorLevel.RowInfo, ActionsMessages.RecordProcessed));
							}
						}
						}
						else
						{
							throw new Exception(Messages.OriginalDocAlreadyReleased);
						}
					}
					catch (Exception e)
					{                        
                        if (updateInfo == true)
                        {
                            string message = e is PXOuterException ? (e.Message + " " + String.Join(" ", ((PXOuterException)e).InnerMessages)) : e.Message;
                            listMessages.Add(caRegisterItem.TranID.Value, new CAMessage(caRegisterItem.TranID.Value, PXErrorLevel.RowError, message));
                        }
						jeForCA.CleanupCreated(batchbind);
						jeForCA.Clear();
                        exception = e;
					}
				}
			}

            Exception caPostingException = null;
            if (rgForCA.AutoPost)
            {
                try
                {
                    PostCABatches(jeForCA.created.Where(b => b.Released == true), updateInfo ? caBatchMapping : CreateBatchMappingDictionary(), listMessages);
                }
				catch (Exception e)
                {
                    caPostingException = e;
                }
            }

			if (exception != null)
				if (list.Count == 1)
					throw exception;
				else
					throw new Exception(Messages.OneOrMoreItemsAreNotReleased);

            if (caPostingException != null)
                if (list.Count == 1)
                    throw caPostingException;
                else
                    throw new Exception(Messages.OneOrMoreItemsAreNotPosted);
		}

        private static void ReleaseAndRecordCADoc<TCADoc>(JournalEntry je, CAReleaseProcess rg, TCADoc doc, long tranID, Dictionary<string, List<long>> batchDocumentMapping)
            where TCADoc : class, ICADocument, new()
        {
            var ignoredBatchList = new List<Batch>();
            rg.Clear();
            var batches = rg.ReleaseDocProc(je, ref ignoredBatchList, doc);

            foreach (var batch in batches)
            {
                if (batchDocumentMapping.ContainsKey(batch.BatchNbr))
                {
                    batchDocumentMapping[batch.BatchNbr].Add(tranID);
                }
                else
                {
                    batchDocumentMapping[batch.BatchNbr] = new List<long> { tranID };
                }
            }

            if(batches.Any() == false)
            {
                batchDocumentMapping[""].Add(tranID);
            }
        }

        private static void PostCABatches(IEnumerable<Batch> batches, Dictionary<string, List<long>> batchMapping, Dictionary<long, CAMessage> listMessages)
        {
            Exception exception = null;

            PostGraph pg = PXGraph.CreateInstance<PostGraph>();

            foreach (var batch in batches)
            {
                try
                {
                    pg.Clear();
                    pg.TimeStamp = batch.tstamp;
                    pg.PostBatchProc(batch);

                    if (batchMapping.ContainsKey(batch.BatchNbr))
                    {
                        foreach (var tranID in batchMapping[batch.BatchNbr])
                        {
                            listMessages[tranID] = new CAMessage(tranID, PXErrorLevel.RowInfo, ActionsMessages.RecordProcessed);
                        }
                    }
                }
                catch (Exception e)
                {
                    exception = e;

                    if (batchMapping.ContainsKey(batch.BatchNbr))
                    {
                        string message = e is PXOuterException ? (e.Message + " " + String.Join(" ", ((PXOuterException)e).InnerMessages)) : e.Message;

                        foreach (var tranID in batchMapping[batch.BatchNbr])
                        {
                            listMessages[tranID] = new CAMessage(tranID, PXErrorLevel.RowError, message);
		}
                    }
                }
            }

            if(exception == null)
            {
                foreach (var tranID in batchMapping[""])
                {
                    listMessages[tranID] = new CAMessage(tranID, PXErrorLevel.RowInfo, ActionsMessages.RecordProcessed);
                }
            }
            else
                throw exception;
        }

        private static Dictionary<string, List<long>> CreateBatchMappingDictionary()
        {
            return new Dictionary<string, List<long>> { { "", new List<long>() } };
        }

		public static CARegister CARegister(CATran item)
		{
			CATranEntry caGraph = PXGraph.CreateInstance<CATranEntry>();

			switch (item.OrigModule)
			{
				case GL.BatchModule.AP:
					APPayment apPay = (APPayment)PXSelect<APPayment, Where<APPayment.cATranID, Equal<Required<APPayment.cATranID>>>>.
																							Select(caGraph, item.TranID);
					if (apPay != null)
					{
						return CARegister(apPay);
					}
					else
						throw new Exception(Messages.OrigDocCanNotBeFound);

				case GL.BatchModule.AR:
					ARPayment arPay = (ARPayment)PXSelect<ARPayment, Where<ARPayment.cATranID, Equal<Required<ARPayment.cATranID>>>>.
																							Select(caGraph, item.TranID);
					if (arPay != null)
					{
						return CARegister(arPay);
					}
					else
						throw new Exception(Messages.OrigDocCanNotBeFound);

				case GL.BatchModule.GL:
					GLTran gLTran = PXSelect<GLTran,
														 Where<GLTran.module, Equal<Required<GLTran.module>>,
															 And<GLTran.cATranID, Equal<Required<GLTran.cATranID>>>>>.
													Select(caGraph, item.OrigModule, item.TranID);
					if (gLTran != null)
					{
						CARegister reg = CARegister(gLTran);
                        int? cashAccountID;
                        if (GL.GLCashTranIDAttribute.CheckGLTranCashAcc(caGraph, gLTran,out cashAccountID) == true)
                        {
                            reg.CashAccountID = cashAccountID;
                            return reg;
                        }
                        else
                        {
							Branch branch = (Branch)PXSelectorAttribute.Select<GLTran.branchID>(caGraph.Caches[typeof(GLTran)], gLTran);
							Account account = (Account)PXSelectorAttribute.Select<GLTran.accountID>(caGraph.Caches[typeof(GLTran)], gLTran);
							Sub sub = (Sub)PXSelectorAttribute.Select<GLTran.subID>(caGraph.Caches[typeof(GLTran)], gLTran);
							throw new PXException(GL.Messages.CashAccountDoesNotExist, branch.BranchCD, account.AccountCD, sub.SubCD);
                        }
					}
					else
						throw new Exception(Messages.OrigDocCanNotBeFound);

				case GL.BatchModule.CA:
					switch (item.OrigTranType)
					{
						case CAAPARTranType.CAAdjustment:
							CAAdj docAdj = PXSelect<CAAdj, Where<CAAdj.tranID, Equal<Required<CAAdj.tranID>>>>.Select(caGraph, item.TranID);
							if (docAdj != null)
							{
								return CARegister(docAdj);
							}
							else
								throw new Exception(Messages.OrigDocCanNotBeFound);
						case CAAPARTranType.CATransferIn:
							CATransfer docTransferIn = PXSelect<CATransfer, Where<CATransfer.tranIDIn, Equal<Required<CATransfer.tranIDIn>>>>
																			.Select(caGraph, item.TranID);
							if (docTransferIn != null)
							{
								return CARegister(docTransferIn, item);
							}
							else
								throw new Exception(Messages.OrigDocCanNotBeFound);
						case CAAPARTranType.CATransferOut:
							CATransfer docTransferOut = PXSelect<CATransfer, Where<CATransfer.tranIDOut, Equal<Required<CATransfer.tranIDOut>>>>
																			.Select(caGraph, item.TranID);
							if (docTransferOut != null)
							{
								return CARegister(docTransferOut, item);
							}
							else
								throw new Exception(Messages.OrigDocCanNotBeFound);
						default:
							throw new Exception(Messages.ThisCATranOrigDocTypeNotDefined);
					}
				default:
					throw new Exception(Messages.ThisCATranOrigDocTypeNotDefined);
			}
			throw new Exception(Messages.ThisCATranOrigDocTypeNotDefined);
		}
        public static CARegister CARegister(CADeposit item)
        {
            CARegister ret = new CARegister();
            ret.TranID = item.TranID;
            ret.Hold = item.Hold;
            ret.Released = item.Released;
            ret.Module = GL.BatchModule.CA;
            ret.TranType = item.TranType;
            ret.Description = item.TranDesc;
            ret.FinPeriodID = item.FinPeriodID;
            ret.DocDate = item.TranDate;
            ret.ReferenceNbr = item.RefNbr;
            ret.NoteID = item.NoteID;
            ret.CashAccountID = item.CashAccountID;
            ret.CuryID = item.CuryID;
            ret.TranAmt = item.TranAmt;
            ret.CuryTranAmt = item.CuryTranAmt;

            return ret;
        }
		public static CARegister CARegister(CAAdj item)
		{
			CARegister ret = new CARegister();
			ret.TranID = item.TranID;
			ret.Hold = item.Hold;
			ret.Released = item.Released;
			ret.Module = GL.BatchModule.CA;
			ret.TranType = item.AdjTranType;
			ret.Description = item.TranDesc;
			ret.FinPeriodID = item.FinPeriodID;
			ret.DocDate = item.TranDate;
			ret.ReferenceNbr = item.AdjRefNbr;
			ret.NoteID = item.NoteID;
			ret.CashAccountID = item.CashAccountID;
			ret.CuryID = item.CuryID;
			ret.TranAmt = item.TranAmt;
			ret.CuryTranAmt = item.CuryTranAmt;

			return ret;
		}

		public static CARegister CARegister(GLTran item)
		{
			CARegister ret = new CARegister();
			ret.TranID = item.CATranID;
			ret.Hold = (item.Released != true);
			ret.Released = item.Released;
			ret.Module = GL.BatchModule.GL;
			ret.TranType = item.TranType;
			ret.Description = item.TranDesc;
			ret.FinPeriodID = item.FinPeriodID;
			ret.DocDate = item.TranDate;
			ret.ReferenceNbr = item.RefNbr;
			ret.NoteID = item.NoteID;
			ret.CashAccountID = item.AccountID;
			//ret.CuryID        = item.;
			ret.TranAmt = item.DebitAmt - item.CreditAmt;
			ret.CuryTranAmt = item.CuryDebitAmt - item.CuryCreditAmt;

			return ret;
		}

		public static CARegister CARegister(ARPayment item)
		{
			CARegister ret = new CARegister();
			ret.TranID = item.CATranID;
			ret.Hold = item.Hold;
			ret.Released = item.Released;
			ret.Module = GL.BatchModule.AR;
			ret.TranType = item.DocType;
			ret.Description = item.DocDesc;
			ret.FinPeriodID = item.FinPeriodID;
			ret.DocDate = item.DocDate;
			ret.ReferenceNbr = item.RefNbr;
			ret.NoteID = item.NoteID;
			ret.CashAccountID = item.CashAccountID;
			ret.CuryID = item.CuryID;
			ret.TranAmt = item.DocBal;
			ret.CuryTranAmt = item.DocBal;

			return ret;
		}

		public static CARegister CARegister(APPayment item)
		{
			CARegister ret = new CARegister();
			ret.TranID = item.CATranID;
			ret.Hold = item.Hold;
			ret.Released = item.Released;
			ret.Module = GL.BatchModule.AP;
			ret.TranType = item.DocType;
			ret.Description = item.DocDesc;
			ret.FinPeriodID = item.FinPeriodID;
			ret.DocDate = item.DocDate;
			ret.ReferenceNbr = item.RefNbr;
			ret.NoteID = item.NoteID;
			ret.CashAccountID = item.CashAccountID;
			ret.CuryID = item.CuryID;
			ret.TranAmt = item.DocBal;
			ret.CuryTranAmt = item.DocBal;

			return ret;
		}

		public static CARegister CARegister(CATransfer item, CATran tran)
		{
			CARegister ret = new CARegister();
			ret.TranID = tran.TranID;
			ret.Hold = item.Hold;
			ret.Released = item.Released;
			ret.Module = GL.BatchModule.CA;
			ret.TranType = CAAPARTranType.CATransfer;
			ret.Description = item.Descr;
			ret.FinPeriodID = tran.FinPeriodID;
			ret.DocDate = item.OutDate;
			ret.ReferenceNbr = item.TransferNbr;
			ret.TranType = CAAPARTranType.CATransfer;
			ret.NoteID = item.NoteID;

            ret.CashAccountID = tran.CashAccountID;
            ret.CuryID = tran.CuryID;
            ret.CuryTranAmt = tran.CuryTranAmt;
            ret.TranAmt = tran.TranAmt;
			return ret;
		}

		public static void ReleaseDoc<TCADocument>(TCADocument _doc, int _item, List<Batch> externalPostList)
			where TCADocument : class, ICADocument, new()
		{
			CAReleaseProcess rg = PXGraph.CreateInstance<CAReleaseProcess>();
			JournalEntry je = PXGraph.CreateInstance<JournalEntry>();
			je.FieldVerifying.AddHandler<GLTran.projectID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			je.FieldVerifying.AddHandler<GLTran.taskID>((PXCache sender, PXFieldVerifyingEventArgs e) => { e.Cancel = true; });
			

			bool skipPost = (externalPostList != null);
			List<Batch> batchlist = new List<Batch>();
			List<int> batchbind = new List<int>();

			bool failed = false;
			rg.Clear();
			rg.ReleaseDocProc(je, ref batchlist, _doc);

			for (int i = batchbind.Count; i < batchlist.Count; i++)
			{
				batchbind.Add(i);
			}
			if (skipPost)
			{
				if (rg.AutoPost)
					externalPostList.AddRange(batchlist);
			}
			else
			{
				PostGraph pg = PXGraph.CreateInstance<PostGraph>();
				for (int i = 0; i < batchlist.Count; i++)
				{
					Batch batch = batchlist[i];
					try
					{
						if (rg.AutoPost)
						{
							pg.Clear();
							pg.TimeStamp = batch.tstamp;
							pg.PostBatchProc(batch);
						}
					}
					catch (Exception e)
					{
						throw new AP.PXMassProcessException(batchbind[i], e);
					}
				}
			}
			if (failed)
			{
				throw new PXException(GL.Messages.DocumentsNotReleased);
			}
		}
		#endregion
	}

	public interface ICADocument
	{
		string DocType
		{
			get;
		}
		string RefNbr
		{
			get;
		}
		Boolean? Released
		{
			get;
			set;
		}
		Boolean? Hold
		{
			get;
			set;
		}
	}

	[PXHidden()]
	public class CAReleaseProcess : PXGraph<CAReleaseProcess>
	{
		public PXSetup<CASetup> casetup;

		public PXSelectJoin<CATran, InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CATran.cashAccountID>>, 
            InnerJoin<Currency, On<Currency.curyID, Equal<CashAccount.curyID>>, 
            InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<CATran.curyInfoID>>, 
            LeftJoin<CAAdj, On<CAAdj.tranID, Equal<CATran.tranID>>>>>>,
            Where<CATran.origModule, Equal<BatchModule.moduleCA>, 
                And<CATran.origTranType, Like<Required<CATran.origTranType>>, 
                And<CATran.origRefNbr, Equal<Required<CATran.origRefNbr>>, 
                And<CATran.released, Equal<boolFalse>>>>>, OrderBy<Asc<CATran.tranID>>> CATran_Ordered;

        public PXSelect<CASplit,
                Where<CASplit.adjTranType, Equal<Required<CASplit.adjTranType>>,
                    And<CASplit.adjRefNbr, Equal<Required<CASplit.adjRefNbr>>>>> CASplits;

		public PXSelectJoin<CATaxTran, InnerJoin<Tax, On<Tax.taxID, Equal<CATaxTran.taxID>>>, Where<CATaxTran.module, Equal<BatchModule.moduleCA>, And<CATaxTran.tranType, Equal<Required<CATaxTran.tranType>>, And<CATaxTran.refNbr, Equal<Required<CATaxTran.refNbr>>>>>, OrderBy<Asc<Tax.taxCalcLevel>>> CATaxTran_TranType_RefNbr;
		public PXSelect<SVATConversionHist> SVATConversionHistory;
		public PXSelect<CADepositEntry.ARPaymentUpdate> arDocs;
		public PXSelect<CADepositEntry.APPaymentUpdate> apDocs;
		public PXSelect<CADepositDetail> depositDetails;
		public PXSelect<CADeposit> deposit;
		public PXSetup<GLSetup> glsetup;

		public bool? RequireControlTaxTotal
		{
			get { return casetup.Current.RequireControlTaxTotal == true && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>(); }
		}

		public decimal? RoundingLimit
		{
			get { return glsetup.Current.RoundingLimit; }
		}



		public bool AutoPost
		{
			get
			{
				return (bool)casetup.Current.AutoPostOption;
			}
		}

		public CAReleaseProcess()
		{
            bool requireExtRefNbr = casetup.Current.RequireExtRefNbr == true;

            PXUIFieldAttribute.SetRequired<CAAdj.extRefNbr>(Caches[typeof(CAAdj)],requireExtRefNbr);
            PXDefaultAttribute.SetPersistingCheck<CAAdj.extRefNbr>(Caches[typeof(CAAdj)], null, requireExtRefNbr ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);

			PXUIFieldAttribute.SetRequired<CADeposit.extRefNbr>(Caches[typeof(CADeposit)], requireExtRefNbr);
			PXDefaultAttribute.SetPersistingCheck<CAAdj.extRefNbr>(Caches[typeof(CADeposit)], null, requireExtRefNbr ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing);
		}


        public virtual void SegregateBatch(JournalEntry je, List<Batch> batchlist, Int32? branchID, string curyID, DateTime? docDate, string finPeriodID, string description, CurrencyInfo curyInfo)
        {
            var batch = je.BatchModule.Current;
            
            if (batch != null)
            {
                je.Save.Press();
                if (batchlist.Contains(batch) == false)
                {
                    batchlist.Add(batch);
                }
            }

            JournalEntry.SegregateBatch(je, BatchModule.CA, branchID, curyID, docDate, finPeriodID, description, curyInfo, null);
        }

		public IEnumerable<Batch> ReleaseDocProc<TCADocument>(JournalEntry je, ref List<Batch> batchlist, TCADocument doc)
			where TCADocument : class, ICADocument, new()
		{
			if ((bool)doc.Hold)
			{
				throw new PXException(Messages.HoldDocCanNotBeRelease);
			}
            var batches = new List<Batch>();
			var caTranIDs = new HashSet<long>();
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				GLTran rgolTranData = new GLTran();
				rgolTranData.DebitAmt = 0m;
				rgolTranData.CreditAmt = 0m;
				Currency rgol_cury = null;
				CurrencyInfo rgol_info = null;
				CurrencyInfo transit_info = null;

				CATran prev_tran = null;

				if (casetup.Current == null || casetup.Current.TransitAcctId == null || casetup.Current.TransitSubID == null)
				{
					throw new PXException(Messages.AbsenceCashTransitAccount);
				}
				Batch batch = null;

				Batch transferBatch = null;
				CATran transferTran = null;

				int? SourceBranchID = null;
				foreach (PXResult<CATran, CashAccount, Currency, CurrencyInfo, CAAdj> res in CATran_Ordered.Select(doc.DocType, doc.RefNbr))
				{
					CATran catran = (CATran)res;
					CashAccount cashacct = (CashAccount)res;
					Currency cury = (Currency)res;
					CurrencyInfo info = (CurrencyInfo)res;
					CAAdj caadj = (CAAdj)res;
					if (caadj != null && caadj.TranID != null && caadj.Hold == true)
					{
						throw new PXException(Messages.HoldDocCanNotBeRelease);
					}
					if (SourceBranchID == null && catran.OrigTranType != CAAPARTranType.CATransferIn) // Type of first transaction CATransferOut or CAAdjust expected
					{
						SourceBranchID = cashacct.BranchID;
					}

					var branchID = SourceBranchID ?? cashacct.BranchID;

					bool shouldSegregate = true;

					bool isTransfer = catran.OrigTranType == CAAPARTranType.CATransferIn || catran.OrigTranType == CAAPARTranType.CATransferOut;

					if (isTransfer && transferTran != null &&
						branchID == transferBatch.BranchID &&
						catran.CuryID == transferTran.CuryID &&
						catran.FinPeriodID == transferTran.FinPeriodID &&
						catran.TranDate == transferTran.TranDate)
					{
						je.BatchModule.Current = transferBatch;
						shouldSegregate = false;
					}

					if (shouldSegregate)
					{
						SegregateBatch(je, batchlist, branchID, catran.CuryID, catran.TranDate, catran.FinPeriodID, catran.TranDesc, info);
					}

					batch = (Batch)je.BatchModule.Current;

					var splits = CASplits.Select(caadj.AdjTranType, caadj.AdjRefNbr).RowCast<CASplit>().ToList();

					if (splits.Any() == false)
					{
						var casplit = new CASplit();

						casplit.AdjTranType = catran.OrigTranType;
						casplit.CuryInfoID = catran.CuryInfoID;
						casplit.CuryTranAmt = (catran.DrCr == CADrCr.CADebit ? catran.CuryTranAmt : -1m * catran.CuryTranAmt);
						casplit.TranAmt = (catran.DrCr == CADrCr.CADebit ? catran.TranAmt : -1m * catran.TranAmt);
						casplit.TranDesc = PXMessages.LocalizeFormatNoPrefix(Messages.Offset);
						
						casplit.AccountID = casetup.Current.TransitAcctId;
						casplit.SubID = casetup.Current.TransitSubID;
						casplit.ReclassificationProhibited = true;
						casplit.ReferenceID = catran.ReferenceID;
						casplit.BranchID = SourceBranchID ?? cashacct.BranchID;

						switch (casplit.AdjTranType)
						{
							case CAAPARTranType.CATransferOut:
								transit_info = PXCache<CurrencyInfo>.CreateCopy(info);
								transit_info.CuryInfoID = null;
								transit_info = je.currencyinfo.Insert(transit_info);
								transit_info.BaseCalc = false;

								casplit.CuryInfoID = transit_info.CuryInfoID;
								break;
							case CAAPARTranType.CATransferIn:
								rgol_cury = cury;
								rgol_info = info;
								rgolTranData.FinPeriodID = catran.FinPeriodID;
								rgolTranData.TranPeriodID = catran.TranPeriodID;
								rgolTranData.TranDate = catran.TranDate;
								rgolTranData.BranchID = cashacct.BranchID;
								rgolTranData.TranDesc = catran.TranDesc;

								if (string.Equals(info.CuryID, transit_info.CuryID))
								{
									casplit.CuryInfoID = transit_info.CuryInfoID;
								}
								break;
							default:
								throw new PXException(Messages.ThisDocTypeNotAvailableForRelease);
						}

						rgolTranData.DebitAmt += (catran.DrCr == DrCr.Debit ? 0m : casplit.TranAmt);
						rgolTranData.CreditAmt += (catran.DrCr == DrCr.Debit ? casplit.TranAmt : 0m);

						splits.Add(casplit);
					}

					if (object.Equals(prev_tran, catran) == false)
					{
						GLTran documentTran = new GLTran();
						documentTran.SummPost = false;
						documentTran.CuryInfoID = catran.CuryInfoID;
						documentTran.TranType = catran.OrigTranType;
						documentTran.RefNbr = catran.OrigRefNbr;
						documentTran.ReferenceID = catran.ReferenceID;
						documentTran.AccountID = cashacct.AccountID;
						documentTran.SubID = cashacct.SubID;
						documentTran.CATranID = catran.TranID;
						documentTran.TranDate = catran.TranDate;
						documentTran.FinPeriodID = catran.FinPeriodID;
						documentTran.TranPeriodID = catran.TranPeriodID;
						documentTran.CuryDebitAmt = (catran.DrCr == DrCr.Debit ? catran.CuryTranAmt : 0m);
						documentTran.DebitAmt = (catran.DrCr == DrCr.Debit ? catran.TranAmt : 0m);
						documentTran.CuryCreditAmt = (catran.DrCr == DrCr.Debit ? 0m : -1m * catran.CuryTranAmt);
						documentTran.CreditAmt = (catran.DrCr == DrCr.Debit ? 0m : -1m * catran.TranAmt);
						documentTran.TranDesc = catran.TranDesc;
						documentTran.Released = true;
						documentTran.BranchID = cashacct.BranchID;
						documentTran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
						je.GLTranModuleBatNbr.Insert(documentTran);

						foreach (PXResult<CATaxTran, Tax> r in CATaxTran_TranType_RefNbr.Select(caadj.AdjTranType, caadj.AdjRefNbr))
						{
							CATaxTran x = (CATaxTran)r;
							Tax salestax = (Tax)r;

							if (salestax.TaxType == CSTaxType.Withholding)
							{
								continue;
							}

							if (salestax.ReverseTax != true)
							{
								GLTran taxTran = new GLTran();
								taxTran.SummPost = false;
								taxTran.CuryInfoID = catran.CuryInfoID;
								taxTran.TranType = caadj.AdjTranType;
								taxTran.TranClass = GLTran.tranClass.Tax;
								taxTran.RefNbr = caadj.AdjRefNbr;
								taxTran.TranDate = caadj.TranDate;
								taxTran.AccountID = (salestax.TaxType == CSTaxType.Use) ? salestax.ExpenseAccountID : x.AccountID;
								taxTran.SubID = (salestax.TaxType == CSTaxType.Use) ? salestax.ExpenseSubID : x.SubID;
								taxTran.TranDesc = salestax.TaxID;
								taxTran.CuryDebitAmt = (caadj.DrCr == DrCr.Credit) ? x.CuryTaxAmt : 0m;
								taxTran.DebitAmt = (caadj.DrCr == DrCr.Credit) ? x.TaxAmt : 0m;
								taxTran.CuryCreditAmt = (caadj.DrCr == DrCr.Credit) ? 0m : x.CuryTaxAmt;
								taxTran.CreditAmt = (caadj.DrCr == DrCr.Credit) ? 0m : x.TaxAmt;
								taxTran.Released = true;
								taxTran.ReferenceID = null;
								taxTran.BranchID = caadj.BranchID;
								taxTran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
								je.GLTranModuleBatNbr.Insert(taxTran);
							}

							if (salestax.TaxType == CSTaxType.Use || (bool)salestax.ReverseTax)
							{
								GLTran taxTran = new GLTran();
								taxTran.SummPost = false;
								taxTran.CuryInfoID = catran.CuryInfoID;
								taxTran.TranType = caadj.AdjTranType;
								taxTran.TranClass = GLTran.tranClass.Tax;
								taxTran.RefNbr = caadj.AdjRefNbr;
								taxTran.TranDate = caadj.TranDate;
								taxTran.AccountID = x.AccountID;
								taxTran.SubID = x.SubID;
								taxTran.TranDesc = salestax.TaxID;
								taxTran.CuryDebitAmt = (caadj.DrCr == DrCr.Credit) ? 0m : x.CuryTaxAmt;
								taxTran.DebitAmt = (caadj.DrCr == DrCr.Credit) ? 0m : x.TaxAmt;
								taxTran.CuryCreditAmt = (caadj.DrCr == DrCr.Credit) ? x.CuryTaxAmt : 0m;
								taxTran.CreditAmt = (caadj.DrCr == DrCr.Credit) ? x.TaxAmt : 0m;
								taxTran.Released = true;
								taxTran.ReferenceID = null;
								taxTran.BranchID = caadj.BranchID;
								taxTran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
								je.GLTranModuleBatNbr.Insert(taxTran);
							}
							if (salestax.DeductibleVAT == true)
							{
								if (salestax.ReportExpenseToSingleAccount == true)
								{

									GLTran tran = new GLTran();
									tran.SummPost = false;
									tran.BranchID = caadj.BranchID;
									tran.CuryInfoID = catran.CuryInfoID;
									tran.TranType = caadj.AdjTranType;
									tran.TranClass = GLTran.tranClass.Tax;
									tran.RefNbr = caadj.AdjRefNbr;
									tran.TranDate = caadj.TranDate;
									tran.AccountID = salestax.ExpenseAccountID;
									tran.SubID = salestax.ExpenseSubID;
									tran.TranDesc = salestax.TaxID;
									bool CR = (caadj.DrCr == DrCr.Credit);
									tran.CuryDebitAmt = CR ? x.CuryExpenseAmt : 0m;
									tran.DebitAmt = CR ? x.ExpenseAmt : 0m;
									tran.CuryCreditAmt = CR ? 0m : x.CuryExpenseAmt;
									tran.CreditAmt = CR ? 0m : x.ExpenseAmt;
									tran.Released = true;
									tran.ReferenceID = null;
									tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

									je.GLTranModuleBatNbr.Insert(tran);
								}
								else if (salestax.TaxCalcType == CSTaxCalcType.Item)
								{
									PXResultset<CATax> deductibleLines = PXSelectJoin<CATax, InnerJoin<CASplit,
										On<CATax.adjTranType, Equal<CASplit.adjTranType>,
											And<CATax.adjRefNbr, Equal<CASplit.adjRefNbr>, And<CATax.lineNbr, Equal<CASplit.lineNbr>>>>>,
										Where<CATax.taxID, Equal<Required<CATax.taxID>>, And<CASplit.adjTranType, Equal<Required<CASplit.adjTranType>>,
											And<CASplit.adjRefNbr, Equal<Required<CASplit.adjRefNbr>>>>>,
										OrderBy<Desc<CATax.curyTaxAmt>>>.Select(this, salestax.TaxID, x.TranType, x.RefNbr);

									APTaxAttribute apTaxAttr = new APTaxAttribute(typeof(CARegister), typeof(CATax), typeof(CATaxTran));
									apTaxAttr.DistributeExpenseDiscrepancy(this, deductibleLines.FirstTableItems, x.CuryExpenseAmt.Value);

									foreach (PXResult<CATax, CASplit> item in deductibleLines)
									{
										CATax taxLine = (CATax)item;
										CASplit split = (CASplit)item;

										GLTran tran = new GLTran();
										tran.SummPost = false;
										tran.BranchID = split.BranchID;
										tran.CuryInfoID = catran.CuryInfoID;
										tran.TranType = caadj.AdjTranType;
										tran.TranClass = GLTran.tranClass.Tax;
										tran.RefNbr = caadj.AdjRefNbr;
										tran.TranDate = caadj.TranDate;
										tran.AccountID = split.AccountID;
										tran.SubID = split.SubID;
										tran.TranDesc = salestax.TaxID;
										tran.TranLineNbr = split.LineNbr;
										bool CR = (caadj.DrCr == DrCr.Credit);
										tran.CuryDebitAmt = CR ? taxLine.CuryExpenseAmt : 0m;
										tran.DebitAmt = CR ? taxLine.ExpenseAmt : 0m;
										tran.CuryCreditAmt = CR ? 0m : taxLine.CuryExpenseAmt;
										tran.CreditAmt = CR ? 0m : taxLine.ExpenseAmt;
										tran.Released = true;
										tran.ReferenceID = null;
										tran.ProjectID = split.ProjectID;
										tran.TaskID = split.TaskID;

										je.GLTranModuleBatNbr.Insert(tran);
									}
								}
							}

							x.Released = true;
							CATaxTran_TranType_RefNbr.Update(x);

							if (PXAccess.FeatureInstalled<FeaturesSet.vATReporting>() &&
								(x.TaxType == TaxType.PendingPurchase || x.TaxType == TaxType.PendingSales))
							{
								decimal mult = ReportTaxProcess.GetMultByTranType(BatchModule.CA, x.TranType);
								SVATConversionHist histSVAT = new SVATConversionHist
								{
									Module = BatchModule.CA,
									AdjdBranchID = x.BranchID,
									AdjdDocType = x.TranType,
									AdjdRefNbr = x.RefNbr,
									AdjgDocType = x.TranType,
									AdjgRefNbr = x.RefNbr,
									AdjdDocDate = caadj.DocDate,
									AdjdFinPeriodID = caadj.FinPeriodID,

									TaxID = x.TaxID,
									TaxType = x.TaxType,
									TaxRate = x.TaxRate,
									VendorID = x.VendorID,
									ReversalMethod = SVATTaxReversalMethods.OnDocuments,

									CuryInfoID = x.CuryInfoID,
									CuryTaxableAmt = x.CuryTaxableAmt * mult,
									CuryTaxAmt = x.CuryTaxAmt * mult,
									CuryUnrecognizedTaxAmt = x.CuryTaxAmt * mult
								};

								histSVAT.FillBaseAmounts(SVATConversionHistory.Cache);
								SVATConversionHistory.Cache.Insert(histSVAT);
							}
						}
					}

					foreach (var casplit in splits)
					{
						PXResultset<CATax> taxes = PXSelectJoin<CATax, LeftJoin<Tax, On<Tax.taxID, Equal<CATax.taxID>>>,
							Where<CATax.adjTranType, Equal<Required<CATax.adjTranType>>,
								And<CATax.adjRefNbr, Equal<Required<CATax.adjRefNbr>>,
									And<CATax.lineNbr, Equal<Required<CATax.lineNbr>>>>>>.Select(this, casplit.AdjTranType, casplit.AdjRefNbr, casplit.LineNbr);
						//sorting on joined tables' fields does not work!
						taxes.Sort((PXResult<CATax> x, PXResult<CATax> y) =>
						{
							Tax taxX = x.GetItem<Tax>();
							Tax taxY = y.GetItem<Tax>();
							if (taxX.TaxCalcLevel == taxY.TaxCalcLevel)
							{
								if (taxX.TaxType == taxY.TaxType)
								{
									return 0;
								}
								else
								{
									return String.Compare(taxX.TaxType, taxY.TaxType);
								}
							}
							else
							{
								return Int32.Parse(taxX.TaxCalcLevel) - Int32.Parse(taxY.TaxCalcLevel);
							}
						});
						Tax firstTax = taxes.Count != 0 ? taxes[0].GetItem<Tax>() : null;
						if (firstTax != null && firstTax.TaxCalcType == CSTaxCalcType.Item && PXAccess.FeatureInstalled<FeaturesSet.netGrossEntryMode>())
						{
							string TaxCalcMode = caadj.TaxCalcMode;
							switch (TaxCalcMode)
							{
								case VendorClass.taxCalcMode.Gross:
									firstTax.TaxCalcLevel = CSTaxCalcLevel.Inclusive;
									break;
								case VendorClass.taxCalcMode.Net:
									firstTax.TaxCalcLevel = CSTaxCalcLevel.CalcOnItemAmt;
									break;
								case VendorClass.taxCalcMode.TaxSetting:
									break;
							}
						}
						GLTran splitTran = new GLTran();
						splitTran.SummPost = (catran.OrigTranType == CATranType.CATransferIn || catran.OrigTranType == CATranType.CATransferOut);
						splitTran.ZeroPost = (catran.OrigTranType == CATranType.CATransferIn || catran.OrigTranType == CATranType.CATransferOut) ? (bool?)false : null;
						splitTran.CuryInfoID = casplit.CuryInfoID;
						splitTran.TranType = catran.OrigTranType;
						splitTran.RefNbr = catran.OrigRefNbr;

						splitTran.InventoryID = casplit.InventoryID;
						splitTran.UOM = casplit.UOM;
						splitTran.Qty = casplit.Qty;
						splitTran.ReferenceID = casplit.ReferenceID;
						splitTran.AccountID = casplit.AccountID;
						splitTran.SubID = casplit.SubID;
						splitTran.ReclassificationProhibited = casplit.ReclassificationProhibited ?? false;
						splitTran.CATranID = null;
						splitTran.TranDate = catran.TranDate;
						splitTran.FinPeriodID = catran.FinPeriodID;
						splitTran.TranPeriodID = catran.TranPeriodID;
						splitTran.BranchID = casplit.BranchID;
						splitTran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
						if (firstTax != null && firstTax.TaxCalcLevel == CSTaxCalcLevel.Inclusive && firstTax.TaxType != CSTaxType.Withholding)
						{
                            splitTran.CuryDebitAmt = (catran.DrCr == DrCr.Debit ? 0m : casplit.CuryTaxableAmt);
                            splitTran.DebitAmt = (catran.DrCr == DrCr.Debit ? 0m : casplit.TaxableAmt);
                            splitTran.CuryCreditAmt = (catran.DrCr == DrCr.Debit ? casplit.CuryTaxableAmt : 0m);
                            splitTran.CreditAmt = (catran.DrCr == DrCr.Debit ? casplit.TaxableAmt : 0m);
						}
						else
						{
                            splitTran.CuryDebitAmt = (catran.DrCr == DrCr.Debit ? 0m : casplit.CuryTranAmt);
                            splitTran.DebitAmt = (catran.DrCr == DrCr.Debit ? 0m : casplit.TranAmt);
                            splitTran.CuryCreditAmt = (catran.DrCr == DrCr.Debit ? casplit.CuryTranAmt : 0m);
                            splitTran.CreditAmt = (catran.DrCr == DrCr.Debit ? casplit.TranAmt : 0m);
						}
						splitTran.TranDesc = casplit.TranDesc;
						splitTran.ProjectID = casplit.ProjectID;
						splitTran.TaskID = casplit.TaskID;
						splitTran.TranLineNbr = splitTran.SummPost == true ? null : casplit.LineNbr;
						splitTran.NonBillable = casplit.NonBillable;
						splitTran.Released = true;
						je.GLTranModuleBatNbr.Insert(splitTran);
					}

					prev_tran = catran;

					if (rgol_cury != null && rgol_info != null && Math.Abs(Math.Round((decimal)(rgolTranData.DebitAmt - rgolTranData.CreditAmt), 4)) >= 0.00005m)
					{
						batch = (Batch)je.BatchModule.Current;

						CurrencyInfo new_info = PXCache<CurrencyInfo>.CreateCopy(rgol_info);
						new_info.CuryInfoID = null;
						new_info = je.currencyinfo.Insert(new_info);

						GLTran rgolTran = new GLTran();
						rgolTran.SummPost = false;
						if (Math.Sign((decimal)(rgolTranData.DebitAmt - rgolTranData.CreditAmt)) == 1)
						{
							rgolTran.AccountID = rgol_cury.RealLossAcctID;
							rgolTran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.realLossSubID>(je, rgolTranData.BranchID, rgol_cury);
							rgolTran.DebitAmt = Math.Round((decimal)(rgolTranData.DebitAmt - rgolTranData.CreditAmt), 4);
							rgolTran.CuryDebitAmt = object.Equals(new_info.CuryID, new_info.BaseCuryID) ? rgolTran.DebitAmt : 0m; //non-zero for base cury
							rgolTran.CreditAmt = 0m;
							rgolTran.CuryCreditAmt = 0m;
						}
						else
						{
							rgolTran.AccountID = rgol_cury.RealGainAcctID;
							rgolTran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.realGainSubID>(je, rgolTranData.BranchID, rgol_cury);
							rgolTran.DebitAmt = 0m;
							rgolTran.CuryDebitAmt = 0m;
							rgolTran.CreditAmt = Math.Round((decimal)(rgolTranData.CreditAmt - rgolTranData.DebitAmt), 4);
							rgolTran.CuryCreditAmt = object.Equals(new_info.CuryID, new_info.BaseCuryID) ? rgolTran.CreditAmt : 0m; //non-zero for base cury
						}
						rgolTran.TranType = CATranType.CATransferRGOL;
						rgolTran.RefNbr = doc.RefNbr;
						rgolTran.TranDesc = "RGOL";
						rgolTran.TranDate = rgolTranData.TranDate;
						rgolTran.FinPeriodID = rgolTranData.FinPeriodID;
						rgolTran.TranPeriodID = rgolTranData.TranPeriodID;
						rgolTran.Released = true;
						rgolTran.CuryInfoID = new_info.CuryInfoID;
						rgolTran.BranchID = rgolTranData.BranchID;
						rgolTran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
						je.GLTranModuleBatNbr.Insert(rgolTran);

						rgolTran.AccountID = casetup.Current.TransitAcctId;
						rgolTran.SubID = casetup.Current.TransitSubID;
						rgolTran.BranchID = SourceBranchID ?? cashacct.BranchID;

						decimal? CuryAmount = rgolTran.CuryDebitAmt;
						decimal? BaseAmount = rgolTran.DebitAmt;
						rgolTran.CuryDebitAmt = rgolTran.CuryCreditAmt;
						rgolTran.DebitAmt = rgolTran.CreditAmt;
						rgolTran.CuryCreditAmt = CuryAmount;
						rgolTran.CreditAmt = BaseAmount;

						je.GLTranModuleBatNbr.Insert(rgolTran);

						rgolTranData = new GLTran();
						rgolTranData.DebitAmt = 0m;
						rgolTranData.CreditAmt = 0m;
						rgol_cury = null;
						rgol_info = null;
					}

					if (caadj.AdjTranType == CATranType.CAAdjustment && Math.Abs(Math.Round((decimal)(batch.CuryDebitTotal - batch.CuryCreditTotal), 4)) >= 0.00005m)
					{
						if (this.RequireControlTaxTotal != true)
						{
							throw new PXException(Messages.DocumentOutOfBalance);
						}
						decimal roundDiff = Math.Abs(Math.Round((decimal)(batch.DebitTotal - batch.CreditTotal), 4));
						if (roundDiff > this.RoundingLimit)
						{
							throw new PXException(AP.Messages.RoundingAmountTooBig, je.currencyinfo.Current.BaseCuryID, roundDiff,
								PXDBQuantityAttribute.Round(this.RoundingLimit));
						}
						GLTran tran = new GLTran();
						tran.SummPost = true;
						tran.BranchID = caadj.BranchID;
						Currency c = PXSelect<Currency, Where<Currency.curyID, Equal<Required<Currency.curyID>>>>.Select(this, caadj.CuryID);

						if (c.RoundingGainAcctID == null || c.RoundingGainSubID == null)
						{
							throw new PXException(AP.Messages.NoRoundingGainLossAccSub, c.CuryID);
						}

						if (Math.Sign((decimal)(batch.CuryDebitTotal - batch.CuryCreditTotal)) == 1)
						{
							tran.AccountID = c.RoundingGainAcctID;
							tran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(je, tran.BranchID, c);
							tran.CuryCreditAmt = Math.Round((decimal)(batch.CuryDebitTotal - batch.CuryCreditTotal), 4);
							tran.CuryDebitAmt = 0m;
						}
						else
						{
							tran.AccountID = c.RoundingLossAcctID;
							tran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(je, tran.BranchID, c);
							tran.CuryCreditAmt = 0m;
							tran.CuryDebitAmt = Math.Round((decimal)(batch.CuryCreditTotal - batch.CuryDebitTotal), 4);
						}
						tran.CreditAmt = 0m;
						tran.DebitAmt = 0m;
						tran.TranType = doc.DocType;
						tran.RefNbr = doc.RefNbr;
						tran.TranClass = GLTran.tranClass.Normal;
						tran.TranDesc = GL.Messages.RoundingDiff;
						tran.LedgerID = batch.LedgerID;
						tran.FinPeriodID = batch.FinPeriodID;
						tran.TranDate = batch.DateEntered;
						tran.Released = true;
						tran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);

						CurrencyInfo infocopy = new CurrencyInfo();
						infocopy = je.currencyinfo.Insert(infocopy) ?? infocopy;

						tran.CuryInfoID = infocopy.CuryInfoID;
						je.GLTranModuleBatNbr.Insert(tran);
					}

					if (batch != null && batch.CuryCreditTotal == batch.CuryDebitTotal)
					{
						//in normal case this happens on moving to next CATran
						AddRoundingTran(je, doc, batch, cury);

						je.Save.Press();

						if (isTransfer)
						{
							transferTran = catran;
							transferBatch = je.BatchModule.Current;
						}

						if (batches.FirstOrDefault(b => b.BatchNbr == batch.BatchNbr) == null)
						{
							batches.Add(batch);
						}

						if (batchlist.Find(_ => je.BatchModule.Cache.ObjectsEqual(_, batch)) == null)
						{
							batchlist.Add(batch);
						}

						doc.Released = true;
						if (catran.TranID.HasValue && catran.CuryTranAmt != 0 && catran.TranAmt != 0)
						{
							caTranIDs.Add(catran.TranID.Value);
						}
						Caches[typeof(TCADocument)].Update(doc);

						if (Caches[typeof(TCADocument)].ObjectsEqual(doc, caadj) == false)
						{
							if (caadj.AdjRefNbr != null)
							{
								caadj.Released = true;
								Caches[typeof(CAAdj)].Update(caadj);
							}
						}
					}
					else
					{
						throw new PXException(Messages.DocumentOutOfBalance);
					}
				}

				doc.Released = true;
				Caches[typeof(TCADocument)].Update(doc);
				Caches[typeof(TCADocument)].Persist(PXDBOperation.Update);
				Caches[typeof(CAAdj)].Persist(PXDBOperation.Update);
				Caches[typeof(CATaxTran)].Persist(PXDBOperation.Update);
				Caches[typeof(SVATConversionHist)].Persist(PXDBOperation.Insert);
				Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);

				CheckMultipleGLPosting(caTranIDs);

				ts.Complete(this);
			}
			Caches[typeof(TCADocument)].Persisted(false);
			Caches[typeof(CAAdj)].Persisted(false);
			Caches[typeof(CATaxTran)].Persisted(false);
			Caches[typeof(CADailySummary)].Persisted(false);

            return batches;
		}

        private void AddRoundingTran(JournalEntry je, ICADocument doc, Batch batch, Currency cury) 
        {
            if (Math.Abs(Math.Round((decimal)(batch.DebitTotal - batch.CreditTotal), 4)) >= 0.00005m)
            {
                GLTran roundingTran = new GLTran();
                roundingTran.SummPost = true;
                roundingTran.ZeroPost = false;

                if (Math.Sign((decimal)(batch.DebitTotal - batch.CreditTotal)) == 1)
                {
                    roundingTran.AccountID = cury.RoundingGainAcctID;
                    roundingTran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingGainSubID>(je, batch.BranchID, cury);
                    roundingTran.CreditAmt = Math.Round((decimal)(batch.DebitTotal - batch.CreditTotal), 4);
                    roundingTran.DebitAmt = 0m;
                }
                else
                {
                    roundingTran.AccountID = cury.RoundingLossAcctID;
                    roundingTran.SubID = GainLossSubAccountMaskAttribute.GetSubID<Currency.roundingLossSubID>(je, batch.BranchID, cury);
                    roundingTran.CreditAmt = 0m;
                    roundingTran.DebitAmt = Math.Round((decimal)(batch.CreditTotal - batch.DebitTotal), 4);
                }
                roundingTran.CuryCreditAmt = 0m;
                roundingTran.CuryDebitAmt = 0m;
                roundingTran.TranType = doc.DocType;
                roundingTran.RefNbr = doc.RefNbr;
                roundingTran.TranClass = GLTran.tranClass.Normal;
                roundingTran.TranDesc = GL.Messages.RoundingDiff;
                roundingTran.LedgerID = batch.LedgerID;
                roundingTran.FinPeriodID = batch.FinPeriodID;
                roundingTran.TranDate = batch.DateEntered;
                roundingTran.Released = true;
                roundingTran.BranchID = batch.BranchID;
                roundingTran.ProjectID = PM.ProjectDefaultAttribute.NonProject(this);
                CurrencyInfo infocopy = new CurrencyInfo();
                infocopy = je.currencyinfo.Insert(infocopy) ?? infocopy;

                roundingTran.CuryInfoID = infocopy.CuryInfoID;
                je.GLTranModuleBatNbr.Insert(roundingTran);
            }
        }


		public virtual void ReleaseDeposit(JournalEntry je, ref List<Batch> batchlist, CADeposit doc)
		{
			je.Clear();

			Currency rgol_cury = null;
			CurrencyInfo rgol_info = null;
            Currency cury = null;
			Dictionary<int, GLTran> rgols = new Dictionary<int, GLTran>();
			GLTran tran;
			CATran prev_tran = null;
			Batch batch = CreateGLBatch(je, doc);

			var caTranIDs = new HashSet<long>();

			PXSelectBase<CATran> select = new PXSelectJoin<CATran,
									InnerJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CATran.cashAccountID>>,
									InnerJoin<Currency, On<Currency.curyID, Equal<CashAccount.curyID>>,
									InnerJoin<CurrencyInfo, On<CurrencyInfo.curyInfoID, Equal<CATran.curyInfoID>>,
									LeftJoin<CADepositDetail, On<CADepositDetail.tranType, Equal<CATran.origTranType>,
												And<CADepositDetail.refNbr, Equal<CATran.origRefNbr>,
												And<CADepositDetail.tranID, Equal<CATran.tranID>>>>,
									LeftJoin<CADepositEntry.ARPaymentUpdate, On<CADepositEntry.ARPaymentUpdate.docType, Equal<CADepositDetail.origDocType>,
													And<CADepositEntry.ARPaymentUpdate.refNbr, Equal<CADepositDetail.origRefNbr>,
													And<CADepositDetail.origModule, Equal<GL.BatchModule.moduleAR>>>>,
									LeftJoin<CADepositEntry.APPaymentUpdate, On<CADepositEntry.APPaymentUpdate.docType, Equal<CADepositDetail.origDocType>,
													And<CADepositEntry.APPaymentUpdate.refNbr, Equal<CADepositDetail.origRefNbr>,
													And<CADepositDetail.origModule, Equal<GL.BatchModule.moduleAP>>>>>>>>>>,
									Where<CATran.origModule, Equal<BatchModule.moduleCA>,
									And<CATran.origTranType, Equal<Required<CATran.origTranType>>,
									And<CATran.origRefNbr, Equal<Required<CATran.origRefNbr>>>>>,
									OrderBy<Asc<CATran.tranID>>>(this);

			foreach (PXResult<CATran, CashAccount, Currency, CurrencyInfo, CADepositDetail, CADepositEntry.ARPaymentUpdate, CADepositEntry.APPaymentUpdate> res in select.Select(doc.DocType, doc.RefNbr))
			{
				CATran catran = (CATran)res;
				CashAccount cashacct = (CashAccount)res;
				cury = (Currency)res;
				CurrencyInfo info = (CurrencyInfo)res;
				CADepositDetail detail = (CADepositDetail)res;
				CADepositEntry.ARPaymentUpdate arDoc = (CADepositEntry.ARPaymentUpdate)res;
				CADepositEntry.APPaymentUpdate apDoc = (CADepositEntry.APPaymentUpdate)res;
				if (catran.CuryID != doc.CuryID)
					throw new PXException(Messages.MultiCurDepositNotSupported);

				batch = (Batch)je.BatchModule.Current;
				if (object.Equals(prev_tran, catran) == false)
				{
					tran = new GLTran();
					tran.SummPost = false;
					tran.CuryInfoID = batch.CuryInfoID;
					tran.TranType = catran.OrigTranType;
					tran.RefNbr = catran.OrigRefNbr;
					tran.ReferenceID = catran.ReferenceID;
                    tran.AccountID = cashacct.AccountID;
					tran.SubID = cashacct.SubID;
                    tran.BranchID = cashacct.BranchID;
					tran.CATranID = catran.TranID;
					tran.TranDate = catran.TranDate;
					tran.FinPeriodID = catran.FinPeriodID;
					tran.TranPeriodID = catran.TranPeriodID;
					tran.CuryDebitAmt = (catran.DrCr == CADrCr.CADebit ? catran.CuryTranAmt : 0m);
					tran.DebitAmt = (catran.DrCr == CADrCr.CADebit ? catran.TranAmt : 0m);
					tran.CuryCreditAmt = (catran.DrCr == CADrCr.CADebit ? 0m : -1m * catran.CuryTranAmt);
					tran.CreditAmt = (catran.DrCr == CADrCr.CADebit ? 0m : -1m * catran.TranAmt);
					tran.TranDesc = catran.TranDesc;
					tran.Released = true;
					je.GLTranModuleBatNbr.Insert(tran);

					if (!String.IsNullOrEmpty(arDoc.RefNbr))
					{
						if (doc.TranType == CATranType.CADeposit)
						{
							arDoc.Deposited = true;
						}
						else
						{
							arDoc.Deposited = false;
							arDoc.DepositType = null;
							arDoc.DepositNbr = null;
							arDoc.DepositDate = null;
						}
						this.Caches[typeof(CADepositEntry.ARPaymentUpdate)].Update(arDoc);
					}

					if (!String.IsNullOrEmpty(apDoc.RefNbr))
					{
						if (doc.TranType == CATranType.CADeposit)
						{
							apDoc.Deposited = true;
						}
						else
						{
							apDoc.Deposited = false;
							apDoc.DepositType = null;
							apDoc.DepositNbr = null;
							apDoc.DepositDate = null;
						}
						this.Caches[typeof(CADepositEntry.APPaymentUpdate)].Update(apDoc);
					}

					if (!String.IsNullOrEmpty(detail.OrigRefNbr))
					{

						decimal rgol = Math.Round((detail.OrigAmtSigned.Value - detail.TranAmt.Value), 3);
						if (rgol != Decimal.Zero)
						{
							GLTran rgol_tran = null;
							if (!rgols.ContainsKey(detail.AccountID.Value))
							{
								rgol_tran = new GLTran();
								rgol_tran.DebitAmt = Decimal.Zero;
								rgol_tran.CreditAmt = Decimal.Zero;
                                rgol_tran.AccountID = cashacct.AccountID;
                                rgol_tran.SubID = cashacct.SubID;
                                rgol_tran.BranchID = cashacct.BranchID;
								rgol_tran.TranDate = catran.TranDate;
								rgol_tran.FinPeriodID = catran.FinPeriodID;
								rgol_tran.TranPeriodID = catran.TranPeriodID;
								rgol_tran.TranType = CATranType.CATransferRGOL;
								rgol_tran.RefNbr = doc.RefNbr;
								rgol_tran.TranDesc = "RGOL";
								rgol_tran.Released = true;
								rgol_tran.CuryInfoID = batch.CuryInfoID;

								rgols[detail.AccountID.Value] = rgol_tran;
							}
							else
							{
								rgol_tran = rgols[detail.AccountID.Value];
							}
							rgol_tran.DebitAmt += ((catran.DrCr == CADrCr.CACredit) == rgol > 0 ? Decimal.Zero : Math.Abs(rgol));
							rgol_tran.CreditAmt += ((catran.DrCr == CADrCr.CACredit) == rgol > 0 ?  Math.Abs(rgol) : Decimal.Zero);
							rgol_cury = cury;
							rgol_info = info;
						}
					}
				}
				prev_tran = catran;

				if (catran.TranID.HasValue && catran.CuryTranAmt != 0 && catran.TranAmt != 0)
				{
					caTranIDs.Add(catran.TranID.Value);
				}
			}
			if (batch != null)
			{
				foreach (CADepositCharge iCharge in PXSelect<CADepositCharge, Where<CADepositCharge.tranType, Equal<Required<CADepositCharge.tranType>>,
																	And<CADepositCharge.refNbr, Equal<Required<CADepositCharge.refNbr>>>>>.Select(this, doc.TranType, doc.RefNbr))
				{
					if (iCharge != null && iCharge.CuryChargeAmt != Decimal.Zero)
					{
						tran = new GLTran();
						tran.SummPost = false;
						tran.CuryInfoID = batch.CuryInfoID;
						tran.TranType = iCharge.TranType;
						tran.RefNbr = iCharge.RefNbr;

						tran.AccountID = iCharge.AccountID;
						tran.SubID = iCharge.SubID;
						tran.TranDate = doc.TranDate;
						tran.FinPeriodID = doc.FinPeriodID;
						tran.TranPeriodID = doc.TranPeriodID;
						tran.CuryDebitAmt = (iCharge.DrCr == CADrCr.CADebit ? Decimal.Zero : iCharge.CuryChargeAmt);
						tran.DebitAmt = (iCharge.DrCr == CADrCr.CADebit ? Decimal.Zero : iCharge.ChargeAmt);
						tran.CuryCreditAmt = (iCharge.DrCr == CADrCr.CADebit ? iCharge.CuryChargeAmt : Decimal.Zero);
						tran.CreditAmt = (iCharge.DrCr == CADrCr.CADebit ? iCharge.ChargeAmt : Decimal.Zero);
						tran.Released = true;
						je.GLTranModuleBatNbr.Insert(tran);
					}
				}

				foreach (KeyValuePair<int, GLTran> it in rgols)
				{
					GLTran rgol_tran = it.Value;
					decimal rgolAmt = (decimal)(rgol_tran.DebitAmt - rgol_tran.CreditAmt);
					int sign = Math.Sign(rgolAmt);
					rgolAmt = Math.Abs(rgolAmt);

					if ((rgolAmt) != Decimal.Zero)
					{
						tran = (GLTran)je.Caches[typeof(GLTran)].CreateCopy(rgol_tran);
						tran.CuryDebitAmt = Decimal.Zero;
						tran.CuryCreditAmt = Decimal.Zero;
						if (doc.DocType == CATranType.CADeposit)
						{
							tran.AccountID = (sign < 0) ? rgol_cury.RealLossAcctID : rgol_cury.RealGainAcctID;
							tran.SubID = (sign < 0)
                                ? GainLossSubAccountMaskAttribute.GetSubID<Currency.realLossSubID>(je, rgol_tran.BranchID, rgol_cury)
                                : GainLossSubAccountMaskAttribute.GetSubID<Currency.realGainSubID>(je, rgol_tran.BranchID, rgol_cury);
						}
						else
						{
							tran.AccountID = (sign < 0) ? rgol_cury.RealGainAcctID : rgol_cury.RealLossAcctID;
							tran.SubID = (sign < 0)
                                ? GainLossSubAccountMaskAttribute.GetSubID<Currency.realGainSubID>(je, rgol_tran.BranchID, rgol_cury)
                                : GainLossSubAccountMaskAttribute.GetSubID<Currency.realLossSubID>(je, rgol_tran.BranchID, rgol_cury);
						}

						tran.DebitAmt = sign < 0 ? rgolAmt : Decimal.Zero;
						tran.CreditAmt = sign < 0 ? Decimal.Zero : rgolAmt;
						tran.TranType = CATranType.CATransferRGOL;
						tran.RefNbr = doc.RefNbr;
						tran.TranDesc = "RGOL";
						tran.TranDate = rgol_tran.TranDate;
						tran.FinPeriodID = rgol_tran.FinPeriodID;
						tran.TranPeriodID = rgol_tran.TranPeriodID;
						tran.Released = true;
						tran.CuryInfoID = batch.CuryInfoID;
						tran = je.GLTranModuleBatNbr.Insert(tran);

						rgol_tran.CuryDebitAmt = Decimal.Zero;
						rgol_tran.DebitAmt = (sign > 0) ? rgolAmt : Decimal.Zero;
						rgol_tran.CreditAmt = (sign > 0) ? Decimal.Zero : rgolAmt;
						je.GLTranModuleBatNbr.Insert(rgol_tran);
					}
				}
			}
            if (batch != null && batch.CuryCreditTotal == batch.CuryDebitTotal)
            {
                AddRoundingTran(je, doc, batch, cury);
            }
			if (batch != null && batch.CuryCreditTotal != batch.CuryDebitTotal)
				throw new PXException(GL.Messages.BatchOutOfBalance);
			using (PXTransactionScope ts = new PXTransactionScope())
			{
				if (batch != null)
				{
					je.Save.Press();
					if (batchlist.Contains(batch) == false)
					{
						batchlist.Add(batch);
					}
					doc.Released = true;
					
					Caches[typeof(CADeposit)].Update(doc);
					if (doc.TranType == CATranType.CAVoidDeposit)
					{
						CADeposit orig = PXSelect<CADeposit, Where<CADeposit.tranType, Equal<CATranType.cADeposit>, And<CADeposit.refNbr, Equal<Required<CADeposit.refNbr>>>>>.Select(this, doc.RefNbr);
						orig.Voided = true;
						Caches[typeof(CADeposit)].Update(orig);
					}
				}
				Caches[typeof(CADeposit)].Persist(PXDBOperation.Update);
				Caches[typeof(CADepositDetail)].Persist(PXDBOperation.Update);
				Caches[typeof(CADepositEntry.ARPaymentUpdate)].Persist(PXDBOperation.Update);
				Caches[typeof(CADepositEntry.APPaymentUpdate)].Persist(PXDBOperation.Update);
				Caches[typeof(CADailySummary)].Persist(PXDBOperation.Insert);

				CheckMultipleGLPosting(caTranIDs);

				ts.Complete();
			}

			Caches[typeof(CADeposit)].Persisted(false);
			Caches[typeof(CADepositDetail)].Persisted(false);
			Caches[typeof(CADepositEntry.ARPaymentUpdate)].Persisted(false);
			Caches[typeof(CADepositEntry.APPaymentUpdate)].Persisted(false);
			Caches[typeof(CADailySummary)].Persisted(false);
		}

		private static Batch CreateGLBatch(JournalEntry je, CADeposit doc)
		{
			CurrencyInfo orig = PXSelectReadonly<CurrencyInfo, Where<CurrencyInfo.curyInfoID, Equal<Required<CurrencyInfo.curyInfoID>>>>.Select(je, doc.CuryInfoID);
			CurrencyInfo newinfo = (CurrencyInfo)je.currencyinfo.Cache.CreateCopy(orig);
			newinfo.CuryInfoID = null;
			newinfo = je.currencyinfo.Insert(newinfo);
            CashAccount cashAccount = PXSelect<CashAccount, Where<CashAccount.cashAccountID,Equal<Required<CashAccount.cashAccountID>>>>.Select(je, doc.CashAccountID);
			Batch newbatch = new Batch();
			newbatch.Module = BatchModule.CA;
			newbatch.Status = "U";
			newbatch.Released = true;
			newbatch.Hold = false;
			newbatch.DateEntered = doc.TranDate;
			newbatch.FinPeriodID = doc.FinPeriodID;
			newbatch.TranPeriodID = doc.TranPeriodID;
			newbatch.CuryID = doc.CuryID;
			newbatch.CuryInfoID = newinfo.CuryInfoID;
			newbatch.DebitTotal = 0m;
			newbatch.CreditTotal = 0m;
            newbatch.BranchID = cashAccount.BranchID;
			newbatch = je.BatchModule.Insert(newbatch);

            CurrencyInfo b_info = je.currencyinfo.Select();
            if (b_info != null)
            {
                b_info.CuryID = orig.CuryID;
                je.currencyinfo.SetValueExt<CurrencyInfo.curyEffDate>(b_info, orig.CuryEffDate);
                b_info.SampleCuryRate = orig.SampleCuryRate ?? b_info.SampleCuryRate;
                b_info.CuryRateTypeID = orig.CuryRateTypeID ?? b_info.CuryRateTypeID;
                je.currencyinfo.Update(b_info);
            }
			je.BatchModule.Current = newbatch;
			return newbatch;
		}

		protected void CheckMultipleGLPosting(HashSet<long> caTranIDs)
		{
			if(!this.casetup.Current.ValidateDataConsistencyOnRelease)
			{
				return;
			}

			foreach (long id in caTranIDs)
			{
				int count = PXSelectReadonly2<CATran, InnerJoin<GLTran,
								On<GLTran.cATranID, Equal<CATran.tranID>>>,
								Where<CATran.released, Equal<True>, And<GLTran.released, Equal<True>,
								And<CATran.tranID, Equal<Required<CATran.tranID>>>>>>.Select(this, id).Count;

				if (count != 1)
				{
					PXTrace.WriteError($"Error message: {Messages.CATranHasExcessGLTran}; Date: {DateTime.Now}; Screen: {Accessinfo.ScreenID}; Count: {count}; CATranID: {id};");
					throw new PXException(Messages.CATranHasExcessGLTran);
				}
			}
		}
	}
}
