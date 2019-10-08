using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Core
{
	public class IndexingQueue : IIndexingQueue
	{
		private readonly Channel<ChannelItem> channel;

		public IndexingQueue(IOptions<IndexingQueueOptions> options)
		{
			if (options?.Value == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			channel = Channel.CreateBounded<ChannelItem>(new BoundedChannelOptions(options.Value.QueueMaxSize)
			{
				FullMode = BoundedChannelFullMode.Wait,
				AllowSynchronousContinuations = false,
			});
		}

		public async Task Enqueue(IReadOnlyCollection<DocumentInfo> documents, CancellationToken cancellationToken)
		{
			foreach (var document in documents)
			{
				await channel.Writer.WriteAsync(new ChannelItem
				{
					NewDocument = document
				}, cancellationToken);
			}
		}

		public async Task<(DocumentInfo oldDocument, DocumentInfo newDocument)> Dequeue(CancellationToken cancellationToken)
		{
			var item = await channel.Reader.ReadAsync(cancellationToken);
			return (oldDocument: item.OldDocument, newDocument: item.NewDocument);
		}

		private struct ChannelItem
		{
			public DocumentInfo OldDocument;
			public DocumentInfo NewDocument;
		}
	}
}
