using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Api.Interfaces
{
	public interface ITempContentStorage
	{
		Task<string> SaveTempContent(IContent content, CancellationToken cancellationToken);

		Task<IContent> GetTempContent(string token, CancellationToken cancellationToken);
	}
}
