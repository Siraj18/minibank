using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Minibank.Core;
using Minibank.Data;
using Minibank.Web.HostedServices;
using Minibank.Web.Middlewares;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Minibank.Web
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
			services.AddLogging();
			
			services.AddControllers().AddJsonOptions(j =>
			{
				j.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minibank.Web", Version = "v1" });

				c.AddSecurityDefinition("oauth2",
					new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.OAuth2,
						Flows = new OpenApiOAuthFlows
						{
							ClientCredentials = new OpenApiOAuthFlow
							{
								TokenUrl = new Uri("https://demo.duendesoftware.com/connect/token"),
								Scopes = new Dictionary<string, string>()
							}
						}
					});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = SecuritySchemeType.OAuth2.GetDisplayName(),
							}
						},
						new List<string>()
					}
				});
			});

			services.AddHostedService<MigrationHostedService>();

			services.AddScoped<ICurrencyRateProvider, CurrencyRateProvider>();
			services.AddData(Configuration).AddCore();

			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.Audience = "api";
					options.Authority = "https://demo.duendesoftware.com/";
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateLifetime = false,
						ValidateAudience = false,
					};
				});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseMiddleware<LoggingMiddleware>();
			app.UseMiddleware<ExceptionMiddleware>();
			if (env.IsDevelopment())
			{
				//app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minibank.Web v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseMiddleware<CustomAuthenticationMiddleware>();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
