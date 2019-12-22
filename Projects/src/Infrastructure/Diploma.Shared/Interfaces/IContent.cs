using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Diploma.Shared.Interfaces
{
	public interface IContent
	{
		IContentHash Hash { get; }

		long Size { get; }

		Task<Stream> OpenReadStream(CancellationToken cancellationToken);
	}
}
