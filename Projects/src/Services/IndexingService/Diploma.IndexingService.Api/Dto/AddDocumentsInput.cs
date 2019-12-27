using System;
using System.Collections.Generic;

namespace Diploma.IndexingService.Api.Dto
{
	public class AddDocumentsInput
	{
		public IReadOnlyCollection<AddDocumentDto> Documents { get; set; }

		public Guid FolderId { get; set; }
	}
}
