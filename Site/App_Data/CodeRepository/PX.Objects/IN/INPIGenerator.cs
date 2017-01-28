using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.CM;

namespace PX.Objects.IN
{

	#region PI Generator Settings

	[Serializable]
	public partial class PIGeneratorSettings : PX.Data.IBqlTable
	{
		#region PIClassID
		public abstract class pIClassID : PX.Data.IBqlField
		{
		}
		protected String _PIClassID;
		[PXDBString(30, IsUnicode = true)]
		[PXUIField(DisplayName = "Type ID", Visibility = PXUIVisibility.SelectorVisible)]
		[PXSelector(typeof(Search<INPIClass.pIClassID>))]
		[PXDefault()]
		public virtual String PIClassID
		{
			get
			{
				return this._PIClassID;
			}
			set
			{
				this._PIClassID = value;
			}
		}
		#endregion

		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[PXDefault()]
		[Site]
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

		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXDefault()]
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion

		#region Method
		public abstract class method : PX.Data.IBqlField
		{
		}
		protected String _Method;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Generation Method", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
		[PIMethod.List]
		public virtual String Method
		{
			get
			{
				return this._Method;
			}
			set
			{
				this._Method = value;
			}
		}
		#endregion

		#region SelectedMethod
		public abstract class selectedMethod : PX.Data.IBqlField
		{
		}
		protected String _SelectedMethod;
		[PXDBString(1, IsFixed = true)]
		[PXUIField(DisplayName = "Selection Method", Enabled = false)]
		[PIInventoryMethod.List]
		public virtual String SelectedMethod
		{
			get
			{
				return this._SelectedMethod;
			}
			set
			{
				this._SelectedMethod = value;
			}
		}
		#endregion

		#region ByFrequency
		public abstract class byFrequency : PX.Data.IBqlField
		{
		}
		protected Boolean? _ByFrequency;
		[PXDBBool()]
		[PXUIField(DisplayName = "By Frequency")]
		public virtual Boolean? ByFrequency
		{
			get
			{
				return this._ByFrequency;
			}
			set
			{
				this._ByFrequency = value;
			}
		}
		#endregion

		#region CycleID
		public abstract class cycleID : PX.Data.IBqlField
		{
		}
		protected String _CycleID;
		[PXDBString(10, IsUnicode = true)]
		[PXSelector(typeof(INPICycle.cycleID), DescriptionField = typeof(INPICycle.descr))]
		[PXUIField(DisplayName = "Cycle ID")]
		public virtual String CycleID
		{
			get
			{
				return this._CycleID;
			}
			set
			{
				this._CycleID = value;
			}
		}
		#endregion

		#region ABCCodeID
		public abstract class aBCCodeID : PX.Data.IBqlField
		{
		}
		protected String _ABCCodeID;
		[PXDBString(1, IsFixed = true)]
		[PXSelector(typeof(INABCCode.aBCCodeID), DescriptionField = typeof(INABCCode.descr))]
		[PXUIField(DisplayName = "ABC Code")]
		public virtual String ABCCodeID
		{
			get
			{
				return this._ABCCodeID;
			}
			set
			{
				this._ABCCodeID = value;
			}
		}
		#endregion

		#region MovementClassID
		public abstract class movementClassID : PX.Data.IBqlField
		{
		}
		protected String _MovementClassID;
		[PXDBString(1, IsFixed = true)]
		[PXSelector(typeof(INMovementClass.movementClassID), DescriptionField = typeof(INMovementClass.descr))]
		[PXUIField(DisplayName = "Movement Class ID")]
		public virtual String MovementClassID
		{
			get
			{
				return this._MovementClassID;
			}
			set
			{
				this._MovementClassID = value;
			}
		}
		#endregion

		#region ItemClassID
		public abstract class itemClassID : PX.Data.IBqlField
		{
		}
		protected String _ItemClassID;
		[PXDBString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
		[PXUIField(DisplayName = "Item Class ID")]
		[PXSelector(typeof(Search<INItemClass.itemClassID, Where<INItemClass.stkItem, Equal<boolTrue>>>), DescriptionField = typeof(INItemClass.descr))]
		public virtual String ItemClassID
		{
			get
			{
				return this._ItemClassID;
			}
			set
			{
				this._ItemClassID = value;
			}
		}
		#endregion

		#region BlankLines
		public abstract class blankLines : IBqlField
		{
		}
		protected Int16? _BlankLines;
		[PXDefault((Int16)0)]
		[PXDBShort(MinValue = 0, MaxValue = 10000)]
		[PXUIField(DisplayName = "Blank Lines To Append")]
		public virtual Int16? BlankLines
		{
			get
			{
				return this._BlankLines;
			}
			set
			{
				this._BlankLines = value;
			}
		}
		#endregion

		#region RandomItemsLimit
		public abstract class randomItemsLimit : IBqlField
		{
		}
		protected Int16? _RandomItemsLimit;
		[PXDefault((Int16)0)]
		[PXShort(MinValue = 0, MaxValue = 10000)]
		[PXUIField(DisplayName = "Number of Items (up to)")]
		public virtual Int16? RandomItemsLimit
		{
			get
			{
				return this._RandomItemsLimit;
			}
			set
			{
				this._RandomItemsLimit = value;
			}
		}
		#endregion

		#region LastCountPeriod
		public abstract class lastCountPeriod : IBqlField
		{
		}
		protected Int16? _LastCountPeriod;
		[PXDefault((Int16)0)]
		[PXDBShort(MinValue = 0, MaxValue = 10000)]
		[PXUIField(DisplayName = "Last Count Before (days)", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int16? LastCountPeriod
		{
			get
			{
				return this._LastCountPeriod;
			}
			set
			{
				this._LastCountPeriod = value;
			}
		}
		#endregion

		#region RandomSeed
		// to save set of randomly-selected items between roundtrips 
		public int RandomSeed;
		#endregion

		#region MaxLastCountDate
		public abstract class maxLastCountDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _MaxLastCountDate;
		[PXDate]
		[PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Last Count Not Later")]
		public virtual DateTime? MaxLastCountDate
		{
			get
			{
				return this._MaxLastCountDate;
			}
			set
			{
				this._MaxLastCountDate = value;
			}
		}
		#endregion
	}

	#endregion

	#region PIPreliminaryResult
    [Serializable]
	public partial class PIPreliminaryResult : PX.Data.IBqlTable
	{

		#region LineNbr
		public abstract class lineNbr : PX.Data.IBqlField { }
		protected Int32? _LineNbr;
		[PXDBInt(IsKey = true)]
		[PXUIField(DisplayName = "Line Number", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? LineNbr
		{
			get
			{
				return this._LineNbr;
			}
			set
			{
				this._LineNbr = value;
			}
		}
		#endregion
		#region TagNumber
		public abstract class tagNumber : PX.Data.IBqlField
		{
		}
		protected Int32? _TagNumber;
		[PXInt(MinValue = 0)]
		[PXUIField(DisplayName = "Tag Number", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Int32? TagNumber
		{
			get
			{
				return this._TagNumber;
			}
			set
			{
				this._TagNumber = value;
			}
		}
		#endregion
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField { }
		protected Int32? _InventoryID;
		[Inventory(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Inventory ID")]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[SubItem(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Subitem")]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField { }
		protected Int32? _LocationID;
		[Location(Visibility = PXUIVisibility.SelectorVisible, DisplayName = "Location")]
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
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true)]
		[PXUIField(DisplayName = "Lot/Serial Number", Visibility = PXUIVisibility.SelectorVisible)]
		//[INLotSerialNbr(typeof(PIPreliminaryResult.inventoryID), typeof(PIPreliminaryResult.subItemID), typeof(PIPreliminaryResult.locationID), PersistingCheck = PXPersistingCheck.Nothing)]
		public virtual String LotSerialNbr
		{
			get
			{
				return this._LotSerialNbr;
			}
			set
			{
				this._LotSerialNbr = value;
			}
		}
		#endregion
		#region ExpireDate
		public abstract class expireDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _ExpireDate;
		[PXDBDate()]
		[PXUIField(DisplayName = "Expiration Date")]
		public virtual DateTime? ExpireDate
		{
			get
			{
				return this._ExpireDate;
			}
			set
			{
				this._ExpireDate = value;
			}
		}
		#endregion
		#region BookQty
		public abstract class bookQty : PX.Data.IBqlField
		{
		}
		protected Decimal? _BookQty;
		[PXDBQuantity]
		//[PXDefault(TypeCode.Decimal, "0.0")]
		[PXUIField(DisplayName = "Book Qty.", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual Decimal? BookQty
		{
			get
			{
				return this._BookQty;
			}
			set
			{
				this._BookQty = value;
			}
		}
		#endregion
		#region BaseUnit
		public abstract class baseUnit : PX.Data.IBqlField
		{
		}
		protected String _BaseUnit;
		[INUnit(DisplayName = "Base Unit", Visibility = PXUIVisibility.Visible)]
		public virtual String BaseUnit
		{
			get
			{
				return this._BaseUnit;
			}
			set
			{
				this._BaseUnit = value;
			}
		}
		#endregion
		#region Descr
		public abstract class descr : PX.Data.IBqlField
		{
		}
		protected String _Descr;
		[PXString(60, IsUnicode = true)]
		[PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
		public virtual String Descr
		{
			get
			{
				return this._Descr;
			}
			set
			{
				this._Descr = value;
			}
		}
		#endregion

	}
	#endregion

	public class PIGenerator : PXGraph<PIGenerator>
	{
		public PXCancel<PIGeneratorSettings> Cancel;

		public PXFilter<PIGeneratorSettings> GeneratorSettings;

		#region Cache Attached

        [PXDBString(30, IsUnicode = true, IsKey = true)]
		[PXDefault(typeof(PIGeneratorSettings.pIClassID))]
		protected virtual void INPIClassLocation_PIClassID_CacheAttached(PXCache sender)
		{
		}

		[Location(typeof(PIGeneratorSettings.siteID), IsKey = true)]
		protected virtual void INPIClassLocation_LocationID_CacheAttached(PXCache sender)
		{
		}

        [PXDBInt(IsKey = true)]
        protected virtual void INLocationStatus_InventoryID_CacheAttached(PXCache sender)
        { 
        }

        [PXDBInt(IsKey = true)]
        protected virtual void INLocationStatus_SubItemID_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt(IsKey = true)]
        protected virtual void INLocationStatus_SiteID_CacheAttached(PXCache sender)
        {
        }

        [PXDBInt(IsKey = true)]
        protected virtual void INLocationStatus_LocationID_CacheAttached(PXCache sender)
        {
        }

		#endregion

		//public PXSelect<INPIClassLocationSelection> LocSelectionSettings;
		public PXSelectOrderBy<PIPreliminaryResult, OrderBy<Asc<PIPreliminaryResult.lineNbr>>> PreliminaryResultRecs;

		public PXSelectJoin<INPIClassLocation,
			InnerJoin<INLocation, On<INLocation.locationID, Equal<IN.INPIClassLocation.locationID>>>,
			Where<INPIClassLocation.pIClassID, Equal<Current<PIGeneratorSettings.pIClassID>>>> locations;

		public PXSelect<INPIHeader> piheader;
		public PXSelect<INPIDetail, Where<INPIDetail.pIID, Equal<Current<INPIHeader.pIID>>>> pidetail;

        public PXSelect<INLocationStatus> inlocationstatus;

		//public PXSelect<INItemSite> itemsite;
		public PXSelect<INPIStatus> pistatus;
		//	// (PICountStatus flag now in INPIStatus, not INItemSite)

		//public PXSelect<INLocationStatus> locstatus;
		//public PXSelect<INLotSerialStatus> lotserialstatus;

		public PXSetup<INSetup> insetup;

		public PXAction<PIGeneratorSettings> GeneratePI;

		[PXUIField(DisplayName = Messages.GeneratePI, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXProcessButton]
		public virtual IEnumerable generatePI(PXAdapter adapter)
		{
			PXLongOperation.StartOperation(this.UID, (argument =>
				{
					PIGenerator graph = PXGraph.CreateInstance<PIGenerator>();
					graph.GeneratorSettings.Current = (PIGeneratorSettings) argument[0];
					foreach (INPIClassLocation location in (List<INPIClassLocation>) argument[1])
					{
						graph.locations.Cache.SetStatus(location, PXEntryStatus.Inserted);
					}

					// based on generation settings, recreate list (as in PreliminaryResultRecs) and save it to INPIHeader / INPIDetail tables, updating statuses in INItemSite and LastTagNumber in INSetup					
                    var list = graph.CalcPIRows(true, false);
                    if (list.Count == 0)
					{
						throw new PXException(Messages.PIEmpty);
					}

					// !!!  upon PI generation - redirect to the PI Review screen here 
					INPIReview target = PXGraph.CreateInstance<INPIReview>();
					target.Clear();
					target.PIHeader.Current = PXSelect<INPIHeader, Where<INPIHeader.pIID, Equal<Current<INPIHeader.pIID>>>>.Select(this);

					throw new PXRedirectRequiredException(target, "View INPIReview");
				}
				),
				new object[] { this.GeneratorSettings.Current, this.locations.Cache.Cached.Cast<INPIClassLocation>().ToList() });

			return adapter.Get();
		}
	
		public PIGenerator()
		{
			PreliminaryResultRecs.Cache.AllowInsert = false;
			PreliminaryResultRecs.Cache.AllowDelete = false;
			PreliminaryResultRecs.Cache.AllowUpdate = false;

			this.Views.Caches.Remove(typeof(INPIClassLocation));

            locations.AllowInsert = false;
            locations.AllowUpdate = false;
            locations.AllowDelete = false;
		}


		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		public virtual List<PIPreliminaryResult> CalcPIRows(bool updateDB)
		{
			return CalcPIRows(updateDB, true);
		}

		public virtual List<PIPreliminaryResult> CalcPIRows(bool updateDB, bool saveEmpty)
		{
			INSetup insetuprec = insetup.Current;
			PIGeneratorSettings gs = GeneratorSettings.Current;

			List<PIPreliminaryResult> ResultList = new List<PIPreliminaryResult>();

			if (gs == null)
			{
				return ResultList;
			} //empty

			if (gs.SiteID == null)
			{
				return ResultList;
			} //empty


			if (updateDB)
			{
				piheader.Cache.Clear();
				pidetail.Cache.Clear();
				pistatus.Cache.Clear();
			}

			HashSet<int> selectedLocations = null;
			var source = this.locations.Select();
			if (source.Count > 0)
			{
				selectedLocations = new HashSet<int>();
				foreach (PXResult<INPIClassLocation> r in source)
				{
					INPIClassLocation l = r;
					if (l.LocationID != null)
						selectedLocations.Add(l.LocationID.Value);
				}
			}

			BqlCommand cmd = BqlCommand.CreateInstance(
				typeof(
				Select2<INLocationStatus,
						InnerJoin<InventoryItem,
							On<InventoryItem.inventoryID, Equal<INLocationStatus.inventoryID>,
							And<InventoryItem.stkItem, Equal<boolTrue>,
							And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
							And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.markedForDeletion>,
							And<Where<Match<InventoryItem, Current<AccessInfo.userName>>>>>>>>,
						LeftJoin<INItemStats,
								On<INItemStats.inventoryID, Equal<INLocationStatus.inventoryID>,
									And<INItemStats.siteID, Equal<INLocationStatus.siteID>>>,
						LeftJoin<INLotSerClass,
									On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>,
						LeftJoin<INLotSerialStatus,
										On<INLotSerialStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
									 And<INLotSerClass.lotSerAssign, Equal<INLotSerAssign.whenReceived>,
									 And<INLotSerialStatus.subItemID, Equal<INLocationStatus.subItemID>,
									 And<INLotSerialStatus.siteID, Equal<INLocationStatus.siteID>,
									 And<INLotSerialStatus.locationID, Equal<INLocationStatus.locationID>>>>>>,
						InnerJoin<INSubItem,
									 On<INSubItem.subItemID, Equal<INLocationStatus.subItemID>>,
						InnerJoin<INLocation,
									 On<INLocation.locationID, Equal<INLocationStatus.locationID>>,
						LeftJoin<INPIStatus,
									On<INPIStatus.siteID, Equal<INLocationStatus.siteID>,
								 And<INPIStatus.active, Equal<boolTrue>,
								 And2<Where<INPIStatus.inventoryID, IsNull,
											   Or<INPIStatus.inventoryID, Equal<INLocationStatus.inventoryID>>>,
												And<Where<INPIStatus.locationID, IsNull,
													   Or<INPIStatus.locationID, Equal<INLocationStatus.locationID>>>>>>>,
						InnerJoin<INItemSite,
												On<INItemSite.inventoryID, Equal<INLocationStatus.inventoryID>,
											 And<INItemSite.siteID, Equal<INLocationStatus.siteID>>>>>>>>>>>,
						Where<InventoryItem.stkItem, Equal<boolTrue>,
							And<INLocationStatus.siteID, Equal<Current<PIGeneratorSettings.siteID>>,
							And<INPIStatus.recordID, IsNull,
							And<
									Where2<
											Where<
												INLotSerialStatus.inventoryID, IsNotNull,
												And<Sub<Sub<INLotSerialStatus.qtyOnHand, INLotSerialStatus.qtySOShipped>, INLotSerialStatus.qtyINIssues>,
														NotEqual<decimal0>>
												 >,
											Or<
												Where<
													INLotSerialStatus.inventoryID, IsNull,
													And<Sub<Sub<INLocationStatus.qtyOnHand, INLocationStatus.qtySOShipped>, INLocationStatus.qtyINIssues>,
														NotEqual<decimal0>>>

												>
											>
										>>>>>));

			cmd = UpdateCommandByGenerationMethod(cmd);

			//			PXResultset<INLocationStatus> IntermediateResult = (PXResultset<INLocationStatus>)cmd.Select();

			// not cmd.Select because we need to implement user-defined sort

			PXResultset<INLocationStatus> IntermediateResult = new PXResultset<INLocationStatus>();

			int startRow = 0;
			int totalRows = 0;

            //if (!updateDB && selectedLocations != null)
            if (selectedLocations != null)
            {
                cmd = BqlCommand.AppendJoin<InnerJoin<INPIClassLocation, On<INPIClassLocation.locationID, Equal<INLocationStatus.locationID>, And<INPIClassLocation.pIClassID, Equal<Current<PIGeneratorSettings.pIClassID>>>>>>(cmd);
            }

			if (cmd != null)
			{
				PXView view = new PXView(this, true, cmd);
				IntermediateResult.AddRange
					(from PXResult<INLocationStatus> r
						in view.Select(null, null, null, SortColumnsFromNAO(), null, null, ref startRow, -1, ref totalRows)
					 where (INLocationStatus)r != null &&
						   ((INLocationStatus)r).LocationID != null &&
								 (selectedLocations == null ||
								  selectedLocations.Contains(((INLocationStatus)r).LocationID.Value))
					 select r);
			}

			// the only generation method which requires post-processing is Randomly-Selected Items:
			if (gs.Method == PIMethod.ByInventoryItemSelected)
				switch (gs.SelectedMethod)
				{
					case PIInventoryMethod.RandomlySelectedItems:
						if ((gs.RandomItemsLimit ?? 0) > 0)
						{
							// random set of items
							HashSet<int> fullInventoryIDSet = new HashSet<int>();
							foreach (PXResult<INLocationStatus, InventoryItem> ii_rec in IntermediateResult)
							{
								if (((InventoryItem)ii_rec).InventoryID != null)
									fullInventoryIDSet.Add((int)((InventoryItem)ii_rec).InventoryID);
							}
							if (fullInventoryIDSet.Count > (gs.RandomItemsLimit ?? 0))
							{
								// get random inventory id

								List<int> fullInventoryIDList = new List<int>(fullInventoryIDSet);
								fullInventoryIDList.Sort(); // to get the same list between roundtrips

								HashSet<int> randomInventoryIDSet = new HashSet<int>();
								SetRandomSeedIfZero();
								Random randObj = new Random(gs.RandomSeed); // to get the same list between roundtrips

								for (int i = 0; i < (gs.RandomItemsLimit ?? 0); i++)
								{
									int nextRandomElementToSelect = randObj.Next(0, (fullInventoryIDList.Count));
									//2nd parm in Next is exclusive, so Count instead of Count-1
									randomInventoryIDSet.Add(fullInventoryIDList[nextRandomElementToSelect]);
									fullInventoryIDList.RemoveAt(nextRandomElementToSelect);
								}

								PXResultset<INLocationStatus> randomlycutIR = new PXResultset<INLocationStatus>();
								foreach (PXResult<INLocationStatus, InventoryItem, INItemStats, INLotSerClass, INLotSerialStatus, INSubItem, INLocation, INPIStatus> it in IntermediateResult)
								{
									if (randomInventoryIDSet.Contains(((InventoryItem)it).InventoryID ?? 0))
										randomlycutIR.Add(it);
								}
								IntermediateResult = randomlycutIR;
							}
						}
						break;

					case PIInventoryMethod.ItemsHavingNegativeBookQty:

						PXResultset<INLocationStatus> NewIntermediateResult = new PXResultset<INLocationStatus>();

						foreach (
							PXResult<INLocationStatus, InventoryItem, INItemStats, INLotSerClass, INLotSerialStatus, INSubItem, INLocation, INPIStatus> it
								in IntermediateResult)
						{
							NewIntermediateResult.Add(it);
							INLocationStatus negativeLocationStatus = it;

							cmd = cmd.WhereNew<Where<InventoryItem.stkItem, Equal<boolTrue>,
								And<INLocationStatus.siteID, Equal<Current<PIGeneratorSettings.siteID>>,
									And<INLocationStatus.inventoryID, Equal<Required<INLocationStatus.inventoryID>>,
										And<INLocationStatus.subItemID, Equal<Required<INLocationStatus.subItemID>>,
											And<INLocationStatus.locationID, NotEqual<Required<INLocationStatus.locationID>>
												>>>>>>();

							var neg = new PXView(this, true, cmd);

							foreach (PXResult<INLocationStatus> item in
								neg.Select(null,
								new object[]{ 
										negativeLocationStatus.InventoryID,
							      negativeLocationStatus.SubItemID,
							      negativeLocationStatus.LocationID},
											null, SortColumnsFromNAO(), null, null, ref startRow, -1, ref totalRows))
							{
								INLocationStatus s = item;
								if (s.LocationID != null &&
									 (selectedLocations == null ||
									  selectedLocations.Contains(s.LocationID.Value)))
									NewIntermediateResult.Add(item);
							}
						}
						IntermediateResult = NewIntermediateResult;
						break;
				}

			int NextLineNbr = 1;
			bool siteLocked = false;
			bool siteLocationsAll = false;
			var locationLocked = new HashSet<int?>();

			if (IntermediateResult.Count == 0 && gs.Method != PIMethod.FullPhysicalInventory )
				updateDB = false;

			if (updateDB)
			{
				INPIHeader ph_rec = new INPIHeader();
				ReasonCode rc_rec =
					PXSelect<ReasonCode, Where<ReasonCode.reasonCodeID, Equal<Current<INSetup.pIReasonCode>>>>.Select(this);
				ph_rec.Descr = gs.Descr;
				ph_rec.SiteID = gs.SiteID;
				ph_rec.Status = INPIHdrStatus.Counting;
				if (rc_rec != null)
				{
					ph_rec.PIAdjAcctID = rc_rec.AccountID;
					ph_rec.PIAdjSubID = rc_rec.SubID;
				}
				ph_rec.TagNumbered = insetuprec.PIUseTags;
				ph_rec.TotalNbrOfTags = 0;

				ph_rec.LineCntr = 0;

				// zeroes - for the init
				ph_rec.TotalVarQty = 0m;
				ph_rec.TotalVarCost = 0m;

				// pih_rec.FinPeriodID pih_rec.TranPeriodID - ��� ������ PI

				piheader.Cache.Insert(ph_rec);
				siteLocationsAll = true;

				if (selectedLocations != null)
					foreach (INLocation location in PXSelect<INLocation,
						Where<INLocation.siteID, Equal<Current<PIGeneratorSettings.siteID>>>,
						OrderBy<Asc<INLocation.locationID>>>.Select(this))
					{
						if (location.LocationID != null &&
								selectedLocations.Contains(location.LocationID.Value))
							siteLocationsAll = false;
					}

				if (gs.Method == PIMethod.FullPhysicalInventory)
				{
					siteLocked = siteLocationsAll;
					if (siteLocationsAll)
						LockInventory(null, null);
					else
						foreach (int locationID in selectedLocations)
						{
							locationLocked.Add(locationID);
							LockInventory(locationID, null);
						}
				}

				if (gs.Method == PIMethod.ByCycle || gs.Method == PIMethod.ByMovementClass || gs.Method == PIMethod.ByABCClass || gs.Method == PIMethod.ByItemClassID)
				{
					BqlCommand cmdmissing = BqlCommand.CreateInstance(typeof(
					Select2<InventoryItem, 
						LeftJoin<INLocationStatus, On<INLocationStatus.inventoryID, Equal<InventoryItem.inventoryID>, And<INLocationStatus.siteID, Equal<Current<PIGeneratorSettings.siteID>>>>>,
					Where<InventoryItem.stkItem, Equal<True>,
						And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
						And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.markedForDeletion>,
						And<INLocationStatus.locationID, IsNull,
						And<Where<Match<InventoryItem, Current<AccessInfo.userName>>>>>>>>>));

					cmdmissing = UpdateCommandByGenerationMethod(cmdmissing);

					PXView viewmissing = new PXView(this, true, cmdmissing);

					foreach (PXResult<InventoryItem> item in viewmissing.SelectMulti())
					{
						LockInventory(null, ((InventoryItem)item).InventoryID);
					}
				}
			}

			foreach (
				PXResult<INLocationStatus, InventoryItem, INItemStats, INLotSerClass, INLotSerialStatus> it in
					IntermediateResult)
			{
				INLocationStatus ls_rec = it;
				InventoryItem ii_rec = it;
				INLotSerialStatus lss_rec = it;
				INItemStats is_rec = it;

				PIPreliminaryResult item = new PIPreliminaryResult();

				item.InventoryID = ls_rec.InventoryID;
				item.SubItemID = ls_rec.SubItemID;
				item.LocationID = ls_rec.LocationID;
				item.Descr = ii_rec.Descr;
				item.BaseUnit = ii_rec.BaseUnit;

				if (lss_rec.InventoryID != null) // nulls left-joined - non-lot-serial
				{
					item.LotSerialNbr = lss_rec.LotSerialNbr;
					item.ExpireDate = lss_rec.ExpireDate;
					item.BookQty = lss_rec.QtyOnHand - lss_rec.QtySOShipped - lss_rec.QtyINIssues;
				}
				else
				{
					item.LotSerialNbr = null;
					item.ExpireDate = null;
					item.BookQty = ls_rec.QtyOnHand - ls_rec.QtySOShipped - ls_rec.QtyINIssues;
				}

				item.LineNbr = NextLineNbr++;
				if (insetuprec.PIUseTags ?? false)
				{
					item.TagNumber = (insetuprec.PILastTagNumber ?? 0) + item.LineNbr;
				}

				if (gs.ByFrequency == true)
				{
					if (gs.Method == PIMethod.ByCycle &&
						CheckFrequency<INPICycle>(it))
						continue;

					if (gs.Method == PIMethod.ByABCClass &&
						CheckFrequency<INABCCode>(it))
						continue;

					if (gs.Method == PIMethod.ByMovementClass &&
						CheckFrequency<INMovementClass>(it))
						continue;
				}

				ResultList.Add(item);

				if (!updateDB) continue;

				// PICountStatus flag now in INLocationStatus, not INItemSite :
				//is_rec.PICountStatus = INPICountStatus.InProgress;
				//itemsite.Update(is_rec);
				//locstatus.Cache.SetStatus(ls_rec, PXEntryStatus.Updated);

				if (!siteLocked && !locationLocked.Contains(ls_rec.LocationID))
					LockInventory(
						siteLocationsAll ? null : ls_rec.LocationID,
						ls_rec.InventoryID);

				INPIHeader pih_rec = piheader.Current;

				INPIDetail pid_rec = new INPIDetail();

				pid_rec.BookQty = item.BookQty;

				//pih_rec.LineCntr++;
				if (insetuprec.PIUseTags ?? false)
				{
					pih_rec.TotalNbrOfTags++;
				}

				pid_rec.LineNbr = item.LineNbr;
				pid_rec.TagNumber = item.TagNumber;

				pid_rec.InventoryID = item.InventoryID;
				pid_rec.LocationID = item.LocationID;
				pid_rec.LotSerialNbr = item.LotSerialNbr;
				pid_rec.ExpireDate = item.ExpireDate;
				pid_rec.Status = INPIDetStatus.NotEntered;
				pid_rec.SubItemID = item.SubItemID;
				pid_rec.UnitCost = is_rec.LastCost;
				pid_rec.LineType = INPIDetLineType.Normal;

				piheader.Update(pih_rec);
				pid_rec = pidetail.Insert(pid_rec);
			}

			// blank lines generation
			for (int i = 0; i < gs.BlankLines; i++)
			{
				PIPreliminaryResult item = new PIPreliminaryResult();

				item.LineNbr = NextLineNbr++;
				item.BookQty = 0m;
				if (insetuprec.PIUseTags ?? false)
				{
					item.TagNumber = (insetuprec.PILastTagNumber ?? 0) + item.LineNbr;
				}
				ResultList.Add(item);

				if (!updateDB) continue;

				INPIHeader pih_rec = piheader.Current;

				INPIDetail pid_rec = new INPIDetail();

				pid_rec.BookQty = item.BookQty;

				// leave NULLs
				//pih_rec.TotalBookQty += item.BookQty;
				//pid_rec.UnitCost = 0m;
				//pid_rec.ExtBookCost = pid_rec.BookQty * pid_rec.UnitCost;
				//pih_rec.TotalBookCost += pid_rec.ExtBookCost;

				pih_rec.LineCntr++;
				if (insetuprec.PIUseTags ?? false)
					pih_rec.TotalNbrOfTags++;

				pid_rec.LineNbr = item.LineNbr;
				pid_rec.TagNumber = item.TagNumber;

				pid_rec.InventoryID = item.InventoryID;
				pid_rec.LocationID = item.LocationID;
				pid_rec.LotSerialNbr = item.LotSerialNbr;
				pid_rec.ExpireDate = item.ExpireDate;
				pid_rec.Status = INPIDetStatus.NotEntered;
				pid_rec.SubItemID = item.SubItemID;

				pid_rec.LineType = INPIDetLineType.Blank;

				piheader.Cache.Update(pih_rec);
				pid_rec = pidetail.Insert(pid_rec);
			}

			if (updateDB)
			{
				if (insetuprec.PIUseTags ?? false)
				{
					insetuprec.PILastTagNumber = (insetuprec.PILastTagNumber ?? 0) + NextLineNbr - 1;
					insetup.Cache.Update(insetuprec);
				}
				if (!siteLocked && 
					gs.SelectedMethod != PIInventoryMethod.ItemsHavingNegativeBookQty && 
					gs.SelectedMethod != PIInventoryMethod.RandomlySelectedItems &&
					gs.SelectedMethod != PIInventoryMethod.LastCountDate &&
					gs.ByFrequency == false)
				{
					BqlCommand cmd_items = BqlCommand.CreateInstance(
						typeof(Select2<InventoryItem,
							LeftJoin<INLocationStatus,
								  On<INLocationStatus.inventoryID, Equal<InventoryItem.inventoryID>,
								 And<INLocationStatus.inventoryID, NotEqual<InventoryItem.inventoryID>>>>,
							Where<InventoryItem.stkItem, Equal<boolTrue>,
							  And<InventoryItem.itemStatus, NotEqual<InventoryItemStatus.inactive>,
							  And<Where<Match<InventoryItem, Current<AccessInfo.userName>>>>>>>));
					cmd_items = UpdateCommandByGenerationMethod(cmd_items);
					PXView view_items = new PXView(this, true, cmd_items);
					foreach (PXResult<InventoryItem> r in
						view_items.Select(null, null, null, null, null, null, ref startRow, -1, ref totalRows))
					{
						InventoryItem i = r;
						if (selectedLocations == null || siteLocationsAll)
							LockInventory(null, i.InventoryID);
						else
							foreach (int location in selectedLocations)
								LockInventory(location, i.InventoryID);						
					}
				}

				if (saveEmpty || ResultList.Count > 0)
				{
					this.Actions.PressSave();
				}
			}
			return ResultList;
		}

		private BqlCommand UpdateCommandByGenerationMethod(BqlCommand cmd)
		{
			PIGeneratorSettings gs = GeneratorSettings.Current;
			switch (gs.Method)
			{
				case PIMethod.FullPhysicalInventory: // full physical inventory : no additional WHERE
					break;
				case PIMethod.ByInventoryItemSelected:

					if (gs.SelectedMethod == PIInventoryMethod.LastCountDate)
					{
						cmd = BqlCommand.AppendJoin<LeftJoin<LastPICountDate, // Last Count Date is needed for several methods
														On<LastPICountDate.siteID, Equal<INLocationStatus.siteID>,
													   And<LastPICountDate.inventoryID, Equal<INLocationStatus.inventoryID>,
																					 And<LastPICountDate.subItemID, Equal<INLocationStatus.subItemID>,
																					 And<LastPICountDate.locationID, Equal<INLocationStatus.locationID>>>>>>>(cmd);
						cmd = cmd.WhereAnd<Where<
							LastPICountDate.lastCountDate, IsNull,
							Or<LastPICountDate.lastCountDate, LessEqual<Current<PIGeneratorSettings.maxLastCountDate>>>
							>>();
					}

                    if (gs.SelectedMethod == PIInventoryMethod.ItemsHavingNegativeBookQty)
                    {
                        cmd = AppendMissingJoins(cmd, typeof(INLotSerialStatus), typeof(INPIStatus));
                        cmd = cmd.WhereNew<
                            Where<InventoryItem.stkItem, Equal<boolTrue>,
                                And<INLocationStatus.siteID, Equal<Current<PIGeneratorSettings.siteID>>,
                                    And<INPIStatus.recordID, IsNull,
                                        And<
                                            Where2<
                                                Where<INLotSerialStatus.inventoryID, IsNotNull,
                                                    And<Sub<Sub<INLotSerialStatus.qtyOnHand, INLotSerialStatus.qtySOShipped>, INLotSerialStatus.qtyINIssues>
                                                            , Less<decimal0>>>,
                                                Or<
                                                    Where<INLotSerialStatus.inventoryID, IsNull,
                                                        And<Where<Sub<Sub<INLocationStatus.qtyOnHand, INLocationStatus.qtySOShipped>, INLocationStatus.qtyINIssues>
                                                            , Less<decimal0>>>>
                                                    >
                                                >
                                            >
                                        >>>>();
                    }

					if (gs.SelectedMethod == PIInventoryMethod.ListOfItems)
						cmd = BqlCommand.AppendJoin<InnerJoin<INPIClassItem,
										On<INPIClassItem.pIClassID, Equal<Current<PIGeneratorSettings.pIClassID>>,
									And<INPIClassItem.inventoryID, Equal<InventoryItem.inventoryID>>>>>(cmd);
					break;
				case PIMethod.ByItemClassID:
					cmd = cmd.WhereAnd<Where<InventoryItem.itemClassID, Equal<Current<PIGeneratorSettings.itemClassID>>>>();
					break;
				case PIMethod.ByABCClass:
                    cmd = AppendMissingJoins(cmd, typeof(INItemSite));
                    if (gs.ByFrequency == true)
						cmd = BqlCommand.AppendJoin<
							InnerJoin<INABCCode,
										 On<INABCCode.aBCCodeID, Equal<INItemSite.aBCCodeID>,
										And<INABCCode.countsPerYear, IsNotNull,
										And<INABCCode.countsPerYear, NotEqual<short0>>>>,
							LeftJoin<LastPICountDate, // Last Count Date is needed for several methods
										On<LastPICountDate.siteID, Equal<INLocationStatus.siteID>,
									 And<LastPICountDate.inventoryID, Equal<INLocationStatus.inventoryID>>>>>>(cmd);
					else
						cmd = cmd.WhereAnd<Where<INItemSite.aBCCodeID, Equal<Current<PIGeneratorSettings.aBCCodeID>>>>();
					break;
				case PIMethod.ByMovementClass:
                    cmd = AppendMissingJoins(cmd, typeof(INItemSite));
                    if (gs.ByFrequency == true)
						cmd = BqlCommand.AppendJoin<
							InnerJoin<INMovementClass,
									 On<INMovementClass.movementClassID, Equal<INItemSite.movementClassID>,
									And<INMovementClass.countsPerYear, IsNotNull,
									And<INMovementClass.countsPerYear, NotEqual<short0>>>>,
						 LeftJoin<LastPICountDate, // Last Count Date is needed for several methods
									 On<LastPICountDate.siteID, Equal<INLocationStatus.siteID>,
								  And<LastPICountDate.inventoryID, Equal<INLocationStatus.inventoryID>>>>>>(cmd);
					else
						cmd = cmd.WhereAnd<Where<INItemSite.movementClassID, Equal<Current<PIGeneratorSettings.movementClassID>>>>();
					break;


				case PIMethod.ByCycle:
					if (gs.ByFrequency == true)
						cmd = BqlCommand.AppendJoin<
							InnerJoin<INPICycle,
									 On<INPICycle.cycleID, Equal<InventoryItem.cycleID>,
									And<INPICycle.countsPerYear, IsNotNull,
									And<INPICycle.countsPerYear, NotEqual<short0>>>>,
							 LeftJoin<LastPICountDate, // Last Count Date is needed for several methods
									 On<LastPICountDate.siteID, Equal<INLocationStatus.siteID>,
								  And<LastPICountDate.inventoryID, Equal<INLocationStatus.inventoryID>>>>>>(cmd);
					else
						cmd = cmd.WhereAnd<Where<InventoryItem.cycleID, Equal<Current<PIGeneratorSettings.cycleID>>>>();
					break;
			}
			return cmd;
		}

        private BqlCommand AppendMissingJoins(BqlCommand cmd, params Type[] list)
        {
            BqlCommand result = cmd;
            foreach (Type t in list)
            {
                if(t.FullName == typeof(INItemSite).FullName)
                {
                    if (!cmd.GetTables().Contains(typeof(INItemSite)))
                        result = BqlCommand.AppendJoin<InnerJoin<INItemSite,
                                                On<INItemSite.inventoryID, Equal<INLocationStatus.inventoryID>,
                                                And<INItemSite.siteID, Equal<INLocationStatus.siteID>>>>>(cmd);
                    continue;
                }
                if (t.FullName == typeof(INPIStatus).FullName)
                {
                    if (!cmd.GetTables().Contains(typeof(INPIStatus)))
                        result = BqlCommand.AppendJoin< LeftJoin<INPIStatus,
                                    On<INPIStatus.siteID, Equal<INLocationStatus.siteID>,
                                    And<INPIStatus.active, Equal<boolTrue>,
                                    And2<Where<INPIStatus.inventoryID, IsNull,
                                            Or<INPIStatus.inventoryID, Equal<INLocationStatus.inventoryID>>>,
                                        And<Where<INPIStatus.locationID, IsNull,
                                            Or<INPIStatus.locationID, Equal<INLocationStatus.locationID>>>>>>>>>(cmd);
                    continue;
                }
                if (t.FullName == typeof(INLotSerialStatus).FullName)
                {
                    if (!cmd.GetTables().Contains(typeof(INLotSerialStatus)))
                        result = BqlCommand.AppendJoin<LeftJoin<INLotSerialStatus,
										    On<INLotSerialStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
                                            And<INLotSerClass.lotSerAssign, Equal<INLotSerAssign.whenReceived>,
                                            And<INLotSerialStatus.subItemID, Equal<INLocationStatus.subItemID>,
                                            And<INLotSerialStatus.siteID, Equal<INLocationStatus.siteID>,
                                            And<INLotSerialStatus.locationID, Equal<INLocationStatus.locationID>>>>>>>>(cmd);
                    continue;
                }
            }
            return result;
        }

		private bool CheckFrequency<Cycle>(PXResult it)
			where Cycle : class, IBqlTable, new()
		{
			var r = (PXResult<INLocationStatus, InventoryItem, INItemStats, INLotSerClass,
						INLotSerialStatus, INSubItem, INLocation, INPIStatus, INItemSite, Cycle, LastPICountDate>)it;

			Cycle cycle = r;
			LastPICountDate last = r;
			if (r == null || last == null) return false;

			DateTime lastCountDate = last.LastCountDate ?? DateTime.MinValue;
			DateTime countDate = this.Accessinfo.BusinessDate.GetValueOrDefault();
			short? CountsPerYear = (short?)this.Caches[typeof(Cycle)].GetValue(cycle, typeof(INMovementClass.countsPerYear).Name);

			if (lastCountDate.Year == countDate.Year && CountsPerYear != null)
			{
				switch (CountsPerYear)
				{
					case 1:
						return true;

					case 2:
					case 3:
					case 4:
					case 6:
					case 12:
						if ((countDate.Month - lastCountDate.Month) < 12 / CountsPerYear)
							return true;
						break;
					default:
						if (countDate.DayOfYear * CountsPerYear / 365 ==
						lastCountDate.DayOfYear * CountsPerYear / 365)
							return true;
						break;
				}
			}
			return false;
		}

		protected virtual IEnumerable preliminaryResultRecs()
		{
			PXUIFieldAttribute.SetVisible<PIPreliminaryResult.tagNumber>(PreliminaryResultRecs.Cache, null, (insetup.Current.PIUseTags ?? false));
			return CalcPIRows(false);
		}


		private void SetRandomSeedIfZero()
		{
			PIGeneratorSettings gs = GeneratorSettings.Current;
			if ((gs != null) && (gs.RandomSeed == 0))
			{
				Random randObj = new Random(); // the same as Random(Environment.TickCount)
				gs.RandomSeed = randObj.Next();
			}
		}

		private void LockInventory(int? locationID, int? inventoryID)
		{
			INPIStatus status =
			inventoryID == null && locationID == null
			? PXSelectReadonly<INPIStatus,
					Where<INPIStatus.siteID, Equal<Required<INPIStatus.siteID>>,
						And<INPIStatus.active, Equal<boolTrue>>>>
					.SelectWindowed(this, 0, 1, piheader.Current.SiteID)
			: inventoryID == null
			? PXSelectReadonly<INPIStatus,
					Where<INPIStatus.siteID, Equal<Required<INPIStatus.siteID>>,
					  And<INPIStatus.active, Equal<boolTrue>,
						And<INPIStatus.locationID, IsNull,
						  Or<INPIStatus.locationID, Equal<Required<INPIStatus.locationID>>>>>>>
					.SelectWindowed(this, 0, 1, piheader.Current.SiteID, locationID)
			: PXSelectReadonly<INPIStatus,
				Where<INPIStatus.siteID, Equal<Required<INPIStatus.siteID>>,
				  And<INPIStatus.active, Equal<boolTrue>,
					And2<Where<INPIStatus.locationID, IsNull,
						Or<INPIStatus.locationID, Equal<Required<INPIStatus.locationID>>>>,
					And<Where<INPIStatus.inventoryID, IsNull,
						Or<INPIStatus.inventoryID, Equal<Required<INPIStatus.inventoryID>>>>>>>>>
				.SelectWindowed(this, 0, 1, piheader.Current.SiteID, locationID, inventoryID);

			if (status != null)
				throw new PXException(Messages.PICountInProgress, status.PIID);

			if (inventoryID != null || locationID != null)
			{
				if (this.Caches[typeof(INPIStatus)]
					.Inserted
					.Cast<INPIStatus>()
					.Any(
					rec => rec.InventoryID == inventoryID && rec.LocationID == locationID
					))
					return;
			}

			INPIStatus loc = new INPIStatus();
			loc.SiteID = piheader.Current.SiteID;
			loc.PIID = piheader.Current.PIID;
			loc.InventoryID = inventoryID;
			loc.LocationID = locationID;
			pistatus.Insert(loc);
		}

		static private string NAOCodeToFieldName(string parm)
		{
			switch (parm)
			{
				case PINumberAssignmentOrder.EmptySort:
					return null;
				case PINumberAssignmentOrder.ByLocationID:
					return "INLocation__locationCD";
				case PINumberAssignmentOrder.ByInventoryID:
					return "InventoryItem__inventoryCD";
				case PINumberAssignmentOrder.BySubItem:
					return "INSubItem__subItemCD";
				case PINumberAssignmentOrder.ByLotSerial:
					return "INLotSerialStatus__lotSerialNbr";
				case PINumberAssignmentOrder.ByInventoryDescription:
					return "InventoryItem__descr";
				default:
					throw new PXException(Messages.UnknownPiTagSortOrder);
			}
		}


		static private void AddSortColumnToListIfSet(List<string> l, string naocode)
		{
			string fieldName = NAOCodeToFieldName(naocode);
			if ((fieldName ?? "") != "") { l.Add(fieldName); }
		}

		private string[] SortColumnsFromNAO()
		{
			PIGeneratorSettings gs = GeneratorSettings.Current;
			if (gs == null) { return null; }
			INPIClass pi = (INPIClass)PXSelectorAttribute.Select<PIGeneratorSettings.pIClassID>(this.GeneratorSettings.Cache, gs);

			List<string> scl = new List<string>();
            if (pi != null)
            {
                AddSortColumnToListIfSet(scl, pi.NAO1);
                AddSortColumnToListIfSet(scl, pi.NAO2);
                AddSortColumnToListIfSet(scl, pi.NAO3);
                AddSortColumnToListIfSet(scl, pi.NAO4);
            }
            return scl.ToArray();
		}

		protected virtual void PIGeneratorSettings_SiteID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			if (e.Row == null) { return; }
			INPIClass pi = (INPIClass)PXSelectorAttribute.Select<PIGeneratorSettings.pIClassID>(sender, e.Row);
			foreach (PXResult<INPIClassLocation> l in this.locations.Select())
				this.locations.Delete(l);
			if (pi!=null && pi.SiteID == ((PIGeneratorSettings)e.Row).SiteID)
				this.locations.Cache.Clear();
		}
		protected virtual void PIGeneratorSettings_ByFrequency_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			PIGeneratorSettings row = (PIGeneratorSettings)e.Row;
			if (row == null) { return; }
			if (row.ByFrequency == true)
			{
				row.ABCCodeID = null;
				row.CycleID = null;
				row.MovementClassID = null;
			}
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
					{
						object val;
						sender.SetValuePending(row, field, ((val = source.GetValueExt(pi, field)) is PXFieldState) ? ((PXFieldState)val).Value : val);
					}
				}

			}
		}

		protected virtual void PIGeneratorSettings_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
            if (e.Row == null) return;

            PIGeneratorSettings row = (PIGeneratorSettings)e.Row;
			this.GeneratePI.SetEnabled(row.Method != null && row.SiteID != null);

			INPIClass pi = (INPIClass)PXSelectorAttribute.Select<PIGeneratorSettings.pIClassID>(sender, row);
			if (pi != null)
			{
				PXCache source = this.Caches[typeof(INPIClass)];
				foreach (string field in sender.Fields)
				{
					if (string.Compare(field, typeof(PIGeneratorSettings.pIClassID).Name, true) == 0 ||
							string.Compare(field, typeof(PIGeneratorSettings.descr).Name, true) == 0 ||
							!source.Fields.Contains(field)) continue;

					object value = source.GetValue(pi, field);
					INPIClass def = new INPIClass();
					object defValue;
					source.RaiseFieldDefaulting(field, def, out defValue);
					PXUIFieldAttribute.SetEnabled(sender, field, value == null || object.Equals(value, defValue));
				}
			}
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.selectedMethod>(sender, row, row.Method == PIMethod.ByInventoryItemSelected);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.randomItemsLimit>(sender, row,
																				row.Method == PIMethod.ByInventoryItemSelected &&
																				row.SelectedMethod == PIInventoryMethod.RandomlySelectedItems);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.lastCountPeriod>(sender, row,
																																		row.Method == PIMethod.ByInventoryItemSelected &&
																																		row.SelectedMethod == PIInventoryMethod.LastCountDate);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.maxLastCountDate>(sender, row,
																	row.Method == PIMethod.ByInventoryItemSelected &&
																	row.SelectedMethod == PIInventoryMethod.LastCountDate);

			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.aBCCodeID>(sender, row, row.Method == PIMethod.ByABCClass);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.movementClassID>(sender, row, row.Method == PIMethod.ByMovementClass);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.cycleID>(sender, row, row.Method == PIMethod.ByCycle);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.itemClassID>(sender, row, row.Method == PIMethod.ByItemClassID);
			PXUIFieldAttribute.SetVisible<PIGeneratorSettings.byFrequency>(sender, row,
						row.Method == PIMethod.ByABCClass ||
						row.Method == PIMethod.ByMovementClass ||
						row.Method == PIMethod.ByCycle);

			PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.byFrequency>(sender, row,
					pi != null && pi.CycleID == null && pi.MovementClassID == null && pi.ABCCodeID == null);

			if (pi != null && pi.CycleID == null && pi.MovementClassID == null && pi.ABCCodeID == null)
			{
				PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.aBCCodeID>(sender, row, row.ByFrequency == false);
				PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.movementClassID>(sender, row, row.ByFrequency == false);
				PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.cycleID>(sender, row, row.ByFrequency == false);
			}
			PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.method>(sender, row, false);
			PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.selectedMethod>(sender, row, false);
			PXUIFieldAttribute.SetEnabled<PIGeneratorSettings.maxLastCountDate>(sender, row,
					row.LastCountPeriod == null || row.LastCountPeriod == 0);

			if (row.LastCountPeriod != null && row.LastCountPeriod != 0)
			{
				row.MaxLastCountDate = this.Accessinfo.BusinessDate.Value.AddDays(-row.LastCountPeriod.GetValueOrDefault());
			}
		}

		protected virtual void INSetup_RowPersisting(PXCache cache, PXRowPersistingEventArgs e)
		{
			INSetup row = (INSetup)e.Row;
			if (row == null) return;

			PXDefaultAttribute.SetPersistingCheck<INSetup.iNTransitAcctID>(cache, e.Row, PXAccess.FeatureInstalled<FeaturesSet.warehouse>() ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
			PXDefaultAttribute.SetPersistingCheck<INSetup.iNTransitSubID>(cache, e.Row, PXAccess.FeatureInstalled<FeaturesSet.warehouse>() ? PXPersistingCheck.Null : PXPersistingCheck.Nothing);
		}		
	}


	#region # Assignment Order

	public class PINumberAssignmentOrder
	{
		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { EmptySort, ByLocationID, ByInventoryID, BySubItem, ByLotSerial, ByInventoryDescription },
				new string[] { "-", Messages.ByLocationID, Messages.ByInventoryID, Messages.BySubItem, Messages.ByLotSerial, Messages.ByInventoryDescription })
			{ }
		}

		public const string EmptySort = "ES";
		public const string ByLocationID = "LI";
		public const string ByInventoryID = "II";
		public const string BySubItem = "SI";
		public const string ByLotSerial = "LS";
		public const string ByInventoryDescription = "ID";

		public class emptySort : Constant<string>
		{
			public emptySort() : base(EmptySort) { }
		}

		public class byLocationID : Constant<string>
		{
			public byLocationID() : base(ByLocationID) { }
		}

		public class byInventoryID : Constant<string>
		{
			public byInventoryID() : base(ByInventoryID) { }
		}

		public class bySubItem : Constant<string>
		{
			public bySubItem() : base(BySubItem) { }
		}
		public class byLotSerial : Constant<string>
		{
			public byLotSerial() : base(ByLotSerial) { }
		}

		public class byInventoryDescription : Constant<string>
		{
			public byInventoryDescription() : base(ByInventoryDescription) { }
		}
	}

	#endregion # Assignment Order

	public static class PIGenerationMethod
	{
		public const string FullPhysicalInventory = "FPI";
		public const string ByMovementClassCountFrequency = "MCF";
		public const string ByABCClassCountFrequency = "ACF";
		public const string ByCycleID = "BCI";
		public const string ByCycleCountFrequency = "CCF";
		public const string LastCountDate = "LCD";
		public const string ByPreviousPIID = "PPI";
		public const string ByItemClassID = "BIC";
		public const string ListOfItems = "LOI";
		public const string RandomlySelectedItems = "RSI";
		public const string ItemsHavingNegativeBookQty = "HNQ";

		public class ListAttribute : PXStringListAttribute
		{
			public ListAttribute()
				: base(
				new string[] { FullPhysicalInventory, ByCycleCountFrequency, ByMovementClassCountFrequency, ByABCClassCountFrequency, ByCycleID, LastCountDate, ByPreviousPIID, ByItemClassID, ListOfItems, RandomlySelectedItems, ItemsHavingNegativeBookQty },
				new string[] { Messages.FullPhysicalInventory, Messages.ByCycleCountFrequency, Messages.ByMovementClassCountFrequency, Messages.ByABCClassCountFrequency, Messages.ByCycleID, Messages.LastCountDate, Messages.ByPreviousPIID, Messages.ByItemClassID, Messages.ListOfItems, Messages.RandomlySelectedItems, Messages.ItemsHavingNegativeBookQty }) { }
		}
	}

	#region Projections

	#region LastPICountDate projection
	// projection to calc last max. CountDate for site-inventory combination
	/*
		SELECT 
	 		h.SiteID,
	 		d.InventoryID, 
	 		MAX (h.PICountDate)
		FROM
					INPIHeader h
	 		JOIN	INPIDetail d ON d.PIID = h.PIID
		WHERE
					h.Status = 'C' -- Completed
			AND		d.Status = 'E' -- Entered	
		GROUP BY 
	 		d.InventoryID, 
	 		h.SiteID
	  
	*/

	[PXProjection(typeof(Select5<INPIHeader,
		InnerJoin<INPIDetail,
			On<INPIDetail.pIID, Equal<INPIHeader.pIID>>>,
		Where<INPIHeader.status, Equal<INPIHdrStatus.completed>,
			And<INPIDetail.status, Equal<INPIDetStatus.entered>>>,
		Aggregate<GroupBy<INPIHeader.siteID,
			GroupBy<INPIDetail.inventoryID,
			GroupBy<INPIDetail.subItemID,
			GroupBy<INPIDetail.locationID,
			Max<INPIHeader.countDate>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class LastPICountDate : PX.Data.IBqlTable
	{
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site(BqlField = typeof(INPIHeader.siteID))]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
			Where<InventoryItem.inventoryID, Equal<Current<INPIDetail.inventoryID>>,
			And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>))]
		[SubItem(typeof(INPIDetail.inventoryID), BqlField = typeof(INPIDetail.subItemID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region LocationID
		public abstract class locationID : PX.Data.IBqlField
		{
		}
		protected Int32? _LocationID;
		[Location(typeof(INPIHeader.siteID), Visibility = PXUIVisibility.SelectorVisible, BqlField = typeof(INPIDetail.locationID))]
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
		#region CountDate
		public abstract class lastCountDate : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastCountDate;
		[PXDBDate(BqlField = typeof(INPIHeader.countDate))]
		public virtual DateTime? LastCountDate
		{
			get
			{
				return this._LastCountDate;
			}
			set
			{
				this._LastCountDate = value;
			}
		}
		#endregion
	}

	#endregion

	#region InventoryByPIID projection
	// projection to get list of items for PI ID
	/*
		SELECT 
			h.PIID,	
	 		d.InventoryID
		FROM
					INPIHeader h
	 		JOIN	INPIDetail d ON d.PIID = h.PIID
		WHERE
					h.Status = 'C' -- Completed
			AND		d.Status = 'E' -- Entered	
		GROUP BY 
	 		h.PIID,	 
	 		d.InventoryID	  
	*/

	[PXProjection(typeof(Select5<INPIHeader,
		InnerJoin<INPIDetail,
			On<INPIDetail.pIID, Equal<INPIHeader.pIID>>>,
		Where<INPIHeader.status, Equal<INPIHdrStatus.completed>,
			And<INPIDetail.status, Equal<INPIDetStatus.entered>>>,
		Aggregate<
			GroupBy<INPIHeader.pIID,
			GroupBy<INPIDetail.inventoryID>>>>))]
    [Serializable]
    [PXHidden]
	public partial class InventoryByPIID : PX.Data.IBqlTable
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

	#region ItemSiteHavingNegativeBookQtys projection
	//Item-Site Having Negative BookQtys projection (on some location / lotserial inside the Site)
	// SELECT similar to the main SELECT in delegate but with filtering by (QtyOnHand-QtyShipNotInv) < 0 and grouping by (SiteID, InventoryID)	  

	[PXProjection(typeof(Select5<INLocationStatus,
		InnerJoin<InventoryItem,
			On<InventoryItem.inventoryID, Equal<INLocationStatus.inventoryID>>,
		LeftJoin<INLotSerClass,
			On<INLotSerClass.lotSerClassID, Equal<InventoryItem.lotSerClassID>>,
		LeftJoin<INLotSerialStatus,
			On<INLotSerialStatus.inventoryID, Equal<INLocationStatus.inventoryID>,
				And<INLotSerClass.lotSerAssign, Equal<INLotSerAssign.whenReceived>,
				And<INLotSerialStatus.subItemID, Equal<INLocationStatus.subItemID>,
				And<INLotSerialStatus.siteID, Equal<INLocationStatus.siteID>,
				And<INLotSerialStatus.locationID, Equal<INLocationStatus.locationID>>>>>>>>>,
		Where2<
				Where<
							INLotSerialStatus.inventoryID, IsNotNull,
					And<Sub<Sub<INLotSerialStatus.qtyOnHand, INLotSerialStatus.qtySOShipped>, INLotSerialStatus.qtyINIssues>, Less<decimal0>>
				>,
			Or<
				Where<
							INLotSerialStatus.inventoryID, IsNull,
					And<Sub<Sub<INLocationStatus.qtyOnHand, INLocationStatus.qtySOShipped>, INLocationStatus.qtyINIssues>, Less<decimal0>>
				>
			>>,
		Aggregate<GroupBy<INLocationStatus.siteID,
			 GroupBy<INLocationStatus.inventoryID>>>>))]

    [Serializable]
    [PXHidden]
	public partial class ItemSiteHavingNegativeBookQtys : PX.Data.IBqlTable
	{
		#region SiteID
		public abstract class siteID : PX.Data.IBqlField
		{
		}
		protected Int32? _SiteID;
		[Site(BqlField = typeof(INLocationStatus.siteID))]
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
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(BqlField = typeof(INLocationStatus.inventoryID))]
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

	#region CostStatusToJoin projection
	// projection to get cost information and not to exceed 10 joined table limitation
	/*
		SELECT 
	 		csc.InventoryID,
			sixr.SubItemID,
			csc.CostSiteID,
			csc.LotSerialNbr,
			csc.TotalCost,  
			csc.QtyOnHand
		FROM	 
					CostStatusConsolidated csc
	 		JOIN	INSubItemXRef sixr ON sixr.SubItemID = csc.SubItemID
	 
	  
	*/

	[PXProjection(typeof(Select2<CostStatusConsolidated,
		InnerJoin<INCostSubItemXRef,
			On<INCostSubItemXRef.costSubItemID, Equal<CostStatusConsolidated.costSubItemID>>>>))]
    [Serializable]
    [PXHidden]
	public partial class CostStatusToJoin : PX.Data.IBqlTable
	{
		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(BqlField = typeof(CostStatusConsolidated.inventoryID))]
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
		#region SubItemID
		public abstract class subItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _SubItemID;
		[PXDBInt(BqlField = typeof(INCostSubItemXRef.subItemID))]
		public virtual Int32? SubItemID
		{
			get
			{
				return this._SubItemID;
			}
			set
			{
				this._SubItemID = value;
			}
		}
		#endregion
		#region CostSiteID
		public abstract class costSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSiteID;
		[PXDBInt(BqlField = typeof(CostStatusConsolidated.costSiteID))]
		public virtual Int32? CostSiteID
		{
			get
			{
				return this._CostSiteID;
			}
			set
			{
				this._CostSiteID = value;
			}
		}
		#endregion
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true, BqlField = typeof(CostStatusConsolidated.lotSerialNbr))]
		public virtual String LotSerialNbr
		{
			get
			{
				return this._LotSerialNbr;
			}
			set
			{
				this._LotSerialNbr = value;
			}
		}
		#endregion
		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand;
		[PXDBQuantity(BqlField = typeof(CostStatusConsolidated.qtyOnHand))]
		public virtual Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion
		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalCost;
		[PXDBBaseCury(BqlField = typeof(CostStatusConsolidated.totalCost))]
		public virtual Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
	}

	#endregion

	#region CostStatusConsolidated projection
	// projection to get cost information and not to exceed 10 joined table limitation
	/*
		SELECT 
	 		cs.InventoryID,
			cs.CostSubItemID,
			cs.CostSiteID,
			cs.LotSerialNbr,
			SUM (cs.TotalCost),  
			SUM (cs.QtyOnHand)
		FROM 
	 		INCostStatus cs	 
	  
	*/

	[PXProjection(typeof(Select4<INCostStatus,
		Aggregate<GroupBy<INCostStatus.inventoryID,
			GroupBy<INCostStatus.costSubItemID,
			GroupBy<INCostStatus.costSiteID,
			GroupBy<INCostStatus.lotSerialNbr,
			Sum<INCostStatus.qtyOnHand,
			Sum<INCostStatus.totalCost>>>>>>>>))]
    [Serializable]
    [PXHidden]
	public partial class CostStatusConsolidated : PX.Data.IBqlTable
	{

		#region InventoryID
		public abstract class inventoryID : PX.Data.IBqlField
		{
		}
		protected Int32? _InventoryID;
		[Inventory(BqlField = typeof(INCostStatus.inventoryID))]
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
		#region CostSubItemID
		public abstract class costSubItemID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSubItemID;
		[PXDBInt(BqlField = typeof(INCostStatus.costSubItemID))]
		public virtual Int32? CostSubItemID
		{
			get
			{
				return this._CostSubItemID;
			}
			set
			{
				this._CostSubItemID = value;
			}
		}
		#endregion
		#region CostSiteID
		public abstract class costSiteID : PX.Data.IBqlField
		{
		}
		protected Int32? _CostSiteID;
		[PXDBInt(BqlField = typeof(INCostStatus.costSiteID))]
		public virtual Int32? CostSiteID
		{
			get
			{
				return this._CostSiteID;
			}
			set
			{
				this._CostSiteID = value;
			}
		}
		#endregion
		#region LotSerialNbr
		public abstract class lotSerialNbr : PX.Data.IBqlField
		{
		}
		protected String _LotSerialNbr;
		[PXDBString(100, IsUnicode = true, BqlField = typeof(INCostStatus.lotSerialNbr))]
		public virtual String LotSerialNbr
		{
			get
			{
				return this._LotSerialNbr;
			}
			set
			{
				this._LotSerialNbr = value;
			}
		}
		#endregion
		#region QtyOnHand
		public abstract class qtyOnHand : PX.Data.IBqlField
		{
		}
		protected Decimal? _QtyOnHand;
		[PXDBQuantity(BqlField = typeof(INCostStatus.qtyOnHand))]
		public virtual Decimal? QtyOnHand
		{
			get
			{
				return this._QtyOnHand;
			}
			set
			{
				this._QtyOnHand = value;
			}
		}
		#endregion
		#region TotalCost
		public abstract class totalCost : PX.Data.IBqlField
		{
		}
		protected Decimal? _TotalCost;
		[PXDBBaseCury(BqlField = typeof(INCostStatus.totalCost))]
		public virtual Decimal? TotalCost
		{
			get
			{
				return this._TotalCost;
			}
			set
			{
				this._TotalCost = value;
			}
		}
		#endregion
	}
	#endregion

	#endregion Projections
}
