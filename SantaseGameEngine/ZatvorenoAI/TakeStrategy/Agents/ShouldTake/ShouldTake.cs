namespace ZatvorenoAI.TakeStrategy.Agents.NeedToTake
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CardTracers.CardStates;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class ShouldTake : IShouldTake
    {
        private readonly ICardTracker cardTracker;

        public ShouldTake(ICardTracker cT)
        {
            this.cardTracker = cT;
        }

        public bool ShouldPlayerTake(PlayerTurnContext context, ICollection<Card> hand)
        {
            // Inlining unused var.
            // var shouldTake = false;

            // must take
            if (this.CheckHandForAnnounces())
            {
                ZatvorenoAI.Report.Add("Reason for taking: Because Can Announce");
                return true;
            }

            if (this.OpponentWins(context, hand))
            {
                ZatvorenoAI.Report.Add("Reason for taking: Because Opponent Would Win");
                return true;
            }

            if (this.PlayerWins(context, hand))
            {
                ZatvorenoAI.Report.Add("Reason for taking: Because I Win");
                return true;
            }

            if (this.OpponentPlaysTooHigh(context, hand))
            {
                ZatvorenoAI.Report.Add("Reason for taking: Because Opponent Played Too High");
                return true;
            }

            if (this.HaveHigherCard(context, hand))
            {
                ZatvorenoAI.Report.Add("Reason for taking: Because I Can!");
                return true;
            }

            // TODO:  should it take depending on possible takes
            return false; // shouldTake;
        }

        private bool HaveHigherCard(PlayerTurnContext context, ICollection<Card> hand)
        {
            if (context.FirstPlayedCard.Suit == context.TrumpCard.Suit) // TODO: Improve this.
            {
                if (context.FirstPlayedCard.Type == CardType.Ten && // TODO: Maybe separate in another agent.
                    hand.Any(c => c.Type == CardType.Ace && c.Suit == context.TrumpCard.Suit))
                {
                    return true;
                }

                return false;
            }

            var shouldTake = hand.Any(c => c.Suit == context.FirstPlayedCard.Suit &&
                                           c.GetValue() > context.FirstPlayedCard.GetValue());

            return shouldTake;
        }

        private bool OpponentPlaysTooHigh(PlayerTurnContext context, ICollection<Card> hand)
        {
            var shouldTake = false;

            if (context.FirstPlayedCard.Type == CardType.Ace ||
                context.FirstPlayedCard.Type == CardType.Ten)
            {
                var trumpSuit = this.cardTracker
                                    .AllCards[context.TrumpCard.Suit];

                foreach (var card in trumpSuit)
                {
                    if ((card.Key != 3 && card.Key != 4) && card.Value == CardTracerState.InHand)
                    {
                        shouldTake = true;
                        break;
                    }
                }
            }

            return shouldTake;
        }

        // must take in all casses
        private bool CheckHandForAnnounces()
        {
            var announceIsPresent = false;

            foreach (var item in this.cardTracker.AllCards)
            {
                var cardsOfSuit = item.Value;

                if (cardsOfSuit[3] == CardTracerState.InHand && cardsOfSuit[4] == CardTracerState.InHand)
                {
                    announceIsPresent = true;
                    break;
                }
            }

            return announceIsPresent;
        }

        private bool OpponentWins(PlayerTurnContext context, ICollection<Card> hand)
        {

            var opponetWinsWithHand = false;

            var opponetPoints = context.FirstPlayerRoundPoints;
            var oppnetCard = context.FirstPlayedCard.GetValue();

            var smallestCard = hand.Where(x => x.Suit != context.TrumpCard.Suit).OrderBy(x => x.GetValue()).FirstOrDefault();

            if (smallestCard == null)
            {
                smallestCard = hand.OrderBy(x => x.GetValue()).First();
            }

            if (opponetPoints + smallestCard.GetValue() + oppnetCard >= 66)
            {
                opponetWinsWithHand = true;
            }

            return opponetWinsWithHand;
        }

        private bool PlayerWins(PlayerTurnContext context, ICollection<Card> hand)
        {
            var playerWinsWithHand = false;

            var playerPoints = context.SecondPlayerRoundPoints;
            var playedCard = context.FirstPlayedCard;

            var playedCardIsTrump = playedCard.Suit == context.TrumpCard.Suit;

            var canTake = hand.Where(x => (x.Suit == playedCard.Suit ||
                                    (playedCard.Suit != context.TrumpCard.Suit
                                    && x.Suit == context.TrumpCard.Suit))
                                    && x.GetValue() > playedCard.GetValue())
                              .OrderByDescending(x => x.GetValue())
                              .FirstOrDefault();

            if (canTake == null)
            {
                return playerWinsWithHand;
            }

            if (playerPoints + playedCard.GetValue() + canTake.GetValue() >= 66)
            {
                playerWinsWithHand = true;
            }

            return playerWinsWithHand;
        }
    }
}
