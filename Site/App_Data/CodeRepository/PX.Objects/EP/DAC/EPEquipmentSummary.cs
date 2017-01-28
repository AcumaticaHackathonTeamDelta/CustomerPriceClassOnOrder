using System;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;

namespace PX.Objects.EP
{
	[PXVirtual]
	[PXCacheName(Messages.EquipmentSummary)]
	[Serializable]
	public partial class EPEquipmentSummary : IBqlTable
	{
		#region TimeCardCD

		public abstract class timeCardCD : IBqlField { }

        [PXDBDefault(typeof(EPEquipmentTimeCard.timeCardCD))]
		[PXDBString(10, IsKey = true)]
		[PXUIField(Visible = false)]
        [PXParent(typeof(Select<EPEquipmentTimeCard, Where<EPEquipmentTimeCard.timeCardCD, Equal<Current<EPEquipmentSummary.timeCardCD>>>>))]
		public virtual String TimeCardCD { get; set; }

		#endregion
		#region LineNbr
		public abstract class lineNbr : IBqlField { }

		[PXDBInt(IsKey = true)]
        [PXLineNbr(typeof(EPEquipmentTimeCard.summaryLineCntr))]
		[PXUIField(Visible = false)]
		public virtual Int32? LineNbr { get; set; }
		#endregion
		
        #region RateType

	    public const string Setup = "ST";
        public const string Run = "RU";
        public const string Suspend = "SD";

		public abstract class rateType : IBqlField { }
        [PXStringList(new string[] { "ST", "RU", "SD" }, new string[] { "Setup", "Run", "Suspend" })]
		[PXDBString(2, IsFixed = true, IsUnicode = false, InputMask=">LL")]
		[PXDefault("RU")]
		[PXUIField(DisplayName = "Rate Type")]
		public virtual string RateType { get; set; }
		#endregion
        
		#region ProjectID
		public abstract class projectID : IBqlField { }
		[ProjectDefault(BatchModule.EP)]
        [EPEquipmentActiveProject]
		public virtual Int32? ProjectID { get; set; }
		#endregion
		#region ProjectTaskID
		public abstract class projectTaskID : IBqlField { }
        [ActiveProjectTask(typeof(EPEquipmentSummary.projectID), BatchModule.EP, DisplayName = "Project Task")]
		public virtual Int32? ProjectTaskID { get; set; }
		#endregion
		#region TimeSpent
		public abstract class timeSpent : IBqlField { }

        [PXTimeList]
        [PXInt]
		[PXUIField(DisplayName = "Time Spent", Enabled = false)]
		public virtual Int32? TimeSpent
		{
			get
			{
				return Mon.GetValueOrDefault() +
					   Tue.GetValueOrDefault() +
					   Wed.GetValueOrDefault() +
					   Thu.GetValueOrDefault() +
					   Fri.GetValueOrDefault() +
					   Sat.GetValueOrDefault() +
					   Sun.GetValueOrDefault();
			}
		}
		#endregion
		#region Sun
		public abstract class sun : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Sun")]
		public virtual Int32? Sun { get; set; }
		#endregion
		#region Mon
		public abstract class mon : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Mon")]
		public virtual Int32? Mon { get; set; }
		#endregion
		#region Tue
		public abstract class tue : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Tue")]
		public virtual Int32? Tue { get; set; }
		#endregion
		#region Wed
		public abstract class wed : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Wed")]
		public virtual Int32? Wed { get; set; }
		#endregion
		#region Thu
		public abstract class thu : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Thu")]
		public virtual Int32? Thu { get; set; }
		#endregion
		#region Fri
		public abstract class fri : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Fri")]
		public virtual Int32? Fri { get; set; }
		#endregion
		#region Sat
		public abstract class sat : IBqlField { }
        [PXTimeList]
        [PXDBInt]
		[PXUIField(DisplayName = "Sat")]
		public virtual Int32? Sat { get; set; }
		#endregion
		#region IsBillable
		public abstract class isBillable : PX.Data.IBqlField
		{
		}
		protected Boolean? _IsBillable;
		[PXDBBool()]
		[PXDefault(false)]
		[PXUIField(DisplayName = "Billable")]
		public virtual Boolean? IsBillable
		{
			get
			{
				return this._IsBillable;
			}
			set
			{
				this._IsBillable = value;
			}
		}
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField
		{
		}
		protected String _Description;
		[PXDBString(255, IsUnicode = true)]
		[PXUIField(DisplayName = "Description")]
		public virtual String Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				this._Description = value;
			}
		}
		#endregion

        #region System Columns
		#region NoteID
		public abstract class noteID : PX.Data.IBqlField
		{
		}
		protected Guid? _NoteID;
		[PXNote()]
		public virtual Guid? NoteID
		{
			get
			{
				return this._NoteID;
			}
			set
			{
				this._NoteID = value;
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
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		public virtual Guid? CreatedByID
		{
			get
			{
				return this._CreatedByID;
			}
			set
			{
				this._CreatedByID = value;
			}
		}
		#endregion
		#region CreatedByScreenID
		public abstract class createdByScreenID : PX.Data.IBqlField
		{
		}
		protected String _CreatedByScreenID;
		[PXDBCreatedByScreenID()]
		public virtual String CreatedByScreenID
		{
			get
			{
				return this._CreatedByScreenID;
			}
			set
			{
				this._CreatedByScreenID = value;
			}
		}
		#endregion
		#region CreatedDateTime
		public abstract class createdDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _CreatedDateTime;
		[PXDBCreatedDateTime()]
		public virtual DateTime? CreatedDateTime
		{
			get
			{
				return this._CreatedDateTime;
			}
			set
			{
				this._CreatedDateTime = value;
			}
		}
		#endregion
		#region LastModifiedByID
		public abstract class lastModifiedByID : PX.Data.IBqlField
		{
		}
		protected Guid? _LastModifiedByID;
		[PXDBLastModifiedByID()]
		public virtual Guid? LastModifiedByID
		{
			get
			{
				return this._LastModifiedByID;
			}
			set
			{
				this._LastModifiedByID = value;
			}
		}
		#endregion
		#region LastModifiedByScreenID
		public abstract class lastModifiedByScreenID : PX.Data.IBqlField
		{
		}
		protected String _LastModifiedByScreenID;
		[PXDBLastModifiedByScreenID()]
		public virtual String LastModifiedByScreenID
		{
			get
			{
				return this._LastModifiedByScreenID;
			}
			set
			{
				this._LastModifiedByScreenID = value;
			}
		}
		#endregion
		#region LastModifiedDateTime
		public abstract class lastModifiedDateTime : PX.Data.IBqlField
		{
		}
		protected DateTime? _LastModifiedDateTime;
		[PXDBLastModifiedDateTime()]
		public virtual DateTime? LastModifiedDateTime
		{
			get
			{
				return this._LastModifiedDateTime;
			}
			set
			{
				this._LastModifiedDateTime = value;
			}
		}
		#endregion
		#endregion


        #region Unbound Fields (Calculated in the TimecardMaint graph)

        public abstract class labourClassCalc : IBqlField { }
        [PXInt]
		[PXUIField(DisplayName = "Labor Item", Enabled = false)]
        [PXSelector(typeof(InventoryItem.inventoryID), SubstituteKey = typeof(InventoryItem.inventoryCD))]
        public virtual Int32? LabourClassCalc { get; set; }
        
        #endregion

        public int? GetTimeTotal(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return Mon;
                case DayOfWeek.Tuesday: return Tue;
                case DayOfWeek.Wednesday: return Wed;
                case DayOfWeek.Thursday: return Thu;
                case DayOfWeek.Friday: return Fri;
                case DayOfWeek.Saturday: return Sat;
                case DayOfWeek.Sunday: return Sun;

                default:
                    return null;

            }
        }

        public int? GetTimeTotal()
        {
            return Mon.GetValueOrDefault() + Tue.GetValueOrDefault() + Wed.GetValueOrDefault() + Thu.GetValueOrDefault() +
                   Fri.GetValueOrDefault() + Sat.GetValueOrDefault() + Sun.GetValueOrDefault();

        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", RateType, Mon, Tue, Wed, Thu, Fri, Sat, Sun);
        }

	}
}
