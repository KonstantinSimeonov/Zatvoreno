﻿namespace ZatvorenoAI.CardEvaluator
{
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CardEvaluatorSecondPlayer
    {
        private float pointValueMultiplier = 1.0f;
        private float annouceMultiplier = 1.0f;
        private float suitCountMutiplier = 1.0f;

        private ICardTracer cardTracer;
        private ZatvorenoAI player;

        public CardEvaluatorSecondPlayer(ICardTracer ct)
        {
            this.cardTracer = ct;
        }

        public float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var parameters = new List<float>();



            return parameters.Sum();
        }

        private float PointsParameter(Card card)
        {
            return -card.GetValue() * this.pointValueMultiplier;
        }

        private float AnnouceParameter(Card card, PlayerTurnContext context)
        {
            var isKingOrQueen = card.Type == CardType.King || card.Type == CardType.Queen;
            return (float)context.FirstPlayerAnnounce * this.annouceMultiplier * Convert.ToInt32(isKingOrQueen);
        }

        private float SuitParameter(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var countOfSuit = allowedCards.Count(x => x.Suit == card.Suit);

            return (float)countOfSuit / allowedCards.Count * this.suitCountMutiplier;
        }
    }
}