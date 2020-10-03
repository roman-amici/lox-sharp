using System;

namespace lox_sharp
{
    class Lox
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: jlox [script]");
                System.Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }

        }

        static void RunFile(string filename)
        {
            throw new NotImplementedException("We'll deal with files later. Not that we need it yet...");
        }

        static void RunPrompt()
        {
            for (; ; )
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (String.IsNullOrEmpty(line))
                {
                    break;
                }
                else
                {
                    Run(line);
                }
            }

        }

        static void Run(string line)
        {
            var scanner = new Scanner(line);
            var tokens = scanner.Scan();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void ReportError(int line, string message)
        {
            Console.WriteLine($"{line} : {message}");
        }
    }
}
