namespace ZatvorenoAI.DecisionMakers
{
    using Contracts;
    using Santase.Logic.Players;

    public class ChangeTrumpDecisionMaker : IDecisionMaker
    {
        public ChangeTrumpDecisionMaker()
        {
        }

        public ChangeTrumpDecisionMaker(IDecisionMaker next)
        {
            this.Next = next;
        }

        public IDecisionMaker Next { get; set; }

        public PlayerAction Handle(DecisionMakingContext context)
        {
            if (context.Validator.IsValid(PlayerAction.ChangeTrump(), context.TurnContext, context.MyCards))
            {
                return PlayerAction.ChangeTrump();
            }

            return this.Next.Handle(context);
        }

        public bool IsMatch(DecisionMakingContext context)
        {
            return context.TurnContext.CardsLeftInDeck > 2 && context.TurnContext.IsFirstPlayerTurn;
        }
    }
}
