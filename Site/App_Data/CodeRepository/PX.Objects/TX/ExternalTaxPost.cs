using System;
using System.Collections.Generic;
using System.Text;
using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.TaxService;
using PX.Data;
using System.Collections;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.GL;

namespace PX.Objects.TX
{
    [Serializable]
	public class ExternalTaxPost : PXGraph<ExternalTaxPost>
	{
		[PXFilterable]
		public PXProcessing<Document> Items;
		
		public IEnumerable items()
		{
			bool found = false;
			foreach (Document item in Items.Cache.Inserted)
			{
				found = true;
				yield return item;
			}
			if (found)
				yield break;

			PXSelectBase<ARInvoice> selectAR = new PXSelectJoin<ARInvoice,
				InnerJoin<TaxZone, On<TaxZone.taxZoneID, Equal<ARInvoice.taxZoneID>>>,
				Where<TaxZone.isExternal, Equal<True>,
				And<ARInvoice.isTaxValid, Equal<True>,
				And<ARInvoice.released, Equal<True>,
				And<ARInvoice.isTaxPosted, Equal<False>>>>>>(this);

			PXSelectBase<APInvoice> selectAP = new PXSelectJoin<APInvoice, 
				InnerJoin<TaxZone, On<TaxZone.taxZoneID, Equal<APInvoice.taxZoneID>>>,
				Where<TaxZone.isExternal, Equal<True>,
				And<APInvoice.isTaxValid, Equal<True>,
				And<APInvoice.released, Equal<True>,
				And<APInvoice.isTaxPosted, Equal<False>>>>>>(this);

			PXSelectBase<CAAdj> selectCA = new PXSelectJoin<CAAdj,
				InnerJoin<TaxZone, On<TaxZone.taxZoneID, Equal<CAAdj.taxZoneID>>>,
				Where<TaxZone.isExternal, Equal<True>,
				And<CAAdj.isTaxValid, Equal<True>,
				And<CAAdj.released, Equal<True>,
				And<CAAdj.isTaxPosted, Equal<False>>>>>>(this);


			foreach ( ARInvoice item in selectAR.Select())
			{
				Document doc = new Document();
				doc.Module = GL.BatchModule.AR;
				doc.DocType = item.DocType;
				doc.RefNbr = item.RefNbr;
				doc.BranchID = item.BranchID;
				doc.DocDate = item.DocDate;
				doc.FinPeriodID = item.FinPeriodID;
				doc.Amount = item.CuryDocBal;
				doc.CuryID = item.CuryID;
				doc.DocDesc = item.DocDesc;
				doc.DrCr = null;

				yield return Items.Insert(doc);
			}

			foreach (APInvoice item in selectAP.Select())
			{
				Document doc = new Document();
				doc.Module = GL.BatchModule.AP;
				doc.DocType = item.DocType;
				doc.RefNbr = item.RefNbr;
				doc.BranchID = item.BranchID;
				doc.DocDate = item.DocDate;
				doc.FinPeriodID = item.FinPeriodID;
				doc.Amount = item.CuryDocBal;
				doc.CuryID = item.CuryID;
				doc.DocDesc = item.DocDesc;
				doc.DrCr = null;

				yield return Items.Insert(doc);
			}

			foreach (CAAdj item in selectCA.Select())
			{
				Document doc = new Document();
				doc.Module = GL.BatchModule.CA;
				doc.DocType = item.DocType;
				doc.RefNbr = item.RefNbr;
				doc.BranchID = item.BranchID;
				doc.DocDate = item.TranDate;
				doc.FinPeriodID = item.FinPeriodID;
				doc.Amount = item.CuryTranAmt;
				doc.CuryID = item.CuryID;
				doc.DocDesc = item.TranDesc;
				doc.DrCr = item.DrCr;

				yield return Items.Insert(doc);
			}

			Items.Cache.IsDirty = false;
		}

		public ExternalTaxPost()
		{
			Items.SetSelected<Document.selected>();
			Items.SetProcessDelegate(Release);
			Items.SetProcessCaption("Post");
			Items.SetProcessAllCaption("Post All");
		}

		public PXAction<Document> viewDocument;
		[PXUIField(Visible = false, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton()]
		public virtual IEnumerable ViewDocument(PXAdapter adapter)
		{
			Document doc = Items.Current;
			if (doc != null && !String.IsNullOrEmpty(doc.Module) && !String.IsNullOrEmpty(doc.DocType) && !String.IsNullOrEmpty(doc.RefNbr))
			{
				switch (doc.Module)
				{
					case GL.BatchModule.AR:
						ARInvoiceEntry argraph = PXGraph.CreateInstance<ARInvoiceEntry>();
						argraph.Document.Current = argraph.Document.Search<ARInvoice.refNbr>(doc.RefNbr, doc.DocType);
						if (argraph.Document.Current != null)
						{
							throw new PXRedirectRequiredException(argraph, true, "View Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
						}						
					break;

					case GL.BatchModule.AP:	
						APInvoiceEntry apgraph = PXGraph.CreateInstance<APInvoiceEntry>();
						apgraph.Document.Current = apgraph.Document.Search<APInvoice.refNbr>(doc.RefNbr, doc.DocType);
						if (apgraph.Document.Current != null)
						{
							throw new PXRedirectRequiredException(apgraph, true, "View Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
						}		
					break;

					case GL.BatchModule.CA:
						CATranEntry cagraph = PXGraph.CreateInstance<CATranEntry>();
						cagraph.CAAdjRecords.Current = cagraph.CAAdjRecords.Search<CAAdj.adjRefNbr>(doc.RefNbr, doc.DocType);
						if (cagraph.CAAdjRecords.Current != null)
						{
							throw new PXRedirectRequiredException(cagraph, true, "View Document") { Mode = PXBaseRedirectException.WindowMode.NewWindow };
						}
					break;
				}
			}

			return adapter.Get();
		}

		public static void Release(List<Document> list)
		{
			ExternalTaxPostProcess rg = PXGraph.CreateInstance<ExternalTaxPostProcess>();
			for (int i = 0; i < list.Count; i++)
			{
				Document doc = list[i];

				try
				{
					rg.Clear();
					rg.Post(doc);
					PXProcessing<Document>.SetInfo(i, ActionsMessages.RecordProcessed);
				}
				catch (Exception e)
				{
					PXProcessing<Document>.SetError(i, e is PXOuterException ? e.Message + "\r\n" + String.Join("\r\n", ((PXOuterException)e).InnerMessages) : e.Message);
				}

			}
		}

		protected virtual void Document_RowPersisting(PXCache sedner, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

        [Serializable]
		public class Document : IBqlTable
		{
			#region Module
			public abstract class module : PX.Data.IBqlField
			{
			}
			protected String _Module;
			[PXString(2, IsFixed = true, IsKey = true)]
			[PXUIField(DisplayName = "Module")]
			[GL.BatchModule.FullList()]
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
			#region DocType
			public abstract class docType : PX.Data.IBqlField
			{
			}
			protected String _DocType;
			[PXString(3, IsKey = true, IsFixed = true)]
			[PXUIField(DisplayName = "Doc. Type")]
			public virtual String DocType
			{
				get
				{
					return this._DocType;
				}
				set
				{
					this._DocType = value;
				}
			}
			#endregion
			#region RefNbr
			public abstract class refNbr : PX.Data.IBqlField
			{
			}
			protected String _RefNbr;
			[PXString(15, IsUnicode = true, IsKey = true, InputMask = "")]
			[PXUIField(DisplayName = "Reference Nbr.")]
			public virtual String RefNbr
			{
				get
				{
					return this._RefNbr;
				}
				set
				{
					this._RefNbr = value;
				}
			}
			#endregion

			#region Selected
			public abstract class selected : PX.Data.IBqlField
			{
			}
			protected bool? _Selected = false;
			[PXBool]
			[PXDefault(false)]
			[PXUIField(DisplayName = "Selected")]
			public virtual bool? Selected
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
			#region BranchID
			public abstract class branchID : PX.Data.IBqlField
			{
			}
			protected Int32? _BranchID;
			[Branch()]
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
			#region DocDate
			public abstract class docDate : PX.Data.IBqlField
			{
			}
			protected DateTime? _DocDate;
			[PXDate()]
			[PXUIField(DisplayName = "Date")]
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
			[PeriodID]
			[PXUIField(DisplayName = "Fin. Period")]
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
			#region Amount
			public abstract class amount : PX.Data.IBqlField
			{
			}
			protected Decimal? _Amount;
			[PXBaseCury]
			[PXUIField(DisplayName = "Amount", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
			public virtual Decimal? Amount
			{
				get
				{
					return this._Amount;
				}
				set
				{
					this._Amount = value;
				}
			}
			#endregion
			#region CuryID
			public abstract class curyID : PX.Data.IBqlField
			{
			}
			protected String _CuryID;
			[PXString(5, IsUnicode = true, InputMask = ">LLLLL")]
			[PXUIField(DisplayName = "Currency")]
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
			#region DocDesc
			public abstract class docDesc : PX.Data.IBqlField
			{
			}
			protected String _DocDesc;
			[PXString(60, IsUnicode = true)]
			[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
			public virtual String DocDesc
			{
				get
				{
					return this._DocDesc;
				}
				set
				{
					this._DocDesc = value;
				}
			}
			#endregion
			#region DrCr
			public abstract class drCr : PX.Data.IBqlField
			{
			}
			protected String _DrCr;
			[PXDBString(1, IsFixed = true)]
			public virtual String DrCr
			{
				get
				{
					return this._DrCr;
				}
				set
				{
					this._DrCr = value;
				}
			}
			#endregion
		}

		public class ExternalTaxPostProcess : PXGraph<ExternalTaxPostProcess>
		{
			protected TaxSvc service;

			public ExternalTaxPostProcess()
			{
				service = new TaxSvc();
				AvalaraMaint.SetupService(this, service);
			}

			public void Post(Document doc)
			{
				TXAvalaraSetup avalaraSetup = PXSelect<TXAvalaraSetup>.Select(this);
				if (avalaraSetup == null)
					throw new PXException(Messages.AvalaraSetupNotConfigured);

				CommitTaxRequest request = new CommitTaxRequest();
				request.CompanyCode = AvalaraMaint.CompanyCodeFromBranch(this, doc.BranchID);
				request.DocCode = string.Format("{0}.{1}.{2}", doc.Module, doc.DocType, doc.RefNbr);

				if (doc.Module == "AP")
				{
					if (doc.DocType == AP.APDocType.Refund)
						request.DocType = DocumentType.ReturnInvoice;
					else
						request.DocType = DocumentType.PurchaseInvoice;
				}
				else if (doc.Module == "AR")
				{
					if (doc.DocType == AR.ARDocType.CreditMemo)
						request.DocType = DocumentType.ReturnInvoice;
					else
						request.DocType = DocumentType.SalesInvoice;
				}
				else if (doc.Module == "CA")
				{
					if (doc.DrCr == CA.CADrCr.CADebit)
						request.DocType = DocumentType.SalesInvoice;
					else
						request.DocType = DocumentType.PurchaseInvoice;
				}
				else
				{
					throw new PXException("Invalid Module {0}. Only AP, AR, CA is expected", doc.Module);
				}
				
				CommitTaxResult result = service.CommitTax(request);
				bool setPosted = false;

				if (result.ResultCode == SeverityLevel.Success)
				{
					setPosted = true;
				}
				else
				{
					//Avalara retuned an error - The given document is already marked as posted on the avalara side.
					//Just fix the discrepency by setting the IsTaxPosted=1 in the acumatica document. Do not return this as an error to the user.
					if (result.ResultCode == SeverityLevel.Error && result.Messages.Count == 1 && result.Messages[0].Details == "Expected Posted")
						setPosted = true;
				}


				if (setPosted)
				{

					if (doc.Module == "AP")
					{
						PXDatabase.Update<AP.APRegister>(
						new PXDataFieldAssign("IsTaxPosted", true),
						new PXDataFieldRestrict("DocType", PXDbType.Char, 3, doc.DocType, PXComp.EQ),
						new PXDataFieldRestrict("RefNbr", PXDbType.NVarChar, 15, doc.RefNbr, PXComp.EQ)
						);
					}
					else if (doc.Module == "AR")
					{
						PXDatabase.Update<AR.ARRegister>(
						new PXDataFieldAssign("IsTaxPosted", true),
						new PXDataFieldRestrict("DocType", PXDbType.Char, 3, doc.DocType, PXComp.EQ),
						new PXDataFieldRestrict("RefNbr", PXDbType.NVarChar, 15, doc.RefNbr, PXComp.EQ)
						);
					}
					else if (doc.Module == "CA")
					{
						PXDatabase.Update<CA.CAAdj>(
						new PXDataFieldAssign("IsTaxPosted", true),
						new PXDataFieldRestrict("AdjRefNbr", PXDbType.NVarChar, 15, doc.RefNbr, PXComp.EQ)
						);
					}
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					foreach (Avalara.AvaTax.Adapter.Message msg in result.Messages)
					{
						sb.AppendLine(msg.Name + ": " + msg.Details);
					}

					throw new PXException(sb.ToString());
				}

			}
		}
	}
}
