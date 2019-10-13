using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Commands;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using MediatR;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Core
{
	public class IndexingQueue : IIndexingQueue
	{
		private readonly Channel<(DocumentInfo oldDocument, DocumentInfo newDocument)> channel;
		private readonly IMediator mediator;

		public IndexingQueue(IOptions<IndexingQueueOptions> options, IMediator mediator)
		{
			if (options?.Value == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

			channel = Channel.CreateBounded<(DocumentInfo oldDocument, DocumentInfo newDocument)>(new BoundedChannelOptions(options.Value.QueueMaxSize)
			{
				FullMode = BoundedChannelFullMode.Wait,
				AllowSynchronousContinuations = false,
			});
		}

		public async Task Enqueue(IReadOnlyCollection<DocumentInfo> documents, CancellationToken cancellationToken)
		{
			var oldDocuments = (await mediator.Send(
				new ReadDocumentsFromDbCommand(documents.Select(x => x.Id).ToArray()),
				cancellationToken)).Documents;

			foreach (var document in documents)
			{
				await channel.Writer.WriteAsync(
					(oldDocument: oldDocuments.First(x => x.Id == document.Id), newDocument: document),
					cancellationToken);
			}
		}

		public async Task<(DocumentInfo oldDocument, DocumentInfo newDocument)> Dequeue(CancellationToken cancellationToken) => 
			await channel.Reader.ReadAsync(cancellationToken);
	}
}
