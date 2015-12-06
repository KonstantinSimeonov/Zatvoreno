namespace ZatvorenoAI.TakeStrategy.Agents.ChoseTake
{
    using System.Linq;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using PossibleTakes.Action;
    using NeedToTake.Contracts;
    using PossibleTakes.Contracts;
    using Santase.Logic.Cards;
    using System.Collections.Generic;
    using Santase.Logic.Players;

    public class ChoseAction : IChoseAction
    {
        private readonly IPossibleActions possibleActions;
        private readonly IShouldTake shouldTake;
        private readonly ICardTracker cardTracker;

        public ChoseAction(IPossibleActions pA, IShouldTake sT)
        {
            this.shouldTake = sT;
            this.possibleActions = pA;
        }

        public Card CardToPlay(PlayerTurnContext context, ICollection<Card> hand)
        {
            var gameActions = this.possibleActions.GetPossibleTakeCard(context, hand);
            var response = this.shouldTake.ShouldPlayerTakeResponse(context, hand);

            if (response.Annouce)
            {
                var takesForAnounceCase = gameActions.Where(x => x.PlayerTakes && (x.PlayerCard.GetValue() > 4 && x.PlayerCard.GetValue() < 3)).ToList();

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

            var take = gameActions.Where(x => x.PlayerTakes).ToList();

            int value;
            if (take.Count == 0)
            {
                value = gameActions.Min(x => x.HandValue);
                return gameActions.First(x => x.HandValue == value).PlayerCard;
            }

            value = take.Min(x => x.HandValue);
            return take.First(x => x.HandValue == value).PlayerCard;
        }
    }
}
