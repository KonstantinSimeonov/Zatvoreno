namespace ZatvorenoAI.Reporters
{
    using System.Text;

    public interface IReport
    {
        StringBuilder Content { get; }

        IReport Add(string info);

        IReport Empty();

        string ToString();
    }
}