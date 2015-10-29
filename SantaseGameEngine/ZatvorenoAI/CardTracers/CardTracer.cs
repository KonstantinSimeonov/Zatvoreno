namespace ZatvorenoAI.CardTracers
{
    using System.Collections.Generic;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class CardTracer : ICardTracer
    {
        public Card CurrentTrumpCard { get; set; }

        public ICollection<Card> PlayedCards { get; set; } = new List<Card>();

        public ICollection<Card> OpponentCards { get; set; } = new List<Card>();

        public void Empty()
        {
            this.PlayedCards.Clear();
            this.OpponentCards.Clear();
        }

        public void TraceTurn(PlayerTurnContext context)
        {
            this.PlayedCards.Add(context.FirstPlayedCard);
            this.PlayedCards.Add(context.SecondPlayedCard);

            if (!context.IsFirstPlayerTurn)
            {
                if (context.FirstPlayerAnnounce != 0)
                {
                    var oppCard = context.FirstPlayedCard;
                    var type = oppCard.Type == CardType.Queen ? CardType.King : CardType.Queen;
                    var cardToAdd = new Card(oppCard.Suit, type);
                    this.OpponentCards.Add(cardToAdd);
                }

                if (this.CurrentTrumpCard != context.TrumpCard)
                {
                    this.OpponentCards.Add(this.CurrentTrumpCard);
                    this.CurrentTrumpCard = context.TrumpCard;
                }

                if (this.OpponentCards.Contains(context.FirstPlayedCard))
                {
                    this.OpponentCards.Remove(context.FirstPlayedCard);
                }
            }
        }
    }
}