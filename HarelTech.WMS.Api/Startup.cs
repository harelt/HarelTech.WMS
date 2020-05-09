using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarelTech.WMS.Api.Models;
using HarelTech.WMS.RestClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HarelTech.WMS.Api
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
            // configure jwt authentication
            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:JwtSecret").Value);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IUserService, UserService>();

            services.AddControllers();
            //priority Db's connections
            var sqlConnectionString = Configuration.GetConnectionString("Priority");
            var dbsSection = Configuration.GetSection("ConnectionStrings:DataBases");
            var dbs = dbsSection.Get<List<string>>();
            var dbList = new List<HarelTech.WMS.Repository.Models.DatabaseConnection>();
            foreach (var db in dbs)
            {
                var strCon = sqlConnectionString.Replace("catalogname", db);
                dbList.Add(new Repository.Models.DatabaseConnection { DbName = db, Connection = strCon });
            }
            services.AddScoped<Repository.Interfaces.IPriorityDbs>(db => new Repository.PriorityDbs(dbList));
            //priority system db connection
            sqlConnectionString = sqlConnectionString.Replace("catalogname", "system");
            services.AddScoped<Repository.Interfaces.IPrioritySystem>(dbs => new Repository.PrioritySystem(sqlConnectionString));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HarelTechWMS API", Version = "v1" });
                c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{Configuration["AppSettings:WebAppUrl"]}" } };
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{Configuration["AppSettings:WebAppUrl"]}/swagger/v1/swagger.json", "HarelTechWMS API V1");
            });
        }
    }
}
