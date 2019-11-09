using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IContentStorage
	{
		Task<ContentStorageItem> Save(string id, string category, IContent content, CancellationToken cancellationToken);

		Task<ContentStorageItem> Get(string id, string category, CancellationToken cancellationToken);

		Task Delete(string id, string category, CancellationToken cancellationToken);

		IAsyncEnumerable<IReadOnlyCollection<ContentStorageItem>> GetAll(
			string category,
			CancellationToken cancellationToken);
	}
}
