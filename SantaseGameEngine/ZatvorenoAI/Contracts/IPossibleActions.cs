namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using TakeStrategy.Agents.PossibleTakes.Action;

    public interface IPossibleActions
    {
        ICollection<GameAction> GetPossibleTakeCard(PlayerTurnContext context, ICollection<Card> hand);
    }
}
