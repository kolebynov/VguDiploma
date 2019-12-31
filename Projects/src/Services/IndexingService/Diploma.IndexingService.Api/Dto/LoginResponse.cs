namespace Diploma.IndexingService.Api.Dto
{
	public class LoginResponse
	{
		public string AccessToken { get; set; }

		public GetUserDto User { get; set; }
	}
}
