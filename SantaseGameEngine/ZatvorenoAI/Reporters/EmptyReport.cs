namespace ZatvorenoAI.Reporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

        public void ToFile(string path)
        {
        }
    }
}
