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
            this.player = player;
        }

        public float CardScore (Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            return this.PointsParameter(card) + this.RemaingSuitParamater(card, context)
                + this.SuitParameter(card, context, allowedCards) + this.TrumpParameter(card,context);
        }

        private float PointsParameter(Card card)
        {
            return -card.GetValue() * this.pointValueMultiplier;
        }

        private float RemaingSuitParamater(Card card, PlayerTurnContext context)
        {
            var cardsRemaingFromTheSuit = 5 - this.cardTracer.PlayedCards.Count(x => x.Suit == card.Suit);
            var s = cardsRemaingFromTheSuit * this.remaingMultiplier;
            return s;

        }

        private float SuitParameter(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var countOfSuit = allowedCards.Count(x => x.Suit == card.Suit);

            return (float)countOfSuit / allowedCards.Count * this.suitCountMutiplier;
        }

        private float TrumpParameter(Card card, PlayerTurnContext context)
        {
            var x = card.Suit == context.TrumpCard.Suit ? 1 : 0;

            return x * this.multy;
        }
    }
}
