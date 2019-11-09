using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	public class ByteArrayContent : IContent
	{
		private readonly Lazy<Task<byte[]>> contentLazy;

		public IContentHash Hash => throw new NotImplementedException();

		public ByteArrayContent(Func<Task<byte[]>> contentProvider)
		{
			if (contentProvider == null)
			{
				throw new ArgumentNullException(nameof(contentProvider));
			}

			contentLazy = new Lazy<Task<byte[]>>(contentProvider);
		}

		public ByteArrayContent(byte[] content)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			contentLazy = new Lazy<Task<byte[]>>(() => Task.FromResult(content));
		}

		public async Task<Stream> OpenReadStream(CancellationToken cancellationToken) => new MemoryStream(await contentLazy.Value);
	}
}
