using System;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Commands;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.Shared.Extensions;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Diploma.IndexingService.Core
{
	internal class DocumentProcessorWorker : BackgroundService
	{
		private readonly IIndexingQueue indexingQueue;
		private readonly IDocumentProcessor documentProcessor;
		private readonly IMediator mediator;

		public DocumentProcessorWorker(IIndexingQueue indexingQueue, IDocumentProcessor documentProcessor, IMediator mediator)
		{
			this.indexingQueue = indexingQueue ?? throw new ArgumentNullException(nameof(indexingQueue));
			this.documentProcessor = documentProcessor ?? throw new ArgumentNullException(nameof(documentProcessor));
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var documents = await indexingQueue.Dequeue(stoppingToken);
				var processedDocument = await documentProcessor.Process(documents.newDocument, stoppingToken);
				await mediator.Send(new SaveDocumentsToDbCommand(processedDocument.AsArray()), stoppingToken);
			}
		}
	}
}
