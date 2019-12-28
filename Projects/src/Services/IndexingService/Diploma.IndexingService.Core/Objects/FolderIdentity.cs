﻿using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class FolderIdentity : IComparable<FolderIdentity>, IComparable
	{
		public Guid Id { get; }

		public string UserIdentity { get; }

		public FolderIdentity(Guid id, string userIdentity)
		{
			if (string.IsNullOrEmpty(userIdentity))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(userIdentity));
			}

			Id = id;
			UserIdentity = userIdentity;
		}

		public override string ToString() => $"{Id:N}_{UserIdentity}";

		public int CompareTo(FolderIdentity other) => ToString().CompareTo(other?.ToString());

		public int CompareTo(object obj) => CompareTo(obj as FolderIdentity);

		public static FolderIdentity FromString(string stringId)
		{
			var lastIndex = stringId.LastIndexOf('_');
			if (lastIndex < 0)
			{
				throw new FormatException("Input string doesn't have separator");
			}

			return new FolderIdentity(new Guid(stringId.Substring(0, lastIndex)), stringId.Substring(lastIndex + 1));
		}
	}
}
