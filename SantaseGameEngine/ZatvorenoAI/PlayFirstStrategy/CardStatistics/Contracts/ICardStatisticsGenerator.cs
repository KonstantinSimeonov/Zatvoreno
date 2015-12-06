namespace ZatvorenoAI.PlayFirstStrategy.CardStatistics.Contracts
{
    using System.Collections.Generic;
    using CardStaistic;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface ICardStatisticsGenerator
    {
        ICollection<CardStatistic> GenerateCardStats(PlayerTurnContext context, ICollection<Card> hand);
    }
}
