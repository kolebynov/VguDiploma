using System.Reflection;
using Diploma.IndexingService.Api.Configuration;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Extensions;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.EsDocumentStorage.Configuration;
using Diploma.IndexingService.EsDocumentStorage.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

			services.AddMvcCore(opt => opt.EnableEndpointRouting = false)
				.AddCors()
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddHostedService<TempContentBackgroundService>();

			services.AddScoped<ITempContentStorage, TempContentStorage>();

			services.AddCoreServices();
			services.AddEsDocumentStorage();

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
				app.UseCors(builder => builder.AllowAnyOrigin());
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
