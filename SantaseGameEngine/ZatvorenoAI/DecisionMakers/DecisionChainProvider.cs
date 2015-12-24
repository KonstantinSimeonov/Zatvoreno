namespace ZatvorenoAI.DecisionMakers
{
    using System.Collections.Generic;
    using Contracts;

    public class DecisionChainProvider
    {
        public static IDecisionMaker GetSubmissionDecisionChain()
        {
            var orderedChain = new List<IDecisionMaker>()
            {
                new ChangeTrumpDecisionMaker(),
                // new EndGameDecisionMaker(),
                new AnnounceDecisionMaker(),
                new PlayingSecondDecisionMaker(),
                new ClosingDecisionMaker()
            };

            for (int i = 1; i < orderedChain.Count; i++)
            {
                orderedChain[i - 1].Next = orderedChain[i];
            }

            return orderedChain[0];
        }
    }
}