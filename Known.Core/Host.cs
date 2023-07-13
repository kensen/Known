﻿using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Core;

public sealed class Host
{
    private Host() { }

    internal static Assembly Assembly { get; private set; }

    public static void RunWebApiAsync<T>(params string[] urls)
    {
        Assembly = typeof(T).Assembly;
        Task.Run(() => CreateWebHostBuilder(urls).Build().Run());
    }

    private static IWebHostBuilder CreateWebHostBuilder(params string[] urls)
    {
        return WebHost.CreateDefaultBuilder()
                      .UseUrls(urls)
                      .UseStartup<Startup>();
    }
}

class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var builder = services.AddControllers();
        builder.ConfigureApplicationPartManager(apm =>
        {
            apm.ApplicationParts.Add(new AssemblyPart(typeof(BaseController).Assembly));
            apm.ApplicationParts.Add(new AssemblyPart(Host.Assembly));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        var upload = KCConfig.GetUploadPath();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(upload),
            RequestPath = "/UploadFiles"
        });
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}