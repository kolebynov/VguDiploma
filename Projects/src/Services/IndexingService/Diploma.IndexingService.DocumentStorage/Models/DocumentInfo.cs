using System;

namespace Diploma.IndexingService.DocumentStorage.Models
{
	internal class DocumentInfo
	{
		public string Id { get; set; }

		public string UserIdentity { get; set; }

		public string FileName { get; set; }

		public DateTimeOffset ModificationDate { get; set; }

		public byte[] Content { get; set; }
	}
}
