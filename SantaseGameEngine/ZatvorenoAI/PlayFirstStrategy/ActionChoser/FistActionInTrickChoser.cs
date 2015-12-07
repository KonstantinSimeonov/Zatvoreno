namespace ZatvorenoAI.PlayFirstStrategy.ActionChoser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts;
    using global::ZatvorenoAI.Contracts;
    using Santase.Logic.Cards;
    using TurnContext.Response;
    using Santase.Logic.Players;
    using TurnContext.Contracts;

    public class FistActionInTrickChoser : IFistActionInTrickChoser
    {
        private readonly ICardTracker cardTracker;
        private readonly IOptionEvaluator optionEval;

        public FistActionInTrickChoser(ICardTracker cT, IOptionEvaluator oE)
        {
            this.cardTracker = cT;
            this.optionEval = oE;
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
                if (evaluatedOption.Closing) // ne mislq 4e ima nujda ot proverka ako sme vlezli trqbva da ima pone 4 sigurni ryce
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
                                       .OrderByDescending(x => x.CardWorth)
                                       .ToList();

                    if(optimalOption.Count == 0)
                    {
                        var wayToPlay = evaluatedOption.CardStats.OrderBy(x => x.CardWorth).First().Card;
                        return new KeyValuePair<bool, Card>(false, wayToPlay);
                    }

                    var card = optimalOption.First().Card;
                    return new KeyValuePair<bool, Card>(false, card);
                }

                var minWortPlay = evaluatedOption.CardStats.OrderBy(x => x.CardWorth).First().Card;
                return new KeyValuePair<bool, Card>(false, minWortPlay);
            }
        }
    }
}
