namespace ZatvorenoAI.CardEvaluator
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using CardTracers.CardStates;

    public class CardEvaluatorFirsPlayer : ICardEval
    {
        private ICardTracker cardTracker;

        public CardEvaluatorFirsPlayer(ICardTracker ct)
        {
            this.cardTracker = ct;
        }

        public float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            float result = 0f;
            result += this.PointAndSuitCountParameter(card, context, allowedCards);
            result -= 0.5f * this.PointAndSuitCountParameter(card, context, allowedCards);

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

            var allOfSuit = this.cardTracker.AllCards[suit];
            var maxAvailableForTaking = allOfSuit.Where(x => x.Value != CardTracerState.TakenByOpponent ||
                                                            x.Value != CardTracerState.TakenByPlayer).Max(x => x.Key);
            return (value + maxAvailableForTaking) / 21f;
        }
    }
}
