namespace ZatvorenoAI.TakeStrategy.Agents.PossibleTakes
{
    using System.Linq;
    using System.Collections.Generic;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class PossibleTakes : IPossibleTakes
    {
        private readonly ICardTracker cardTracker;

        public PossibleTakes(ICardTracker cT)
        {
            this.cardTracker = cT;
        }

        public ICollection<Card> GetPossibleTakeCard(PlayerTurnContext context, ICollection<Card> hand)
        {
            var cardsThatCanTake = new List<Card>();

            var playedCard = context.FirstPlayedCard;

            var takeCards = hand.Where(x => x.Suit == playedCard.Suit && x.GetValue() > playedCard.GetValue()).ToList();

            if (playedCard.Suit != context.TrumpCard.Suit)
            {
                var trumps = hand.Where(x => x.Suit == context.TrumpCard.Suit);

                foreach (var item in trumps)
                {
                    takeCards.Add(item);
                }
            }

            return cardsThatCanTake;
        }
    }
}
