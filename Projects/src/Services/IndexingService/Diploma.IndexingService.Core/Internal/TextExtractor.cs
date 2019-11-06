using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	public class TextExtractor : ITextExtractor
	{
		public Task<string> Extract(string fileName, IContent content, CancellationToken cancellationToken)
		{
			using var textReader = new StreamReader(content.OpenReadStream());
			return textReader.ReadToEndAsync();
		}
	}
}
