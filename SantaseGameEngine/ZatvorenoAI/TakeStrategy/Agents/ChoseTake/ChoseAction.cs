namespace ZatvorenoAI.TakeStrategy.Agents.ChoseTake
{
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class ChoseAction : IChoseAction
    {
        private readonly IPossibleActions possibleActions;
        private readonly IShouldTake shouldTake;

        public ChoseAction(IPossibleActions possibleActions, IShouldTake shouldTake)
        {
            this.shouldTake = shouldTake;
            this.possibleActions = possibleActions;
        }

        public Card CardToPlay(PlayerTurnContext context, ICollection<Card> hand)
        {
            var gameActions = this.possibleActions.GetPossibleTakeCard(context, hand);
            var response = this.shouldTake.ShouldPlayerTakeResponse(context, hand);

            if (response.Annouce)
            {
                var announceCards = hand
                    .Where(c => c.Type == CardType.King || c.Type == CardType.Queen)
                    .GroupBy(c => c.Suit)
                    .Where(g => g.Count() == 2)
                    .SelectMany(g => g); // TODO: This is ugly.

                var takesForAnounceCase = gameActions.Where(x => x.PlayerTakes && !announceCards.Contains(x.PlayerCard)).ToList();
                int minValue;
                if (takesForAnounceCase.Count == 0)
                {
                    minValue = gameActions.Min(x => x.HandValue);
                    return gameActions.First(x => x.HandValue == minValue).PlayerCard;
                }

                minValue = takesForAnounceCase.Min(x => x.HandValue);
                return takesForAnounceCase.First(x => x.HandValue == minValue).PlayerCard;
            }

            if (response.OpponentWins)
            {
                var takesForOpponentWins = gameActions.Where(x => x.PlayerTakes).ToList();

                int minValue;
                int maxValue;
                if (takesForOpponentWins.Count == 0)
                {
                    minValue = gameActions.Min(x => x.HandValue);
                    return gameActions.First().PlayerCard;
                }

                maxValue = takesForOpponentWins.Max(x => x.HandValue);
                return takesForOpponentWins.First(x => x.HandValue == maxValue).PlayerCard;
            }

            if (response.PlayerWins)
            {
                var takesPlayerWin = gameActions.Where(x => x.PlayerTakes).ToList();

                var maxValue = takesPlayerWin.Max(x => x.HandValue);
                return takesPlayerWin.First(x => x.HandValue == maxValue).PlayerCard;
            }

            if (response.GetLastTrump)
            {
                var weakCard = gameActions.Where(a => !a.PlayerTakes).OrderBy(a => a.HandValue).FirstOrDefault();
                if (weakCard != null)
                {
                    return weakCard.PlayerCard;
                }
            }

            if (response.Take)
            {
                var take = gameActions.Where(x => x.PlayerTakes && x.PlayerCard.Suit != context.TrumpCard.Suit).ToList();

                int value;
                if (take.Count == 0)
                {
                    value = gameActions.Min(x => x.HandValue);
                    return gameActions.First(x => x.HandValue == value).PlayerCard;
                }

                value = take.Min(x => x.HandValue);
                return take.First(x => x.HandValue == value).PlayerCard;
            }

            if (response.OpponentHighCard)
            {
                var trumpCard = hand.Where(c => c.Suit == context.TrumpCard.Suit).OrderByDescending(c => c.GetValue()).First();
                return trumpCard;
            }

            if (context.State.ShouldObserveRules)
            {
                var cardsToReturn = gameActions
                    .Where(g => g.PlayerCard.Suit == g.OpponetCard.Suit);
                if (cardsToReturn.Count() == 0)
                {
                    var trumpCards = gameActions.Where(g => g.PlayerCard.Suit == context.TrumpCard.Suit);

                    if (trumpCards.Count() == 0)
                    {
                        return hand.OrderBy(c => c.GetValue()).First();
                    }

                    return trumpCards.OrderBy(g => g.HandValue).First().PlayerCard;
                }

                return cardsToReturn.OrderBy(g => g.HandValue).First().PlayerCard;
            }

            return hand.Where(c => c.Suit != context.TrumpCard.Suit).OrderBy(c => c.GetValue()).First();
        }
    }
}
