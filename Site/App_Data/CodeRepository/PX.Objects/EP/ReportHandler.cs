using System;
using System.Collections.Generic;
using PX.Common;
using PX.Data;
using PX.Data.EP;
using PX.Data.Wiki.Parser;
using PX.Objects.CR;
using PX.Reports.Data;
using PX.Reports.Mail;
using MailSender = PX.Common.Mail.MailSender;

namespace PX.Objects.EP
{
	public class ReportHandler : DynamicAttachmentManager.IHandler
	{
		private const string _DEFAULT_REPORTFORMAT = "PDF";

		public IDictionary<string, byte[]> Process(PXGraph graph, string name, Guid? refNoteID, byte[] src)
		{
			if (src == null) return null;

			var report = ExtractReport(src);
			if (report == null) return null;

			var format = GetFormat(graph, refNoteID);
			return GenerateFile(report, name, format);
		}

		private IDictionary<string, byte[]> GenerateFile(ReportNode report, string name, string format)
		{
			var source = new GroupMessage(null, 
				null, 
				PX.Common.Mail.MailSender.MessageAddressee.Empty,
				PX.Common.Mail.MailSender.MessageContent.Empty, 
				null, 
				format,
				MessageRelationship.Empty, ReportAttachment.Empty);
			var message = new PX.Reports.Mail.Message(source, report, null);
			var attachments = message.Attachments;
			if (attachments == null) return null;

			var result = new Dictionary<string, byte[]>(attachments.Count);
			var index = 0;
			foreach (var item in attachments)
			{
				var filename = item.Name;
				if (!string.IsNullOrEmpty(name))
				{
					filename = name;
					if (index++ > 0) filename += " (" + index.ToString() + ")";
				}
				if (!System.IO.Path.HasExtension(item.Name))
				{
					var ext = MimeTypes.GetExtension(item.MimeType);
					if (ext != null)
						filename = System.IO.Path.GetFileNameWithoutExtension(filename) + ext;
				}
				result.Add(filename, item.ToArray());
			}
			return result;
		}

		private ReportNode ExtractReport(byte[] data)
		{
			var reports = PXDatabase.Deserialize(data);
			return reports.Length == 0 ? null : (ReportNode)reports[0];
		}

		private string GetFormat(PXGraph graph, Guid? activityNoteID)
		{
			var format = activityNoteID.
				With(id => (SMEmail)PXSelect<SMEmail,
					Where<SMEmail.refNoteID, Equal<Required<SMEmail.refNoteID>>>>.
					Select(graph, id.Value)).
				With(n => n.ReportFormat);
			return string.IsNullOrEmpty(format) ? _DEFAULT_REPORTFORMAT : format;
		}
	}
}
