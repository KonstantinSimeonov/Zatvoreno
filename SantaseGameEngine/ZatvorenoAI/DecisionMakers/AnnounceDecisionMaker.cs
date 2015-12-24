namespace ZatvorenoAI.DecisionMakers
{
    using System.Linq;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class AnnounceDecisionMaker : IDecisionMaker
    {
        private PlayerAction announce;

        public AnnounceDecisionMaker()
        {
        }

        public AnnounceDecisionMaker(IDecisionMaker next)
        {
            this.Next = next;
        }

        public IDecisionMaker Next { get; set; }

        public PlayerAction Handle(DecisionMakingContext context)
        {
            this.announce = this.GetAnnounces(context);
            if (this.IsMatch(context))
            {
                return this.announce;
            }

            return this.Next.Handle(context);
        }

        public bool IsMatch(DecisionMakingContext context)
        {
            return this.announce != null;
        }

        private PlayerAction GetAnnounces(DecisionMakingContext context)
        {
            var availableCardsFromHand = context.Validator.GetPossibleCardsToPlay(context.TurnContext, context.MyCards);
            if (context.TurnContext.IsFirstPlayerTurn &&
                context.TurnContext.CardsLeftInDeck != 12)
            {
                var announceCards = availableCardsFromHand
                                        .Where(c => c.Type == CardType.King || c.Type == CardType.Queen)
                                        .GroupBy(c => c.Suit)
                                        .Where(g => g.Count() > 1)
                                        .ToList();

                var fortyAnnounce = announceCards
                    .Where(g => g.FirstOrDefault().Suit == context.TurnContext.TrumpCard.Suit)
                    .Select(suit => suit.First(card => card.Type == CardType.Queen))
                    .FirstOrDefault();

                if (fortyAnnounce != null)
                {
                    // SummaryReport.AnnounceStatistics[Santase.Logic.Announce.Forty]++;
                    return PlayerAction.PlayCard(fortyAnnounce);
                }
                else if (announceCards.Count > 0)
                {
                    // SummaryReport.AnnounceStatistics[Santase.Logic.Announce.Twenty]++;
                    return PlayerAction.PlayCard(announceCards.FirstOrDefault().FirstOrDefault(x => x.Type == CardType.Queen));
                }
            }

            return null;
        }
    }
}
