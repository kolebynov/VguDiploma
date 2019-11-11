using System.Collections.Generic;

namespace Diploma.IndexingService.Api.Dto
{
	public class FoundDocument
	{
		public string Id { get; set; }

		public string FileName { get; set; }

		public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Matches { get; set; }
	}
}
