using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    /// <summary>
    /// Describe the menu title and description. The description is optional. Can be used on a class but also on a parameter, if you want to dynamicaly assign choices to a submenu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
    public class MenuAttribute : Attribute
    {
        /// <summary>
        /// Configure the title of the menu
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the menu. Can be skipped.
        /// </summary>
        public string? Description{ get; set; }

        public MenuAttribute(string title)
        {
            Title = title;
        }
    }
}
