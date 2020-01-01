using System;

namespace Diploma.IndexingService.Api.Exceptions
{
	public class ApiServiceException : Exception
	{
		public ApiServiceException()
		{
		}

		public ApiServiceException(string message)
			: base(message)
		{
		}

		public ApiServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
