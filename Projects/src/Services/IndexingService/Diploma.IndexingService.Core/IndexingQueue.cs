using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core
{
	internal class IndexingQueue : IIndexingQueue
	{
		private readonly Channel<DocumentInfo> channel;
		private readonly IInProgressDocumentsStorage inProgressDocumentsStorage;

		public IndexingQueue(IInProgressDocumentsStorage inProgressDocumentsStorage, Channel<DocumentInfo> channel)
		{
			this.inProgressDocumentsStorage = inProgressDocumentsStorage ?? throw new ArgumentNullException(nameof(inProgressDocumentsStorage));
			this.channel = channel ?? throw new ArgumentNullException(nameof(channel));
		}

		public async Task Enqueue(IReadOnlyCollection<DocumentInfo> documents, CancellationToken cancellationToken)
		{
			foreach (var document in documents)
			{
				await inProgressDocumentsStorage.UpdateState(document, InProcessDocumentState.InQueue, cancellationToken);
				await channel.Writer.WriteAsync(document, cancellationToken);
			}
		}

		public async Task<DocumentInfo> Dequeue(CancellationToken cancellationToken) => 
			await channel.Reader.ReadAsync(cancellationToken);
	}
}
