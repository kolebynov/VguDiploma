using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Diploma.IndexingService.Core.Objects
{
	public class User : IdentityUser<Guid>
	{
	}
}
