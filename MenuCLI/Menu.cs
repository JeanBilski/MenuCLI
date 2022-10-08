using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    public class Menu
    {
        private string _title;

        private string? _description;

        private List<MenuChoice> _menuChoices = new List<MenuChoice>();

        private MenuChoice _exitChoice;

        private bool _ended;

        internal Menu()
        {
            _title = "title";
            _description = "description";
            _exitChoice = new MenuChoice("Exit", () => { }, false);
        }

        internal Menu(string title, string? description)
        {
            _title = title;
            _description = description;
            _exitChoice = new MenuChoice("Exit", () => { }, false);
        }

        /// <summary>
        /// Register the choice callback to be executed.
        /// </summary>
        /// <param name="description">Display text of the menu choice.</param>
        /// <param name="action">async lambda callback.</param>
        /// <param name="waitForUserInput">Determine if the callback wait for a user input to exit its screen or if it returns directly on the previous menu.</param>
        public void AddMenuChoice(string description, Func<Task> action, bool waitForUserInput = true)
        {
            _menuChoices.Add(new MenuChoice(description, action, waitForUserInput));
        }

        /// <summary>
        /// Register the choice callback to be executed.
        /// </summary>
        /// <param name="description">Display text of the menu choice</param>
        /// <param name="action">sync lambda callback</param>
        /// <param name="waitForUserInput">Determine if the callback wait for a user input to exit its screen or if it returns directly on the previous menu.</param>
        public void AddMenuChoice(string description, Action action, bool waitForUserInput = true)
        {
            _menuChoices.Add(new MenuChoice(description, action, waitForUserInput));
        }

        /// <summary>
        /// Clear all the registered choices of a menu.
        /// </summary>
        public void ClearMenuChoices()
        {
            _menuChoices.Clear();
        }

        internal async Task Run()
        {
            while (!_ended)
            {
                Display();
                await EvaluateAnswer();
            }
            _ended = false;
        }

        #region Display
        private void Display()
        {
            Console.Clear();
            DisplayTitle();
            DisplayDescription();
            DisplayChoices();
        }

        private void DisplayTitle()
        {
            Console.WriteLine(_title);
            for (int i = 0; i < _title.Count(); i++)
            {
                Console.Write('-');
            }
            Console.WriteLine();
        }

        private void DisplayDescription()
        {
            Console.WriteLine(_description);
            Console.WriteLine();
        }

        private void DisplayChoices()
        {
            for (int i = 0; i < _menuChoices.Count; i++)
            {
                _menuChoices[i].Display(i);
            }
            _exitChoice.Display(-1);
        }
        #endregion

        #region Control
        private async Task EvaluateAnswer()
        {
            var choice = Console.ReadLine();
            if (IsEndCondition(choice))
            {
                _ended = true;

                return;
            }
            else if (IsACorrectInput(choice, out var index)) 
            {
                await _menuChoices[index - 1].Run();
            }
        }

        private bool IsEndCondition(string? choice)
        {
            return choice == "0";
        }

        private bool IsACorrectInput(string? choice, out int value)
        {
            if (Int32.TryParse(choice, out value))
            {
                if (value <= _menuChoices.Count && value > 0)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
