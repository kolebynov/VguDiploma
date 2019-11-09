using System;

namespace Diploma.IndexingService.Api.Exceptions
{
	public class TempContentRemovedException : Exception
	{
		public TempContentRemovedException()
		{
		}

		public TempContentRemovedException(string message)
			: base(message)
		{
		}

		public TempContentRemovedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
