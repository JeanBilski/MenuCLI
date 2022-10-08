using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    /// <summary>
    /// Attribute made to describe the choice of a menu. It defines the text to display, and if there is a submenu or not. See the doc for more detail.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ChoiceAttribute : Attribute
    {
        /// <summary>
        /// Display text of the menu choice
        /// </summary>
        public string ChoiceDescription { get; set; }

        /// <summary>
        /// Type of the static submenu to enter after the method has finished.
        /// </summary>
        public Type? SubMenu{ get; set; }

        public ChoiceAttribute(string choiceDescription, Type? subMenu = default)
        {
            ChoiceDescription = choiceDescription;  
            SubMenu = subMenu;
        }
    }
}
