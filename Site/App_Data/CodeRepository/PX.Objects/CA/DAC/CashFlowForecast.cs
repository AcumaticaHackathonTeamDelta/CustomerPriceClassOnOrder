using System;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.GL;
using PX.Objects.CR;


namespace PX.Objects.CA
{
	/// <summary>
	/// This DAC does not have a corresponded table in the database - it's calculated on the fly.
	/// </summary>
	[PXCacheName(Messages.CashFlowForecast)]
	[System.SerializableAttribute()]
	public class CashFlowForecast: IBqlTable
	{
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int64? _RecordID;
		[PXDBLongIdentity(IsKey = true)]
		public virtual Int64? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion

		#region StartingDate
		public abstract class startingDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartingDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Beginning Date", Visibility = PXUIVisibility.Visible, Enabled=false)]
		public virtual DateTime? StartingDate
		{
			get
			{
				return this._StartingDate;
			}
			set
			{
				this._StartingDate = value;
			}
		}
		#endregion

		#region RecordType
		public abstract class recordType : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordType;
		
		[PXDBInt()]
		[PXDefault()]
		[CashFlowForecastRecordType.List()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible,Enabled=false)]
		public virtual Int32? RecordType
		{
			get
			{
				return this._RecordType;
			}
			set
			{
				this._RecordType = value;
			}
		}
		#endregion

		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where2<Match<Current<AccessInfo.userName>>, And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>), DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr),Enabled=false)]
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

		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
		[PXSelector(typeof(BAccountR.bAccountID),
						SubstituteKey = typeof(BAccountR.acctCD),
					 DescriptionField = typeof(BAccountR.acctName))]
		
		[PXUIField(DisplayName = "Customer/Vendor",Enabled=false)]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion
		
		#region EntryID
		public abstract class entryID : PX.Data.IBqlField
		{
		}
		protected Int32? _EntryID;
		[PXDBInt()]
		[PXSelector(typeof(Search<CashForecastTran.tranID>),DescriptionField = typeof(CashForecastTran.tranDesc))]
		[PXUIField(DisplayName = "Forecast Tran", Visibility = PXUIVisibility.Visible, Visible = true,Enabled = false)]
		public virtual Int32? EntryID
		{
			get
			{
				return this._EntryID;
			}
			set
			{
				this._EntryID = value;
			}
		}
		#endregion

		#region CuryID
		public abstract class curyID : PX.Data.IBqlField
		{
		}
		protected String _CuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]		
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

		#region AcctCuryID
		public abstract class acctCuryID : PX.Data.IBqlField
		{
		}
		protected String _AcctCuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Account Currency", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String AcctCuryID
		{
			get
			{
				return this._AcctCuryID;
			}
			set
			{
				this._AcctCuryID = value;
			}
		}
		#endregion

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(ModuleCode = "CA")]
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

		#region CuryAmountDay0
		public abstract class curyAmountDay0 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay0 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay0))]
		
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 0", Enabled = false)]
		public virtual Decimal? CuryAmountDay0
		{
			get
			{
				return this._CuryAmountDay0;
			}
			set
			{
				this._CuryAmountDay0 = value;
			}
		}
		#endregion
		#region AmountDay0
		public abstract class amountDay0 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay0 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Day 0 Amount", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay0
		{
			get
			{
				return this._AmountDay0;
			}
			set
			{
				this._AmountDay0 = value;
			}
		}
		#endregion

		#region CuryAmountDay1
		public abstract class curyAmountDay1 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay1 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay1))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 1", Enabled = false)]
		public virtual Decimal? CuryAmountDay1
		{
			get
			{
				return this._CuryAmountDay1;
			}
			set
			{
				this._CuryAmountDay1 = value;
			}
		}
		#endregion
		#region AmountDay1
		public abstract class amountDay1 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay1 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 1", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay1
		{
			get
			{
				return this._AmountDay1;
			}
			set
			{
				this._AmountDay1 = value;
			}
		}
		#endregion

		#region CuryAmountDay2
		public abstract class curyAmountDay2 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay2 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay2))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 2", Enabled = false)]
		public virtual Decimal? CuryAmountDay2
		{
			get
			{
				return this._CuryAmountDay2;
			}
			set
			{
				this._CuryAmountDay2 = value;
			}
		}
		#endregion
		#region AmountDay2
		public abstract class amountDay2 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay2 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 2", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay2
		{
			get
			{
				return this._AmountDay2;
			}
			set
			{
				this._AmountDay2 = value;
			}
		}
		#endregion

		#region CuryAmountDay3
		public abstract class curyAmountDay3 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay3 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay3))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 3", Enabled = false)]
		public virtual Decimal? CuryAmountDay3
		{
			get
			{
				return this._CuryAmountDay3;
			}
			set
			{
				this._CuryAmountDay3 = value;
			}
		}
		#endregion
		#region AmountDay3
		public abstract class amountDay3 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay3 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 3", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay3
		{
			get
			{
				return this._AmountDay3;
			}
			set
			{
				this._AmountDay3 = value;
			}
		}
		#endregion

		#region CuryAmountDay4
		public abstract class curyAmountDay4 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay4 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay4))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 4", Enabled = false)]
		public virtual Decimal? CuryAmountDay4
		{
			get
			{
				return this._CuryAmountDay4;
			}
			set
			{
				this._CuryAmountDay4 = value;
			}
		}
		#endregion
		#region AmountDay4
		public abstract class amountDay4 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay4=0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 4", Enabled = false, Visible=false)]
		public virtual Decimal? AmountDay4
		{
			get
			{
				return this._AmountDay4;
			}
			set
			{
				this._AmountDay4 = value;
			}
		}
		#endregion

		#region CuryAmountDay5
		public abstract class curyAmountDay5 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay5 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay5))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 5", Enabled = false)]
		public virtual Decimal? CuryAmountDay5
		{
			get
			{
				return this._CuryAmountDay5;
			}
			set
			{
				this._CuryAmountDay5 = value;
			}
		}
		#endregion
		#region AmountDay5
		public abstract class amountDay5 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay5 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 5", Enabled = false,Visible=false)]
		public virtual Decimal? AmountDay5
		{
			get
			{
				return this._AmountDay5;
			}
			set
			{
				this._AmountDay5 = value;
			}
		}
		#endregion

		#region CuryAmountDay6
		public abstract class curyAmountDay6 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay6 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay6))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 6", Enabled = false)]
		public virtual Decimal? CuryAmountDay6
		{
			get
			{
				return this._CuryAmountDay6;
			}
			set
			{
				this._CuryAmountDay6 = value;
			}
		}
		#endregion
		#region AmountDay6
		public abstract class amountDay6 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay6 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 6", Enabled = false,Visible=false)]
		public virtual Decimal? AmountDay6
		{
			get
			{
				return this._AmountDay6;
			}
			set
			{
				this._AmountDay6 = value;
			}
		}
		#endregion

		#region CuryAmountDay7
		public abstract class curyAmountDay7 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay7 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay7))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 7", Enabled = false)]
		public virtual Decimal? CuryAmountDay7
		{
			get
			{
				return this._CuryAmountDay7;
			}
			set
			{
				this._CuryAmountDay7 = value;
			}
		}
		#endregion
		#region AmountDay7
		public abstract class amountDay7 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay7 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 7", Enabled = false,Visible=false)]
		public virtual Decimal? AmountDay7
		{
			get
			{
				return this._AmountDay7;
			}
			set
			{
				this._AmountDay7 = value;
			}
		}
		#endregion

		#region CuryAmountDay8
		public abstract class curyAmountDay8 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay8 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay8))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 8", Enabled = false)]
		public virtual Decimal? CuryAmountDay8
		{
			get
			{
				return this._CuryAmountDay8;
			}
			set
			{
				this._CuryAmountDay8 = value;
			}
		}
		#endregion
		#region AmountDay8
		public abstract class amountDay8 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay8 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 9", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay8
		{
			get
			{
				return this._AmountDay8;
			}
			set
			{
				this._AmountDay8 = value;
			}
		}
		#endregion

		#region CuryAmountDay9
		public abstract class curyAmountDay9 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay9 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay9))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 9", Enabled = false)]
		public virtual Decimal? CuryAmountDay9
		{
			get
			{
				return this._CuryAmountDay9;
			}
			set
			{
				this._CuryAmountDay9 = value;
			}
		}
		#endregion
		#region AmountDay9
		public abstract class amountDay9 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay9 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 9", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay9
		{
			get
			{
				return this._AmountDay9;
			}
			set
			{
				this._AmountDay9 = value;
			}
		}
		#endregion

		#region CuryAmountDay10
		public abstract class curyAmountDay10 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay10 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay10))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 10", Enabled = false)]
		public virtual Decimal? CuryAmountDay10
		{
			get
			{
				return this._CuryAmountDay10;
			}
			set
			{
				this._CuryAmountDay10 = value;
			}
		}
		#endregion
		#region AmountDay10
		public abstract class amountDay10 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay10 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Day 0 Amount", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay10
		{
			get
			{
				return this._AmountDay10;
			}
			set
			{
				this._AmountDay10 = value;
			}
		}
		#endregion

		#region CuryAmountDay11
		public abstract class curyAmountDay11 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay11 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay11))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 11", Enabled = false)]
		public virtual Decimal? CuryAmountDay11
		{
			get
			{
				return this._CuryAmountDay11;
			}
			set
			{
				this._CuryAmountDay11 = value;
			}
		}
		#endregion
		#region AmountDay11
		public abstract class amountDay11 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay11 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 11", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay11
		{
			get
			{
				return this._AmountDay11;
			}
			set
			{
				this._AmountDay11 = value;
			}
		}
		#endregion

		#region CuryAmountDay12
		public abstract class curyAmountDay12 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay12 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay12))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 12", Enabled = false)]
		public virtual Decimal? CuryAmountDay12
		{
			get
			{
				return this._CuryAmountDay12;
			}
			set
			{
				this._CuryAmountDay12 = value;
			}
		}
		#endregion
		#region AmountDay12
		public abstract class amountDay12 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay12 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 12", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay12
		{
			get
			{
				return this._AmountDay12;
			}
			set
			{
				this._AmountDay12 = value;
			}
		}
		#endregion

		#region CuryAmountDay13
		public abstract class curyAmountDay13 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay13 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay13))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 13", Enabled = false)]
		public virtual Decimal? CuryAmountDay13
		{
			get
			{
				return this._CuryAmountDay13;
			}
			set
			{
				this._CuryAmountDay13 = value;
			}
		}
		#endregion
		#region AmountDay13
		public abstract class amountDay13 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay13 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 13", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay13
		{
			get
			{
				return this._AmountDay13;
			}
			set
			{
				this._AmountDay13 = value;
			}
		}
		#endregion

		#region CuryAmountDay14
		public abstract class curyAmountDay14 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay14 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay14))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 14", Enabled = false)]
		public virtual Decimal? CuryAmountDay14
		{
			get
			{
				return this._CuryAmountDay14;
			}
			set
			{
				this._CuryAmountDay14 = value;
			}
		}
		#endregion
		#region AmountDay14
		public abstract class amountDay14 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay14 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 14", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay14
		{
			get
			{
				return this._AmountDay14;
			}
			set
			{
				this._AmountDay14 = value;
			}
		}
		#endregion

		#region CuryAmountDay15
		public abstract class curyAmountDay15 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay15 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay15))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 15", Enabled = false)]
		public virtual Decimal? CuryAmountDay15
		{
			get
			{
				return this._CuryAmountDay15;
			}
			set
			{
				this._CuryAmountDay15 = value;
			}
		}
		#endregion
		#region AmountDay15
		public abstract class amountDay15 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay15 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 15", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay15
		{
			get
			{
				return this._AmountDay15;
			}
			set
			{
				this._AmountDay15 = value;
			}
		}
		#endregion

		#region CuryAmountDay16
		public abstract class curyAmountDay16 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay16 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay16))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 16", Enabled = false)]
		public virtual Decimal? CuryAmountDay16
		{
			get
			{
				return this._CuryAmountDay16;
			}
			set
			{
				this._CuryAmountDay16 = value;
			}
		}
		#endregion
		#region AmountDay16
		public abstract class amountDay16 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay16 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 16", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay16
		{
			get
			{
				return this._AmountDay16;
			}
			set
			{
				this._AmountDay16 = value;
			}
		}
		#endregion

		#region CuryAmountDay17
		public abstract class curyAmountDay17 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay17 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay17))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 17", Enabled = false)]
		public virtual Decimal? CuryAmountDay17
		{
			get
			{
				return this._CuryAmountDay17;
			}
			set
			{
				this._CuryAmountDay17 = value;
			}
		}
		#endregion
		#region AmountDay17
		public abstract class amountDay17 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay17 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 17", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay17
		{
			get
			{
				return this._AmountDay17;
			}
			set
			{
				this._AmountDay17 = value;
			}
		}
		#endregion

		#region CuryAmountDay18
		public abstract class curyAmountDay18 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay18 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay18))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 18", Enabled = false)]
		public virtual Decimal? CuryAmountDay18
		{
			get
			{
				return this._CuryAmountDay18;
			}
			set
			{
				this._CuryAmountDay18 = value;
			}
		}
		#endregion
		#region AmountDay18
		public abstract class amountDay18 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay18 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 18", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay18
		{
			get
			{
				return this._AmountDay18;
			}
			set
			{
				this._AmountDay18 = value;
			}
		}
		#endregion

		#region CuryAmountDay19
		public abstract class curyAmountDay19 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay19 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay19))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 19", Enabled = false)]
		public virtual Decimal? CuryAmountDay19
		{
			get
			{
				return this._CuryAmountDay19;
			}
			set
			{
				this._CuryAmountDay19 = value;
			}
		}
		#endregion
		#region AmountDay19
		public abstract class amountDay19 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay19 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 19", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay19
		{
			get
			{
				return this._AmountDay19;
			}
			set
			{
				this._AmountDay19 = value;
			}
		}
		#endregion

		#region CuryAmountDay20
		public abstract class curyAmountDay20 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay20 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay20))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 20", Enabled = false)]
		public virtual Decimal? CuryAmountDay20
		{
			get
			{
				return this._CuryAmountDay20;
			}
			set
			{
				this._CuryAmountDay20 = value;
			}
		}
		#endregion
		#region AmountDay20
		public abstract class amountDay20 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay20 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 20", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay20
		{
			get
			{
				return this._AmountDay20;
			}
			set
			{
				this._AmountDay20 = value;
			}
		}
		#endregion

		#region CuryAmountDay21
		public abstract class curyAmountDay21 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay21 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay21))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 21", Enabled = false)]
		public virtual Decimal? CuryAmountDay21
		{
			get
			{
				return this._CuryAmountDay21;
			}
			set
			{
				this._CuryAmountDay21 = value;
			}
		}
		#endregion
		#region AmountDay21
		public abstract class amountDay21 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay21 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 21", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay21
		{
			get
			{
				return this._AmountDay21;
			}
			set
			{
				this._AmountDay21 = value;
			}
		}
		#endregion

		#region CuryAmountDay22
		public abstract class curyAmountDay22 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay22 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay22))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 22", Enabled = false)]
		public virtual Decimal? CuryAmountDay22
		{
			get
			{
				return this._CuryAmountDay22;
			}
			set
			{
				this._CuryAmountDay22 = value;
			}
		}
		#endregion
		#region AmountDay22
		public abstract class amountDay22 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay22 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 22", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay22
		{
			get
			{
				return this._AmountDay22;
			}
			set
			{
				this._AmountDay22 = value;
			}
		}
		#endregion

		#region CuryAmountDay23
		public abstract class curyAmountDay23 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay23 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay23))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 23", Enabled = false)]
		public virtual Decimal? CuryAmountDay23
		{
			get
			{
				return this._CuryAmountDay23;
			}
			set
			{
				this._CuryAmountDay23 = value;
			}
		}
		#endregion
		#region AmountDay23
		public abstract class amountDay23 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay23 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 23", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay23
		{
			get
			{
				return this._AmountDay23;
			}
			set
			{
				this._AmountDay23 = value;
			}
		}
		#endregion

		#region CuryAmountDay24
		public abstract class curyAmountDay24 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay24 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay24))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 24", Enabled = false)]
		public virtual Decimal? CuryAmountDay24
		{
			get
			{
				return this._CuryAmountDay24;
			}
			set
			{
				this._CuryAmountDay24 = value;
			}
		}
		#endregion
		#region AmountDay24
		public abstract class amountDay24 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay24 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 24", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay24
		{
			get
			{
				return this._AmountDay24;
			}
			set
			{
				this._AmountDay24 = value;
			}
		}
		#endregion

		#region CuryAmountDay25
		public abstract class curyAmountDay25 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay25 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay25))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 25", Enabled = false)]
		public virtual Decimal? CuryAmountDay25
		{
			get
			{
				return this._CuryAmountDay25;
			}
			set
			{
				this._CuryAmountDay25 = value;
			}
		}
		#endregion
		#region AmountDay25
		public abstract class amountDay25 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay25 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 25", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay25
		{
			get
			{
				return this._AmountDay25;
			}
			set
			{
				this._AmountDay25 = value;
			}
		}
		#endregion

		#region CuryAmountDay26
		public abstract class curyAmountDay26 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay26 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay26))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 26", Enabled = false)]
		public virtual Decimal? CuryAmountDay26
		{
			get
			{
				return this._CuryAmountDay26;
			}
			set
			{
				this._CuryAmountDay26 = value;
			}
		}
		#endregion
		#region AmountDay26
		public abstract class amountDay26 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay26 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 26", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay26
		{
			get
			{
				return this._AmountDay26;
			}
			set
			{
				this._AmountDay26 = value;
			}
		}
		#endregion

		#region CuryAmountDay27
		public abstract class curyAmountDay27 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay27 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay27))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 27", Enabled = false)]
		public virtual Decimal? CuryAmountDay27
		{
			get
			{
				return this._CuryAmountDay27;
			}
			set
			{
				this._CuryAmountDay27 = value;
			}
		}
		#endregion
		#region AmountDay27
		public abstract class amountDay27 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay27 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 27", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay27
		{
			get
			{
				return this._AmountDay27;
			}
			set
			{
				this._AmountDay27 = value;
			}
		}
		#endregion

		#region CuryAmountDay28
		public abstract class curyAmountDay28 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay28 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay28))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 28", Enabled = false)]
		public virtual Decimal? CuryAmountDay28
		{
			get
			{
				return this._CuryAmountDay28;
			}
			set
			{
				this._CuryAmountDay28 = value;
			}
		}
		#endregion
		#region AmountDay28
		public abstract class amountDay28 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay28 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 29", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay28
		{
			get
			{
				return this._AmountDay28;
			}
			set
			{
				this._AmountDay28 = value;
			}
		}
		#endregion

		#region CuryAmountDay29
		public abstract class curyAmountDay29 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay29 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay29))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 29", Enabled = false)]
		public virtual Decimal? CuryAmountDay29
		{
			get
			{
				return this._CuryAmountDay29;
			}
			set
			{
				this._CuryAmountDay29 = value;
			}
		}
		#endregion
		#region AmountDay29
		public abstract class amountDay29 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay29 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 29", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay29
		{
			get
			{
				return this._AmountDay29;
			}
			set
			{
				this._AmountDay29 = value;
			}
		}
		#endregion

		#region CuryAmountDay30
		public abstract class curyAmountDay30 : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay30 = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountDay30))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 30", Enabled = false)]
		public virtual Decimal? CuryAmountDay30
		{
			get
			{
				return this._CuryAmountDay30;
			}
			set
			{
				this._CuryAmountDay30 = value;
			}
		}
		#endregion
		#region AmountDay30
		public abstract class amountDay30 : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay30 = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Day 30", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay30
		{
			get
			{
				return this._AmountDay30;
			}
			set
			{
				this._AmountDay30 = value;
			}
		}
		#endregion


		#region CuryAmountSummary
		public abstract class curyAmountSummary : PX.Data.IBqlField
		{
		}

		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast.amountSummary))]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Summary", Enabled = false)]
		public virtual Decimal? CuryAmountSummary
		{
			get
			{
				if (this.RecordType.HasValue == false || this.RecordType.Value == (int)CashFlowForecastRecordType.RecordType.Balance)
				{
					return this._CuryAmountDay30;
				}
				else 
				{
					return ((this.CuryAmountDay0??Decimal.Zero) +
						   (this.CuryAmountDay1??Decimal.Zero) +
						   (this.CuryAmountDay2??Decimal.Zero) +
						   (this.CuryAmountDay3??Decimal.Zero) +
						   (this.CuryAmountDay4??Decimal.Zero) +
						   (this.CuryAmountDay5??Decimal.Zero) +
						   (this.CuryAmountDay6??Decimal.Zero) +
						   (this.CuryAmountDay7??Decimal.Zero) +
						   (this.CuryAmountDay8??Decimal.Zero) +
						   (this.CuryAmountDay9??Decimal.Zero) +
						   (this.CuryAmountDay10??Decimal.Zero) +
						   (this.CuryAmountDay11??Decimal.Zero) +
						   (this.CuryAmountDay12??Decimal.Zero) +
						   (this.CuryAmountDay13??Decimal.Zero) +
						   (this.CuryAmountDay14??Decimal.Zero) +
						   (this.CuryAmountDay15??Decimal.Zero) +
						   (this.CuryAmountDay16??Decimal.Zero) +
						   (this.CuryAmountDay17??Decimal.Zero) +
						   (this.CuryAmountDay18??Decimal.Zero) +
						   (this.CuryAmountDay19??Decimal.Zero) +
						   (this.CuryAmountDay20??Decimal.Zero) +
						   (this.CuryAmountDay21??Decimal.Zero) +
						   (this.CuryAmountDay22??Decimal.Zero) +
						   (this.CuryAmountDay23??Decimal.Zero) +
						   (this.CuryAmountDay24??Decimal.Zero) +
						   (this.CuryAmountDay25??Decimal.Zero) +
						   (this.CuryAmountDay26??Decimal.Zero) +
						   (this.CuryAmountDay27??Decimal.Zero) +
						   (this.CuryAmountDay28??Decimal.Zero) +
						   (this.CuryAmountDay29??Decimal.Zero));
				}
			}
		
		}
		#endregion
		#region AmountSummary
		public abstract class amountSummary : PX.Data.IBqlField
		{
		}
		
		[PXDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount Summary", Enabled = false, Visible = false)]
		public virtual Decimal? AmountSummary
		{
			get
			{
				if (this.RecordType.HasValue == false || this.RecordType.Value == (int)CashFlowForecastRecordType.RecordType.Balance)
				{
					return this._AmountDay30;
				}
				else
				{
					return ((this.AmountDay0 ?? Decimal.Zero) +
						   (this.AmountDay1 ?? Decimal.Zero) +
						   (this.AmountDay2 ?? Decimal.Zero) +
						   (this.AmountDay3 ?? Decimal.Zero) +
						   (this.AmountDay4 ?? Decimal.Zero) +
						   (this.AmountDay5 ?? Decimal.Zero) +
						   (this.AmountDay6 ?? Decimal.Zero) +
						   (this.AmountDay7 ?? Decimal.Zero) +
						   (this.AmountDay8 ?? Decimal.Zero) +
						   (this.AmountDay9 ?? Decimal.Zero) +
						   (this.AmountDay10 ?? Decimal.Zero) +
						   (this.AmountDay11 ?? Decimal.Zero) +
						   (this.AmountDay12 ?? Decimal.Zero) +
						   (this.AmountDay13 ?? Decimal.Zero) +
						   (this.AmountDay14 ?? Decimal.Zero) +
						   (this.AmountDay15 ?? Decimal.Zero) +
						   (this.AmountDay16 ?? Decimal.Zero) +
						   (this.AmountDay17 ?? Decimal.Zero) +
						   (this.AmountDay18 ?? Decimal.Zero) +
						   (this.AmountDay19 ?? Decimal.Zero) +
						   (this.AmountDay20 ?? Decimal.Zero) +
						   (this.AmountDay21 ?? Decimal.Zero) +
						   (this.AmountDay22 ?? Decimal.Zero) +
						   (this.AmountDay23 ?? Decimal.Zero) +
						   (this.AmountDay24 ?? Decimal.Zero) +
						   (this.AmountDay25 ?? Decimal.Zero) +
						   (this.AmountDay26 ?? Decimal.Zero) +
						   (this.AmountDay27 ?? Decimal.Zero) +
						   (this.AmountDay28 ?? Decimal.Zero) +
						   (this.AmountDay29 ?? Decimal.Zero));
				}
			}
			set
			{
				
			}
		}
		#endregion

		public virtual bool IsZero()
		{
			bool resultCury= (((this.CuryAmountDay0 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay1 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay2 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay3 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay4 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay5 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay6 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay7 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay8 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay9 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay10 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay11 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay12 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay13 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay14 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay15 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay16 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay17 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay18 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay19 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay20 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay21 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay22 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay23 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay24 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay25 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay26 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay27 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay28 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.CuryAmountDay29 ?? Decimal.Zero) == Decimal.Zero)); 

		bool resultBase =(((this.AmountDay0 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay1 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay2 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay3 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay4 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay5 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay6 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay7 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay8 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay9 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay10 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay11 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay12 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay13 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay14 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay15 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay16 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay17 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay18 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay19 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay20 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay21 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay22 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay23 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay24 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay25 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay26 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay27 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay28 ?? Decimal.Zero) == Decimal.Zero) &&
				((this.AmountDay29 ?? Decimal.Zero) == Decimal.Zero));
			return resultCury && resultBase;
		}
	}

	
	public class CashFlowForecastRecordType
	{
		public enum RecordType
		{
			CashOut = -1,
			CashOutUnapplied =-2,
			Balance = 0,
			CashIn = 1,
			CashInUnapplied = 2
		}
		public class ListAttribute : PXIntListAttribute
		{
			public ListAttribute()
				: base(
				new int[] { (int)RecordType.CashOut, (int)RecordType.CashOutUnapplied, (int)RecordType.Balance, (int)RecordType.CashIn, (int)RecordType.CashInUnapplied },
				new string[] { "Cash Paid Out","Cash Paid Non-Applied", "Cash On Hand", "Cash Receipts", "Non-Applied Receipts"}) { }
		}		
	}

	[PXCacheName(Messages.CashFlowForecast2)]
	[System.SerializableAttribute()]
    [PXHidden]
	public class CashFlowForecast2 : IBqlTable
	{
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int64? _RecordID;
		[PXDBLongIdentity(IsKey = true)]
		public virtual Int64? RecordID
		{
			get
			{
				return this._RecordID;
			}
			set
			{
				this._RecordID = value;
			}
		}
		#endregion

		#region TranDate
		public abstract class tranDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _TranDate;
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "Beginning Date", Visibility = PXUIVisibility.Visible, Enabled = false)]
		public virtual DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}
			set
			{
				this._TranDate = value;
			}
		}
		#endregion

		#region CashAccountID
		public abstract class cashAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _CashAccountID;
		[GL.CashAccount(null, typeof(Search<CashAccount.cashAccountID, Where2<Match<Current<AccessInfo.userName>>, And<CashAccount.clearingAccount, Equal<CS.boolFalse>>>>), DisplayName = "Cash Account", Visibility = PXUIVisibility.SelectorVisible, DescriptionField = typeof(CashAccount.descr), Enabled = false)]
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
		[PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
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

		#region AcctCuryID
		public abstract class acctCuryID : PX.Data.IBqlField
		{
		}
		protected String _AcctCuryID;
		[PXDBString(5, IsUnicode = true, InputMask = ">LLLLL")]
		[PXUIField(DisplayName = "Account Currency", Visibility = PXUIVisibility.Visible)]
		[PXSelector(typeof(Currency.curyID))]
		public virtual String AcctCuryID
		{
			get
			{
				return this._AcctCuryID;
			}
			set
			{
				this._AcctCuryID = value;
			}
		}
		#endregion

		#region RecordType
		public abstract class recordType : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordType;

		[PXDBInt()]
		[PXDefault()]
		[PXUIField(DisplayName = "Type", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual Int32? RecordType
		{
			get
			{
				return this._RecordType;
			}
			set
			{
				this._RecordType = value;
			}
		}
		#endregion

		#region BAccountID
		public abstract class bAccountID : PX.Data.IBqlField
		{
		}
		protected Int32? _BAccountID;
		[PXDBInt()]
		[PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]		
		[PXSelector(typeof(BAccountR.bAccountID),
						SubstituteKey = typeof(BAccountR.acctCD),
					 DescriptionField = typeof(BAccountR.acctName))]

		[PXUIField(DisplayName = "Business Account", Enabled = false)]
		public virtual Int32? BAccountID
		{
			get
			{
				return this._BAccountID;
			}
			set
			{
				this._BAccountID = value;
			}
		}
		#endregion

		#region EntryID
		public abstract class entryID : PX.Data.IBqlField
		{
		}
		protected Int32? _EntryID;
		[PXDBInt()]
		[PXSelector(typeof(Search<CashForecastTran.tranID>), DescriptionField = typeof(CashForecastTran.tranDesc))]
		[PXUIField(DisplayName = "Tran ID", Visibility = PXUIVisibility.Visible, Visible = false)]
		public virtual Int32? EntryID
		{
			get
			{
				return this._EntryID;
			}
			set
			{
				this._EntryID = value;
			}
		}
		#endregion

		#region CuryInfoID
		public abstract class curyInfoID : PX.Data.IBqlField
		{
		}
		protected Int64? _CuryInfoID;
		[PXDBLong()]
		[CurrencyInfo(ModuleCode = "CA")]
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

		#region CuryAmountDay
		public abstract class curyAmountDay : PX.Data.IBqlField
		{
		}
		protected Decimal? _CuryAmountDay = 0m;
		[PXDBCurrency(typeof(CashFlowForecast.curyInfoID), typeof(CashFlowForecast2.amountDay))]

		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Enabled = false)]
		public virtual Decimal? CuryAmountDay
		{
			get
			{
				return this._CuryAmountDay;
			}
			set
			{
				this._CuryAmountDay = value;
			}
		}
		#endregion
		#region AmountDay
		public abstract class amountDay : PX.Data.IBqlField
		{
		}
		protected Decimal? _AmountDay = 0m;
		[PXDBDecimal(4)]
		[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Amount", Enabled = false, Visible = false)]
		public virtual Decimal? AmountDay
		{
			get
			{
				return this._AmountDay;
			}
			set
			{
				this._AmountDay = value;
			}
		}
		#endregion

		public CashFlowForecast2 Copy(DateTime startDate, AP.APInvoice src)
		{
			this.BAccountID = src.VendorID;
			this.CashAccountID = src.PayAccountID;
			this.TranDate = src.DueDate >= startDate ? src.DueDate : startDate;
			this.RecordType = (int)CashFlowForecastRecordType.RecordType.CashOut;
			return this;
		}

		public CashFlowForecast2 Copy(DateTime startDate, AP.APPayment src)
		{
			this.BAccountID = src.VendorID;
			this.CashAccountID = src.CashAccountID;
			this.TranDate = src.AdjDate >= startDate ? src.AdjDate : startDate;
			this.RecordType = (int)CashFlowForecastRecordType.RecordType.CashOut;
			this.CuryID = src.CuryID;
			this.AcctCuryID = src.CuryID;
			return this;
		}

		public CashFlowForecast2 Copy(DateTime startDate, AR.ARInvoice src)
		{
			this.BAccountID = src.CustomerID;
			this.CashAccountID = src.CashAccountID;
			this.TranDate = src.DueDate >= startDate ? src.DueDate : startDate;
			this.RecordType = (int)CashFlowForecastRecordType.RecordType.CashIn;
			return this;
		}

		public CashFlowForecast2 Copy(DateTime startDate, AR.ARPayment src)
		{
			this.BAccountID = src.CustomerID;
			this.CashAccountID = src.CashAccountID;
			this.TranDate = src.AdjDate >= startDate? src.AdjDate: startDate;
			this.CuryID = src.CuryID;
			this.AcctCuryID = src.CuryID;
			this.RecordType = (int)CashFlowForecastRecordType.RecordType.CashIn;
			return this;
		}
	}
}
