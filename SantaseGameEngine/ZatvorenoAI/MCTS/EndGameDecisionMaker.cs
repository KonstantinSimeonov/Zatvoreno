namespace ZatvorenoAI.MCTS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Santase.Logic.Cards;

    public static class EndGameDecisionMaker
    {
        public const int WinPoints = 66;

        public static Func<Card, Func<Card, bool>> CanPlay { get; set; } = card => c => c != null && c.Suit == card.Suit;

        public static CardSuit TrumpSuit { get; set; }

        internal static bool IsTrump(this Card card)
        {
            return card.Suit == TrumpSuit;
        }

        internal static bool IsKingOrQueen(this Card card)
        {
            return card.Type == CardType.Queen || card.Type == CardType.King;
        }

        public static void Compute(IList<Card> myCards, IList<Card> oppCards, Card oppCard, int myPoints, int oppPoints, MCSTNode parent)
        {
            bool amFirst = oppCard == null;

            if (!myCards.Any(x => x != null) || !oppCards.Any(x => x != null))
            {

            }

            if (myPoints >= WinPoints)
            {
                parent.WinRate++;
                parent.TotalChildren++;
                return;
            }

            if (oppPoints >= WinPoints)
            {
                parent.TotalChildren++;
                return;
            }

            if (amFirst)
            {
                var announcesInHand = myCards.Where(x => x != null && x.IsKingOrQueen())
                                             .GroupBy(x => x.Suit).Where(x => x.Count() == 2)
                                             .SelectMany(x => x)
                                             .ToList();

                for (int i = 0; i < myCards.Count; i++)
                {
                    if (myCards[i] != null)
                    {
                        var card = myCards[i];
                        myCards[i] = null;

                        var myCardValue = card.GetValue();

                        var nextParent = new MCSTNode(0, 0, card);
                        parent.Children.Add(nextParent);

                        var oppCanAnswer = oppCards.Where(x => x != null && x.Suit == card.Suit).Any();
                        var oppHasTrumps = oppCards.Where(x => x != null && x.IsTrump()).Any();

                        for (int j = 0; j < oppCards.Count; j++)
                        {
                            // TODO: trumping
                            if (oppCards[j] != null && (oppCards[j].Suit == card.Suit || (!oppCanAnswer && ((oppHasTrumps && oppCards[j].IsTrump()) || !oppHasTrumps))))
                            {
                                var oppChoice = oppCards[j];
                                oppCards[j] = null;

                                var myUpdate = 0;
                                var oppUpdate = 0;
                                var oppCardValue = oppChoice.GetValue();

                                // TODO: daiba
                                if ((myCardValue > oppCardValue && oppCanAnswer) || (!oppCanAnswer && !oppChoice.IsTrump()))
                                {
                                    myUpdate += myCardValue + oppCardValue;
                                    if (announcesInHand.Contains(card))
                                    {
                                        myUpdate += card.IsTrump() ? 40 : 20;
                                    }

                                    Compute(myCards, oppCards, null, myPoints + myUpdate, oppPoints + oppUpdate, nextParent);
                                }
                                else
                                {
                                    oppUpdate += myCardValue + oppCardValue;

                                    for (int k = 0; k < oppCards.Count; k++)
                                    {
                                        if (oppCards[k] != null)
                                        {
                                            var opp = oppCards[k];
                                            oppCards[k] = null;

                                            Compute(myCards, oppCards, opp, myPoints + myUpdate, oppPoints + oppUpdate, nextParent);

                                            oppCards[k] = opp;
                                        }
                                    }
                                }

                                oppCards[j] = oppChoice;
                            }
                        }

                        parent.TotalChildren += nextParent.TotalChildren;
                        parent.WinRate += nextParent.WinRate;

                        myCards[i] = card;
                    }
                }
            }
            else
            {
                var canAnswer = myCards.Where(x => x != null && x.Suit == oppCard.Suit).Any();
                var hasTrumps = myCards.Where(x => x != null && x.Suit == TrumpSuit).Any();

                var announcesInHand = oppCards.Where(x => x != null && x.IsKingOrQueen())
                                             .GroupBy(x => x.Suit).Where(x => x.Count() == 2)
                                             .SelectMany(x => x)
                                             .ToList();

                var oppCardValue = oppCard.GetValue();

                for (int i = 0; i < myCards.Count; i++)
                {
                    if (myCards[i] != null && (myCards[i].Suit == oppCard.Suit || (!canAnswer && ((hasTrumps && myCards[i].IsTrump()) || !hasTrumps))))
                    {
                        var card = myCards[i];
                        myCards[i] = null;

                        var nextParent = new MCSTNode(0, 0, card);
                        parent.Children.Add(nextParent);

                        var myUpdate = 0;
                        var oppUpdate = 0;
                        var myCardValue = card.GetValue();

                        if ((myCardValue > oppCardValue && canAnswer) || (card.IsTrump() && !oppCard.IsTrump()))
                        {
                            myUpdate += myCardValue + oppCardValue;

                            Compute(myCards, oppCards, null, myPoints + myUpdate, oppPoints + oppUpdate, nextParent);
                        }
                        else
                        {
                            oppUpdate += myCardValue + oppCardValue;

                            if (!oppCards.Any(x => x != null))
                            {
                                if (oppPoints + oppUpdate >= WinPoints)
                                {
                                    parent.TotalChildren++;
                                }
                            }
                            else
                            {
                                for (int j = 0; j < oppCards.Count; j++)
                                {
                                    if (oppCards[j] != null)
                                    {
                                        var oppChoice = oppCards[j];
                                        oppCards[j] = null;

                                        if (announcesInHand.Contains(oppChoice))
                                        {
                                            oppUpdate += oppChoice.IsTrump() ? 40 : 20;
                                        }

                                        Compute(myCards, oppCards, oppChoice, myPoints + myUpdate, oppPoints + oppUpdate, nextParent);

                                        oppCards[j] = oppChoice;

                                        if (announcesInHand.Contains(oppChoice))
                                        {
                                            oppUpdate -= oppChoice.IsTrump() ? 40 : 20;
                                        }
                                    }
                                }
                            }
                        }

                        parent.TotalChildren += nextParent.TotalChildren;
                        parent.WinRate += nextParent.WinRate;

                        myCards[i] = card;
                    }
                }
            }

            //var first = amFirst ? myCards : oppCards;
            //var second = !amFirst ? myCards : oppCards;

            //for (var i = 0; i < first.Count; i++)
            //{
            //    var f = first[i];
            //    var possibilities = second.Where(c => c.Suit == first[i].Suit).ToList();

            //    if (possibilities.Count == 0)
            //    {
            //        possibilities = second as List<Card>;
            //    }

            //    for (var j = 0; j < possibilities.Count; j++)
            //    {
            //        var s = possibilities[j];

            //        // remove for subtree calculation
            //        first.Remove(f);
            //        second.Remove(s);

            //        var nextParent = new MCSTNode(0, 0, amFirst ? f : s);
            //        Compute(myCards, oppCards, (amFirst ? f : s).GetValue() > (!amFirst ? f : s).GetValue(), myPoints + (amFirst ? f : s).GetValue(), oppPoints + (!amFirst ? f : s).GetValue(), nextParent);

            //        // update parent on the recursive tail
            //        parent.Children.Add(nextParent);
            //        parent.WinRate += nextParent.WinRate;
            //        parent.TotalChildren += nextParent.TotalChildren;

            //        // add for future calculations
            //        first.Add(f);
            //        second.Add(s);
            //    }
            //}
        }
    }
}