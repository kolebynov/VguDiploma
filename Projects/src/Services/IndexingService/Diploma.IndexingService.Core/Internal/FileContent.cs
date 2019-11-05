using System;
using System.IO;
using Diploma.IndexingService.Core.Interfaces;

namespace Diploma.IndexingService.Core.Internal
{
	public class FileContent : IContent
	{
		private readonly string path;

		public IContentHash Hash => throw new NotImplementedException();

		public FileContent(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(path));
			}

			this.path = path;
		}

		public Stream OpenReadStream()
		{
			if (!File.Exists(path))
			{
				throw new InvalidOperationException("Cannot open read stream because file doesn't exits");
			}

			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
		}
	}
}
