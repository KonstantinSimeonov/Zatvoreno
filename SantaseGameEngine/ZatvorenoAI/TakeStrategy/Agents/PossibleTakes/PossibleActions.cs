namespace ZatvorenoAI.TakeStrategy.Agents.PossibleTakes
{
    using System.Collections.Generic;
    using Action;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class PossibleActions : IPossibleActions
    {
        private readonly ICardTracker cardTracker;

        public PossibleActions(ICardTracker cT)
        {
            this.cardTracker = cT;
        }

        public ICollection<GameAction> GetPossibleTakeCard(PlayerTurnContext context, ICollection<Card> hand)
        {
            var actions = new List<GameAction>();

            foreach (var item in hand)
            {
                actions.Add(new GameAction(context.FirstPlayedCard, item, context, this.cardTracker));
            }

            return actions;
        }
    }
}
