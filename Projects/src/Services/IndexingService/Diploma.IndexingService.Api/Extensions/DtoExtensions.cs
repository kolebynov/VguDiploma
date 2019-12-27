using System;
using System.Linq;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Extensions;
using Diploma.Shared.Interfaces;
using FoundDocumentDto = Diploma.IndexingService.Api.Dto.FoundDocumentDto;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class DtoExtensions
	{
		public static DocumentInfo ToDocumentInfo(this AddDocumentDto document, IContent content, User currentUser,
			Folder parentFolder)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			if (currentUser == null)
			{
				throw new ArgumentNullException(nameof(currentUser));
			}

			return new DocumentInfo(new DocumentIdentity(document.Id, currentUser.Id), document.FileName, document.ModificationDate, content,
				parentFolder.Id, parentFolder.ParentsPath.Concat(parentFolder.Id.AsArray()).ToList());
		}

		public static FoundDocumentDto ToDto(this FoundDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			return new FoundDocumentDto
			{
				Id = document.DocumentId.GetClientId(),
				FileName = document.FileName,
				Matches = document.Matches
			};
		}

		public static InProgressDocumentDto ToDto(this InProgressDocument inProgressDocument)
		{
			if (inProgressDocument == null)
			{
				throw new ArgumentNullException(nameof(inProgressDocument));
			}

			return new InProgressDocumentDto
			{
				Document = new GetDocumentDto { Id = inProgressDocument.Id.GetClientId(), FileName = inProgressDocument.FileName },
				State = inProgressDocument.State,
				ErrorInfo = inProgressDocument.ErrorInfo,
				LastStatusUpdateTime = inProgressDocument.LastStatusUpdateTime
			};
		}

		public static GetFolderDto ToDto(this Folder folder)
		{
			if (folder == null)
			{
				throw new ArgumentNullException(nameof(folder));
			}

			return new GetFolderDto
			{
				Id = folder.Id.GetClientId(),
				ParentId = folder.ParentId.GetClientId(),
				Name = folder.Name
			};
		}

		public static string GetClientId(this DocumentIdentity documentIdentity)
		{
			if (documentIdentity == null)
			{
				throw new ArgumentNullException(nameof(documentIdentity));
			}

			return documentIdentity.Id;
		}

		public static Guid GetClientId(this FolderIdentity folderIdentity)
		{
			if (folderIdentity == null)
			{
				throw new ArgumentNullException(nameof(folderIdentity));
			}

			return folderIdentity.Id;
		}
	}
}
