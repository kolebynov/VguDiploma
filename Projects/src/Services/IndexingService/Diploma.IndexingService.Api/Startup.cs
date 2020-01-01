using System.Reflection;
using System.Threading.Tasks;
using Diploma.IndexingService.Api.Configuration;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Extensions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Diploma.IndexingService.EsDocumentStorage.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Diploma.IndexingService.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(IIndexingQueue).Assembly);
			services.AddSignalR();
			services.AddCoreServices();
			services.AddEsDocumentStorage();

			services.AddAuthentication(
					opt =>
					{
						opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
						opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					})
				.AddJwtBearer(opt =>
				{
					opt.RequireHttpsMetadata = false;
					opt.TokenValidationParameters = new TokenValidationParameters
					{
						ValidIssuer = JwtAuthOptions.Issuer,
						ValidateAudience = true,
						ValidAudience = JwtAuthOptions.Audience,
						ValidateLifetime = true,
						IssuerSigningKey = JwtAuthOptions.SigningKey,
						ValidateIssuerSigningKey = true
					};

					opt.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var accessToken = context.Request.Query["access_token"];

							var path = context.HttpContext.Request.Path;
							if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/signalr"))
							{
								context.Token = accessToken;
							}

							return Task.CompletedTask;
						}
					};
				});

			services.AddMvc(opt => opt.EnableEndpointRouting = false)
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddHostedService<TempContentBackgroundService>();

			services.AddScoped<ITempContentStorage, TempContentStorage>();
			services.AddScoped<IUserService, UserService>();

			services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

			services.Configure<CoreOptions>(Configuration);
			services.Configure<IndexingQueueOptions>(Configuration.GetSection("indexingQueue"));
			services.Configure<ContentStorageOptions>(Configuration.GetSection("contentStorage"));
			services.Configure<TempContentStorageOptions>(Configuration.GetSection("tempContentStorage"));
			services.Configure<DocumentStorageOptions>(Configuration.GetSection("documentStorage"));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.EnvironmentName == "Development")
			{
				app.UseCors(builder => builder.WithOrigins("http://127.0.0.1:7778").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseStaticFiles();
			app.UseMvc();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endPoints =>
			{
				endPoints.MapHub<SignalrHub>("/signalr");
				endPoints.MapFallbackToFile("index.html");
			});
		}
	}
}
