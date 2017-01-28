namespace PX.Objects.TX
{
	using System;
	using PX.Data;

	public class TaxPeriodStatus
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { Prepared, Open, Closed },
				new string[] { Messages.Prepared, Messages.Open, Messages.Closed }) { ; }
		}

		public const string Prepared = "P";
		public const string Open = "N";
		public const string Closed = "C";
		public const string Dummy = "D";

		public class prepared : Constant<string>
		{
			public prepared() : base(Prepared) { ;}
		}

		public class open : Constant<string>
		{
			public open() : base(Open) { ;}
		}

		public class closed : Constant<string>
		{
			public closed() : base(Closed) { ;}
		}
	}

	/// <summary>
	/// Represent an agency tax period at the company level.
	/// The instance of DAC is created on the Prepare Tax Report(TX501000) page when the user selects a tax agency.
    /// </summary>
	[System.SerializableAttribute()]
	[PXCacheName(Messages.ReportingPeriod)]
	public partial class TaxPeriod : PX.Data.IBqlTable
	{
		#region VendorID
		public abstract class vendorID : PX.Data.IBqlField
		{
		}
		protected Int32? _VendorID;

		/// <summary>
		/// <see cref="PX.Objects.AP.Vendor.BAccountID"/> of a tax agency to which the tax period belongs. 
		/// </summary>
		[PXDBInt(IsKey = true)]
		[PXDefault()]
		public virtual Int32? VendorID
		{
			get
			{
				return this._VendorID;
			}
			set
			{
				this._VendorID = value;
			}
		}
		#endregion
		#region TaxPeriodID
		public abstract class taxPeriodID : PX.Data.IBqlField
		{
		}
		protected String _TaxPeriodID;

		/// <summary>
		/// Identifier of the tax period. 
		/// </summary>
		[GL.FinPeriodID(IsKey=true)]
		[PXDefault()]
		[PXUIField(DisplayName = Messages.ReportingPeriod, Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String TaxPeriodID
		{
			get
			{
				return this._TaxPeriodID;
			}
			set
			{
				this._TaxPeriodID = value;
			}
		}
		#endregion
		#region TaxYear
		public abstract class taxYear : PX.Data.IBqlField
		{
		}
		protected String _TaxYear;

		/// <summary>
		/// Identifier of a <see cref="PX.Objects.TX.TaxYear"/> to which the tax period belongs.
		/// </summary>
		[PXDBString(4, IsFixed = true)]
		[PXDefault()]
		public virtual String TaxYear
		{
			get
			{
				return this._TaxYear;
			}
			set
			{
				this._TaxYear = value;
			}
		}
		#endregion
		#region StartDate
		public abstract class startDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _StartDate;

		/// <summary>
		/// The start date of the period.
		/// </summary>
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "From", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual DateTime? StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				this._StartDate = value;
			}
		}
		#endregion
		#region EndDate
		public abstract class endDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _EndDate;

		/// <summary>
		/// The end date of the period (exclusive).
		/// </summary>
		[PXDBDate()]
		[PXDefault()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.Visible)]
		public virtual DateTime? EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				this._EndDate = value;
			}
		}
		#endregion
		#region EndDateUI
		public abstract class endDateUI : PX.Data.IBqlField { }

		/// <summary>
		/// The field that is used to display <see cref="EndDate"/> of the period (inclusive) in the UI.
		/// </summary>
		/// <value>
		/// Depends on and changes the value of the <see cref="EndDate"/> field by performing additional transformations.
		/// </value>
		[PXDate()]
		[PXUIField(DisplayName = "To", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual DateTime? EndDateUI
		{
			[PXDependsOnFields(typeof(endDate))]
			get
			{
				return this._EndDate.HasValue ? this._EndDate.Value.AddDays(-1) : this._EndDate;
			}
		}
		#endregion
		#region Status
		public abstract class status : PX.Data.IBqlField
		{
		}
		protected String _Status;

		/// <summary>
		/// The status of the tax period.
		/// </summary>
		/// <value>
		/// The field can have the following values:
		/// <c>"N"</c> - Open.
		/// <c>"P"</c> - Prepared.
		/// <c>"C"</c> - Closed.
		/// <c>"D"</c> - Dummy. This status is used to show periods without <see cref="TaxTran"/> on the Prepare Tax Report (TX501000) page.
		/// </value>
		[PXDBString(1, IsFixed = true)]
		[PXDefault(TaxPeriodStatus.Open)]
		[TaxPeriodStatus.List()]
		[PXUIField(DisplayName = "Status", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		public virtual String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
			}
		}
		#endregion		
		#region Filed
		public abstract class filed : PX.Data.IBqlField
		{
		}
		protected Boolean? _Filed;

		/// <summary>
		/// The field is not used.
		/// </summary>
		[Obsolete("This property is obsolete and will be removed in Acumatica 8.0")]
		[PXDBBool()]
		[PXDefault(false)]
		public virtual Boolean? Filed
		{
			get
			{
				return this._Filed;
			}
			set
			{
				this._Filed = value;
			}
		}
		#endregion
		#region tstamp
		public abstract class Tstamp : PX.Data.IBqlField
		{
		}
		protected Byte[] _tstamp;
		[PXDBTimestamp()]
		public virtual Byte[] tstamp
		{
			get
			{
				return this._tstamp;
			}
			set
			{
				this._tstamp = value;
			}
		}
		#endregion
	}
}
