using System;

namespace Diploma.IndexingService.EsDocumentStorage.Configuration
{
	public class DocumentStorageOptions
	{
		public Uri ElasticSearchUri { get; set; }

		public string IndexName { get; set; } = "documents";

		public int MaxAnalyzedOffsetForHighlighting { get; set; } = int.MaxValue;
	}
}
