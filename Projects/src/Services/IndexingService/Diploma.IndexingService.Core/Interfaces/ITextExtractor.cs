using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface ITextExtractor
	{
		Task<string> Extract(string fileName, Stream content, CancellationToken cancellationToken);
	}
}
