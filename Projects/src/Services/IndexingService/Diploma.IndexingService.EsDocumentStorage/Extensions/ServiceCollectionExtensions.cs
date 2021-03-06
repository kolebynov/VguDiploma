﻿using System;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Diploma.IndexingService.EsDocumentStorage.Interfaces;
using Diploma.IndexingService.EsDocumentStorage.Internal;
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
				var connectionSettings = new ConnectionSettings(pool, JsonNetSerializer.Default)
					.MaximumRetries(10);

				var elasticClient = new ElasticClient(connectionSettings);
				InitIndex(elasticClient, options);
				return elasticClient;
			});
			services.AddSingleton<ITextHighlighter, TextHighlighter>();

			services.AddScoped<IDocumentStorage, DocumentStorage>();

			return services;
		}

		private static void InitIndex(IElasticClient elasticClient, DocumentStorageOptions options)
		{
			if (!elasticClient.Indices.Exists(options.IndexName).Exists)
			{
				var response = elasticClient.Indices.Create(
					options.IndexName,
					cid => cid
						.Settings(isd => isd
							.Setting("index.highlight.max_analyzed_offset", options.MaxAnalyzedOffsetForHighlighting)));
				if (!response.IsValid)
				{
					throw new InvalidOperationException("Unable to create index", response.OriginalException);
				}
			}
		}
	}
}
