namespace ZatvorenoAI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Contracts;
    using Santase.Logic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class ZatvorenoAI : BasePlayer
    {
        private const string AIName = "Peshoq";

        private static readonly IReport GameReport = new Report();        

        public override string Name
        {
            get
            {
                return AIName;
            }
        }

        public static string GetReports()
        {
            return GameReport.ToString();
        }

        public override PlayerAction GetTurn(PlayerTurnContext context)
        {
            var myPoints = context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
                GameReport.TrumpsChanged++;
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
                    GameReport.AnnounceStatistics[gosho]++;
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
                            GameReport.TrumpedHighCards++;
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

        public override void EndGame(bool amIWinner)
        {
            if (amIWinner)
            {
                GameReport.Wins++;
            }

            base.EndGame(amIWinner);
        }
    }
}
