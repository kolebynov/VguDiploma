using System.ComponentModel.DataAnnotations;

namespace Diploma.IndexingService.Api.Dto
{
	public class LoginInput
	{
		[Required(AllowEmptyStrings = false)]
		[MaxLength(50)]
		public string UserName { get; set; }

		[Required(AllowEmptyStrings = false)]
		[MinLength(6)]
		[MaxLength(32)]
		public string Password { get; set; }
	}
}
