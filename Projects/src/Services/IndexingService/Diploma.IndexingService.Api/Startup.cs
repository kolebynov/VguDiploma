using System.Reflection;
using Diploma.IndexingService.Api.Configuration;
using Diploma.IndexingService.Api.Interfaces;
using Diploma.IndexingService.Api.Internal;
using Diploma.IndexingService.Core;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
using Diploma.IndexingService.DocumentStorage.Extensions;
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
			services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(IndexingQueue).Assembly);

			services.AddMvcCore(opt => opt.EnableEndpointRouting = false)
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddHostedService<DocumentProcessorWorker>();
			services.AddHostedService(sp => (TempContentStorage)sp.GetRequiredService<ITempContentStorage>());

			services.AddSingleton<ITempContentStorage, TempContentStorage>();
			services.AddSingleton<IIndexingQueue, IndexingQueue>();
			services.AddSingleton<ITextExtractor, TextExtractor>();

			services.AddDocumentStorage("123");

			services.Configure<IndexingQueueOptions>(Configuration.GetSection("indexingQueue"));
			services.Configure<TempContentStorageOptions>(Configuration.GetSection("tempContentStorage"));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.EnvironmentName == "Development")
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
		}
	}
}
