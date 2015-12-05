namespace ZatvorenoAI.TakeStrategy.Agents.NeedToTake
{
    using System.Collections.Generic;
    using System.Linq;
    using CardTracers.CardStates;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class ShouldTake : IShouldTake
    {
        private readonly ICardTracker cardTracker;

        public ShouldTake(ICardTracker cT)
        {
            this.cardTracker = cT;
        }

        public bool ShouldPlayerTake(PlayerTurnContext context, ICollection<Card> hand)
        {
            var shouldTake = false;

            // must take
            if (this.CheckHandForAnnounces())
            {
                return true;
            }

            if (this.OpponentWins(context, hand))
            {
                return true;
            }

            if (this.PlayerWins(context, hand))
            {
                return true;
            }

            // TODO:  should it take depending on possible takes
            return shouldTake;
        }

        // must take in all casses
        private bool CheckHandForAnnounces()
        {
            var announceIsPresent = false;

            foreach (var item in this.cardTracker.AllCards)
            {
                var cardsOfSuit = item.Value;

                if (cardsOfSuit[3] == CardTracerState.InHand && cardsOfSuit[4] == CardTracerState.InHand)
                {
                    announceIsPresent = true;
                    break;
                }
            }

            return announceIsPresent;
        }

        private bool OpponentWins(PlayerTurnContext context, ICollection<Card> hand)
        {
            var opponetWinsWithHand = false;

            var opponetPoints = context.FirstPlayerRoundPoints;
            var oppnetCard = context.FirstPlayedCard.GetValue();

            var smallestCard = hand.Where(x => x.Suit != context.TrumpCard.Suit).Min(x => x.GetValue());

            if (opponetPoints + smallestCard + oppnetCard >= 66)
            {
                opponetWinsWithHand = true;
            }

            return opponetWinsWithHand;
        }

        private bool PlayerWins(PlayerTurnContext context, ICollection<Card> hand)
        {
            var playerWinsWithHand = false;

            var playerPoints = context.SecondPlayerRoundPoints;
            var playedCard = context.FirstPlayedCard;

            var playedCardIsTrump = playedCard.Suit == context.TrumpCard.Suit;


            var canTake = hand.Where(x => (x.Suit == playedCard.Suit ||
                                    (playedCard.Suit != context.TrumpCard.Suit
                                    && x.Suit == context.TrumpCard.Suit))
                                    && x.GetValue() > playedCard.GetValue())
                              .OrderByDescending(x => x.GetValue())
                              .First();

            if (canTake == null)
            {
                return playerWinsWithHand;
            }

            if (playerPoints + playedCard.GetValue() + canTake.GetValue() >= 66)
            {
                playerWinsWithHand = true;
            }


            return playerWinsWithHand;
        }
    }
}
