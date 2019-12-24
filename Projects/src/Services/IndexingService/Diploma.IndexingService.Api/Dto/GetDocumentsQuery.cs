namespace Diploma.IndexingService.Api.Dto
{
	public class GetDocumentsQuery
	{
		public int Limit { get; set; } = 10;

		public int Skip { get; set; }
	}
}
