using System;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using Nest.JsonNetSerializer;

namespace Diploma.IndexingService.EsDocumentStorage.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddEsDocumentStorage(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddSingleton<IElasticClient>(sp =>
			{
				var options = sp.GetRequiredService<IOptions<DocumentStorageOptions>>().Value;
				var pool = new SingleNodeConnectionPool(options.ElasticSearchUri);
				var connectionSettings = new ConnectionSettings(pool, JsonNetSerializer.Default);

				return new ElasticClient(connectionSettings);
			});
			services.AddScoped<IDocumentStorage, DocumentStorage>();

			return services;
		}
	}
}
