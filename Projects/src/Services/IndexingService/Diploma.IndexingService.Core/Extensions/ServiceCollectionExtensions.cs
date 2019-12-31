using System;
using System.Reflection;
using System.Threading.Channels;
using Diploma.IndexingService.Core.Configuration;
using Diploma.IndexingService.Core.Database;
using Diploma.IndexingService.Core.Interfaces;
using Diploma.IndexingService.Core.Internal;
using Diploma.IndexingService.Core.Objects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Diploma.IndexingService.Core.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCoreServices(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddHostedService<DocumentProcessorWorker>();

			services.AddDbContext<DatabaseContext, DatabaseContext>(
				(sp, opt) => opt.UseSqlServer(
					sp.GetRequiredService<IOptions<CoreOptions>>().Value.ContentStorage.MsSqlConnectionString,
					sqlOpt => sqlOpt
						.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)
						.EnableRetryOnFailure(10)));

			services.AddIdentity<User, IdentityRole<Guid>>()
				.AddEntityFrameworkStores<DatabaseContext>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(opt =>
			{
				opt.Password.RequireDigit = false;
				opt.Password.RequireLowercase = false;
				opt.Password.RequiredLength = 6;
				opt.Password.RequireNonAlphanumeric = false;
				opt.Password.RequireUppercase = false;
				opt.Password.RequiredUniqueChars = 2;

				opt.SignIn.RequireConfirmedEmail = false;
				opt.SignIn.RequireConfirmedAccount = false;
				opt.SignIn.RequireConfirmedPhoneNumber = false;

				opt.User.RequireUniqueEmail = true;
			});

			services.AddSingleton<IDocumentTextExtractor, DocumentTextExtractor>();
			services.AddSingleton(sp =>
			{
				var options = sp.GetRequiredService<IOptions<IndexingQueueOptions>>();
				return Channel.CreateBounded<DocumentInfo>(
					new BoundedChannelOptions(options.Value.QueueMaxSize > 0 ? options.Value.QueueMaxSize : 10)
					{
						FullMode = BoundedChannelFullMode.Wait,
						AllowSynchronousContinuations = false,
					});
			});

			services.AddScoped<IContentStorage, EfContentStorage>();
			services.AddScoped<IInProgressDocumentsStorage, EfInProgressDocumentsStorage>();
			services.AddScoped<IFoldersStorage, EfFoldersStorage>();

			services.AddTransient<IIndexingQueue, IndexingQueue>();


			return services;
		}
	}
}
