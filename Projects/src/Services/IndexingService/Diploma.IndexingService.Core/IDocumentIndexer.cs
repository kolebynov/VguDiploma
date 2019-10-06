using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core
{
	public interface IDocumentIndexer
	{
		Task AddDocuments(IReadOnlyCollection<Document> documents);

		Task AddDocumentContent(string documentId, Stream content);
	}
}
