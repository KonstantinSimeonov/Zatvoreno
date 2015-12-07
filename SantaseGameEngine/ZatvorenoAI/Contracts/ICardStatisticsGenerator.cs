namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using PlayFirstStrategy.CardStatistics;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface ICardStatisticsGenerator
    {
        ICollection<CardStatistic> GenerateCardStats(PlayerTurnContext context, ICollection<Card> hand);
    }
}
