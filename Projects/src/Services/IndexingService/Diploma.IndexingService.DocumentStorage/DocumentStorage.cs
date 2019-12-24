using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
using Diploma.IndexingService.Core.Objects;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Diploma.IndexingService.EsDocumentStorage.Interfaces;
using Microsoft.Extensions.Options;
using Nest;
using DocumentInfoModel = Diploma.IndexingService.EsDocumentStorage.Models.DocumentInfo;

namespace Diploma.IndexingService.EsDocumentStorage
{
	internal class DocumentStorage : IDocumentStorage
	{
		private const string ContentCategory = nameof(DocumentStorage);

		private readonly IElasticClient elasticClient;
		private readonly DocumentStorageOptions options;
		private readonly IContentStorage contentStorage;
		private readonly ITextHighlighter textHighlighter;

		public DocumentStorage(
			IElasticClient elasticClient,
			IOptions<DocumentStorageOptions> options,
			IContentStorage contentStorage,
			ITextHighlighter textHighlighter)
		{
			this.elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
			this.contentStorage = contentStorage ?? throw new ArgumentNullException(nameof(contentStorage));
			this.textHighlighter = textHighlighter ?? throw new ArgumentNullException(nameof(textHighlighter));
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task SaveDocumentToDb(FullDocumentInfo document, CancellationToken cancellationToken)
		{
			var response = await elasticClient.IndexAsync(new IndexRequest<DocumentInfoModel>(options.IndexName, document.Id.ToString())
			{
				Document = new DocumentInfoModel
				{
					Id = document.Id,
					ModificationDate = document.ModificationDate,
					FileName = document.FileName,
					Text = textHighlighter.EscapeText(document.ExtractedText)
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
			if (searchQuery == null)
			{
				throw new ArgumentNullException(nameof(searchQuery));
			}

			var response = await elasticClient.SearchAsync<DocumentInfoModel>(
				sd => sd
					.Index(options.IndexName)
					.Query(qd => qd
						.MultiMatch(mmqd => mmqd
							.Fields(new[] { "text", "fileName" })
							.Query(searchQuery.SearchString)))
					.Highlight(hs => hs
						.Fields(hfd => hfd.Field("text"), hfd => hfd.Field("fileName")))
					.Source(sfd => sfd.Includes(fd => fd.Field("fileName")))
					.Skip(searchQuery.Skip)
					.Take(searchQuery.Limit),
				cancellationToken);

			return response.Hits.Select(hit => new FoundDocument
			{
				DocumentId = DocumentIdentity.FromString(hit.Id),
				Matches = hit.Highlight
					.ToDictionary(
						x => x.Key,
						x => (IReadOnlyCollection<IReadOnlyCollection<DocumentTextEntry>>)
							x.Value.Select(y => textHighlighter.ParseHighlightedText(y)).ToArray()),
				FileName = hit.Source.FileName
			}).ToArray();
		}

		public async Task<IReadOnlyCollection<DocumentInfo>> GetDocuments(User user, int limit, int skip,
			CancellationToken cancellationToken)
		{
			var response = await elasticClient.SearchAsync<DocumentInfoModel>(
				sd => sd
					.Index(options.IndexName)
					.Query(qd => qd
						.Term("id.userIdentity.keyword", user.Id))
					.Source(sfd => sfd.Excludes(fd => fd.Field("text")))
					.Take(limit)
					.Skip(skip),
				cancellationToken);

			return response.Hits.Select(x => ToDocumentInfo(x.Source, cancellationToken)).ToArray();
		}

		public async Task<DocumentInfo> GetDocument(
			DocumentIdentity id,
			CancellationToken cancellationToken)
		{
			var response = await elasticClient.GetAsync<DocumentInfoModel>(
				id.ToString(),
				selector => selector
					.Index(options.IndexName)
					.SourceExcludes("text"),
				cancellationToken);

			return ToDocumentInfo(response.Source, cancellationToken);
		}

		private DocumentInfo ToDocumentInfo(DocumentInfoModel source, CancellationToken cancellationToken) =>
			new DocumentInfo(
				source.Id,
				source.FileName,
				source.ModificationDate,
				new LazyContent(async () =>
					(await contentStorage.Get(source.Id.ToString(), ContentCategory, cancellationToken)).Content));
	}
}
