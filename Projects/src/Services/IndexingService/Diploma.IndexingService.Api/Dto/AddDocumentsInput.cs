using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Diploma.IndexingService.Api.Dto
{
	public class AddDocumentsInput
	{
		[Required]
		public IReadOnlyCollection<AddDocumentDto> Documents { get; set; }

		[Required]
		public Guid FolderId { get; set; }
	}
}
