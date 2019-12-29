using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
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
		private readonly IUserService userService;

		public SearchController(IDocumentStorage documentStorage, IUserService userService)
		{
			this.documentStorage = documentStorage ?? throw new ArgumentNullException(nameof(documentStorage));
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[HttpGet]
		public async Task<PaginationApiResult<IReadOnlyCollection<FoundDocumentDto>>> Search([FromQuery] SearchQuery searchQuery)
		{
			var searchResult = await documentStorage.Search(searchQuery, await userService.GetCurrentUser(), CancellationToken.None);
			var dtos = (IReadOnlyCollection<FoundDocumentDto>)searchResult.FoundDocuments
				.Select(DtoExtensions.ToDto)
				.ToArray();

			return ApiResult.SuccessPaginationResult(
				dtos, new PaginationData {TotalCount = searchResult.TotalDocuments});
		}
	}
}
