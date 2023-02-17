using AutoMapper;
using CardStorageService.Data;
using CardStorageService.Mappings;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Models.Validators;
using CardStorageService.Providers;
using CardStorageService.Services;
using CardStorageService.Services.Impl;
using CardStorageService.Services.Impl.Grpc;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Text;

namespace CardStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Grpc

            //use appsetting.{Environment}.json file to configuration kestrel server
            //builder.WebHost.ConfigureKestrel(options =>
            //{
            //    options.Listen(IPAddress.Any, 5001, listenOptions =>
            //    {
            //        listenOptions.Protocols = HttpProtocols.Http2;
            //    });
            //});

            //builder.Services.AddGrpc();

            #endregion

            #region Configure AutoMapper

            var mapperConfiguration = new MapperConfiguration(mapper => mapper.AddProfile(new MappingsProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            builder.Services.AddSingleton(mapper);

            #endregion

            #region Configure FluentValidations

            builder.Services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidation>();
            builder.Services.AddScoped<IValidator<CreateClientRequest>, CreateClientRequestValidation>();
            builder.Services.AddScoped<IValidator<CardStorageServiceProtos.CreateClientRequest>, CreateClientRequestValidationProto>();
            builder.Services.AddScoped<IValidator<CreateCardRequest>, CreateCardRequestValidation>();
            builder.Services.AddScoped<IValidator<CardStorageServiceProtos.CreateCardRequest>, CreateCardRequestValidationProto>();

            #endregion

            #region Configure Settings Files and Options

            builder.Configuration
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
                .Add(new CacheSource(builder.Environment));

            builder.Services.Configure<DatabaseOptions>(options =>
            {
                builder.Configuration.GetSection("Settings:DatabaseOptions").Bind(options);
            });

            builder.Services.Configure<MongoOptions>(options =>
            {
                builder.Configuration.GetSection("Settings:MongoOptions").Bind(options);
            });

            #endregion

            #region Logger

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.ResponseBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            }).UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = true });

            #endregion

            #region Configure EF DbContext

            builder.Services.AddDbContext<CardStorageServiceDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);
            });

            #endregion

            #region Congigure Repositories

            builder.Services.AddScoped<ICardRepositoryService, CardRepository>();
            builder.Services.AddScoped<IClientRepositoryService, ClientRepository>();
            builder.Services.AddScoped<IBookRepository, BookRepository>();

            #endregion

            #region Configure Authenticate

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["SECRET_KEY"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();

            #endregion


            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();


            #region Configure Swagger

            builder.Services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "ѕриложение дл€ регистрации карт клиентов.", Version = "v1" });

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer schema (Example: 'Bearer 123213dfdslnfsdknf')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            #endregion


            var app = builder.Build();
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseWhen(c => c.Request.ContentType != "application/grpc",
                builder =>
                {
                    builder.UseHttpLogging();
                });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGrpcService<ClientService>();
            //    endpoints.MapGrpcService<CardService>();
            //});

            app.MapControllers();

            app.Run();
        }
    }
}