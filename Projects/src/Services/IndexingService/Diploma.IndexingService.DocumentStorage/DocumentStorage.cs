using System;
using System.IO;
using System.Threading.Tasks;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Objects;

namespace Diploma.IndexingService.DocumentStorage
{
	internal class DocumentStorage : IDocumentStorage
	{
		private readonly DbContext dbContext;

		public DocumentStorage(DbContext dbContext)
		{
			this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public async Task SaveDocumentToDb(DocumentInfo document, string text)
		{
			var dbDocument = await dbContext.Documents.FindAsync(document.Id.Id, document.Id.UserIdentity);
			if (dbDocument == null)
			{
				dbDocument = new Models.DocumentInfo { Id = document.Id.Id, UserIdentity = document.Id.UserIdentity };
				dbContext.Documents.Add(dbDocument);
			}
			else
			{
				dbContext.Documents.Update(dbDocument);
			}

			dbDocument.ModificationDate = document.ModificationDate;
			dbDocument.FileName = document.FileName;

			await using var stream = document.Content.OpenReadStream();
			await using var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			dbDocument.Content = memoryStream.ToArray();

			await dbContext.SaveChangesAsync();
		}
	}
}
