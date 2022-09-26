using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    public class MenuAttribute : Attribute
    {
        public string Title { get; set; }

        public string? Description{ get; set; }

        public MenuAttribute(string title)
        {
            Title = title;
        }
    }
}
