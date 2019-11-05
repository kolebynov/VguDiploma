using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Exceptions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;

namespace Diploma.IndexingService.Api.Internal
{
	public class TempContentStorage : ITempContentStorage
	{
		private readonly string folderPath = Path.Combine(Path.GetTempPath(), nameof(TempContentStorage));

		public TempContentStorage()
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}

		public async Task<string> SaveTempContent(IContent content)
		{
			await using var contentStream = content.OpenReadStream();

			var token = Enumerable.Range(0, int.MaxValue)
				.Select(i => $"{DateTimeOffset.UtcNow:yy-MM-dd-HH-mm-ss}-{i}")
				.First(t =>
				{
					try
					{
						new FileStream(GetFullPath(t), FileMode.CreateNew).Dispose();
						return true;
					}
					catch (IOException e)
					{
						return false;
					}
				});

			await using (var file = File.OpenWrite(GetFullPath(token)))
			{
				await contentStream.CopyToAsync(file);
			}

			return token;
		}

		public Task<IContent> GetTempContent(string token)
		{
			var path = GetFullPath(token);
			if (!File.Exists(path))
			{
				throw new TempContentRemovedException();
			}

			return Task.FromResult((IContent)new FileContent(path));
		}

		private string GetFullPath(string token) => Path.Combine(folderPath, token);
	}
}
