using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    public static class MenuCLIModule
    {
        private static MenuSqueleton _mainScreenSqueleton;

        public static IServiceCollection AddMenuCLI(this IServiceCollection services, params Assembly[] assemblies)
        {
            var menuClasses = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .Where(x => x.GetCustomAttributes(typeof(MenuAttribute), false).FirstOrDefault() != null)
                .ToArray();

            var menusDictionary = menuClasses.ToLookup(c => c.FullName, c => 
                {
                    var menuAttribute = c.GetCustomAttribute<MenuAttribute>();
                    return (new Screen(menuAttribute.Title, menuAttribute.Description), menuAttribute.IsMainMenu);
                });

            var mainMenu = menuClasses.Where(c => c.GetCustomAttribute<MenuAttribute>().IsMainMenu).FirstOrDefault();

            if (mainMenu != null)
            {
                _mainScreenSqueleton = ScanAScreen(mainMenu, services);
            }

            return services;
        }

        public static async Task StartMenu(this IServiceProvider serviceProvider)
        {
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            var screen = GenerateScreen(_mainScreenSqueleton, provider);

            await screen.Run();
        }

        private static Screen GenerateScreen(MenuSqueleton menuSqueleton, IServiceProvider provider)
        {
            var menu = provider.GetRequiredService(menuSqueleton.Menu);
            var screen = new Screen(menuSqueleton.Title, menuSqueleton.Description);
            foreach (var method in menuSqueleton.ChoiceSqueletons)
            {
                if (method.SubMenu != null)
                {
                    var subScreen = GenerateScreen(method.SubMenu, provider);
                    screen.AddMenuChoice(method.Description, async () => { await subScreen.Run(); });
                }
                else
                {
                    screen.AddMenuChoice(method.Description, async () => 
                        {   if (method.MethodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null)
                            {
                                await (Task)method.MethodInfo.Invoke(menu, null);
                            }
                            else
                            {
                                method.MethodInfo.Invoke(menu, null);
                            }
                        });
                }
            }

            return screen;
        }

        private static MenuSqueleton ScanAScreen(Type menu, IServiceCollection services)
        {
            services.AddTransient(menu);

            var mainMenuAttributes = menu.GetCustomAttribute<MenuAttribute>();

            var methods = menu.GetMethods().Where(m => m.GetCustomAttribute(typeof(ChoiceAttribute), false) != null).ToArray();
            var choicesSqueleton = new List<ChoiceSqueleton>();
            foreach (var method in methods)
            {
                var methodAttributes = method.GetCustomAttribute<ChoiceAttribute>();
                var subMenu = methodAttributes.SubMenu != null ? ScanAScreen(methodAttributes.SubMenu, services) : null;
                choicesSqueleton.Add(new ChoiceSqueleton(methodAttributes.ChoiceDescription, method, subMenu));
            }

            return new MenuSqueleton(menu, mainMenuAttributes.Title, mainMenuAttributes.Description, choicesSqueleton);
        }

        record MenuSqueleton(Type Menu, string Title, string Description, IEnumerable<ChoiceSqueleton> ChoiceSqueletons);

        record ChoiceSqueleton(string Description, MethodInfo MethodInfo, MenuSqueleton SubMenu = null);
    }
}
