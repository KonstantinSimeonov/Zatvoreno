namespace ZatvorenoAI.TakeStrategy.Agents.PossibleTakes.Action
{
    using System.Linq;
    using CardTracers.CardStates;
    using global::ZatvorenoAI.Contracts;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;

    public class GameAction
    {
        public GameAction(Card opponent, Card player, PlayerTurnContext context, ICardTracker cardTracker)
        {
            this.OpponetCard = opponent;
            this.PlayerCard = player;
            this.PlayerTakes = this.CheckIfPlayerTakes(opponent, player, context);
            this.HandValue = opponent.GetValue() + player.GetValue();
            this.CardMaxWorth = this.CardMaxWorthCalculator(player, cardTracker);
            this.OpponentPoints = context.FirstPlayerRoundPoints;
            this.PlayerPoints = context.SecondPlayerRoundPoints;
        }

        public Card OpponetCard { get; private set; }

        public Card PlayerCard { get; private set; }

        public int OpponentPoints { get; set; }

        public int PlayerPoints { get; set; }

        public bool PlayerTakes { get; private set; }

        public int HandValue { get; private set; }

        public int CardMaxWorth { get; private set; }

        private bool CheckIfPlayerTakes(Card opponent, Card player, PlayerTurnContext context)
        {
            var trumpSuit = context.TrumpCard.Suit;

            if (opponent.Suit == player.Suit)
            {
                return player.GetValue() > opponent.GetValue();
            }

            if (player.Suit != trumpSuit)
            {
                return false;
            }

            return true;
        }

        private int CardMaxWorthCalculator(Card player, ICardTracker cardTracker)
        {
            var worth = 0;

            worth += player.GetValue();

            var maxCardCanTake = cardTracker.AllCards[player.Suit].Where(x => x.Key > player.GetValue()
                && (x.Value == CardTracerState.Unknown || x.Value == CardTracerState.InOpponentHand)).ToList();

            if (maxCardCanTake.Count == 0)
            {
                return worth;
            }

            worth += maxCardCanTake.Max(x => x.Key);

            return worth;
        }
    }
}
