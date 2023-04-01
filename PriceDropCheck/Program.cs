using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriceDropCheck;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IEmailService, EmailService>();
        services.AddTransient<HttpRequest>();
    })
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("appsettings.json");
    })
    .Build();

Run(host.Services);

await host.RunAsync();

static void Run(IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    var runner = provider.GetRequiredService<HttpRequest>();
    runner.Create();
}