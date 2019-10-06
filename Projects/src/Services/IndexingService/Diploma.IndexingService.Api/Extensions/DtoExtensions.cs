using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class DtoExtensions
	{
		public static Document ToIndexDocument(this Dto.DocIndex.Document document)
		{
			if (document == null)
			{
				throw new ArgumentNullException(nameof(document));
			}

			return new Document();
		}
	}
}
