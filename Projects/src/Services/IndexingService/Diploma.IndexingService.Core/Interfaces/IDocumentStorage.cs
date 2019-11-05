using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Core.Interfaces
{
	public interface IDocumentStorage
	{
		Task SaveDocumentToDb(DocumentInfo document, string text);
	}
}
