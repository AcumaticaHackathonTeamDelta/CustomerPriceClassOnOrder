using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.CS;

namespace PX.Objects.IN
{
    [Serializable]
	public class INPIEntry : PXGraph<INPIEntry>, PXImportAttribute.IPXPrepareItems, PXImportAttribute.IPXProcess
	{
		#region Menu
		public PXSave<INPIHeader> Save;
		public PXCancel<INPIHeader> Cancel;
		public PXAction<INPIHeader> Insert;
		public PXCopyPasteAction<INPIHeader> CopyPaste;
		public PXDelete<INPIHeader> Delete;
		public PXFirst<INPIHeader> First;
		public PXPrevious<INPIHeader> Previous;
		public PXNext<INPIHeader> Next;
		public PXLast<INPIHeader> Last;
		#endregion

		public PXSelect<INPIHeader> PIHeader;
		[PXFilterable]
		[PXImport(typeof(INPIHeader))]
		public PXSelectJoin<INPIDetail, 
		 LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<INPIDetail.inventoryID>>, 
				LeftJoin<INSubItem, On<INSubItem.subItemID, Equal<INPIDetail.subItemID>>>>,	
		 Where<INPIDetail.pIID, Equal<Optional<INPIHeader.pIID>>,
		 And<Where<INPIDetail.inventoryID, IsNull, Or<InventoryItem.inventoryID, IsNotNull>>>>, 
			OrderBy<Asc<INPIDetail.lineNbr>>> PIDetail;
		public PXSelect<INPIHeader, Where<INPIHeader.pIID, Equal<Current<INPIHeader.pIID>>>> PIHeaderInfo;
		public PXSelect<INPIStatus> PIStatus;
		public PXSelect<INSetup> INSetup;
		public PXSetup<INSite, Where<INSite.siteID, Equal<Current<INPIHeader.siteID>>>> insite;
		public PXFilter<PIGeneratorSettings> GeneratorSettings;
		public INBarCodeItemLookup<INBarCodeItem> AddByBarCode;
		public PXSetup<INSetup> Setup; 
		

		public INPIEntry()
		{		
            //PXDimensionSelectorAttribute.SetValidCombo<INPIDetail.subItemID>(PIDetail.Cache, true);			
			PXDefaultAttribute.SetPersistingCheck<PIGeneratorSettings.randomItemsLimit>(GeneratorSettings.Cache, null, PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<PIGeneratorSettings.lastCountPeriod>(GeneratorSettings.Cache, null, PXPersistingCheck.Nothing);
		}

		[PXUIField(DisplayName = ActionsMessages.Insert, MapEnableRights = PXCacheRights.Insert, MapViewRights = PXCacheRights.Insert)]
		[PXInsertButton]
		protected virtual IEnumerable insert(PXAdapter adapter)
		{
			if(GeneratorSettings.AskExt((graph, name) => ResetGenerateSettings(graph)) == WebDialogResult.OK && GeneratorSettings.VerifyRequired())
			{
                PIGenerator generator = PXGraph.CreateInstance<PIGenerator>();
				generator.GeneratorSettings.Current = GeneratorSettings.Current;
				generator.CalcPIRows(true);
				if (generator.piheader.Current != null)
				{
					this.Clear();
					return PIHeader.Search<INPIHeader.pIID>(generator.piheader.Current.PIID);
				}
			}
			return adapter.Get();
		}

		private static void ResetGenerateSettings(PXGraph graph)
		{
			INPIEntry entry = graph as INPIEntry;
			if (entry == null) return;
			entry.GeneratorSettings.Cache.Clear();
			entry.GeneratorSettings.Insert(new PIGeneratorSettings());			
		}
		#region Events
		#region Add Line
		public PXAction<INPIHeader> addLine;
		[PXUIField(DisplayName = Messages.Add, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXLookupButton(Tooltip = Messages.AddNewLine)]
		public virtual IEnumerable AddLine(PXAdapter adapter)
		{
			if (AddByBarCode.AskExt(
				(graph, view) => ((INPIReview)graph).AddByBarCode.Reset(false)) == WebDialogResult.OK &&
				this.AddByBarCode.VerifyRequired())
				UpdatePhysicalQty();
			return adapter.Get();
		}

		public PXAction<INPIHeader> addLine2;
		[PXUIField(DisplayName = Messages.Add, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXLookupButton(Tooltip = Messages.AddNewLine)]
		public virtual IEnumerable AddLine2(PXAdapter adapter)
		{
			if (this.AddByBarCode.VerifyRequired())
			{
				UpdatePhysicalQty();
			}
			return adapter.Get();
		}

		protected virtual void INBarCodeItem_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			PXUIFieldAttribute.SetEnabled<INBarCodeItem.uOM>(this.AddByBarCode.Cache, null, false);
		}
		protected virtual void INBarCodeItem_ExpireDate_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			INPIDetail exists =
			PXSelectReadonly<INPIDetail,
				Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
					And<INPIDetail.inventoryID, Equal<Current<INBarCodeItem.inventoryID>>,
						And<INPIDetail.lotSerialNbr, Equal<Current<INBarCodeItem.lotSerialNbr>>>>>>.SelectWindowed(this, 0, 1);
			if (exists != null)
			{
				e.NewValue = exists.ExpireDate;
				e.Cancel = true;
			}
		}

        protected virtual void INBarCodeItem_SiteID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            if (this.PIHeader.Current != null)
                e.NewValue = this.PIHeader.Current.SiteID;
            e.Cancel = true;
        }

		protected virtual void INBarCodeItem_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			INBarCodeItem row = (INBarCodeItem)e.Row;
			if (row != null && row.AutoAddLine == true && this.AddByBarCode.VerifyRequired(true) && row.Qty > 0)
				UpdatePhysicalQty();
		}

		private void UpdatePhysicalQty()
		{
			INBarCodeItem item = AddByBarCode.Current;
			INPIHeader d = this.PIHeader.Current;

			this.SelectTimeStamp();

			INPIDetail detail =
			PXSelect<INPIDetail,
				Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
					And<INPIDetail.inventoryID, Equal<Current<INBarCodeItem.inventoryID>>,
						And<INPIDetail.subItemID, Equal<Current<INBarCodeItem.subItemID>>,
							And<INPIDetail.locationID, Equal<Current<INBarCodeItem.locationID>>,
								And<Where<INPIDetail.lotSerialNbr, IsNull,
									Or<INPIDetail.lotSerialNbr, Equal<Current<INBarCodeItem.lotSerialNbr>>>>>>>>>>.SelectWindowed(this, 0, 1);
			if (detail == null)
			{
				detail = PXCache<INPIDetail>.CreateCopy(this.PIDetail.Insert(new INPIDetail()));
				detail.InventoryID = item.InventoryID;
				detail = PXCache<INPIDetail>.CreateCopy(this.PIDetail.Update(detail));
				detail.SubItemID = item.SubItemID;
				detail.LocationID = item.LocationID;
				detail.LotSerialNbr = item.LotSerialNbr;
				detail.PhysicalQty = item.Qty;
				detail.ExpireDate = item.ExpireDate;
				this.PIDetail.Update(detail);					
			}
			else
			{
				detail = PXCache<INPIDetail>.CreateCopy(detail);
				detail.PhysicalQty = detail.PhysicalQty.GetValueOrDefault() + item.Qty.GetValueOrDefault();
				this.PIDetail.Update(detail);
			}

			item.Description = PXMessages.LocalizeFormatNoPrefixNLA(Messages.PILineUpdated,
																			 AddByBarCode.GetValueExt<INBarCodeItem.inventoryID>(item).ToString().Trim(),
																			 Setup.Current.UseInventorySubItem == true ? ":" + AddByBarCode.GetValueExt<INBarCodeItem.subItemID>(item) : string.Empty,
																			 AddByBarCode.GetValueExt<INBarCodeItem.qty>(item),
																			 item.UOM,
																			 detail.LineNbr);			
			AddByBarCode.Reset(true);
			this.AddByBarCode.View.RequestRefresh();
		}

		#endregion
		#region _RowSelected events
		protected virtual void INPIHeader_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (this.IsContractBasedAPI)
            {
                return;
            }

			if (e.Row == null) { return; }
			INPIHeader h = (INPIHeader)e.Row;

			PIHeader.Cache.AllowDelete = h.Status != INPIHdrStatus.Completed;

			PIHeader.Cache.AllowUpdate =		
			PIDetail.Cache.AllowInsert = 
			PIDetail.Cache.AllowDelete =
			PIDetail.Cache.AllowUpdate = (h.Status == INPIHdrStatus.Counting) || (h.Status == INPIHdrStatus.Entering);

            addLine.SetEnabled(PIDetail.Cache.AllowUpdate);
        }

		protected virtual void INPIHeader_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			INPIHeader h = (INPIHeader)e.Row;
			if (h == null) { return; }
				UnlockInventory(true);
		}

		protected virtual void INPIDetail_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null)
			{
				return;
			}
			
			INPIDetail d = (INPIDetail)e.Row;
			INLotSerClass lotSer = SelectLotSerClass(d.InventoryID);
			bool notNormal = d.LineType != INPIDetLineType.Normal;
			bool notNormalSerial = notNormal & LSRequired(lotSer);
			bool requestExpireDate = notNormalSerial && lotSer.LotSerTrackExpiration == true; 
			PXUIFieldAttribute.SetEnabled<INPIDetail.inventoryID>(sender, d, notNormal);
			PXUIFieldAttribute.SetEnabled<INPIDetail.subItemID>(sender, d, notNormal);
			PXUIFieldAttribute.SetEnabled<INPIDetail.locationID>(sender, d, notNormal);
			PXUIFieldAttribute.SetEnabled<INPIDetail.lotSerialNbr>(sender, d, notNormalSerial);
			PXUIFieldAttribute.SetEnabled<INPIDetail.expireDate>(sender, d, requestExpireDate);
	
		
		}

		protected virtual void INPIDetail_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
		{
			INPIDetail d = (INPIDetail)e.Row;
			if(d.LineType != INPIDetLineType.UserEntered && e.ExternalCall)
				throw new PXException(Messages.PILineDeleted);
		}

		private static void CheckDefault<Field>(PXCache sender, INPIDetail row)
			where Field : IBqlField
		{
			string fieldname = typeof (Field).Name;
			if(sender.GetValue<Field>(row) == null)
				if (sender.RaiseExceptionHandling(fieldname, row, null,
					new PXSetPropertyException(PXMessages.LocalizeFormat(ErrorMessages.FieldIsEmpty, fieldname))))
					throw new PXRowPersistingException(fieldname, null, ErrorMessages.FieldIsEmpty, fieldname);			
		}

		#endregion RowSelected events




		#region _FieldVerifying events

		protected virtual void INPIDetail_InventoryID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (e.NewValue == null || e.Row == null) return;

			INPIDetail d = (INPIDetail)e.Row;
			ValidateInventory(this, (int?) e.NewValue, d.LocationID);						
			INLotSerClass inclass = SelectLotSerClass((int?)e.NewValue);			

			if (inclass != null && 
				d.PhysicalQty != null &&
				LSRequired(inclass) &&
				inclass.LotSerTrack == INLotSerTrack.SerialNumbered &&
				 d.PhysicalQty != 0m && d.PhysicalQty != 1m)
				throw new PXSetPropertyException(Messages.PIPhysicalQty);
			
		}		

		protected virtual void INPIDetail_SubItemID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((e.NewValue as int?) == null) || (e.Row == null))
			{
				return;
			}
			INPIDetail d = (INPIDetail)e.Row;
			ValidateDuplicate(d.Status, d.InventoryID, (int?) e.NewValue, d.LocationID, d.LotSerialNbr, d.LineNbr);
		}
		
		protected virtual void INPIDetail_LocationID_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((e.NewValue as int?) == null) || (e.Row == null))
			{
				return;
			}
			INPIDetail d = (INPIDetail)e.Row;

			ValidateInventory(this, d.InventoryID, (int?) e.NewValue);			
			ValidateDuplicate(d.Status, d.InventoryID, d.SubItemID, (int?) e.NewValue, d.LotSerialNbr, d.LineNbr);			
		}

		protected virtual void INPIDetail_LotSerialNbr_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (((e.NewValue as string) == null) || (e.Row == null))
			{
				return;
			}
			INPIDetail d = (INPIDetail)e.Row;

			ValidateDuplicate(d.Status, d.InventoryID, d.SubItemID, d.LocationID, (string) e.NewValue, d.LineNbr);			
		}
		protected virtual void INPIDetail_PhysicalQty_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if(e.NewValue == null) return;

			decimal value = (decimal) e.NewValue;
			INPIDetail d = (INPIDetail)e.Row;
			INLotSerClass inclass = SelectLotSerClass(d.InventoryID);
			if ((decimal?)e.NewValue < 0m)			
				throw new PXSetPropertyException(CS.Messages.Entry_GE, PXErrorLevel.Error, (int)0);			
		
			if (inclass != null &&
				LSRequired(inclass) && 
				inclass.LotSerTrack == INLotSerTrack.SerialNumbered &&				
				 value != 0m && value != 1m)
				throw new PXSetPropertyException(Messages.PIPhysicalQty);

		}

		#endregion _FieldVerifying events

		
		#region misc

		public virtual void UnlockInventory(bool deleteLock)
		{
			PIStatus.Cache.Clear();			
			foreach (PXResult<INPIStatus> it in PXSelect<INPIStatus,
							 Where<INPIStatus.pIID, Equal<Current<INPIHeader.pIID>>>>.Select(this))
			{								
				if(deleteLock)
					PIStatus.Delete(it);
				else
				{
					INPIStatus upd = PXCache<INPIStatus>.CreateCopy(it);
					upd.Active = false;
					PIStatus.Update(upd);
				}
			}
		}

		#endregion misc


		protected virtual void INPIDetail_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			INPIDetail row = e.Row as INPIDetail;
			if (e.Operation == PXDBOperation.Insert || e.Operation == PXDBOperation.Update)
			{
				INPIDetail d = (INPIDetail)e.Row;

				if (d.LineType != INPIDetLineType.Normal && d.Status != INPIDetStatus.Skipped)
				{
					CheckDefault<INPIDetail.inventoryID>(sender, d);
					CheckDefault<INPIDetail.subItemID>(sender, d);
					CheckDefault<INPIDetail.locationID>(sender, d);
					INLotSerClass lotSer = SelectLotSerClass(d.InventoryID);
					if (LSRequired(lotSer))
					{
						CheckDefault<INPIDetail.lotSerialNbr>(sender, d);
						if (lotSer.LotSerTrackExpiration == true)
							CheckDefault<INPIDetail.expireDate>(sender, d);
					}
				}
			}
			if (row != null && sender.GetStatus(row) == PXEntryStatus.Inserted)
			{
				INSetup setup = INSetup.Select();

				if (setup.PIUseTags == true)
				{
					setup.PILastTagNumber++;
					row.TagNumber = setup.PILastTagNumber;
					INSetup.Update(setup);
				}
			}
		}
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Type ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INPIClass.pIClassID, Where<INPIClass.method, Equal<PIMethod.fullPhysicalInventory>>>))]
		[PXDefault()]
		protected virtual void PIGeneratorSettings_PIClassID_CacheAttached(PXCache sender)
		{			
		}

		protected virtual void PIGeneratorSettings_PIClassID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PIGeneratorSettings row = (PIGeneratorSettings)e.Row;
			if (row == null) { return; }
			INPIClass pi = (INPIClass)PXSelectorAttribute.Select<PIGeneratorSettings.pIClassID>(sender, row);
			if (pi != null)
			{
				PXCache source = this.Caches[typeof(INPIClass)];
				foreach (string field in sender.Fields)
				{
					if (string.Compare(field, typeof(PIGeneratorSettings.pIClassID).Name, true) != 0 &&
						source.Fields.Contains(field))
						sender.SetValuePending(row, field, source.GetValueExt(pi, field));
				}

			}
		}
		
		#region _FieldUpdated events

		protected virtual void INPIDetail_InventoryID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row == null) { return; }

			INPIDetail d = (INPIDetail)e.Row;			
			INPIHeader h = (INPIHeader)PIHeader.Cache.Current;
			if (h == null) { return; }

			sender.SetValue<INPIDetail.subItemID>(d, null);
			sender.SetValue<INPIDetail.locationID>(d, null);
			sender.SetValue<INPIDetail.lotSerialNbr>(d, null);
			sender.SetValue<INPIDetail.physicalQty>(d, null);
			sender.SetValue<INPIDetail.varQty>(d, null);
			sender.SetDefaultExt<INPIDetail.unitCost>(d);
			sender.SetValue<INPIDetail.extVarCost>(d, null);
			sender.SetValue<INPIDetail.status>(d, INPIDetStatus.NotEntered);

			RecalcTotals();

						
		}


		protected virtual void INPIDetail_SubItemID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row == null) { return; }

			INPIDetail d = (INPIDetail)e.Row;
			int? old_SubItemID = (e.OldValue as int?);

			INPIHeader h = (INPIHeader)PIHeader.Cache.Current;
			if (h == null) { return; }

			sender.SetValue<INPIDetail.physicalQty>(d, null);
			sender.SetValue<INPIDetail.varQty>(d, null);
			sender.SetValue<INPIDetail.extVarCost>(d, null);
			sender.SetValue<INPIDetail.status>(d, INPIDetStatus.NotEntered);

			RecalcTotals();			
		}
		protected virtual void INPIDetail_UnitCost_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
		{
			INItemStats stats = 
			PXSelect<INItemStats,
				Where<INItemStats.inventoryID, Equal<Current<INPIDetail.inventoryID>>,
					And<INItemStats.siteID, Equal<Current<INPIHeader.siteID>>>>>.SelectSingleBound(this,
					                                                                               new object[]
					                                                                               	{e.Row, this.PIHeader.Current});

			e.NewValue = stats != null ? stats.LastCost : 0;
		}


		protected virtual void INPIDetail_LocationID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row == null) { return; }

			INPIDetail d = (INPIDetail)e.Row;
			int? old_LocationID = (e.OldValue as int?);

			INPIHeader h = (INPIHeader)PIHeader.Cache.Current;
			if (h == null) { return; }
		
			RecalcTotals();			
		}

		protected virtual void INPIDetail_LotSerialNbr_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row == null) { return; }

			INPIDetail d = (INPIDetail)e.Row;
			string old_LotSerialNbr = (e.OldValue as string);

			INPIHeader h = (INPIHeader)PIHeader.Cache.Current;
			if (h == null) { return; }


			RecalcTotals();			
		}		



		protected virtual void INPIDetail_PhysicalQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			INPIDetail d = e.Row as INPIDetail;
			if (d == null) return;			

			sender.SetValue<INPIDetail.varQty>(d, d.PhysicalQty - d.BookQty);
			sender.SetValue<INPIDetail.extVarCost>(d, 
				PXDBCurrencyAttribute.BaseRound(this,(d.UnitCost * (d.PhysicalQty - d.BookQty))??0m));			
			sender.SetValue<INPIDetail.status>(d, d.PhysicalQty != null
				? INPIDetStatus.Entered
				: INPIDetStatus.NotEntered);			
			RecalcTotals();			
		}

		#endregion _FieldUpdated events

		#endregion Events

		#region misc.

		private void ValidateDuplicate(string status, int? inventoryID, int? subItemID, int? locationID, string lotSerialNbr, int? lineNbr)
		{			
			if(inventoryID == null || subItemID == null || locationID == null) return;
			
			foreach (INPIDetail it in 
					PXSelect<INPIDetail, 
					Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
						And<INPIDetail.inventoryID, Equal<Required<INPIDetail.inventoryID>>, 
						And<INPIDetail.subItemID, Equal<Required<INPIDetail.subItemID>>, 
						And<INPIDetail.locationID, Equal<Required<INPIDetail.locationID>>,
						And<INPIDetail.lineNbr, NotEqual<Required<INPIDetail.lineNbr>>>>>>>>
					.Select(this, inventoryID, subItemID, locationID, lineNbr))
			{
				if ((it.LotSerialNbr ?? "").Trim() == (lotSerialNbr ?? "").Trim())
					throw new PXSetPropertyException(Messages.ThisCombinationIsUsedAlready, it.LineNbr);
			}
			if (string.IsNullOrEmpty(lotSerialNbr)) return;

			INLotSerClass lsc_rec = SelectLotSerClass(inventoryID);
			if(lsc_rec.LotSerTrack == INLotSerTrack.SerialNumbered &&
				 lsc_rec.LotSerAssign == INLotSerAssign.WhenReceived)
			{
				INPIDetail serialDuplicate =
					PXSelect<INPIDetail,
						Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
							And<INPIDetail.inventoryID, Equal<Required<INPIDetail.inventoryID>>,
							And<INPIDetail.lotSerialNbr, Equal<Required<INPIDetail.lotSerialNbr>>,
							And<INPIDetail.lineType, Equal<INPIDetLineType.userEntered>,
							And<INPIDetail.lineNbr, NotEqual<Required<INPIDetail.lineNbr>>>>>>>>
						.Select(this, inventoryID, lotSerialNbr, lineNbr);
				if (serialDuplicate != null)
					throw new PXSetPropertyException(Messages.ThisSerialNumberIsUsedAlready, serialDuplicate.LineNbr);

				if(status == INPIDetStatus.NotEntered)
				{
					INLotSerialStatus lotstatus =
						PXSelect<INLotSerialStatus,
							Where<INLotSerialStatus.siteID, Equal<Current<INPIHeader.siteID>>,
								And<INLotSerialStatus.inventoryID, Equal<Required<INLotSerialStatus.inventoryID>>,
									And<INLotSerialStatus.lotSerialNbr, Equal<Required<INLotSerialStatus.lotSerialNbr>>,
									And<INLotSerialStatus.qtyOnHand, Greater<decimal0>>>>>>
							.SelectWindowed(this, 0, 1, inventoryID, lotSerialNbr);

					if (lotstatus != null)
					{
						INPIDetail locationPI =
						PXSelect<INPIDetail,
							Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
								And<INPIDetail.inventoryID, Equal<Required<INPIDetail.inventoryID>>,								
									And<INPIDetail.locationID, Equal<Required<INPIDetail.locationID>>,
									And<INPIDetail.lotSerialNbr, Equal<Required<INPIDetail.lotSerialNbr>>,
									And<INPIDetail.lineNbr, NotEqual<Required<INPIDetail.lineNbr>>>>>>>>
							.Select(this, inventoryID, lotstatus.LocationID, lotSerialNbr, lineNbr);		
						if(locationPI == null)								
							throw new PXSetPropertyException(Messages.ThisSerialNumberIsUsedInItem);						
					}
				}
			}			
		}

		public static void ValidateInventory(PXGraph graph, int? inventoryID, int? locationID)
		{
			if(inventoryID == null || locationID == null) return;

			INPIStatus rec = 
			PXSelect<INPIStatus,
					Where<INPIStatus.siteID, Equal<Current<INPIHeader.siteID>>,
						And<INPIStatus.pIID, Equal<Current<INPIHeader.pIID>>,
					 And2<Where<INPIStatus.inventoryID, IsNull,
									 Or<INPIStatus.inventoryID, Equal<Required<INPIStatus.inventoryID>>>>,
						And<Where<INPIStatus.locationID, IsNull,
									 Or<INPIStatus.locationID, Equal<Required<INPIStatus.locationID>>>>>>>>>
				.SelectWindowed(graph, 0, 1, inventoryID, locationID);
			if (rec == null)
				throw new PXSetPropertyException(Messages.InventoryShouldBeUsedInCurrentPI);
		}


		protected virtual bool LSRequired(int? p_InventoryID)
		{
			return LSRequired(SelectLotSerClass(p_InventoryID));			
		}

		protected virtual bool LSRequired(INLotSerClass lsc_rec)
		{
			return lsc_rec == null ? false :
				 lsc_rec.LotSerTrack != INLotSerTrack.NotNumbered &&
				 lsc_rec.LotSerAssign == INLotSerAssign.WhenReceived;			
		}

		protected virtual INLotSerClass SelectLotSerClass(int? p_InventoryID)
		{
			if (p_InventoryID == null) return null;
			
			InventoryItem ii_rec = PXSelect<InventoryItem, Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, p_InventoryID);
			return ii_rec.LotSerClassID == null
			       	? null
			       	: PXSelect<INLotSerClass, 
								Where<INLotSerClass.lotSerClassID, Equal<Required<INLotSerClass.lotSerClassID>>>>
								.Select(this, ii_rec.LotSerClassID);
		}


		#region Recalc Cost methods

		//	cost recalculation algorythm briefly :
		//	given PI detail line ; 
		//	1. get [cost layers set] for this line
		//	2. get set of the pi detail lines which pertain to the same [cost layers set]
		//	3. for this set of lines do :
		//		3.1	set Costs to null
		//		3.2	for all the positive VarQty lines : add (virtually , counting it in CostStatusSupplInfoRec and ProjectedAdjustmentRec) ) their VarQty to the last record in [cost layers set] , using UnitCost of this last line in [cost layers set]
		//			3.2.1	.. and posting unit/ext cost to the affected lines
		//		3.3	(after 3.2 !) for the negative VarQty lines : subtract their VarQty from the records in [cost layers set] starting from the last record 
		//			3.3.1	.. and posting unit/ext cost to the affected lines
		
		protected void RecalcCostsForCostEntity
			// recalcs prospective Unit/Ext Cost in PIDetail cache for the cost-entity determined by p_d parameter (if not null)
		(
			INPIDetail p_d,
			List<ProjectedTranRec> p_projectedTrans
		)
		{
			INPIHeader h = (INPIHeader)PIHeader.Cache.Current;
			if (h == null) { return; }
			RecalcCostsForCostEntity(p_d.InventoryID, p_d.SubItemID, h.SiteID, p_d.LocationID, p_d.LotSerialNbr, p_d, p_projectedTrans);
		}

	
		protected virtual void RecalcCostsForCostEntity
		(
			int? p_InventoryID,
			int? p_SubItemID,
			int? p_SiteID,
			int? p_LocationID,
			string p_LotSerialNbr
			, INPIDetail p_d // to let update this only line with nulls if cost layers are not found
			, List<ProjectedTranRec> p_projectedTrans   //  (filled if not null)
		)
		{

			if (p_InventoryID == null) { return; }
			if (p_SubItemID == null) { return; }
			if (p_SiteID == null) { return; }
			if (p_LocationID == null) { return; }

            InventoryItem ii_rec = PXSelect<InventoryItem, 
                Where<InventoryItem.inventoryID, Equal<Required<InventoryItem.inventoryID>>>>.Select(this, p_InventoryID);
			
			bool ls_required = LSRequired(p_InventoryID);
			if (ls_required && ((p_LotSerialNbr ?? "").Trim() == "")) { return; }

			// 1. get [cost layers set] for this line
			// (positive lines only)

			PXResultset<INCostStatus> cost_layers =  // cost layers
				PXSelectJoin<INCostStatus,
					InnerJoin<INCostSubItemXRef,
						On<INCostSubItemXRef.costSubItemID, Equal<INCostStatus.costSubItemID>>,
					CrossJoin<INLocation>>, // see WHERE
				Where<INCostStatus.inventoryID, Equal<Required<INCostStatus.inventoryID>>,

					And<INCostStatus.qtyOnHand,GreaterEqual<decimal0>, 
					//And<INCostStatus.qtyOnHand, Greater<decimal0>,
				// ignore OVERSOLD and zero layers ; 
				// in case there are no other layers - receipt will be done instead of adjustment

					And<INCostSubItemXRef.subItemID, Equal<Required<INCostSubItemXRef.subItemID>>,

					And<INLocation.locationID, Equal<Required<INLocation.locationID>>,
				// INLocation was cross-joined

					And2<
						Where2<
									Where2<
												Where<
														INCostStatus.valMethod, Equal<INValMethod.standard>, 
													Or<INCostStatus.valMethod, Equal<INValMethod.specific>,
													Or<INLocation.isCosted, Equal<boolFalse>>>>,
											And<INCostStatus.costSiteID, Equal<Required<INCostStatus.costSiteID>>>>,  // siteID param should be for this
								Or<
									Where2<Not<
												Where<
														INCostStatus.valMethod, Equal<INValMethod.standard>, 
													Or<INCostStatus.valMethod, Equal<INValMethod.specific>,
													Or<INLocation.isCosted, Equal<boolFalse>>>>>,											
													And<INCostStatus.costSiteID, Equal<Required<INCostStatus.costSiteID>>>>>>,	// locationID param should be for this

					And<Where<INCostStatus.lotSerialNbr, Equal<Required<INCostStatus.lotSerialNbr>>,
						Or<INCostStatus.lotSerialNbr, IsNull,
						Or<INCostStatus.lotSerialNbr, Equal<BQLConstants.EmptyString>>>>>>>>>>,
				OrderBy<Asc<INCostStatus.receiptDate, Asc<INCostStatus.receiptNbr>>>>.Select
					(
						this,
						p_InventoryID,
						p_SubItemID,
						p_LocationID,
						p_SiteID,
						p_LocationID,
						p_LotSerialNbr
					);

			if (cost_layers.Count == 0)
			{
				// simple case
				if (p_d != null)
				{
					if ((((p_d.PhysicalQty - p_d.BookQty) ?? 0m) > 0m) && (p_d.Status != INPIDetStatus.Skipped))
					{
						// we'll do a receipt, not adjustment

						// get LastCost
						// from where Al saved it.. 
						PXResult<InventoryItem, INItemSite> is_res =
							(PXResult<InventoryItem, INItemSite>)
							PXSelectJoin<InventoryItem, 
								LeftJoin<INItemSite, 
								      On<INItemSite.inventoryID, Equal<InventoryItem.inventoryID>,
											And<INItemSite.siteID, Equal<Required<INItemSite.siteID>>>>>,
								Where<InventoryItem.inventoryID, Equal<Required<INItemSite.inventoryID>>>>
								.Select(this, p_SiteID, p_InventoryID);

						INItemSite is_rec = is_res;						
						p_d.UnitCost = is_rec != null ? is_rec.LastCost : 0m;
						p_d.ExtVarCost = PXDBCurrencyAttribute.BaseRound(this,
						                                                 (((p_d.PhysicalQty - p_d.BookQty) ?? 0m)*p_d.UnitCost ?? 0m));
						PIDetail.Update(p_d);
						if (p_projectedTrans != null)
						{
							ProjectedTranRec pt_rec = new ProjectedTranRec();

							pt_rec.AdjNotReceipt = false;

							pt_rec.LineNbr = p_d.LineNbr ?? 0;
							//pt_rec.ReceiptDate = Accessinfo.BusinessDate;
							//pt_rec.ReceiptNbr = p_d.PIID ?? "";
							//pt_rec.OrigRefNbr = //leave null
							pt_rec.UOM = ((InventoryItem) is_res).BaseUnit;
							pt_rec.VarQtyPortion = ((p_d.PhysicalQty - p_d.BookQty) ?? 0m);
							pt_rec.VarCostPortion = p_d.ExtVarCost.Value;

							// for receipts : left NULL ; acct & sub will be set from ItemSite during release
							pt_rec.AcctID = null;
							pt_rec.SubID = null;

							pt_rec.InventoryID = p_InventoryID;
							pt_rec.SubItemID = p_SubItemID;
							pt_rec.LocationID = p_LocationID;
							pt_rec.LotSerialNbr = p_LotSerialNbr;
                            pt_rec.ReasonCode = p_d.ReasonCode;
							pt_rec.ExpireDate = p_d.ExpireDate;

							p_projectedTrans.Add(pt_rec);
						}
					}
				}
				return;
			}

			int i = 0;
			CostStatusSupplInfoRec[] cost_layers_ext = new CostStatusSupplInfoRec[cost_layers.Count];  // adfditional cost info 
			foreach (INCostStatus it in cost_layers)
			{

				//cost_layers_ext[i].VarCost = 0m;
				//cost_layers_ext[i].VarQty = 0m;
				cost_layers_ext[i].ProjectedQty = (it.QtyOnHand ?? 0m);
				cost_layers_ext[i].ProjectedCost = (it.TotalCost ?? 0m);
				i++;
			}


			//	2. get set of the detail lines which pertain to the same [cost layers set]

			PXResultset<INPIDetail> affected_lines =
				PXSelectJoin<INPIDetail,  // it's critical to have INPIDetail 1st table in the join since we need cached record
					InnerJoin<INCostSubItemXRef,
						On<INCostSubItemXRef.subItemID, Equal<INPIDetail.subItemID>>,
			    LeftJoin<INLocation, On<INLocation.locationID, Equal<INPIDetail.locationID>,
						   And<INLocation.isCosted, Equal<boolFalse>>>>>,
					Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
						And<INPIDetail.inventoryID, Equal<Required<INPIDetail.inventoryID>>,
						And<INCostSubItemXRef.costSubItemID, Equal<Required<INCostSubItemXRef.costSubItemID>>,
						And2<Where<Current<INPIHeader.siteID>, Equal<Required<INPIHeader.siteID>>,
							    And<INLocation.locationID, IsNotNull,
							     Or<INPIDetail.locationID, Equal<Required<INPIDetail.locationID>>>>>,
						And<Where<INPIDetail.lotSerialNbr, Equal<Required<INPIDetail.lotSerialNbr>>,
							Or<Required<INPIDetail.lotSerialNbr>, IsNull,
							Or<Required<INPIDetail.lotSerialNbr>, Equal<BQLConstants.EmptyString>>>>>>>>>,
					OrderBy<Asc<INPIDetail.lineNbr>>>
						.Select
							(
								this,

								// params for the query ((we've checked above that cl is non-empty)
								((INCostStatus)cost_layers[0]).InventoryID,
								((INCostStatus)cost_layers[0]).CostSubItemID,
								((INCostStatus)cost_layers[0]).CostSiteID,
								((INCostStatus)cost_layers[0]).CostSiteID,
								((INCostStatus)cost_layers[0]).LotSerialNbr,
								((INCostStatus)cost_layers[0]).LotSerialNbr,
								((INCostStatus)cost_layers[0]).LotSerialNbr
							);

			// 3.1	set Costs to null :
			foreach (INPIDetail it in affected_lines)
			{
				it.UnitCost = null;
				it.ExtVarCost = null;
				PIDetail.Update(it);
			}

			//	3.2	for all the positive VarQty lines : add (virtually , counting it in CostStatusSupplInfoRec and ProjectedAdjustmentRec) ) their VarQty to the last record in [cost layers set] , using UnitCost of this last line in [cost layers set]
			foreach (INPIDetail it in affected_lines)
			{
				if ((((it.PhysicalQty - it.BookQty) ?? 0m) > 0m) && (it.Status != INPIDetStatus.Skipped))
				// ignoring negative and not set yet (PhysicalQty == null)
				// ; and skipped too
				{
					// cost_layers.Count == 0 situation was processed above
					INCostStatus last_cost_layer = cost_layers[cost_layers.Count - 1];

					// here and below : rounding is not applied to the Qtys simce we assume that Qtys are rounded properly
					//cost_layers_ext[cost_layers_ext.Length - 1].VarQty += ((it.PhysicalQty - it.BookQty) ?? 0m);
					//cost_layers_ext[cost_layers_ext.Length - 1].ProjectedQty = (last_cost_layer.QtyOnHand ?? 0m) + cost_layers_ext[cost_layers_ext.Length - 1].VarQty;
					cost_layers_ext[cost_layers_ext.Length - 1].ProjectedQty += ((it.PhysicalQty - it.BookQty) ?? 0m);

					decimal explicit_cl_unit_cost;
					
					if (last_cost_layer.QtyOnHand != 0m)
					{
						explicit_cl_unit_cost = (last_cost_layer.TotalCost ?? 0m) / (last_cost_layer.QtyOnHand ?? 0m);
					}
					else
					{
						PXResult<InventoryItem, INItemSite> is_res =
							(PXResult<InventoryItem, INItemSite>)
							PXSelectJoin<InventoryItem, 
								LeftJoin<INItemSite, 
								      On<INItemSite.inventoryID, Equal<InventoryItem.inventoryID>,
											And<INItemSite.siteID, Equal<Required<INItemSite.siteID>>>>>,
								Where<InventoryItem.inventoryID, Equal<Required<INItemSite.inventoryID>>>>
								.Select(this, p_SiteID, p_InventoryID);

						INItemSite is_rec = is_res;
						explicit_cl_unit_cost = is_rec != null ? (is_rec.LastCost ?? 0m) : 0m;

					}
					decimal cost_addition_raw = ((it.PhysicalQty - it.BookQty) ?? 0m) * explicit_cl_unit_cost;
					decimal cost_addition = PXDBCurrencyAttribute.BaseRound(this, cost_addition_raw);

					//cost_layers_ext[cost_layers_ext.Length - 1].VarCost += cost_addition;
					//cost_layers_ext[cost_layers_ext.Length - 1].ProjectedCost = (last_cost_layer.TotalCost ?? 0m) + cost_layers_ext[cost_layers_ext.Length - 1].VarCost;
					cost_layers_ext[cost_layers_ext.Length - 1].ProjectedCost += cost_addition;

					if (p_projectedTrans != null)
					{
						ProjectedTranRec pt_rec = new ProjectedTranRec();

						pt_rec.LineNbr = (int)it.LineNbr;						
						pt_rec.OrigRefNbr = last_cost_layer.ValMethod == INValMethod.FIFO ? last_cost_layer.ReceiptNbr : null;

						pt_rec.VarQtyPortion = ((it.PhysicalQty - it.BookQty) ?? 0m);
						pt_rec.VarCostPortion = cost_addition;

						pt_rec.AcctID = last_cost_layer.AccountID;
						pt_rec.SubID = last_cost_layer.SubID;

						pt_rec.AdjNotReceipt = true;
						pt_rec.InventoryID = it.InventoryID;
						pt_rec.SubItemID = it.SubItemID;
						pt_rec.LocationID = it.LocationID;
						pt_rec.LotSerialNbr = it.LotSerialNbr;
						pt_rec.ExpireDate = it.ExpireDate;
                        pt_rec.ReasonCode = p_d.ReasonCode;

						p_projectedTrans.Add(pt_rec);
					}

					// 3.2.1	.. and posting unit/ext cost to the affected lines
					it.UnitCost = Math.Round(cost_addition_raw/((it.PhysicalQty - it.BookQty) ?? 0m), 4, 
						MidpointRounding.AwayFromZero);
					// p_d.ExtBookCost = ... // abandoned column
					it.ExtVarCost = cost_addition;

					PIDetail.Update(it);

				}
			}

			//3.3	(after 3.2 !) for the negative VarQty lines : subtract their VarQty from the records in [cost layers set] starting from the last record 
			foreach (INPIDetail it in affected_lines)
			{
				if ( (((it.PhysicalQty - it.BookQty) ?? 0m) < 0m)&& (it.Status != INPIDetStatus.Skipped) )
				// ignoring positive and not set yet (PhysicalQty == null)
				// ; and skipped too
				{
					// distribute negative var qty over cost layers starting from the last and moving to the first
					decimal rest_of_qty_to_distribute = -((it.PhysicalQty - it.BookQty) ?? 0m);

					decimal total_line_var_cost_raw = 0m;
					decimal total_line_var_cost = 0m;

					while (rest_of_qty_to_distribute > 0m)
					{
						int layer_to_deduct_from_idx = 
                            ii_rec.ValMethod == INValMethod.FIFO ?
                            Array.FindIndex(cost_layers_ext, p => p.ProjectedQty > 0m):
                            Array.FindLastIndex(cost_layers_ext, p => p.ProjectedQty > 0m);

						if (layer_to_deduct_from_idx >= 0)
						{
							INCostStatus cost_layer_to_deduct_from = cost_layers[layer_to_deduct_from_idx];
							decimal qty_to_deduct_from_layer, cost_to_deduct_from_layer_raw, cost_to_deduct_from_layer;

							if (rest_of_qty_to_distribute >= cost_layers_ext[layer_to_deduct_from_idx].ProjectedQty)
							{
								qty_to_deduct_from_layer = cost_layers_ext[layer_to_deduct_from_idx].ProjectedQty;
								cost_to_deduct_from_layer_raw = cost_layers_ext[layer_to_deduct_from_idx].ProjectedCost;							
								cost_to_deduct_from_layer     = cost_layers_ext[layer_to_deduct_from_idx].ProjectedCost;
							}
							else
							{
								qty_to_deduct_from_layer = rest_of_qty_to_distribute;
								decimal explicit_cl_unit_cost = cost_layers_ext[layer_to_deduct_from_idx].ProjectedCost / cost_layers_ext[layer_to_deduct_from_idx].ProjectedQty;
								// we are sure here that ProjectedQty > 0 because ProjectedQty > rest_of_qty_to_distribute and rest_of_qty_to_distribute > 0
								cost_to_deduct_from_layer_raw = rest_of_qty_to_distribute*explicit_cl_unit_cost;
								cost_to_deduct_from_layer = PXDBCurrencyAttribute.BaseRound(this, cost_to_deduct_from_layer_raw);
							}

							//cost_layers_ext[layer_to_deduct_from_idx].VarQty -= qty_to_deduct_from_layer;
							cost_layers_ext[layer_to_deduct_from_idx].ProjectedQty -= qty_to_deduct_from_layer;

							cost_layers_ext[layer_to_deduct_from_idx].ProjectedCost -= cost_to_deduct_from_layer;

							rest_of_qty_to_distribute -= qty_to_deduct_from_layer;

							total_line_var_cost_raw -= cost_to_deduct_from_layer_raw;
							total_line_var_cost -= cost_to_deduct_from_layer;

							if (p_projectedTrans != null)
							{
								ProjectedTranRec pt_rec = new ProjectedTranRec();

								pt_rec.AdjNotReceipt = true;

								pt_rec.LineNbr = (int)it.LineNbr;
								pt_rec.OrigRefNbr = (cost_layer_to_deduct_from.ValMethod == INValMethod.FIFO) ? cost_layer_to_deduct_from.ReceiptNbr : null;								

								pt_rec.VarQtyPortion = -qty_to_deduct_from_layer;
								pt_rec.VarCostPortion = -cost_to_deduct_from_layer;

								pt_rec.AcctID = cost_layer_to_deduct_from.AccountID;
								pt_rec.SubID = cost_layer_to_deduct_from.SubID;

								pt_rec.InventoryID = it.InventoryID;
								pt_rec.SubItemID = it.SubItemID;
								pt_rec.LocationID = it.LocationID;
								pt_rec.LotSerialNbr = it.LotSerialNbr;
								pt_rec.ExpireDate = it.ExpireDate;
                                pt_rec.ReasonCode = p_d.ReasonCode;

								p_projectedTrans.Add(pt_rec);
							}
						}
						else
						{	
							
							PXCache cache = this.PIDetail.Cache;
							INSite site = PXSelect<INSite,
								Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.SelectWindowed(this, 0, 1, p_SiteID);

							throw new PXException(Messages.PICreateAbjustment,
								cache.GetValueExt<INPIDetail.lineNbr>(it),
								cache.GetValueExt<INPIDetail.inventoryID>(it),
								cache.GetValueExt<INPIDetail.subItemID>(it),
								site.SiteCD,
								cache.GetValueExt<INPIDetail.locationID>(it)); // impossible in correct database
						}
					}

					// 3.3.1	.. and posting unit/ext cost to the affected lines
					it.UnitCost = Math.Round(total_line_var_cost_raw/((it.PhysicalQty - it.BookQty) ?? 0m), 4,
					                         MidpointRounding.AwayFromZero);
					// p_d.ExtBookCost = ... // abandoned column
					it.ExtVarCost = total_line_var_cost;

					PIDetail.Update(it);
				}
			}

			//3.4	zero VarQty lines : just set Unit & ExtCost to zero to avoid repeat calculating for this lines
			foreach (INPIDetail it in affected_lines)
			{
				if ((((it.PhysicalQty - it.BookQty) ?? 0m) == 0m) || (it.Status == INPIDetStatus.Skipped))
				{
					it.UnitCost = 0m; 
					if(it.PhysicalQty != null)
						it.ExtVarCost = 0m;
					PIDetail.Update(it);
				}
			}
		}
		protected virtual void RecalcDemandCost()
		{
			RecalcDemandCost(null);
		}
		protected virtual void RecalcDemandCost(List<ProjectedTranRec> p_projectedTrans)
		{
			bool recalc = false;
			foreach (INPIDetail detail in
				PXSelect<INPIDetail,
				Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>,
					And<INPIDetail.varQty, IsNotNull,
					And<INPIDetail.varQty, NotEqual<decimal0>,
					And<INPIDetail.extVarCost, IsNull>>>>>.Select(this))
			{				
				INPIDetail rec = (INPIDetail)PIDetail.Cache.Locate(detail) ?? detail;
				if (rec.ExtVarCost == null)
				{
					RecalcCostsForCostEntity(detail, p_projectedTrans);
					recalc = true;
				}
			}
			if (recalc)
				RecalcTotals();
		}

		#endregion Recalc Cost methods


	    private bool skipRecalcTotals = false;
		protected virtual void RecalcTotals()
		{
			if (skipRecalcTotals) return;

			decimal total_var_qty = 0m, total_var_cost = 0m, total_phys_cost = 0m;
			//  manually , not via PXFormula because of the problems in INPIReview with double-counting during cost recalculation called from FieldUpdated event

			foreach (INPIDetail d in PIDetail.Select())
			{
				if ((d != null) && (d.Status != INPIDetStatus.Skipped))
				{
					total_phys_cost += d.PhysicalQty ?? 0m;
					total_var_qty += d.VarQty ?? 0m;
					total_var_cost += d.ExtVarCost ?? 0m;
				}
			}

			INPIHeader h = (INPIHeader)PIHeader.Cache.Current;
			if (h != null)
			{
				h.TotalPhysicalQty = total_phys_cost;
				h.TotalVarQty = total_var_qty;
				h.TotalVarCost = total_var_cost;
				PIHeader.Update(h);
			}
		}


		#endregion misc.

		#region InventoryByPIIDAnyStatus projection
		// projection to get list of items for PI ID
		// similar to InventoryByPIID but without status condition
		/*
			SELECT 
				h.PIID,	
				d.InventoryID
			FROM
						INPIHeader h
				JOIN	INPIDetail d ON d.PIID = h.PIID
			GROUP BY 
				h.PIID,	 
				d.InventoryID	  
		*/

		[PXProjection(typeof(Select5<INPIHeader,
			InnerJoin<INPIDetail,
				On<INPIDetail.pIID, Equal<INPIHeader.pIID>>>,
			Aggregate<
				GroupBy<INPIHeader.pIID,
				GroupBy<INPIDetail.inventoryID>>>>))]
        [Serializable]
        [PXHidden]
		public partial class InventoryByPIIDAnyStatus : PX.Data.IBqlTable
		{
			#region PIID
			public abstract class pIID : PX.Data.IBqlField
			{
			}
			protected String _PIID;
			[PXDBString(15, IsUnicode = true, BqlField = typeof(INPIHeader.pIID))]
			public virtual String PIID
			{
				get
				{
					return this._PIID;
				}
				set
				{
					this._PIID = value;
				}
			}
			#endregion
			#region InventoryID
			public abstract class inventoryID : PX.Data.IBqlField
			{
			}
			protected Int32? _InventoryID;
			[Inventory(BqlField = typeof(INPIDetail.inventoryID))]
			public virtual Int32? InventoryID
			{
				get
				{
					return this._InventoryID;
				}
				set
				{
					this._InventoryID = value;
				}
			}
			#endregion
		}

		#endregion

		#region Couple Of ..

		protected struct CostStatusSupplInfoRec
		{
			public decimal ProjectedQty; // OnHand + Var
			public decimal ProjectedCost; // OnHand + Var
		}


		protected class ProjectedTranRec
		{
			public bool AdjNotReceipt;
			public int LineNbr;
			public string UOM;
			public decimal VarQtyPortion;
			public decimal VarCostPortion;

			// acct & sub from cost layer
			public int? AcctID;
			public int? SubID;

			public int? InventoryID;
			public int? SubItemID;
			public int? LocationID;
			public string LotSerialNbr;
			public DateTime? ExpireDate;

			public string OrigRefNbr;
            public string ReasonCode;
		}

		#endregion Couple Of ..


		#region import function
		public int excelRowNumber = 2;
		public bool importHasError = false;

		private object GetImportedValue<Field>(IDictionary values, bool isRequired)
			where Field : IBqlField
		{
			INPIDetail item = (INPIDetail)PIDetail.Cache.CreateInstance();
			string displayName = PXUIFieldAttribute.GetDisplayName<Field>(PIDetail.Cache);
			if (!values.Contains(typeof(Field).Name) && isRequired)
				throw new PXException(Messages.CollumnIsMandatory, displayName);
			object value = values[typeof(Field).Name];
			PIDetail.Cache.RaiseFieldUpdating<Field>(item, ref value);
			if (isRequired && value == null)
				throw new PXException(ErrorMessages.FieldIsEmpty, displayName);
			return value;
		}
		#endregion

		#region IPXPrepareItems

		public bool PrepareImportRow(string viewName, IDictionary keys, IDictionary values)
		{
			skipRecalcTotals = true;
			if (string.Compare(viewName, PIDetail.View.Name, true) == 0)
			{
				PXCache barCodeCache = AddByBarCode.Cache;
				INBarCodeItem item = (INBarCodeItem)(AddByBarCode.Current ?? barCodeCache.CreateInstance());
				try
				{
					barCodeCache.SetValueExt<INBarCodeItem.inventoryID>(item, GetImportedValue<INPIDetail.inventoryID>(values, true));
					if (PXAccess.FeatureInstalled<FeaturesSet.subItem>())
						barCodeCache.SetValueExt<INBarCodeItem.subItemID>(item, GetImportedValue<INPIDetail.subItemID>(values, true));
					if (PXAccess.FeatureInstalled<FeaturesSet.warehouseLocation>())
						barCodeCache.SetValueExt<INBarCodeItem.locationID>(item, GetImportedValue<INPIDetail.locationID>(values, true));
					if (PXAccess.FeatureInstalled<FeaturesSet.lotSerialTracking>())
					{
						barCodeCache.SetValueExt<INBarCodeItem.lotSerialNbr>(item, GetImportedValue<INPIDetail.lotSerialNbr>(values, false));
						barCodeCache.SetValueExt<INBarCodeItem.expireDate>(item, GetImportedValue<INPIDetail.expireDate>(values, false));
					}
					barCodeCache.SetValueExt<INBarCodeItem.qty>(item, GetImportedValue<INPIDetail.physicalQty>(values, true));
					barCodeCache.SetValueExt<INBarCodeItem.autoAddLine>(item, false);
					barCodeCache.Update(item);
					UpdatePhysicalQty();
				}
				catch (Exception e)
				{
					PXTrace.WriteError(IN.Messages.RowError, excelRowNumber, e.Message);
					importHasError = true;
				}
				finally
				{
					excelRowNumber ++;
				}
			}
			return false;
		}

		public bool RowImporting(string viewName, object row)
		{
			return false;
		}

		public bool RowImported(string viewName, object row, object oldRow)
		{
			return false;
		}

		public void PrepareItems(string viewName, IEnumerable items)
		{
		}

		#endregion

		#region IPXProcess
		public void ImportDone(PXImportAttribute.ImportMode.Value mode)
		{
			skipRecalcTotals = false;
			RecalcTotals();

			if (importHasError)
				throw new Exception(IN.Messages.ImportHasError);
		}
		#endregion
	}


	public class INPIReview : INPIEntry  // (= PIEntry + some extended functionality)
	{

		public PXAction<INPIHeader> finishCounting;
		public PXAction<INPIHeader> completePI;
		public PXAction<INPIHeader> cancelPI;
		public PXAction<INPIHeader> updateCost;

		public PXAction<INPIHeader> setNotEnteredToZero;
		public PXAction<INPIHeader> setNotEnteredToSkipped;

		#region Adaptors

		[PXUIField(DisplayName = Messages.SetNotEnteredToZero)]
		//[PXProcessButton]
		protected virtual IEnumerable SetNotEnteredToZero(PXAdapter adapter)
		{
			INPIHeader header = PIHeader.Current;
			if (header == null || !((header.Status == INPIHdrStatus.Counting) || (header.Status == INPIHdrStatus.Entering))) 
				{ return adapter.Get(); }
			
			foreach (INPIDetail det in PIDetail.Select())
			{
				if ((det.Status == INPIDetStatus.NotEntered))
				{
					if ((det.InventoryID != null) && (det.SubItemID != null) && (det.LocationID != null) && (LSRequired(det.InventoryID) ? (det.LotSerialNbr != null) : true))
					{
						PIDetail.Cache.SetValue<INPIDetail.physicalQty>(det, 0m);
						PIDetail.Cache.SetValue<INPIDetail.varQty>(det, -det.BookQty);
						PIDetail.Cache.SetValue<INPIDetail.extVarCost>(det, 
							PXDBCurrencyAttribute.BaseRound(this,(-det.BookQty*det.UnitCost) ?? 0m));							

						PIDetail.Cache.SetValue<INPIDetail.status>(det, INPIDetStatus.Entered);
						PIDetail.Cache.SetStatus(det, PXEntryStatus.Updated);
					}
				}
			}
			RecalcTotals();
			//RecalcDemandCost();
			return adapter.Get();
		}
		

		[PXUIField(DisplayName = Messages.SetNotEnteredToSkipped)]
		[PXProcessButton]
		protected virtual IEnumerable SetNotEnteredToSkipped(PXAdapter adapter)
		{
			INPIHeader header = PIHeader.Current;
			if (header == null || !((header.Status == INPIHdrStatus.Counting) || (header.Status == INPIHdrStatus.Entering))) 
				{ return adapter.Get(); }

			foreach (INPIDetail det in PIDetail.Select())
			{
				if ((det.Status == INPIDetStatus.NotEntered))
				{
					//if ((d.InventoryID != null) && (d.SubItemID != null) && (d.LocationID != null) && (LSRequired(d.InventoryID) ? (d.LotSerialNbr != null) : true))
					// does not matter are these fields entered or not
					{
						det.PhysicalQty = null;
						det.VarQty = null;
						det.Status = INPIDetStatus.Skipped;
						PIDetail.Update(det);
						// no need to recalc cost & totals because these values are not affected by this action
					}

				}
			}
			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.UpdateCost)]
		[PXProcessButton]
		protected virtual IEnumerable UpdateCost(PXAdapter adapter)
		{
			INPIHeader header = PIHeader.Current;
			if (header == null || !((header.Status == INPIHdrStatus.Counting) || (header.Status == INPIHdrStatus.Entering)))
			{ return adapter.Get(); }

			foreach (INPIDetail det in PIDetail.Select())
			{
				if (det.InventoryID != null && det.SubItemID != null && det.LocationID != null && (det.VarQty ?? 0) != 0)
				{					
					PIDetail.Cache.SetValue<INPIDetail.unitCost>(det, null);
					PIDetail.Cache.SetValue<INPIDetail.extVarCost>(det, null);					
					PIDetail.Cache.SetStatus(det, PXEntryStatus.Updated);
				}
			}
			RecalcDemandCost();
			return adapter.Get();
		}


		[PXUIField(DisplayName = Messages.CompletePI, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CompletePI(PXAdapter adapter)
		{
			foreach (INPIHeader ph in adapter.Get())
			{
				bool checkOK = true;
				PIHeader.Current = ph;								

				if (!((ph.Status == INPIHdrStatus.Counting) || (ph.Status == INPIHdrStatus.Entering)))
				{
					throw new PXException(Messages.Document_Status_Invalid);
				}
				
				// line entered status
				foreach (INPIDetail pd in this.PIDetail.Select())
				{
					if (pd.Status == INPIDetStatus.NotEntered)
					{
						if (pd.InventoryID != null)
						{
							PIDetail.Cache.RaiseExceptionHandling<INPIDetail.lineNbr>(pd, pd.LineNbr, new PXSetPropertyException(Messages.NotEnteredLineDataError, PXErrorLevel.RowError));
							checkOK = false;
						}
					}
				}

				if (checkOK)
				{
					Save.Press();
					INPIHeader ph1 = ph;
					PXLongOperation.StartOperation
					(
						this,
						() =>
							{
								INPIReview docgraph = new INPIReview();
								docgraph.DoComplete(ph1);
							}
						);
				}
				yield return ph;
			}
		}

		[PXUIField(DisplayName = Messages.FinishCounting, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable FinishCounting(PXAdapter adapter)
		{
			INPIHeader header = PIHeader.Current;

			if (header == null || header.Status != INPIHdrStatus.Counting) 
				return adapter.Get();

			header.Status = INPIHdrStatus.Entering;
			PIHeader.Update(header);

			INSite site = PXSelect<INSite, Where<INSite.siteID, Equal<Required<INSite.siteID>>>>.SelectWindowed(this, 0, 1, header.SiteID);
			if (site == null || site.LockSitePICountEntry != true)
				UnlockInventory(false);

			this.Save.Press();

			return adapter.Get();
		}

		[PXUIField(DisplayName = Messages.CancelPI, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable CancelPI(PXAdapter adapter)
		{
			INPIHeader header = PIHeader.Current;
			if (header == null || !((header.Status == INPIHdrStatus.Counting) || (header.Status == INPIHdrStatus.Entering))) 
				return adapter.Get(); 

			header.Status = INPIHdrStatus.Cancelled;
			PIHeader.Update(header);

			UnlockInventory(true);

			this.Save.Press();

			return adapter.Get();
		}

		#region viewAdjustment

		/*
		public PXAction<INPIHeader> viewAdjustment;
		[PXUIField(DisplayName = "View Adjustment", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewAdjustment(PXAdapter adapter)
		{
			if (PIHeader.Current != null && !String.IsNullOrEmpty(PIHeader.Current.PIAdjRefNbr))
			{
				INAdjustmentEntry piadjgraph = PXGraph.CreateInstance<INAdjustmentEntry>();
				piadjgraph.adjustment.Current = piadjgraph.adjustment.Search<INRegister.refNbr>(PIHeader.Current.PIAdjRefNbr, INDocType.Adjustment);
				throw new PXRedirectRequiredException(piadjgraph, "Adjustment");
			}
			return adapter.Get();
		} 
		*/

		#endregion viewAdjustment



		#endregion Adaptors

		#region DAC overrides

		public PXFilter<PX.Data.PXImportAttribute.CSVSettings> cSVSettings;
		public PXFilter<PX.Data.PXImportAttribute.XLSXSettings> xLSXSettings;

		[PXString]
		[PXUIField(Visible = false)]
		public virtual void CSVSettings_Mode_CacheAttached(PXCache sender)
		{
		}

		[PXString]
		[PXUIField(Visible = false)]
		public virtual void XLSXSettings_Mode_CacheAttached(PXCache sender)
		{
		}

		#endregion

		public INPIReview()
		{
			//PIHeader.Cache.AllowInsert = false;
			//PIHeader.Cache.AllowDelete = false;

			//PIDetail.Cache.AllowInsert = false;
			//PIDetail.Cache.AllowDelete = false;

			// AllowUpdate : see INPIHeader_RowSelected

            //PXDimensionSelectorAttribute.SetValidCombo<INPIDetail.subItemID>(PIDetail.Cache, true);
		}

		public virtual void DoComplete(INPIHeader p_h)
		{
			INSetup insetup = INSetup.Select();

			INPIHeader header = PIHeader.Current = PIHeader.Search<INPIHeader.pIID>(p_h.PIID);
			if (header == null || insetup == null || insite.Current == null) { return; }

			foreach (INPIDetail det in PIDetail.Select())
			{
				if (det != null)
				{
					det.UnitCost = null;
					det.ExtVarCost = null;
					PIDetail.Update(det);
				}
			}

			List<ProjectedTranRec> projectedTrans = new List<ProjectedTranRec>();
			RecalcDemandCost(projectedTrans);

			List<INRegister> releaseList = new List<INRegister>();
			using (new PXConnectionScope())
			{
				using (PXTransactionScope ts = new PXTransactionScope())
				{

					//KeyValuePair<PXErrorLevel, string>?[] messages = (KeyValuePair<PXErrorLevel, string>?[])PXLongOperation.GetCustomInfo();
					// (not using PXProcessing)					


                    INAdjustmentEntry je = PXGraph.CreateInstance<INAdjustmentEntry>();

                    je.insetup.Current.RequireControlTotal = false;
                    je.insetup.Current.HoldEntry = false;

                    if (je.adjustment.Current == null)
                    {
                        INRegister newdoc = new INRegister();
                        newdoc.BranchID = insite.Current.BranchID;
                        newdoc.OrigModule = INRegister.origModule.PI;
                        je.adjustment.Cache.Insert(newdoc);
                    }

                    foreach (ProjectedTranRec projectedTran in projectedTrans)
                    {
                        if (projectedTran.AdjNotReceipt)
                        {
                            INTran tran = new INTran();
                            tran.BranchID = insite.Current.BranchID;
                            tran.TranType = INTranType.Adjustment;
                            // INTranType.StandardCostAdjustment for standard-costed items ? ..not

                            tran.InvtAcctID = projectedTran.AcctID;
                            tran.InvtSubID = projectedTran.SubID;

                            tran.AcctID = null; // left to be defaulted during release
                            tran.SubID = null;

                            tran.InventoryID = projectedTran.InventoryID;
                            tran.SubItemID = projectedTran.SubItemID;
                            tran.SiteID = header.SiteID;
                            tran.LocationID = projectedTran.LocationID;
                            tran.Qty = projectedTran.VarQtyPortion;
                            tran.TranCost = projectedTran.VarCostPortion;
                            tran.ReasonCode = projectedTran.ReasonCode;

                            tran.OrigRefNbr = projectedTran.OrigRefNbr;
                            tran.LotSerialNbr = projectedTran.LotSerialNbr;
                            tran.ExpireDate = projectedTran.ExpireDate;
                            tran = PXCache<INTran>.CreateCopy(je.transactions.Insert(tran));
                            //tran = je.transactions.Update(tran);

                            /*System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1}-{2}",
                                    tran.LineNbr, tran.InventoryID, tran.LotSerialNbr));*/
                        }

                        if (!projectedTran.AdjNotReceipt)
                        {
                            INTran tran = new INTran();
                            tran.BranchID = insite.Current.BranchID;
                            tran.TranType = INTranType.Adjustment;
                            tran = PXCache<INTran>.CreateCopy(je.transactions.Insert(tran));
                            tran.InvtAcctID = projectedTran.AcctID;
                            tran.InvtSubID = projectedTran.SubID;

                            tran.AcctID = null; // left to be defaulted during release
                            tran.SubID = null;

                            tran.InventoryID = projectedTran.InventoryID;
                            tran.SubItemID = projectedTran.SubItemID;
                            tran.SiteID = header.SiteID;
                            tran.LocationID = projectedTran.LocationID;
                            tran.UOM = projectedTran.UOM;
                            tran.Qty = projectedTran.VarQtyPortion;
                            tran.TranCost = projectedTran.VarCostPortion;
                            tran.ReasonCode = projectedTran.ReasonCode;
                            tran = PXCache<INTran>.CreateCopy(je.transactions.Update(tran));
                            tran.LotSerialNbr = projectedTran.LotSerialNbr;
                            tran.ExpireDate = projectedTran.ExpireDate;
                            tran = PXCache<INTran>.CreateCopy(je.transactions.Update(tran));
                        }
                    }

                    je.Save.Press();
                    header.PIAdjRefNbr = je.adjustment.Current.RefNbr;

                    if (je.adjustment.Current != null) { releaseList.Add(je.adjustment.Current); }

					PIHeader.Current = header;
					header.Status = INPIHdrStatus.Completed;										
					RecalcTotals();
					UnlockInventory(true);

					this.Save.Press();					
					ts.Complete();										
				}
			}
			if (releaseList.Count > 0)
			{
				INDocumentRelease.ReleaseDoc(releaseList, false);
			}
		}

		#region Events
		protected override void INPIHeader_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			base.INPIHeader_RowSelected(sender, e);
			INPIHeader h = (INPIHeader)e.Row;
			if (h == null) { return; }

			bool statusCountingOrEntering = (h.Status == INPIHdrStatus.Counting) || (h.Status == INPIHdrStatus.Entering);			

			updateCost.SetEnabled(h.Status == INPIHdrStatus.Entering);
			finishCounting.SetEnabled(h.Status == INPIHdrStatus.Counting); 
			cancelPI.SetEnabled(statusCountingOrEntering);
			completePI.SetEnabled(statusCountingOrEntering);

		}
		#endregion Events
	}

	#region PIInvtSiteLoc projection

	[System.SerializableAttribute]
	[PXProjection(typeof(Select5<INPIHeader,
		InnerJoin<INPIDetail,
			On<INPIDetail.pIID, Equal<INPIHeader.pIID>>>,
		Aggregate<
			GroupBy<INPIHeader.pIID,
			GroupBy<INPIDetail.inventoryID,
			GroupBy<INPIHeader.siteID,
			GroupBy<INPIDetail.locationID>>>>>>))]

    [PXHidden]
	public partial class PIInvtSiteLoc : PX.Data.IBqlTable
	{
		#region PIID
		public abstract class pIID : PX.Data.IBqlField
		{
		}
		protected String _PIID;
		[GL.FinPeriodID(IsKey = true, BqlField = typeof(INPIHeader.pIID))]
		public virtual String PIID
		{
			get
			{
				return this._PIID;
			}
			set
			{
				this._PIID = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[StockItem(IsKey = true, BqlField = typeof(INPIDetail.inventoryID))]
		[PXDefault]
		public virtual Int32? InventoryID
		{
			get
			{
				return this._InventoryID;
			}
			set
			{
				this._InventoryID = value;
			}
		}
		#endregion
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site(IsKey = true, BqlField = typeof(INPIHeader.siteID))]
		[PXDefault]
		public virtual Int32? SiteID
		{
			get
			{
				return this._SiteID;
			}
			set
			{
				this._SiteID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[IN.Location(IsKey = true, BqlField = typeof(INPIDetail.locationID))]
		[PXDefault]
		public virtual Int32? LocationID
		{
			get
			{
				return this._LocationID;
			}
			set
			{
				this._LocationID = value;
			}
		}
		#endregion
	}

	#endregion PIInvtSiteLoc projection

}
