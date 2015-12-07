﻿namespace ZatvorenoAI.PlayFirstStrategy.CardStatistics
{
    using System.Collections.Generic;
    using System.Linq;

    using CardStaistic;
    using CardTracers.CardStates;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
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
                var taken3 = this.NumberOfCardsThatTakeCard(item, context);
                stats.Add(new CardStatistic(worth, canTake, taken3, item));
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

            if (card.Suit != context.TrumpCard.Suit && count == 0 && context.State.ShouldObserveRules)
            {
                count += this.cardTracker.AllCards[context.TrumpCard.Suit].Count(x => x.Value == CardTracerState.InOpponentHand || x.Value == CardTracerState.Unknown);
            }

            if (card.Suit != context.TrumpCard.Suit && !context.State.ShouldObserveRules)
            {
                count += this.cardTracker.AllCards[context.TrumpCard.Suit].Count(x => x.Value == CardTracerState.InOpponentHand || x.Value == CardTracerState.Unknown);
            }

            return count;
        }
    }
}
