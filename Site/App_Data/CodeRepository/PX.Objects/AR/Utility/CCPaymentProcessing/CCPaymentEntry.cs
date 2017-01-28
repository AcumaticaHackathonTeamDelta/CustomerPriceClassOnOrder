using PX.CCProcessingBase;
using PX.Common;
using PX.Data;
using PX.Objects.Common;
using System;
using System.Collections.Generic;
using System.Text;


namespace PX.Objects.AR.CCPaymentProcessing
{
	public static class CCPaymentEntry
	{
		public class CCProcTranOrderComparer : IComparer<CCProcTran>
		{
			private bool _descending;

			public CCProcTranOrderComparer(bool aDescending)
			{
				this._descending = aDescending;
			}
			public CCProcTranOrderComparer()
			{
				this._descending = false;
			}

			#region IComparer<CCProcTran> Members

			public virtual int Compare(CCProcTran x, CCProcTran y)
			{
				int order = x.TranNbr.Value.CompareTo(y.TranNbr.Value);
				return (this._descending ? -order : order);
			}

			#endregion
		}

		public static string FormatCCPaymentState(CCPaymentState aState)
		{
			Dictionary<CCPaymentState, string> stateDict = new Dictionary<CCPaymentState, string>();

			stateDict[CCPaymentState.None] = PXMessages.LocalizeNoPrefix(Messages.CCNone);
			stateDict[CCPaymentState.PreAuthorized] = PXMessages.LocalizeNoPrefix(Messages.CCPreAuthorized);
			stateDict[CCPaymentState.PreAuthorizationFailed] = PXMessages.LocalizeNoPrefix(Messages.CCPreAuthorizationFailed);
			stateDict[CCPaymentState.PreAuthorizationExpired] = PXMessages.LocalizeNoPrefix(Messages.CCPreAuthorizationExpired);
			stateDict[CCPaymentState.Captured] = PXMessages.LocalizeNoPrefix(Messages.CCCaptured);
			stateDict[CCPaymentState.CaptureFailed] = PXMessages.LocalizeNoPrefix(Messages.CCCaptureFailed);
			stateDict[CCPaymentState.Voided] = PXMessages.LocalizeNoPrefix(Messages.CCVoided);
			stateDict[CCPaymentState.VoidFailed] = PXMessages.LocalizeNoPrefix(Messages.CCVoidFailed);
			stateDict[CCPaymentState.Refunded] = PXMessages.LocalizeNoPrefix(Messages.CCRefunded);
			stateDict[CCPaymentState.RefundFailed] = PXMessages.LocalizeNoPrefix(Messages.CCRefundFailed);
			stateDict[CCPaymentState.VoidFailed] = PXMessages.LocalizeNoPrefix(Messages.CCRefundFailed);
			StringBuilder result = new StringBuilder();
			foreach (KeyValuePair<CCPaymentState, string> it in stateDict)
			{
				if ((aState & it.Key) != 0)
				{
					if (result.Length > 0)
						result.Append(",");
					result.Append(it.Value);
				}
			}
			return result.ToString();
		}

		public static CCPaymentState ResolveCCPaymentState(IEnumerable<PXResult<CCProcTran>> ccProcTrans)
		{
			CCProcTran lastTran;
			return ResolveCCPaymentState(ccProcTrans, out lastTran);
		}

		public static CCPaymentState ResolveCCPaymentState(IEnumerable<PXResult<CCProcTran>> ccProcTrans, out CCProcTran aLastTran)
		{
			CCPaymentState result = CCPaymentState.None;
			CCProcTran lastTran = null;
			CCProcTran lastSSTran = null; //Last successful tran
			CCProcTran preLastSSTran = null;
			CCProcTranOrderComparer ascComparer = new CCProcTranOrderComparer();
			aLastTran = null;
			foreach (CCProcTran iTran in ccProcTrans)
			{
				if (iTran.ProcStatus != CCProcStatus.Finalized && iTran.ProcStatus != CCProcStatus.Error)
					continue;
				if (lastTran == null)
				{
					lastTran = iTran;
				}
				else
				{
					if (ascComparer.Compare(iTran, lastTran) > 0)
					{
						lastTran = iTran;
					}
				}

				if (iTran.TranStatus == CCTranStatusCode.Approved)
				{
					if (lastSSTran == null)
					{
						lastSSTran = iTran;
					}
					else if (ascComparer.Compare(iTran, lastSSTran) > 0)
					{
						lastSSTran = iTran;
					}

					if (lastSSTran != null && (ascComparer.Compare(iTran, lastSSTran) < 0))
					{
						if (preLastSSTran == null)
						{
							preLastSSTran = iTran;
						}
						else if (ascComparer.Compare(iTran, preLastSSTran) > 0)
						{
							preLastSSTran = iTran;
						}
					}
				}
			}

			if (lastTran != null)
			{
				if (lastSSTran != null)
				{
					switch (lastSSTran.TranType)
					{
						case CCTranTypeCode.Authorize:
							if (!IsExpired(lastSSTran))
								result = CCPaymentState.PreAuthorized;
							else
								result = CCPaymentState.PreAuthorizationExpired;
							break;
						case CCTranTypeCode.AuthorizeAndCapture:
						case CCTranTypeCode.PriorAuthorizedCapture:
						case CCTranTypeCode.CaptureOnly:
							result = CCPaymentState.Captured;
							break;
						case CCTranTypeCode.VoidTran:
							if (preLastSSTran != null)
							{
								result = (preLastSSTran.TranType == CCTranTypeCode.Authorize) ?
											result = CCPaymentState.None : result = CCPaymentState.Voided; //Voidin of credit currenly is not allowed								
							}
							break;
						case CCTranTypeCode.Credit:
							result = CCPaymentState.Refunded;
							break;
					}
				}

				if (lastSSTran == null || lastSSTran.TranNbr != lastTran.TranNbr) //this means that lastOp failed
				{
					switch (lastTran.TranType)
					{
						case CCTranTypeCode.Authorize:
							result |= CCPaymentState.PreAuthorizationFailed;
							break;
						case CCTranTypeCode.AuthorizeAndCapture:
						case CCTranTypeCode.PriorAuthorizedCapture:
						case CCTranTypeCode.CaptureOnly:
							result |= CCPaymentState.CaptureFailed;
							break;
						case CCTranTypeCode.VoidTran:
							result |= CCPaymentState.VoidFailed;
							break;
						case CCTranTypeCode.Credit:
							result |= CCPaymentState.RefundFailed;
							break;
					}
				}
			}
			aLastTran = lastTran;
			return result;
		}

		public static bool HasSuccessfulCCTrans(PXSelectBase<CCProcTran> ccProcTran)
		{
			CCProcTran lastSTran = findCCLastSuccessfulTran(ccProcTran);
			if (lastSTran != null && !IsExpired(lastSTran))
			{
				return true;
			}
            return false;
		}

		public static bool HasUnfinishedCCTrans(PXGraph aGraph, CustomerPaymentMethod aCPM)
		{
			if (aCPM.PMInstanceID < 0)
			{
				return false;
			}

			Dictionary<string, List<PXResult<CCProcTran>>> TranDictionary = new Dictionary<string, List<PXResult<CCProcTran>>>();
			PXResultset<CCProcTran> ccTrans = PXSelect<CCProcTran, Where<CCProcTran.pMInstanceID, Equal<Required<CCProcTran.pMInstanceID>>,
					And<CCProcTran.pCTranNumber, IsNotNull>>, OrderBy<Asc<CCProcTran.pCTranNumber>>>.Select(aGraph, aCPM.PMInstanceID);

			foreach (var row in ccTrans)
			{
				CCProcTran tran = (CCProcTran)row;
				if (tran.PCTranNumber != "0")
				{
					if (!TranDictionary.ContainsKey(tran.PCTranNumber))
					{
						TranDictionary[tran.PCTranNumber] = new List<PXResult<CCProcTran>>();
					}
					TranDictionary[tran.PCTranNumber].Add(row);
				}
			}

			bool hasUnfinishedTrans = false;
			foreach (var kvp in TranDictionary)
			{
				List<PXResult<CCProcTran>> tranList = kvp.Value;
				CCProcTran lastTran;
				CCPaymentState ccPaymentState = CCPaymentEntry.ResolveCCPaymentState(tranList, out lastTran);
				bool isCCPreAuthorized = (ccPaymentState & CCPaymentState.PreAuthorized) != 0;
				if (isCCPreAuthorized && lastTran != null && (lastTran.ExpirationDate == null || lastTran.ExpirationDate > DateTime.Now))
				{
					hasUnfinishedTrans = true;
					break;
				}
			}
			return hasUnfinishedTrans;
		}

		public static bool UpdateCapturedState<T>(T doc, IEnumerable<PXResult<CCProcTran>> ccProcTrans)
			where T : class, IBqlTable, ICCCapturePayment
		{
			CCProcTran lastTran;
			CCPaymentState ccPaymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTrans, out lastTran);
			bool isCCVoided = (ccPaymentState & CCPaymentState.Voided) != 0;
			bool isCCCaptured = (ccPaymentState & CCPaymentState.Captured) != 0;
			bool isCCPreAuthorized = (ccPaymentState & CCPaymentState.PreAuthorized) != 0;
			bool isCCRefunded = (ccPaymentState & CCPaymentState.Refunded) != 0;
			bool isCCVoidingAttempted = (ccPaymentState & CCPaymentState.VoidFailed) != 0;
			bool needUpdate = false;

			if (doc.IsCCCaptured != isCCCaptured)
			{
				doc.IsCCCaptured = isCCCaptured;
				needUpdate = true;
			}

			if (lastTran != null
				&& (lastTran.TranType == CCTranTypeCode.PriorAuthorizedCapture
					|| lastTran.TranType == CCTranTypeCode.AuthorizeAndCapture
					|| lastTran.TranType == CCTranTypeCode.CaptureOnly))
			{
				if (isCCCaptured)
				{
					doc.CuryCCCapturedAmt = lastTran.Amount;
					doc.IsCCCaptureFailed = false;
					needUpdate = true;
				}
				else
				{
					doc.IsCCCaptureFailed = true;
					needUpdate = true;
				}
			}

			if (doc.IsCCCaptured == false && (doc.CuryCCCapturedAmt != Decimal.Zero))
			{
				doc.CuryCCCapturedAmt = Decimal.Zero;
				needUpdate = true;
			}

			return needUpdate;
		}

		public struct CCTransState
		{
			public bool NeedUpdate;
			public CCPaymentState CCState;
			public CCProcTran LastTran;
		}

		public static CCTransState UpdateCCPaymentState<T>(T doc, IEnumerable<PXResult<CCProcTran>> ccProcTrans)
			where T : class, ICCAuthorizePayment, ICCCapturePayment
		{
			CCProcTran lastTran;
			CCPaymentState ccPaymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTrans, out lastTran);
			bool isCCVoided = (ccPaymentState & CCPaymentState.Voided) != 0;
			bool isCCCaptured = (ccPaymentState & CCPaymentState.Captured) != 0;
			bool isCCPreAuthorized = (ccPaymentState & CCPaymentState.PreAuthorized) != 0;
			bool isCCRefunded = (ccPaymentState & CCPaymentState.Refunded) != 0;
			bool isCCVoidingAttempted = (ccPaymentState & CCPaymentState.VoidFailed) != 0 || (ccPaymentState & CCPaymentState.RefundFailed) != 0;
			bool needUpdate = false;

			if (doc.IsCCAuthorized != isCCPreAuthorized || doc.IsCCCaptured != isCCCaptured)
			{
				if (!isCCVoidingAttempted)
				{
					doc.IsCCAuthorized = isCCPreAuthorized;
					doc.IsCCCaptured = isCCCaptured;
					needUpdate = true;
				}
				else
				{
					doc.IsCCAuthorized = false;
					doc.IsCCCaptured = false;
					needUpdate = false;
				}
			}


			if (lastTran != null && isCCPreAuthorized && lastTran.TranType == CCTranTypeCode.Authorize)
			{
				doc.CCAuthExpirationDate = lastTran.ExpirationDate;
				doc.CuryCCPreAuthAmount = lastTran.Amount;
				needUpdate = true;
			}

			if (doc.IsCCAuthorized == false && (doc.CCAuthExpirationDate != null || doc.CuryCCPreAuthAmount > Decimal.Zero))
			{
				doc.CCAuthExpirationDate = null;
				doc.CuryCCPreAuthAmount = Decimal.Zero;

				needUpdate = true;
			}

			if (lastTran != null
				&& (lastTran.TranType == CCTranTypeCode.PriorAuthorizedCapture
					|| lastTran.TranType == CCTranTypeCode.AuthorizeAndCapture
					|| lastTran.TranType == CCTranTypeCode.CaptureOnly))
			{
				if (isCCCaptured)
				{
					doc.CuryCCCapturedAmt = lastTran.Amount;
					doc.IsCCCaptureFailed = false;
					needUpdate = true;
				}
				else
				{
					doc.IsCCCaptureFailed = true;
					needUpdate = true;
				}
			}

			if (doc.IsCCCaptured == false && (doc.CuryCCCapturedAmt != Decimal.Zero))
			{
				doc.CuryCCCapturedAmt = Decimal.Zero;
				needUpdate = true;
			}

			return new CCTransState { NeedUpdate = needUpdate, CCState = ccPaymentState, LastTran = lastTran };
		}


		public static bool IsExpired(CCProcTran aTran)
		{
			return (aTran.ExpirationDate.HasValue && (aTran.ExpirationDate.Value < PXTimeZoneInfo.Now));
		}

		public static bool HasOpenCCTran(PXSelectBase<CCProcTran> ccProcTran)
		{
			foreach (CCProcTran iTr in ccProcTran.Select())
			{
				if (iTr.ProcStatus == CCProcStatus.Opened && !IsExpired(iTr))
					return true;
			}
			return false;
		}

		public static bool HasCCTransactions(PXSelectBase<CCProcTran> ccProcTran)
		{
			return (ccProcTran.Any());
		}

		public static CCProcTran findCCPreAthorizing(IEnumerable<PXResult<CCProcTran>> ccProcTran)
		{
			List<CCProcTran> authTrans = new List<CCProcTran>(1);
			List<CCProcTran> result = new List<CCProcTran>(1);
			foreach (CCProcTran iTran in ccProcTran)
			{
				if (iTran.ProcStatus != CCProcStatus.Finalized)
					continue;
				if (iTran.TranType == CCTranTypeCode.Authorize && iTran.TranStatus == CCTranStatusCode.Approved)
				{
					authTrans.Add(iTran);
				}
			}

			foreach (CCProcTran it in authTrans)
			{
				bool cancelled = false;
				foreach (CCProcTran iTran in ccProcTran)
				{
					if (iTran.ProcStatus != CCProcStatus.Finalized)
						continue;
					if (iTran.RefTranNbr == it.TranNbr && iTran.TranStatus == CCTranStatusCode.Approved)
					{
						if (iTran.TranType.Trim() == CCTranTypeCode.PriorAuthorizedCapture
								|| iTran.TranType.Trim() == CCTranTypeCode.VoidTran)
						{
							cancelled = true;
							break;
						}
					}
				}
				if (!cancelled)
				{
					result.Add(it);
				}
			}

			if (result.Count > 0)
			{
				result.Sort(new CCProcTranOrderComparer(true)
						/*new Comparison<CCProcTran>(delegate(CCProcTran a, CCProcTran b)
						{
							return a.EndTime.Value.CompareTo(b.EndTime.Value);
						}) */
						);
				return result[0];
			}
			return null;
		}

		public static CCProcTran findCapturing(PXSelectBase<CCProcTran> ccProcTran)
		{
			List<CCProcTran> authTrans = new List<CCProcTran>(1);
			List<CCProcTran> result = new List<CCProcTran>(1);
			foreach (CCProcTran iTran in ccProcTran.Select())
			{
				if (iTran.ProcStatus != CCProcStatus.Finalized)
					continue;
				if ((
						iTran.TranType == CCTranTypeCode.PriorAuthorizedCapture ||
						iTran.TranType == CCTranTypeCode.AuthorizeAndCapture ||
						iTran.TranType == CCTranTypeCode.CaptureOnly
					)
					&& iTran.TranStatus == CCTranStatusCode.Approved)
				{
					authTrans.Add(iTran);
				}
			}
			foreach (CCProcTran it in authTrans)
			{
				bool cancelled = false;
				foreach (CCProcTran iTran in ccProcTran.Select())
				{
					if (iTran.ProcStatus != CCProcStatus.Finalized)
						continue;
					if (iTran.RefTranNbr == it.TranNbr && iTran.TranStatus == CCTranStatusCode.Approved)
					{
						if (iTran.TranType == CCTranTypeCode.Credit
								|| iTran.TranType == CCTranTypeCode.VoidTran)
						{
							cancelled = true;
							break;
						}
					}
				}
				if (!cancelled)
				{
					result.Add(it);
				}
			}
			if (result.Count > 0)
			{
				result.Sort(new CCProcTranOrderComparer(true) /*Comparison<CCProcTran>(delegate(CCProcTran a, CCProcTran b)
						{
							return a.EndTime.Value.CompareTo(b.EndTime.Value);
						}
						)*/
					);
				return result[0];
			}
			return null;
		}

		public static CCProcTran findCCLastSuccessfulTran(PXSelectBase<CCProcTran> ccProcTran)
		{
			CCProcTran lastTran = null;
			CCProcTranOrderComparer ascComparer = new CCProcTranOrderComparer();
			foreach (CCProcTran iTran in ccProcTran.Select())
			{
				if (iTran.ProcStatus != CCProcStatus.Finalized)
					continue;
				if (iTran.TranStatus == CCTranStatusCode.Approved)
				{
					if (lastTran == null || (ascComparer.Compare(iTran, lastTran) > 0))
					{
						lastTran = iTran;
					}
				}
			}
			return lastTran;
		}

		public static void ProcessCCTransaction(ICCPayment aDoc, CCProcTran refTran, CCTranType aTranType)
		{
			if (aDoc != null && aDoc.PMInstanceID != null && aDoc.CuryDocBal != null)
			{
				CCPaymentProcessing processBO = new CCPaymentProcessing();
				int tranID = 0;
				bool result = false;
				bool processed = false;
				if (aTranType == CCTranType.AuthorizeOnly || aTranType == CCTranType.AuthorizeAndCapture)
				{
					bool doCapture = (aTranType == CCTranType.AuthorizeAndCapture);
					result = processBO.Authorize(aDoc, doCapture, ref tranID);
					processed = true;
				}

				if (aTranType == CCTranType.PriorAuthorizedCapture)
				{
					if (refTran == null)
					{
						throw new PXException(Messages.ERR_CCTransactionMustBeAuthorizedBeforeCapturing);
					}
					result = processBO.Capture(aDoc.PMInstanceID, refTran.TranNbr, aDoc.CuryID, aDoc.CuryDocBal, ref tranID);
					processed = true;
				}

				if (aTranType == CCTranType.Void)
				{
					if (refTran == null)
					{
						throw new PXException(Messages.ERR_CCOriginalTransactionNumberIsRequiredForVoiding);
					}
					result = processBO.Void(aDoc.PMInstanceID, refTran.TranNbr, ref tranID);
					processed = true;
				}

				if (aTranType == CCTranType.VoidOrCredit)
				{
					if (refTran == null)
					{
						throw new PXException(Messages.ERR_CCOriginalTransactionNumberIsRequiredForVoidingOrCrediting);
					}
					result = processBO.VoidOrCredit(aDoc.PMInstanceID, refTran.TranNbr, ref tranID);
					processed = true;
				}

				if (aTranType == CCTranType.Credit)
				{
					if (refTran == null)
					{
						throw new PXException(Messages.ERR_CCOriginalTransactionNumberIsRequiredForCrediting);
					}
					if (refTran.TranNbr.HasValue)
						result = processBO.Credit(aDoc, refTran.TranNbr.Value, ref tranID);
					else
						result = processBO.Credit(aDoc, refTran.PCTranNumber, ref tranID);
					processed = true;
				}

				if (aTranType == CCTranType.CaptureOnly)
				{
					//Uses Authorization Number received from Processing center in a special way (for example, by phone)
					if (refTran == null || string.IsNullOrEmpty(refTran.AuthNumber))
					{
						throw new PXException(Messages.ERR_CCExternalAuthorizationNumberIsRequiredForCaptureOnlyTrans);
					}
					result = processBO.CaptureOnly(aDoc, refTran.AuthNumber, ref tranID);
					processed = true;
				}

				if (!processed)
				{
					throw new PXException(Messages.ERR_CCUnknownOperationType);
				}

				if (!result)
				{
					throw new PXException(Messages.ERR_CCTransactionWasNotAuthorizedByProcCenter, tranID);
				}

			}
		}

		public static void CaptureCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran, bool doRelease, ReleaseDelegate CustomPreReleaseDelegate)
			where TNode : ARRegister, IBqlTable, ICCPayment, new()
		{
			ReleaseDelegate combinedReleaseDelegate = doRelease ? CustomPreReleaseDelegate + ReleaseARDocument : CustomPreReleaseDelegate;
			CaptureCCPayment<TNode>(doc, ccProcTran, combinedReleaseDelegate, null);
		}

		public static void CreditCCPayment<TNode>(TNode doc, string aPCRefTranNbr, PXSelectBase<CCProcTran> ccProcTran, bool doRelease, ReleaseDelegate CustomPreReleaseDelegate)
			where TNode : ARRegister, IBqlTable, ICCPayment, new()
		{
			ReleaseDelegate combinedReleaseDelegate = doRelease ? CustomPreReleaseDelegate + ReleaseARDocument : CustomPreReleaseDelegate;
			CreditCCPayment<TNode>(doc, aPCRefTranNbr, ccProcTran, combinedReleaseDelegate, null);
		}


		public static void CaptureOnlyCCPayment<TNode>(TNode doc, string aAuthorizationNbr, PXSelectBase<CCProcTran> ccProcTran, bool doRelease)
			where TNode : ARRegister, IBqlTable, ICCPayment, new()
		{
			CaptureOnlyCCPayment<TNode>(doc, aAuthorizationNbr, ccProcTran, doRelease ? ReleaseARDocument : (ReleaseDelegate)null, null);
		}
		public static void VoidCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran, bool doRelease)
			where TNode : ARRegister, ICCPayment, new()
		{
			VoidCCPayment<TNode>(doc, ccProcTran, doRelease ? ReleaseARDocument : (ReleaseDelegate)null, null);
		}

		public static void VoidCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran, bool doRelease, ReleaseDelegate CustomPreReleaseDelegate)
			where TNode : ARRegister, ICCPayment, new()
		{
			ReleaseDelegate combinedReleaseDelegate = doRelease ? CustomPreReleaseDelegate + ReleaseARDocument : CustomPreReleaseDelegate;
			VoidCCPayment<TNode>(doc, ccProcTran, combinedReleaseDelegate, null);
		}

		public static void RecordCCPayment<TNode>(TNode doc, string aExtPCTranNbr, string aPCAuthNbr, PXSelectBase<CCProcTran> ccProcTran, bool doRelease)
			where TNode : ARRegister, ICCPayment, new()
		{
			RecordCCPayment<TNode>(doc, aExtPCTranNbr, aPCAuthNbr, ccProcTran, doRelease ? ReleaseARDocument : (ReleaseDelegate)null, null);
		}

		public static void RecordCCCredit<TNode>(TNode doc, string aRefPCTranNbr, string aExtPCTranNbr, string aPCAuthNbr, PXSelectBase<CCProcTran> ccProcTran, bool doRelease)
			where TNode : ARRegister, ICCPayment, new()
		{
			RecordCCCredit<TNode>(doc, aRefPCTranNbr, aExtPCTranNbr, aPCAuthNbr, ccProcTran, doRelease ? ReleaseARDocument : (ReleaseDelegate)null, null);
		}
		public static void AuthorizeCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			AuthorizeCCPayment<TNode>(doc, ccProcTran, null);
		}

		public static void AuthorizeCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran, UpdateDocStateDelegate aDocStateUpdater)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);

				CCPaymentState paymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select());
				if ((paymentState & (CCPaymentState.Captured | CCPaymentState.PreAuthorized)) > 0)
				{
					throw new PXException(Messages.ERR_CCPaymentAlreadyAuthorized);
				}

				ccProcTran.View.Graph.Actions.PressSave();

				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate () {
					try
					{
						CCPaymentEntry.ProcessCCTransaction(toProc, null, CCTranType.AuthorizeOnly);
					}
					finally
					{
						if (aDocStateUpdater != null)
						{
							aDocStateUpdater(doc, CCTranType.AuthorizeOnly);
						}
					}
				});
			}
		}

		public static void VoidCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran, CCPaymentEntry.ReleaseDelegate aReleaseDelegate, UpdateDocStateDelegate aDocStateUpdater)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);

				CCProcTran toVoid = CCPaymentEntry.findCCLastSuccessfulTran(ccProcTran);
				if (toVoid == null)
				{
					throw new PXException(Messages.ERR_CCNoTransactionToVoid);
				}
				else if (toVoid.TranType == CCTranTypeCode.VoidTran || toVoid.TranType == CCTranTypeCode.Credit)
				{
					throw new PXException(Messages.ERR_CCTransactionOfThisTypeInvalidToVoid);
				}

				if (CCPaymentEntry.IsExpired(toVoid))
				{
					throw new PXException(Messages.TransactionHasExpired);
				}

				ccProcTran.View.Graph.Actions.PressSave();
				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate () {
					try
					{
						CCPaymentEntry.ProcessCCTransaction(toProc, toVoid, CCTranType.VoidOrCredit);
					}
					finally
					{
						if (aDocStateUpdater != null)
							aDocStateUpdater(doc, CCTranType.VoidOrCredit);
					}
					if (aReleaseDelegate != null)
						aReleaseDelegate(toProc);

				});
			}
		}

		public static void CreditCCPayment<TNode>(TNode doc, string aPCRefTranNbr, PXSelectBase<CCProcTran> ccProcTran, CCPaymentEntry.ReleaseDelegate aReleaseDelegate, UpdateDocStateDelegate aDocStateUpdater)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);


				ccProcTran.View.Graph.Actions.PressSave();
				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate ()
				{
					try
					{
						CCProcTran refTran = new CCProcTran();
						refTran.TranNbr = null;
						refTran.PCTranNumber = aPCRefTranNbr;
						CCPaymentEntry.ProcessCCTransaction(toProc, refTran, CCTranType.Credit);
					}
					finally
					{
						if (aDocStateUpdater != null)
							aDocStateUpdater(doc, CCTranType.VoidOrCredit);
					}
					if (aReleaseDelegate != null)
						aReleaseDelegate(toProc);

				});
			}
		}

		public static void CaptureCCPayment<TNode>(TNode doc, PXSelectBase<CCProcTran> ccProcTran, CCPaymentEntry.ReleaseDelegate aReleaseDelegate, UpdateDocStateDelegate aDocStateUpdater)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);

				CCPaymentState paymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select());
				if ((paymentState & (CCPaymentState.Captured)) > 0)
				{
					throw new PXException(Messages.ERR_CCAuthorizedPaymentAlreadyCaptured);
				}

				ccProcTran.View.Graph.Actions.PressSave();
				CCProcTran authTran = CCPaymentEntry.findCCPreAthorizing(ccProcTran.Select());
				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				CCProcTran authTranCopy = null;
				if (authTran != null && !CCPaymentEntry.IsExpired(authTran))
					authTranCopy = PXCache<CCProcTran>.CreateCopy(authTran);
				CCTranType operation = (authTranCopy) != null ? CCTranType.PriorAuthorizedCapture : CCTranType.AuthorizeAndCapture;
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate ()
				{
					try
					{
						CCPaymentEntry.ProcessCCTransaction(toProc, authTranCopy, operation);
					}
					finally
					{
						//Update doc state in any case
						if (aDocStateUpdater != null)
							aDocStateUpdater(doc, operation);
					}
					if (aReleaseDelegate != null)
						aReleaseDelegate(toProc);       //On Success Only			

				});
			}
		}

		public static void CaptureOnlyCCPayment<TNode>(TNode doc, string aAuthorizationNbr, PXSelectBase<CCProcTran> ccProcTran, CCPaymentEntry.ReleaseDelegate aReleaseDelegate, UpdateDocStateDelegate aDocStateUpdater)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);

				if (string.IsNullOrEmpty(aAuthorizationNbr))
					throw new PXException(Messages.ERR_CCExternalAuthorizationNumberIsRequiredForCaptureOnlyTrans);

				ccProcTran.View.Graph.Actions.PressSave();
				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate ()
				{
					try
					{
						CCProcTran refTran = new CCProcTran();
						refTran.AuthNumber = aAuthorizationNbr;
						CCPaymentEntry.ProcessCCTransaction(toProc, refTran, CCTranType.CaptureOnly);
					}
					finally
					{
						if (aDocStateUpdater != null)
							aDocStateUpdater(doc, CCTranType.VoidOrCredit);
					}
					if (aReleaseDelegate != null)
						aReleaseDelegate(toProc);
				});
			}
		}


		public static void RecordCCPayment<TNode>(TNode doc, string aExtPCTranNbr, string aPCAuthNbr, PXSelectBase<CCProcTran> ccProcTran, CCPaymentEntry.ReleaseDelegate aReleaseDelegate, UpdateDocStateDelegate aDocStateUpdater)
			where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);

				if (string.IsNullOrEmpty(aExtPCTranNbr))
					throw new PXException(Messages.ERR_PCTransactionNumberOfTheOriginalPaymentIsRequired);


				CCPaymentState paymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select());
				if ((paymentState & (CCPaymentState.Captured)) > 0)
				{
					throw new PXException(Messages.ERR_CCAuthorizedPaymentAlreadyCaptured);
				}


				ccProcTran.View.Graph.Actions.PressSave();
				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				CCProcTran authTran = CCPaymentEntry.findCCPreAthorizing(ccProcTran.Select());
				CCTranType operation = CCTranType.AuthorizeAndCapture;
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate ()
				{
					try
					{
						CCPaymentProcessing ccProcGraph = PXGraph.CreateInstance<CCPaymentProcessing>();
						int? tranID = 0;
						string extTranID = aExtPCTranNbr;
						if (ccProcGraph.RecordCapture(doc, extTranID, aPCAuthNbr, null, ref tranID))
						{

						}
					}
					finally
					{
						//Update doc state in any case
						if (aDocStateUpdater != null)
							aDocStateUpdater(doc, operation);
					}
					if (aReleaseDelegate != null)
						aReleaseDelegate(toProc);       //On Success Only			

				});
			}
		}

		public static void RecordCCCredit<TNode>(TNode doc, string aRefPCTranNbr, string aExtPCTranNbr, string aPCAuthNbr, PXSelectBase<CCProcTran> ccProcTran, CCPaymentEntry.ReleaseDelegate aReleaseDelegate, UpdateDocStateDelegate aDocStateUpdater)
		where TNode : class, IBqlTable, ICCPayment, new()
		{
			if (doc != null && doc.PMInstanceID != null && doc.CuryDocBal != null)
			{
				if (CCPaymentEntry.HasOpenCCTran(ccProcTran))
					throw new PXException(Messages.ERR_CCTransactionCurrentlyInProgress);

				if (string.IsNullOrEmpty(aExtPCTranNbr))
					throw new PXException(Messages.ERR_PCTransactionNumberOfTheOriginalPaymentIsRequired);


				CCPaymentState paymentState = CCPaymentEntry.ResolveCCPaymentState(ccProcTran.Select());
				if ((paymentState & (CCPaymentState.Refunded)) > 0)
				{
					throw new PXException(Messages.ERR_CCPaymentIsAlreadyRefunded);
				}


				ccProcTran.View.Graph.Actions.PressSave();
				TNode toProc = PXCache<TNode>.CreateCopy(doc);
				CCTranType operation = CCTranType.Credit;
				PXLongOperation.StartOperation(ccProcTran.View.Graph, delegate ()
				{
					try
					{
						CCPaymentProcessing ccProcGraph = PXGraph.CreateInstance<CCPaymentProcessing>();
						int? tranID;
						if (ccProcGraph.RecordCredit(doc, aRefPCTranNbr, aExtPCTranNbr, aPCAuthNbr, out tranID))
						{

						}
					}
					finally
					{
						//Update doc state in any case
						if (aDocStateUpdater != null)
							aDocStateUpdater(doc, operation);
					}
					if (aReleaseDelegate != null)
						aReleaseDelegate(toProc);       //On Success Only			

				});
			}
		}

		public delegate void ReleaseDelegate(IBqlTable aTable);

		public delegate void UpdateDocStateDelegate(IBqlTable aDoc, CCTranType aLastOperation);

		public static void ReleaseARDocument(IBqlTable aTable)
		{
			ARRegister toProc = (ARRegister)aTable;
			if (!(toProc.Released ?? false))
			{
				List<ARRegister> list = new List<ARRegister>(1);
				list.Add(toProc);
				ARDocumentRelease.ReleaseDoc(list, false);
			}
				}
			}
}
