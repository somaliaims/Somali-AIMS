﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AutoMapper;
using AIMS.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AIMS.APIs.AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AIMS.APIs.Scheduler;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using AIMS.Models;
using Microsoft.OpenApi.Models;

namespace AIMS.APIs
{
    public class Startup
    {
        string managerEmail = "";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
                    });
            });
            services.AddMvc();
            string connectionString = "";
            string hostName = Environment.GetEnvironmentVariable("SQLSERVER_HOST");
            string hostPassword = Environment.GetEnvironmentVariable("SQLSERVER_SA_PASSWORD");
            managerEmail = Environment.GetEnvironmentVariable("MANAGER_EMAIL");
            if (!string.IsNullOrEmpty(hostName) && !string.IsNullOrEmpty(hostPassword))
            {
                connectionString = $"Server={hostName};Database=AIMSDb;User ID=sa;Password={hostPassword}; MultipleActiveResultSets=true";
            }
            else
            {
                connectionString = Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            }
            
            services.AddDbContext<AIMSDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString,
                    //sqlOptions => sqlOptions.EnableRetryOnFailure());
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    });
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AIMS Somalia", Version = "v1" });
            });

            //Locate the XML file being generated by ASP.NET...
            /*var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            //... and tell Swagger to use those XML comments.
            c.IncludeXmlComments(xmlPath);*/
            //});

            /*services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AIMSDbContext>()
                .AddDefaultTokenProviders();*/


            // ===== Add Jwt Authentication ========
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JwtIssuer"],
                    ValidAudience = Configuration["JwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JwtKey"]))
                };
            });

            services.AddControllers();

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<ISectorTypesService, SectorTypesService>();
            services.AddScoped<ISectorMappingsService, SectorMappingsService>();
            services.AddScoped<ISectorService, SectorService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrganizationTypeService, OrganizationTypeService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ISubLocationService, SubLocationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IIATIService, IATIService>();
            services.AddScoped<ISMTPSettingsService, SMTPSettingsService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IFinancialYearService, FinancialYearService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportNamesService, ReportNamesService>();
            services.AddScoped<IReportSubscriptionService, ReportSubscriptionService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();
            services.AddScoped<IEnvelopeTypeService, EnvelopeTypeService>();
            services.AddScoped<IEnvelopeService, EnvelopeService>();
            services.AddScoped<IMarkersService, MarkersService>();
            services.AddScoped<IFundingTypeService, FundingTypeService>();
            services.AddScoped<IManualExchangeRatesService, ManualExchangeRatesService>();
            services.AddScoped<IEmailMessageService, EmailMessageService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IExcelGeneratorService, ExcelGeneratorService>();
            services.AddScoped<IProjectMembershipService, ProjectMembershipService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IDataImportService, DataImportService>();
            services.AddScoped<IProjectDeletionService, ProjectDeletionService>();
            services.AddScoped<IExchangeRatesUsageService, ExchangeRatesUsageService>();
            services.AddScoped<IHelpService, HelpService>();
            services.AddScoped<IHomePageService, HomePageService>();
            services.AddScoped<IDataBackupService, DataBackupService>();
            services.AddScoped<IDropboxService, DropboxService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IFinancialYearSettingsService, FinancialYearSettingsService>();
            services.AddScoped<IOrganizationMergeService, OrganizationMergeService>();
            services.AddScoped<IFinancialYearTransitionService, FinancialYearTransitionService>();
            services.AddScoped<IDocumentLinkService, DocumentLinkService>();
            services.AddScoped<IContactMessageService, ContactMessageService>();
            services.AddScoped<ISponsorLogoService, SponsorLogoService>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddHttpClient();
            services.AddHttpClient<IExchangeRateHttpService, ExchangeRateHttpService>();
            services.AddSingleton<IHostedService, ScheduleTask>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Included full qualified for Ihosting environment because of ambiguity for same name with other namespace
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime lifetime, IServiceProvider serviceProvider)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/ExcelFiles");
            DirectoryInfo info = Directory.CreateDirectory(directoryPath);

            /*app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
             );*/
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIMS APIs Version 1, FG of Somalia");
            });

            //app.UseHttpsRedirection();
            //app.UseAuthentication();

            /*Enabling cache and setting expiration time*/
            //app.UseMvc();

            app.UseRouting();
            app.UseCors();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot/ExcelFiles")),
                RequestPath = new PathString("/StaticFiles")
            });

            if (!string.IsNullOrEmpty(managerEmail))
            {
                AIMSDbContext dbContext = serviceProvider.GetRequiredService<AIMSDbContext>();
                IMapper imapper = serviceProvider.GetRequiredService<IMapper>();
                IUserService userService = new UserService(dbContext, imapper);
                userService.AddAsync(new UserModel()
                {
                    Email = managerEmail,
                    Password = "",
                    UserType = UserTypes.Manager,
                }, "").GetAwaiter().GetResult();

            }
        }
    }
}
