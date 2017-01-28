using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.IN;

namespace PX.Objects.EP
{

    public class EmployeeCostEngine
	{
		protected PXGraph graph;
		protected string defaultUOM = EPSetup.Hour;
		
		public EmployeeCostEngine(PXGraph graph)
		{
			if ( graph == null )
				throw new ArgumentNullException();

			this.graph = graph;

			EPSetup setup = PXSelect<EPSetup>.Select(graph);
            if (setup != null && !String.IsNullOrEmpty(setup.EmployeeRateUnit))
			    defaultUOM = setup.EmployeeRateUnit;
		}


		public class Rate
		{
			public int? EmployeeID { get; private set; }
			public decimal HourlyRate { get; private set; }
			public string Type { get; private set; }
			public string UOM { get; private set; }

            /// <summary>
            /// Total Hours in week
            /// </summary>
			public decimal? RegularHours { get; private set; }

            /// <summary>
            /// Employee Rate for a Week
            /// </summary>
			public decimal? RateByType { get; private set; }
			
			public Rate(int? employeeID, string type, decimal? hourlyRate, string uom, decimal? regularHours, decimal? rateByType)
			{
				this.EmployeeID = employeeID;
				this.UOM = uom;
				this.HourlyRate = hourlyRate ?? 0m;
				this.Type = string.IsNullOrEmpty(type) ? RateTypesAttribute.Hourly : type;
				this.RegularHours = regularHours ?? 0;
				this.RateByType = rateByType;
			}
		}
		

		public virtual Rate GetEmployeeRate(int? projectId, int? projectTaskId, int? employeeId, DateTime? date)
		{
			decimal? hourlyRate = null;
			decimal? rate = null;
			PXResult<EPEmployeeRate, EPEmployeeRateByProject> result = ((PXResult<EPEmployeeRate, EPEmployeeRateByProject>)
				PXSelectJoin<
					EPEmployeeRate
					, LeftJoin<
						EPEmployeeRateByProject
						, On<
							EPEmployeeRate.rateID, Equal<EPEmployeeRateByProject.rateID>
							, And<EPEmployeeRateByProject.projectID, Equal<Required<EPEmployeeRateByProject.projectID>>
								, And<Where<
									EPEmployeeRateByProject.taskID, Equal<Required<EPEmployeeRateByProject.taskID>>
									, Or<EPEmployeeRateByProject.taskID, IsNull>
									>>
								>
							>
						>
					, Where<
						EPEmployeeRate.employeeID, Equal<Required<EPEmployeeRate.employeeID>>
						, And<EPEmployeeRate.effectiveDate, LessEqual<Required<EPEmployeeRate.effectiveDate>>>
						>
						, OrderBy<Desc<EPEmployeeRate.effectiveDate, Desc<EPEmployeeRateByProject.taskID>>>
					>.SelectWindowed(graph, 0, 1, projectId, projectTaskId, employeeId, date)).
				With(_ => (_));

			EPEmployeeRateByProject employeeRateByProject = (EPEmployeeRateByProject) result;
			EPEmployeeRate employeeRate = (EPEmployeeRate) result;

			if (employeeRateByProject != null && employeeRateByProject.HourlyRate != null)
				hourlyRate = employeeRateByProject.HourlyRate;
            else if (employeeRate != null)
                hourlyRate = employeeRate.HourlyRate;
            else
            {
                CR.BAccountR baccount = PXSelect<CR.BAccountR, Where<CR.BAccountR.bAccountID, Equal<Required<CR.BAccountR.bAccountID>>>>.Select(graph, employeeId);
                throw new PXException(Messages.HourlyRateIsNotSet, date, baccount.AcctCD);
            }
		    if (employeeRate.RateType == RateTypesAttribute.Hourly)
				rate = hourlyRate;
			else
				rate = hourlyRate * employeeRate.RegularHours;

			return new Rate(employeeId, employeeRate.RateType, hourlyRate, defaultUOM, employeeRate.RegularHours, rate);
		}

        public virtual EPEmployeeRate GetEmployeeRate(int? employeeID, DateTime? date)
        {
            PXSelectBase<EPEmployeeRate> select = new PXSelect<EPEmployeeRate, Where<EPEmployeeRate.employeeID, Equal<Required<EPEmployeeRate.employeeID>>,
                    And<EPEmployeeRate.effectiveDate, LessEqual<Required<EPEmployeeRate.effectiveDate>>>>,
                    OrderBy<Desc<EPEmployeeRate.effectiveDate>>>(graph);

            return select.SelectWindowed(0, 1, employeeID, date);
        }

		public virtual decimal? CalculateEmployeeCost(PMTimeActivity activity, int? employeeID, DateTime date)
		{
			decimal? cost;
			Rate employeeRate = GetEmployeeRate(activity.ProjectID, activity.ProjectTaskID, employeeID, date);

			if (employeeRate.Type == RateTypesAttribute.SalaryWithExemption && activity.TimeCardCD != null)
			{
				//Overtime is not applicable. Rate is prorated based on actual hours worked on the given week

				EPTimeCard timecard = PXSelect<EPTimeCard, 
					Where<EPTimeCard.timeCardCD, Equal<Required<PMTimeActivity.timeCardCD>>>>.Select(graph, activity.TimeCardCD);

				if (timecard.TotalSpentCalc <= employeeRate.RegularHours * 60m)
                {
                    cost = employeeRate.RateByType / employeeRate.RegularHours;
                }
                else
                {
					cost = employeeRate.RateByType / (timecard.TotalSpentCalc / 60m);
                }
			}
			else
			{
				cost = employeeRate.HourlyRate * GetOvertimeMultiplier(activity.EarningTypeID, (int)employeeID, date);
			}

			return cost;
		}

		public virtual decimal GetOvertimeMultiplier(string earningTypeID, int employeeID, DateTime effectiveDate)
		{
			EPEmployeeRate employeeRate = GetEmployeeRate(employeeID, effectiveDate);
			if (employeeRate != null && employeeRate.RateType == RateTypesAttribute.SalaryWithExemption)
				return 1;
			EPEarningType earningType = PXSelect<EPEarningType>.Search<EPEarningType.typeCD>(graph, earningTypeID);
			return earningType != null && earningType.IsOvertime == true ? (decimal)earningType.OvertimeMultiplier : 1;
	    }

	    public virtual int? GetLaborClass(PMTimeActivity activity)
		{
			CRCase refCase = PXSelectJoin<CRCase, 
				InnerJoin<CRActivityLink, 
					On<CRActivityLink.refNoteID, Equal<CRCase.noteID>>>,
				Where<CRActivityLink.noteID, Equal<Required<PMTimeActivity.refNoteID>>>>.Select(graph, activity.RefNoteID);

			EPEmployee employee = PXSelect<EPEmployee>.Search<EPEmployee.userID>(graph, activity.OwnerID);

			return GetLaborClass(activity, employee, refCase);
		}

		public virtual int? GetLaborClass(PMTimeActivity activity, EPEmployee employee, CRCase refCase)
		{
			if (employee == null)
				throw new ArgumentNullException("employee", Messages.EmptyEmployeeID);

			int? laborClassID = null;

			if (refCase != null)
			{
				CRCaseClass caseClass = (CRCaseClass)PXSelectorAttribute.Select<CRCase.caseClassID>(graph.Caches[typeof(CRCase)], refCase);
				if (caseClass.PerItemBilling == BillingTypeListAttribute.PerActivity)
				laborClassID = CRCaseClassLaborMatrix.GetLaborClassID(graph, caseClass.CaseClassID, activity.EarningTypeID);
			}

			if (laborClassID == null && activity.ProjectID != null && employee.BAccountID != null)
				laborClassID = EPContractRate.GetProjectLaborClassID(graph, (int)activity.ProjectID, (int)employee.BAccountID, activity.EarningTypeID);

			if (laborClassID == null)
				laborClassID = EPEmployeeClassLaborMatrix.GetLaborClassID(graph, employee.BAccountID, activity.EarningTypeID);

			if (laborClassID == null)
				laborClassID = employee.LabourItemID;

			return laborClassID;
		}



	}
}
