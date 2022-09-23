
using MenuCLI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddMenuCLI(Assembly.GetExecutingAssembly());
    })
    .Build();

await host.Services.StartMenu();

await host.RunAsync();
