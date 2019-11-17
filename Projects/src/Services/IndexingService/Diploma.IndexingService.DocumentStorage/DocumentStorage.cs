using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
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

		public async Task<IReadOnlyCollection<FoundDocument>> Search(SearchQuery searchQuery,
			CancellationToken cancellationToken)
		{
			var response = await elasticClient.SearchAsync<DocumentInfo>(
				sd => sd
					.Index(options.IndexName)
					.Query(qd => qd
						.MultiMatch(mmqd => mmqd
							.Fields(new[] { "text", "fileName" })
							.Query(searchQuery.SearchString)))
					.Highlight(hs => hs
						.Fields(hfd => hfd.Field("text"), hfd => hfd.Field("fileName")))
					.Source(sfd => sfd.Includes(fd => fd.Field("fileName"))),
				cancellationToken);

			return response.Hits.Select(hit => new FoundDocument
			{
				DocumentId = DocumentIdentity.FromString(hit.Id),
				Matches = hit.Highlight,
				FileName = hit.Source.FileName
			}).ToArray();
		}

		public async Task<IReadOnlyCollection<Core.Objects.DocumentInfo>> GetDocuments(User user, int limit, int skip,
			CancellationToken cancellationToken)
		{
			var response = await elasticClient.SearchAsync<DocumentInfo>(
				sd => sd
					.Index(options.IndexName)
					.Query(qd => qd
						.Term("id.userIdentity.keyword", user.Id))
					.Source(sfd => sfd.Excludes(fd => fd.Field("text")))
					.Take(limit)
					.Skip(skip),
				cancellationToken);

			return response.Hits.Select(x => new Core.Objects.DocumentInfo(
					x.Source.Id,
					x.Source.FileName,
					x.Source.ModificationDate,
					new LazyContent(async () =>
						(await contentStorage.Get(x.Source.Id.ToString(), ContentCategory, cancellationToken)).Content)))
				.ToArray();
		}
	}
}
