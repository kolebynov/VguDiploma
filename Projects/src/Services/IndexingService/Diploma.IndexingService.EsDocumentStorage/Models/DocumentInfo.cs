using System;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.EsDocumentStorage.Models
{
	internal class DocumentInfo
	{
		public DocumentIdentity Id { get; set; }

		public string FileName { get; set; }

		public DateTimeOffset ModificationDate { get; set; }

		public string Text { get; set; }
	}
}
