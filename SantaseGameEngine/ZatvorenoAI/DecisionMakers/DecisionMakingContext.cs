namespace ZatvorenoAI.DecisionMakers
{
    using System.Collections.Generic;
    using Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.PlayerActionValidate;
    using Santase.Logic.Players;

    public class DecisionMakingContext
    {
        public IPlayerActionValidator Validator { get; set; }

        public ICollection<Card> MyCards { get; set; }

        public PlayerTurnContext TurnContext { get; set; }

        public ICardTracker Tracker { get; set; }

        public IFistActionInTrickChoser CardChoser { get; set; }

        public IChoseAction ActionChoser { get; set; }
    }
}
