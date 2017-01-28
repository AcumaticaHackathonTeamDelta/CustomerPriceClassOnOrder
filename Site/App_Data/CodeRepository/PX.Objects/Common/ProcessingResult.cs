using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PX.Data;

namespace PX.Objects.Common
{
	public class ProcessingResult
	{
		protected readonly List<Message> _messages;

		public IReadOnlyList<Message> Messages
		{
			get { return _messages; }
		}

		/// <summary>
		/// true, if result does not have error level messages
		/// </summary>
		public virtual bool IsSuccess
		{
			get { return _messages.All(message => message.ErrorLevel != PXErrorLevel.RowError
												&& message.ErrorLevel != PXErrorLevel.Error);
			}
		}

		public bool HasWarning
		{
			get
			{
				return _messages.Any(message => message.ErrorLevel == PXErrorLevel.RowWarning
				                                || message.ErrorLevel == PXErrorLevel.Warning);
			}
		}

		public bool HasWarningOrError
		{
			get { return HasWarning || !IsSuccess; }
		}

		public PXErrorLevel MaxErrorLevel
		{
			get { return _messages.Max(message => message.ErrorLevel); }
		}

		public static ProcessingResult Success
		{
			get
			{
				return new ProcessingResult();
			}
		}

		public ProcessingResult()
		{
			_messages = new List<Message>();
		}

		public void AddErrorMessage(string message, params object[] args)
		{
			AddMessage(PXErrorLevel.Error, message, args);
		}

		public void AddErrorMessage(string message)
		{
			AddMessage(PXErrorLevel.Error, message);
		}

		public void AddMessage(PXErrorLevel errorLevel, string message, params object[] args)
		{
			_messages.Add(new Message(errorLevel, PXMessages.LocalizeFormatNoPrefix(message, args)));
		}

		public void AddMessage(PXErrorLevel errorLevel, string message)
		{
			_messages.Add(new Message(errorLevel, PXMessages.LocalizeNoPrefix(message)));
		}

		public void Aggregate(ProcessingResult processingResult)
		{
			foreach (var message in processingResult.Messages)
			{
				_messages.Add(message);
			}
		}

		public void RaiseIfHasError()
		{
			if (!IsSuccess)
				throw new PXException(GetGeneralMessage());
		}

		public virtual string GetGeneralMessage()
		{
			return string.Join(Environment.NewLine, Messages.Select(message => message.ToString()).ToArray());
		}

		public class Message
		{
			public string Text { get; set; }

			public PXErrorLevel ErrorLevel { get; set; }

			public Message(PXErrorLevel errorLevel, string text)
			{
				Text = text;
				ErrorLevel = errorLevel;
			}

			public override string ToString()
			{
				string errorLevelPrefix = null;

				if (ErrorLevel == PXErrorLevel.Error || ErrorLevel == PXErrorLevel.RowError)
				{
					errorLevelPrefix = Common.Messages.Error;
				}
				else if (ErrorLevel == PXErrorLevel.Warning || ErrorLevel == PXErrorLevel.RowWarning)
				{
					errorLevelPrefix = Common.Messages.Warning;
				}

				return errorLevelPrefix != null 
					? string.Concat(errorLevelPrefix, ": ", Text) 
					: Text;
			}
		}
	}
}
