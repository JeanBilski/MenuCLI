using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ChoiceAttribute : Attribute
    {
        public string ChoiceDescription { get; set; }

        public Type? SubMenu{ get; set; }

        public ChoiceAttribute(string choiceDescription, Type? subMenu = default)
        {
            ChoiceDescription = choiceDescription;  
            SubMenu = subMenu;
        }
    }
}
