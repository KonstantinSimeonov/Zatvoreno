namespace ZatvorenoAI.DecisionMakers
{
    using System.Linq;
    using Contracts;
    using Santase.Logic.Players;

    public class ClosingDecisionMaker : IDecisionMaker
    {
        private const int ShouldCloseTrumpCount = 4;

        public ClosingDecisionMaker()
        {
        }

        public ClosingDecisionMaker(IDecisionMaker next)
        {
            this.Next = next;
        }

        public IDecisionMaker Next { get; set; }

        public PlayerAction Handle(DecisionMakingContext context)
        {
            if (this.IsMatch(context))
            {
                PlayerAction cardToPlay;

                var availableCardsFromHand = context.Validator.GetPossibleCardsToPlay(context.TurnContext, context.MyCards);

                var condition = context.CardChoser.CardToPlayAndCloseLogic(context.TurnContext, availableCardsFromHand);

                if (context.TurnContext.State.CanClose && context.MyCards.Count(c => c.Suit == context.TurnContext.TrumpCard.Suit) > ShouldCloseTrumpCount)
                {
                    return PlayerAction.CloseGame();
                }

                cardToPlay = PlayerAction.PlayCard(condition.Value);

                return cardToPlay;
            }

            return this.Next.Handle(context);
        }

        public bool IsMatch(DecisionMakingContext ctx)
        {
            return true;
        }
    }
}
