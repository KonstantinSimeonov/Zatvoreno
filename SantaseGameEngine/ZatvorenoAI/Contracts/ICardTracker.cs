namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using CardTracers.CardStates;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface ICardTracker
    {
        Dictionary<CardSuit, Dictionary<int, CardTracerState>> AllCards { get; }

        void TraceTurn(PlayerTurnContext context);

        void NewGame();
    }
}
