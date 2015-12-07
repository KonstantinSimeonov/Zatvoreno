namespace ZatvorenoAI.TakeStrategy.Agents.ChoseTake.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IChoseAction
    {
        Card CardToPlay(PlayerTurnContext context, ICollection<Card> hand);
    }
}