namespace ZatvorenoAI.Reporters
{
    using Santase.Logic.Players;

    public static class ContextExtensions
    {
        private const string Separator = " || ";

        private const int StateIndex = 3;

        public static string Stringify(this PlayerTurnContext context, bool amFirst)
        {
            var line = new string[]
            {
                context.State.ToString().Split('.')[StateIndex],
                "your turn: " + amFirst,
                "your points: " + (amFirst ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints),
                "opponent points: " + (!amFirst ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints),
                "your card: " + (amFirst ? context.FirstPlayedCard : context.SecondPlayedCard),
                "opponent card: " + (!amFirst ? context.FirstPlayedCard : context.SecondPlayedCard),
                "announce: " + context.FirstPlayerAnnounce
            };

            return string.Join(Separator, line);
        }
    }
}