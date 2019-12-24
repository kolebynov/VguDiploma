using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class InProgressDocument
	{
		public DocumentIdentity Id { get; }

		public string FileName { get; }

		public InProcessDocumentState State { get; }

		public string ErrorInfo { get; }

		public InProgressDocument(DocumentIdentity id, string fileName, InProcessDocumentState state, string errorInfo = null)
		{
			if (string.IsNullOrEmpty(errorInfo) && state == InProcessDocumentState.Error)
			{
				throw new ArgumentException($"Error info can't be empty if state is {state}", nameof(errorInfo));
			}

			Id = id ?? throw new ArgumentNullException(nameof(id));
			FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
			State = state;
			ErrorInfo = errorInfo;
		}
	}
}
