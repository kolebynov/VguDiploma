using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Interfaces
{
	public interface IUserService
	{
		Task<User> GetCurrentUser();
	}
}
