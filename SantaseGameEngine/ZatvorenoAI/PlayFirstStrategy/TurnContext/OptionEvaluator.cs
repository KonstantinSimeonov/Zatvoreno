namespace ZatvorenoAI.PlayFirstStrategy.TurnContext
{
    using System.Collections.Generic;

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
            return null;
        }
    }
}
