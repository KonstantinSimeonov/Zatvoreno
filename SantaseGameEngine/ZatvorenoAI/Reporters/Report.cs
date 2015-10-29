namespace ZatvorenoAI.Reporters
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Santase.Logic;

    public class Report : IReport
    {
        private const string ReportFormat = "Games won: {1}{0}Trumps changes: {2}{0}Trumped aces and tens: {3}{0}Announces:{0}  - Forty: {4}{0}  - Twenty: {5}{0}";

        public Report()
        {
            this.Wins = 0;
            this.TrumpsChanged = 0;
            this.TrumpedHighCards = 0;
            this.AnnounceStatistics = new Dictionary<Announce, int>()
            {
                { Announce.Forty, 0 },
                { Announce.Twenty, 0 }
            };
        }

        public int Wins { get; set; }

        public int TrumpsChanged { get; set; }

        public int TrumpedHighCards { get; set; }

        public IDictionary<Announce, int> AnnounceStatistics { get; set; }

        public override string ToString()
        {
            // "Games won: {1}{0}Trumps changes: {2}{0} Trumped aces and tens: {3}{0} Announces:{0}  - Forty: {4}{0}  - Twenty: {5}{0}";
            return string.Format(
                                 ReportFormat,
                                 Environment.NewLine,
                                 this.Wins,
                                 this.TrumpsChanged,
                                 this.TrumpedHighCards,
                                 this.AnnounceStatistics[Announce.Forty],
                                 this.AnnounceStatistics[Announce.Twenty]);
        }
    }
}