namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using DecisionMakers;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public interface IDecisionMaker
    {
        IDecisionMaker Next { get; set; }

        bool IsMatch(DecisionMakingContext ctx);

        PlayerAction Handle(DecisionMakingContext context);
    }
}