using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class InProgressDocument
	{
		public DocumentIdentity Id { get; }

		public string FileName { get; }

		public InProcessDocumentState State { get; }

		public string ErrorInfo { get; }

		public DateTimeOffset LastStatusUpdateTime { get; }

		public InProgressDocument(DocumentIdentity id, string fileName, InProcessDocumentState state,
			DateTimeOffset lastStatusUpdateTime, string errorInfo = null)
		{
			Id = id ?? throw new ArgumentNullException(nameof(id));
			FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
			State = state;
			ErrorInfo = errorInfo;
			LastStatusUpdateTime = lastStatusUpdateTime;
		}
	}
}
