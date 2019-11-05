using System.IO;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IContent
	{
		IContentHash Hash { get; }

		Stream OpenReadStream();
	}
}
