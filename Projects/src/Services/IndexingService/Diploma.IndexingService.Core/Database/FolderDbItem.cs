using System;

namespace Diploma.IndexingService.Core.Database
{
	internal class FolderDbItem
	{
		public Guid Id { get; set; }

		public Guid UserIdentity { get; set; }

		public string Name { get; set; }

		public Guid? ParentId { get; set; }

		public string ParentsPath { get; set; }
	}
}
