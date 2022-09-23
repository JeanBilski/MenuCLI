
using MenuCLILib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Xml.Linq;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<Screen>();
    })
    .Build();

await Scoping(host.Services);

await host.RunAsync();

static async Task Scoping(IServiceProvider serviceProvider)
{
    using IServiceScope serviceScope = serviceProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;



    var mainScreen = provider.GetRequiredService<Screen>();
    mainScreen.AddMenuChoice("Choice 1", async () => { await Test(); Console.WriteLine("after test"); });
    mainScreen.AddMenuChoice("Choice 2", () => { Console.WriteLine(); });
    mainScreen.AddMenuChoice("Choice 3", () => { Console.WriteLine(); });

    await mainScreen.Run();
}

async static Task Test()
{
    await Task.Delay(1000);
    Console.WriteLine("this is a test");
}