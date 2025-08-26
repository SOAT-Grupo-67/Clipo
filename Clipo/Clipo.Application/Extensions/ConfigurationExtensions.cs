using System.Reflection;
using Clipo.Application.Services.VideoConverter;
using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Domain.AggregatesModel.Base.Interface;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Clipo.Infrastructure.Data;
using Clipo.Infrastructure.Repository;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Clipo.Application.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration cfg)
        {
            IConfigurationSection dbSection = cfg.GetSection("Database");
            string host = dbSection["Host"] ?? "localhost";
            string port = dbSection["Port"] ?? "5432";
            string user = dbSection["User"] ?? "postgres";
            string password = dbSection["Password"] ?? "postgres";
            string dbName = dbSection["Name"] ?? "postgres";

            string connectionString =
                $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";

            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.EnableRetryOnFailure(maxRetryCount: 5);
                    npgsql.MigrationsHistoryTable("__ef_migrations");
                }));

            using(ServiceProvider serviceProvider = services.BuildServiceProvider())
            using(IServiceScope scope = serviceProvider.CreateScope())
            {
                ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                context.Database.Migrate();
            }

            return services;
        }
        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration cfg)
        {
            string conn = cfg.GetConnectionString("HangfireConnection")
                ?? throw new InvalidOperationException("Connection string 'HangfireConnection' not found.");

            services.AddHangfire(config =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(opt =>
                    {
                        opt.UseNpgsqlConnection(cfg.GetConnectionString("HangfireConnection")!);
                    });
            });

            services.AddHangfireServer();

            return services;
        }
        public static IServiceCollection AddSwaggerDocs(this IServiceCollection services, IConfiguration cfg)
        {
            IConfigurationSection section = cfg.GetSection("Swagger");
            string title = section.GetValue("Title", "Clipo API");
            string version = section.GetValue("Version", "v1");
            string prefix = section.GetValue("RoutePrefix", "swagger");

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(version, new OpenApiInfo
                {
                    Title = title,
                    Version = version,
                    Description = "Documentação da API Clipo gerada automaticamente"
                });

                string basePath = AppContext.BaseDirectory;
                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                    .Distinct();

                foreach(Assembly? assembly in assemblies)
                {
                    string xmlFile = $"{assembly.GetName().Name}.xml";
                    string xmlPath = Path.Combine(basePath, xmlFile);
                    if(File.Exists(xmlPath))
                        options.IncludeXmlComments(xmlPath);
                }

            });

            services.Configure<SwaggerOptions>(opt =>
            {
                opt.RoutePrefix = prefix;
                opt.Version = version;
                opt.Title = title;
            });

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IVideoStatusRepository, VideoRepository>();

            return services;
        }

        public static IServiceCollection AddMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            return services;
        }

        public static IServiceCollection AddRefitClients(this IServiceCollection services, IConfiguration cfg)
        {

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration cfg)
        {
            services
                .AddDatabase(cfg)
                .AddRepositories()
                .AddMapping()
                .AddRefitClients(cfg)
                .AddVideoConverterUseCases()
                .AddSwaggerDocs(cfg)
                .AddHangfireServices(cfg);

            services.AddScoped<VideoConverterService>();

            return services;
        }
        public static IServiceCollection AddVideoConverterUseCases(this IServiceCollection s)
        {
            s.AddScoped<IConvertVideoToFrameInputPort, ConvertVideoToFrameInteractor>();
            return s;
        }
        internal sealed record SwaggerOptions
        {
            public string Title { get; set; } = "API";
            public string Version { get; set; } = "v1";
            public string RoutePrefix { get; set; } = "swagger";
        }
    }
}
