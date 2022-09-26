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
        private static MenuSqueleton? _entrypointSqueleton;

        public static IServiceCollection AddMenuCLI(this IServiceCollection services, Type entrypoint)
        {
            if (entrypoint == null)
            {
                throw new ArgumentNullException(nameof(entrypoint));
            }
            if (!entrypoint.IsClass)
            {
                throw new ArgumentException($"The entrypoint {entrypoint.Name} must be a class");
            }

            _entrypointSqueleton = PrepareTheMenuSqueleton(entrypoint, services);

            return services;
        }

        public static async Task StartMenu(this IServiceProvider serviceProvider)
        {
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            if (_entrypointSqueleton == null)
            {
                throw new ArgumentNullException("The generation of the menu squeleton went wrong");
            }

            var screen = GenerateScreen(_entrypointSqueleton, provider);
            await screen.Run();
        }

        private static MenuSqueleton PrepareTheMenuSqueleton(Type menu, IServiceCollection services)
        {
            services.AddTransient(menu);

            var menuAttributes = menu.GetCustomAttribute<MenuAttribute>();
            if (menuAttributes == null)
            {
                throw new ArgumentException($"The entrypoint must have the MenuAttribute");
            }

            var methods = menu.GetMethods().Where(m => m.GetCustomAttribute(typeof(ChoiceAttribute), false) != null).ToArray();
            var choicesSqueleton = new List<ChoiceSqueleton>();
            foreach (var method in methods)
            {
                var methodAttributes = method.GetCustomAttribute<ChoiceAttribute>();
                if (methodAttributes == null)
                {
                    throw new ArgumentException($"The method {method.Name} in the class {menu.Name} miss the ChoiceAttribute");
                }
                var subMenu = methodAttributes.SubMenu != null ? PrepareTheMenuSqueleton(methodAttributes.SubMenu, services) : null;
                choicesSqueleton.Add(new ChoiceSqueleton(methodAttributes.ChoiceDescription, method, subMenu));
            }

            return new MenuSqueleton(menu, menuAttributes.Title, menuAttributes.Description, choicesSqueleton);
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
                    screen.AddMenuChoice(
                        method.Description, 
                        async () => 
                        {
                            await ExecuteCallback(method.MethodInfo, menu);
                            await subScreen.Run(); 
                        }, 
                        false);
                }
                else
                {
                    screen.AddMenuChoice(method.Description, async () => await ExecuteCallback(method.MethodInfo, menu));
                }
            }

            return screen;
        }

        private static async Task ExecuteCallback(MethodInfo method, object? menu)
        {
            if (method.GetCustomAttribute<AsyncStateMachineAttribute>() != null)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await (Task)method.Invoke(menu, null);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            else
            {
                method.Invoke(menu, null);
            }
        }

        record MenuSqueleton(Type Menu, string Title, string? Description, IEnumerable<ChoiceSqueleton> ChoiceSqueletons);

        record ChoiceSqueleton(string Description, MethodInfo MethodInfo, MenuSqueleton? SubMenu = null);
    }
}
