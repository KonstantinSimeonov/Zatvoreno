namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface ICardTracer
    {
        Card CurrentTrumpCard { get; set; }

        ICollection<Card> OpponentCards { get; set; }

        ICollection<Card> PlayedCards { get; set; }

        void Empty();

        void TraceTurn(PlayerTurnContext context);
    }
}