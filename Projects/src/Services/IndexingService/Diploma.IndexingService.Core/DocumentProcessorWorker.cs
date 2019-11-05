using System;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Diploma.IndexingService.Core
{
	public class DocumentProcessorWorker : BackgroundService
	{
		private readonly IIndexingQueue indexingQueue;
		private readonly ITextExtractor textExtractor;
		private readonly IDocumentStorage documentStorage;

		public DocumentProcessorWorker(IIndexingQueue indexingQueue, ITextExtractor textExtractor, IDocumentStorage documentStorage)
		{
			this.indexingQueue = indexingQueue ?? throw new ArgumentNullException(nameof(indexingQueue));
			this.textExtractor = textExtractor ?? throw new ArgumentNullException(nameof(textExtractor));
			this.documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var document = await indexingQueue.Dequeue(stoppingToken);
				var extractedText = await textExtractor.Extract(document.FileName, document.Content.OpenReadStream(), stoppingToken);
				await documentStorage.SaveDocumentToDb(document, extractedText);
			}
		}
	}
}
