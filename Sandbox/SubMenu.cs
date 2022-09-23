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
        [Choice("Sub Choice 1")]
        public void Choice1()
        {
            Console.WriteLine("Sub Menu First Choice");
        }

        [Choice("Async Choice 2")]
        public async Task Choice2()
        {
            await Task.Delay(1000);
            Console.WriteLine("Sub Menu First Choice");
        }
    }
}
