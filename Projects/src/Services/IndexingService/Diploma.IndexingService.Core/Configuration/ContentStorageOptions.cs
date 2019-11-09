namespace Diploma.IndexingService.Core.Configuration
{
	public class ContentStorageOptions
	{
		public string MsSqlConnectionString { get;set; }

		public int PageSize { get; set; } = 100;
	}
}
