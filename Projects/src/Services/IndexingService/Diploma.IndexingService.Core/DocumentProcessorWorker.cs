using System;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Diploma.IndexingService.Core
{
	internal class DocumentProcessorWorker : BackgroundServiceWithScope
	{
		private readonly IIndexingQueue indexingQueue;
		private readonly IDocumentTextExtractor textExtractor;
		private readonly ILogger<DocumentProcessorWorker> logger;

		public DocumentProcessorWorker(
			IIndexingQueue indexingQueue,
			IDocumentTextExtractor textExtractor,
			ILogger<DocumentProcessorWorker> logger,
			IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			this.indexingQueue = indexingQueue ?? throw new ArgumentNullException(nameof(indexingQueue));
			this.textExtractor = textExtractor ?? throw new ArgumentNullException(nameof(textExtractor));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected override async Task ExecuteWithScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
		{
			var documentStorage = serviceProvider.GetRequiredService<IDocumentStorage>();

			while (!stoppingToken.IsCancellationRequested)
			{
				var document = await indexingQueue.Dequeue(stoppingToken);

				try
				{
					logger.LogInformation("Start processing document {documentId} ({documentFileName})", document.Id, document.FileName);
					var extractedText = await textExtractor.Extract(document.FileName, document.Content, stoppingToken);
					await documentStorage.SaveDocumentToDb(new FullDocumentInfo(document, extractedText), stoppingToken);
				}
				catch (Exception e)
				{
					logger.LogError(e, "Error while processing document {documentId} ({documentFileName})", document.Id, document.FileName);
				}
			}
		}
	}
}
