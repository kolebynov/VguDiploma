using System;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.Api.Internal
{
	public class DocumentStorage : IDocumentStorage
	{
		public Task SaveDocumentToDb(DocumentInfo document, string text)
		{
			throw new NotImplementedException();
		}
	}
}
