namespace MenuCLILib
{
    public class MenuChoice
    {
        private string _displayString;

        private Func<Task> _asyncCallback;

        private Action _callback;

        private bool _isAsync;

        public MenuChoice(string displayString, Func<Task> asyncCallback)
        {
            _displayString = displayString;
            _asyncCallback = asyncCallback;
            _isAsync = true;
        }

        public MenuChoice(string displayString, Action action)
        {
            _displayString = displayString;
            _callback = action;
            _isAsync = false;
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
            if (_isAsync)
            {
                await _asyncCallback.Invoke();
            } 
            else
            {
                _callback();
            }
            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
    }
}