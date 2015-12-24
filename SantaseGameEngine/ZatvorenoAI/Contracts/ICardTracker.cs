namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using CardTracers.CardStates;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface ICardTracker
    {
        IList<Card> OpponentTookWith { get; }

        Dictionary<CardSuit, Dictionary<int, CardTracerState>> AllCards { get; }

        void TrickResolution(PlayerTurnContext context);

        void NewGame(Card trumpCard);

        void SetFinalRoundHands(ICollection<Card> my);
    }
}
