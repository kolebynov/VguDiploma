using System.ComponentModel.DataAnnotations;

namespace Diploma.IndexingService.Api.Dto
{
	public class ChangePasswordInput
	{
		[Required(AllowEmptyStrings = false)]
		public string OldPassword { get; set; }

		[Required(AllowEmptyStrings = false)]
		[MinLength(6)]
		[MaxLength(32)]
		public string NewPassword { get; set; }
	}
}
