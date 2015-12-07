﻿namespace ZatvorenoAI.PlayFirstStrategy.CardStatistics.CardStaistic
{
    using Santase.Logic.Cards;

    public class CardStatistic
    {
        public CardStatistic(int worth, int canTake, int taken3, int lengthOfSuit, Card card ) // ve4e izperkvam :D 
        {
            this.CardWorth = worth;
            this.CanTakeCount = canTake;
            this.CanBeTakenCount = taken3;
            this.Card = card;
            this.LengthOfSuit = lengthOfSuit;
        }

        public Card Card { get; set; }

        public int CanTakeCount { get; private set; }

        public int CanBeTakenCount { get; private set; }

        public int CardWorth { get; private set; }

        public int LengthOfSuit { get; set; }
    }
}
