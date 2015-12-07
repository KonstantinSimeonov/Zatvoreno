namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IFistActionInTrickChoser
    {
        KeyValuePair<bool, Card> CardToPlayAndCloseLogic(PlayerTurnContext context, ICollection<Card> hand);
    }
}