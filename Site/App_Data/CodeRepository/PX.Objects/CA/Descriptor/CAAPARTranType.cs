using PX.Data;
using System;
using System.Collections.Generic;


namespace PX.Objects.CA
{
    public class CAAPARTranType
    {
		public class ListByModuleRestrictedAttribute : ListByModuleAttribute
		{
			public ListByModuleRestrictedAttribute(Type moduleField)
				: base(moduleField)
			{
				_AllowedValuesAR = new string[]{AR.ARInvoiceType.Invoice, 
                    AR.ARInvoiceType.CashSale, AR.ARInvoiceType.CreditMemo,
                    AR.ARInvoiceType.DebitMemo, AR.ARInvoiceType.Payment,
					AR.ARInvoiceType.Prepayment};
				_AllowedLabelsAR = new string[]{ AR.Messages.Invoice,
                    AR.Messages.CashSale, AR.Messages.CreditMemo,
                    AR.Messages.DebitMemo, AR.Messages.Payment,
					AR.Messages.Prepayment};

				_AllowedValuesAP = new string[]{AP.APInvoiceType.Invoice, 
                    AP.APInvoiceType.QuickCheck, AP.APInvoiceType.Check, 
                    AP.APInvoiceType.CreditAdj, AP.APInvoiceType.DebitAdj, 
                    AP.APInvoiceType.Prepayment};
				_AllowedLabelsAP = new string[]{AP.Messages.Invoice,
                    AP.Messages.QuickCheck, AP.Messages.Check,
                    AP.Messages.CreditAdj, AP.Messages.DebitAdj,
                    AP.Messages.Prepayment};

				_AllowedValuesCA = new string[] { CA.CATranType.CAAdjustment };
				_AllowedLabelsCA = new string[] { CA.Messages.CAAdjustment };

				_AllowedValuesOther = new string[] { CA.CATranType.CAAdjustment,
					CA.CAAPARTranType.GLEntry,
					
					AR.ARInvoiceType.Invoice, 
                    AR.ARInvoiceType.CashSale, AR.ARInvoiceType.CreditMemo,
                    AR.ARInvoiceType.DebitMemo, AR.ARInvoiceType.Payment,
					AR.ARInvoiceType.Prepayment,
				
				//	AP.APInvoiceType.Invoice, 
                    AP.APInvoiceType.QuickCheck, AP.APInvoiceType.Check, 
                    AP.APInvoiceType.CreditAdj, AP.APInvoiceType.DebitAdj, 
                  //  AP.APInvoiceType.Prepayment
				};

				_AllowedLabelsOther = new string[] { CA.Messages.CAAdjustment,
					CA.Messages.GLEntry,

					CA.Messages.ARAPInvoice, 
                    AR.Messages.CashSale, AR.Messages.CreditMemo,
                    AR.Messages.DebitMemo, AR.Messages.Payment,
					CA.Messages.ARAPPrepayment,

				//	AP.Messages.Invoice,
                    AP.Messages.QuickCheck, AP.Messages.Check,
                    AP.Messages.CreditAdj, AP.Messages.DebitAdj,
                  //  AP.Messages.Prepayment
				};
			}
		}
        public class ListByModuleAttribute : PXStringListAttribute
        {
            protected string[] _AllowedValuesAR ={ AR.ARDocType.Invoice, AR.ARDocType.CashSale, AR.ARDocType.CashReturn, 
			    AR.ARInvoiceType.Refund, AR.ARDocType.Prepayment, AR.ARDocType.VoidPayment, AR.ARDocType.CreditMemo, 
			    AR.ARInvoiceType.DebitMemo, AR.ARDocType.Payment, CA.CAAPARTranType.GLEntry};
            protected string[] _AllowedLabelsAR ={ AR.Messages.Invoice, AR.Messages.CashSale, AR.Messages.CashReturn,	
                AR.Messages.Refund, AR.Messages.Prepayment,	AR.Messages.VoidPayment, AR.Messages.CreditMemo, 
                AR.Messages.DebitMemo, AR.Messages.Payment, CA.Messages.GLEntry};

            protected string[] _AllowedValuesAP ={ AP.APDocType.Invoice, AP.APDocType.QuickCheck, AP.APDocType.VoidCheck, 
                AP.APDocType.VoidQuickCheck, AP.APDocType.Check, AP.APDocType.CreditAdj, AP.APDocType.Refund, 
			    AP.APDocType.DebitAdj, AP.APDocType.Prepayment, CA.CAAPARTranType.GLEntry, CA.CATranType.CABatch};
            protected string[] _AllowedLabelsAP ={ AP.Messages.Invoice, AP.Messages.QuickCheck, AP.Messages.VoidCheck, 
                AP.Messages.VoidQuickCheck, AP.Messages.Check, AP.Messages.CreditAdj, AP.Messages.Refund, 
                AP.Messages.DebitAdj, AP.Messages.Prepayment, CA.Messages.GLEntry, AP.Messages.APBatch};

            protected string[] _AllowedValuesCA ={
                CA.CATranType.CAAdjustment, 
                CA.CAAPARTranType.CATransfer, CA.CATranType.CATransferIn, CA.CATranType.CATransferOut, CA.CATranType.CATransferExp,
                CA.CATranType.CADeposit, CA.CATranType.CAVoidDeposit, CA.CAAPARTranType.GLEntry};
            protected string[] _AllowedLabelsCA ={
                CA.Messages.CAAdjustment, 
                CA.Messages.CATransfer, CA.Messages.CATransferIn, CA.Messages.CATransferOut, CA.Messages.CATransferExp,
                CA.Messages.CADeposit, CA.Messages.CAVoidDeposit, CA.Messages.GLEntry};

            protected string[] _AllowedValuesGL = { CA.CAAPARTranType.GLEntry };
            protected string[] _AllowedLabelsGL = { CA.Messages.GLEntry };

            protected string[] _AllowedValuesOther ={
                CA.CATranType.CAAdjustment, 
                CA.CAAPARTranType.CATransfer, CA.CATranType.CATransferIn, CA.CATranType.CATransferOut, CA.CATranType.CATransferExp,
                CA.CATranType.CADeposit, CA.CATranType.CAVoidDeposit, CA.CATranType.CABatch,
                
                CA.CAAPARTranType.GLEntry,

                AR.ARDocType.Invoice, AR.ARDocType.Refund, AR.ARDocType.Prepayment,
             // AP.APDocType.Invoice, AP.APDocType.Refund, AP.APDocType.Prepayment,
                
			    AR.ARDocType.Payment, AR.ARDocType.VoidPayment,
                AR.ARDocType.CashSale, AR.ARDocType.CashReturn, 
                AR.ARDocType.CreditMemo, AR.ARDocType.DebitMemo, 

                AP.APDocType.Check, AP.APDocType.VoidCheck, 
                AP.APDocType.QuickCheck, AP.APDocType.VoidQuickCheck, 
                AP.APDocType.CreditAdj, AP.APDocType.DebitAdj };

            protected string[] _AllowedLabelsOther ={
                CA.Messages.CAAdjustment, 
                CA.Messages.CATransfer, CA.Messages.CATransferIn, CA.Messages.CATransferOut, CA.Messages.CATransferExp,
                CA.Messages.CADeposit, CA.Messages.CAVoidDeposit, AP.Messages.APBatch, 
                
                CA.Messages.GLEntry,

                CA.Messages.ARAPInvoice, CA.Messages.ARAPRefund, CA.Messages.ARAPPrepayment,

               	AR.Messages.Payment, AR.Messages.VoidPayment, 
                AR.Messages.CashSale, AR.Messages.CashReturn,	
                AR.Messages.CreditMemo, AR.Messages.DebitMemo, 

                AP.Messages.Check, AP.Messages.VoidCheck, 
                AP.Messages.QuickCheck, AP.Messages.VoidQuickCheck,  
                AP.Messages.CreditAdj, AP.Messages.DebitAdj};

            protected Type _ModuleField;
			protected string lastModule;
            public ListByModuleAttribute(Type moduleField)
                : base(new string[] { }, new string[] { })
            {
                _ModuleField = moduleField;
            }
            public override void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
            {
                string module = (string)sender.GetValue(e.Row, _ModuleField.Name);
				if (module != lastModule || module == null)
				{
					switch (module)
					{
						case GL.BatchModule.AP:
							SetNewList(sender, _AllowedValuesAP, _AllowedLabelsAP);
							break;
						case GL.BatchModule.AR:
							SetNewList(sender, _AllowedValuesAR, _AllowedLabelsAR);
							break;
						case GL.BatchModule.CA:
							SetNewList(sender, _AllowedValuesCA, _AllowedLabelsCA);
							break;
						case GL.BatchModule.GL:
							SetNewList(sender, _AllowedValuesGL, _AllowedLabelsGL);
							break;
						default:
							SetNewList(sender, _AllowedValuesOther, _AllowedLabelsOther);
							break;
					}
				}
				lastModule = module;
                if (sender.Graph.GetType()==typeof(PXGraph))//report mode
                {
                    for (int i = 0; i < _AllowedValues.Length; i++)
                    {
                        if (((string)e.ReturnValue) == _AllowedValues[i])
                        {
                            e.ReturnValue = _AllowedLabels[i];
                            return;
                        }
                    }
                }
                else
                {
                    base.FieldSelecting(sender, e);
                }
            }

			protected void SetNewList(PXCache sender, string[] values, string[] labels)
			{
				List<ListByModuleAttribute> list = new List<ListByModuleAttribute>();
				list.Add(this);
				string[] allowedValuesNew = new string[values.Length];
				string[] allowedLabelsNew = new string[labels.Length];
				values.CopyTo(allowedValuesNew, 0);
				labels.CopyTo(allowedLabelsNew, 0);
				SetListInternal(list, allowedValuesNew, allowedLabelsNew, sender);
			}

			protected override void TryLocalize(PXCache sender)
			{
				base.TryLocalize(sender);
				base.RipDynamicLabels(_AllowedLabelsAP, sender);
				base.RipDynamicLabels(_AllowedLabelsAR, sender);
				base.RipDynamicLabels(_AllowedLabelsCA, sender);
				base.RipDynamicLabels(_AllowedLabelsGL, sender);
			}

		}

        public const string GLEntry = "GLE";
        public const string CATransfer = "CT%";
        public const string CATransferOut = "CTO";
        public const string CATransferIn = "CTI";
        public const string CATransferExp = "CTE";
        public const string CAAdjustment = "CAE";

        public const string CADeposit = "CDT";
        public const string CAVoidDeposit = "CVD";

        public class cATransfer : Constant<string>
        {
            public cATransfer() : base(CATransfer) { ;}
        }

        public class cATransferOut : Constant<string>
        {
            public cATransferOut() : base(CATransferOut) { ;}
        }

        public class cATransferIn : Constant<string>
        {
            public cATransferIn() : base(CATransferIn) { ;}
        }

        public class cATransferExp : Constant<string>
        {
            public cATransferExp() : base(CATransferExp) { ;}
        }

        public class cAAdjustment : Constant<string>
        {
            public cAAdjustment() : base(CAAdjustment) { ;}
        }

        public class gLEntry : Constant<string>
        {
            public gLEntry() : base(GLEntry) { ;}
        }

	    public static bool IsTransfer(string tranType)
	    {
		    return tranType == CATransferOut || tranType == CATransferIn;
	    }
    }
}
