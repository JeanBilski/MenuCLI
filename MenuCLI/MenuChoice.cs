namespace MenuCLILib
{
    public class MenuChoice
    {
        private string _displayString;

        private Action _callback;

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

        public void Run()
        {
            Console.Clear();
            Console.WriteLine(_displayString);
            Console.WriteLine();
            _callback();
            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
    }
}