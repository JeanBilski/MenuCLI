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

        /// <summary>
        /// Add and configure MenuCLI in the Dependency Injection.
        /// </summary>
        /// <typeparam name="T">Type of the entrypoint of the console app. This is your main menu.</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The entrypoint must be a non abstract class.</exception>
        public static IServiceCollection AddMenuCLI<T>(this IServiceCollection services)
        {
            var entrypoint = typeof(T);

            if (!entrypoint.IsClass || entrypoint.IsAbstract)
            {
                throw new ArgumentException($"The entrypoint {entrypoint.Name} must be a non abstract class");
            }

            _entrypointSqueleton = PrepareTheMenuSqueleton(entrypoint, services);

            return services;
        }

        /// <summary>
        /// Start MenuCLI. Execute it after the Dependency Injection configuration.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException">Internal error</exception>
        public static async Task StartMenu(this IServiceProvider serviceProvider)
        {
            using IServiceScope serviceScope = serviceProvider.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            if (_entrypointSqueleton == null)
            {
                throw new ApplicationException("The generation of the menu squeleton went wrong");
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

        private static Menu GenerateScreen(MenuSqueleton menuSqueleton, IServiceProvider provider)
        {
            var menu = provider.GetRequiredService(menuSqueleton.Menu);
            var screen = new Menu(menuSqueleton.Title, menuSqueleton.Description);
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
                else if (method.MethodInfo.GetParameters().Length == 1)
                {
                    var parameter = method.MethodInfo.GetParameters()[0];
                    if (parameter.ParameterType == typeof(Menu) && parameter.GetCustomAttribute<MenuAttribute>() != null)
                    {
                        var menuAttribute = parameter.GetCustomAttribute<MenuAttribute>();
                        Menu dynamicMenu = new Menu(menuAttribute?.Title ?? "Title", menuAttribute?.Description);

                        screen.AddMenuChoice(method.Description, async () => 
                        { 
                            await ExecuteCallback(method.MethodInfo, menu, dynamicMenu);
                            await dynamicMenu.Run();
                            dynamicMenu.ClearMenuChoices();
                        });
                    }
                    else
                    {
                        throw new ArgumentException($"The method {method.MethodInfo.Name} can have at most one argument of type Menu and with a Menu attribute.");
                    }
                }
                else
                {
                    screen.AddMenuChoice(method.Description, async () => await ExecuteCallback(method.MethodInfo, menu));
                }
            }

            return screen;
        }

        private static async Task ExecuteCallback(MethodInfo method, object? menu, Menu? dynamicMenu = null)
        {
            var parameter = dynamicMenu != null ? new[] { dynamicMenu } : null;

            if (method.GetCustomAttribute<AsyncStateMachineAttribute>() != null)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await (Task)method.Invoke(menu, parameter);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            else
            {
                method.Invoke(menu, parameter);
            }
        }

        record MenuSqueleton(Type Menu, string Title, string? Description, IEnumerable<ChoiceSqueleton> ChoiceSqueletons);

        record ChoiceSqueleton(string Description, MethodInfo MethodInfo, MenuSqueleton? SubMenu = null);
    }
}
