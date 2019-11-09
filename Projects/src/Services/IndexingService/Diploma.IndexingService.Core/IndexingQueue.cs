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
	internal class IndexingQueue : IIndexingQueue
	{
		private readonly Channel<DocumentInfo> channel;

		public IndexingQueue(IOptions<IndexingQueueOptions> options)
		{
			if (options?.Value == null)
			{
				throw new ArgumentNullException(nameof(options));
			}

			channel = Channel.CreateBounded<DocumentInfo>(new BoundedChannelOptions(options.Value.QueueMaxSize > 0 ? options.Value.QueueMaxSize : 10)
			{
				FullMode = BoundedChannelFullMode.Wait,
				AllowSynchronousContinuations = false,
			});
		}

		public async Task Enqueue(IReadOnlyCollection<DocumentInfo> documents, CancellationToken cancellationToken)
		{
			foreach (var document in documents)
			{
				await channel.Writer.WriteAsync(document, cancellationToken);
			}
		}

		public async Task<DocumentInfo> Dequeue(CancellationToken cancellationToken) => 
			await channel.Reader.ReadAsync(cancellationToken);
	}
}
