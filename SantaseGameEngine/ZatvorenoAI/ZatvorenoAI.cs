namespace ZatvorenoAI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CardEvaluator;
    using CardTracers;
    using Contracts;
    using PlayFirstStrategy.ActionChoser;
    using PlayFirstStrategy.ActionChoser.Contracts;
    using PlayFirstStrategy.CardStatistics;
    using PlayFirstStrategy.CardStatistics.Contracts;
    using PlayFirstStrategy.TurnContext;
    using PlayFirstStrategy.TurnContext.Contracts;
    using Reporters;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using TakeStrategy.Agents.ChoseTake;
    using TakeStrategy.Agents.ChoseTake.Contracts;
    using TakeStrategy.Agents.NeedToTake;
    using TakeStrategy.Agents.NeedToTake.Contracts;
    using TakeStrategy.Agents.PossibleTakes;
    using TakeStrategy.Agents.PossibleTakes.Contracts;

    public class ZatvorenoAI : BasePlayer
    {
        // Utils
        public static IReport report;

        private const string AIName = "Zatvoreno";

        private static readonly ISummaryReport SummaryReport = new SummaryReport();

        private static readonly ICardTracker Tracker = new QuickSpecificCardSearchTracker();

        private static readonly ICardEval Evaluator2 = new CardEvaluatorFirstPlayer(Tracker);

        private static readonly IShouldTake TrickDecisionMakerWhenSecond = new ShouldTake(Tracker);

        private static readonly IPossibleActions PossibleActionGenerator = new PossibleActions(Tracker);

        private static readonly IChoseAction ActionChoser = new ChoseAction(PossibleActionGenerator, TrickDecisionMakerWhenSecond);

        private static readonly ICardStatisticsGenerator CardStatistics = new CardStatisticsGenerator(Tracker);

        private static readonly IOptionEvaluator OptionEval = new OptionEvaluator(Tracker, CardStatistics);

        private static readonly IFistActionInTrickChoser CardChoser = new FirstActionInTrickChoser(Tracker, OptionEval);

        public ZatvorenoAI()
        {
            report = true ?
                            (IReport)new DetailedReport() :
                            (IReport)new EmptyReport();
        }

        // Logic
        private bool myTurn = false;

        private int currentGameId = 0;

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
            report.Add("Trump for current game is: " + trumpCard.ToString());
            base.StartRound(cards, trumpCard, myTotalPoints, opponentTotalPoints);
        }

        public override void EndTurn(PlayerTurnContext context)
        {
            Tracker.TrickResolution(context);

            report.Add(context.Stringify(this.myTurn) + " --- current hand: " + string.Join(", ", this.Cards.Select(x => x.ToString())));
            base.EndTurn(context);
        }

        public override void EndRound()
        {
            report.Add(" ------ END ROUND ------");
            base.EndRound();
        }

        public override PlayerAction GetTurn(PlayerTurnContext context)
        {
            this.myTurn = context.IsFirstPlayerTurn;

            var myPoints = GetMyPoints(context);

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
                SummaryReport.TrumpsChanged++;
                return this.ChangeTrump(context.TrumpCard);
            }

            // TODO: closing logic

            var availableCardsFromHand = this.PlayerActionValidator.GetPossibleCardsToPlay(context, this.Cards);

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

            cardToPlay = this.PlayCard(
                                CardChoser.CardToPlayAndCloseLogic(context, availableCardsFromHand)
                                .Value
                                );

            return cardToPlay;
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

        private static int GetMyPoints(PlayerTurnContext context)
        {
            return context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;
        }
    }
}
