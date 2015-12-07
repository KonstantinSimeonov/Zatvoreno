namespace ZatvorenoAI.PlayFirstStrategy.TurnContext
{
    using System.Collections.Generic;
    using System.Linq;

    using CardStatistics.Contracts;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using Response;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class OptionEvaluator : IOptionEvaluator
    {
        private readonly ICardTracker cardTracker;
        private readonly ICardStatisticsGenerator statsGen;

        public OptionEvaluator(ICardTracker ct, ICardStatisticsGenerator st)
        {
            this.cardTracker = ct;
            this.statsGen = st;
        }

        public OptionsEvaluationResponse EvaluateSituation(PlayerTurnContext context, ICollection<Card> hand)
        {
            var options = this.statsGen.GenerateCardStats(context, hand);

            var isEndGame = context.State.ShouldObserveRules;

            if (isEndGame)
            {
                ZatvorenoAI.Report.Add("Is EndGame");
            }

            var playerUnder33Points = context.FirstPlayerRoundPoints < 33;

            if (playerUnder33Points)
            {
                ZatvorenoAI.Report.Add("Me Under 33");
            }

            var opponentAbove50Points = context.SecondPlayerRoundPoints > 50;

            if (opponentAbove50Points)
            {
                ZatvorenoAI.Report.Add("He Above 50");
            }

            var strongCards = options.Where(x => x.CanBeTakenCount == 0)
                                     .Select(x => x.Card.GetValue())
                                     .Sum() + context.FirstPlayerRoundPoints >= 66;

            if (strongCards)
            {
                ZatvorenoAI.Report.Add("Strong Cards");
            }

            return new OptionsEvaluationResponse(options, isEndGame, playerUnder33Points, opponentAbove50Points, strongCards);
        }
    }
}