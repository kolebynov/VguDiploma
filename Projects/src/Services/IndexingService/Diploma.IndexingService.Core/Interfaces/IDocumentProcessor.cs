using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	internal interface IDocumentProcessor
	{
		Task<ProcessedDocument> Process(DocumentInfo document, CancellationToken cancellationToken);
	}
}
