using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IFoldersStorage
	{
		Task<IReadOnlyCollection<Folder>> GetFolders(FolderIdentity parentFolderId, int limit, int skip,
			CancellationToken cancellationToken);

		Task<Folder> GetFolder(FolderIdentity folderId, CancellationToken cancellationToken);

		Task<int> GetFoldersCount(FolderIdentity parentFolderId, CancellationToken cancellationToken);

		Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken);
	}
}
