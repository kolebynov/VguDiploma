using System;
using Diploma.IndexingService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.IndexingService.DocumentStorage.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDocumentStorage(this IServiceCollection services, string mssqlConnectionString)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddDbContext<DbContext>(opt => opt.UseInMemoryDatabase("123"), ServiceLifetime.Singleton);
				
			services.AddSingleton<IDocumentStorage, DocumentStorage>();

			return services;
		}
	}
}
