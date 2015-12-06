namespace ZatvorenoAI.PlayFirstStrategy.CardStatistics.CardStaistic
{
    using Santase.Logic.Cards;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CardStatistic
    {
        public CardStatistic(int worth, int canTake, int taken3, Card card ) // ve4e izperkvam :D 
        {
            this.CardWorth = worth;
            this.CanTakeCount = canTake;
            this.CanBeTakenCount = taken3;
            this.Card = card;
        }

        public Card Card { get; set; }

        public int CanTakeCount { get; private set; }

        public int CanBeTakenCount { get; private set; }

        public int CardWorth { get; private set; }
    }
}
