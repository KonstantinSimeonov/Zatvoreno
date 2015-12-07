namespace ZatvorenoAI.PlayFirstStrategy.ActionChoser.Contracts
{
    using System.Collections.Generic;

    using Santase.Logic.Cards;
    using TurnContext.Response;
    using Santase.Logic.Players;

    public interface IFistActionInTrickChoser
    {
        KeyValuePair<bool, Card> CardToPlayAndCloseLogic(PlayerTurnContext context, ICollection<Card> hand);
    }
}
