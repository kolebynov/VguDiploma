using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	public class ByteArrayContent : IContent
	{
		private readonly byte[] content;

		public IContentHash Hash => throw new NotImplementedException();

		public ByteArrayContent(byte[] content)
		{
			this.content = content ?? throw new ArgumentNullException(nameof(content));
		}

		public Task<Stream> OpenReadStream(CancellationToken cancellationToken) => Task.FromResult((Stream)new MemoryStream(content));
	}
}
