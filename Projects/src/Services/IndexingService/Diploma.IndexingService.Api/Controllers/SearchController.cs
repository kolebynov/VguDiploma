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
using FoundDocumentDto = Diploma.IndexingService.Api.Dto.FoundDocumentDto;

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
		public async Task<ApiResult<IReadOnlyCollection<FoundDocumentDto>>> Search([FromQuery] SearchQuery searchQuery) => 
			ApiResult.SuccessResultWithData(
				(IReadOnlyCollection<FoundDocumentDto>)(await documentStorage.Search(searchQuery, CancellationToken.None))
					.Select(DtoExtensions.ToDto)
					.ToArray());
	}
}
