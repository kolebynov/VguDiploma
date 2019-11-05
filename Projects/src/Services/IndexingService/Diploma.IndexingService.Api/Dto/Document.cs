using System;

namespace Diploma.IndexingService.Api.Dto
{
	public class Document
	{
		public string Id { get; set; }

		public string FileName { get; set; }

		public DateTimeOffset ModificationDate { get; set; }

		public string ContentToken { get; set; }
	}
}
