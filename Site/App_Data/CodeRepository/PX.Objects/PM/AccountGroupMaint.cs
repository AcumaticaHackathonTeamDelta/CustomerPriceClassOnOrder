using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CR;
using System.Collections;

namespace PX.Objects.PM
{
    public class AccountGroupMaint : PXGraph<AccountGroupMaint, PMAccountGroup>
	{
		#region Views/Selects

		public PXSelect<PMAccountGroup> AccountGroup;
		public PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Current<PMAccountGroup.groupID>>>> AccountGroupProperties;
		[PXCopyPasteHiddenView]
		[PXVirtualDAC]
		public PXSelect<AccountPtr> Accounts;
		[PXCopyPasteHiddenView]
		public PXSelect<Account> GLAccount;


	    [PXViewName(Messages.AccountGroupAnswers)]
	    public CRAttributeList<PMAccountGroup> Answers;

		public IEnumerable accounts()
		{
			Dictionary<int, AccountPtr> inCache = new Dictionary<int, AccountPtr>();

			foreach (AccountPtr item in Accounts.Cache.Cached)
			{
				PXEntryStatus status = Accounts.Cache.GetStatus(item);
				inCache.Add(item.AccountID.Value, item);
				if (status == PXEntryStatus.Inserted || status == PXEntryStatus.Updated || status == PXEntryStatus.Notchanged)
				{
					yield return item;
				}
			}

			PXSelectBase<Account> select = new PXSelect<Account,
				Where<Account.accountGroupID, Equal<Current<PMAccountGroup.groupID>>>>(this);

			foreach (Account acct in select.Select())
			{
				if (!inCache.ContainsKey(acct.AccountID.Value))
				{
					AccountPtr ptr = new AccountPtr();
					ptr.AccountID = acct.AccountID;
					ptr.AccountClassID = acct.AccountClassID;
					ptr.CuryID = acct.CuryID;
					ptr.Description = acct.Description;
					ptr.Type = acct.Type;

					ptr = Accounts.Insert(ptr);
					Accounts.Cache.SetStatus(ptr, PXEntryStatus.Notchanged);

					yield return ptr;
				}
			}


			Accounts.Cache.IsDirty = false;
		}

		public PXSetup<GLSetup> GLSetup;

		#endregion

		public AccountGroupMaint()
		{
			PXUIFieldAttribute.SetVisible<AccountPtr.curyID>(Accounts.Cache, null, IsMultiCurrency);
		}

		#region Event Handlers
		
		protected virtual void AccountPtr_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			AccountPtr row = e.Row as AccountPtr;
			if (row != null)
			{
				Account account = GetAccountByID(row.AccountID);
				if (account != null)
				{
					row.AccountClassID = account.AccountClassID;
					row.CuryID = account.CuryID;
					row.Description = account.Description;
					row.Type = account.Type;
				}
			}
		}

		protected virtual void AccountPtr_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			AccountPtr row = e.Row as AccountPtr;
			if (row != null)
			{
				switch (e.Operation)
				{
					case PXDBOperation.Delete:
						RemoveAccount(row);
						break;
					case PXDBOperation.Insert:
					case PXDBOperation.Update:
						AddAccount(row);
						break;
				}

				e.Cancel = true;
			}
		}

		protected virtual void AccountPtr_AccountID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			AccountPtr row = e.Row as AccountPtr;
			if (row != null)
			{
				Account account = PXSelect<Account>.Search<Account.accountID>(this, e.NewValue);
				if (account != null && AccountGroup.Current != null && account.AccountGroupID != null && account.AccountGroupID != AccountGroup.Current.GroupID)
				{
					PMAccountGroup ag = PXSelect<PMAccountGroup, Where<PMAccountGroup.groupID, Equal<Required<PMAccountGroup.groupID>>>>.Select(this, account.AccountGroupID);
					
					sender.RaiseExceptionHandling<AccountPtr.accountID>(row, e.NewValue,
                        new PXSetPropertyException(Warnings.AccountIsUsed, PXErrorLevel.Warning, ag.GroupCD));
				}
			}
		}

		protected virtual void AccountPtr_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			AccountGroup.Cache.IsDirty = true;
		}
		

		protected virtual void PMAccountGroup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PMAccountGroup row = e.Row as PMAccountGroup;
			if (row != null)
			{
				PXUIFieldAttribute.SetVisible<PMAccountGroup.sortOrder>(sender, row, GLSetup.Current.COAOrder == 4); //todo show on custom sort order. ??

				Accounts.Cache.AllowInsert = row.Type != PMAccountType.OffBalance;
				PXUIFieldAttribute.SetEnabled<PMAccountGroup.type>(sender, row, !IsAccountsExist() && !IsBalanceExist());

				
			}
		}

		protected virtual void PMAccountGroup_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			PMAccountGroup row = e.Row as PMAccountGroup;
			if (row != null)
			{
				if (IsAccountsExist())
				{
					e.Cancel = true;
					throw new PXException(Messages.Account_FK);
				}

				PMProjectStatus ps = PXSelect<PMProjectStatus, Where<PMProjectStatus.accountGroupID, Equal<Required<PMAccountGroup.groupID>>>>.SelectWindowed(this, 0, 1, row.GroupID);
				if (ps != null)
				{
					e.Cancel = true;
					throw new PXException(Messages.ProjectStatus_FK);
				}
			}
		}


		protected virtual void PMAccountGroup_IsActive_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMAccountGroup row = e.Row as PMAccountGroup;
			if (row != null && row.IsActive != true)
			{
				if (IsAccountsExist())
				{
					sender.RaiseExceptionHandling<PMAccountGroup.isActive>(row, true, new PXSetPropertyException(Messages.AccountDiactivate_FK, PXErrorLevel.Error));
				}
			}
		}

		protected virtual void PMAccountGroup_Type_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PMAccountGroup row = e.Row as PMAccountGroup;
			if (row != null)
			{
				sender.SetDefaultExt<PMAccountGroup.sortOrder>(e.Row);
			}
		}
		
		protected virtual void PMAccountGroup_SortOrder_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			PMAccountGroup row = e.Row as PMAccountGroup;
			if (row != null)
			{
				if (row.Type == PMAccountType.OffBalance)
				{
					e.NewValue = (short) 5;
				}
				else
				{
					int ordinal = GL.AccountType.Ordinal(row.Type);
					GL.GLSetup setup = PXSelect<GL.GLSetup>.Select(this);

					if (setup != null)
					{
						if (setup.COAOrder.Value.ToString() == "128") //Custom Order - fixed preset order
						{
							if (row.Type == GL.AccountType.Income)
							{
								e.NewValue = (short) 1;
							}
							else if (row.Type == GL.AccountType.Expense)
							{
								e.NewValue = (short) 2;
							}
							else if (row.Type == GL.AccountType.Asset)
							{
								e.NewValue = (short) 3;
							}
							else if (row.Type == GL.AccountType.Liability)
							{
								e.NewValue = (short) 4;
							}
						}
						else
						{
							string order = GL.AccountType.COAOrderOptions[setup.COAOrder.Value];
							e.NewValue = short.Parse(order.Substring(ordinal, 1));
						}
					}

				}

			}
		}

		#endregion

		private bool IsAccountsExist()
		{
			Account account = PXSelect<Account, Where<Account.accountGroupID, Equal<Current<PMAccountGroup.groupID>>>>.SelectWindowed(this, 0, 1);

			if (account == null)
			{
				foreach (object x in Accounts.Cache.Inserted)
					return true;
			}

			return account != null;	
		}

		private bool IsBalanceExist()
		{
			PMProjectStatus status = PXSelect<PMProjectStatus, Where<PMProjectStatus.accountGroupID, Equal<Current<PMAccountGroup.groupID>>>>.SelectWindowed(this, 0, 1);
			return status != null;
		}

		protected virtual void RemoveAccount(AccountPtr ptr)
		{
			Account account = GetAccountByID(ptr.AccountID);
			if (account != null)
			{
				account.AccountGroupID = null;
				GLAccount.Update(account);
				GLAccount.Cache.PersistUpdated(account);
			}
		}

		protected virtual void AddAccount(AccountPtr ptr)
		{
			Account account = GetAccountByID(ptr.AccountID);
			if (account != null && AccountGroup.Current != null)
			{
				account.AccountGroupID = AccountGroup.Current.GroupID;
				GLAccount.Update(account);
				GLAccount.Cache.PersistUpdated(account);
			}
		}
	
		private Account GetAccountByID(int? id)
		{
			return PXSelect<Account, Where<Account.accountID, Equal<Required<Account.accountID>>>>.Select(this, id);
		}

		private bool IsMultiCurrency
		{
			get { return PXAccess.FeatureInstalled<FeaturesSet.multicurrency>(); }
		}

		#region Local Types
        [Serializable]
		public partial class AccountPtr : PX.Data.IBqlTable
		{
			#region AccountID
			public abstract class accountID : PX.Data.IBqlField
			{
			}
			protected Int32? _AccountID;
			[PMAccountAttribute(IsKey = true)]
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
			#region Type
			public abstract class type : PX.Data.IBqlField
			{
			}
			protected string _Type;
			[PXString(1)]
			[AccountType.List()]
			[PXUIField(DisplayName = "Type", Enabled = false)]
			public virtual string Type
			{
				get
				{
					return this._Type;
				}
				set
				{
					this._Type = value;
				}
			}
			#endregion
			#region AccountClassID
			public abstract class accountClassID : PX.Data.IBqlField
			{
			}
			protected string _AccountClassID;
			[PXString(20, IsUnicode = true)]
			[PXUIField(DisplayName = "Account Class", Enabled = false)]
			[PXSelector(typeof(AccountClass.accountClassID))]
			public virtual string AccountClassID
			{
				get
				{
					return this._AccountClassID;
				}
				set
				{
					this._AccountClassID = value;
				}
			}
			#endregion
			#region Description
			public abstract class description : PX.Data.IBqlField
			{
			}
			protected String _Description;
			[PXString(60, IsUnicode = true)]
			[PXUIField(DisplayName = "Description", Enabled = false)]
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
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected string _CuryID;
			[PXString(5, IsUnicode = true)]
			[PXUIField(DisplayName = "Currency", Enabled = false)]
			//[PXSelector(typeof(Currency.curyID))]
			public virtual string CuryID
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
		} 
		#endregion
	}

	
}
