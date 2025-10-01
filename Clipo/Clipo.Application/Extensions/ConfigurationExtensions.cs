using System.Reflection;
using System.Text;
using Amazon.S3;
using Clipo.Application.Services.EmailSender;
using Clipo.Application.Services.S3Storage;
using Clipo.Application.Services.VideoConverter;
using Clipo.Application.UseCases.ConvertVideoToFrame;
using Clipo.Application.UseCases.GetVideosByUser;
using Clipo.Application.UseCases.GetVideoStatus;
using Clipo.Domain.AggregatesModel.Base.Interface;
using Clipo.Domain.AggregatesModel.VideoAggregate.Interface;
using Clipo.Infrastructure.Data;
using Clipo.Infrastructure.Repository;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Clipo.Application.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration cfg)
        {
            IConfigurationSection dbSection = cfg.GetSection("Database");
            string host = dbSection["Host"] ?? "localhost";
            string port = dbSection["Port"] ?? "5433";
            string user = dbSection["User"] ?? "postgres";
            string password = dbSection["Password"] ?? "postgres";
            string dbName = dbSection["Name"] ?? "postgres";
            bool autoCreate = dbSection.GetValue<bool>("AutoCreate", false);

            string connectionString =
                $"Host={host};Port={port};Database={dbName};Username={user};Password={password}";

            services.AddDbContext<ApplicationContext>(options =>
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.EnableRetryOnFailure(maxRetryCount: 5);
                    if(!autoCreate)
                    {
                        npgsql.MigrationsHistoryTable("__ef_migrations");
                    }
                }));

            using(ServiceProvider serviceProvider = services.BuildServiceProvider())
            using(IServiceScope scope = serviceProvider.CreateScope())
            {
                ApplicationContext context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

                if(autoCreate)
                {
                    context.Database.EnsureCreated();
                }
                else
                {
                    context.Database.Migrate();
                }
            }

            return services;
        }
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration cfg)
        {
            string secret = cfg.GetSection("Auth")["Secret"]
                         ?? throw new InvalidOperationException("Auth:Secret não configurado!");

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true; // em dev você pode desligar
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromMinutes(5),

                        IssuerSigningKey = key,
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },

                        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => new[] { key }
                    };
                });

            services.AddAuthorization();
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

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",                 // <= minúsculo
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Informe apenas o JWT (sem o prefixo 'Bearer ')"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
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

        public static IServiceCollection AddAwsServices(this IServiceCollection services, IConfiguration cfg)
        {
            try
            {
                IConfigurationSection awsSection = cfg.GetSection("AWS");
                string? accessKeyId = awsSection["AccessKeyId"];
                string? secretAccessKey = awsSection["SecretAccessKey"];
                string? sessionToken = awsSection["SessionToken"];
                string region = awsSection["Region"] ?? "us-east-1";

                if(!string.IsNullOrEmpty(accessKeyId) && !string.IsNullOrEmpty(secretAccessKey))
                {
                    Amazon.Runtime.AWSCredentials awsCredentials;

                    if(!string.IsNullOrEmpty(sessionToken))
                    {
                        awsCredentials = new Amazon.Runtime.SessionAWSCredentials(accessKeyId, secretAccessKey, sessionToken);
                    }
                    else
                    {
                        awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKeyId, secretAccessKey);
                    }

                    AmazonS3Config s3Config = new AmazonS3Config
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
                    };

                    services.AddSingleton<IAmazonS3>(provider => new AmazonS3Client(awsCredentials, s3Config));
                }
                else
                {
                    AmazonS3Config s3Config = new AmazonS3Config
                    {
                        RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
                    };
                    services.AddSingleton<IAmazonS3>(provider => new AmazonS3Client(s3Config));
                }

                services.AddScoped<IS3StorageService, S3StorageService>();
            }
            catch(Exception ex)
            {

                Console.WriteLine($"Erro ao configurar AWS S3: {ex.Message}");

                services.AddScoped<IS3StorageService, S3StorageService>();
            }

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration cfg)
        {
            services
                .AddDatabase(cfg)
                .AddRepositories()
                .AddMapping()
                .AddRefitClients(cfg)
                .AddAwsServices(cfg)
                .AddVideoConverterUseCases()
                .AddSwaggerDocs(cfg)
                .AddAuthenticationServices(cfg)
                .AddHangfireServices(cfg);

            services.AddScoped<VideoConverterService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            return services;
        }
        public static IServiceCollection AddVideoConverterUseCases(this IServiceCollection s)
        {
            s.AddScoped<IConvertVideoToFrameInputPort, ConvertVideoToFrameInteractor>();
            s.AddScoped<IGetVideosByUserInputPort, GetVideosByUserInteractor>();
            s.AddScoped<IGetVideoStatusInputPort, GetVideoStatusInteractor>();
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
