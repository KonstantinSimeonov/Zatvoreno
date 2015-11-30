namespace ZatvorenoAI.CardEvaluator
{
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using System.Collections.Generic;

    public interface ICardEval
    {
        float CardScore(Card card, PlayerTurnContext context, ICollection<Card> allowedCards);
    }
}