namespace Diploma.IndexingService.Core.Configuration
{
	public class CoreOptions
	{
		public IndexingQueueOptions IndexingQueue { get; set; } = new IndexingQueueOptions();

		public ContentStorageOptions ContentStorage { get; set; } = new ContentStorageOptions();
	}
}
