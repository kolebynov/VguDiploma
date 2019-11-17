using System;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Interfaces;
using FoundDocument = Diploma.IndexingService.Api.Dto.FoundDocument;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class DtoExtensions
	{
		public static DocumentInfo ToDocumentInfo(this AddDocument document, IContent content, User currentUser)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			if (currentUser == null)
			{
				throw new ArgumentNullException(nameof(currentUser));
			}

			return new DocumentInfo(new DocumentIdentity(document.Id, currentUser.Id), document.FileName, document.ModificationDate, content);
		}

		public static FoundDocument ToDto(this Core.Objects.FoundDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			return new FoundDocument
			{
				Id = document.DocumentId.GetClientId(),
				FileName = document.FileName,
				Matches = document.Matches
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
	}
}
