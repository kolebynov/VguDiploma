using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Shared.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Diploma.IndexingService.Api.Internal
{
	public class FormFileContent : IContent
	{
		private readonly IFormFile formFile;

		public IContentHash Hash => throw new NotImplementedException();

		public FormFileContent(IFormFile formFile)
		{
			this.formFile = formFile ?? throw new ArgumentNullException(nameof(formFile));
		}

		public Task<Stream> OpenReadStream(CancellationToken cancellationToken) => Task.FromResult(formFile.OpenReadStream());
	}
}
