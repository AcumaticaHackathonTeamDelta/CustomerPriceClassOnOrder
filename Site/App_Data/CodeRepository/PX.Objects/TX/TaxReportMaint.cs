using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CR;

namespace PX.Objects.TX
{
	[System.SerializableAttribute()]
	[PXCacheName(Messages.VendorMaster)]
	public partial class VendorMaster : Vendor
	{
		#region BAccountID
		public new abstract class bAccountID : PX.Data.IBqlField
		{
		}
		[Vendor(typeof(Search<Vendor.bAccountID, Where<Vendor.taxAgency,Equal<True>>>), IsKey=true, DisplayName = "Tax Agency ID", DescriptionField=typeof(Vendor.acctName))]
		public override Int32? BAccountID
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
		#region AcctCD
		public new abstract class acctCD : PX.Data.IBqlField
		{
		}
		[PXDefault()]
		[VendorRaw(IsKey = false)]
		[PX.Data.EP.PXFieldDescription]
		public override String AcctCD
		{
			get
			{
				return this._AcctCD;
			}
			set
			{
				this._AcctCD = value;
			}
		}
		#endregion
		#region TaxAgency
		public new abstract class taxAgency : PX.Data.IBqlField
		{
		}
		#endregion
		#region ShowNoTemp
		public abstract class showNoTemp : PX.Data.IBqlField
		{
		}
		protected Boolean? _ShowNoTemp = false;
		[PXBool()]
		[PXUIField(DisplayName = "Show Tax Zones")]
		public virtual Boolean? ShowNoTemp
		{
			get
			{
				return this._ShowNoTemp;
			}
			set
			{
				this._ShowNoTemp = value;
			}
		}
		#endregion
	}

	public class TaxReportMaint : PXGraph<TaxReportMaint>
	{
        #region Cache Attached Events
        #region TaxBucket
        #region VendorID

        [PXDBInt(IsKey = true)]
        [PXDefault(typeof(VendorMaster.bAccountID))]        
        protected virtual void TaxBucket_VendorID_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #region BucketID

        [PXDBInt(IsKey = true)]
		[PXLineNbr(typeof(VendorMaster))]
		[PXParent(typeof(Select<VendorMaster, Where<VendorMaster.bAccountID, Equal<Current<TaxBucket.vendorID>>>>), LeaveChildren = true)]
        [PXUIField(DisplayName = "Reporting Group", Visibility = PXUIVisibility.Visible)]
        protected virtual void TaxBucket_BucketID_CacheAttached(PXCache sender)
        {
        }
        #endregion       
        
        #endregion
        #endregion

        public PXSave<VendorMaster> Save;
		public PXCancel<VendorMaster> Cancel;
		public PXFirst<VendorMaster> First;
		public PXPrevious<VendorMaster> Prev;
		public PXNext<VendorMaster> Next;
		public PXLast<VendorMaster> Last;

		public PXSelect<VendorMaster, Where<VendorMaster.taxAgency, Equal<boolTrue>>> TaxVendor;
		public PXSelect<TaxReportLine, Where<TaxReportLine.vendorID, Equal<Current<VendorMaster.bAccountID>>, And<Where<Current<VendorMaster.showNoTemp>, Equal<boolFalse>, And<TaxReportLine.tempLineNbr, IsNull, Or<Current<VendorMaster.showNoTemp>, Equal<boolTrue>, And<TaxReportLine.tempLineNbr, IsNotNull>>>>>>> ReportLine;
		public PXSelect<TaxBucket, Where<TaxBucket.vendorID, Equal<Current<VendorMaster.bAccountID>>>> Bucket;
		public PXSelect<TaxBucketLine, Where<TaxBucketLine.vendorID, Equal<Required<TaxReportLine.vendorID>>, And<TaxBucketLine.lineNbr, Equal<Required<TaxBucketLine.lineNbr>>>>> TaxBucketLine_Vendor_LineNbr;

        protected IEnumerable reportLine()
        {
			if (this.TaxVendor.Current.BAccountID == null) yield break;
            bool showTaxZones = TaxVendor.Current.ShowNoTemp == true;
			TaxBucketAnalizer AnalyzerTax = new TaxBucketAnalizer(this, (int)this.TaxVendor.Current.BAccountID, TaxReportLineType.TaxAmount);
			Dictionary<int, List<int>> taxBucketsDict = AnalyzerTax.AnalyzeBuckets(showTaxZones);
			TaxBucketAnalizer TestAnalyzerTaxable = new TaxBucketAnalizer(this, (int)this.TaxVendor.Current.BAccountID, TaxReportLineType.TaxableAmount);
			Dictionary<int, List<int>> taxableBucketsDict = TestAnalyzerTaxable.AnalyzeBuckets(showTaxZones);
            Dictionary<int, List<int>>[] bucketsArr = { taxBucketsDict, taxableBucketsDict };
            StringBuilder sb = new StringBuilder();
            foreach (TaxReportLine taxline in PXSelect<TaxReportLine, Where<TaxReportLine.vendorID, Equal<Current<VendorMaster.bAccountID>>, And<Where<Current<VendorMaster.showNoTemp>, Equal<False>, And<TaxReportLine.tempLineNbr, IsNull, Or<Current<VendorMaster.showNoTemp>, Equal<True>, And<TaxReportLine.tempLineNbr, IsNotNull>>>>>>>.Select(this))
            {
                int linenbr = (int)taxline.LineNbr;
                foreach (var BucketsDict in bucketsArr)
                {
                    if (BucketsDict != null && BucketsDict.ContainsKey(linenbr))
                    {
                        sb.Clear();
                        for(int i = 0; i < BucketsDict[linenbr].Count; i++)
                        {
                            if (i == 0)
                            {
                                sb.Append(BucketsDict[linenbr][i]);
                            }
                            else
                            {
                                sb.AppendFormat("+{0}", BucketsDict[linenbr][i]);
                            }
                        }
                        taxline.BucketSum = sb.ToString();
                    }
                }
                yield return taxline;
            }
        }

		public const string TAG_TAXZONE = "<TAXZONE>";

		protected virtual void VendorMaster_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			e.Cancel = true;
		}

		public PXAction<VendorMaster> newVendor;
		[PXUIField(DisplayName = Messages.NewVendor, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable NewVendor(PXAdapter adapter)
		{
			VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();
			throw new PXRedirectRequiredException(graph, Messages.NewVendor);
		}

		public PXAction<VendorMaster> editVendor;
		[PXUIField(DisplayName = Messages.EditVendor, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable EditVendor(PXAdapter adapter)
		{
			if (TaxVendor.Current != null)
			{
					VendorMaint graph = PXGraph.CreateInstance<VendorMaint>();

					string strAcctCD = (string) TaxVendor.Cache.GetValue<VendorMaster.acctCD>(TaxVendor.Current);
					graph.BAccount.Current = (VendorR) graph.BAccount.Search<Vendor.acctCD>(strAcctCD);
					throw new PXRedirectRequiredException(graph, Messages.EditVendor);
			}
			return adapter.Get();
		}

		public PXAction<VendorMaster> viewGroupDetails;
		[PXUIField(DisplayName = Messages.ViewGroupDetails, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
		[PXLookupButton]
		public virtual IEnumerable ViewGroupDetails(PXAdapter adapter)
		{
			if (TaxVendor.Current != null && Bucket.Current != null)
			{
				TaxBucketMaint graph = CreateInstance<TaxBucketMaint>();
				graph.Bucket.Current.VendorID = TaxVendor.Current.BAccountID;
				graph.Bucket.Current.BucketID = Bucket.Current.BucketID;
				graph.Bucket.Current.BucketType = Bucket.Current.BucketType;
				throw new PXRedirectRequiredException(graph, Messages.ViewGroupDetails);
			}
			return adapter.Get();
		}

		public PXAction<VendorMaster> updateTaxZoneLines;

		[PXUIField(DisplayName = Messages.CreateReportLinesForNewTaxZones, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
		[PXButton]
		public virtual IEnumerable UpdateTaxZoneLines(PXAdapter adapter)
		{
			var vendor = TaxVendor.Current;

			if (vendor == null)
				return adapter.Get();

			var templateLines = PXSelect<TaxReportLine,
				Where<TaxReportLine.vendorID, Equal<Required<TaxReportLine.vendorID>>,
					And<TaxReportLine.tempLine, Equal<True>>>>.Select(this, vendor.BAccountID).RowCast<TaxReportLine>();

			if (templateLines.Any() == false)
				throw new PXException(Messages.NoLinesByTaxZone);

			bool addedSome = false;

			foreach (var template in templateLines)
			{
				foreach (TaxZone zone in PXSelectJoin<TaxZone, LeftJoin<TaxReportLine,
					On<TaxReportLine.taxZoneID, Equal<TaxZone.taxZoneID>,
					And<TaxReportLine.tempLineNbr, Equal<Required<TaxReportLine.lineNbr>>,
					And<TaxReportLine.vendorID, Equal<Required<TaxReportLine.vendorID>>>>>>,
					Where<TaxReportLine.lineNbr, IsNull>>.Select(this, template.LineNbr, vendor.BAccountID))
				{
					var child = CreateChildLine(template, zone);
					ReportLine.Cache.Insert(child);

					addedSome = true;
				}
			}

			if (addedSome == false)
				throw new PXException(Messages.NoNewZonesLoaded);

			return adapter.Get();
		}

		public class TaxBucketAnalizer
		{
			private Dictionary<int, List<int>> _bucketsLinesAggregates;
			private Dictionary<int, List<int>> _bucketsLinesAggregatesSorted;
			private Dictionary<int, List<int>> _bucketsDict;
			private Dictionary<int, int> _bucketLinesOccurence;
			private Dictionary<int, Dictionary<int, int>> _bucketsLinesPairs;
			private int _bAccountID;
			private string _taxLineType;
			private PXGraph _graph;

			private PXSelectJoin<TaxBucketLine, LeftJoin<TaxReportLine,
				On<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>, And<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>>>>,
				Where<TaxBucketLine.vendorID, Equal<Required<TaxBucketLine.vendorID>>, And<TaxReportLine.lineType, Equal<Required<TaxReportLine.lineType>>>>> _vendorBucketLines;

			public Func<TaxReportLine, bool> showTaxReportLine
			{
				get; set;
			}

			private IEnumerable<PXResult<TaxBucketLine>> selectBucketLines(int VendorId, string LineType)
			{
				return _vendorBucketLines.Select(VendorId, LineType).Where(set => showTaxReportLine(set.GetItem<TaxReportLine>()));
			}

			public TaxBucketAnalizer(PXGraph graph, int BAccountID, string TaxLineType)
			{
				_bAccountID = BAccountID;
				_taxLineType = TaxLineType;
				_graph = graph;
				_vendorBucketLines = new PXSelectJoin<TaxBucketLine, LeftJoin<TaxReportLine,
					On<TaxBucketLine.lineNbr, Equal<TaxReportLine.lineNbr>, And<TaxBucketLine.vendorID, Equal<TaxReportLine.vendorID>>>>,
					Where<TaxBucketLine.vendorID, Equal<Required<TaxBucketLine.vendorID>>, And<TaxReportLine.lineType, Equal<Required<TaxReportLine.lineType>>>>>(_graph);
				showTaxReportLine = (line) => true;
			}

			public Dictionary<int, List<int>> AnalyzeBuckets(bool CalcWithZones)
			{
				calcOccurances(CalcWithZones);
				fillAgregates();
				return _bucketsLinesAggregatesSorted;
			}

			public void DoChecks(int BucketID)
			{
				if (_bucketsDict == null)
				{
					calcOccurances(true);
					fillAgregates();
				}
				doChecks(BucketID);
			}

			#region Public Static functions

			public static void CheckTaxAgencySettings(PXGraph graph, int BAccountID)
			{
				PXResultset<TaxBucket> buckets = PXSelect<TaxBucket, Where<TaxBucket.vendorID, Equal<Required<TaxBucket.vendorID>>>>.Select(graph, BAccountID);
				if (buckets == null) return;
				TaxBucketAnalizer taxAnalizer = new TaxBucketAnalizer(graph, BAccountID, TaxReportLineType.TaxAmount);
				TaxBucketAnalizer taxableAnalizer = new TaxBucketAnalizer(graph, BAccountID, TaxReportLineType.TaxableAmount);
				foreach (TaxBucket bucket in buckets)
				{
					taxAnalizer.DoChecks((int) bucket.BucketID);
					taxableAnalizer.DoChecks((int) bucket.BucketID);
				}
			}

			public static Dictionary<int, int> TransposeDictionary(Dictionary<int, List<int>> oldDict)
			{
				if (oldDict == null)
				{
					return null;
				}
				Dictionary<int, int> newDict = new Dictionary<int, int>();
				foreach (KeyValuePair<int, List<int>> kvp in oldDict)
				{
					foreach (int val in kvp.Value)
					{
						newDict[val] = kvp.Key;
					}
				}
				return newDict;
			}

			public static bool IsSubList(List<int> SearchList, List<int> SubList)
			{
				if (SubList.Count > SearchList.Count)
				{
					return false;
				}
				for (int i = 0; i < SubList.Count; i++)
				{
					if (!SearchList.Contains(SubList[i]))
					{
						return false;
					}
				}
				return true;
			}

			public static List<int> SubstList(List<int> SearchList, List<int> SubstList, int SubstVal)
			{
				if (!IsSubList(SearchList, SubstList))
				{
					return SearchList;
				}
				List<int> resList = new List<int>();
				resList.AddRange(SearchList);
				foreach (int val in SubstList)
				{
					resList.Remove(val);
				}
				resList.Add(SubstVal);
				return resList;
			}

			#endregion

			#region Private Methods

			private void calcOccurances(bool CalcWithZones)
			{
				if (!CalcWithZones)
				{
					_vendorBucketLines.WhereAnd<Where<TaxReportLine.tempLineNbr, IsNull>>();
				}
				IEnumerable<PXResult<TaxBucketLine>> BucketLineTaxAmt = selectBucketLines(_bAccountID, _taxLineType);
				if (BucketLineTaxAmt == null)
				{
					_bucketsDict = null;
					return;
				}
				_bucketsDict = new Dictionary<int, List<int>>();
				foreach (PXResult<TaxBucketLine> bucketLineSet in BucketLineTaxAmt)
				{
					TaxBucketLine bucketLine = (TaxBucketLine) bucketLineSet[typeof (TaxBucketLine)];
					TaxReportLine reportLine = (TaxReportLine) bucketLineSet[typeof (TaxReportLine)];
					if (bucketLine.BucketID != null && reportLine.LineNbr != null)
					{
						if (!_bucketsDict.ContainsKey((int) bucketLine.BucketID))
						{
							_bucketsDict[(int) bucketLine.BucketID] = new List<int>();
						}
						_bucketsDict[(int) bucketLine.BucketID].Add((int) bucketLine.LineNbr);
					}
				}
				List<int> bucketsList = _bucketsDict.Keys.ToList<int>();
				for (int i = 0; i < bucketsList.Count; i++)
				{
					for (int j = i + 1; j < bucketsList.Count; j++)
					{
						if (_bucketsDict[bucketsList[i]].Count == _bucketsDict[bucketsList[j]].Count
						    && IsSubList(_bucketsDict[bucketsList[i]], _bucketsDict[bucketsList[j]]))
						{
							_bucketsDict.Remove(bucketsList[i]);
							break;
						}
					}
				}
				_bucketLinesOccurence = new Dictionary<int, int>();
				_bucketsLinesPairs = new Dictionary<int, Dictionary<int, int>>();
				foreach (KeyValuePair<int, List<int>> kvp in _bucketsDict)
				{
					foreach (int lineNbr in kvp.Value)
					{
						if (!_bucketLinesOccurence.ContainsKey(lineNbr))
						{
							_bucketLinesOccurence[lineNbr] = 0;
						}
						_bucketLinesOccurence[lineNbr]++;
					}
					for (int i = 0; i < kvp.Value.Count - 1; i++)
					{
						for (int j = i + 1; j < kvp.Value.Count; j++)
						{
							int key;
							int value;
							if (kvp.Value[i] < kvp.Value[j])
							{
								key = kvp.Value[i];
								value = kvp.Value[j];
							}
							else
							{
								key = kvp.Value[j];
								value = kvp.Value[i];
							}
							if (!_bucketsLinesPairs.ContainsKey(key))
							{
								_bucketsLinesPairs[key] = new Dictionary<int, int>();
							}
							if (!_bucketsLinesPairs[key].ContainsKey(value))
							{
								_bucketsLinesPairs[key][value] = 0;
							}
							_bucketsLinesPairs[key][value]++;
						}
					}
				}
			}

			private void fillAgregates()
			{
				if (_bucketsDict == null || _bucketLinesOccurence == null || _bucketsLinesPairs == null) return;
				_bucketsLinesAggregates = new Dictionary<int, List<int>>();
				foreach (KeyValuePair<int, Dictionary<int, int>> kvp in _bucketsLinesPairs)
				{
					foreach (KeyValuePair<int, int> innerkvp in kvp.Value)
					{
						if (innerkvp.Value == 1)
						{
							int keyOccurence = _bucketLinesOccurence[kvp.Key];
							int valOccurence = _bucketLinesOccurence[innerkvp.Key];
							int aggregate = 0;
							int standAloneVal = 0;
							if (keyOccurence != valOccurence)
							{
								if (keyOccurence > valOccurence)
								{
									aggregate = kvp.Key;
									standAloneVal = innerkvp.Key;
								}
								else
								{
									aggregate = innerkvp.Key;
									standAloneVal = kvp.Key;
								}
							}
							if (aggregate != 0)
							{
								if (!_bucketsLinesAggregates.ContainsKey(aggregate))
								{
									_bucketsLinesAggregates[aggregate] = new List<int>();
								}
								_bucketsLinesAggregates[aggregate].Add(standAloneVal);
							}
						}
					}
				}
				List<KeyValuePair<int, List<int>>> sortedAggregatesList = _bucketsLinesAggregates.ToList();
				sortedAggregatesList.Sort((firstPair, nextPair) => firstPair.Value.Count - nextPair.Value.Count == 0 ?
					                                                   firstPair.Key - nextPair.Key : firstPair.Value.Count - nextPair.Value.Count);
				for (int i = 0; i < sortedAggregatesList.Count; i++)
				{
					for (int j = i + 1; j < sortedAggregatesList.Count; j++)
					{
						List<int> newList = SubstList(sortedAggregatesList[j].Value, sortedAggregatesList[i].Value, sortedAggregatesList[i].Key);
						if (newList != sortedAggregatesList[j].Value)
						{
							sortedAggregatesList[j].Value.Clear();
							sortedAggregatesList[j].Value.AddRange(newList);
						}
					}
				}
				_bucketsLinesAggregatesSorted = new Dictionary<int, List<int>>();
				foreach (KeyValuePair<int, List<int>> kvp in sortedAggregatesList)
				{
					kvp.Value.Sort();
					_bucketsLinesAggregatesSorted[kvp.Key] = kvp.Value;
				}
			}

			private void doChecks(int BucketID)
			{
				if (_bucketsLinesAggregatesSorted == null)
				{
					throw new PXException("Unexpected call!");
				}
				int standaloneLinesCount = 0;
				int aggrergateLinesCount = 0;
				if (_bucketsDict.ContainsKey(BucketID))
				{
					foreach (int line in _bucketsDict[BucketID])
					{
						//can only be 1 or more
						if (_bucketsLinesAggregatesSorted.ContainsKey(line))
						{
							aggrergateLinesCount++;
						}
						else
						{
							standaloneLinesCount++;
						}
					}
					if (aggrergateLinesCount > 0 && standaloneLinesCount == 0)
					{
						throw new PXSetPropertyException(Messages.BucketContainsOnlyAggregateLines, PXErrorLevel.Error, BucketID.ToString());
					}
				}
			}

			#endregion
		}

		public static Dictionary<int, List<int>> AnalyseBuckets(PXGraph graph, int BAccountID, string TaxLineType, bool CalcWithZones, Func<TaxReportLine, bool> ShowTaxReportLine = null)
        {
			TaxBucketAnalizer analizer = new TaxBucketAnalizer(graph, BAccountID, TaxLineType);
			if (ShowTaxReportLine != null)
			{
				analizer.showTaxReportLine = ShowTaxReportLine;
			}
			return analizer.AnalyzeBuckets(CalcWithZones);
        }
  
        private void UpdateNet(object Row)
		{
			bool RefreshNeeded = false;

			TaxReportLine currow = Row as TaxReportLine;

			if ((bool)currow.NetTax && currow.TempLineNbr == null)
			{
				foreach (TaxReportLine reportrow in PXSelect<TaxReportLine, Where<TaxReportLine.vendorID, Equal<Required<VendorMaster.bAccountID>>>>.Select(this,currow.VendorID))
				{
					if ((bool)reportrow.NetTax && reportrow.LineNbr != currow.LineNbr && reportrow.TempLineNbr != currow.LineNbr)
					{
						reportrow.NetTax = (bool)false;
						ReportLine.Cache.Update(reportrow);
						RefreshNeeded = true;
					}
				}
			}

			if (RefreshNeeded)
			{
				ReportLine.View.RequestRefresh();
			}
		}

		private TaxReportLine CreateChildLine(TaxReportLine template, TaxZone zone)
		{
			TaxReportLine child = PXCache<TaxReportLine>.CreateCopy(template);

			child.TempLineNbr = child.LineNbr;
			child.TaxZoneID = zone.TaxZoneID;
			child.LineNbr = null;
			child.TempLine = false;
			child.ReportLineNbr = null;

			if (string.IsNullOrEmpty(child.Descr) == false)
			{
				int fid;
				if ((fid = child.Descr.IndexOf(TAG_TAXZONE, StringComparison.OrdinalIgnoreCase)) >= 0)
				{
					child.Descr = child.Descr.Remove(fid, TAG_TAXZONE.Length).Insert(fid, child.TaxZoneID);
				}
			}

			return child;
		}

		private void UpdateZones(PXCache sender, object OldRow, object NewRow)
		{
			TaxReportLine oldRow = OldRow as TaxReportLine;
			TaxReportLine newRow = NewRow as TaxReportLine;

			if (oldRow != null && (newRow == null || newRow.TempLine == false))
			{
				if (string.IsNullOrEmpty(newRow?.Descr) == false)
				{
					int fid;
					if ((fid = newRow.Descr.IndexOf(TAG_TAXZONE, StringComparison.OrdinalIgnoreCase)) >= 0)
					{
						newRow.Descr = newRow.Descr.Remove(fid, TAG_TAXZONE.Length).TrimEnd(' ');
					}
				}

				foreach (TaxReportLine child in PXSelect<TaxReportLine, 
					Where<TaxReportLine.vendorID,Equal<Required<TaxReportLine.vendorID>>, 
						And<TaxReportLine.tempLineNbr, Equal<Required<TaxReportLine.tempLineNbr>>>>>
					.Select(this, oldRow.VendorID, oldRow.LineNbr))
				{
					sender.Delete(child);
				}
			}

			if (newRow?.TempLine == true && newRow.TempLine != oldRow?.TempLine)
			{
				newRow.TaxZoneID = null;

				if (string.IsNullOrEmpty(newRow.Descr) || 
					(newRow.Descr.IndexOf(TAG_TAXZONE, StringComparison.OrdinalIgnoreCase) < 0))
				{
					newRow.Descr += ' ' + TAG_TAXZONE;
				}

				foreach (TaxZone zone in PXSelect<TaxZone>.Select(this))
				{
					TaxReportLine child = CreateChildLine(newRow, zone);
					sender.Insert(child);
				}
			}

			if (newRow?.TempLine == true && oldRow?.TempLine == true)
			{
				foreach(TaxReportLine child in PXSelect<TaxReportLine, 
					Where<TaxReportLine.vendorID,Equal<Required<TaxReportLine.vendorID>>, 
						And<TaxReportLine.tempLineNbr, Equal<Required<TaxReportLine.tempLineNbr>>>>>
					.Select(this, oldRow.VendorID, oldRow.LineNbr))
				{
					child.Descr = newRow.Descr;

					if (string.IsNullOrEmpty(child.Descr) == false)
					{
						int fid;
						if ((fid = child.Descr.IndexOf(TAG_TAXZONE, StringComparison.OrdinalIgnoreCase)) >= 0)
						{
							child.Descr = child.Descr.Remove(fid, TAG_TAXZONE.Length).Insert(fid, child.TaxZoneID);
						}
					}

					child.NetTax = newRow.NetTax;
					child.LineType = newRow.LineType;
					child.LineMult = newRow.LineMult;
					sender.Update(child);
				}
			}
		}

		protected virtual void VendorMaster_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row != null)
			{
				var vendor = (VendorMaster) e.Row;

				PXUIFieldAttribute.SetVisible<TaxReportLine.tempLine>(ReportLine.Cache, null, (bool)vendor.ShowNoTemp == false);

				PXUIFieldAttribute.SetEnabled<TaxReportLine.tempLine>(ReportLine.Cache, null, (bool)vendor.ShowNoTemp == false);
				PXUIFieldAttribute.SetEnabled<TaxReportLine.netTax>(ReportLine.Cache, null, (bool)vendor.ShowNoTemp == false);
				PXUIFieldAttribute.SetEnabled<TaxReportLine.taxZoneID>(ReportLine.Cache, null, (bool)vendor.ShowNoTemp == false);
				PXUIFieldAttribute.SetEnabled<TaxReportLine.lineType>(ReportLine.Cache, null, (bool)vendor.ShowNoTemp == false);
				PXUIFieldAttribute.SetEnabled<TaxReportLine.lineMult>(ReportLine.Cache, null, (bool)vendor.ShowNoTemp == false);

				CheckReportSettingsEditableAndSetWarningTo<VendorMaster.bAccountID>(this, sender, vendor, vendor.BAccountID);
			}

			sender.IsDirty = false;
		}

		protected virtual void VendorMaster_RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			if (e.Row != null && ((VendorMaster)e.Row).ShowNoTemp == null)
			{
				((VendorMaster)e.Row).ShowNoTemp = false;
			}
		}

		protected virtual void TaxReportLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			TaxReportLine line = e.Row as TaxReportLine;
			if (line == null) return;
			PXUIFieldAttribute.SetEnabled<TaxReportLine.reportLineNbr>(sender, line, line.HideReportLine != true);
		}
		
		protected virtual void TaxReportLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			UpdateNet(e.Row);
			UpdateZones(sender, null, e.Row);
		}

		protected virtual void TaxReportLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
		{
			UpdateNet(e.Row);
			UpdateZones(sender, e.OldRow, e.Row);
		}

		protected virtual void TaxReportLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
		{
			UpdateZones(sender, e.Row, null);
		}

		protected virtual void TaxReportLine_HideReportLine_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			TaxReportLine line = e.Row as TaxReportLine;
			if (line == null) return;
			if (line.HideReportLine == true)
			{
				sender.SetValueExt<TaxReportLine.reportLineNbr>(line, null);
			}
		}

		public override void Persist()
		{
			CheckAndWarnTaxBoxNumbers();
			CheckReportSettingsEditable(this, TaxVendor.Current.BAccountID);

			SyncReportLinesAndBucketLines();

			base.Persist();
		}

		private void SyncReportLinesAndBucketLines()
		{
			TaxBucketLine_Vendor_LineNbr.Cache.Clear();

			foreach (var reportLine in ReportLine.Cache.Inserted.RowCast<TaxReportLine>())
			{
				var parentBuckets = TaxBucketLine_Vendor_LineNbr.Select(reportLine.VendorID, reportLine.TempLineNbr).RowCast<TaxBucketLine>();

				foreach (var bucketLine in parentBuckets)
				{
					TaxBucketLine newBucketLine = PXCache<TaxBucketLine>.CreateCopy(bucketLine);
					newBucketLine.LineNbr = reportLine.LineNbr;
					TaxBucketLine_Vendor_LineNbr.Cache.Insert(newBucketLine);
				}
			}

			foreach (var reportLine in ReportLine.Cache.Deleted.RowCast<TaxReportLine>())
			{
				var bucketLinesToDelete = TaxBucketLine_Vendor_LineNbr.Select(reportLine.VendorID, reportLine.LineNbr);
				foreach (var bucketLine in bucketLinesToDelete)
				{
					TaxBucketLine_Vendor_LineNbr.Cache.Delete(bucketLine);
				}
			}
		}

		public static void CheckReportSettingsEditableAndSetWarningTo<TVendorIDField>(PXGraph graph, PXCache cache, object row, int? vendorID)
			where TVendorIDField : IBqlField
		{
			if (ReportTaxProcess.PrepearedTaxPeriodForVendorExists(graph, vendorID))
			{
				var bAccIDfieldState = (PXFieldState)cache.GetStateExt<TVendorIDField>(row);

				cache.RaiseExceptionHandling<TVendorIDField>(row, bAccIDfieldState.Value,
					new PXSetPropertyException(Messages.TheTaxReportSettingsCannotBeModified, PXErrorLevel.Warning));
			}
		}

		public static void CheckReportSettingsEditable(PXGraph graph, int? vendorID)
		{
			if (ReportTaxProcess.PrepearedTaxPeriodForVendorExists(graph, vendorID))
				throw new PXException(Messages.TheTaxReportSettingsCannotBeModified);
		}

        private void CheckAndWarnTaxBoxNumbers()
        {
            HashSet<String> taxboxNumbers = new HashSet<String>();
            foreach (TaxReportLine line in ReportLine.Select())
            {
                if (ReportLine.Cache.GetStatus(line) == PXEntryStatus.Notchanged && line.ReportLineNbr != null)
                {
                    taxboxNumbers.Add(line.ReportLineNbr);
                }
            }

			CheckTaxBoxNumberUniqueness(ReportLine.Cache.Inserted, taxboxNumbers);
			CheckTaxBoxNumberUniqueness(ReportLine.Cache.Updated, taxboxNumbers);
        }

        public virtual void CheckTaxBoxNumberUniqueness(IEnumerable toBeChecked, HashSet<String> taxboxNumbers)
        {
            foreach (TaxReportLine line in toBeChecked)
            {
                if (line.ReportLineNbr != null && !taxboxNumbers.Add(line.ReportLineNbr))
                {
                    ReportLine.Cache.RaiseExceptionHandling<TaxReportLine.reportLineNbr>(line, line.ReportLineNbr, new PXSetPropertyException(Messages.TaxBoxNumbersMustBeUnique));
                }
            }
        }

	    protected virtual void TaxReportLine_LineType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			TaxReportLine line = (TaxReportLine)e.Row;

			if (e.NewValue != null && line.NetTax != null && (bool)line.NetTax && (string)e.NewValue == "A")
			{
				throw new PXSetPropertyException(Messages.NetTaxMustBeTax, PXErrorLevel.RowError);
			}
		}

		protected virtual void TaxReportLine_NetTax_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			TaxReportLine line = (TaxReportLine)e.Row;

			if (e.NewValue != null && (bool)e.NewValue && line.LineType == "A")
			{
				throw new PXSetPropertyException(Messages.NetTaxMustBeTax, PXErrorLevel.RowError);
			}
		}

		public TaxReportMaint()
		{
			APSetup setup = APSetup.Current;
			FieldDefaulting.AddHandler<BAccountR.type>((sender, e) => { if (e.Row != null) e.NewValue = BAccountType.VendorType; });
		}

		public PXSetup<APSetup> APSetup;
	}
}
