using System.Reflection;

namespace MenuCLI
{
    public class MenuChoice
    {
        private string _displayString;

        private Func<Task>? _asyncCallback;

        private Action? _callback;

        public MenuChoice(string displayString, Func<Task> asyncCallback)
        {
            _displayString = displayString;
            _asyncCallback = asyncCallback;
        }

        public MenuChoice(string displayString, Action action)
        {
            _displayString = displayString;
            _callback = action;
        }

        public void Display(int index)
        {
            Console.WriteLine($"    {index + 1}. {_displayString}");
            Console.WriteLine();
        }

        public async Task Run()
        {
            Console.Clear();
            Console.WriteLine(_displayString);
            Console.WriteLine();

            if (_asyncCallback != null)
            {
                await _asyncCallback.Invoke();
            } 
            else if (_callback != null)
            {
                _callback();
            }
            else
            {
                throw new ArgumentNullException($"The choice need to have a callback to be executed");
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
    }
}