namespace ZatvorenoAI.PlayFirstStrategy.ActionChoser.Contracts
{
    using System.Collections.Generic;

    using Santase.Logic.Cards;
    using TurnContext.Response;

    public interface IFistActionInTrickChoser
    {
        KeyValuePair<bool, Card> CardToPlayAndCloseLogic(OptionsEvaluationResponse evaluatedOption);
    }
}
