// This File is Distributed as Part of Acumatica Shared Source Code 
/* ---------------------------------------------------------------------*
*                               Acumatica Inc.                          *
*              Copyright (c) 1994-2011 All rights reserved.             *
*                                                                       *
*                                                                       *
* This file and its contents are protected by United States and         *
* International copyright laws.  Unauthorized reproduction and/or       *
* distribution of all or any portion of the code contained herein       *
* is strictly prohibited and will result in severe civil and criminal   *
* penalties.  Any violations of this copyright will be prosecuted       *
* to the fullest extent possible under law.                             *
*                                                                       *
* UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN     *
* PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR  *
* SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY ProjectX PRODUCT.        *
*                                                                       *
* THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.              *
* ---------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace PX.Data
{
	#region PXUIBaseStateAttribute
	public class PXBaseConditionAttribute : PXEventSubscriberAttribute
	{
		protected IBqlCreator _Condition;

		public virtual Type Condition
		{
			get
			{
				return _Condition?.GetType();
			}
			set
			{
				_Condition = PXFormulaAttribute.InitFormula(value);
			}
		}

		public PXBaseConditionAttribute() {}

		public PXBaseConditionAttribute(Type conditionType)
			: this()
		{
			Type condition = conditionType;
			if (typeof(IBqlWhere).IsAssignableFrom(condition))
			{
				condition = BqlCommand.MakeGenericType(typeof(Switch<,>), typeof(Case<,>), conditionType, typeof(True), typeof(False));
			}
			_Condition = PXFormulaAttribute.InitFormula(condition);
		}

		public override PXEventSubscriberAttribute Clone(PXAttributeLevel attributeLevel)
		{
			if (attributeLevel == PXAttributeLevel.Item)
				return this;
			return base.Clone(attributeLevel);
		}

		protected static bool GetConditionResult(PXCache sender, object row, Type conditionType)
		{
			IBqlCreator condition = PXFormulaAttribute.InitFormula(conditionType);
			bool? result = null;
			object value = null;
			BqlFormula.Verify(sender, row, condition, ref result, ref value);
			return (value as bool?) == true;
		}

		protected static Type GetCondition<AttrType, Field>(PXCache sender, object row)
			where AttrType : PXBaseConditionAttribute
			where Field : IBqlField
		{
			return sender.GetAttributesReadonly<Field>().OfType<AttrType>().Select(attr => (attr).Condition).FirstOrDefault();
		}

		protected static Type GetCondition<AttrType>(PXCache sender, object row, string fieldName)
			where AttrType : PXBaseConditionAttribute
		{
			return sender.GetAttributes(row, fieldName).OfType<AttrType>().Select(attr => (attr).Condition).FirstOrDefault();
		}
	}
	#endregion

	#region PXUIRequiredAttribute
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = false)]
	public class PXUIRequiredAttribute : PXBaseConditionAttribute, IPXFieldSelectingSubscriber, IPXRowPersistingSubscriber
	{
		public PXUIRequiredAttribute(Type conditionType) : base(conditionType) {}

		public override void CacheAttached(PXCache sender)
		{
			PXDefaultAttribute defAttr = sender.GetAttributesReadonly(_FieldName).OfType<PXDefaultAttribute>().FirstOrDefault();
			if(defAttr != null)
			{
				defAttr.PersistingCheck = PXPersistingCheck.Nothing;
			}
			else
			{
				throw new PXException(ErrorMessages.UsageOfAttributeWithoutPrerequisites, GetType().Name, typeof(PXDefaultAttribute).Name, sender.GetItemType().FullName, FieldName);
			}
			base.CacheAttached(sender);
		}

		public virtual void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
		{
			if (_AttributeLevel == PXAttributeLevel.Item || e.IsAltered)
			{
				object row = e.Row;
				Type conditionType = Condition;
				if (row == null || conditionType == null)
					return;

				var persistingCheck = GetConditionResult(sender, row, conditionType) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing;
				e.ReturnState = PXFieldState.CreateInstance(e.ReturnState, null, null,
					null, persistingCheck == PXPersistingCheck.Nothing ? (int?)null : 1, null, null,
					null, _FieldName, null, null, null, PXErrorLevel.Undefined, null, null, null, PXUIVisibility.Undefined, null, null, null);
			}
		}

		public virtual void RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			object row = e.Row;
			Type conditionType = Condition;
			if (row == null || conditionType == null)
				return;

			PXPersistingCheck persistingCheck = GetConditionResult(sender, row, conditionType) ? PXPersistingCheck.NullOrBlank : PXPersistingCheck.Nothing;
			PXDefaultAttribute defaultAttribute = sender.GetAttributesReadonly(_FieldName).OfType<PXDefaultAttribute>().FirstOrDefault();
			if (defaultAttribute != null)
			{
				defaultAttribute.PersistingCheck = persistingCheck;
				defaultAttribute.RowPersisting(sender, e);
				defaultAttribute.PersistingCheck = PXPersistingCheck.Nothing;
			}
		}
	}
	#endregion

	#region PXUIEnabledAttribute
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method)]
	public class PXUIEnabledAttribute : PXBaseConditionAttribute, IPXRowSelectedSubscriber
	{
		public PXUIEnabledAttribute(Type conditionType)
			: base(conditionType)
		{
		}

		public virtual void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			if (e.Row == null || _Condition == null)
				return;
			SetEnabled(sender, e.Row, _FieldName, Condition);
		}

		public static void SetEnabled(PXCache sender, object row, string fieldName, Type conditionType)
		{
			if (row == null || conditionType == null)
				return;
			PXUIFieldAttribute.SetEnabled(sender, row, fieldName, GetConditionResult(sender, row, conditionType));
		}

		public static void SetEnabled(PXCache sender, object row, string fieldName)
		{
			if (row == null)
				return;
			SetEnabled(sender, row, fieldName, GetCondition<PXUIEnabledAttribute>(sender, row, fieldName));
		}

		public static void SetEnabled<Field>(PXCache sender, object row, Type conditionType)
			where Field : IBqlField
		{
			if (row == null || conditionType == null)
				return;
			SetEnabled(sender, row, typeof(Field).Name, conditionType);
		}

		public static void SetEnabled<Field>(PXCache sender, object row)
			where Field : IBqlField
		{
			if (row == null)
				return;
			SetEnabled<Field>(sender, row, GetCondition<PXUIEnabledAttribute, Field>(sender, row));
		}

	}
	#endregion

	[Flags]
	public enum TriggerPoints
	{
		RowInserted = 1,
		RowPersisting = 1 << 1,
		RowSelected = 1 << 2,
		FieldVerifying = 1 << 3,
	}

	#region PXUIVerifyAttribute
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true)]
	public class PXUIVerifyAttribute : PXBaseConditionAttribute, IPXFieldVerifyingSubscriber, IPXRowInsertedSubscriber, IPXRowPersistingSubscriber
	{
		public TriggerPoints VerificationPoints { get; protected set; }
		protected PXErrorLevel _ErrorLevel;
		protected string _Message;
		protected Type[] _args;

		[Obsolete("This field has been deprecated and will be removed in Acumatica 7.0")]
		protected bool _OnSelectingVerify;
		[Obsolete("This field has been deprecated and will be removed in Acumatica 7.0")]
		protected bool _CheckOnVerifying;
		[Obsolete("This field has been deprecated and will be removed in Acumatica 7.0")]
		protected bool _CheckOnInserted;

		private void SetVerificationPoint(TriggerPoints triggerPoint, bool value)
		{
			if (value)
			{
				this.VerificationPoints |= triggerPoint;
			}
			else
			{
				this.VerificationPoints &= ~triggerPoint;
			}
		}

		public bool CheckOnVerify
		{
			get
			{
				return this.VerificationPoints.HasFlag(TriggerPoints.FieldVerifying);
			}
			set
			{
				this.SetVerificationPoint(TriggerPoints.FieldVerifying, value);
			}
		}

		public bool CheckOnInserted
		{
			get
			{
				return this.VerificationPoints.HasFlag(TriggerPoints.RowInserted);
			}
			set
			{
				this.SetVerificationPoint(TriggerPoints.RowInserted, value);
			}
		}

		public bool CheckOnRowSelected
		{
			get
			{
				return this.VerificationPoints.HasFlag(TriggerPoints.RowSelected);
			} 
			set
			{
				this.SetVerificationPoint(TriggerPoints.RowSelected, value);
			}
		}

		public bool CheckOnRowPersisting
		{
			get
			{
				return this.VerificationPoints.HasFlag(TriggerPoints.RowPersisting);
			}
			set
			{
				this.SetVerificationPoint(TriggerPoints.RowPersisting, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether BQL fields specified by 
		/// <see cref="_args"/> should be converted to field names, not field values,
		/// when composing the error message.
		/// </summary>
		public bool MessageArgumentsAreFieldNames { get; set; } = false;

		public PXUIVerifyAttribute(Type conditionType, PXErrorLevel errorLevel, string message, bool OnSelectingVerify, params Type[] args)
			: this(conditionType, errorLevel, message, args)
		{
			this.CheckOnRowSelected = OnSelectingVerify;
		}

		public PXUIVerifyAttribute(Type conditionType, TriggerPoints verificationPoints, PXErrorLevel errorLevel, string message, params Type[] args)
			: this(conditionType, errorLevel, message, args)
		{
			this.VerificationPoints = verificationPoints;
		}

		public PXUIVerifyAttribute(Type conditionType, PXErrorLevel errorLevel, string message, params Type[] args)
			: base(conditionType)
		{
			_ErrorLevel = errorLevel;
			_Message = message;
			_args = args;
			this.VerificationPoints = TriggerPoints.RowInserted | TriggerPoints.RowPersisting | TriggerPoints.FieldVerifying;
		}

		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);
			if (_Condition != null)
			{
				List<Type> fields = new List<Type>();
				_Condition.Parse(sender.Graph, null, null, fields, null, null, null);
				foreach (Type t in fields)
				{
					if (t.IsNested && (BqlCommand.GetItemType(t) == sender.GetItemType() || sender.GetItemType().IsSubclassOf(BqlCommand.GetItemType(t))))
					{
						if (!t.Name.Equals(_FieldName, StringComparison.OrdinalIgnoreCase))
						{
							sender.FieldUpdatedEvents[t.Name.ToLower()] += delegate(PXCache cache, PXFieldUpdatedEventArgs e)
							{
								dependentFieldUpdated(cache, e.Row);
							};
						}
					}
				}
				if (this.CheckOnRowSelected)
				{
					sender.RowSelected += delegate(PXCache cache, PXRowSelectedEventArgs e)
					{
						var ex = VerifyingAndGetError(this, sender, e.Row);

						object newValue = null;
						if (ex != null && ex.ErrorLevel == PXErrorLevel.Error)
						{
							newValue = sender.GetValueExt(e.Row, _FieldName);
							if (newValue is PXFieldState)
								newValue = ((PXFieldState)newValue).Value;
						}
						cache.RaiseExceptionHandling(FieldName, e.Row, newValue, ex);
					};
				}

			}
		}

		protected virtual void dependentFieldUpdated(PXCache sender, object row)
		{
			//if (PXUIFieldAttribute.GetError(sender, row, _FieldName) != null)
			//	return;
			var newValue = sender.GetValueExt(row, _FieldName);
			if (newValue is PXFieldState)
				newValue = ((PXFieldState)newValue).Value;

			try
			{
                if (this.CheckOnVerify)
				    Verifying(sender, row);
				sender.RaiseExceptionHandling(_FieldName, row, newValue, null);
			}
			catch (PXSetPropertyException ex)
			{
				sender.RaiseExceptionHandling(_FieldName, row, newValue, ex);
			}
		}

		private void Verifying(PXCache sender, object row)
		{            
			PXSetPropertyException error = VerifyingAndGetError(this, sender, row);
			if(error != null)
				throw error;
		}

		private static PXSetPropertyException VerifyingAndGetError(PXUIVerifyAttribute attr, PXCache sender, object row)
		{
			if (row == null)
				return null;

			if (attr == null)
				throw new PXException(ErrorMessages.AttributeNotDefined, typeof(PXUIVerifyAttribute).Name);

			if (attr.Condition != null && !GetConditionResult(sender, row, attr.Condition))
			{
				List<object> messageParams = new List<object>();

				foreach (Type arg in attr._args)
				{
					if (typeof(Constant).IsAssignableFrom(arg))
					{
						Constant constantInstance = Activator.CreateInstance(arg) as Constant;
						messageParams.Add(constantInstance?.Value);
					}
					else if (attr.MessageArgumentsAreFieldNames)
					{
						messageParams.Add(PXUIFieldAttribute.GetDisplayName(sender, arg.Name));
					}
					else
					{
						var value = sender.GetValueExt(row, arg.Name);

						if (value is PXFieldState)
							messageParams.Add(((PXFieldState)value).Value);
						else
							messageParams.Add(value);
					}
				}
				return new PXSetPropertyException(attr._Message, attr._ErrorLevel, messageParams.ToArray());
			}
			return null;
		}

		public static PXSetPropertyException VerifyingAndGetError<Field>(PXCache sender, object row) where Field:IBqlField
		{
			PXSetPropertyException err = null;
			foreach (PXSetPropertyException currentError in sender.GetAttributes<Field>(row).OfType<PXUIVerifyAttribute>()
				.Select(attr => VerifyingAndGetError(attr, sender, row))
				.Where(currentError => currentError != null))
			{
				switch (currentError.ErrorLevel)
				{
					case PXErrorLevel.RowError:
						return currentError;
					case PXErrorLevel.Error:
					case PXErrorLevel.RowWarning:
						if (err == null || err.ErrorLevel != PXErrorLevel.Error)
							err = currentError;
						break;
					case PXErrorLevel.Warning:
						if (err == null || err.ErrorLevel != PXErrorLevel.RowWarning)
							err = currentError;
						break;
					case PXErrorLevel.RowInfo:
						if (err == null || err.ErrorLevel != PXErrorLevel.Warning)
							err = currentError;
						break;
					default:
						err = currentError;
						break;
				}
			}
			return err;
		}

		void IPXFieldVerifyingSubscriber.FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
		{
			if (this.CheckOnVerify)
			{
				var row = sender.CreateCopy(e.Row);
				sender.SetValue(row, _FieldName, e.NewValue);

				try
				{
					Verifying(sender, row);
                    sender.RaiseExceptionHandling(_FieldName, e.Row, null, null);
				}
				catch (PXSetPropertyException ex)
				{
					var newValue = sender.GetValueExt(row, _FieldName);
					if (newValue is PXFieldState)
						newValue = ((PXFieldState)newValue).Value;
					sender.RaiseExceptionHandling(_FieldName, e.Row, newValue, ex);
				}
			}
		}

		void IPXRowInsertedSubscriber.RowInserted(PXCache sender, PXRowInsertedEventArgs e)
		{
			if (this.CheckOnInserted)
			{
				try
				{
					Verifying(sender, e.Row);
				}
				catch (PXSetPropertyException ex)
				{
					var newValue = sender.GetValue(e.Row, _FieldName);
					sender.RaiseExceptionHandling(_FieldName, e.Row, newValue, ex);
				}
			}
		}

		void IPXRowPersistingSubscriber.RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
		{
			if (!this.CheckOnRowPersisting)
			{
				return;
			}

			try
			{
				Verifying(sender, e.Row);
			}
			catch (PXSetPropertyException ex)
			{
				var newValue = sender.GetValueExt(e.Row, _FieldName);
				if (newValue is PXFieldState)
					newValue = ((PXFieldState)newValue).Value;
				sender.RaiseExceptionHandling(_FieldName, e.Row, newValue, ex);
			}
		}
	}
	#endregion


}
