using System;

namespace Diploma.IndexingService.Api.Dto
{
	public class AddFolderDto
	{
		public string Name { get; set; }

		public Guid ParentId { get; set; }
	}
}
