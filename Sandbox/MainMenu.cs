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
        [Choice("Choice 1")]
        public void Choice1()
        {
            Console.WriteLine("First Choice Action");
            throw new NotImplementedException();
        }

        [Choice("Sub Menu", typeof(SubMenu))]
        public void Choice2()
        {
            Console.WriteLine("Second Choice Action");
        }
    }
}
