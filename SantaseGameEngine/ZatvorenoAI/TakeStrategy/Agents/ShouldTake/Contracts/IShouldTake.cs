namespace ZatvorenoAI.TakeStrategy.Agents.NeedToTake.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IShouldTake
    {
        bool ShouldPlayerTake(PlayerTurnContext context, ICollection<Card> hand);
    }
}
