using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Configuration;
using Diploma.IndexingService.Api.Exceptions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Api.Internal
{
	public class TempContentStorage : BackgroundService, ITempContentStorage
	{
		private readonly string folderPath = Path.Combine(Path.GetTempPath(), nameof(TempContentStorage));
		private readonly TempContentStorageOptions options;

		public TempContentStorage(IOptions<TempContentStorageOptions> options)
		{
			this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
		}

		public async Task<string> SaveTempContent(IContent content)
		{
			await using var contentStream = content.OpenReadStream();

			var token = Enumerable.Range(0, options.MaxFileNumber)
				.Select(i => $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{i}")
				.First(t =>
				{
					try
					{
						new FileStream(GetFullPath(t), FileMode.CreateNew).Dispose();
						return true;
					}
					catch (IOException)
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

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				RemoveExpiredContent();
				await Task.Delay(options.CheckTimeout, stoppingToken);
			}
		}

		private void RemoveExpiredContent()
		{
			foreach (var filePath in Directory.EnumerateFiles(folderPath))
			{
				var parts = Path.GetFileName(filePath).Split('-');
				if (parts.Length == 2
					&& long.TryParse(parts[0], out var unixTimestamp)
					&& DateTimeOffset.FromUnixTimeMilliseconds(unixTimestamp).Add(options.ContentSavePeriod) <= DateTimeOffset.UtcNow)
				{
					File.Delete(filePath);
				}
			}
		}

		private string GetFullPath(string token) => Path.Combine(folderPath, token);
	}
}
