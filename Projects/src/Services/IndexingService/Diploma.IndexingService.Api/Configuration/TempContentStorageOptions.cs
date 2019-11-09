using System;
using Diploma.IndexingService.Api.Internal;

namespace Diploma.IndexingService.Api.Configuration
{
	public class TempContentStorageOptions
	{
		public TimeSpan ContentSavePeriod { get; set; } = TimeSpan.FromDays(1);

		public TimeSpan CheckTimeout { get; set; } = TimeSpan.FromMinutes(1);

		public string ContentCategory { get; set; } = nameof(TempContentStorage);
	}
}
