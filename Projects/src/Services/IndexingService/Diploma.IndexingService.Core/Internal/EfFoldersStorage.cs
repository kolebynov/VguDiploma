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

		public Task RemoveFolders(IReadOnlyCollection<FolderIdentity> folderIds, CancellationToken cancellationToken)
		{
			return ResilientTransaction.New(context).ExecuteAsync(async () =>
			{
				foreach (var folderId in folderIds)
				{
					await RemoveFolder(await GetFolder(folderId, cancellationToken), cancellationToken);
				}

				await context.SaveChangesAsync(cancellationToken);
			});
		}

		private async Task RemoveFolder(Folder folder, CancellationToken cancellationToken)
		{
			List<Folder> subFolders;
			int skip = 0;
			int limit = 1000;

			do
			{
				subFolders = await context.Folders
					.Where(x => x.ParentId == folder.Id)
					.Take(limit)
					.Skip(skip)
					.ToListAsync(cancellationToken);
				foreach (var subFolder in subFolders)
				{
					await RemoveFolder(subFolder, cancellationToken);
				}

				skip += limit;
			}
			while (subFolders.Any());

			context.Folders.Remove(folder);
		}
	}
}
