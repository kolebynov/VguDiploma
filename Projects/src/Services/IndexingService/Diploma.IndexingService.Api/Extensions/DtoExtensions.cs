using System;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class DtoExtensions
	{
		public static DocumentInfo ToDocumentInfo(this Dto.Document document)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			return new DocumentInfo(document.Id, document.FileName, document.ModificationDate, null);
		}
	}
}
