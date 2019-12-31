using Diploma.IndexingService.Api.Extensions;
using Diploma.IndexingService.Core.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Diploma.IndexingService.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var webHost = CreateWebHostBuilder(args).Build();
			using (var scope = webHost.Services.CreateScope())
			{
				scope.ServiceProvider
					.MigrateContentDatabase()
					.AddDefaultUsersAndRoles();
			}

			webHost.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
