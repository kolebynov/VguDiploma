using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	public class ByteArrayContent : IContent
	{
		private readonly byte[] byteContent;

		public IContentHash Hash => throw new NotImplementedException();

		public long Size => byteContent.LongLength;

		public ByteArrayContent(byte[] content)
		{
			byteContent = content ?? throw new ArgumentNullException(nameof(content));
		}

		public Task<Stream> OpenReadStream(CancellationToken cancellationToken) =>
			Task.FromResult((Stream)new MemoryStream(byteContent));
	}
}
