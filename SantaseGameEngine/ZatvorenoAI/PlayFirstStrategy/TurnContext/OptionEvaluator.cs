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
                // message in reporter
            }

            var playrUnder33Points = context.FirstPlayerRoundPoints < 33;

            if (playrUnder33Points)
            {
                // message in reporter
            }

            var opponentAbove50Points = context.SecondPlayerRoundPoints > 50;

            if (opponentAbove50Points)
            {
                // message in reporter
            }

            var strongCards = options.Where(x => x.CanBeTakenCount == 0)
                                     .Select(x => x.Card.GetValue())
                                     .Sum() + context.FirstPlayerRoundPoints >= 60;
            if (strongCards)
            {
                // message in reporter
            }

            return new OptionsEvaluationResponse(options, isEndGame, playrUnder33Points, opponentAbove50Points, strongCards);
        }
    }
}
