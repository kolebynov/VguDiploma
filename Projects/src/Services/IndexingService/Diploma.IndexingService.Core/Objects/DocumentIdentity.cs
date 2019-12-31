using System;

namespace Diploma.IndexingService.Core.Objects
{
	public class DocumentIdentity
	{
		public string Id { get; }

		public Guid UserIdentity { get; }

		public DocumentIdentity(string id, Guid userIdentity)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Value cannot be null or empty.", nameof(id));
			}

			Id = id;
			UserIdentity = userIdentity;
		}

		public void Deconstruct(out string id, out Guid userIdentity)
		{
			id = Id;
			userIdentity = UserIdentity;
		}

		public override string ToString() => $"{Id}_{UserIdentity:N}";

		public static DocumentIdentity FromString(string stringId)
		{
			var lastIndex = stringId.LastIndexOf('_');
			if (lastIndex < 0)
			{
				throw new FormatException("Input string doesn't have separator");
			}

			return new DocumentIdentity(stringId.Substring(0, lastIndex), new Guid(stringId.Substring(lastIndex + 1)));
		}
	}
}
