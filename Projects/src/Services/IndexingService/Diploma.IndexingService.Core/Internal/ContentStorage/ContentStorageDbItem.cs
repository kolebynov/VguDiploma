using System;

namespace Diploma.IndexingService.Core.Internal.ContentStorage
{
	internal class ContentStorageDbItem
	{
		public string Id { get; set; }

		public string Category { get; set; }

		public byte[] Content { get; set; }

		public DateTimeOffset Timestamp { get; set; }
	}
}
