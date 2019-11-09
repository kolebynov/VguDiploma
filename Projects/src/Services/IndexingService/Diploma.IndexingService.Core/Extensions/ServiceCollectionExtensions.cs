using System;
using System.Reflection;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
using Diploma.IndexingService.Core.Internal.ContentStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Core.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddHostedService<DocumentProcessorWorker>();

			services.AddDbContext<ContentStorageContext, ContentStorageContext>(
				(sp, opt) => opt.UseSqlServer(
					sp.GetRequiredService<IOptions<CoreOptions>>().Value.ContentStorage.MsSqlConnectionString,
					sqlOpt => sqlOpt.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));

			services.AddSingleton<IIndexingQueue, IndexingQueue>();
			services.AddSingleton<ITextExtractor, TextExtractor>();

			services.AddScoped<IContentStorage, EfContentStorage>();

			return services;
		}
	}
}
