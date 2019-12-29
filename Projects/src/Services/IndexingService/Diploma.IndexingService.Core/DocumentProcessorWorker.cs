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
		private readonly IDocumentTextExtractor textExtractor;
		private readonly ILogger<DocumentProcessorWorker> logger;

		public DocumentProcessorWorker(
			IDocumentTextExtractor textExtractor,
			ILogger<DocumentProcessorWorker> logger,
			IServiceProvider serviceProvider)
			: base(serviceProvider)
		{
			this.textExtractor = textExtractor ?? throw new ArgumentNullException(nameof(textExtractor));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected override async Task ExecuteWithScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
		{
			var documentStorage = serviceProvider.GetRequiredService<IDocumentStorage>();
			var inProgressDocumentsStorage = serviceProvider.GetRequiredService<IInProgressDocumentsStorage>();
			var indexingQueue = serviceProvider.GetRequiredService<IIndexingQueue>();

			while (!stoppingToken.IsCancellationRequested)
			{
				var document = await indexingQueue.Dequeue(stoppingToken);

				try
				{
					logger.LogInformation("Start processing document {documentId} ({documentFileName})", document.Id, document.FileName);
					await inProgressDocumentsStorage.UpdateState(document, InProcessDocumentState.Processing, stoppingToken);
					var extractedText = await textExtractor.Extract(document.FileName, document.Content, stoppingToken);
					await documentStorage.SaveDocumentToDb(new FullDocumentInfo(document, extractedText), stoppingToken);
					await inProgressDocumentsStorage.UpdateState(document, InProcessDocumentState.Done, stoppingToken);
				}
				catch (Exception e)
				{
					logger.LogError(e, "Error while processing document {documentId} ({documentFileName})", document.Id, document.FileName);
					await inProgressDocumentsStorage.SetErrorState(document, e.Message, stoppingToken);
				}
			}
		}
	}
}
