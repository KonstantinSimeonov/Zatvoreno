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
        private readonly ICardStatisticsGenerator stastGen;

        public OptionEvaluator(ICardTracker ct, ICardStatisticsGenerator st)
        {
            this.cardTracker = ct;
            this.stastGen = st;
        }

        public OptionsEvaluationResponse EvaluateSituation(PlayerTurnContext context, ICollection<Card> hand)
        {
            var options = this.stastGen.GenerateCardStats(context, hand);

            var isEndGame = context.State.ShouldObserveRules;

            if (isEndGame)
            {
                ZatvorenoAI.report.Add("Is EndGame");
            }

            var playrUnder33Points = context.FirstPlayerRoundPoints < 33;

            if (playrUnder33Points)
            {
                ZatvorenoAI.report.Add("Me Under 33");
            }

            var opponentAbove50Points = context.SecondPlayerRoundPoints > 50;

            if (opponentAbove50Points)
            {
                ZatvorenoAI.report.Add("He Above 50");
            }

            var strongCards = options.Where(x => x.CanBeTakenCount == 0)
                                     .Select(x => x.Card.GetValue())
                                     .Sum() + context.FirstPlayerRoundPoints >= 66;

            if (strongCards)
            {
                ZatvorenoAI.report.Add("Strong Cards");
            }

            return new OptionsEvaluationResponse(options, isEndGame, playrUnder33Points, opponentAbove50Points, strongCards);
        }
    }
}
