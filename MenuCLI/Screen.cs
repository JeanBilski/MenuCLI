﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuCLILib
{
    public class Screen
    {
        private string _title;
        private string _description;
        private List<MenuChoice> _menuChoices = new List<MenuChoice>();
        private bool _ended;

        public Screen(string title, string description)
        {
            _title = title;
            _description = description;
            _ended = false;
        }

        public void AddMenuChoice(string description, Action action)
        {
            _menuChoices.Add(new MenuChoice(description, action));
        }

        public void Run()
        {
            while (!_ended)
            {
                Display();
                EvaluateAnswer();
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
        }
        #endregion

        #region Control
        private void EvaluateAnswer()
        {
            var choice = Console.ReadLine();
            if (IsEndCondition(choice))
            {
                _ended = true;
                return;
            }
            else if (IsACorrectInput(choice, out var index)) 
            {
                _menuChoices[index - 1].Run();
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
