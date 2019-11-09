using System;
using Diploma.IndexingService.Core.Objects;
using Diploma.Shared.Interfaces;

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
	}
}
