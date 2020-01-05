using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Diploma.Api.Shared.Dto;
using Diploma.IndexingService.Api.Dto;
using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Diploma.IndexingService.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService userService;

		public UsersController(IUserService userService)
		{
			this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		[Authorize]
		[HttpGet("currentUser")]
		public async Task<ApiResult<GetUserDto>> GetCurrentUser()
		{
			var currentUser = await userService.GetCurrentUser();
			return ApiResult.SuccessResultWithData(currentUser.ToDto());
		}

		[Authorize]
		[HttpPost("currentUser/changePassword")]
		public async Task<ApiResult> ChangePassword(ChangePasswordInput input)
		{
			await userService.ChangePasswordForCurrentUser(input.OldPassword, input.NewPassword,
				CancellationToken.None);
			return ApiResult.SuccessResult;
		}

		[HttpPost("login")]
		public async Task<ApiResult<LoginResponse>> Login([FromBody] LoginInput input)
		{
			var user = await userService.GetUser(input.UserName, input.Password, CancellationToken.None);
			var jwt = new JwtSecurityToken(
				JwtAuthOptions.Issuer,
				JwtAuthOptions.Audience,
				await userService.GetClaims(user, CancellationToken.None),
				DateTime.UtcNow,
				DateTime.UtcNow.AddDays(1),
				new SigningCredentials(JwtAuthOptions.SigningKey, SecurityAlgorithms.HmacSha256));

			return ApiResult.SuccessResultWithData(new LoginResponse
			{
				User = user.ToDto(),
				AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt)
			});
		}

		[HttpPost("register")]
		public async Task<ApiResult> RegisterUser([FromBody] RegisterUserInput input)
		{
			var newUser = new User
			{
				UserName = input.UserName,
				Email = input.Email
			};

			await userService.AddUser(newUser, input.Password, "user", CancellationToken.None);
			return ApiResult.SuccessResult;
		}
	}
}
