using System;
using System.Linq;
using System.Threading;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.IndexingService.Api.Extensions
{
	public static class ServiceProviderExtensions
	{
		public static IServiceProvider AddDefaultUsersAndRoles(this IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			var userService = serviceProvider.GetRequiredService<IUserService>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

			if (!roleManager.RoleExistsAsync("admin").Result)
			{
				CheckResult(roleManager.CreateAsync(new IdentityRole<Guid>("admin")).Result);
				CheckResult(roleManager.CreateAsync(new IdentityRole<Guid>("user")).Result);
			}

			if (userManager.FindByIdAsync("bb98b6e7-da9a-4620-a56f-a5a09b15a4ce").Result == null)
			{
				userService.AddUser(new User
				{
					Id = new Guid("bb98b6e7-da9a-4620-a56f-a5a09b15a4ce"),
					UserName = "Test_user",
					Email = "test@example.com"
				}, "12345678", "admin", CancellationToken.None).Wait();
			}


			return serviceProvider;
		}

		private static void CheckResult(IdentityResult result)
		{
			if (!result.Succeeded)
			{
				var errors = string.Join(". ", result.Errors.Select(x => $"{x.Code} - {x.Description}"));
				throw new InvalidOperationException($"Identity result is not succeeded. Errors: {errors}");
			}
		}
	}
}
