using System;

namespace Diploma.IndexingService.Core.Exceptions
{
	public class DocumentStorageException : Exception
	{
		public DocumentStorageException(string message)
			: base(message)
		{
		}

		public DocumentStorageException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
