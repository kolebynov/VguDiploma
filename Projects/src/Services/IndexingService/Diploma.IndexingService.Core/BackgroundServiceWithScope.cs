using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Diploma.IndexingService.Core
{
	public abstract class BackgroundServiceWithScope : BackgroundService
	{
		private readonly IServiceProvider serviceProvider;

		protected BackgroundServiceWithScope(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var scope = serviceProvider.CreateScope();
			await ExecuteWithScopeAsync(scope.ServiceProvider, stoppingToken);
		}

		protected abstract Task ExecuteWithScopeAsync(IServiceProvider serviceProvider, CancellationToken stoppingToken);
	}
}
