using System;
using System.ComponentModel.DataAnnotations;

namespace Diploma.IndexingService.Api.Dto
{
	public class AddFolderDto
	{
		[Required(AllowEmptyStrings = false)]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		public Guid ParentId { get; set; }
	}
}
