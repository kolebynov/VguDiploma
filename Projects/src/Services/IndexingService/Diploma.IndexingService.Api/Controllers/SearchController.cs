﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Mvc;

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
		public Task<IReadOnlyCollection<FoundDocument>> Search(string searchString)
		{
			return documentStorage.Search(new SearchQuery { SearchString = searchString }, CancellationToken.None);
		}
	}
}
