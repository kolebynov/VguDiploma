using Diploma.IndexingService.Core.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Diploma.IndexingService.Api
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var webHost = CreateWebHostBuilder(args).Build();
			webHost.Services.MigrateContentDatabase();
			webHost.Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>();
	}
}
