namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using CardTracers.CardStates;

    public interface ICardTracker
    {
        Dictionary<CardSuit, Dictionary<int, CardTracerState>> AllCards { get; }

        void TraceTurn(PlayerTurnContext context);

        void NewGame();
    }
}
