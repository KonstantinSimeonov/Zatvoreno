namespace ZatvorenoAI.CardTracers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CardStates;
    using Contracts;
    using Santase.Logic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class AdequateCardTracker : ICardTracker
    {

        private Dictionary<int, CardTracerState> clubs;
        private Dictionary<int, CardTracerState> diamonds;
        private Dictionary<int, CardTracerState> hearts;
        private Dictionary<int, CardTracerState> spades;

        public Card LastTrump { get; private set; }

        public AdequateCardTracker()
        {
            // init fields
            this.clubs = new Dictionary<int, CardTracerState>();
            this.diamonds = new Dictionary<int, CardTracerState>();
            this.hearts = new Dictionary<int, CardTracerState>();
            this.spades = new Dictionary<int, CardTracerState>();

            this.NewGame(null);
        }

        public Dictionary<CardSuit, Dictionary<int, CardTracerState>> AllCards { get; private set; }

        public void AddCard(Card card)
        {
            this.AllCards[card.Suit][card.GetValue()] = CardTracerState.InHand;
        }

        public void TrickResolution(PlayerTurnContext context)
        {
            //this.AllCards[context.FirstPlayedCard.Suit][context.FirstPlayedCard.GetValue()] = CardTracerState.Played;
            //if (context.SecondPlayedCard == null)
            //{
            //    return;
            //}

            //this.AllCards[context.SecondPlayedCard.Suit][context.SecondPlayedCard.GetValue()] = CardTracerState.Played;

            if (this.LastTrump == null)
            {
                this.LastTrump = context.TrumpCard;
            }
            else if (context.TrumpCard != this.LastTrump)
            {
                if (context.IsFirstPlayerTurn)
                {
                    this.AllCards[this.LastTrump.Suit][this.LastTrump.GetValue()] = CardTracerState.InHand;
                }
                else
                {
                    this.AllCards[this.LastTrump.Suit][this.LastTrump.GetValue()] = CardTracerState.InOpponentHand;
                }
            }

            if (!context.IsFirstPlayerTurn && context.FirstPlayerAnnounce != Announce.None)
            {
                var playedCard = context.FirstPlayedCard.GetValue() == 3 ? 4 : 3;

                this.AllCards[context.FirstPlayedCard.Suit][playedCard] = CardTracerState.InOpponentHand;
            }
        }

        public void SetFinalRoundHands()
        {
            var oppCards = new List<int>();
            var suits = new List<CardSuit>();

            foreach (var suit in this.AllCards)
            {
                foreach (var card in suit.Value)
                {
                    if (suit.Value[card.Key] == CardTracerState.Unknown)
                    {
                        oppCards.Add(card.Key);
                        suits.Add(suit.Key);
                    }
                }
            }

            for (int i = 0; i < oppCards.Count; i++)
            {
                var suit = suits[i];
                var value = oppCards[i];

                this.AllCards[suit][value] = CardTracerState.InOpponentHand;
            }
        }

        public void NewGame(Card trumpCard)
        {
            this.LastTrump = trumpCard;

            // fill suits
            this.InitSuiteCollection(this.clubs);
            this.InitSuiteCollection(this.diamonds);
            this.InitSuiteCollection(this.hearts);
            this.InitSuiteCollection(this.spades);

            // add to dictionary for easy access
            this.AllCards = new Dictionary<CardSuit, Dictionary<int, CardTracerState>>();
            this.AllCards.Add(CardSuit.Club, this.clubs);
            this.AllCards.Add(CardSuit.Diamond, this.diamonds);
            this.AllCards.Add(CardSuit.Heart, this.hearts);
            this.AllCards.Add(CardSuit.Spade, this.spades);
        }

        private void InitSuiteCollection(Dictionary<int, CardTracerState> suit)
        {
            suit[11] = CardTracerState.Unknown;
            suit[4] = CardTracerState.Unknown;
            suit[3] = CardTracerState.Unknown;
            suit[2] = CardTracerState.Unknown;
            suit[10] = CardTracerState.Unknown;
            suit[0] = CardTracerState.Unknown;
        }
    }
}