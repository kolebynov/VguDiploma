using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Database;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.EntityFrameworkCore;

namespace Diploma.IndexingService.Core.Internal
{
	internal class EfFoldersStorage : IFoldersStorage
	{
		private readonly DatabaseContext context;

		public EfFoldersStorage(DatabaseContext context)
		{
			this.context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IReadOnlyCollection<Folder>> GetFolders(FolderIdentity parentFolderId, int limit, int skip,
			CancellationToken cancellationToken) =>
			await context.Folders
				.Where(x => x.ParentId == parentFolderId)
				.OrderBy(x => x.Name)
				.Skip(skip)
				.Take(limit)
				.ToArrayAsync(cancellationToken);

		public async Task<Folder> GetFolder(FolderIdentity folderId, CancellationToken cancellationToken) => 
			await context.Folders.FindAsync(new object[] { folderId }, cancellationToken);

		public Task<int> GetFoldersCount(FolderIdentity parentFolderId, CancellationToken cancellationToken) => 
			context.Folders.CountAsync(x => x.ParentId == parentFolderId, cancellationToken);

		public async Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken)
		{
			if (await context.Folders.AnyAsync(x => x.Name == folder.Name && x.ParentId == folder.ParentId,
				cancellationToken))
			{
				throw new InvalidOperationException($"Folder with name {folder.Name} already exists in this folder");
			}

			context.Folders.Add(folder);
			await context.SaveChangesAsync(cancellationToken);

			return folder;
		}
	}
}
