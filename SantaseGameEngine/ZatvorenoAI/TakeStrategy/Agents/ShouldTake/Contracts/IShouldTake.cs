namespace ZatvorenoAI.TakeStrategy.Agents.NeedToTake.Contracts
{
    using System.Collections.Generic;

    using Agents.ShouldTake.Response;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IShouldTake
    {
        ShouldTakeResponse ShouldPlayerTakeResponse(PlayerTurnContext context, ICollection<Card> hand);
    }
}
