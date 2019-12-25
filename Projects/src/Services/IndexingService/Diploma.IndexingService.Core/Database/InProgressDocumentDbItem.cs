using System;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Database
{
	internal class InProgressDocumentDbItem
	{
		public string Id { get; set; }

		public string UserIdentity { get; set; }

		public string FileName { get; set; }

		public InProcessDocumentState State { get; set; }

		public string ErrorInfo { get; set; }

		public DateTimeOffset LastStatusUpdateTime { get; set; }
	}
}
