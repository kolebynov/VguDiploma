using System;
using Diploma.IndexingService.Core.Internal.ContentStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.IndexingService.Core.Extensions
{
	public static class ServiceProviderExtensions
	{
		public static IServiceProvider MigrateContentDatabase(this IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			using var scope = serviceProvider.CreateScope();
			scope.ServiceProvider.GetRequiredService<ContentStorageContext>().Database.Migrate();

			return serviceProvider;
		}
	}
}
