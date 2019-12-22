using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Dto
{
	public class FoundDocumentDto
	{
		public string Id { get; set; }

		public string FileName { get; set; }

		public IReadOnlyDictionary<string, IReadOnlyCollection<IReadOnlyCollection<DocumentTextEntry>>> Matches { get; set; }
	}
}
