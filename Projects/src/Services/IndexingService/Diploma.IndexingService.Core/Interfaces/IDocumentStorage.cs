using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IDocumentStorage
	{
		Task SaveDocumentToDb(FullDocumentInfo document, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<FoundDocument>> Search(SearchQuery searchQuery, User user,
			CancellationToken cancellationToken);

		Task<IReadOnlyCollection<DocumentInfo>> GetDocuments(User user, FolderIdentity parentFolderId, int limit,
			int skip,
			CancellationToken cancellationToken);

		Task<DocumentInfo> GetDocument(DocumentIdentity id, CancellationToken cancellationToken);

		Task RemoveDocuments(IReadOnlyCollection<DocumentIdentity> documentIds, CancellationToken cancellationToken);

		Task RemoveDocumentsFromFolder(FolderIdentity folderId, CancellationToken cancellationToken);
	}
}
