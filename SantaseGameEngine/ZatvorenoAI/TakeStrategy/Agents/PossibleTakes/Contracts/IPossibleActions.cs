namespace ZatvorenoAI.TakeStrategy.Agents.PossibleTakes.Contracts
{
    using System.Collections.Generic;
    using Action;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IPossibleActions
    {
        ICollection<GameAction> GetPossibleTakeCard(PlayerTurnContext context, ICollection<Card> hand);
    }
}
