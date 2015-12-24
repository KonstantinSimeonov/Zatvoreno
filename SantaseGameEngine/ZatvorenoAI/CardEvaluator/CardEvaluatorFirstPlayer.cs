namespace ZatvorenoAI.CardEvaluator
{
    using System.Collections.Generic;
    using System.Linq;

    using CardTracers.CardStates;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class CardEvaluatorFirstPlayer : ICardEval
    {
        public const float PointAndSuitMultiplier = 0.5f;
        public const float BiggestGainDivisor = 21f;

        private ICardTracker cardtracker;

        public CardEvaluatorFirstPlayer(ICardTracker tracker)
        {
            this.cardtracker = tracker;
        }

        public float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            float result = 0f;
            result += this.PointAndSuitCountParameter(card, context, allowedCards);
            result -= PointAndSuitMultiplier * this.PointAndSuitCountParameter(card, context, allowedCards);

            return result;
        }

        private float PointAndSuitCountParameter(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var cardSuit = card.Suit;
            float cardValue = card.GetValue();
            float coutOfSuitInHand = allowedCards.Count(x => x.Suit == cardSuit);
            return (11f - cardValue) * coutOfSuitInHand;
        }

        private float BiggestGainParameter(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var suit = card.Suit;
            float value = card.GetValue();

            var allOfSuit = this.cardtracker.AllCards[suit];
            var maxAvailableForTaking = allOfSuit.Where(x => x.Value != CardTracerState.TakenByOpponent ||
                                                            x.Value != CardTracerState.TakenByPlayer).Max(x => x.Key);
            return (value + maxAvailableForTaking) / BiggestGainDivisor;
        }
    }
}
