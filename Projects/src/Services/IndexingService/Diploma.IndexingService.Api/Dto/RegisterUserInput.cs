using System.ComponentModel.DataAnnotations;

namespace Diploma.IndexingService.Api.Dto
{
	public class RegisterUserInput
	{
		[Required(AllowEmptyStrings = false)]
		[MaxLength(50)]
		public string UserName { get; set; }

		[Required(AllowEmptyStrings = false)]
		[EmailAddress]
		[MaxLength(50)]
		public string Email { get; set; }

		[Required(AllowEmptyStrings = false)]
		[MinLength(6)]
		[MaxLength(32)]
		public string Password { get; set; }
	}
}
