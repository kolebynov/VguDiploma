using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IIndexingQueue
	{
		Task Enqueue(IReadOnlyCollection<DocumentInfo> documents, CancellationToken cancellationToken);

		Task<DocumentInfo> Dequeue(CancellationToken cancellationToken);
	}
}
