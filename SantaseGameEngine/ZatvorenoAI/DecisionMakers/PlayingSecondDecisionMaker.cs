namespace ZatvorenoAI.DecisionMakers
{
    using Contracts;
    using Santase.Logic.Players;

    public class PlayingSecondDecisionMaker : IDecisionMaker
    {
        public PlayingSecondDecisionMaker()
        {
        }

        public PlayingSecondDecisionMaker(IDecisionMaker next)
        {
            this.Next = next;
        }

        public IDecisionMaker Next { get; set; }

        public PlayerAction Handle(DecisionMakingContext context)
        {
            var availableCardsFromHand = context.Validator.GetPossibleCardsToPlay(context.TurnContext, context.MyCards);

            if (this.IsMatch(context))
            {
                var cardByChoser = context.ActionChoser.CardToPlay(context.TurnContext, availableCardsFromHand);

                return PlayerAction.PlayCard(cardByChoser);
            }

            return this.Next.Handle(context);
        }

        public bool IsMatch(DecisionMakingContext ctx)
        {
            return !ctx.TurnContext.IsFirstPlayerTurn;
        }
    }
}
