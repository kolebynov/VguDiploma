using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IDocumentTextExtractor
	{
		Task<string> Extract(string fileName, IContent content, CancellationToken cancellationToken);
	}
}
