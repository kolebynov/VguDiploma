using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class FolderIdentity : IComparable<FolderIdentity>, IComparable
	{
		public Guid Id { get; }

		public Guid UserIdentity { get; }

		public FolderIdentity(Guid id, Guid userIdentity)
		{
			Id = id;
			UserIdentity = userIdentity;
		}

		public override string ToString() => $"{Id:N}_{UserIdentity:N}";

		public int CompareTo(FolderIdentity other) => ToString().CompareTo(other?.ToString());

		public int CompareTo(object obj) => CompareTo(obj as FolderIdentity);

		public static FolderIdentity FromString(string stringId)
		{
			var lastIndex = stringId.LastIndexOf('_');
			if (lastIndex < 0)
			{
				throw new FormatException("Input string doesn't have separator");
			}

			return new FolderIdentity(new Guid(stringId.Substring(0, lastIndex)), new Guid(stringId.Substring(lastIndex + 1)));
		}

		public static FolderIdentity RootFolderId(Guid userIdentity) => new FolderIdentity(Guid.Empty, userIdentity);
	}
}
