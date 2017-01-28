using System;
using System.Collections.Generic;
using System.Text;
using PX.Data;

namespace PX.Objects.EP
{
	[Serializable]
	[PXCacheName(Messages.Wingman)]
	public partial class EPWingman : IBqlTable
	{
		#region RecordID
		public abstract class recordID : PX.Data.IBqlField
		{
		}
		protected Int32? _RecordID;
		[PXDBIdentity(IsKey = true)]
		public virtual Int32? RecordID
		{
			get
			{
				return _RecordID;
			}
			set
			{
				_RecordID = value;
			}
		}
		#endregion
		#region EmployeeID
		public abstract class employeeID : PX.Data.IBqlField
		{
		}
		protected Int32? _EmployeeID;
		[PXDBInt()]
		[PXDBDefault(typeof(EPEmployee.bAccountID))]
		[PXParent(typeof(Select<EPEmployee, Where<EPEmployee.bAccountID, Equal<Current<EPWingman.employeeID>>>>))]
		public Int32? EmployeeID
		{
			get
			{
				return _EmployeeID;
			}
			set
			{
				_EmployeeID = value;
			}
		}
		#endregion
		#region WingmanID
		public abstract class wingmanID : PX.Data.IBqlField
		{
		}
		protected Int32? _WingmanID;
		[PXDBInt()]
		[PXRestrictor(typeof(Where<EPEmployee.bAccountID, NotEqual<Current<EPWingman.employeeID>>>), null)]
		[PXEPEmployeeSelector]
		[PXCheckUnique(typeof(employeeID))]
		[PXUIField(DisplayName = "Delegate")]
		public Int32? WingmanID
		{
			get
			{
				return _WingmanID;
			}
			set
			{
				_WingmanID = value;
			}
		}
		#endregion
		#region CreatedByID
		public abstract class createdByID : PX.Data.IBqlField
		{
		}
		protected Guid? _CreatedByID;
		[PXDBCreatedByID()]
		[PXUIField(DisplayName = "Created By")]
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
		[PXUIField(DisplayName = "Created Date")]
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
		[PXUIField(DisplayName = "Last Modified By")]
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
		[PXUIField(DisplayName = "Last Modified Date")]
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

	public class WingmanUser<Operand> : IBqlComparison
	where Operand : IBqlOperand, new()
	{
		private IBqlCreator _operand;
		private const string EMPLOYEERALIAS = "EMPLOYEERALIAS";
		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			result = null;
			value = null;
		}

		public void Parse(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, BqlCommand.Selection selection)
		{
			if (graph != null && text != null)
			{
				text.Append(" IN ").Append(BqlCommand.SubSelect);
				text.Append(typeof(EPWingman).Name).Append('.').Append(typeof(EPWingman.employeeID).Name);
				text.Append(" FROM ").Append(typeof(EPWingman).Name).Append(' ').Append(typeof(EPWingman).Name);
				text.Append(" INNER JOIN ").Append(typeof(EPEmployee).Name).Append(' ').Append(EMPLOYEERALIAS);
				text.Append(" ON ").Append(EMPLOYEERALIAS).Append('.').Append(typeof(EPEmployee.bAccountID).Name);
				text.Append('=').Append(typeof(EPWingman).Name).Append('.').Append(typeof(EPWingman.wingmanID).Name);
				text.Append(" AND ").Append(EMPLOYEERALIAS).Append('.').Append(typeof(EPEmployee.userID).Name);
				text.Append('=');
				ParseOperand(graph, pars, tables, fields, sortColumns, text, selection);
				text.Append(" WHERE 1=1");
				text.Append(')');
			}
			else
				ParseOperand(graph, pars, tables, fields, sortColumns, text, selection);
		}
		private void ParseOperand(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, BqlCommand.Selection selection)
		{
			BqlCommand.EqualityList list = fields as BqlCommand.EqualityList;
			if (list != null)
				list.NonStrict = true;
			if (!typeof(IBqlCreator).IsAssignableFrom(typeof(Operand)))
			{
				if (graph != null && text != null)
					text.Append(" ").Append(BqlCommand.GetSingleField(typeof(Operand), graph, tables, selection, BqlCommand.FieldPlace.Condition));
				if (fields != null)
					fields.Add(typeof(Operand));
			}
			else
			{
				if (_operand == null)
					_operand = _operand.createOperand<Operand>();
				_operand.Parse(graph, pars, tables, fields, sortColumns, text, selection);
			}
		}
	}
	
}
