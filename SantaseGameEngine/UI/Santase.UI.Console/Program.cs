namespace Santase.UI.Console
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using AI.DummyPlayer;
    using Santase.AI.SmartPlayer;
    using Santase.Logic;
    using Santase.Logic.GameMechanics;
    using Santase.Logic.Players;
    using ZatvorenoAI;

    public static class Program
    {
        public static void Main()
        {
            var sw = new Stopwatch();

            sw.Start();
            for (int i = 0, length = 10000; i < length; i++)
            {
                var game = CreateGameVersusBot();
                game.Start(PlayerPosition.FirstPlayer);
                if (i < 10)
                {
                    File.WriteAllText("../../Reports/report" + i + ".txt", ZatvorenoAI.report.ToString());
                    ZatvorenoAI.report.Empty();
                }

                if (i % (length / 20) == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine((i) + " games done");
                }
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Console.WriteLine(ZatvorenoAI.GetReports());

            // Console.WriteLine(ZatvorenoAI.WinRate);
        }

        // ReSharper disable once UnusedMember.Local
        private static ISantaseGame CreateTwoPlayerGame()
        {
            Console.BufferHeight = Console.WindowHeight = 17;
            Console.BufferWidth = Console.WindowWidth = 50;

            IPlayer firstPlayer = new ConsolePlayer(5, 10);
            IPlayer secondPlayer = new ConsolePlayer(10, 10);
            ISantaseGame game = new SantaseGame(firstPlayer, secondPlayer);
            return game;
        }

        // ReSharper disable once UnusedMember.Local
        private static ISantaseGame CreateGameVersusBot()
        {
            Console.BufferHeight = Console.WindowHeight = 17;
            Console.BufferWidth = Console.WindowWidth = 50;

            IPlayer firstPlayer = new ZatvorenoAI();
            IPlayer secondPlayer = new SmartPlayer();
            ISantaseGame game = new SantaseGame(firstPlayer, secondPlayer);
            return game;
        }
    }
}
