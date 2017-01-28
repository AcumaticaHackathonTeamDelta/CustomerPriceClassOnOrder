using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.CR.MassProcess;
using PX.Objects.CS;
using PX.SM;

namespace PX.Objects.CR
{
	public class CRSetupMaint : PXGraph<CRSetupMaint>
	{

		public PXSave<CRSetup> Save;
		public PXCancel<CRSetup> Cancel;
		public PXSelect<CRSetup> CRSetupRecord;

		public PXSelect<CRCampaignType> CampaignType;
		public PXSelectOrderBy<CROpportunityProbability, 
			OrderBy<Asc<CROpportunityProbability.probability>>> 
			OpportunityProbabilities;

		[PXHidden]
		public PXSelect<CRValidationRules> ValidationRules;

		public PXSelect<LeadContactValidationRules, Where<LeadContactValidationRules.validationType, Equal<ValidationTypesAttribute.leadContact>>> LeadContactValidationRules;
		public PXSelect<LeadAccountValidationRules, Where<LeadAccountValidationRules.validationType, Equal<ValidationTypesAttribute.leadAccount>>> LeadAccountValidationRules;
		public PXSelect<AccountValidationRules, Where<AccountValidationRules.validationType, Equal<ValidationTypesAttribute.account>>> AccountValidationRules;


		public CRSetupMaint()
		{
			InitRulesHandlres(LeadContactValidationRules);
			InitRulesHandlres(LeadAccountValidationRules);
			InitRulesHandlres(AccountValidationRules);
		}

		#region Event Handlers

		protected virtual void CRSetup_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			var mcFeatureInstalled = PXAccess.FeatureInstalled<FeaturesSet.multicurrency>();
			PXUIFieldAttribute.SetVisible<CRSetup.defaultCuryID>(sender, null, mcFeatureInstalled);
			PXUIFieldAttribute.SetVisible<CRSetup.defaultRateTypeID>(sender, null, mcFeatureInstalled);
			PXUIFieldAttribute.SetVisible<CRSetup.allowOverrideCury>(sender, null, mcFeatureInstalled);
			PXUIFieldAttribute.SetVisible<CRSetup.allowOverrideRate>(sender, null, mcFeatureInstalled);
		}

		protected virtual void CRSetup_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			CROpportunityProbability p =  OpportunityProbabilities.Select();
            if (p == null)
            {
                p.StageCode = "A";
                p.Probability = 60;
                OpportunityProbabilities.Insert(p);
                p.StageCode = "L";
                p.Probability = 0;
                OpportunityProbabilities.Insert(p);
                p.StageCode = "P";
                p.Probability = 10;
                OpportunityProbabilities.Insert(p);
                p.StageCode = "Q";
                p.Probability = 20;
                OpportunityProbabilities.Insert(p);
                p.StageCode = "R";
                p.Probability = 80;
                OpportunityProbabilities.Insert(p);
                p.StageCode = "V";
                p.Probability = 40;
                OpportunityProbabilities.Insert(p);
                p.StageCode = "W";
                p.Probability = 100;
                OpportunityProbabilities.Insert(p);
                OpportunityProbabilities.Cache.IsDirty = false;
            }
		}

		public virtual void CRSetup_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			if (!sender.ObjectsEqual<CRSetup.contactEmailUnique, CRSetup.leadValidationThreshold>(e.Row, e.OldRow))
			{
				CRSetup row = (CRSetup) e.Row;
				LeadContactValidationRules rule = LeadContactValidationRules.Search<CRValidationRules.matchingField>(typeof(Contact.eMail).Name);
				if (row.ContactEmailUnique == true)
				{
					var emailRule = (LeadContactValidationRules)(LeadContactValidationRules.Cache.CreateCopy(rule ?? LeadContactValidationRules.Cache.CreateInstance()));
					emailRule.ValidationType = ValidationTypesAttribute.LeadContact;
					emailRule.MatchingField = this.Caches[typeof(Contact)].GetField(typeof(Contact.eMail));
					emailRule.TransformationRule = TransformationRulesAttribute.None;
					emailRule.ScoreWeight = row.LeadValidationThreshold;
					LeadContactValidationRules.Update(emailRule);					
				}
				else 
				{					
					if (rule != null)
					{
						LeadContactValidationRules.Cache.SetDefaultExt<CRValidationRules.scoreWeight>(rule);
						LeadContactValidationRules.Cache.SetDefaultExt<CRValidationRules.transformationRule>(rule);
						LeadContactValidationRules.Update(rule);						
					}
				}
			}
		}

		public virtual void LeadContactValidationRules_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			LeadContactValidationRules rule = e.Row as LeadContactValidationRules;
			if (rule == null) return;
			PXUIFieldAttribute.SetEnabled(sender, rule, 
				!(CRSetupRecord.Current.ContactEmailUnique == true && rule.MatchingField == this.Caches[typeof(Contact)].GetField(typeof(Contact.eMail))));			
		}

		public virtual void LeadContactValidationRules_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			LeadContactValidationRules rule = e.Row as LeadContactValidationRules;
			if (rule != null && CRSetupRecord.Current.ContactEmailUnique == true &&
			    rule.MatchingField == this.Caches[typeof (Contact)].GetField(typeof (Contact.eMail)))
				throw new PXException(Messages.EmailsRuleOnDuplicate);

		}

		private void InitRulesHandlres(PXSelectBase select)
		{
			Type cacheType = select.Cache.GetItemType();
		    if (select.Name == AccountValidationRules.Name)
		    {
                this.FieldSelecting.AddHandler(cacheType, typeof(CRValidationRules.matchingField).Name, (sender, e) => CreateFieldStateForFieldName(e, typeof(Location)));
		    }
		    else
		    {
                this.FieldSelecting.AddHandler(cacheType, typeof(CRValidationRules.matchingField).Name, (sender, e) => CreateFieldStateForFieldName(e, typeof(Contact)));    
		    }
			
			this.RowInserted.AddHandler(cacheType, (sender, e) => UpdateGrammValidationDate());
			this.RowUpdated.AddHandler(cacheType, (sender, e) => UpdateGrammValidationDate());
			this.RowDeleted.AddHandler(cacheType, (sender, e) => UpdateGrammValidationDate());
		}

		
		protected virtual void CRSetup_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			CRSetup row = e.Row as CRSetup;
			if (row != null && row.GrammValidationDateTime == null)			
				row.GrammValidationDateTime = PXTimeZoneInfo.Now;			
		}

		private void UpdateGrammValidationDate()
		{			
			CRSetup record = PXCache<CRSetup>.CreateCopy(this.CRSetupRecord.Current) as CRSetup;
			record.GrammValidationDateTime = null;			
			CRSetupRecord.Update(record);
		}

		#endregion

		private void CreateFieldStateForFieldName(PXFieldSelectingEventArgs e, Type type)
		{
			List<string> allowedValues = new List<string>();
			List<string> allowedLabels = new List<string>();

			Dictionary<string, string> fields = new Dictionary<string, string>();

			foreach (var field in PXCache.GetBqlTable(type)
						.GetProperties(BindingFlags.Instance | BindingFlags.Public)
						.SelectMany(p => p.GetCustomAttributes(true).Where(atr => atr is PXMassMergableFieldAttribute),(p, atr) => p))
			{

				PXFieldState fs = this.Caches[type].GetStateExt(null, field.Name) as PXFieldState;
				if(!fields.ContainsKey(field.Name))
					fields[field.Name] = fs != null ? fs.DisplayName : field.Name;
			}

            if (type == typeof(Location))
            {
                foreach (var field in PXCache.GetBqlTable(typeof(Contact))
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .SelectMany(p => p.GetCustomAttributes(true).Where(atr => atr is PXMassMergableFieldAttribute), (p, atr) => p))
                {

                    PXFieldState fs = this.Caches[type].GetStateExt(null, field.Name) as PXFieldState;
                    if (!fields.ContainsKey(field.Name))
                        fields[field.Name] = fs != null ? fs.DisplayName : field.Name;
                }
            }

			foreach (var item in fields.OrderBy(i => i.Value))
			{
				allowedValues.Add(item.Key);
				allowedLabels.Add(item.Value);
			}

			e.ReturnState = PXStringState.CreateInstance(e.ReturnValue, 60, null, "FieldName", false, 1, null,
												allowedValues.ToArray(), allowedLabels.ToArray(), true, null);
		}
	}
}
