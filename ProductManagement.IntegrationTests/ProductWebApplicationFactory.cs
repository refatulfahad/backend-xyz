using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Data;

namespace ProductManagement.IntegrationTests
{
    public class ProductWebApplicationFactory<TProgram> : WebApplicationFactory<Program> where TProgram : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //remove the database context registration from the program class
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ProductContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                //add the database context to the service container
                services.AddDbContext<ProductContext>(options =>
                {
                    //instruct it to use the in-memory database instead of the real database
                    options.UseInMemoryDatabase("InMemoryProductTest");
                });
                var sp = services.BuildServiceProvider();

                //seed the data from the ProductContext class (same as main project)
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ProductContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            });
        }
    }
}
