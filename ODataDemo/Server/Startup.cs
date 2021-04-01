using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Newtonsoft.Json.Serialization;
using OData.Swagger.Services;
using ODataDemo.Server.Data;
using ODataDemo.Shared;

namespace ODataDemo.Server {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllersWithViews();

            //    .AddNewtonsoftJson(setupAction => {
            //    setupAction.SerializerSettings.ContractResolver =
            //        new CamelCasePropertyNamesContractResolver();
            //});
            
            services.AddRazorPages();

            services.AddDbContext<NorthwindContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("NorthwindDbContext")));


            services.AddOData(opt =>
                opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(50)
                    .AddModel("odata", GetEdmModel())
            );

            IEdmModel GetEdmModel() {
                var builder = new ODataConventionModelBuilder();

                builder.EntitySet<Supplier>("Suppliers");
                builder.EntitySet<Product>("Products");

                var category = builder.EntitySet<Category>("Categories");
                category.EntityType.Ignore(s => s.Picture);

                return builder.GetEdmModel();
            }

            services.AddSwaggerGen((config) => {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() {
                    Title = "Swagger Odata Demo Api",
                    Description = "Swagger Odata Demo",
                    Version = "v1"
                });
            });

            services.AddOdataSwaggerSupport();
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();

                app.UseSwagger();
                app.UseSwaggerUI((config) => {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Odata Demo Api");
                });
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {

                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }


    }
}
