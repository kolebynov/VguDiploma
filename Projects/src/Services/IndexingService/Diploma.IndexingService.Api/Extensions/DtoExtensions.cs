﻿using System;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Interfaces;
using FoundDocumentDto = Diploma.IndexingService.Api.Dto.FoundDocumentDto;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class DtoExtensions
	{
		public static DocumentInfo ToDocumentInfo(this AddDocumentDto document, IContent content, User currentUser)
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
