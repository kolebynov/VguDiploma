using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Configuration;
using Diploma.IndexingService.Core;
using Diploma.IndexingService.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Api.Internal
{
	public class TempContentBackgroundService : BackgroundServiceWithScope
	{
		private readonly TempContentStorageOptions options;
		private readonly ILogger<TempContentBackgroundService> logger;

		public TempContentBackgroundService(
			IServiceProvider serviceProvider,
			IOptions<TempContentStorageOptions> options,
			ILogger<TempContentBackgroundService> logger)
			: base(serviceProvider)
		{
			this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		protected override async Task ExecuteWithScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await RemoveExpiredContent(serviceProvider.GetRequiredService<IContentStorage>(), stoppingToken);
				}
				catch (Exception e)
				{
					logger.LogError(e, "Error while removing expired temp content");
				}

				await Task.Delay(options.CheckTimeout, stoppingToken);
			}
		}

		private async Task RemoveExpiredContent(IContentStorage contentStorage, CancellationToken cancellationToken)
		{
			await foreach (var page in contentStorage.GetAll(options.ContentCategory, cancellationToken))
			{
				foreach (var item in page.Where(x => x.Timestamp.Add(options.ContentSavePeriod) < DateTimeOffset.UtcNow))
				{
					await contentStorage.Delete(item.Id, options.ContentCategory, cancellationToken);
				}
			}
		}
	}
}
