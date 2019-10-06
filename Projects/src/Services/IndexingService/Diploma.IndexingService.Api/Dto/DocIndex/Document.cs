using System;

namespace Diploma.IndexingService.Api.Dto.DocIndex
{
	public class Document
	{
		public string Id { get; set; }

		public string FileName { get; set; }

		public DateTimeOffset ModificationDate { get; set; }
	}
}
