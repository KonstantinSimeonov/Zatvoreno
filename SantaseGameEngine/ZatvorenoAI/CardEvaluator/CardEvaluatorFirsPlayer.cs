namespace ZatvorenoAI.CardEvaluator
{
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CardEvaluatorFirsPlayer : ICardEval
    {
        private float pointValueMultiplier = 3.0f;
        private float remaingMultiplier = 3f;
        private float suitCountMutiplier = 2f;
        private float multy = 2f;

        private ICardTracer cardTracer;
        private ZatvorenoAI player;

        public CardEvaluatorFirsPlayer(ICardTracer ct)
        {
            this.cardTracer = ct;
        }

        public float CardScore (Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var cardSuit = card.Suit;
            var cardValue = card.GetValue();
            var coutOfSuitInHand = allowedCards.Count(x => x.Suit == cardSuit);

            return (11 - cardValue) * coutOfSuitInHand;
        }
    }
}
