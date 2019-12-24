using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Dto
{
	public class InProgressDocumentDto
	{
		public GetDocumentDto Document { get; set; }

		public InProcessDocumentState State { get; set; }

		public string ErrorInfo { get; set; }
	}
}
