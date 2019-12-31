using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Events;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Core.Objects;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Diploma.IndexingService.Api.Internal
{
	internal class UserService : IUserService
	{
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IMediator mediator;

		public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, SignInManager<User> signInManager,
			IMediator mediator)
		{
			this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
			this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}

		public Task<User> GetCurrentUser() => 
			userManager.GetUserAsync(httpContextAccessor.HttpContext.User);

		public async Task<User> GetUser(string userName, string password, CancellationToken cancellationToken)
		{
			var user = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
			var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, false);
			if (!signInResult.Succeeded)
			{
				throw new InvalidOperationException("User name or password wrong");
			}

			return user;
		}

		public async Task<IReadOnlyCollection<Claim>> GetClaims(User user, CancellationToken cancellationToken)
		{
			var claims = await userManager.GetClaimsAsync(user);
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			return claims.ToArray();
		}

		public async Task AddUser(User newUser, string password, string role, CancellationToken cancellationToken)
		{
			var createResult = await userManager.CreateAsync(newUser, password);
			if (!createResult.Succeeded)
			{
				throw new InvalidOperationException("Failed to create new user");
			}

			var addToRoleResult = await userManager.AddToRoleAsync(newUser, role);
			if (!addToRoleResult.Succeeded)
			{
				throw new InvalidOperationException($"Failed to add new user to the role {role}");
			}

			await mediator.Publish(new NewUserAdded(newUser), cancellationToken);
		}
	}
}
