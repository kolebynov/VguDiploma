using System;

namespace Diploma.IndexingService.Api.Dto
{
	public class GetFolderDto
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid ParentId { get; set; }
	}
}
