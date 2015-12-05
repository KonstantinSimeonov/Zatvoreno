namespace ZatvorenoAI.Reporters
{
    using Santase.Logic.Players;

    public static class ContextExtensions
    {
        public static string Stringify(this PlayerTurnContext context, bool gosho)
        {
            var b = gosho;
            var line = new object[] 
            {
                context.State.ToString().Split('.')[3],
                "your turn: " + b,
                "your points: " + (b ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints),
                "opponent points: " + (!b ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints),
                "your card: " + (b ? context.FirstPlayedCard : context.SecondPlayedCard),
                "opponent card: " + (!b ? context.FirstPlayedCard : context.SecondPlayedCard),
                "announce: " + context.FirstPlayerAnnounce

            };
            return string.Join(" || ", line);
        }
    }
}