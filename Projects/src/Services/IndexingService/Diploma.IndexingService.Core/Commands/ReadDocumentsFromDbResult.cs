using System.Collections.Generic;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Commands
{
	internal class ReadDocumentsFromDbResult
	{
		public IReadOnlyCollection<DocumentInfo> Documents { get; set; }
	}
}
