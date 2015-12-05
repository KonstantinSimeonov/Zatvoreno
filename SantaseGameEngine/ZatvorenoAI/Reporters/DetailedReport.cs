namespace ZatvorenoAI.Reporters
{
    using System;
    using System.IO;
    using System.Text;

    public class DetailedReport : IReport
    {
        public StringBuilder Content { get; private set; }

        public DetailedReport()
        {
            this.Content = new StringBuilder();
        }

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

        public void ToFile(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }

            File.AppendAllText(path, this.Content.ToString());
        }

        public override string ToString()
        {
            return this.Content.ToString();
        }
    }
}