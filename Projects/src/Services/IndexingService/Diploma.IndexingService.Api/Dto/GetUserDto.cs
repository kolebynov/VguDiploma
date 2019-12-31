using System;

namespace Diploma.IndexingService.Api.Dto
{
	public class GetUserDto
	{
		public Guid Id { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }
	}
}
