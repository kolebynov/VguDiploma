using System;
using System.ComponentModel.DataAnnotations;

namespace Diploma.IndexingService.Api.Dto
{
	public class AddDocumentDto
	{
		[Required(AllowEmptyStrings = false)]
		public string Id { get; set; }

		[Required(AllowEmptyStrings = false)]
		[MaxLength(100)]
		public string FileName { get; set; }

		[Required]
		public DateTimeOffset ModificationDate { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string ContentToken { get; set; }
	}
}
