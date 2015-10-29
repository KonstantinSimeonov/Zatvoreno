﻿namespace ZatvorenoAI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Santase.Logic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class ZatvorenoAI : BasePlayer
    {
        public const string ReportFormat = "Games won: {1}{0}Trumps changes: {2}{0}Trumped aces and tens: {3}{0}Announces:{0}  - Forty: {4}{0}  - Twenty: {5}{0}";
        public static int WinRate = 0;
        public static int TrumpsChanged = 0;
        public static int TrumpedHighCards = 0;
        public static IDictionary<Announce, int> AnnounceStatistics = new Dictionary<Announce, int>()
        {
            { Announce.Forty, 0 },
            { Announce.Twenty, 0 }
        };

        private const string name = "Peshoq";

        private ICollection<Card> cardsPlayed;

        public override string Name
        {
            get
            {
                return name;
            }
        }

        public ZatvorenoAI ()
        {
            this.cardsPlayed = new List<Card>();
        }

        public ICollection<Card> CardsPlayed
        {
            get
            {
                return this.cardsPlayed;
            }

            set
            {
                this.cardsPlayed = value;
            }
        }

        public override void EndTurn (PlayerTurnContext context)
        {
            this.cardsPlayed.Add(context.FirstPlayedCard);
            this.cardsPlayed.Add(context.SecondPlayedCard);

            base.EndTurn(context);
        }

        public override void EndRound ()
        {
            cardsPlayed.Clear();

            base.EndRound();
        }

        public override PlayerAction GetTurn (PlayerTurnContext context)
        {
            var myPoints = GetMyPoints(context);

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
                TrumpsChanged++;
                return this.ChangeTrump(context.TrumpCard);
            }

            // TODO: closing logic

            var cardsToPlay = this.PlayerActionValidator.GetPossibleCardsToPlay(context, this.Cards);

            var announceCards = cardsToPlay
                                    .Where(c => c.Type == CardType.King || c.Type == CardType.Queen)
                                    .GroupBy(c => c.Suit)
                                    .Where(g => g.Count() > 1)
                                    .ToList();

            var gosho = this.AnnounceValidator.GetPossibleAnnounce(this.Cards, announceCards.FirstOrDefault() == null ? null : announceCards.FirstOrDefault().FirstOrDefault(), context.TrumpCard, context.IsFirstPlayerTurn);

            if (gosho == Santase.Logic.Announce.Forty || gosho == Santase.Logic.Announce.Twenty)
            {
                if (this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(announceCards.FirstOrDefault().First()), context, this.Cards))
                {
                    AnnounceStatistics[gosho]++;
                    return this.PlayCard(announceCards.FirstOrDefault().First());
                }
            }

            if (!context.IsFirstPlayerTurn)
            {
                if (context.FirstPlayedCard.Type == CardType.Ace || context.FirstPlayedCard.Type == CardType.Ten)
                {
                    if (context.TrumpCard.Suit != context.FirstPlayedCard.Suit)
                    {
                        var trump = cardsToPlay.FirstOrDefault(x => x.Suit == context.TrumpCard.Suit);

                        if (trump != null && this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(trump), context, this.Cards))
                        {
                            TrumpedHighCards++;
                            return this.PlayCard(trump);
                        }

                    }

                }

                var cards2Play = cardsToPlay
                                    .Where(c => context.FirstPlayedCard.Suit == c.Suit && context.FirstPlayedCard.Type.CompareTo(c.Type) < 0)
                                    .OrderByDescending(c => c.Type)
                                    .ToList();

                var card = cards2Play.FirstOrDefault();

                if (card != null)
                {
                    return this.PlayCard(card);
                }
            }

            return this.PlayCard(this.PlayerActionValidator.GetPossibleCardsToPlay(context, this.Cards).First());
        }

        private static int GetMyPoints (PlayerTurnContext context)
        {
            return context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;
        }

        public override void EndGame (bool amIWinner)
        {
            if (amIWinner)
            {
                WinRate++;
            }

            base.EndGame(amIWinner);
        }

        private bool ShouldCloseGame (PlayerTurnContext context, ICollection<Card> cards)
        {
            var trumpsInPower = cards.Where(x => x.Suit == context.TrumpCard.Suit).OrderBy(x => x.Type);

            if (GetMyPoints(context) + trumpsInPower.Sum(x => (long?)x.Type) >= 66)
            {

            }

            return false;
        }

        public static string GetReports()
        {
            // "Games won: {1}{0}Trumps changes: {2}{0} Trumped aces and tens: {3}{0} Announces:{0}  - Forty: {4}{0}  - Twenty: {5}{0}";
            return string.Format(
                                 ReportFormat,
                                 Environment.NewLine,
                                 WinRate,
                                 TrumpsChanged,
                                 TrumpedHighCards,
                                 AnnounceStatistics[Announce.Forty],
                                 AnnounceStatistics[Announce.Twenty]);
        }
    }
}
