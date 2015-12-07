namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using TakeStrategy.Agents.ShouldTake.Response;

    public interface IShouldTake
    {
        ShouldTakeResponse ShouldPlayerTakeResponse(PlayerTurnContext context, ICollection<Card> hand);
    }
}
