namespace ZatvorenoAI.PlayFirstStrategy.ActionChoser
{
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class FirstActionInTrickChoser : IFistActionInTrickChoser
    {
        private readonly ICardTracker cardTracker;
        private readonly IOptionEvaluator optionEval;

        public FirstActionInTrickChoser(ICardTracker tracker, IOptionEvaluator evaluator)
        {
            this.cardTracker = tracker;
            this.optionEval = evaluator;
        }

        public KeyValuePair<bool, Card> CardToPlayAndCloseLogic(PlayerTurnContext context, ICollection<Card> hand)
        {
            var evaluatedOption = this.optionEval.EvaluateSituation(context, hand);

            if (evaluatedOption.IsEndGame)
            {
                var optimalOptions = evaluatedOption.CardStats.Where(x => x.CanBeTakenCount == 0)
                    .OrderByDescending(x => x.CardWorth);

                if (optimalOptions.Count() == 0)
                {
                    var cardToRetun = evaluatedOption.CardStats.OrderBy(x => x.CardWorth).First().Card;
                    return new KeyValuePair<bool, Card>(false, cardToRetun);
                }

                return new KeyValuePair<bool, Card>(false, optimalOptions.First().Card);
            }
            else
            {
                if (evaluatedOption.Closing)
                {
                    var optimalOptions = evaluatedOption.CardStats.Where(x => x.CanBeTakenCount == 0)
                   .OrderByDescending(x => x.CardWorth);

                    return new KeyValuePair<bool, Card>(true, optimalOptions.First().Card);
                }

                if (evaluatedOption.OpponetAbove50)
                {
                    var optimalOption = evaluatedOption.CardStats
                                        .OrderByDescending(x => x.CardWorth)
                                        .First().Card;

                    return new KeyValuePair<bool, Card>(false, optimalOption);
                }

                if (evaluatedOption.PlayerUnder33 && context.CardsLeftInDeck > 6)
                {
                    var optimalOption = evaluatedOption.CardStats
                                       .Where(x => x.CanBeTakenCount == 0)
                                       .Where(x => x.Card.Suit != context.TrumpCard.Suit)
                                       .OrderByDescending(x => x.CardWorth)
                                       .ThenByDescending(x => x.LengthOfSuit)
                                       .ToList();

                    if (optimalOption.Count == 0)
                    {
                        var wayToPlay = evaluatedOption.CardStats
                            .OrderBy(x => x.CardWorth)
                            .First()
                            .Card;
                        return new KeyValuePair<bool, Card>(false, wayToPlay);
                    }

                    var card = optimalOption.First().Card;
                    return new KeyValuePair<bool, Card>(false, card);
                }

                var minWorthPlay = evaluatedOption.CardStats.OrderBy(x => x.CardWorth).ThenByDescending(x => x.LengthOfSuit).First().Card;
                return new KeyValuePair<bool, Card>(false, minWorthPlay);
            }
        }
    }
}
