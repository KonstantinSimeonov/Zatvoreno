namespace ZatvorenoAI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class ZatvorenoAI : BasePlayer
    {
        public static int WinRate = 0;

        private const string name = "Peshoq";

        public override string Name
        {
            get
            {
                return name;
            }
        }

        public override PlayerAction GetTurn(PlayerTurnContext context)
        {
            var myPoints = context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
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
                WinRate++;
            }

            base.EndGame(amIWinner);
        }
    }
}
