namespace ZatvorenoAI.CardTracers
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using CardStates;

    public class QuickSpecificCardSearchTracker
    {
        private Dictionary<CardType, CardTracerState> clubs;
        private Dictionary<CardType, CardTracerState> diamonds;
        private Dictionary<CardType, CardTracerState> hearts;
        private Dictionary<CardType, CardTracerState> spades;

        public QuickSpecificCardSearchTracker()
        {
            this.clubs = new Dictionary<CardType, CardTracerState>();
            this.diamonds = new Dictionary<CardType, CardTracerState>();
            this.hearts = new Dictionary<CardType, CardTracerState>();
            this.spades = new Dictionary<CardType, CardTracerState>();

            this.InitSuiteCollection(this.clubs);
            this.InitSuiteCollection(this.diamonds);
            this.InitSuiteCollection(this.hearts);
            this.InitSuiteCollection(this.spades);

            this.AllCards = new Dictionary<CardSuit, Dictionary<CardType, CardTracerState>>();
            this.AllCards.Add(CardSuit.Club, this.clubs);
            this.AllCards.Add(CardSuit.Diamond, this.diamonds);
            this.AllCards.Add(CardSuit.Heart, this.hearts);
            this.AllCards.Add(CardSuit.Spade, this.spades);
        }

        //Nali ne obi4ame switch
        public Dictionary<CardSuit, Dictionary<CardType, CardTracerState>> AllCards { get; private set; }

        private void InitSuiteCollection(Dictionary<CardType, CardTracerState> suit)
        {
            suit.Add(CardType.Ace, CardTracerState.Unknown);
            suit.Add(CardType.King, CardTracerState.Unknown);
            suit.Add(CardType.Queen, CardTracerState.Unknown);
            suit.Add(CardType.Jack, CardTracerState.Unknown);
            suit.Add(CardType.Ten, CardTracerState.Unknown);
            suit.Add(CardType.Nine, CardTracerState.Unknown);
        }
    }
}
