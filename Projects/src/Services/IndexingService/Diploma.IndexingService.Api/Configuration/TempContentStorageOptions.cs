using System;

namespace Diploma.IndexingService.Api.Configuration
{
	public class TempContentStorageOptions
	{
		public TimeSpan ContentSavePeriod { get; set; } = TimeSpan.FromDays(1);

		public TimeSpan CheckTimeout { get; set; } = TimeSpan.FromMinutes(1);

		public int MaxFileNumber { get; set; } = 100;
	}
}
