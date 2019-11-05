using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;

namespace Diploma.IndexingService.Api.Interfaces
{
	public interface ITempContentStorage
	{
		Task<string> SaveTempContent(IContent content);

		Task<IContent> GetTempContent(string token);
	}
}
