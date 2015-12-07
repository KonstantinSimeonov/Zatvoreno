﻿namespace ZatvorenoAI.CardEvaluator
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class CardEval : ICardEval
    {
        public const int WinPoints = 66;
        public const float PointValueParameter = 1;
        public const float TrickValueParameter = 1;
        public const float OpponetPointsValueParameter = 1;
        public const float MyPointsValueParameter = 1;

        private ICardTracer cardTracer;

        public CardEval(ICardTracer ct)
        {
            this.cardTracer = ct;
        }

        public float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
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

        private float CardPointValueMod(Card card)
        {
            return PointValueParameter * card.GetValue();
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

            return -TrickValueParameter * possibleHandsCount;
        }

        private float OpponetsPointsMod(Card card, PlayerTurnContext context)
        {
            return -card.GetValue() / (WinPoints - context.FirstPlayerRoundPoints) * OpponetPointsValueParameter;
        }

        private float MyPointsMod(Card card, PlayerTurnContext context)
        {
            return +card.GetValue() / (WinPoints - context.SecondPlayerRoundPoints) * OpponetPointsValueParameter;
        }
    }
}
