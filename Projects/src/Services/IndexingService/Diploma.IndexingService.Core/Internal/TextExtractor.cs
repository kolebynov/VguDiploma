using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	internal class TextExtractor : ITextExtractor
	{
		public async Task<string> Extract(string fileName, IContent content, CancellationToken cancellationToken)
		{
			using var textReader = new StreamReader(await content.OpenReadStream(cancellationToken));
			return await textReader.ReadToEndAsync();
		}
	}
}
