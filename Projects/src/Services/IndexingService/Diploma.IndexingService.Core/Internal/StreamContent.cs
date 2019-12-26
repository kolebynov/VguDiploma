using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	internal class StreamContent : IContent
	{
		private readonly Stream stream;

		public IContentHash Hash { get; }

		public long Size { get; }

		public StreamContent(Stream stream, long size)
		{
			this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
			Size = size;
		}

		public Task<Stream> OpenReadStream(CancellationToken cancellationToken) => 
			Task.FromResult(stream);
	}
}
