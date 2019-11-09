using System;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Configuration;
using Diploma.IndexingService.Api.Exceptions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Exceptions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Api.Internal
{
	public class TempContentStorage : ITempContentStorage
	{
		private readonly TempContentStorageOptions options;
		private readonly IContentStorage contentStorage;
		private readonly ILogger<TempContentStorage> logger;

		public TempContentStorage(
			IOptions<TempContentStorageOptions> options,
			IContentStorage contentStorage,
			ILogger<TempContentStorage> logger)
		{
			this.contentStorage = contentStorage ?? throw new ArgumentNullException(nameof(contentStorage));
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task<string> SaveTempContent(IContent content, CancellationToken cancellationToken) => 
			(await contentStorage.Save(Guid.NewGuid().ToString(), options.ContentCategory, content, cancellationToken)).Id;

		public async Task<IContent> GetTempContent(string token, CancellationToken cancellationToken)
		{
			try
			{
				return (await contentStorage.Get(token, options.ContentCategory, cancellationToken)).Content;
			}
			catch (ContentNotFoundException e)
			{
				throw new TempContentRemovedException("Temp content not found or has been removed by timeout", e);
			}
		}
	}
}
