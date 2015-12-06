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

    public class FistActionInTrickChoser : IFistActionInTrickChoser
    {
        private readonly ICardTracker cardTracker;

        public FistActionInTrickChoser(ICardTracker cT)
        {
            this.cardTracker = cT;
        }

        public KeyValuePair<bool, Card> CardToPlayAndCloseLogic(OptionsEvaluationResponse evaluatedOption)
        {
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

                if (evaluatedOption.PlayerUnder33 && evaluatedOption.OpponetAbove50)
                {
                    var optimalOption = evaluatedOption.CardStats
                                        .OrderByDescending(x => x.CardWorth)
                                        .First().Card;

                    return new KeyValuePair<bool, Card>(false, optimalOption);
                }

                var minWortPlay = evaluatedOption.CardStats.OrderBy(x => x.CardWorth).First().Card;
                return new KeyValuePair<bool, Card>(false, minWortPlay);
            }
        }
    }
}
