
using MenuCLI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sandbox;
using System.Reflection;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddMenuCLI(typeof(MainMenu));

        services.AddTransient<DependancyInjectionExemple>();
    })
    .Build();

await host.Services.StartMenu();
