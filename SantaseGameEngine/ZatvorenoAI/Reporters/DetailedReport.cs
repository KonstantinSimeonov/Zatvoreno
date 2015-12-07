namespace ZatvorenoAI.Reporters
{
    using System.Text;

    public class DetailedReport : IReport
    {
        public DetailedReport()
        {
            this.Content = new StringBuilder();
        }

        public StringBuilder Content { get; private set; }

        public IReport Add(string info)
        {
            this.Content.AppendLine(info);
            return this;
        }

        public IReport Empty()
        {
            this.Content.Clear();
            return this;
        }

        public override string ToString()
        {
            return this.Content.ToString();
        }
    }
}