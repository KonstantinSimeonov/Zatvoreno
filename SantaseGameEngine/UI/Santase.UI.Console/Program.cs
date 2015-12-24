namespace Santase.UI.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using AI.SmartPlayer;
    using Logic;
    using Logic.GameMechanics;
    using Logic.Players;
    using Logic.Cards;
    using ZatvorenoAI;
    using ZatvorenoAI.MCST;
    using System.Text;

    public static class Program
    {
        public static void TestAnalyzer()
        {
            var myCards = new List<Card>()
            {
                new Card(CardSuit.Club, CardType.Ace),
                new Card(CardSuit.Heart, CardType.King),
                new Card(CardSuit.Club, CardType.Nine),
                new Card(CardSuit.Spade, CardType.Ten),
                new Card(CardSuit.Diamond, CardType.Nine),
                new Card(CardSuit.Heart, CardType.Ten),
            };

            var oppCards = new List<Card>()
            {
                new Card(CardSuit.Club, CardType.Ten),
                new Card(CardSuit.Heart, CardType.Queen),
                new Card(CardSuit.Spade, CardType.Ace),
                new Card(CardSuit.Spade, CardType.Nine),
                new Card(CardSuit.Diamond, CardType.Jack),
                new Card(CardSuit.Heart, CardType.Jack),
            };

            var root = new Node(null, new Card(CardSuit.Diamond, CardType.Ace), true);

            EndgameAnalyzer.Compute(root, null, myCards, oppCards, 40, 40);

            output = new StringBuilder();
            OutInBuilder(root);

            // File.WriteAllText("../../report.txt", output.ToString());

            foreach (var c in root.Children)
            {
                Console.WriteLine(c.Wins + "/" + c.Total + " " + c.Card);
            }

            Console.WriteLine(root.Wins + " " + root.Total);
        }

        static StringBuilder output;

        static void OutInBuilder(Node root)
        {
            var nodes = new Stack<Node>();

            nodes.Push(root);

            while (nodes.Count > 0)
            {
                var current = nodes.Pop();

                output.AppendLine(current.ToString());

                if (current.Children.Count == 0)
                {
                    break;
                }

                foreach (var c in current.Children)
                {
                    nodes.Push(c);
                }
            }
        }

        public static void Main()
        {
            //TestAnalyzer();

            //return;

            var sw = new Stopwatch();

            sw.Start();
            for (int i = 0, length = 1000; i < length; i++)
            {
                var game = CreateGameVersusBot();
                game.Start(PlayerPosition.FirstPlayer);
                if (i < 10)
                {
                    File.WriteAllText("../../Reports/report" + i + ".txt", ZatvorenoAI.Report.ToString());
                    ZatvorenoAI.Report.Empty();
                }

                if (i % (length / 20) == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(i + " games done");
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
