namespace ZatvorenoAI.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Santase.Logic.Cards;

    public class MCSTNode
    {
        public MCSTNode(int wins, int totalChildren, Card card)
        {
            this.WinRate = wins;
            this.TotalChildren = totalChildren;
            this.Children = new List<MCSTNode>();
            this.Card = card;
        }

        public int WinRate { get; set; }

        public int TotalChildren { get; set; }

        public IList<MCSTNode> Children { get; set; }

        public Card Card { get; set; }

        public override string ToString()
        {
            return this.WinRate + "/" + this.TotalChildren + " " + this.Card;
        }
    }
}