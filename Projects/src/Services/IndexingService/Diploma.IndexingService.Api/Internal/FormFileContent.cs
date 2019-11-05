using System;
using System.IO;
using Diploma.IndexingService.Core.Interfaces;
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

		public Stream OpenReadStream() => formFile.OpenReadStream();
	}
}
