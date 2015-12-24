namespace ZatvorenoAI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CardTracers;
    using Contracts;
    using DecisionMakers;
    using PlayFirstStrategy.ActionChoser;
    using PlayFirstStrategy.CardStatistics;
    using PlayFirstStrategy.TurnContext;
    using Reporters;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using TakeStrategy.Agents.ChoseTake;
    using TakeStrategy.Agents.NeedToTake;
    using TakeStrategy.Agents.PossibleTakes;

    public class ZatvorenoAI : BasePlayer
    {
        private const string AIName = "Zatvoreno";
        private const int ShouldCloseTrumpCount = 4;

        private static readonly ISummaryReport SummaryReport;
        private static readonly ICardTracker Tracker;
        private static readonly IShouldTake TrickDecisionMakerWhenSecond;
        private static readonly IPossibleActions PossibleActionGenerator;
        private static readonly IChoseAction ActionChoser;
        private static readonly ICardStatisticsGenerator CardStatistics;
        private static readonly IOptionEvaluator OptionEval;
        private static readonly IFistActionInTrickChoser CardChoser;

        private IDecisionMaker decisionMaker;

        private bool myTurn;
        private int currentGameId;

        static ZatvorenoAI()
        {
            SummaryReport = new SummaryReport();
            Tracker = new QuickSpecificCardSearchTracker();
            TrickDecisionMakerWhenSecond = new ShouldTake(Tracker);
            PossibleActionGenerator = new PossibleActions(Tracker);
            ActionChoser = new ChoseAction(PossibleActionGenerator, TrickDecisionMakerWhenSecond);
            CardStatistics = new CardStatisticsGenerator(Tracker);
            OptionEval = new OptionEvaluator(Tracker, CardStatistics);
            CardChoser = new FirstActionInTrickChoser(Tracker, OptionEval);
        }

        public ZatvorenoAI()
        {
            Report = true ? (IReport)new DetailedReport() : new EmptyReport();

            this.decisionMaker = DecisionChainProvider.GetSubmissionDecisionChain();
        }

        public static IReport Report { get; private set; }

        public override string Name
        {
            get
            {
                return AIName;
            }
        }

        public static string GetReports()
        {
            return SummaryReport.ToString();
        }

        public override void StartRound(ICollection<Card> cards, Card trumpCard, int myTotalPoints, int opponentTotalPoints)
        {
            Report.Add("Trump for current game is: " + trumpCard.ToString());
            base.StartRound(cards, trumpCard, myTotalPoints, opponentTotalPoints);
        }

        public override void EndTurn(PlayerTurnContext context)
        {
            Tracker.TrickResolution(context);

            Report.Add(context.Stringify(this.myTurn) + " --- current hand: " + string.Join(", ", this.Cards.Select(x => x.ToString())));
            base.EndTurn(context);
        }

        public override void EndRound()
        {
            Report.Add(" ------ END ROUND ------");
            base.EndRound();
        }

        public override PlayerAction GetTurn(PlayerTurnContext context)
        {
            return false ? this.GetTurnOld(context) : this.GetTurnChain(context);
        }

        public override void EndGame(bool amIWinner)
        {
            if (amIWinner)
            {
                ++SummaryReport.Wins;
            }

            this.currentGameId++;

            base.EndGame(amIWinner);
        }

        private PlayerAction GetTurnChain(PlayerTurnContext context)
        {
            var decisionContext = new DecisionMakingContext()
            {
                ActionChoser = ActionChoser,
                CardChoser = CardChoser,
                MyCards = this.Cards,
                Tracker = Tracker,
                TurnContext = context,
                Validator = this.PlayerActionValidator
            };

            // TODO: this of something more elegant
            var action = decisionMaker.Handle(decisionContext);

            switch (action.Type)
            {
                case PlayerActionType.PlayCard:
                    return this.PlayCard(action.Card);
                case PlayerActionType.ChangeTrump:
                    return this.ChangeTrump(context.TrumpCard);
                case PlayerActionType.CloseGame:
                    return this.CloseGame();
                default:
                    throw new InvalidOperationException("Invalid type of player action");
            }
        }

        // TODO: delete
        private PlayerAction GetTurnOld(PlayerTurnContext context)
        {
            this.myTurn = context.IsFirstPlayerTurn;

            var myPoints = GetMyPoints(context);

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
                SummaryReport.TrumpsChanged++;
                return this.ChangeTrump(context.TrumpCard);
            }

            var availableCardsFromHand = this.PlayerActionValidator.GetPossibleCardsToPlay(context, this.Cards);

            // announce logic
            if (context.IsFirstPlayerTurn &&
                context.CardsLeftInDeck != 12)
            {
                var announceCards = availableCardsFromHand
                                        .Where(c => c.Type == CardType.King || c.Type == CardType.Queen)
                                        .GroupBy(c => c.Suit)
                                        .Where(g => g.Count() > 1)
                                        .ToList();

                var fortyAnnounce = announceCards
                    .Where(g => g.FirstOrDefault().Suit == context.TrumpCard.Suit)
                    .Select(suit => suit.First(card => card.Type == CardType.Queen))
                    .FirstOrDefault();

                if (fortyAnnounce != null)
                {
                    SummaryReport.AnnounceStatistics[Santase.Logic.Announce.Forty]++;
                    return this.PlayCard(fortyAnnounce);
                }
                else if (announceCards.Count > 0)
                {
                    SummaryReport.AnnounceStatistics[Santase.Logic.Announce.Twenty]++;
                    return this.PlayCard(announceCards.FirstOrDefault().FirstOrDefault(x => x.Type == CardType.Queen));
                }
            }

            if (!context.IsFirstPlayerTurn)
            {
                var cardByChoser = ActionChoser.CardToPlay(context, availableCardsFromHand);

                return this.PlayCard(cardByChoser);
            }

            PlayerAction cardToPlay;

            var condition = CardChoser.CardToPlayAndCloseLogic(context, availableCardsFromHand);

            if (context.State.CanClose && this.Cards.Count(c => c.Suit == context.TrumpCard.Suit) > ShouldCloseTrumpCount)
            {
                return this.CloseGame();
            }

            cardToPlay = this.PlayCard(condition.Value);

            return cardToPlay;
        }

        private static int GetMyPoints(PlayerTurnContext context)
        {
            return context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;
        }
    }
}