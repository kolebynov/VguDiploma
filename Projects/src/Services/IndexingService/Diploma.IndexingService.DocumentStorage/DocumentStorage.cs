using System;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Microsoft.Extensions.Options;
using Nest;
using DocumentInfo = Diploma.IndexingService.EsDocumentStorage.Models.DocumentInfo;

namespace Diploma.IndexingService.EsDocumentStorage
{
	internal class DocumentStorage : IDocumentStorage
	{
		private const string ContentCategory = nameof(DocumentStorage);

		private readonly IElasticClient elasticClient;
		private readonly DocumentStorageOptions options;
		private readonly IContentStorage contentStorage;

		public DocumentStorage(IElasticClient elasticClient, IOptions<DocumentStorageOptions> options, IContentStorage contentStorage)
		{
			this.elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
			this.contentStorage = contentStorage ?? throw new ArgumentNullException(nameof(contentStorage));
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task SaveDocumentToDb(FullDocumentInfo document, CancellationToken cancellationToken)
		{
			var response = await elasticClient.IndexAsync(new IndexRequest<DocumentInfo>(options.IndexName, document.Id.ToString())
			{
				Document = new DocumentInfo
				{
					Id = document.Id,
					ModificationDate = document.ModificationDate,
					FileName = document.FileName,
					Text = document.ExtractedText
				}
			}, cancellationToken);

			if (!response.IsValid)
			{
				throw new InvalidOperationException("error");
			}

			await contentStorage.Save(document.Id.ToString(), ContentCategory, document.Content, cancellationToken);
		}
	}
}
