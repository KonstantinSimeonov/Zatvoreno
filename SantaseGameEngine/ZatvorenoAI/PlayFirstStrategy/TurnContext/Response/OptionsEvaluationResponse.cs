namespace ZatvorenoAI.PlayFirstStrategy.TurnContext.Response
{
    using System.Collections.Generic;
    using CardStatistics;

    public class OptionsEvaluationResponse
    {
        public OptionsEvaluationResponse(ICollection<CardStatistic> cS, bool endGame, bool playerUnder33, bool oppAbove50, bool closing)
        {
            this.CardStats = cS;
            this.IsEndGame = endGame;
            this.PlayerUnder33 = playerUnder33;
            this.OpponetAbove50 = oppAbove50;
            this.Closing = closing;
        }

        public ICollection<CardStatistic> CardStats { get; private set; }

        public bool IsEndGame { get; private set; }

        public bool PlayerUnder33 { get; private set; }

        public bool OpponetAbove50 { get; private set; }

        public bool Closing { get; private set; }
    }
}
