namespace ZatvorenoAI.Contracts
{
    using System.Collections.Generic;
    using Santase.Logic;

    public interface ISummaryReport
    {
        int Wins { get; set; }

        int TrumpsChanged { get; set; }

        IDictionary<Announce, int> AnnounceStatistics { get; set; }
    }
}