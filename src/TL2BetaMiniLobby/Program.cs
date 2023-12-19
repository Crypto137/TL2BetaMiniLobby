using System.Globalization;

namespace TL2BetaMiniLobby
{
    public class Program
    {
        public static LobbyServer LobbyServer { get; private set; }
        public static Thread LobbyServerThread { get; private set; }

        public static bool MessageDumpMode { get; private set; } = false;

        static void Main(string[] args)
        {
            Console.WriteLine("TL2BetaMiniLobby starting...");

            foreach (string arg in args)
            {
                if (arg.ToLower() == "-dump")
                {
                    MessageDumpMode = true;
                    Console.WriteLine("Message dump mode enabled");
                }
            }

            // Start lobby server
            LobbyServer = new();
            LobbyServerThread = new(LobbyServer.Run) { IsBackground = true, CurrentCulture = CultureInfo.InvariantCulture };
            LobbyServerThread.Start();

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
