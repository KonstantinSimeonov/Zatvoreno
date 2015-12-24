namespace ZatvorenoAI.DecisionMakers
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using MCST;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class EndGameDecisionMaker : IDecisionMaker
    {
        private Node root;

        public EndGameDecisionMaker()
        {
        }

        public EndGameDecisionMaker(IDecisionMaker next)
        {
            this.Next = next;
        }

        public IDecisionMaker Next { get; set; }

        public bool IsMatch(DecisionMakingContext ctx)
        {
            return ctx.TurnContext.CardsLeftInDeck == 0;
        }

        public PlayerAction Handle(DecisionMakingContext context)
        {
            if (this.IsMatch(context))
            {
                return this.EndGameTurn(context.TurnContext, context.Tracker, context.MyCards);
            }

            return this.Next.Handle(context);
        }

        private PlayerAction EndGameTurn(PlayerTurnContext context, ICardTracker tracker, ICollection<Card> myHand)
        {
            this.root = new Node(null, null, context.IsFirstPlayerTurn);

            if (context.IsFirstPlayerTurn && this.root.Children.Count == 0)
            {
                tracker.SetFinalRoundHands(myHand);
                var myCards = myHand.ToList();

                EndgameAnalyzer.Compute(this.root, null, myCards, tracker.OpponentTookWith, context.SecondPlayerRoundPoints, context.FirstPlayerRoundPoints);
            }
            else if (this.root.Children.Count == 0)
            {
                tracker.SetFinalRoundHands(myHand);
                var myCards = myHand.ToList();

                while (tracker.OpponentTookWith.Count < 6)
                {
                    tracker.OpponentTookWith.Add(null);
                }

                EndgameAnalyzer.Compute(this.root, context.FirstPlayedCard, myCards, tracker.OpponentTookWith, context.FirstPlayerRoundPoints, context.SecondPlayerRoundPoints);
            }

            if (!context.IsFirstPlayerTurn)
            {
                this.root = this.root.Children.First(x => x.Card == context.FirstPlayedCard);
            }

            var card = this.root.Children.OrderByDescending(x => x.Wins / (decimal)x.Total).First().Card;

            return PlayerAction.PlayCard(card);
        }
    }
}