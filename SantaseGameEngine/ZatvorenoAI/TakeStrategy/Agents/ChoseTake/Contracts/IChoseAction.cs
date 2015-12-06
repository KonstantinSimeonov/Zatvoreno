namespace ZatvorenoAI.TakeStrategy.Agents.ChoseTake.Contracts
{
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using System.Collections.Generic;

    public interface IChoseAction
    {
        Card CardToPlay(PlayerTurnContext context, ICollection<Card> hand);
    }
}
