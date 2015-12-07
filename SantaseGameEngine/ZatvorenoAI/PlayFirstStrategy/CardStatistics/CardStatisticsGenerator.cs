namespace ZatvorenoAI.PlayFirstStrategy.CardStatistics
{
    using System.Collections.Generic;
    using System.Linq;
    using CardTracers.CardStates;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class CardStatisticsGenerator : ICardStatisticsGenerator
    {
        private readonly ICardTracker cardTracker;

        public CardStatisticsGenerator(ICardTracker cT)
        {
            this.cardTracker = cT;
        }

        public ICollection<CardStatistic> GenerateCardStats(PlayerTurnContext context, ICollection<Card> hand)
        {
            var stats = new List<CardStatistic>();

            foreach (var item in hand)
            {
                var worth = this.CardWorth(item, hand, context);
                var canTake = this.NumberOfHandsCardCanTake(item, context);
                var takenBy = this.NumberOfCardsThatTakeCard(item, context);
                var lengthOfSuit = this.LengthOfSuit(item, hand, context);
                stats.Add(new CardStatistic(worth, canTake, takenBy, lengthOfSuit, item));
            }

            return stats;
        }

        private int CardWorth(Card card, ICollection<Card> hand, PlayerTurnContext context)
        {
            var possibleTakes = this.cardTracker.AllCards[card.Suit]
                .Where(x => (x.Value == CardTracerState.InOpponentHand || x.Value == CardTracerState.Unknown)
                            && x.Key < card.GetValue())
                .ToList();

            if (possibleTakes.Count == 0)
            {
                return card.GetValue();
            }

            var value = card.GetValue() + possibleTakes.Max(x => x.Key);
            return value;
        }

        private int LengthOfSuit(Card card, ICollection<Card> hand, PlayerTurnContext context)
        {
            var length = hand.Where(c => c.Suit == card.Suit).Count();

            return 0;
        }

        private int NumberOfHandsCardCanTake(Card card, PlayerTurnContext context)
        {
            var count = this.cardTracker.AllCards[card.Suit]
                   .Where(x => (x.Value == CardTracerState.InOpponentHand || x.Value == CardTracerState.Unknown)
                               && x.Key < card.GetValue())
                   .Count();

            return count;
        }

        private int NumberOfCardsThatTakeCard(Card card, PlayerTurnContext context)
        {
            var count = this.cardTracker.AllCards[card.Suit]
                  .Where(x => (x.Value == CardTracerState.InOpponentHand || x.Value == CardTracerState.Unknown)
                              && x.Key > card.GetValue())
                  .Count();

            return count;
        }
    }
}
