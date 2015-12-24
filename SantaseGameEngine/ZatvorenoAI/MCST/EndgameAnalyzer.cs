namespace ZatvorenoAI.MCST
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Santase.Logic.Cards;

    public static class EndgameAnalyzer
    {
        private static CardSuit Trump = CardSuit.Club;

        public static bool IsTrump(this Card card)
        {
            return card.Suit == Trump;
        }

        public static bool HasSameSuitAs(this Card card, Card card2)
        {
            return card.Suit == card2.Suit;
        }

        public static void Compute(Node root, Card oppCard, IList<Card> myCards, IList<Card> oppCards, int myPoints, int oppPoints)
        {
            if (myPoints >= 66)
            {
                root.Wins++;
                root.Total++;

                return;
            }

            if (oppPoints >= 66)
            {
                root.Total++;

                return;
            }

            if (oppCard == null)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (myCards[i] == null)
                    {
                        continue;
                    }

                    var myNextCard = myCards[i];
                    myCards[i] = null;

                    var root1 = new Node(root, myNextCard, true);

                    var myValue = myNextCard.GetValue();

                    for (int j = 0; j < 6; j++)
                    {
                        if (oppCards[j] == null)
                        {
                            continue;
                        }

                        var nextOppCard = oppCards[j];
                        oppCards[j] = null;

                        var oppValue = nextOppCard.GetValue();

                        if ((nextOppCard.HasSameSuitAs(myNextCard) && oppValue > myValue) || (!oppCards.Any(x => x!= null && x.Suit == myNextCard.Suit) && nextOppCard.IsTrump()))
                        {
                            var root2 = new Node(root1, nextOppCard, false);

                            for (int k = 0; k < 6; k++)
                            {
                                if (oppCards[k] == null)
                                {
                                    continue;
                                }

                                var oppCard3 = oppCards[k];
                                oppCards[k] = null;

                                var root3 = new Node(root2, oppCard3, false);

                                Compute(root3, oppCard3, myCards, oppCards, myPoints, oppPoints + myValue + oppValue);

                                root2.Children.Add(root3);
                                root2.Wins += root3.Wins;
                                root2.Total += root3.Total;

                                oppCards[k] = oppCard3;
                            }

                            root1.Children.Add(root2);
                            root1.Wins += root2.Wins;
                            root1.Total += root2.Total;
                        }
                        else
                        {
                            var root2 = new Node(root1, nextOppCard, true);

                            Compute(root2, null, myCards, oppCards, myPoints + myValue + oppValue, oppPoints);

                            root1.Children.Add(root2);
                            root1.Wins += root2.Wins;
                            root1.Total += root2.Total;
                        }

                        oppCards[j] = nextOppCard;
                    }

                    root.Children.Add(root1);
                    root.Wins += root1.Wins;
                    root.Total += root1.Total;

                    myCards[i] = myNextCard;
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (myCards[i] == null)
                    {
                        continue;
                    }

                    var myNextCard = myCards[i];
                    myCards[i] = null;

                    var root1 = new Node(root, myNextCard, true);

                    if (myNextCard.HasSameSuitAs(oppCard) && myNextCard.GetValue() > oppCard.GetValue() && !(!myNextCard.IsTrump() && oppCard.IsTrump()))
                    {
                        Compute(root1, null, myCards, oppCards, myPoints + myNextCard.GetValue() + oppCard.GetValue(), oppPoints);
                    }
                    else
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            if (oppCards[j] == null)
                            {
                                continue;
                            }

                            var oppNextCard = oppCards[j];
                            oppCards[j] = null;

                            var root2 = new Node(root1, oppNextCard, false);

                            Compute(root2, oppNextCard, myCards, oppCards, myPoints, oppPoints + myNextCard.GetValue() + oppCard.GetValue());

                            root1.Children.Add(root2);
                            root1.Wins += root2.Wins;
                            root1.Total += root2.Total;

                            oppCards[j] = oppNextCard;
                        }
                    }

                    root.Children.Add(root1);
                    root.Wins += root1.Wins;
                    root.Total += root1.Total;

                    myCards[i] = myNextCard;
                }
            }
        }

        internal static void Compute(Node root, Card firstPlayedCard, object myCards, object opponentTookWith, int firstPlayerRoundPoints, int secondPlayerRoundPoints)
        {
            throw new NotImplementedException();
        }
    }
}