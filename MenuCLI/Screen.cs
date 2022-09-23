using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLI
{
    public class Screen
    {
        private string _title;

        private string _description;

        private List<MenuChoice> _menuChoices = new List<MenuChoice>();

        private MenuChoice _exitChoice;

        private bool _ended;

        public Screen()
        {
            _title = "title";
            _description = "description";
            _exitChoice = new MenuChoice("Exit", () => { });
        }

        public Screen(string title, string description)
        {
            _title = title;
            _description = description;
            _exitChoice = new MenuChoice("Exit", () => { });
        }

        public void AddMenuChoice(string description, Func<Task> action)
        {
            _menuChoices.Add(new MenuChoice(description, action));
        }

        public void AddMenuChoice(string description, Action action)
        {
            _menuChoices.Add(new MenuChoice(description, action));
        }

        public async Task Run()
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

        private bool IsEndCondition(string choice)
        {
            return choice == "0";
        }

        private bool IsACorrectInput(string choice, out int value)
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
