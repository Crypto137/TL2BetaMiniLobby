using System.Reflection;

namespace TL2BetaMiniLobby
{
    public class Program
    {
        public static bool MessageDumpMode { get; private set; } = false;

        public static LobbyServer LobbyServer { get; private set; }

        static void Main(string[] args)
        {
            Console.WriteLine($"TL2BetaMiniLobby {Assembly.GetExecutingAssembly().GetName().Version} starting...");

            foreach (string arg in args)
            {
                if (arg.ToLower() == "-dump")
                {
                    MessageDumpMode = true;
                    Console.WriteLine("Message dump mode enabled");
                }
            }

            // Create and start the lobby server
            LobbyServer = new();
            LobbyServer.Start();

            // Process input
            while (true)
            {
                string input = Console.ReadLine();
                switch (input.ToLower())
                {
                    case "commands":
                        Console.WriteLine("Available commands: status, stop");
                        break;

                    case "status":
                        Console.WriteLine(LobbyServer.GetStatus());
                        break;

                    case "stop":
                        LobbyServer.Shutdown();
                        Console.WriteLine("Server shut down. Press any key to exit");
                        Console.ReadKey();
                        return;

                    default:
                        Console.WriteLine($"Invalid command '{input}'. Type 'commands' for a list of available commands.");
                        break;
                }
            }
        }
    }
}
