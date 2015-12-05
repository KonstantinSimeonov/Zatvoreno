using System.Text;

namespace ZatvorenoAI.Reporters
{
    public interface IReport
    {
        StringBuilder Content { get; }

        IReport Add(string info);

        IReport Empty();

        void ToFile(string path);

        string ToString();
    }
}