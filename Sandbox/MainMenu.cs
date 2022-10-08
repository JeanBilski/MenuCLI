using MenuCLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    [Menu("Main Menu", Description = "This is a wonderfull description")]
    internal class MainMenu
    {
        private readonly DependancyInjectionExemple _injectionExemple;

        public MainMenu(DependancyInjectionExemple injectionExemple)
        {
            _injectionExemple = injectionExemple;
        }

        [Choice("Choice 1")]
        public void Choice1()
        {
            Console.WriteLine($"First Choice Action with a sync call {_injectionExemple.ExempleOfASyncCall()}");
        }

        [Choice("Sub Menu", SubMenu = typeof(SubMenu))]
        public async Task Choice2()
        {
            Console.WriteLine("Second Choice Action");
            Console.WriteLine("wait for the callback to finish...");
            await Task.Delay(3000);
        }


        [Choice("Dynamic Menu")]
        public void Choice3([Menu("Dynamic Menu", Description = "This is a generated menu from a callback")]  Menu menu)
        {
            var random = new Random();
            var choiceNumber = random.Next(9);
            for (int i = 0; i < choiceNumber + 1; i++)
            {
                menu.AddMenuChoice($"Choice Id {Guid.NewGuid()}", () => Console.WriteLine("What a choice !"));
            }
        }
    }
}
