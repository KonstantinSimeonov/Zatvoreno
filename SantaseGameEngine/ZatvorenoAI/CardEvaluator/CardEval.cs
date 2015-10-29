namespace ZatvorenoAI.CardEvaluator
{
    using System.Linq;
    using CardTracers;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class CardEval : ICardEvaluator
    {
        public const float PointValueParameter = 1;
        public const float TrickValueParameter = 1;
        public const float OpponetPointsValueParameter = 1;
        public const float MyPointsValueParameter = 1;

        private ICardTracer cardTracer;

        public CardEval(ICardTracer ct)
        {
            this.cardTracer = ct;
        }

        public float CardPlayabilityValue(Card card, PlayerTurnContext context)
        {
            var value = 0f;

            value += this.CardPointValueMod(card);
            value += this.NumberOfPossibleHandsWonInSuitMod(card);

            if (context.IsFirstPlayerTurn)
            {
                value += this.OpponetsPointsMod(card, context);
            }
            else
            {
                value += this.MyPointsMod(card, context);
            }

            value += this.CardPointValueMod(card);

            return value;
        }

        private float CardPointValueMod (Card card)
        {
            return CardEval.PointValueParameter * card.GetValue();
        }

        private float NumberOfPossibleHandsWonInSuitMod(Card card)
        {
            var cardSuit = card.Suit;
            var cardValue = (float)card.GetValue();

            var possibleHandsCount = this.cardTracer.PlayedCards
                .Where(x => x.Suit == cardSuit)
                .Select(x => new
                {
                    Values = x.GetValue()
                })
                .Count(x => x.Values < cardValue);

            return -CardEval.TrickValueParameter * (float)possibleHandsCount;
        }

        private float OpponetsPointsMod(Card card, PlayerTurnContext context)
        {
            return -card.GetValue() / (66 - context.FirstPlayerRoundPoints) * CardEval.OpponetPointsValueParameter;
        }

        private float MyPointsMod(Card card, PlayerTurnContext context)
        {
            return +card.GetValue() / (66-context.SecondPlayerRoundPoints) * CardEval.OpponetPointsValueParameter;
        }
    }
}
