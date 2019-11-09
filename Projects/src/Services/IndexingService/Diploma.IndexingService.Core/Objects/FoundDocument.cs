using System.Collections.Generic;

namespace Diploma.IndexingService.Core.Objects
{
	public class FoundDocument
	{
		public DocumentIdentity DocumentId { get; set; }

		public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Matches { get; set; }
	}
}
