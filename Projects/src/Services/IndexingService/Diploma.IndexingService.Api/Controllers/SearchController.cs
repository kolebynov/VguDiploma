using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Mvc;
using FoundDocument = Diploma.IndexingService.Api.Dto.FoundDocument;

namespace Diploma.IndexingService.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SearchController : ControllerBase
	{
		private readonly IDocumentStorage documentStorage;

		public SearchController(IDocumentStorage documentStorage)
		{
			this.documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
		}

		[HttpGet]
		public async Task<ApiResult<IReadOnlyCollection<FoundDocument>>> Search(string searchString) => 
			ApiResult.SuccessResultWithData(
				(IReadOnlyCollection<FoundDocument>)(await documentStorage.Search(new SearchQuery { SearchString = searchString }, CancellationToken.None))
					.Select(DtoExtensions.ToDto)
					.ToArray());
	}
}
