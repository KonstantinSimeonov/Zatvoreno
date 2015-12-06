namespace ZatvorenoAI.TakeStrategy.Agents.NeedToTake.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using Agents.ShouldTake.Response;

    public interface IShouldTake
    {
        bool ShouldPlayerTake(PlayerTurnContext context, ICollection<Card> hand);

        ShouldTakeResponse ShouldPlayerTakeResponse(PlayerTurnContext context, ICollection<Card> hand);
    }
}
