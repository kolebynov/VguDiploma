using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IDocumentStorage
	{
		Task SaveDocumentToDb(FullDocumentInfo document, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<FoundDocument>> Search(SearchQuery searchQuery, CancellationToken cancellationToken);
	}
}
