namespace ZatvorenoAI.CardEvaluator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CardTracers.CardStates;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class CardEvaluatorWorthBased : ICardEval
    {
        private ICardTracker cardTracker;

        public CardEvaluatorWorthBased(ICardTracker ct)
        {
            this.cardTracker = ct;
        }

        public float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            return 0;
        }

        private float CurrentCardWorth(Card card, PlayerTurnContext context, ICollection<Card> allowedCards)
        {
            var result = 0f;

            var cardValue = card.GetValue();

            if (cardValue == 0)
            {
                result = this.NineEvaluation(card, context);
            }
            else if (cardValue == 3 || cardValue == 4)
            {
                result = this.QueenKingEvaliation(card, context, allowedCards);
            }
            else
            {
                result = this.MaxHandValue(card, context);
            }

            return result;
        }

        private float NineEvaluation(Card nine, PlayerTurnContext context)
        {
            var isTrump = nine.Suit == context.TrumpCard.Suit;

            if (isTrump)
            {
                return context.TrumpCard.GetValue();
            }

            return 0f;
        }

        private float QueenKingEvaliation(Card card, PlayerTurnContext context, ICollection<Card> hand)
        {
            var result = 0f;
            var value = card.GetValue();
            var valueOfCounterPart = value == 4 ? 3 : 4;
            var anouceValue = card.Suit == context.TrumpCard.Suit ? 40f : 20f;

            var suit = card.Suit;

            var havePair = this.cardTracker.AllCards[suit][valueOfCounterPart] == CardTracerState.InHand;
            var counterPartCardTrackInfo = this.cardTracker.AllCards[suit][valueOfCounterPart];
            var pairIsPossible = counterPartCardTrackInfo == CardTracerState.Unknown ||
                counterPartCardTrackInfo == CardTracerState.TrumpIndicator;

            if (havePair)
            {
                result = anouceValue + value;
            }
            else if (pairIsPossible)
            {
                result = (anouceValue / 2f) + value;
            }
            else
            {
                result += this.MaxHandValue(card, context);
            }

            return result;
        }

        private float MaxHandValue(Card card, PlayerTurnContext context)
        {
            var suit = card.Suit;
            var value = card.GetValue();
            var isTrump = context.TrumpCard.Suit == suit;

            var result = 0f;

            var cardsToTake = this.cardTracker.AllCards[suit].Where(x => x.Key < value &&
                                (x.Value == CardTracerState.InOpponentHand || x.Value == CardTracerState.Unknown));

            var high = cardsToTake.Max(x => x.Key);

            result += high;

            if (isTrump)
            {
                result += 10;
            }

            return 5f*result /(this.MaxTakes(value) - cardsToTake.Count());
        }

        private int MaxTakes(int point)
        {
            int possibleHands = 0;

            switch (point)
            {
                case 0:
                    possibleHands = 0;
                    break;
                case 2:
                    possibleHands = 1;
                    break;
                case 3:
                    possibleHands = 2;
                    break;
                case 4:
                    possibleHands = 3;
                    break;
                case 10:
                    possibleHands = 4;
                    break;
                case 11:
                    possibleHands = 5;
                    break;
                default:
                    throw new ArgumentException("Wrong card point value");
            }

            return possibleHands;
        }
    }
}
