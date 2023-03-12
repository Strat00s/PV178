using Overloading_02;

var prettyPrinter = new PrettyPrinter();

prettyPrinter.Print("Test text");
prettyPrinter.Print("Test text", ConsoleColor.Cyan);
prettyPrinter.Print("Test text", ConsoleColor.Red, ConsoleColor.DarkGreen);
prettyPrinter.Print("Test text", "***");