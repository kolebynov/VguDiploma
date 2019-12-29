using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.IndexingService.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FoldersController : ControllerBase
	{
		private readonly IFoldersStorage foldersStorage;
		private readonly IUserService userService;
		private readonly IDocumentStorage documentStorage;

		public FoldersController(IFoldersStorage foldersStorage, IUserService userService,
			IDocumentStorage documentStorage)
		{
			this.foldersStorage = foldersStorage ?? throw new ArgumentNullException(nameof(foldersStorage));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
			this.documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
		}

		[HttpGet("{id}/items")]
		public async Task<PaginationApiResult<IReadOnlyCollection<GetFolderItem>>> GetFolderItems(Guid id, [FromQuery] GetQuery query)
		{
			var currentUser = await userService.GetCurrentUser();

			var parentFolderId = new FolderIdentity(id, currentUser.Id);
			var folders = await foldersStorage.GetFolders(parentFolderId, query.Limit, query.Skip,
				CancellationToken.None);
			IReadOnlyCollection<DocumentInfo> documents = new List<DocumentInfo>();

			var foldersCount = await foldersStorage.GetFoldersCount(parentFolderId, CancellationToken.None);

			if (folders.Count < query.Limit)
			{
				var limitForDocuments = query.Limit - folders.Count;
				var skipForDocuments = Math.Max(query.Skip - foldersCount, 0);
				documents = await documentStorage.GetDocuments(currentUser, parentFolderId, limitForDocuments,
					skipForDocuments, CancellationToken.None);
			}

			var result = folders
				.Select(f => new GetFolderItem
				{
					Folder = f.ToDto()
				})
				.Concat(documents
					.Select(d => new GetFolderItem
					{
						Document = new GetDocumentDto
						{
							Id = d.Id.GetClientId(),
							FileName = d.FileName,
							ModificationDate = d.ModificationDate
						}
					}))
				.ToArray();

			return ApiResult.SuccessPaginationResult(
				(IReadOnlyCollection<GetFolderItem>)result,
				new PaginationData
				{
					TotalCount = foldersCount + await documentStorage.GetDocumentsCount(parentFolderId, CancellationToken.None)
				});
		}

		[HttpGet("{folderId}")]
		public async Task<ApiResult<GetFolderDto>> GetFolder(Guid folderId)
		{
			var folder = await foldersStorage.GetFolder(
				new FolderIdentity(folderId, (await userService.GetCurrentUser()).Id), CancellationToken.None);

			return ApiResult.SuccessResultWithData(folder.ToDto());
		}

		[HttpPost]
		public async Task<ApiResult<GetFolderDto>> AddFolder(AddFolderDto addFolder)
		{
			var currentUser = await userService.GetCurrentUser();
			var parentFolder = await foldersStorage.GetFolder(
				new FolderIdentity(addFolder.ParentId, currentUser.Id),
				CancellationToken.None);
			var newFolderId = new FolderIdentity(Guid.NewGuid(), currentUser.Id);
			var newFolder = await foldersStorage.AddFolder(
				new Folder(newFolderId, addFolder.Name, parentFolder.Id,
					parentFolder.ParentsPath.Concat(parentFolder.Id.AsArray()).ToList()),
				CancellationToken.None);

			return ApiResult.SuccessResultWithData(newFolder.ToDto());
		}

		[HttpDelete("items")]
		public async Task<ApiResult> RemoveFolderItems([FromBody] IReadOnlyCollection<GetFolderItem> itemsToRemove)
		{
			var currentUser = await userService.GetCurrentUser();
			var foldersToRemove = itemsToRemove
				.Where(x => x.Folder != null)
				.Select(x => new FolderIdentity(x.Folder.Id, currentUser.Id))
				.ToArray();
			await foldersStorage.RemoveFolders(
				foldersToRemove,
				CancellationToken.None);

			foreach (var folderId in foldersToRemove)
			{
				await documentStorage.RemoveDocumentsFromFolder(folderId, CancellationToken.None);
			}
			
			await documentStorage.RemoveDocuments(
				itemsToRemove
					.Where(x => x.Document != null)
					.Select(x => new DocumentIdentity(x.Document.Id, currentUser.Id))
					.ToArray(),
				CancellationToken.None);

			return ApiResult.SuccessResult;
		}
	}
}
