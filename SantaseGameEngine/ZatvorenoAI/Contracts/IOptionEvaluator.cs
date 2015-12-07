namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using PlayFirstStrategy.TurnContext.Response;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IOptionEvaluator
    {
        OptionsEvaluationResponse EvaluateSituation(PlayerTurnContext context, ICollection<Card> hand);
    }
}
