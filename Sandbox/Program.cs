
using MenuCLILib;

var mainScreen = new Screen("Title !", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");

mainScreen.AddMenuChoice("Choice 1", () => { Console.WriteLine("Choice 1"); });
mainScreen.AddMenuChoice("Choice 2", () => { Console.WriteLine("Choice 2"); });
mainScreen.AddMenuChoice("Choice 3", () => { Console.WriteLine("Choice 3"); });


var secondScreen = new Screen("second screen", "test");

secondScreen.AddMenuChoice("Choice 1", () => { Console.WriteLine("Choice 1"); });
secondScreen.AddMenuChoice("Choice 2", () => { Console.WriteLine("Choice 2"); });

mainScreen.AddMenuChoice("Sub menu", () => { secondScreen.Run(); });

mainScreen.Run();