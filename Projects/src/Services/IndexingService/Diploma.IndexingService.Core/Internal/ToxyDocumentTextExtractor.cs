using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.Shared.Interfaces;
using Toxy;

namespace Diploma.IndexingService.Core.Internal
{
	internal class ToxyDocumentTextExtractor : IDocumentTextExtractor
	{
		public async Task<string> Extract(string fileName, IContent content, CancellationToken cancellationToken)
		{
			await using var stream = await content.OpenReadStream(cancellationToken);
			var path = Path.Combine(Path.GetTempPath(), fileName);
			await using (var fileStream = File.Create(path))
			{
				await stream.CopyToAsync(fileStream, cancellationToken);
			}

			var text = ParserFactory.CreateText(new ParserContext(path)).Parse();
			File.Delete(path);

			return text;
		}
	}
}
