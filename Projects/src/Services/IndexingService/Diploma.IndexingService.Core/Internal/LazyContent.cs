using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	public class LazyContent : IContent
	{
		private readonly Lazy<Task<IContent>> innerLazyContent;

		public IContentHash Hash => throw new NotImplementedException();

		public LazyContent(Func<Task<IContent>> contentProvider)
		{
			innerLazyContent = new Lazy<Task<IContent>>(contentProvider);
		}

		public async Task<Stream> OpenReadStream(CancellationToken cancellationToken) => 
			await (await innerLazyContent.Value).OpenReadStream(cancellationToken);
	}
}
