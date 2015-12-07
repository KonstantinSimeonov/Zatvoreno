namespace ZatvorenoAI.TakeStrategy.Agents.ShouldTake.Response
{
    public class ShouldTakeResponse
    {
        public const int ShouldTakeCases = 6;

        public ShouldTakeResponse(bool[] parameters)
        {
            this.Annouce = parameters[0];
            this.OpponentWins = parameters[1];
            this.PlayerWins = parameters[2];
            this.OpponentHighCard = parameters[3];
            this.Take = parameters[4];
            this.GetLastTrump = parameters[5];
        }

        public bool Annouce { get; private set; }

        public bool OpponentWins { get; private set; }

        public bool PlayerWins { get; private set; }

        public bool OpponentHighCard { get; private set; }

        public bool Take { get; private set; }

        public bool GetLastTrump { get; private set; }
    }
}
