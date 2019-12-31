using System;
using Diploma.IndexingService.Core.Database;
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

			serviceProvider.GetRequiredService<DatabaseContext>().Database.Migrate();

			return serviceProvider;
		}
	}
}
