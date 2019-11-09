using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;

namespace Diploma.Shared.Extensions
{
	public static class ContentExtensions
	{
		public static async Task<byte[]> ReadAsByteArray(this IContent content, CancellationToken cancellationToken)
		{
			if (content == null)
			{
				throw new ArgumentNullException(nameof(content));
			}

			await using var contentStream = await content.OpenReadStream(cancellationToken);
			await using var memorySteam = new MemoryStream();

			await contentStream.CopyToAsync(memorySteam, cancellationToken);

			return memorySteam.ToArray();
		}
	}
}
