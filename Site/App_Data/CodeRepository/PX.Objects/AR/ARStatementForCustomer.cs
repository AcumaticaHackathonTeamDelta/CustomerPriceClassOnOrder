using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.AP;
using PX.Objects.CM;
using System.Globalization;


namespace PX.Objects.AR
{
#if false
	public class DetailKey : IComparable<DetailKey>, IEquatable<DetailKey>
	{
		public DetailKey(string aFirst, string aSecond) 
		{
			this.first = aFirst;
			this.second = aSecond;
		}
		public string first;
		public string second;

    #region IComparable<CashAcctKey> Members
		public virtual int CompareTo(DetailKey other)
		{
			int res = this.first.CompareTo(other.first);
			if (res == 0) return (this.second.CompareTo(other.second));
			return res;
		}

		public override int GetHashCode()
		{
			return (this.first.GetHashCode())^(this.second.GetHashCode()); //Force to call the CompareTo methods in dicts
		}
    #endregion


    #region IComparable<DetailKey> Members

		int IComparable<DetailKey>.CompareTo(DetailKey other)
		{
			return this.CompareTo(other);
		}

    #endregion

    #region IEquatable<DetailKey> Members

		public virtual bool Equals(DetailKey other)
		{
		    return (this.CompareTo(other)==0);
		}

		//public override bool Equals(Object obj) 
		//{
		//    DetailKey key = obj as DetailKey;
		//    if (key == null) return false;
		//    return Equals(key);
		//}
    #endregion
	}
#endif
    [PX.Objects.GL.TableAndChartDashboardType]
    public class ARStatementForCustomer : PXGraph<ARStatementForCustomer>
    {
        [System.SerializableAttribute()]
        public partial class ARStatementForCustomerParameters : IBqlTable
        {
            #region CustomerID
            public abstract class customerID : PX.Data.IBqlField
            {
            }
            protected Int32? _CustomerID;
            [PXInt()]
            [PXDefault()]
            [PXUIField(DisplayName = "Customer")]
            [Customer(DescriptionField = typeof(Customer.acctName))]
            public virtual Int32? CustomerID
            {
                get
                {
                    return this._CustomerID;
                }
                set
                {
                    this._CustomerID = value;
                }
            }
            #endregion
        }

        [Serializable]
        public partial class DetailsResult : IBqlTable
        {
            #region StatementCycleId
            public abstract class statementCycleId : PX.Data.IBqlField
            {
            }
            protected String _StatementCycleId;
            [PXString(10, IsUnicode = true)]
            [PXUIField(DisplayName = "Statement Cycle")]
            [PXSelector(typeof(ARStatementCycle.statementCycleId))]
            public virtual String StatementCycleId
            {
                get
                {
                    return this._StatementCycleId;
                }
                set
                {
                    this._StatementCycleId = value;
                }
            }
            #endregion
            #region StatementDate
            public abstract class statementDate : PX.Data.IBqlField
            {
            }
            protected DateTime? _StatementDate;
            [PXDate(IsKey = true)]
            [PXDefault()]
            [PXUIField(DisplayName = "Statement Date")]
            [PXSelector(typeof(Search4<ARStatement.statementDate, Aggregate<GroupBy<ARStatement.statementDate>>>))]
            public virtual DateTime? StatementDate
            {
                get
                {
                    return this._StatementDate;
                }
                set
                {
                    this._StatementDate = value;
                }
            }
            #endregion
            #region StatementBalance
            public abstract class statementBalance : PX.Data.IBqlField
            {
            }
            protected Decimal? _StatementBalance;
            [PXDBBaseCury()]
            [PXUIField(DisplayName = "Statement Balance")]
            public virtual Decimal? StatementBalance
            {
                get
                {
                    return this._StatementBalance;
                }
                set
                {
                    this._StatementBalance = value;
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
            #region CuryStatementBalance
            public abstract class curyStatementBalance : PX.Data.IBqlField
            {
            }
            protected Decimal? _CuryStatementBalance;
            [PXCury(typeof(DetailsResult.curyID))]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "FC Statement Balance")]
            public virtual Decimal? CuryStatementBalance
            {
                get
                {
                    return this._CuryStatementBalance;
                }
                set
                {
                    this._CuryStatementBalance = value;
                }
            }
            #endregion
            #region DontPrint
            public abstract class dontPrint : PX.Data.IBqlField
            {
            }
            protected Boolean? _DontPrint;
            [PXBool()]
            [PXDefault(true)]
            [PXUIField(DisplayName = "Don't Print")]
            public virtual Boolean? DontPrint
            {
                get
                {
                    return this._DontPrint;
                }
                set
                {
                    this._DontPrint = value;
                }
            }
            #endregion
            #region Printed
            public abstract class printed : PX.Data.IBqlField
            {
            }
            protected Boolean? _Printed;
            [PXBool()]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Printed")]
            public virtual Boolean? Printed
            {
                get
                {
                    return this._Printed;
                }
                set
                {
                    this._Printed = value;
                }
            }
            #endregion
            #region DontEmail
            public abstract class dontEmail : PX.Data.IBqlField
            {
            }
            protected Boolean? _DontEmail;
            [PXDBBool()]
            [PXDefault(true)]
            [PXUIField(DisplayName = "Don't Email")]
            public virtual Boolean? DontEmail
            {
                get
                {
                    return this._DontEmail;
                }
                set
                {
                    this._DontEmail = value;
                }
            }
            #endregion
            #region Emailed
            public abstract class emailed : PX.Data.IBqlField
            {
            }
            protected Boolean? _Emailed;
            [PXDBBool()]
            [PXDefault(false)]
            [PXUIField(DisplayName = "Emailed")]
            public virtual Boolean? Emailed
            {
                get
                {
                    return this._Emailed;
                }
                set
                {
                    this._Emailed = value;
                }
            }
            #endregion
            #region AgeBalance00
            public abstract class ageBalance00 : PX.Data.IBqlField
            {
            }
            protected Decimal? _AgeBalance00;
            [PXDBBaseCury()]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "Age00 Balance")]
            public virtual Decimal? AgeBalance00
            {
                get
                {
                    return this._AgeBalance00;
                }
                set
                {
                    this._AgeBalance00 = value;
                }
            }
            #endregion
            #region CuryAgeBalance00
            public abstract class curyAgeBalance00 : PX.Data.IBqlField
            {
            }
            protected Decimal? _CuryAgeBalance00;
            [PXCury(typeof(DetailsResult.curyID))]
            [PXDefault(TypeCode.Decimal, "0.0")]
            [PXUIField(DisplayName = "FC Age00 Balance")]
            public virtual Decimal? CuryAgeBalance00
            {
                get
                {
                    return this._CuryAgeBalance00;
                }
                set
                {
                    this._CuryAgeBalance00 = value;
                }
            }
            #endregion
            #region OverdueBalance
            public abstract class overdueBalance : PX.Data.IBqlField
            {
            }
            [PXBaseCury()]
            [PXUIField(DisplayName = "Overdue Balance")]
            public virtual Decimal? OverdueBalance
            {
                [PXDependsOnFields(typeof(statementBalance), typeof(ageBalance00))]
                get
                {
                    return this.StatementBalance - this.AgeBalance00;
                }
            }
            #endregion
            #region CuryOverdueBalance
            public abstract class curyOverdueBalance : PX.Data.IBqlField
            {
            }
            [PXCury(typeof(DetailsResult.curyID))]
            [PXUIField(DisplayName = "FC Overdue Balance")]
            public virtual Decimal? CuryOverdueBalance
            {
                [PXDependsOnFields(typeof(curyStatementBalance), typeof(curyAgeBalance00))]
                get
                {
                    return (this._CuryStatementBalance ?? Decimal.Zero) - (this.CuryAgeBalance00 ?? Decimal.Zero);
                }
            }
            #endregion
            #region BranchID
            public abstract class branchID : PX.Data.IBqlField
            {
            }
            protected Int32? _BranchID;
            [PXDBInt(IsKey = true)]
            [PXDefault()]
            [PX.Objects.GL.Branch()]
            [PXUIField(DisplayName = "Branch", Visible = false)]
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

            public virtual void Copy(ARStatement aSrc, Customer cust)
            {
                this.StatementCycleId = aSrc.StatementCycleId;
                this.StatementDate = aSrc.StatementDate;
                this.StatementBalance = aSrc.EndBalance ?? decimal.Zero;
                this.AgeBalance00 = aSrc.AgeBalance00 ?? decimal.Zero;
                this.CuryID = aSrc.CuryID;
                this.CuryStatementBalance = aSrc.CuryEndBalance ?? decimal.Zero;
                this.CuryAgeBalance00 = aSrc.CuryAgeBalance00 ?? decimal.Zero;
                this.DontPrint = aSrc.DontPrint;
                this.Printed = aSrc.Printed;
                this.DontEmail = aSrc.DontEmail;
                this.Emailed = aSrc.Emailed;
                this.BranchID = aSrc.BranchID;
            }
            public virtual void Append(DetailsResult aSrc)
            {
                this.StatementBalance += aSrc.StatementBalance;
                this.AgeBalance00 += aSrc.AgeBalance00;
                if (this.CuryID == aSrc.CuryID)
                {
                    this.CuryStatementBalance += aSrc.CuryStatementBalance;
                    this.CuryAgeBalance00 += aSrc.CuryAgeBalance00;
                }
                else
                {
                    this.CuryStatementBalance = Decimal.Zero;
                    this.CuryAgeBalance00 = Decimal.Zero;
                }

                if (aSrc.DontPrint == false)
                    this.DontPrint = false;
                if (aSrc.DontEmail == false)
                    this.DontEmail = false;
                if (aSrc.Printed == false)
                    this.Printed = false;
                if (aSrc.Emailed == false)
                    this.Emailed = false;

            }
            public virtual void ResetToBaseCury(string aBaseCuryID)
            {
                this._CuryID = aBaseCuryID;
                this._CuryStatementBalance = this._StatementBalance;
                this._CuryAgeBalance00 = this._AgeBalance00;
            }
        }
        public class DetailKey : AP.Pair<DateTime, string>, IEquatable<DetailKey>
        {
            public DetailKey(DateTime aFirst, string aSecond)
                : base(aFirst, aSecond)
            {

            }

            #region IComparable<CashAcctKey> Members


            public override int GetHashCode()
            {
                return (this.first.GetHashCode()) ^ (this.second.GetHashCode()); //Force to call the CompareTo methods in dicts
            }
            #endregion



            #region IEquatable<DetailKey> Members

            public virtual bool Equals(DetailKey other)
            {
                return base.Equals(other);
            }

            #endregion
        }

        public PXFilter<ARStatementForCustomerParameters> Filter;
        public PXCancel<ARStatementForCustomerParameters> Cancel;
        [PXFilterable]
        public PXSelect<DetailsResult> Details;

        public ARStatementForCustomer()
        {
            ARSetup setup = ARSetup.Current;
            Details.Cache.AllowDelete = false;
            Details.Cache.AllowInsert = false;
            Details.Cache.AllowUpdate = false;
        }

        public PXSetup<ARSetup> ARSetup;

        protected virtual System.Collections.IEnumerable details()
        {
            ARStatementForCustomerParameters header = Filter.Current;
            Dictionary<DetailKey, DetailsResult> result = new Dictionary<DetailKey, DetailsResult>(EqualityComparer<DetailKey>.Default);
            List<DetailsResult> curyResult = new List<DetailsResult>();
            if (header == null)
            {
                return curyResult;
            }
            Customer customer = PXSelect<Customer, Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>.Select(this, header.CustomerID);
            if (customer != null)
            {
                bool useCurrency = customer.PrintCuryStatements ?? false;
                GL.Company company = PXSelect<GL.Company>.Select(this);

                foreach (ARStatement st in PXSelect<ARStatement,
                       Where<ARStatement.statementCustomerID, Equal<Required<ARStatement.customerID>>>,
                       OrderBy<Asc<ARStatement.statementCycleId, Asc<ARStatement.statementDate, Asc<ARStatement.curyID>>>>>
                       .Select(this, header.CustomerID))
                {
                    DetailsResult res = new DetailsResult();
                    res.Copy(st, customer);
                    if (useCurrency)
                    {
                        DetailsResult last = curyResult.Count > 0 ? curyResult[curyResult.Count - 1] : null;
                        if (last != null
                            && last.StatementCycleId == res.StatementCycleId
                            && last.StatementDate == res.StatementDate && last.CuryID == res.CuryID)
                        {
                            last.Append(res);
                        }
                        else
                        {
                            curyResult.Add(res);
                        }
                        //curyResult.Add(res);
                    }
                    else
                    {
                        DetailKey key = new DetailKey(res.StatementDate.Value, res.StatementCycleId);
                        res.ResetToBaseCury(company.BaseCuryID);
                        if (!result.ContainsKey(key))
                        {
                            result[key] = res;
                        }
                        else
                        {
                            result[key].Append(res);
                        }
                    }
                }
                PXUIFieldAttribute.SetVisible<DetailsResult.curyID>(this.Details.Cache, null, useCurrency);
                PXUIFieldAttribute.SetVisible<DetailsResult.curyStatementBalance>(this.Details.Cache, null, useCurrency);
                PXUIFieldAttribute.SetVisible<DetailsResult.curyOverdueBalance>(this.Details.Cache, null, useCurrency);
                PXUIFieldAttribute.SetVisible<DetailsResult.statementBalance>(this.Details.Cache, null, !useCurrency);
                PXUIFieldAttribute.SetVisible<DetailsResult.overdueBalance>(this.Details.Cache, null, !useCurrency);

                return useCurrency ? (System.Collections.IEnumerable)curyResult : (System.Collections.IEnumerable)result.Values;
                //return (System.Collections.IEnumerable)result.Values;
            }
            return curyResult;
        }

        #region Sub-screen Navigation Button
        public PXAction<ARStatementForCustomerParameters> printReport;
        [PXUIField(DisplayName = "Print Statement", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton]
        public System.Collections.IEnumerable PrintReport(PXAdapter adapter)
        {
            if (this.Details.Current != null && this.Filter.Current != null)
            {
                if (this.Filter.Current.CustomerID.HasValue)
                {
                    Customer customer = PXSelect<Customer,
                        Where<Customer.bAccountID, Equal<Required<Customer.bAccountID>>>>
                        .Select(this, Filter.Current.CustomerID);
                    if (customer != null)
                    {
                        Dictionary<string, string> parameters = ARStatementReportParams.FromCustomer(customer);
                        Export(parameters, this.Details.Current);

                        string reportID = ARStatementReportParams.ReportIDForCustomer(this, customer, this.Details.Current.BranchID);
                        var reportRequired = PXReportRequiredException.CombineReport(
                            null,
                            reportID,
                            parameters);

                        if (reportRequired != null)
                            throw reportRequired;
                    }
                }
            }
            return Filter.Select();
        }
        protected static void Export(Dictionary<string, string> aRes, DetailsResult aDetail)
        {
            aRes[ARStatementReportParams.Parameters.StatementCycleID] = aDetail.StatementCycleId;
            aRes[ARStatementReportParams.Parameters.StatementDate] = aDetail.StatementDate.Value.ToString("d", CultureInfo.InvariantCulture);
        }


        #endregion
    }
}
