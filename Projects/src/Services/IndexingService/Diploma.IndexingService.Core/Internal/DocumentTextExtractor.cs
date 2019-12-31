using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.Shared.Interfaces;
using Toxy;

namespace Diploma.IndexingService.Core.Internal
{
	internal class DocumentTextExtractor : IDocumentTextExtractor
	{
		public async Task<string> Extract(string fileName, IContent content, CancellationToken cancellationToken)
		{
			await using var stream = await content.OpenReadStream(cancellationToken);

			try
			{
				var path = Path.Combine(Path.GetTempPath(), fileName);
				var toxyParser = ParserFactory.CreateText(new ParserContext(path));
				await using (var fileStream = File.Create(path, 81920, FileOptions.Asynchronous))
				{
					await stream.CopyToAsync(fileStream, cancellationToken);
				}

				var text = toxyParser.Parse();
				File.Delete(path);

				return text;
			}
			catch (Exception e) when (e is InvalidDataException || e is NotSupportedException)
			{
				return await ExtractAsSimpleText(stream);
			}
		}

		private static async Task<string> ExtractAsSimpleText(Stream content)
		{
			using var reader = new StreamReader(content);
			return await reader.ReadToEndAsync();
		}
	}
}
