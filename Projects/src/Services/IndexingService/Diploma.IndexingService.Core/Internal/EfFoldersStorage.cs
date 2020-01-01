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
			(await context.Folders
				.Where(x => x.ParentId == parentFolderId.Id && x.UserIdentity == parentFolderId.UserIdentity)
				.OrderBy(x => x.Name)
				.Skip(skip)
				.Take(limit)
				.ToArrayAsync(cancellationToken))
				.Select(ToModel)
				.ToArray();

		public async Task<Folder> GetFolder(FolderIdentity folderId, CancellationToken cancellationToken) => 
			ToModel(await context.Folders.FindAsync(new object[] { folderId.Id, folderId.UserIdentity }, cancellationToken));

		public Task<int> GetFoldersCount(FolderIdentity parentFolderId, CancellationToken cancellationToken) => 
			context.Folders.CountAsync(x => x.ParentId == parentFolderId.Id && x.UserIdentity == parentFolderId.UserIdentity, cancellationToken);

		public async Task<Folder> AddFolder(Folder folder, CancellationToken cancellationToken)
		{
			var parentFolderId = folder.ParentId?.Id;
			if (await context.Folders.AnyAsync(x => x.Name == folder.Name && x.ParentId == parentFolderId && x.UserIdentity == folder.Id.UserIdentity,
				cancellationToken))
			{
				throw new InvalidOperationException($"Folder with name {folder.Name} already exists in this folder");
			}

			context.Folders.Add(ToDbItem(folder));
			await context.SaveChangesAsync(cancellationToken);

			return folder;
		}

		public Task RemoveFolders(IReadOnlyCollection<FolderIdentity> folderIds, CancellationToken cancellationToken)
		{
			return ResilientTransaction.New(context).ExecuteAsync(async () =>
			{
				foreach (var folderId in folderIds)
				{
					await RemoveFolder(await context.Folders.FindAsync(
						new object[] { folderId.Id, folderId.UserIdentity }, cancellationToken), cancellationToken);
				}

				await context.SaveChangesAsync(cancellationToken);
			});
		}

		private async Task RemoveFolder(FolderDbItem folder, CancellationToken cancellationToken)
		{
			List<FolderDbItem> subFolders;
			int skip = 0;
			int limit = 1000;

			do
			{
				subFolders = await context.Folders
					.Where(x => x.ParentId == folder.Id && x.UserIdentity == folder.UserIdentity)
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

		private static FolderDbItem ToDbItem(Folder folder) => new FolderDbItem
		{
			Id = folder.Id.Id,
			UserIdentity = folder.Id.UserIdentity,
			ParentId = folder.ParentId?.Id,
			Name = folder.Name,
			ParentsPath = string.Join(",", folder.ParentsPath?.Select(y => y.Id.ToString()) ?? Enumerable.Empty<string>())
		};

		private static Folder ToModel(FolderDbItem dbItem) => new Folder(
			new FolderIdentity(dbItem.Id, dbItem.UserIdentity),
			dbItem.Name,
			dbItem.ParentId != null? new FolderIdentity(dbItem.ParentId.Value, dbItem.UserIdentity) : null,
			dbItem.ParentsPath.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => new FolderIdentity(new Guid(x), dbItem.UserIdentity)).ToList());
	}
}
