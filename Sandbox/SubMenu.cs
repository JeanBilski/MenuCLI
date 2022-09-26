using MenuCLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    [Menu("Sub Menu")]
    internal class SubMenu
    {
        private readonly DependancyInjectionExemple _injectionExemple;

        public SubMenu(DependancyInjectionExemple injectionExemple)
        {
            _injectionExemple = injectionExemple;
        }

        [Choice("Sub Choice 1")]
        public void Choice1()
        {
            Console.WriteLine("Sub Menu First Choice");
        }

        [Choice("Async Choice 2")]
        public async Task Choice2()
        {
            Console.WriteLine($"Sub Menu Choice with an async call {await _injectionExemple.ExempleOfAnAsyncCall()}");
        }
    }
}
