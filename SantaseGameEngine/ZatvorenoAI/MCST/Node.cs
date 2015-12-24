namespace ZatvorenoAI.MCST
{
    using System.Collections.Generic;
    using Santase.Logic.Cards;

    public class Node
    {
        public Node(Node parent, Card card, bool myCard)
        {
            this.Parent = parent;
            this.Card = card;
            this.MyCard = myCard;
            this.Children = new List<Node>();
            this.Wins = 0;
            this.Total = 0;
        }

        public bool MyCard { get; set; }

        public int Wins { get; set; }

        public int Total { get; set; }

        public Node Parent { get; set; }

        public Card Card { get; set; }

        public IList<Node> Children { get; set; }

        public override string ToString()
        {
            return string.Format("{1} played by {0}", this.MyCard ? "me" : "opponent", this.Card.ToString());
        }
    }
}