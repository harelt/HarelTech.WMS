using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            services.AddControllers();

            var sqlConnectionString = Configuration.GetConnectionString("Priority");
            var dbsSection = Configuration.GetSection("ConnectionStrings:DataBases");
            var dbs = dbsSection.Get<List<string>>();
            var dbList = new List<HarelTech.WMS.Repository.Models.DatabaseConnection>();
            foreach (var db in dbs)
            {
                var strCon = sqlConnectionString.Replace("catalogname", db);
                dbList.Add(new Repository.Models.DatabaseConnection { DbName = db, Connection = strCon });
            }

            services.AddScoped<Repository.IPriorityDbs>(db => new Repository.PriorityDbs(dbList));
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
        }
    }
}
