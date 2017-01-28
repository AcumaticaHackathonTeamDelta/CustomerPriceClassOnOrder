using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PX.Objects.Common
{
	public static class Messages
	{
		#region Validation messages

		public const string EntityWithIDDoesNotExist = "{0} with ID '{1}' does not exist";
		public const string NotApprover = "You are not an authorized approver for this document.";
		public const string IncomingApplicationCannotBeReversed = "This application cannot be reversed from {0}. Open {1} {2} to reverse this application.";
		public const string ConstantMustBeDeclaredInsideLabelProvider = "The specified constant type '{0}' must be declared inside a class implementing the " + nameof(ILabelProvider) + " interface.";
		public const string TypeMustImplementLabelProvider = "The specified type '{0}' must implement the " + nameof(ILabelProvider) + " interface.";
		public const string LabelProviderMustHaveParameterlessConstructor = "The label provider class '{0}' must have a parameterless constructor.";
		public const string FieldIsNotOfStringType = "The {0} field is not of string type.";
		public const string FieldDoesNotHaveItemOrCacheLevelAttribute = "The {0} field does not have an item-level or cache-level {1}.";
		public const string FieldDoesNotHaveCacheLevelAttribute = "The {0} field does not have a cache-level {1}.";
		public const string StringListAttributeDoesNotDefineLabelForValue = "The string list attribute does not define a label for value '{0}'.";
		public const string BqlCommandsHaveDifferentParameters = "The BQL commands have different parameters.";
		public const string RecordCanNotBeSaved = "The record cannot be saved because some error have occurred. Please review the errors.";

		#endregion

		public const string Error = "Error";
		public const string Warning = "Warning";

		#region Translatable strings used in code

		public const string ScheduleID = "Schedule ID";
		public const string NewSchedule = "New Schedule";

		public const string AttributeDeprecatedWillRemoveInAcumatica7 = "This attribute has been deprecated and will be removed in Acumatica 7.0";
		public const string FieldIsObsoleteRemoveInAcumatica7 = "This field has been deprecated and will be removed in Acumatica 7.0";

		#endregion
	}
}
