using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Interfaces
{
	public interface IUserService
	{
		Task<User> GetCurrentUser();

		Task<User> GetUser(string userName, string password, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<Claim>> GetClaims(User user, CancellationToken cancellationToken);

		Task AddUser(User newUser, string password, string role, CancellationToken cancellationToken);

		Task ChangePasswordForCurrentUser(string oldPassword, string newPassword, CancellationToken cancellationToken);
	}
}
