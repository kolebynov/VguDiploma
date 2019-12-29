using System;
using System.Collections.Generic;

namespace Diploma.IndexingService.Core.Objects
{
	public class SearchResult
	{
		public IReadOnlyCollection<FoundDocument> FoundDocuments { get; }

		public int TotalDocuments { get; }

		public SearchResult(IReadOnlyCollection<FoundDocument> foundDocuments, int totalDocuments)
		{
			FoundDocuments = foundDocuments ?? throw new ArgumentNullException(nameof(foundDocuments));
			TotalDocuments = totalDocuments;
		}
	}
}
