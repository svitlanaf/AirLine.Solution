using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;

namespace AirLine
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
    }

    public void Configure(IApplicationBuilder app)
    {
      // // using Microsoft.AspNetCore.Diagnostics;
      app.UseExceptionHandler(
        new ExceptionHandlerOptions
        {
          ExceptionHandler = async context =>
          {
            context.Response.ContentType = "text/html";
            var ex = context.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
              var err = $"<h1>Error: {ex.Error.Message}</h1>";
              await context.Response.WriteAsync(err);
            }
          }
        });
      app.UseDeveloperExceptionPage();


      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });

        app.UseStaticFiles();
        // app.Run(async (context) =>
        // {
        //   await context.Response.WriteAsync("Something went wrong!");
        // });

    }
  }
  public static class DBConfiguration
  {
    public static string ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=airline_planner;";
  }
}
