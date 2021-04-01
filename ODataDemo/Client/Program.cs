using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ODataDemo.Shared;
using ReflectionIT.Blazor.Paging;

namespace ODataDemo.Client {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTransient<IODataPagingClient<Supplier>, ODataPagingClient<Supplier>>();
            builder.Services.AddTransient<IODataPagingClient<Product>, ODataPagingClient<Product>>();
            builder.Services.AddTransient<IPagingClient<Product>, PagingClient<Product, PagingResult<Product>>>();

            await builder.Build().RunAsync();
        }
    }
}
