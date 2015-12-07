namespace ZatvorenoAI.Reporters
{
    using System.Text;
    using Contracts;

    public class EmptyReport : IReport
    {
        public StringBuilder Content
        {
            get
            {
                return new StringBuilder();
            }
        }

        public IReport Add(string info)
        {
            return this;
        }

        public IReport Empty()
        {
            return this;
        }
    }
}