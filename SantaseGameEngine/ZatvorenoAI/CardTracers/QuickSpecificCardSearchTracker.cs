﻿namespace ZatvorenoAI.CardTracers
{
    using System.Collections.Generic;
    using CardStates;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class QuickSpecificCardSearchTracker : ICardTracker
    {
        private Dictionary<int, CardTracerState> clubs;
        private Dictionary<int, CardTracerState> diamonds;
        private Dictionary<int, CardTracerState> hearts;
        private Dictionary<int, CardTracerState> spades;

        private Card lastTrump = null;

        public QuickSpecificCardSearchTracker()
        {
            // init fields
            this.clubs = new Dictionary<int, CardTracerState>();
            this.diamonds = new Dictionary<int, CardTracerState>();
            this.hearts = new Dictionary<int, CardTracerState>();
            this.spades = new Dictionary<int, CardTracerState>();

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

            //instatntiate take collections
            this.PlayerTookWith = new List<Card>();
            this.OpponentTookWith = new List<Card>();
        }

        public Dictionary<CardSuit, Dictionary<int, CardTracerState>> AllCards { get; private set; }

        public IList<Card> PlayerTookWith { get; private set; }

        public IList<Card> OpponentTookWith { get; private set; }

        public void SetFinalRoundHands(ICollection<Card> my)
        {
            var oppCards = new List<int>();
            var suits = new List<CardSuit>();

            //    Nine = 9,
            //Ten = 10,
            //Jack = 11,
            //Queen = 12,
            //King = 13,
            //Ace = 1,

            var salzichka = new Dictionary<int, CardType>()
            {
                { 11, CardType.Ace },
                { 10, CardType.Ten },
                { 2, CardType.Jack },
                { 3, CardType.Queen },
                { 4, CardType.King },
                { 0, CardType.Nine },
            };

            foreach (var suit in this.AllCards)
            {
                foreach (var card in suit.Value)
                {
                    if (suit.Value[card.Key] == CardTracerState.Unknown)
                    {
                        var novoKarta = new Card(suit.Key, salzichka[card.Key]);
                        if (my.Contains(novoKarta))
                        {
                            continue;
                        }

                        oppCards.Add(card.Key);
                        this.OpponentTookWith.Add(novoKarta);
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

        public void TrickResolution(PlayerTurnContext context)
        {
            if (context.SecondPlayedCard == null)
            {
                return;
            }

            var isPlayerFirst = context.IsFirstPlayerTurn;
            var firstCard = context.FirstPlayedCard;
            var secondCard = context.SecondPlayedCard;
            var trumpCard = context.TrumpCard;

            // handle first turn
            if (this.lastTrump == null)
            {
                this.lastTrump = trumpCard;
            }

            if (trumpCard != this.lastTrump)
            {
                if (!isPlayerFirst)
                {
                    this.AllCards[this.lastTrump.Suit][this.lastTrump.GetValue()] = CardTracerState.InOpponentHand;
                }

                this.AllCards[trumpCard.Suit][trumpCard.GetValue()] = CardTracerState.TrumpIndicator;
            }

            // annouce
            if (!context.IsFirstPlayerTurn)
            {
                if (context.FirstPlayerAnnounce != 0)
                {
                    var type = firstCard.Type == CardType.Queen ? 4 : 3;
                    this.AllCards[firstCard.Suit][type] = CardTracerState.InOpponentHand;
                }
            }

            bool firstCardTake = false;

            if (firstCard.Suit == secondCard.Suit)
            {
                firstCardTake = firstCard.GetValue() > secondCard.GetValue();
            }
            else
            {
                if (secondCard.Suit != trumpCard.Suit)
                {
                    firstCardTake = true;
                }
                else
                {
                    firstCardTake = false;
                }
            }

            // taking
            if (isPlayerFirst && firstCardTake)
            {
                this.AllCards[firstCard.Suit][firstCard.GetValue()] = CardTracerState.TakenByPlayer;
                this.AllCards[secondCard.Suit][secondCard.GetValue()] = CardTracerState.TakenByPlayer;
            }
            else if (!isPlayerFirst && !firstCardTake)
            {
                this.AllCards[firstCard.Suit][firstCard.GetValue()] = CardTracerState.TakenByPlayer;
                this.AllCards[secondCard.Suit][secondCard.GetValue()] = CardTracerState.TakenByPlayer;
            }
            else
            {
                this.AllCards[firstCard.Suit][firstCard.GetValue()] = CardTracerState.TakenByOpponent;
                this.AllCards[secondCard.Suit][secondCard.GetValue()] = CardTracerState.TakenByOpponent;
            }
        }

        public void NewGame(Card cardTrump)
        {
            foreach (var suit in this.AllCards)
            {
                foreach (var card in suit.Value)
                {
                    suit.Value[card.Key] = CardTracerState.Unknown;
                }
            }

            this.OpponentTookWith.Clear();
            this.PlayerTookWith.Clear();
        }

        private void InitSuiteCollection(Dictionary<int, CardTracerState> suit)
        {
            var cardValues = new int[] { 0, 2, 3, 4, 10, 11 };

            foreach (var value in cardValues)
            {
                suit[value] = CardTracerState.Unknown;
            }
        }
    }
}
