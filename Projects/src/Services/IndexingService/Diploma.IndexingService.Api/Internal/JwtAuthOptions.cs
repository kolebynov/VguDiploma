using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Diploma.IndexingService.Api.Internal
{
	public static class JwtAuthOptions
	{
		public static string Issuer => "MyApp";

		public static string Audience => "MyClient";

		public static SymmetricSecurityKey SigningKey =>
			new SymmetricSecurityKey(Encoding.ASCII.GetBytes("1234567890ABCDEFGH_SECRET_KEY"));
	}
}
