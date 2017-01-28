using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.GL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PX.Objects.CA
{
	public class CAReconEntry : PXGraph<CAReconEntry>
	{
		#region Internal Type Definitions
		[Serializable]
		public partial class CABatchDetail2 : CABatchDetail
		{
			#region BatchNbr
			public new abstract class batchNbr : PX.Data.IBqlField
			{
			}
			#endregion
			#region OrigModule
			public new abstract class origModule : PX.Data.IBqlField
			{
			}
			#endregion
			#region OrigDocType
			public new abstract class origDocType : PX.Data.IBqlField
			{
			}
			#endregion
			#region OrigRefNbr
			public new abstract class origRefNbr : PX.Data.IBqlField
			{
			}
			#endregion
		}
		[Serializable]
		public partial class CABankTranMatch2 : CABankTranMatch
		{
			#region CATranID
			public new abstract class cATranID : PX.Data.IBqlField
			{
			}
			#endregion
			#region TranID
			public new abstract class tranID : PX.Data.IBqlField
			{
			}
			#endregion
		}
		[Serializable]
		public partial class CATranR : CATran
		{
			public void CopyFrom(CATran source)
			{
				this.ClearDate = source.ClearDate;
				this.Cleared = source.Cleared;
				this.Reconciled = source.Reconciled;
				this.ReconDate = source.ReconDate;
				this.ReconNbr = source.ReconNbr;
			}
		}
		[Serializable]
		public partial class CATranExt : CATran
		{
			#region CuryEffTranAmt
			public abstract class curyEffTranAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _CuryEffTranAmt;
			[PXCury(typeof(CATran.curyID))]
			[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.Visible)]
			public virtual Decimal? CuryEffTranAmt
			{
				get
				{
					return this._CuryEffTranAmt;
				}
				set
				{
					this._CuryEffTranAmt = value;
				}
			}
			#endregion
			#region EffTranAmt
			public abstract class effTranAmt : PX.Data.IBqlField
			{
			}
			protected Decimal? _EffTranAmt;
			[PXDecimal(4)]
			[PXUIField(DisplayName = "Tran. Amount")]
			public virtual Decimal? EffTranAmt
			{
				get
				{
					return this._EffTranAmt;
				}
				set
				{
					this._EffTranAmt = value;
				}
			}
			#endregion
			#region CuryEffDebitAmt
			public abstract class curyEffDebitAmt : PX.Data.IBqlField
			{
			}

			[PXDecimal()]
			[PXUIField(DisplayName = "Receipt")]
			public virtual Decimal? CuryEffDebitAmt
			{
				[PXDependsOnFields(typeof(drCr), typeof(curyTranAmt))]
				get
				{
					if (this._DrCr == null)
					{
						return null;
					}
					else
					{
						return (this._DrCr == CADrCr.CADebit) ? this._CuryEffTranAmt : 0m;
					}
				}
				set
				{

				}
			}
			#endregion
			#region CuryEffCreditAmt
			public abstract class curyEffCreditAmt : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			//[PXFormula(null, typeof(SumCalc<CARecon.curyReconciledCredits>))]
			[PXUIField(DisplayName = "Disbursement")]
			public virtual Decimal? CuryEffCreditAmt
			{
				[PXDependsOnFields(typeof(drCr), typeof(curyEffTranAmt))]
				get
				{
					if (this._DrCr == null)
					{
						return null;
					}
					else
					{
						return (this._DrCr == CADrCr.CACredit) ? -this._CuryEffTranAmt : 0m;
					}
				}
				set
				{

				}
			}
			#endregion
			#region CuryEffClearedDebitAmt
			public abstract class curyEffClearedDebitAmt : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			[PXUIField(DisplayName = "Receipt")]
			public virtual Decimal? CuryEffClearedDebitAmt
			{
				[PXDependsOnFields(typeof(cleared), typeof(drCr), typeof(curyEffDebitAmt))]
				get
				{
					return (this._Cleared == true ? (this.CuryEffDebitAmt ?? Decimal.Zero) : Decimal.Zero);
				}
				set
				{
				}
			}
			#endregion
			#region CuryEffClearedCreditAmt
			public abstract class curyEffClearedCreditAmt : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			[PXUIField(DisplayName = "Disbursement")]
			public virtual Decimal? CuryEffClearedCreditAmt
			{
				[PXDependsOnFields(typeof(cleared), typeof(drCr), typeof(curyTranAmt))]
				get
				{
					return (this._Cleared == true && this._DrCr == CADrCr.CACredit) ? -this._CuryEffTranAmt : 0m;
				}
				set
				{
				}
			}
			#endregion
			#region Reconcilation Members
			#region ReconciledDebit
			public abstract class reconciledDebit : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			[PXFormula(null, typeof(SumCalc<CARecon.reconciledDebits>))]
			public virtual Decimal? ReconciledDebit
			{
				[PXDependsOnFields(typeof(reconciled), typeof(drCr), typeof(effTranAmt), typeof(skipCount))]
				get
				{
					if (this._SkipCount == true)
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CACredit) ? this._EffTranAmt : 0m;
					}
					else
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CADebit) ? this._EffTranAmt : 0m;
					}
				}
				set
				{
				}
			}
			#endregion
			#region CuryReconciledDebit
			public abstract class curyReconciledDebit : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			[PXFormula(null, typeof(SumCalc<CARecon.curyReconciledDebits>))]
			public virtual Decimal? CuryReconciledDebit
			{
				[PXDependsOnFields(typeof(reconciled), typeof(drCr), typeof(curyEffTranAmt), typeof(skipCount))]
				get
				{
					if (this._SkipCount == true)
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CACredit) ? this._CuryEffTranAmt : 0m;
					}
					else
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CADebit) ? this._CuryEffTranAmt : 0m;
					}
				}
				set
				{
				}
			}
			#endregion
			#region ReconciledCredit
			public abstract class reconciledCredit : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			[PXFormula(null, typeof(SumCalc<CARecon.reconciledCredits>))]
			public virtual Decimal? ReconciledCredit
			{
				[PXDependsOnFields(typeof(reconciled), typeof(drCr), typeof(effTranAmt), typeof(skipCount))]
				get
				{
					if (this._SkipCount == true)
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CADebit) ? -this._EffTranAmt : 0m;
					}
					else
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CACredit) ? -this._EffTranAmt : 0m;
					}
				}
				set
				{
				}
			}
			#endregion
			#region CuryReconciledCredit
			public abstract class curyReconciledCredit : PX.Data.IBqlField
			{
			}
			[PXDecimal()]
			[PXFormula(null, typeof(SumCalc<CARecon.curyReconciledCredits>))]
			public virtual Decimal? CuryReconciledCredit
			{
				[PXDependsOnFields(typeof(reconciled), typeof(drCr), typeof(curyEffTranAmt), typeof(skipCount))]
				get
				{
					if (this._SkipCount == true)
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CADebit) ? -this._CuryEffTranAmt : 0m;
					}
					else
					{
						return (this._Reconciled == true && this._DrCr == CADrCr.CACredit) ? -this._CuryEffTranAmt : 0m;
					}
				}
				set
				{
				}
			}
			#endregion
			#region SkipCount
			public abstract class skipCount : PX.Data.IBqlField
			{
			}
			protected Boolean? _SkipCount;
			/// <summary>
			/// This flag is required for the specific situation when SkipVoids is true and 
			/// transaction was included to Reconcillation Statement before it was voided and then voiding transaction was included.
			/// </summary>
			/// <value>
			/// If this flag is set to true Transaction will not be counted in Reconcillation Statement header and 
			/// Transaction's credit amount will be subtracted from Reconcillation Statement's debit amount 
			/// instead of adding to Reconcillation Statement's credit amount and 
			/// Transaction's debit amount will be subtracted from Reconcillation Statement's credit amount 
			/// instead of adding to Reconcillation Statement's debit amount.  
			/// </value>
			[PXBool()]
			[PXDefault(false)]
			public virtual Boolean? SkipCount
			{
				get
				{
					return this._SkipCount;
				}
				set
				{
					this._SkipCount = value;
				}
			}
			#endregion
			#region CountDebit
			public abstract class countDebit : PX.Data.IBqlField
			{
			}
			[PXInt()]
			[PXFormula(null, typeof(SumCalc<CARecon.countDebit>))]
			public virtual Int32? CountDebit
			{
				[PXDependsOnFields(typeof(reconciled), typeof(drCr))]
				get
				{
					return (this._Reconciled == true && this._DrCr == CADrCr.CADebit && this._SkipCount != true) ? (int)1 : (int)0;
				}
				set
				{
				}
			}
			#endregion
			#region CountCredit
			public abstract class countCredit : PX.Data.IBqlField
			{
			}
			[PXInt()]
			[PXFormula(null, typeof(SumCalc<CARecon.countCredit>))]
			public virtual Int32? CountCredit
			{
				[PXDependsOnFields(typeof(reconciled), typeof(drCr))]
				get
				{
					return (this._Reconciled == true && this._DrCr == CADrCr.CACredit && this._SkipCount != true) ? (int)1 : (int)0;
				}
				set
				{
				}
			}
			#endregion
			#endregion
			#region Voided
			public abstract class voided : PX.Data.IBqlField
			{
			}
			protected Boolean? _Voided;
			[PXBool()]
			[PXUIField(DisplayName = "Voided")]
			public virtual Boolean? Voided
			{
				get
				{
					return this._Voided;
				}
				set
				{
					this._Voided = value;
				}
			}
			#endregion
			#region VoidingTranID
			public abstract class voidingTranID : PX.Data.IBqlField
			{
			}
			protected Int64? _VoidingTranID;
			[PXLong()]
			public virtual Int64? VoidingTranID
			{
				get
				{
					return this._VoidingTranID;
				}
				set
				{
					this._VoidingTranID = value;
				}
			}
			#endregion
			#region VoidingNotReleased
			public abstract class voidingNotReleased : PX.Data.IBqlField
			{
			}
			protected Boolean? _VoidingNotReleased;
			[PXBool()]
			public virtual Boolean? VoidingNotReleased
			{
				get
				{
					return this._VoidingNotReleased;
				}
				set
				{
					this._VoidingNotReleased = value;
				}
			}
			#endregion
			#region Status
			public abstract new class status : PX.Data.IBqlField
			{
			}
			[PXString(11, IsFixed = true)]
			[PXUIField(DisplayName = "Status", Enabled = false)]
			[ExtTranStatus.List()]
			public override string Status
			{
				[PXDependsOnFields(typeof(posted), typeof(released), typeof(hold), typeof(voided), typeof(voidingNotReleased))]
				get
				{
					if (this.VoidingNotReleased == true)
					{
						return ExtTranStatus.PartiallyVoided;
					}
					else if (this.Voided == true)
					{
						return ExtTranStatus.Voided;
					}
					else
						if (this._Posted == true)
					{
						if (this._Released == true)
							return GL.BatchStatus.Posted;
						else
							return GL.BatchStatus.Unposted;
					}
					else if (this._Released == true && this._Posted != true)
					{
						return GL.BatchStatus.Released;
					}
					else if (this._Hold == true)
					{
						return GL.BatchStatus.Hold;
					}
					else
					{
						return GL.BatchStatus.Balanced;
					}
				}
				set
				{
				}
			}
			#endregion
			#region ProcessedFromFeed
			public abstract class processedFromFeed : IBqlField { }

			private bool? _ProcessedFromFeed;

			[PXBool]
			public bool? ProcessedFromFeed
			{
				get { return _ProcessedFromFeed; }
				set { _ProcessedFromFeed = value; }
			}
			#endregion
			#region ProcessedFromStatement
			public abstract class processedFromStatement : IBqlField { }

			private bool? _ProcessedFromStatement;

			[PXBool]
			public bool? ProcessedFromStatement
			{
				get { return _ProcessedFromStatement; }
				set { _ProcessedFromStatement = value; }
			}
			#endregion


			public class ExtTranStatus : BatchStatus
			{
				public new class ListAttribute : PXStringListAttribute
				{
					public ListAttribute()
						: base(
					new string[] { Hold, Balanced, Unposted, Posted, Completed, Voided, Released, PartiallyReleased, Scheduled, PartiallyVoided },
							new string[] { Messages.Hold, Messages.Balanced, GL.Messages.Unposted, GL.Messages.Posted, GL.Messages.Completed, Messages.Voided, Messages.Released, GL.Messages.PartiallyReleased, GL.Messages.Scheduled, CA.Messages.VoidPendingStatus })
					{ }
				}
				public const string PartiallyVoided = "PV";
			}
		}

		[Serializable]
		[UseIndexWhenJoined("CATran_VoidedTranID", On = typeof(On<CATran2.voidedTranID, Equal<CATranExt.tranID>>))]
		[PXCacheName(Messages.CATran2)]
		public partial class CATran2 : CATran
		{
			#region TranID
			public new abstract class tranID : PX.Data.IBqlField
			{
			}
			#endregion
			#region CashAccountID
			public new abstract class cashAccountID : PX.Data.IBqlField
			{
			}
			#endregion
			#region VoidedTranID
			public new abstract class voidedTranID : PX.Data.IBqlField
			{
			}
			#endregion
			#region Released
			public new abstract class released : PX.Data.IBqlField
			{
			}
			#endregion
			#region Reconciled
			public new abstract class reconciled : PX.Data.IBqlField
			{
			}

			#endregion
			#region ReconDate
			public new abstract class reconDate : PX.Data.IBqlField
			{
			}

			#endregion
			#region ReconNbr
			public new abstract class reconNbr : PX.Data.IBqlField
			{
			}
			#endregion
			#region Cleared
			public new abstract class cleared : PX.Data.IBqlField
			{
			}
			#endregion
			#region ClearDate
			public new abstract class clearDate : PX.Data.IBqlField
			{
			}

			#endregion
		}
		#endregion
		#region Buttons Definition
		public PXSave<CARecon> Save;
		#region Button Cancel
		public PXAction<CARecon> cancel;
		[PXCancelButton]
		[PXUIField(MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		protected virtual System.Collections.IEnumerable Cancel(PXAdapter a)
		{
			CARecon current = null;
			CARecon cachesRecon = null;
			PXSelectBase<CARecon> currentSelectStatement;
			object cashAccountCD = a.Searches != null && a.Searches.Length > 0
				? a.Searches[0]
				: null;

			object reconNbr = a.Searches != null && a.Searches.Length > 1
				? a.Searches[1]
				: null;

			foreach (PXResult<CARecon> recon in (new PXCancel<CARecon>(this, "Cancel")).Press(a))
			{
				cachesRecon = recon;
			}

			CashAccount acct = PXSelect<CashAccount,
				Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Select(this, cashAccountCD);

			if (acct != null)
			{
				bool inserted = (cachesRecon != null && CAReconRecords.Cache.GetStatus(cachesRecon) == PXEntryStatus.Inserted);
				bool manualRefFilled = false;
				if (inserted)
				{
					Numbering numbering = PXSelect<Numbering, Where<Numbering.numberingID, Equal<Required<CashAccount.reconNumberingID>>>>.Select(this, acct.ReconNumberingID);
					manualRefFilled = (numbering != null && numbering.UserNumbering == true);
				}

				if (reconNbr != null && !manualRefFilled)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																			And<CARecon.reconNbr, Equal<Required<CARecon.reconNbr>>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID, reconNbr);
				}

				if (current == null && !manualRefFilled)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																			And<CARecon.reconciled, Equal<boolFalse>, And<CARecon.voided, Equal<False>>>>,
																		OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID);
				}

				if (current == null)
				{
					if (inserted)
					{
						current = cachesRecon;
					}
					else
					{
						current = new CARecon();
						current.CashAccountID = acct.CashAccountID;
						current = CAReconRecords.Insert(current);
					}
				}
				if (cachesRecon != null && current != null && (cachesRecon.CashAccountID != current.CashAccountID || cachesRecon.ReconNbr != current.ReconNbr))
				{
					if (CAReconRecords.Cache.GetStatus(cachesRecon) == PXEntryStatus.Inserted)
					{
						CAReconRecords.Delete(cachesRecon);
						CAReconRecords.Cache.IsDirty = false;
					}
					CAReconRecords.Current = current;
				}
			}
			yield return current;

			// return new PXCancel<CARecon>(this, "Cancel").Press(a);
		}
		#endregion
		public PXInsert<CARecon> Insert;
		#region Button Delete
		public PXAction<CARecon> delete;
		[PXDeleteButton]
		[PXUIField]
		protected virtual System.Collections.IEnumerable Delete(PXAdapter a)
		{
			CARecon current = null;
			object cashAccountCD = a.Searches != null && a.Searches.Length > 0
				? a.Searches[0]
				: null;
			foreach (PXResult<CARecon> headerDeleted in (new PXDelete<CARecon>(this, "Delete")).Press(a))
			{
				current = headerDeleted;
			}

			CashAccount acct = PXSelect<CashAccount,
				Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Select(this, cashAccountCD);

			if (acct != null)
			{
				PXSelectBase<CARecon> activitySelect = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																					   And<CARecon.reconciled, Equal<boolFalse>>>,
																					 OrderBy<Asc<CARecon.cashAccountID, Desc<CARecon.reconNbr>>>>(this);
				current = (CARecon)activitySelect.View.SelectSingle(acct.CashAccountID);
			}

			if (current == null)
			{
				current = new CARecon();
				current.CashAccountID = acct.CashAccountID;
				current = CAReconRecords.Insert(current);
				CAReconRecords.Cache.SetStatus(current, PXEntryStatus.Inserted);
			}

			yield return current;
		}
		#endregion
		#region Button First
		public PXAction<CARecon> first;
		[PXFirstButton]
		[PXUIField]
		protected virtual System.Collections.IEnumerable First(PXAdapter a)
		{
			PXLongOperation.ClearStatus(this.UID);
			CARecon current = null;
			CARecon cachesRecon = null;
			PXSelectBase<CARecon> currentSelectStatement;
			object cashAccountCD = a.Searches != null && a.Searches.Length > 0
				? a.Searches[0]
				: null;

			object reconNbr = a.Searches != null && a.Searches.Length > 1
				? a.Searches[1]
				: null;

			foreach (PXResult<CARecon> recon in (new PXFirst<CARecon>(this, "First")).Press(a))
			{
				cachesRecon = recon;
			}

			CashAccount acct = PXSelect<CashAccount,
				Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Select(this, cashAccountCD);

			if (acct != null)
			{
				if (reconNbr != null)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>>,
																		   OrderBy<Asc<CARecon.reconDate, Asc<CARecon.reconNbr>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID);
				}
				if (cachesRecon != null && current != null && (cachesRecon.CashAccountID != current.CashAccountID || cachesRecon.ReconNbr != current.ReconNbr))
				{
					if (CAReconRecords.Cache.GetStatus(cachesRecon) == PXEntryStatus.Inserted)
					{
						CAReconRecords.Delete(cachesRecon);
						CAReconRecords.Cache.IsDirty = false;
					}
					CAReconRecords.Current = current;
				}
			}
			yield return current;
		}
		#endregion
		#region Button Prev
		public PXAction<CARecon> prev;
		[PXPreviousButton]
		[PXUIField]
		protected virtual System.Collections.IEnumerable Prev(PXAdapter a)
		{
			CARecon current = null;
			CARecon currentPre = CAReconRecords.Current;
			CARecon cachesRecon = null;
			PXSelectBase<CARecon> currentSelectStatement;
			object cashAccountCD = a.Searches != null && a.Searches.Length > 0
				? a.Searches[0]
				: null;

			object reconNbr = a.Searches != null && a.Searches.Length > 1
				? a.Searches[1]
				: null;

			foreach (PXResult<CARecon> recon in (new PXPrevious<CARecon>(this, "Prev")).Press(a))
			{
				cachesRecon = recon;
			}

			CashAccount acct = PXSelect<CashAccount,
				Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Select(this, cashAccountCD);

			if (acct != null)
			{
				if (reconNbr != null)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																			And<CARecon.reconNbr, Less<Required<CARecon.reconNbr>>>>,
																		   OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID, reconNbr);
				}
				if (current == null)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>>,
																		   OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID);
				}
				if (cachesRecon != null && current != null && (cachesRecon.CashAccountID != current.CashAccountID || cachesRecon.ReconNbr != current.ReconNbr))
				{
					if (CAReconRecords.Cache.GetStatus(cachesRecon) == PXEntryStatus.Inserted)
					{
						CAReconRecords.Delete(cachesRecon);
						CAReconRecords.Cache.IsDirty = false;
					}
					CAReconRecords.Current = current;
				}
			}
			yield return current;
		}
		#endregion
		#region Button Next
		public PXAction<CARecon> next;
		[PXNextButton]
		[PXUIField]
		protected virtual System.Collections.IEnumerable Next(PXAdapter a)
		{
			PXLongOperation.ClearStatus(this.UID);
			CARecon current = null;
			CARecon cachesRecon = null;
			PXSelectBase<CARecon> currentSelectStatement;
			object cashAccountCD = a.Searches != null && a.Searches.Length > 0
				? a.Searches[0]
				: null;

			object reconNbr = a.Searches != null && a.Searches.Length > 1
				? a.Searches[1]
				: null;

			foreach (PXResult<CARecon> recon in (new PXNext<CARecon>(this, "Next")).Press(a))
			{
				cachesRecon = recon;
			}

			CashAccount acct = PXSelect<CashAccount,
				Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Select(this, cashAccountCD);

			if (acct != null)
			{
				if (reconNbr != null)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																			And<CARecon.reconNbr, Greater<Required<CARecon.reconNbr>>>>,
																		   OrderBy<Asc<CARecon.reconDate, Asc<CARecon.reconNbr>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID, reconNbr);
				}
				if (cachesRecon != null && current != null && (cachesRecon.CashAccountID != current.CashAccountID || cachesRecon.ReconNbr != current.ReconNbr))
				{
					if (CAReconRecords.Cache.GetStatus(cachesRecon) == PXEntryStatus.Inserted)
					{
						CAReconRecords.Delete(cachesRecon);
						CAReconRecords.Cache.IsDirty = false;
					}
					CAReconRecords.Current = current;
				}
			}
			yield return current;
		}
		#endregion
		#region Button Last
		public PXAction<CARecon> last;
		[PXLastButton]
		[PXUIField]
		protected virtual System.Collections.IEnumerable Last(PXAdapter a)
		{
			PXLongOperation.ClearStatus(this.UID);
			CARecon current = null;
			CARecon cachesRecon = null;
			PXSelectBase<CARecon> currentSelectStatement;
			object cashAccountCD = a.Searches != null && a.Searches.Length > 0
				? a.Searches[0]
				: null;

			object reconNbr = a.Searches != null && a.Searches.Length > 1
				? a.Searches[1]
				: null;

			foreach (PXResult<CARecon> recon in (new PXLast<CARecon>(this, "Last")).Press(a))
			{
				cachesRecon = recon;
			}

			CashAccount acct = PXSelect<CashAccount,
				Where<CashAccount.cashAccountCD, Equal<Required<CashAccount.cashAccountCD>>>>.Select(this, cashAccountCD);

			if (acct != null)
			{
				if (reconNbr != null)
				{
					currentSelectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>>,
																		   OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>(this);
					current = (CARecon)currentSelectStatement.View.SelectSingle(acct.CashAccountID);
				}
				if (cachesRecon != null && current != null && (cachesRecon.CashAccountID != current.CashAccountID || cachesRecon.ReconNbr != current.ReconNbr))
				{
					if (CAReconRecords.Cache.GetStatus(cachesRecon) == PXEntryStatus.Inserted)
					{
						CAReconRecords.Delete(cachesRecon);
						CAReconRecords.Cache.IsDirty = false;
					}
					CAReconRecords.Current = current;
				}
			}
			yield return current;
		}
		#endregion

		#region Button Toggle Reconciled
		public PXAction<CARecon> ToggleReconciled;

		[PXUIField(DisplayName = "Toggle Reconciled", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable toggleReconciled(PXAdapter adapter)
		{
			bool? reconciledValue = null;
			if (CAReconTranRecords.Current == null)
				return adapter.Get();
			foreach (CATranExt tran in GetReconTrans())
			{
				if (tran.Released != true)
					continue;

				reconciledValue = reconciledValue ?? !tran.Reconciled;

				CATranExt copy = PXCache<CATranExt>.CreateCopy(tran);
				copy.Reconciled = reconciledValue;
				CAReconTranRecords.Update(copy);
			}
			return adapter.Get();
		}
		#endregion

		#region Button Toggle Cleared
		public PXAction<CARecon> ToggleCleared;

		[PXUIField(DisplayName = "Toggle Cleared", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable toggleCleared(PXAdapter adapter)
		{
			bool? clearedValue = null;

			if (CAReconTranRecords.Current == null)
				return adapter.Get();
			foreach (CATranExt tran in GetReconTrans())
			{
				if (tran.Released != true)
					continue;

				if (tran.Cleared == true && (tran.Reconciled == true || tran.ProcessedFromFeed == true || tran.ProcessedFromStatement == true))
					continue;

				clearedValue = clearedValue ?? !tran.Cleared;
				CATranExt copy = PXCache<CATranExt>.CreateCopy(tran);
				copy.Cleared = clearedValue;
				CAReconTranRecords.Update(copy);
			}
			return adapter.Get();
		}
		#endregion

		#region ToggleReconciled and ToggleCleared helper
		private IEnumerable<CATranExt> GetReconTrans()
		{
			PXSelectBase<CATranExt> trans = new PXSelectJoin<CATranExt, LeftJoin<CABankStatementDetail, On<CATranExt.tranID, Equal<CABankStatementDetail.cATranID>>,
																		LeftJoin<CABankStatement, On<CABankStatementDetail.refNbr, Equal<CABankStatement.refNbr>>,
																		LeftJoin<CATran2, On<CATran2.cashAccountID, Equal<CATranExt.cashAccountID>,
																				And<CATran2.voidedTranID, Equal<CATranExt.tranID>>>,
																		LeftJoin<CABatchDetail, On<CATran.origModule, Equal<CABatchDetail.origModule>,
																			And<CATran.origTranType, Equal<CABatchDetail.origDocType>,
																			And<CATran.origRefNbr, Equal<CABatchDetail.origRefNbr>>>>>>>>,
																		   Where<CATranExt.cashAccountID, Equal<Current<CARecon.cashAccountID>>,
																		   And<Where<CATranExt.reconNbr, Equal<Current<CARecon.reconNbr>>,
																		   Or<Current<CARecon.reconciled>, Equal<boolFalse>,
																		   And<CATranExt.reconNbr, IsNull,
																		   And<Where<CATranExt.tranDate, LessEqual<Optional<CARecon.loadDocumentsTill>>,
																			  Or<Optional<CARecon.loadDocumentsTill>, IsNull>>>>>>>>,
																		   OrderBy<Asc<CATranExt.tranDate>>>(this);
			if (CAReconRecords.Current.ShowBatchPayments == true)
			{
				trans.WhereAnd<Where<CABatchDetail.batchNbr, IsNull>>();
			}
			int start = 0;
			int total = 0;
			bool skipVoids = CAReconRecords.Current.SkipVoided ?? false;

			foreach (PXResult<CATranExt, CABankStatementDetail, CABankStatement, CATran2> record in trans.View.Select(
				PXView.Currents, PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings,
				CAReconTranRecords.View.GetExternalFilters(), ref start, PXView.MaximumRows, ref total))
			{
				CATranExt tran = record;
				if (skipVoids && Skip(tran))
				{
					continue; //Skip voiding transaction
				}
				yield return tran;
			}
			if (CAReconRecords.Current.ShowBatchPayments == true)
			{
				//include batches
				PXCache cache = this.Caches[typeof(CATranExt)];
				foreach (CATranExt inserted in cache.Cached)
				{
					if (inserted.OrigModule == BatchModule.AP && inserted.OrigTranType == CATranType.CABatch)
					{
						PXEntryStatus status = cache.GetStatus(inserted);
						if (status == PXEntryStatus.Notchanged)
							cache.SetStatus(inserted, PXEntryStatus.Held);
						yield return inserted;
					}
				}
			}
		}
		#endregion

		#region Button ReconcileProcessed
		public PXAction<CARecon> ReconcileProcessed;

		[PXUIField(DisplayName = "Reconcile Processed", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable reconcileProcessed(PXAdapter adapter)
		{
			foreach (CATranExt record in CAReconTranRecords.Select())
			{
				if (record.ProcessedFromFeed == true && record.Reconciled == false)
				{
					record.Reconciled = true;
					CAReconTranRecords.Update(record);
				}
			}

			return adapter.Get();
		}
		#endregion

		#region Button Release
		public PXAction<CARecon> Release;
		[PXUIField(DisplayName = "Release", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable release(PXAdapter adapter)
		{
			PXCache cache = this.CAReconRecords.Cache;
			List<CARecon> list = new List<CARecon>();
			foreach (PXResult<CARecon> r in adapter.Get())
			{
				list.Add((CARecon)r);
			}

			Save.Press();

			foreach (CARecon recon in list)
			{
				PXLongOperation.StartOperation(this, () => ReconRelease(recon, true));
			}
			return list;
		}
		#endregion

		#region Button Voided
		public PXAction<CARecon> Voided;
		[PXUIField(DisplayName = "Void", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable voided(PXAdapter adapter)
		{
			CARecon recon = CAReconRecords.Current;
			if (CAReconRecordByCashAccountAndNumbr.Ask(Messages.Void, Messages.ConfirmVoid, MessageButtons.YesNo) == WebDialogResult.Yes)
			{
				PXLongOperation.ClearStatus(this.UID);
				PXLongOperation.StartOperation(this, () => ReconVoided(recon));
				yield return recon;
			}
			else
			{
				yield return recon;
			}
		}
		#endregion

		#region Button ViewDoc
		public PXAction<CARecon> viewDoc;
		[PXUIField(
			DisplayName = "View Document", 
			MapEnableRights = PXCacheRights.Select, 
			MapViewRights = PXCacheRights.Select,
			Visible = false)]
		[PXLookupButton]
		public virtual IEnumerable ViewDoc(PXAdapter adapter)
		{
			CATran tran = CAReconTranRecords.Current;
			CAReconTranRecords.Current = null;
			PXSelect<CATran, Where<CATran.cashAccountID, Equal<Current<CARecon.cashAccountID>>,
				And<Where<CATran.reconNbr, Equal<Current<CARecon.reconNbr>>,
				Or<Current<CARecon.reconciled>, Equal<boolFalse>,
				And<CATran.reconNbr, IsNull,
				And<Where<CATran.tranDate, LessEqual<Current<CARecon.loadDocumentsTill>>,
				Or<Current<CARecon.loadDocumentsTill>, IsNull>>>>>>>>>.Clear(this);
			CATran.Redirect(CAReconRecords.Cache, tran);
			return adapter.Get();
		}
		#endregion

		#region Button CreateAdjustment
		public PXAction<CARecon> CreateAdjustment;
		[PXUIField(DisplayName = "Create Adjustment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable createAdjustment(PXAdapter adapter)
		{
			CARecon header = CAReconRecords.Current;
			if ((header != null) && (this.headerUpdateEnabled(header) == true))
			{
				WebDialogResult result = AddFilter.AskExt(true);
				if (result == WebDialogResult.OK)
				{
					using (new PXTimeStampScope(this.TimeStamp))
					{
						CATran rectran = AddTrxFilter.VerifyAndCreateTransaction(this, AddFilter, currencyinfo);
						Save.Press();
					}
				}
				AddFilter.Cache.Clear();
			}

			yield return header;
		}
		#endregion
		#endregion

		#region Variables
		public ToggleCurrency<CARecon> CurrencyView;

		public PXSelectJoin<CARecon,
				LeftJoin<CashAccount, On<CashAccount.cashAccountID, Equal<CARecon.cashAccountID>>,
				LeftJoin<Account, On<Account.accountID, Equal<CashAccount.accountID>>,
				LeftJoin<Sub, On<Sub.subID, Equal<CashAccount.subID>>>>>,
		 Where<CARecon.cashAccountID, IsNull, Or2<Match<Account, Current<AccessInfo.userName>>, And<Match<Sub, Current<AccessInfo.userName>>>>>,
		 OrderBy<Asc<CARecon.cashAccountID, Desc<CARecon.reconDate>>>> CAReconRecords;
		public PXSelect<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>, And<CARecon.reconNbr, Equal<Required<CARecon.reconNbr>>>>> CAReconRecordByCashAccountAndNumbr;
		[PXFilterable]
		public PXSelectJoin<CATranExt, LeftJoin<CABankStatementDetail, On<CATranExt.tranID, Equal<CABankStatementDetail.cATranID>>,
																LeftJoin<CABankStatement, On<CABankStatementDetail.refNbr, Equal<CABankStatement.refNbr>>,
																LeftJoin<CATran2, On<CATran2.cashAccountID, Equal<CATranExt.cashAccountID>,
																And<CATran2.voidedTranID, Equal<CATranExt.tranID>>>>>>,
																			Where<True, Equal<True>>, OrderBy<Asc<CATran.tranDate>>> CAReconTranRecords;

		public PXSelect<CATran2, Where<CATran2.cashAccountID, Equal<Required<CATran2.cashAccountID>>, And<CATran2.tranID, Equal<Required<CATran2.tranID>>>>> VoidingTrans;
		public PXSelectJoin<CATranExt, InnerJoin<CATran2, On<CATran2.cashAccountID, Equal<CATranExt.cashAccountID>,
																And<CATran2.voidedTranID, Equal<CATranExt.tranID>,
																And<CATran2.released, Equal<False>>>>>,
											Where<CATranExt.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
											And<Where<CATranExt.reconNbr, Equal<Required<CARecon.reconNbr>>>>>> PartiallyVoidedTrans;



		public PXFilter<AddTrxFilter> AddFilter;
		public PXSelect<CurrencyInfo> currencyinfo;

		public PXSetup<CashAccount, Where<CashAccount.reconcile, Equal<boolTrue>>> cashaccount;
		public PXSetup<CASetup> casetup;
		public CMSetupSelect CMSetup;
		public PXSelect<CABatch, Where<CABatch.batchNbr, Equal<Required<CABatch.batchNbr>>>> cabatches;
		public PXSelectJoin<CATranR, InnerJoin<CABatchDetail, On<CATranR.origModule, Equal<CABatchDetail.origModule>,
								And<CATranR.origTranType, Equal<CABatchDetail.origDocType>,
								And<CATranR.origRefNbr, Equal<CABatchDetail.origRefNbr>>>>>, Where<CABatchDetail.batchNbr, Equal<Required<CABatch.batchNbr>>>> transactionsInBatch;
		public PXSelect<CR.BAccountR> BaccountCache;
		//public bool updateEnabled = false;
		//public bool insertEnabled = false;
		#endregion

		#region Execute Select
		protected virtual IEnumerable carecontranrecords()
		{
			if (CAReconRecords.Current == null || CAReconRecords.Current.Voided == true)
			{
				yield break;
			}

			CARecon doc = CAReconRecords.Current;
			CashAccount cashAcct = PXSelect<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CARecon.cashAccountID>>>>.Select(this, doc.CashAccountID);
			if (cashAcct == null)
				yield break;

			HashSet<long> catrans = new HashSet<long>();
			bool skipVoids = doc.SkipVoided ?? false;
			if (doc.ShowBatchPayments == true)
			{
				foreach (CATranExt result in GetBatchTransactions(catrans))
				{
					PXCache cache = this.Caches[typeof(CATranExt)];
					CATranExt tran = null;
					foreach (CATranExt inserted in cache.Cached)
					{
						if (inserted.OrigModule == BatchModule.AP && inserted.OrigTranType == CATranType.CABatch && inserted.OrigRefNbr == result.OrigRefNbr)
						{
							PXEntryStatus status = cache.GetStatus(inserted);
							if (status == PXEntryStatus.Notchanged)
								cache.SetStatus(inserted, PXEntryStatus.Held);
							tran = inserted;
							break;
						}
					}
					if (tran == null)
					{
						tran = result;
						bool isDirty = cache.IsDirty;
						object newVal = null;
						cache.RaiseFieldDefaulting<CATran.tranID>(tran, out newVal);
						tran.TranID = (long?)newVal;
						cache.SetStatus(tran, PXEntryStatus.Held);
					}

					if (!catrans.Contains(tran.TranID.Value))
					{
						catrans.Add(tran.TranID.Value);
						yield return new PXResult<CATranExt, CABankStatementDetail, CABankStatement, CATran2>(tran, null, null, null);
					}
					else
					{
						continue;
					}

				}
			}

			foreach (PXResult<CATranExt, CABankStatementDetail, CABankStatement, CABankTranMatch, CABankTran, CATran2> result in
					PXSelectJoin<CATranExt, LeftJoin<CABankStatementDetail, On<CATranExt.tranID, Equal<CABankStatementDetail.cATranID>>,
																							LeftJoin<CABankStatement, On<CABankStatementDetail.refNbr, Equal<CABankStatement.refNbr>>,
																							LeftJoin<CABankTranMatch, On<CABankTranMatch.cATranID, Equal<CATranExt.tranID>>,
																							LeftJoin<CABankTran, On<CABankTran.tranID, Equal<CABankTranMatch.tranID>>,
																							LeftJoin<CATran2, On<CATran2.voidedTranID, Equal<CATranExt.tranID>>>>>>>,
																		   Where<CATranExt.cashAccountID, Equal<Current<CARecon.cashAccountID>>,
																		   And<Where<CATranExt.reconNbr, Equal<Current<CARecon.reconNbr>>,
																		   Or<Current<CARecon.reconciled>, Equal<boolFalse>,
																		   And<CATranExt.reconNbr, IsNull,
																		   And<Where<CATranExt.tranDate, LessEqual<Current2<CARecon.loadDocumentsTill>>,
																			  Or<Current2<CARecon.loadDocumentsTill>, IsNull>>>>>>>>>.Select(this))
			{
				CATranExt tran = result;
				CATran2 voidingTran = result;
				tran.CuryEffTranAmt = tran.CuryTranAmt;
				tran.EffTranAmt = tran.TranAmt;
				tran.Voided = false;
				tran.VoidingNotReleased = false;
				tran.VoidingTranID = null;
				if (skipVoids)
				{
					if (Skip(tran)) continue; //Skip voiding transaction
					if (voidingTran != null && voidingTran.TranID.HasValue && voidingTran.CashAccountID == tran.CashAccountID)
					{
						if (voidingTran.Released == true)
						{
							if (Skip(voidingTran, tran))
							{
								tran.CuryEffTranAmt += (voidingTran.CuryTranAmt ?? Decimal.Zero);
								tran.EffTranAmt += (voidingTran.TranAmt ?? Decimal.Zero);
							}

							tran.Voided = true;
							tran.VoidingTranID = voidingTran.TranID;
						}
						else
						{
							tran.VoidingNotReleased = true;
							tran.VoidingTranID = voidingTran.TranID;
						}
					}
				}

				CABankTran bankTran = result;
				if (bankTran != null && bankTran.Processed == true)
				{
					tran.ProcessedFromFeed = true;
				}

				CABankStatement statement = result;
				if (statement != null && statement.Released == true)
				{
					tran.ProcessedFromStatement = true;
				}

				PXCache cache = this.Caches[typeof(CATranExt)];
				PXEntryStatus status = cache.GetStatus(tran);
				if (status == PXEntryStatus.Notchanged)
					cache.SetStatus(tran, PXEntryStatus.Held);
				//remove duplicates
				if (!catrans.Contains(tran.TranID.Value))
				{
					catrans.Add(tran.TranID.Value);
					yield return new PXResult<CATranExt, CABankStatementDetail, CABankStatement, CATran2>(tran, result, result, result);
				}
				else
				{
					continue;
				}
			}
		}

		private List<CATranExt> GetBatchTransactions(HashSet<long> catrans)
		{
			HashSet<string> partiallyMatchedBatches = new HashSet<string>();
			HashSet<string> partiallyReconciledBatches = new HashSet<string>();
			var lines = PXSelectJoin<CABatch,
						InnerJoin<CABatchDetail, On<CABatchDetail.batchNbr, Equal<CABatch.batchNbr>>,
						InnerJoin<CATran, On<CATran.origModule, Equal<CABatchDetail.origModule>,
							And<CATran.origTranType, Equal<CABatchDetail.origDocType>,
							And<CATran.origRefNbr, Equal<CABatchDetail.origRefNbr>>>>,
						LeftJoin<CABankTranMatch, On<CABankTranMatch.docRefNbr, Equal<CABatch.batchNbr>,
							And<CABankTranMatch.docModule, Equal<BatchModule.moduleAP>,
							And<CABankTranMatch.docType, Equal<CATranType.cABatch>>>>,
						LeftJoin<CABankTran, On<CABankTran.tranID, Equal<CABankTranMatch.tranID>>,
						LeftJoin<CABankTranMatch2, On<CABankTranMatch2.cATranID, Equal<CATran.tranID>>>>>>>,
						Where<CABatch.cashAccountID, Equal<Current<CARecon.cashAccountID>>,
							And<Where<CABatch.reconNbr, Equal<Current<CARecon.reconNbr>>,
							Or<Current<CARecon.reconciled>, Equal<boolFalse>,
								And<CABatch.reconNbr, IsNull,
							And<Where<CABatch.tranDate, LessEqual<Current2<CARecon.loadDocumentsTill>>,
								Or<Current2<CARecon.loadDocumentsTill>, IsNull>>>>>>>>,
					OrderBy<Asc<CABatch.batchNbr, Desc<CABankTranMatch2.tranID>>>>.Select(this);

			foreach (PXResult<CABatch, CABatchDetail, CATran, CABankTranMatch, CABankTran, CABankTranMatch2> result in lines)
			{
				CATran catran = result;
				CABatch batch = result;
				CABankTranMatch2 match = result;

				if (catran.ReconNbr != this.CAReconRecords.Current.ReconNbr &&
					(this.CAReconRecords.Current.Reconciled == true || catran.ReconNbr != null))
				{
					partiallyReconciledBatches.Add(batch.BatchNbr);
				}

				if (match != null && match.TranID.HasValue)
				{
					partiallyMatchedBatches.Add(batch.BatchNbr);
				}
			}

			List<CATranExt> trans = new List<CATranExt>();
			CATranExt tran = null;
			string prevBatchNbr = null;
			foreach (PXResult<CABatch, CABatchDetail, CATran, CABankTranMatch, CABankTran, CABankTranMatch2> result in lines)
			{
				CATran catran = result;
				CABatch batch = result;

				if (partiallyReconciledBatches.Contains(batch.BatchNbr))
				{
					continue;
				}
				if (partiallyMatchedBatches.Contains(batch.BatchNbr))
				{
					continue;
				}

				catrans.Add(catran.TranID.Value);
				

				if (prevBatchNbr != batch.BatchNbr)
				{
					tran = new CATranExt();
					batch.CopyTo(tran);
					tran.Voided = false;
					tran.VoidingNotReleased = false;
					tran.VoidingTranID = null;
					tran.TranPeriodID = catran.TranPeriodID;
					tran.FinPeriodID = catran.FinPeriodID;
					tran.CuryEffTranAmt = tran.CuryTranAmt;
					tran.EffTranAmt = tran.TranAmt;
					tran.ReferenceID = catran.ReferenceID;
					CABankTran bankTran = result;
					if (bankTran != null && bankTran.Processed == true)
					{
						tran.ProcessedFromFeed = true;
					}
					//old Bank Statement is not supported
					trans.Add(tran);
					prevBatchNbr = batch.BatchNbr;
				}
				else
				{
					if (tran != null && tran.ReferenceID != ((CATran)result).ReferenceID)
						tran.ReferenceID = null;
				}
			}
			return trans;
		}


		#endregion

		#region Implement
		public override void Persist()
		{
			List<CATran> list = new List<CATran>((IEnumerable<CATran>)this.Caches[typeof(CATran)].Updated);

			using (PXTransactionScope ts = new PXTransactionScope())
			{
				base.Persist();

				foreach (CATran tran in list)
				{
					if (tran.OrigModule == BatchModule.AP && tran.OrigTranType == CATranType.CABatch)
					{
						foreach (PXResult<CATranR> res in transactionsInBatch.Select(tran.OrigRefNbr))
						{
							UpdateClearedOnSourceDoc(res);
						}
					}
					else
					{
						UpdateClearedOnSourceDoc(tran);
					}
				}

				ts.Complete();
			}

			foreach (CM.CurrencyInfo iInfo in Caches[typeof(CM.CurrencyInfo)].Cached)
			{
				long? key = this.AddFilter.Current.CuryInfoID;
				if (iInfo.CuryInfoID == key)
				{
					Caches[typeof(CM.CurrencyInfo)].SetStatus(iInfo, PXEntryStatus.Inserted);
				}
			}
		}

		public static void UpdateClearedOnSourceDoc(CATran tran)
		{
			Type table = null;

			var updateStatement = new List<PXDataFieldParam>
				{
					new PXDataFieldAssign("Cleared", tran.Cleared),
					new PXDataFieldAssign("ClearDate", tran.ClearDate)
				};

			switch (tran.OrigModule)
			{
				case GL.BatchModule.AP:
					if (tran.OrigTranType != CATranType.CABatch)
					{
						table = typeof(APPayment);
						updateStatement.Add(new PXDataFieldRestrict<APPayment.docType>(PXDbType.Char, tran.OrigTranType));
						updateStatement.Add(new PXDataFieldRestrict<APPayment.refNbr>(tran.OrigRefNbr));
					}
					else
					{
						table = typeof(CABatch);
						updateStatement.Add(new PXDataFieldRestrict<CABatch.batchNbr>(tran.OrigRefNbr));
					}
					break;
				case GL.BatchModule.AR:
					table = typeof(ARPayment);
					updateStatement.Add(new PXDataFieldRestrict<ARPayment.docType>(PXDbType.Char, tran.OrigTranType));
					updateStatement.Add(new PXDataFieldRestrict<ARPayment.refNbr>(tran.OrigRefNbr));
					break;
				case GL.BatchModule.CA:
					switch (tran.OrigTranType)
					{
						case CATranType.CADeposit:
						case CATranType.CAVoidDeposit:
							table = typeof(CADeposit);
							updateStatement.Add(new PXDataFieldRestrict<CADeposit.tranType>(PXDbType.Char, tran.OrigTranType));
							updateStatement.Add(new PXDataFieldRestrict<CADeposit.refNbr>(tran.OrigRefNbr));
							break;
						case CATranType.CAAdjustment:
							table = typeof(CAAdj);
							updateStatement.Add(new PXDataFieldRestrict<CAAdj.adjTranType>(PXDbType.Char, tran.OrigTranType));
							updateStatement.Add(new PXDataFieldRestrict<CAAdj.adjRefNbr>(tran.OrigRefNbr));
							break;
						case CATranType.CATransferExp:
							table = typeof(CAAdj);
							updateStatement.Add(new PXDataFieldRestrict<CAAdj.adjTranType>(PXDbType.Char, tran.OrigTranType));
							updateStatement.Add(new PXDataFieldRestrict<CAAdj.tranID>(tran.TranID));
							break;
						case CATranType.CATransferIn:
							table = typeof(CATransfer);
							updateStatement = new List<PXDataFieldParam>
								{
									new PXDataFieldAssign<CATransfer.clearedIn>(tran.Cleared),
									new PXDataFieldAssign<CATransfer.clearDateIn>(tran.ClearDate),
									new PXDataFieldRestrict<CATransfer.transferNbr>(tran.OrigRefNbr)
								};
							break;
						case CATranType.CATransferOut:
							table = typeof(CATransfer);
							updateStatement = new List<PXDataFieldParam>
								{
									new PXDataFieldAssign<CATransfer.clearedOut>(tran.Cleared),
									new PXDataFieldAssign<CATransfer.clearDateOut>(tran.ClearDate),
									new PXDataFieldRestrict<CATransfer.transferNbr>(tran.OrigRefNbr)
								};
							break;
					}
					break;
			}

			if (table != null && updateStatement.Any())
			{
				PXDatabase.Update(table, updateStatement.ToArray());
			}
		}

		public override void Clear()
		{
			if (AddFilter.Current != null)
			{
				AddFilter.Current.TranDate = null;
				AddFilter.Current.FinPeriodID = null;
				AddFilter.Current.CuryInfoID = null;
			}
			base.Clear();
		}

		public CAReconEntry()
		{
			CASetup setup = casetup.Current;
			PXCache tranCache = CAReconTranRecords.Cache;

			PXParentAttribute.SetLeaveChildren<CATran.reconNbr>(tranCache, null, true);
			Views["_AddTrxFilter.curyInfoID_CurrencyInfo.CuryInfoID_"] = new PXView(this, false, new Select<AddTrxFilter>(), new PXSelectDelegate(AddFilter.Get));

			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyClearedCreditAmt>(tranCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyClearedDebitAmt>(tranCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyCreditAmt>(tranCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyDebitAmt>(tranCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyReconciledCredit>(tranCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyReconciledDebit>(tranCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CATranExt.curyTranAmt>(tranCache, null, false);

			PXCache reconCache = CAReconRecords.Cache;
			PXDBCurrencyAttribute.SetBaseCalc<CARecon.curyReconciledBalance>(reconCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CARecon.curyReconciledCredits>(reconCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CARecon.curyReconciledDebits>(reconCache, null, false);
			PXDBCurrencyAttribute.SetBaseCalc<CARecon.curyReconciledTurnover>(reconCache, null, false);
			PXUIFieldAttribute.SetVisible<CARecon.curyID>(reconCache, null, PXAccess.FeatureInstalled<FeaturesSet.multicurrency>());
		}
		#endregion

		#region Functions

		public static void ReconCreate(CashAccount acct)
		{
			if (acct == null) return;
			if (acct.Reconcile != true)
				throw new Exception(Messages.CashAccounNotReconcile);
			CAReconEntry graph = PXGraph.CreateInstance<CAReconEntry>();
			PXSelectBase<CARecon> selectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>>,
																					  OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>(graph);
			CARecon lastRecon = (CARecon)selectStatement.View.SelectSingle(acct.CashAccountID);
			if (lastRecon != null && lastRecon.Reconciled != true)
				throw new Exception(Messages.CanNotCreateStatement);

			CARecon current = new CARecon();
			current.CashAccountID = acct.CashAccountID;
			graph.CAReconRecords.Insert(current);
			throw new PXRedirectRequiredException(graph, "Document");
		}

		public static void ReconVoided(CARecon recon)
		{

			CAReconEntry graph = PXGraph.CreateInstance<CAReconEntry>();

			if (!IsVoidAllowed(recon, graph))
				throw new Exception(Messages.CanNotVoidStatement);

			recon = (CARecon)graph.CAReconRecords.Update(recon);
			decimal? reconciledDebits = recon.ReconciledDebits;
			decimal? reconciledCredits = recon.ReconciledCredits;
			decimal? curyReconciledDebits = recon.CuryReconciledDebits;
			decimal? curyReconciledCredits = recon.CuryReconciledCredits;
			int? countDebit = recon.CountDebit;
			int? countCredit = recon.CountCredit;
			CATranExt newrow;
			foreach (CATranExt res in PXSelect<CATranExt, Where<CATranExt.reconNbr, Equal<Required<CARecon.reconNbr>>, And<CATranExt.cashAccountID, Equal<Required<CARecon.cashAccountID>>>>>
												  .Select(graph, recon.ReconNbr, recon.CashAccountID))
			{
				if (res.Reconciled == true)
				{
					newrow = PXCache<CATranExt>.CreateCopy(res);
					newrow.Reconciled = false;
					graph.CAReconTranRecords.Update(newrow);
				}
			}
			CABatch newbatch;
			foreach (CABatch res in PXSelect<CABatch, Where<CABatch.reconNbr, Equal<Required<CARecon.reconNbr>>, And<CABatch.cashAccountID, Equal<Required<CARecon.cashAccountID>>>>>
												  .Select(graph, recon.ReconNbr, recon.CashAccountID))
			{
				if (res.Reconciled == true)
				{
					newbatch = PXCache<CABatch>.CreateCopy(res);
					newbatch.Reconciled = false;
					newbatch.ReconNbr = null;
					newbatch.ReconDate = null;
					graph.cabatches.Update(newbatch);
				}
			}

			CARecon newheader = (CARecon)graph.CAReconRecords.Cache.CreateCopy(recon);
			newheader.Reconciled = false;
			newheader.Voided = true;
			newheader.ReconciledDebits = reconciledDebits;
			newheader.ReconciledCredits = reconciledCredits;
			newheader.CuryReconciledDebits = curyReconciledDebits;
			newheader.CuryReconciledCredits = curyReconciledCredits;
			newheader.CountDebit = countDebit;
			newheader.CountCredit = countCredit;
			graph.CAReconRecords.Update(newheader);

			graph.Save.Press();
		}

		private static bool IsVoidAllowed(CARecon recon, CAReconEntry graph)
		{
			PXSelectBase<CARecon> statements = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																				And<CARecon.reconDate, GreaterEqual<Required<CARecon.reconDate>>>>,
																					  OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>(graph);

			foreach (CARecon statement in statements.Select(recon.CashAccountID, recon.ReconDate))
			{
				if (statement != null)
				{
					if (statement.ReconNbr == recon.ReconNbr)
						return statement.Reconciled ?? false;
					else if (statement.Voided == false)
						return false;
				}
			}

			return false;
		}

		public static void ReconRelease(CARecon recon, bool updateCATranInfo)
		{
			if (recon.Hold == true)
			{
				throw new PXException(AP.Messages.Document_OnHold_CannotRelease);
			}

			CAReconEntry graph = PXGraph.CreateInstance<CAReconEntry>();
			List<CATran> tranList = new List<CATran>();
			bool skipVoids = recon.SkipVoided ?? false;
			if (skipVoids)
			{
				CATranExt tran = graph.PartiallyVoidedTrans.Select(recon.CashAccountID, recon.ReconNbr);
				if (tran != null && tran.TranID.HasValue)
				{
					throw new PXException(CA.Messages.TransactionsWithVoidPendingStatusMayNotBeAddedToReconciliation);
				}
			}

			foreach (CATran transToRelease in PXSelect<CATran, Where<CATran.reconNbr, Equal<Required<CARecon.reconNbr>>, And<CATran.cashAccountID, Equal<Required<CARecon.cashAccountID>>>>>
												  .Select(graph, recon.ReconNbr, recon.CashAccountID))
			{
				if (transToRelease.Reconciled == true &&
					transToRelease.Cleared == true)
				{
					transToRelease.Selected = true;
					tranList.Add(transToRelease);
				}
			}
			try
			{
				bool releaseComplete = true;
				if (tranList.Count > 0)
				{
					CATrxRelease.GroupReleaseTransaction(tranList, true, true, updateCATranInfo);

					for (int i = 0; i < tranList.Count; i++)
					{
						CATran tran = (CATran)PXSelectReadonly<CATran, Where<CATran.tranID, Equal<Required<CATran.tranID>>>>.Select(graph, ((CATran)tranList[i]).TranID);
						if (tran != null && tran.Reconciled == true && tran.Released != true)
						{
							releaseComplete = false;
							break;
						}
					}
				}

				if (releaseComplete == true)
				{
					using (new PXConnectionScope())
					{
						using (PXTransactionScope ts = new PXTransactionScope())
						{
							recon.Reconciled = true;
							graph.CAReconRecords.Update(recon);
							graph.Save.Press();
							ts.Complete();
						}
					}
				}
				else
					throw new Exception(Messages.OneOrMoreItemsAreNotReleasedAndStatementCannotBeCompleted);
			}
			catch (Exception)
			{
				throw new Exception(Messages.OneOrMoreItemsAreNotReleasedAndStatementCannotBeCompleted);
			}

		}

		public virtual CARecon GetLastRecon(CARecon row)
		{
			CARecon last = null;
			if (row != null && row.CashAccountID != null)
			{
				PXSelectBase<CARecon> selectStatement = new PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>, And<CARecon.voided, NotEqual<True>>>,
																					  OrderBy<Asc<CARecon.cashAccountID, Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>>(this);
				last = (CARecon)selectStatement.View.SelectSingle(row.CashAccountID);
			}
			return last;
		}

		//public virtual bool headerInsertEnabled(CARecon aRow)
		//{
		//    return ((aRow == null) || (aRow.Reconciled == true) || aRow.Voided == true);
		//}

		public virtual bool headerUpdateEnabled(CARecon _header)
		{
			return ((_header.Reconciled != true) && (_header.Voided != true) && (_header.CashAccountID != null));
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
				else
				{
					CMSetup cmsetup = PXSelect<CMSetup>.Select(this);
					if (cmsetup != null && !string.IsNullOrEmpty(cmsetup.CARateTypeDflt))
					{
						e.NewValue = cmsetup.CARateTypeDflt;
						e.Cancel = true;
					}
				}
			}
		}

		protected virtual void CurrencyInfo_CuryEffDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			if (CAReconRecords.Current != null)
			{
				e.NewValue = CAReconRecords.Current.ReconDate;
				e.Cancel = true;
			}
		}

		protected virtual void CurrencyInfo_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			if (AddFilter.Current != null && AddFilter.Current.CuryInfoID == ((CurrencyInfo)e.Row).CuryInfoID)
			{
				e.Cancel = true;
			}
		}

		#endregion

		#region CATran Events
		protected virtual void CATranExt_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;
			bool skipVoids = this.CAReconRecords.Current == null ? false : this.CAReconRecords.Current.SkipVoided ?? false;
			if (row.OrigModule == BatchModule.AP && row.OrigTranType == CATranType.CABatch)
			{
				e.Cancel = true;
			}
			if (row.Reconciled == true)
			{
				if (row.Cleared != true)
				{
					sender.RaiseExceptionHandling<CATran.cleared>(row, row.Cleared, new PXSetPropertyException(Messages.ReconciledDocCanNotBeNotCleared));
				}

				if (row.ClearDate == null)
				{
					sender.RaiseExceptionHandling<CATran.clearDate>(row, row.ClearDate, new PXSetPropertyException(Messages.ReconciledDocCanNotBeNotCleared));
				}

				if (row.Released == false)
				{
					sender.RaiseExceptionHandling<CATran.reconciled>(row, row.Reconciled, new PXSetPropertyException(Messages.NotReleasedDocCanNotAddToReconciliation));
				}

				if (skipVoids && row.VoidingTranID.HasValue && row.VoidingNotReleased == true)
				{
					sender.RaiseExceptionHandling<CATran.reconciled>(row, row.ClearDate, new PXSetPropertyException(Messages.VoidedTransactionHavingNotReleasedVoidCannotBeAddedToReconciliation));
				}
			}

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				CARecon header = CAReconRecords.Current;
				if (row.Reconciled == true)
				{
					row.ReconNbr = header.ReconNbr;
					row.ReconDate = header.ReconDate;
					UpdateBatchTransactions(row);
				}
				else
				{
					row.ReconNbr = null;
					row.ReconDate = null;
				}
			}
		}
		protected virtual void CATranExt_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			CATran row = (CATran)e.Row;

			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update) &&
				e.TranStatus == PXTranStatus.Aborted && row.Reconciled == true)
			{
				CARecon header = CAReconRecords.Current;
				row.ReconNbr = header.ReconNbr;
				row.ReconDate = header.ReconDate;
			}
		}
		protected virtual void CATranExt_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;
			CARecon recon = CAReconRecords.Current;
			if (row == null || recon == null) return;
			bool skipVoids = recon.SkipVoided ?? false;
			bool isReconcilable = (recon.Reconciled != true);
			PXUIFieldAttribute.SetEnabled(cache, row, false);
			PXUIFieldAttribute.SetEnabled<CATran.tranID>(cache, row, false);
			PXUIFieldAttribute.SetEnabled<CATran.hold>(cache, row, false);
			PXUIFieldAttribute.SetEnabled<CATran.reconciled>(cache, row, isReconcilable);

			bool allowClear = isReconcilable && row.ProcessedFromFeed != true && row.ProcessedFromStatement != true;

			PXUIFieldAttribute.SetEnabled<CATran.cleared>(CAReconTranRecords.Cache, row, allowClear);
			PXUIFieldAttribute.SetEnabled<CATran.clearDate>(CAReconTranRecords.Cache, row, allowClear && row.Cleared == true);
			if (isReconcilable)
			{
				if (row.Released != true || (skipVoids && row.VoidingNotReleased == true && row.VoidingTranID.HasValue))
				{
					PXUIFieldAttribute.SetEnabled(cache, row, false);
					if (skipVoids && row.VoidingNotReleased == true && row.VoidingTranID.HasValue && row.Reconciled == true)
					{
						PXUIFieldAttribute.SetEnabled<CATran.reconciled>(cache, row, true);
					}
				}
			}
		}
		protected virtual void CATranExt_RowUpdating(PXCache cache, PXRowUpdatingEventArgs e)
		{
			CATranExt newRow = (CATranExt)e.NewRow;
			CATranExt row = (CATranExt)e.Row;
			CARecon recon = CAReconRecords.Current;
			if (newRow == null || row == null || recon == null) return;
			bool skipVoids = recon.SkipVoided ?? false;
			if (skipVoids && newRow.Reconciled == true && Skip(newRow, null, false) && !Skip(row, null, false))
			{
				CATranExt voidedTran = CAReconTranRecords.Locate(new CATranExt() { TranID = newRow.VoidedTranID });
				if (voidedTran.Reconciled == true)
				{
					newRow.SkipCount = true;
					CAReconTranRecords.View.RequestRefresh();
				}
			}
			else if (skipVoids && newRow.Reconciled == false && newRow.VoidingTranID.HasValue)
			{
				CAReconTranRecords.View.RequestRefresh();
			}
		}
		protected virtual void CATranExt_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;
			CATranExt oldRow = (CATranExt)e.OldRow;
			CARecon recon = CAReconRecords.Current;
			if (row == null || recon == null) return;
			bool skipVoids = recon.SkipVoided ?? false;
			if (skipVoids && row.VoidingTranID.HasValue)
			{
				UpdateVoidingTran(row, oldRow, row.VoidingTranID);
			}
			if (skipVoids && row.VoidedTranID.HasValue && row.Reconciled != oldRow.Reconciled)
			{
				CATranExt voidedTran = PXSelect<CATranExt, Where<CATranExt.cashAccountID, Equal<Required<CATranExt.cashAccountID>>,
													And<CATranExt.tranID, Equal<Required<CATranExt.tranID>>>>>.Select(this, row.CashAccountID, row.VoidedTranID);
				if (voidedTran != null && voidedTran.TranID.HasValue)
				{
					if (voidedTran.Reconciled == true && voidedTran.ReconNbr == row.ReconNbr)
					{
						UpdateVoidingTran(row, oldRow, row.TranID);
						cache.SetStatus(row, PXEntryStatus.Notchanged);
					}
				}
			}
			UpdateBatchTransactions(row);
			row.SkipCount = false;
		}

		private void UpdateVoidingTran(CATranExt row, CATranExt oldRow, long? voidingTranID)
		{
			CATran2 voidingTran = null;
			voidingTran = this.VoidingTrans.Select(row.CashAccountID, voidingTranID);
			if (voidingTran != null && voidingTran.TranID.HasValue)
			{
				bool isModified = false;
				if (row.Cleared != oldRow.Cleared || row.ClearDate != oldRow.ClearDate)
				{
					voidingTran.ClearDate = row.ClearDate;
					voidingTran.Cleared = row.Cleared;
					isModified = true;
				}
				if (row.Reconciled != oldRow.Reconciled || row.ReconDate != oldRow.ReconDate || row.ReconNbr != oldRow.ReconNbr)
				{
					isModified = true;
					voidingTran.Reconciled = row.Reconciled;
					voidingTran.ReconDate = row.ReconDate;
					voidingTran.ReconNbr = row.ReconNbr;
					if (voidingTran.Reconciled == true && voidingTran.Cleared != true)
					{
						voidingTran.ClearDate = row.ClearDate;
						voidingTran.Cleared = row.Cleared;
					}
				}
				if (isModified)
				{
					voidingTran = (CATran2)this.VoidingTrans.Update(voidingTran);
				}
			}
		}

		private void UpdateBatchTransactions(CATranExt row)
		{
			if (row.OrigModule == BatchModule.AP && row.OrigTranType == CATranType.CABatch)
			{
				foreach (CATranR innerTran in transactionsInBatch.Select(row.OrigRefNbr))
				{
					innerTran.CopyFrom(row);
					transactionsInBatch.Update(innerTran);
				}
				CABatch batch = cabatches.Select(row.OrigRefNbr);
				batch.Cleared = row.Cleared;
				batch.ClearDate = row.ClearDate;
				batch.Reconciled = row.Reconciled;
				batch.ReconDate = row.ReconDate;
				batch.ReconNbr = row.ReconNbr;
				cabatches.Update(batch);
			}
		}
		protected virtual void CATranExt_Reconciled_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;
			bool skipVoids = this.CAReconRecords.Current.SkipVoided ?? false;
			if ((bool)e.NewValue == true)
			{
				if (row.Hold == true)
				{
					e.NewValue = false;
					e.Cancel = true;
					throw new PXSetPropertyException(Messages.HoldDocCanNotAddToReconciliation);
				}
				if (row.Released != true)
				{
					e.NewValue = false;
					e.Cancel = true;
					throw new PXSetPropertyException(Messages.NotReleasedDocCanNotAddToReconciliation);
				}
				if (skipVoids == true && row.VoidingTranID.HasValue && row.VoidingNotReleased == true)
				{
					e.NewValue = false;
					e.Cancel = true;
					throw new PXSetPropertyException(Messages.TransactionsWithVoidPendingStatusMayNotBeAddedToReconciliation);
				}

				if (skipVoids == true && Skip(row))
				{
					e.NewValue = false;
					e.Cancel = true;
					throw new PXSetPropertyException(Messages.TransactionsWithVoidPendingStatusMayNotBeAddedToReconciliation);
				}
			}
		}
		protected virtual void CATranExt_Cleared_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;
			if (row.Cleared == true)
			{
				row.ClearDate = CAReconRecords.Current.ReconDate;
			}
			else
			{
				row.ClearDate = null;
			}
		}
		protected virtual void CATranExt_Reconciled_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;

			if (row.Reconciled == true)
			{
				row.Cleared = true;
				row.ReconNbr = CAReconRecords.Current.ReconNbr;
				row.ReconDate = CAReconRecords.Current.ReconDate;
				if (row.ClearDate == null)
				{
					row.ClearDate = CAReconRecords.Current.ReconDate;
				}
			}
			else
			{
				row.ReconNbr = null;
				row.ReconDate = null;
			}
		}
		protected virtual void CATranExt_Status_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			CATranExt row = (CATranExt)e.Row;
			if (row != null)
			{
				bool skipVoids = (this.CAReconRecords.Current.SkipVoided ?? false);
				Dictionary<long, CAMessage> listMessages = PXLongOperation.GetCustomInfo(this.UID) as Dictionary<long, CAMessage>;
				TimeSpan timespan;
				Exception ex;
				PXLongRunStatus status = PXLongOperation.GetStatus(this.UID, out timespan, out ex);
				string fieldName = typeof(CATranExt.status).Name;
				if ((status == PXLongRunStatus.Aborted || status == PXLongRunStatus.Completed) && listMessages != null)
				{
					CAMessage message = null;
					if (listMessages.ContainsKey(row.TranID.Value))
						message = listMessages[row.TranID.Value];
					if (message != null)
					{

						e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
									null, null, message.Message, message.ErrorLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
						e.IsAltered = true;
					}
				}
				else
				{
					string message = String.Empty;
					PXErrorLevel msgLevel = PXErrorLevel.RowWarning;
					if (skipVoids && row.VoidingNotReleased == true && row.VoidingTranID.HasValue)
					{
						message = CA.Messages.VoidedTransactionHavingNotReleasedVoidCannotBeAddedToReconciliation;
						if (row.Reconciled == true)
						{
							msgLevel = PXErrorLevel.RowError;
						}
					}
					else if (row.Released != true)
					{
						message = Messages.NotReleasedDocCanNotAddToReconciliation;
					}
					else if (row.Hold == true)
					{
						message = Messages.NotReleasedDocCanNotAddToReconciliation;
					}

					if (string.IsNullOrEmpty(message) == false)
					{
						e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, typeof(String), false, null, null, null, null, null, fieldName,
						 null, null, message, msgLevel, null, null, null, PXUIVisibility.Undefined, null, null, null);
						e.IsAltered = true;
					}

				}
			}
		}

		protected virtual void CATran2_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CATran2 row = (CATran2)e.Row;
			if (row.Reconciled == true)
			{
				if (row.Cleared != true)
				{
					sender.RaiseExceptionHandling<CATran.cleared>(row, row.Cleared, new PXSetPropertyException(Messages.ReconciledDocCanNotBeNotCleared));
				}
				if (row.ClearDate == null)
				{
					sender.RaiseExceptionHandling<CATran.clearDate>(row, row.ClearDate, new PXSetPropertyException(Messages.ReconciledDocCanNotBeNotCleared));
				}
			}

			if ((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update)
			{
				CARecon header = CAReconRecords.Current;
				if (row.Reconciled == true)
				{
					row.ReconNbr = header.ReconNbr;
					row.ReconDate = header.ReconDate;
				}
				else
				{
					row.ReconNbr = null;
					row.ReconDate = null;
				}
			}
		}
		protected virtual void CATran2_RowPersisted(PXCache sender, PXRowPersistedEventArgs e)
		{
			CATran2 row = (CATran2)e.Row;

			if (((e.Operation & PXDBOperation.Command) == PXDBOperation.Insert ||
				(e.Operation & PXDBOperation.Command) == PXDBOperation.Update) &&
				e.TranStatus == PXTranStatus.Aborted && row.Reconciled == true)
			{
				CARecon header = CAReconRecords.Current;
				row.ReconNbr = header.ReconNbr;
				row.ReconDate = header.ReconDate;
			}
		}


		#endregion

		#region CARecon Events
		protected virtual void CArecon_RowDeleting(PXCache cache, PXRowDeletingEventArgs e)
		{
			CARecon row = (CARecon)e.Row;
			if (row.Reconciled == true || row.Voided == true)
			{
				throw new PXException(Messages.CannotDeleteRecon);
			}
		}
		protected virtual void CARecon_RowDeleted(PXCache cache, PXRowDeletedEventArgs e)
		{
			CARecon header = (CARecon)e.Row;
			foreach (CATranExt res in CAReconTranRecords.Select())
			{
				if (res.Reconciled == true)
				{
					CATranExt newTran = (CATranExt)CAReconTranRecords.Cache.CreateCopy(res);
					newTran.Reconciled = false;
					CAReconTranRecords.Update(newTran);
				}
			}
			if (CAReconRecords.Cache.GetStatus(header) != PXEntryStatus.Inserted)
			{
				Save.Press();
			}
		}
		protected virtual void CARecon_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			CARecon row = (CARecon)e.Row;
			//CARecon previousHeader = this.previousHeader(row);
			PXEntryStatus status = cache.GetStatus(row);
			if (status == PXEntryStatus.Inserted)
			{
				CARecon openRecon = PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>, And<CARecon.voided, Equal<False>, And<CARecon.reconciled, Equal<False>>>>>.Select(this, row.CashAccountID);
				if (openRecon != null)
					cache.RaiseExceptionHandling<CARecon.reconNbr>(row, row.ReconNbr, new PXSetPropertyException(Messages.PrevStatementNotReconciled));
			}
			if (row.Voided == false)
			{
				CARecon laterRecon = PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>, And<CARecon.voided, Equal<False>,
								And<CARecon.reconNbr, NotEqual<Required<CARecon.reconNbr>>,
								And<CARecon.reconDate, GreaterEqual<Required<CARecon.reconDate>>>>>>>.Select(this, row.CashAccountID, row.ReconNbr, row.ReconDate);
				if (laterRecon != null)
				{
					cache.RaiseExceptionHandling<CARecon.reconDate>(row, row.ReconDate, new PXSetPropertyException(Messages.ReconDateNotAvailable));
				}
			}


			if (row.Hold != true && row.Voided != true)
			{
				if (row.CuryDiffBalance != 0)
				{
					cache.RaiseExceptionHandling<CARecon.curyBalance>(row, row.CuryBalance, new PXSetPropertyException(Messages.DocumentOutOfBalance));
				}
			}
		}
		protected virtual void CARecon_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
		{
			CARecon header = (CARecon)e.Row;
			if (header == null)
			{
				CAReconTranRecords.Cache.AllowDelete = false; 
				CAReconTranRecords.Cache.AllowInsert = false; 
				CAReconTranRecords.Cache.AllowUpdate = false;
				CAReconRecords.Cache.AllowUpdate = false;
				CAReconRecords.Cache.AllowDelete = false;
				CAReconRecords.Cache.AllowInsert = false;
				next.SetEnabled(false);
				prev.SetEnabled(false);
				first.SetEnabled(false);
				last.SetEnabled(false);
				return;
			}

			if (header.CashAccountID != null && (cashaccount.Current == null || cashaccount.Current.CashAccountID != header.CashAccountID))
			{
				cashaccount.Current = PXSelectReadonly<CashAccount, Where<CashAccount.cashAccountID, Equal<Required<CashAccount.cashAccountID>>>>.Select(this, header.CashAccountID);
			}
			if (cashaccount.Current != null && AddFilter.Current != null && AddFilter.Current.CashAccountID != header.CashAccountID)
			{
				AddFilter.Cache.SetValueExt<AddTrxFilter.cashAccountID>(AddFilter.Current, cashaccount.Current.CashAccountCD);
			}

			header.CuryReconciledTurnover = header.CuryReconciledDebits - header.CuryReconciledCredits;
			header.CuryReconciledBalance = header.CuryBegBalance + header.CuryReconciledTurnover;
			header.CuryDiffBalance = header.CuryBalance - header.CuryReconciledBalance;

			//PXUIFieldAttribute.SetEnabled(cache, header, false);
			PXUIFieldAttribute.SetEnabled<CARecon.reconNbr>(cache, header, true);
			PXUIFieldAttribute.SetEnabled<CARecon.cashAccountID>(cache, header, true);

			bool headerReconciled = (header.Reconciled == true);
			bool headerOnHold = (header.Hold == true);
			bool headerVoided = (header.Voided == true);
			bool headerEnalable = headerOnHold && !headerReconciled && !headerVoided;


			CARecon lastHeader = this.GetLastRecon(header);

			bool voidEnabled = header.Reconciled == true && header.Voided == false && (lastHeader != null && (lastHeader.ReconNbr == header.ReconNbr));
			bool insertEnabled = (header.CashAccountID != null) && (lastHeader == null || lastHeader.Reconciled == true || lastHeader.Voided == true);
			bool updateEnabled = this.headerUpdateEnabled(header);
			bool deleteEnabled = !headerReconciled && !headerVoided && (cache.GetStatus(header) == PXEntryStatus.Inserted || lastHeader != null && lastHeader.ReconNbr == header.ReconNbr);
			bool releaseEnabled = !headerReconciled && !headerOnHold && !headerVoided;

			Voided.SetEnabled(voidEnabled);

			CAReconRecords.Cache.AllowUpdate = updateEnabled;
			CAReconRecords.Cache.AllowDelete = deleteEnabled;
			CAReconRecords.Cache.AllowInsert = insertEnabled;

			CAReconTranRecords.Cache.AllowDelete = false; // updateEnabled;
			CAReconTranRecords.Cache.AllowInsert = false; // updateEnabled;
			CAReconTranRecords.Cache.AllowUpdate = updateEnabled;

			PXUIFieldAttribute.SetEnabled<CARecon.curyBalance>(cache, header, !headerReconciled && !headerVoided);
			PXUIFieldAttribute.SetEnabled<CARecon.hold>(cache, header, !headerReconciled && !headerVoided);
			PXUIFieldAttribute.SetEnabled<CARecon.reconDate>(cache, header, !headerReconciled && !headerVoided);
			PXUIFieldAttribute.SetEnabled<CARecon.loadDocumentsTill>(cache, header, !headerReconciled && !headerVoided);

			PXUIFieldAttribute.SetVisible<CARecon.skipVoided>(cache, header, ((CASetup)PXSelect<CASetup>.Select(this)).SkipVoided != header.SkipVoided);
			PXUIFieldAttribute.SetVisible<CARecon.showBatchPayments>(cache, header, cashaccount.Current != null && cashaccount.Current.MatchToBatch != null && header.ShowBatchPayments != null && cashaccount.Current.MatchToBatch != header.ShowBatchPayments);


			AddFilter.Cache.RaiseRowSelected(AddFilter.Current);
			Release.SetEnabled(releaseEnabled);

			bool nextEnabled = false;
			bool prevEnabled = false;
			bool lastEnabled = false;
			bool firstEnabled = false;
			if (header.CashAccountID.HasValue)
			{
				if (CAReconRecords.Cache.GetStatus(header) == PXEntryStatus.Inserted)
				{
					CARecon cachesReconPrev = PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>>,
																			 OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>.Select(this, header.CashAccountID);
					prevEnabled = cachesReconPrev != null;
				}
				else
				{
					CARecon cachesReconNext = PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																			  And<CARecon.reconNbr, Greater<Required<CARecon.reconNbr>>>>,
																			 OrderBy<Asc<CARecon.reconDate, Asc<CARecon.reconNbr>>>>.Select(this, header.CashAccountID, header.ReconNbr);
					CARecon cachesReconPrev = PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																			  And<CARecon.reconNbr, Less<Required<CARecon.reconNbr>>>>,
																			 OrderBy<Desc<CARecon.reconDate, Desc<CARecon.reconNbr>>>>.Select(this, header.CashAccountID, header.ReconNbr);
					nextEnabled = cachesReconNext != null;
					prevEnabled = cachesReconPrev != null;
				}
				firstEnabled = prevEnabled;
				lastEnabled = nextEnabled;
			}
			next.SetEnabled(nextEnabled);
			prev.SetEnabled(prevEnabled);
			first.SetEnabled(firstEnabled);
			last.SetEnabled(lastEnabled);

			bool CuryViewStateNotSet = (this.Accessinfo.CuryViewState != true);
			PXUIFieldAttribute.SetVisible<CARecon.curyBegBalance>(cache, header, CuryViewStateNotSet);
			PXUIFieldAttribute.SetVisible<CARecon.curyBalance>(cache, header, CuryViewStateNotSet);
			PXUIFieldAttribute.SetVisible<CARecon.curyDiffBalance>(cache, header, CuryViewStateNotSet);
			PXUIFieldAttribute.SetVisible<CARecon.curyReconciledBalance>(cache, header, CuryViewStateNotSet);
		}

		protected virtual void CARecon_Hold_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			CARecon header = (CARecon)e.Row;
			if ((bool)e.NewValue != true)
			{
				if (header.CuryDiffBalance != null && (Decimal)header.CuryDiffBalance != 0)
				{
					cache.RaiseExceptionHandling<CARecon.curyBalance>(header, header.CuryBalance, new PXSetPropertyException(Messages.DocumentOutOfBalance));
				}
			}
		}
		protected virtual void CARecon_ReconDate_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			CARecon row = (CARecon)e.Row;
			if (row.CashAccountID != null)
			{
				if (row.LastReconDate != null && ((DateTime)e.NewValue).Date <= row.LastReconDate.Value.Date)
				{
					throw new PXSetPropertyException(Messages.ReconDateNotAvailable);
				}
			}
		}
		protected virtual void CARecon_LoadDocumentsTill_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e)
		{
			CARecon header = (CARecon)e.Row;
			if (e.NewValue != null && (DateTime)e.NewValue < header.ReconDate)
			{
				cache.RaiseExceptionHandling<CARecon.loadDocumentsTill>(header, (DateTime)e.NewValue, new PXSetPropertyException(Messages.LastDateToLoadNotAvailable, PXErrorLevel.Warning));
			}
		}

		protected virtual void CARecon_ReconDate_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			CARecon header = (CARecon)e.Row;
			if (header == null) return;
			header.LoadDocumentsTill = header.ReconDate;

			CATranExt newrow;
			foreach (CATranExt res in PXSelect<CATranExt, Where<CATranExt.reconNbr, Equal<Required<CARecon.reconNbr>>, And<CATranExt.cashAccountID, Equal<Required<CARecon.cashAccountID>>>>>
												  .Select(this, header.ReconNbr, header.CashAccountID))
			{
				if (res.Reconciled == true)
				{
					if (header.SkipVoided == true && Skip(res))
						continue; //Skip voiding transaction
					newrow = PXCache<CATranExt>.CreateCopy(res);
					newrow.ReconDate = header.ReconDate;
					CAReconTranRecords.Update(newrow);
				}
			}
		}
		protected virtual bool Skip(CATran voidingTran, CATran voidedTran = null, bool searchInCache = true)
		{
			if (!voidingTran.VoidedTranID.HasValue || voidingTran.Released != true)
				return false;
			if (voidedTran == null)
			{
				voidedTran = PXSelect<CATran, Where<CATran.tranID, Equal<Required<CATran.voidedTranID>>,
					And<CATran.cashAccountID, Equal<Required<CATran.cashAccountID>>>>>.Select(this, voidingTran.VoidedTranID, voidingTran.CashAccountID);
			}
			if (voidedTran == null)
				return false;
			//The updated voidingTran may be stored in the Cache
			if (searchInCache)
			{
				voidingTran = PXSelect<CATran2, Where<CATran2.voidedTranID, Equal<Required<CATran.tranID>>>>.Select(this, voidedTran.TranID);
			}
			if (CAReconRecords.Current.ShowBatchPayments == true)
			{
				bool batchExistsAndNotPartiallyMatched = false;
				foreach (PXResult<CABatchDetail, CABatchDetail2, AP.Standalone.APPayment, CABankTranMatch> res in PXSelectJoin<CABatchDetail,
						LeftJoin<CABatchDetail2, On<CABatchDetail2.batchNbr, Equal<CABatchDetail.batchNbr>,
							And<CABatchDetail2.origModule, Equal<BatchModule.moduleAP>>>,
						LeftJoin<AP.Standalone.APPayment, On<AP.Standalone.APPayment.docType, Equal<CABatchDetail2.origDocType>,
							And<AP.Standalone.APPayment.refNbr, Equal<CABatchDetail2.origRefNbr>>>,
						LeftJoin<CABankTranMatch, On<CABankTranMatch.cATranID, Equal<AP.Standalone.APPayment.cATranID>>>>>,
						Where<CABatchDetail.origDocType, Equal<Required<CABatchDetail.origDocType>>,
							And<CABatchDetail.origModule, Equal<Required<CABatchDetail.origModule>>,
							And<CABatchDetail.origRefNbr, Equal<Required<CABatchDetail.origRefNbr>>>>>>.Select(this, voidedTran.OrigTranType, voidedTran.OrigModule, voidedTran.OrigRefNbr))
				{
					batchExistsAndNotPartiallyMatched = true;
					CABankTranMatch match = res;
					if (match != null && match.TranID.HasValue)
					{
						batchExistsAndNotPartiallyMatched = false;
						break;
					}
				}
				if (batchExistsAndNotPartiallyMatched)
					return false;
			}
			return (voidedTran.Reconciled == false && voidingTran.Reconciled == false) ||
				(voidedTran.ReconNbr == voidingTran.ReconNbr && voidedTran.Reconciled == voidingTran.Reconciled);
		}
		protected virtual void CARecon_RowUpdated(PXCache cache, PXRowUpdatedEventArgs e)
		{
			CARecon row = (CARecon)e.Row;
			CARecon oldRow = (CARecon)e.OldRow;
			if (oldRow.ReconDate != row.ReconDate
					|| oldRow.LoadDocumentsTill != row.LoadDocumentsTill)
			{
				this.CAReconTranRecords.View.RequestRefresh();
			}
		}

		protected virtual void CARecon_RowInserted(PXCache cache, PXRowInsertedEventArgs e)
		{
			CARecon header = (CARecon)e.Row;
			if (PXAccess.FeatureInstalled<FeaturesSet.multicurrency>())
			{
				CurrencyInfo info = CurrencyInfoAttribute.SetDefaults<CARecon.curyInfoID>(cache, header);

				string message = PXUIFieldAttribute.GetError<CurrencyInfo.curyID>(currencyinfo.Cache, info);
				if (string.IsNullOrEmpty(message) != true)
				{
					throw new PXSetPropertyException(message, PXErrorLevel.Error);
				}

				message = PXUIFieldAttribute.GetError<CurrencyInfo.curyEffDate>(currencyinfo.Cache, info);
				if (string.IsNullOrEmpty(message) != true)
				{
					throw new PXSetPropertyException(message, PXErrorLevel.Error);
				}

				if (info != null)
				{
					header.CuryID = info.CuryID;
				}
			}

			CARecon lastDoc = (CARecon)PXSelectReadonly<CARecon, Where<CARecon.cashAccountID, Equal<Required<CARecon.cashAccountID>>,
																					And<CARecon.reconNbr, NotEqual<Required<CARecon.reconNbr>>, And<CARecon.voided, NotEqual<True>>>>,
																				OrderBy<Asc<CARecon.cashAccountID, Desc<CARecon.reconDate>>>>.Select(this, header.CashAccountID, header.ReconNbr);
			if ((lastDoc != null) && (lastDoc.ReconDate != null))
			{
				if (lastDoc.Reconciled == true)
				{
					header.LastReconDate = lastDoc.ReconDate;
					header.CuryBegBalance = lastDoc.CuryBalance;
					header.CuryReconciledBalance = header.CuryBegBalance + header.CuryReconciledDebits - header.CuryReconciledCredits;
					header.CuryDiffBalance = header.CuryBalance - header.CuryReconciledBalance;
				}
				else
				{
					cache.RaiseExceptionHandling<CARecon.reconNbr>(header, header.ReconNbr, new PXSetPropertyException(Messages.PrevStatementNotReconciled));
				}
			}
		}
		#endregion

		[PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "DisplayName", CR.Messages.BAccountName)]
		protected virtual void BAccountR_AcctName_CacheAttached(PXCache sender) { }
	}
}
