using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Exceptions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
using Diploma.IndexingService.Core.Objects;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Diploma.IndexingService.EsDocumentStorage.Interfaces;
using Microsoft.Extensions.Options;
using Nest;
using DocumentInfoModel = Diploma.IndexingService.EsDocumentStorage.Models.DocumentInfoModel;

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
					Text = textHighlighter.EscapeText(document.ExtractedText),
					FolderId = document.FolderId.ToString(),
					ParentFoldersPath = document.ParentFoldersPath.Select(x => x.ToString()).ToArray()
				}
			}, cancellationToken);

			CheckResponse(response, "Error during saving document into db");

			await contentStorage.Save(document.Id.ToString(), ContentCategory, document.Content, cancellationToken);
		}

		public async Task<IReadOnlyCollection<FoundDocument>> Search(SearchQuery searchQuery,
			User user,
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
						.Bool(bqd => bqd
							.Must(
								qcd => qcd.MultiMatch(mmqd => mmqd
									.Fields(new[] { "text", "fileName" })
									.Query(searchQuery.SearchString)),
								qcd => qcd
									.Term("id.userIdentity.keyword", user.Id))))
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

		public async Task<IReadOnlyCollection<DocumentInfo>> GetDocuments(User user, FolderIdentity parentFolderId,
			int limit, int skip, CancellationToken cancellationToken)
		{
			var response = await elasticClient.SearchAsync<DocumentInfoModel>(
				sd => sd
					.Index(options.IndexName)
					.Query(qd => qd
						.Bool(bqd => bqd
							.Must(
								qcd => qcd.Term("id.userIdentity.keyword", user.Id),
								qcd => qcd.Term("folderId", parentFolderId.ToString()))))
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

		public async Task RemoveDocuments(IReadOnlyCollection<DocumentIdentity> documentIds, CancellationToken cancellationToken)
		{
			if (documentIds == null)
			{
				throw new ArgumentNullException(nameof(documentIds));
			}

			if (documentIds.Any())
			{
				var response = await elasticClient.BulkAsync(
					sd => sd
						.Index(options.IndexName)
						.DeleteMany<DocumentInfoModel>(documentIds.Select(x => x.ToString())),
					cancellationToken);

				CheckResponse(response, "Error occured during removing documents.");
			}
		}

		public async Task RemoveDocumentsFromFolder(FolderIdentity folderId, CancellationToken cancellationToken)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException(nameof(folderId));
			}

			var response = await elasticClient.DeleteByQueryAsync<DocumentInfoModel>(dbqd => dbqd
					.Index(options.IndexName)
					.Query(qcd => qcd
						.Term("parentFoldersPath", folderId.ToString())),
				CancellationToken.None);

			CheckResponse(response, $"Error occured during removing items from folder {folderId}");
		}

		private DocumentInfo ToDocumentInfo(DocumentInfoModel source, CancellationToken cancellationToken) =>
			new DocumentInfo(
				source.Id,
				source.FileName,
				source.ModificationDate,
				new LazyContent(async () =>
					(await contentStorage.Get(source.Id.ToString(), ContentCategory, cancellationToken)).Content),
				FolderIdentity.FromString(source.FolderId),
				source.ParentFoldersPath.Select(FolderIdentity.FromString).ToList());

		private static void CheckResponse(ResponseBase response, string errorText)
		{
			if (!response.IsValid || response.ServerError != null)
			{
				throw new DocumentStorageException(errorText, response.OriginalException);
			}
		}
	}
}
