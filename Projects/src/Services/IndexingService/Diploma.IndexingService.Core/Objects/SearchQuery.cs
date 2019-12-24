namespace Diploma.IndexingService.Core.Objects
{
	public class SearchQuery
	{
		public string SearchString { get; set; }

		public int Limit { get; set; } = 10;

		public int Skip { get; set; }
	}
}
