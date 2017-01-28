namespace PX.Objects.EP
{
	using System;
	using PX.Data;

	[PXPrimaryGraph(typeof(EPAssignmentMaint))]
	[Serializable]
	[PXCacheName(Messages.AssignmentMap)]
	public partial class EPAssignmentMap : PX.Data.IBqlTable
	{
		#region AssignmentMapID
		public abstract class assignmentMapID : PX.Data.IBqlField
		{
		}
		protected Int32? _AssignmentMapID;
		[PXDBIdentity(IsKey = true)]
		[PXUIField(DisplayName = "Map")]
		[EPAssignmentMapSelector]
		public virtual Int32? AssignmentMapID
		{
			get
			{
				return this._AssignmentMapID;
			}
			set
			{
				this._AssignmentMapID = value;
			}
		}
		#endregion
		#region Name
		public abstract class name : PX.Data.IBqlField
		{
		}
		protected String _Name;
		[PXDBString(60, IsUnicode = true)]
		[PXDefault]
		[PXUIField(DisplayName = "Name")]
		[PX.Data.EP.PXFieldDescription]
		public virtual String Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}
		#endregion		

		#region EntityType
		public abstract class entityType : PX.Data.IBqlField
		{
		}
		protected string _EntityType;
		[PXDBString(255)]
		[PXDefault]
		public virtual string EntityType
		{
			get
			{
				return this._EntityType;
			}
			set
			{
				this._EntityType = value;
			}
		}
		#endregion
        #region GraphType
        public abstract class graphType : PX.Data.IBqlField
        {
        }
        protected string _GraphType;
        [PXDBString(255)]
        [PXDefault]
        [PXUIField(DisplayName = "Entity")]
        public virtual string GraphType
        {
            get
            {
                return this._GraphType;
            }
            set
            {
                this._GraphType = value;
            }
        }
        #endregion

		#region NoteID
		public abstract class noteID : IBqlField { }

		[PXNote(DescriptionField = typeof(EPAssignmentMap.assignmentMapID),
			Selector = typeof(Search<EPAssignmentMap.assignmentMapID>))]
		public virtual Guid? NoteID { get; set; }
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
	}

	public static class AssignmentMapType
	{
		public class AssignmentMapTypeLead : Constant<string>
		{
			public AssignmentMapTypeLead() : base(typeof(PX.Objects.CR.Contact).FullName) { }
		}

		public class AssignmentMapTypeCase : Constant<string>
		{
			public AssignmentMapTypeCase() : base(typeof(PX.Objects.CR.CRCase).FullName) { }
		}

		public class AssignmentMapTypeOpportunity : Constant<string>
		{
			public AssignmentMapTypeOpportunity() : base(typeof(PX.Objects.CR.CROpportunity).FullName) { }
		}

		public class AssignmentMapTypeExpenceClaim : Constant<string>
		{
			public AssignmentMapTypeExpenceClaim() : base(typeof(PX.Objects.EP.EPExpenseClaim).FullName) { }
		}
		
        public class AssignmentMapTypeTimeCard : Constant<string>
		{
			public AssignmentMapTypeTimeCard() : base(typeof(PX.Objects.EP.EPTimeCard).FullName) { }
		}

        public class AssignmentMapTypeEquipmentTimeCard : Constant<string>
        {
            public AssignmentMapTypeEquipmentTimeCard() : base(typeof(PX.Objects.EP.EPEquipmentTimeCard).FullName) { }
        }

		public class AssignmentMapTypeSalesOrder : Constant<string>
		{
			public AssignmentMapTypeSalesOrder() : base(typeof(PX.Objects.SO.SOOrder).FullName) { }
		}
		public class AssignmentMapTypeSalesOrderShipment : Constant<string>
		{
			public AssignmentMapTypeSalesOrderShipment() : base(typeof(PX.Objects.SO.SOShipment).FullName) { }
		}
		public class AssignmentMapTypePurchaseOrder : Constant<string>
		{
			public AssignmentMapTypePurchaseOrder() : base(typeof(PX.Objects.PO.POOrder).FullName) { }
		}
		public class AssignmentMapTypePurchaseOrderReceipt : Constant<string>
		{
			public AssignmentMapTypePurchaseOrderReceipt() : base(typeof(PX.Objects.PO.POReceipt).FullName) { }
		}
		public class AssignmentMapTypePurchaseRequestItem : Constant<string>
		{
			public AssignmentMapTypePurchaseRequestItem() : base(typeof(PX.Objects.RQ.RQRequest).FullName) { }
		}
		public class AssignmentMapTypePurchaseRequisition : Constant<string>
		{
			public AssignmentMapTypePurchaseRequisition() : base(typeof(PX.Objects.RQ.RQRequisition).FullName) { }
		}
		public class AssignmentMapTypeCashTransaction : Constant<string>
		{
			public AssignmentMapTypeCashTransaction() : base(typeof(PX.Objects.CA.CAAdj).FullName) { }
		}
		public class AssignmentMapTypeProspect : Constant<string>
		{
			public AssignmentMapTypeProspect() : base(typeof(PX.Objects.CR.BAccount).FullName) { }
		}
		public class AssignmentMapTypeProject : Constant<string>
		{
			public AssignmentMapTypeProject() : base(typeof(PX.Objects.PM.PMProject).FullName) { }
		}
		public class AssignmentMapTypeActivity : Constant<string>
		{
			public AssignmentMapTypeActivity() : base(typeof(PX.Objects.CR.CRActivity).FullName) { }
		}
        public class AssignmentMapTypeImplementationScenario : Constant<string>
        {
             public AssignmentMapTypeImplementationScenario() : base(typeof(PX.Objects.WZ.WZScenario).FullName) { }
        }

		public class AssignmentMapTypeAPInvoice : Constant<string>
		{
			public AssignmentMapTypeAPInvoice() : 
				base(typeof(PX.Objects.AP.APInvoice).FullName) { }
		}

		public class AssignmentMapTypeExpenceClaimDetails : Constant<string>
		{
			public AssignmentMapTypeExpenceClaimDetails() : base(typeof(PX.Objects.EP.EPExpenseClaimDetails).FullName) { }
		}
	}
}
