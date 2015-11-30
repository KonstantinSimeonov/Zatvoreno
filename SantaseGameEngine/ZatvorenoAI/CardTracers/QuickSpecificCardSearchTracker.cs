namespace ZatvorenoAI.CardTracers
{
    using System.Collections.Generic;
    using System.Linq;
    using CardStates;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class QuickSpecificCardSearchTracker
    {
        private Dictionary<CardType, CardTracerState> clubs;
        private Dictionary<CardType, CardTracerState> diamonds;
        private Dictionary<CardType, CardTracerState> hearts;
        private Dictionary<CardType, CardTracerState> spades;

        private Card lastTrump = null;

        public QuickSpecificCardSearchTracker()
        {
            // init fields
            this.clubs = new Dictionary<CardType, CardTracerState>();
            this.diamonds = new Dictionary<CardType, CardTracerState>();
            this.hearts = new Dictionary<CardType, CardTracerState>();
            this.spades = new Dictionary<CardType, CardTracerState>();

            // fill suits
            this.InitSuiteCollection(this.clubs);
            this.InitSuiteCollection(this.diamonds);
            this.InitSuiteCollection(this.hearts);
            this.InitSuiteCollection(this.spades);

            // add to dictionary for easy access
            this.AllCards = new Dictionary<CardSuit, Dictionary<CardType, CardTracerState>>();
            this.AllCards.Add(CardSuit.Club, this.clubs);
            this.AllCards.Add(CardSuit.Diamond, this.diamonds);
            this.AllCards.Add(CardSuit.Heart, this.hearts);
            this.AllCards.Add(CardSuit.Spade, this.spades);
        }

        // Nali ne obi4ame switch
        public Dictionary<CardSuit, Dictionary<CardType, CardTracerState>> AllCards { get; private set; }

        // Needs to be optimized
        public void TraceTurn(PlayerTurnContext context, ICollection<Card> playerHand)
        {
            var isPlayerFirst = context.IsFirstPlayerTurn;
            var hand = playerHand;
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
                    this.AllCards[this.lastTrump.Suit][this.lastTrump.Type] = CardTracerState.InOpponentHand;
                }

                this.AllCards[trumpCard.Suit][trumpCard.Type] = CardTracerState.TrumpIndicator;
            }

            // annouce
            if (!context.IsFirstPlayerTurn)
            {
                if (context.FirstPlayerAnnounce != 0)
                {
                    var type = firstCard.Type == CardType.Queen ? CardType.King : CardType.Queen;
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
                this.AllCards[firstCard.Suit][firstCard.Type] = CardTracerState.TakenByPlayer;
                this.AllCards[secondCard.Suit][secondCard.Type] = CardTracerState.TakenByPlayer;
            }
            else if (!isPlayerFirst && !firstCardTake)
            {
                this.AllCards[firstCard.Suit][firstCard.Type] = CardTracerState.TakenByPlayer;
                this.AllCards[secondCard.Suit][secondCard.Type] = CardTracerState.TakenByPlayer;
            }
            else
            {
                this.AllCards[firstCard.Suit][firstCard.Type] = CardTracerState.TakenByOpponent;
                this.AllCards[secondCard.Suit][secondCard.Type] = CardTracerState.TakenByOpponent;
            }
        }

        public void NewGame()
        {
            foreach (var suit in this.AllCards)
            {
                foreach (var card in suit.Value)
                {
                    suit.Value[card.Key] = CardTracerState.Unknown;
                }
            }
        }

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
