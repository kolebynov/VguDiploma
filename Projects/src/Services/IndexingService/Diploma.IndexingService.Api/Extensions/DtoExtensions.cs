using System;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Interfaces;
using FoundDocument = Diploma.IndexingService.Api.Dto.FoundDocument;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class DtoExtensions
	{
		public static DocumentInfo ToDocumentInfo(this Dto.Document document, IContent content)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			return new DocumentInfo(new DocumentIdentity(document.Id, "123"), document.FileName, document.ModificationDate, content);
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
