using System;
using System.Collections.Generic;

namespace Diploma.IndexingService.Core.Objects
{
	public class Folder
	{
		public FolderIdentity Id { get; }

		public string Name { get; }

		public FolderIdentity ParentId { get; }

		public IReadOnlyList<FolderIdentity> ParentsPath { get; }

		public Folder(FolderIdentity id, string name, FolderIdentity parentId, IReadOnlyList<FolderIdentity> parentsPath)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(name));
			}

			Id = id ?? throw new ArgumentNullException(nameof(id));
			Name = name;
			ParentsPath = parentsPath ?? new List<FolderIdentity>();
			ParentId = parentId;
		}
	}
}
