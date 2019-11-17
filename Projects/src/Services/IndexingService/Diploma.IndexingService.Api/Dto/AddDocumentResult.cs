using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Dto
{
	public class AddDocumentResult
	{
		public string Id { get;set; }

		public InProcessDocumentState State { get;set; }
	}
}
