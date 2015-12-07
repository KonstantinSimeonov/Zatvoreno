namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface ICardEval
    {
        float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards);
    }
}