namespace ZatvorenoAI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CardTracers;
    using Contracts;
    using CardEvaluator;
    using Reporters;
    using Santase.Logic.Cards;
    using Santase.Logic.Players;
    using System.IO;
    using TakeStrategy.Agents.NeedToTake.Contracts;
    using TakeStrategy.Agents.NeedToTake;
    using CardTracers.CardStates;

    public class ZatvorenoAI : BasePlayer
    {
        // Utils
        public static DetailedReport Report = new DetailedReport();

        private const string AIName = "Zatvoreno";

        private static readonly IReport GameReport = new Report();

        // Deck tracers
        private static readonly ICardTracer Tracer = new CardTracer();

        private static readonly ICardTracker Tracker = new QuickSpecificCardSearchTracker();

        // Card evaluation and decision makers
        private static readonly ICardEval Evaluator = new CardEval(Tracer);

        private static readonly ICardEval Evaluator2 = new CardEvaluatorFirsPlayer(Tracker);

        private static readonly IShouldTake TrickDecisionMakerWhenSecond = new ShouldTake(Tracker);

        // Logic
        private bool myTurn = false;

        private int currentGameId = 0;

        public override string Name
        {
            get
            {
                return AIName;
            }
        }

        public static string GetReports()
        {
            return GameReport.ToString();
        }

        public override void StartRound(ICollection<Card> cards, Card trumpCard, int myTotalPoints, int opponentTotalPoints)
        {
            Tracer.CurrentTrumpCard = trumpCard;
            Report.Add("Trump for current game is: " + trumpCard.ToString());
            base.StartRound(cards, trumpCard, myTotalPoints, opponentTotalPoints);
        }

        public override void EndTurn(PlayerTurnContext context)
        {
            Tracer.TraceTurn(context);
            Tracker.TraceTurn(context);
            Report.Add(context.Stringify(this.myTurn) + " --- current hand: " + string.Join(", ", this.Cards.Select(x => x.ToString())));

            base.EndTurn(context);
        }

        public override void EndRound()
        {
            Tracer.Empty();

            // this.report.ToFile(string.Format("../../report{0}.txt", this.currentGameId));
            //File.WriteAllText("../../report " + this.currentGameId++ + ".txt", this.report.ToString());
            Report.Add(" ------ END ROUND ------");
            base.EndRound();
        }

        public override PlayerAction GetTurn(PlayerTurnContext context)
        {
            this.myTurn = context.IsFirstPlayerTurn;

            var myPoints = GetMyPoints(context);

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
                GameReport.TrumpsChanged++;
                return this.ChangeTrump(context.TrumpCard);
            }

            // TODO: closing logic

            var availableCardsFromHand = this.PlayerActionValidator.GetPossibleCardsToPlay(context, this.Cards);

            if (context.IsFirstPlayerTurn &&
                context.CardsLeftInDeck != 12)
            {
                    var announceCards = availableCardsFromHand
                                            .Where(c => c.Type == CardType.King || c.Type == CardType.Queen)
                                            .GroupBy(c => c.Suit)
                                            .Where(g => g.Count() > 1)
                                            .ToList();

                    var fortyAnnounce = announceCards
                        .Where(g => g.FirstOrDefault().Suit == context.TrumpCard.Suit)
                        .Select(suit => suit.First(card => card.Type == CardType.Queen))
                        .FirstOrDefault();

                    if (fortyAnnounce != null)
                    {
                        GameReport.AnnounceStatistics[Santase.Logic.Announce.Forty]++;
                        return this.PlayCard(fortyAnnounce);
                    }
                    else if (announceCards.Count > 0)
                    {
                        GameReport.AnnounceStatistics[Santase.Logic.Announce.Twenty]++;
                        return this.PlayCard(announceCards.FirstOrDefault().FirstOrDefault(x => x.Type == CardType.Queen));
                    }
                // var possibleCardsToPlay = this.AnnounceValidator
                //     .GetPossibleAnnounce(this.Cards,
                //         announceCards.FirstOrDefault() == null ? null : announceCards.FirstOrDefault().FirstOrDefault(),
                //         context.TrumpCard,
                //         context.IsFirstPlayerTurn);

                // if (possibleCardsToPlay == Santase.Logic.Announce.Forty || possibleCardsToPlay == Santase.Logic.Announce.Twenty)
                // {
                //     if (this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(announceCards.FirstOrDefault().First()), context, this.Cards))
                //     {
                //         GameReport.AnnounceStatistics[possibleCardsToPlay]++;
                //         return this.PlayCard(announceCards.FirstOrDefault().First());
                //     }
                // }
            }

            if (!context.IsFirstPlayerTurn)
            {
                var shouldTake = TrickDecisionMakerWhenSecond.ShouldPlayerTake(context, this.Cards);

                if (shouldTake)
                {
                    // to extract taking agents.
                    // Should separate logic for following suit.
                    var viableCards = availableCardsFromHand
                        .Where(c => (c.Suit == context.FirstPlayedCard.Suit &&
                        c.GetValue() > context.FirstPlayedCard.GetValue()) ||
                        (context.FirstPlayedCard.Suit != context.TrumpCard.Suit && c.Suit == context.TrumpCard.Suit) ||
                        (context.FirstPlayedCard.Suit == context.TrumpCard.Suit && c.Suit == context.TrumpCard.Suit && c.GetValue() > context.FirstPlayedCard.GetValue()));

                    var viableCardsWithoutAnnouncePairs = viableCards
                        .Where(c => c.Type != CardType.Queen &&
                                    c.Type != CardType.King)
                        .ToArray();

                    if (viableCardsWithoutAnnouncePairs.Length > 0)
                    {
                        var minorNonAnnounceCard = viableCardsWithoutAnnouncePairs.OrderBy(c => c.GetValue()).First();

                        return this.PlayCard(minorNonAnnounceCard);
                    }

                    if (viableCards.ToArray().Length > 0)
                    {
                        var minorForcedAnnounceCard = viableCards.OrderBy(c => c.GetValue()).First();

                        return this.PlayCard(minorForcedAnnounceCard);
                    }
                }

                // var minorCardWhenCannotTake = availableCardsFromHand.OrderBy(c => c.GetValue()).First();
                var card = availableCardsFromHand.OrderBy(c => Evaluator.CardScore(c, context, availableCardsFromHand)).First();

                // return this.PlayCard(minorCardWhenCannotTake);
                return this.PlayCard(card);

                // if (context.FirstPlayedCard.Type == CardType.Ace || context.FirstPlayedCard.Type == CardType.Ten)
                // {
                //     if (context.TrumpCard.Suit != context.FirstPlayedCard.Suit)
                //     {
                //         var trump = cardsToPlay.Where(x => x.Suit == context.TrumpCard.Suit).OrderBy(x => x.GetValue()).FirstOrDefault();
                //         if (trump != null && this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(trump), context, this.Cards))
                //         {
                //             GameReport.TrumpedHighCards++;
                //             return this.PlayCard(trump);
                //         }
                //     }
                // }

                //var card = cardsToPlay.OrderBy(c => Evaluator.CardScore(c, context, cardsToPlay)).First();

                //if (card != null)
                //{
                //    return this.PlayCard(card);
                //}
            }

            PlayerAction cardToPlay;
            cardToPlay = this.PlayCard(this
                    .PlayerActionValidator
                    .GetPossibleCardsToPlay(context, this.Cards)
                    .OrderBy(c => Evaluator2.CardScore(c, context, availableCardsFromHand))
                    .First());

            return cardToPlay;
        }

        public override void EndGame(bool amIWinner)
        {
            if (amIWinner)
            {
                ++GameReport.Wins;
            }

            this.currentGameId++;

            base.EndGame(amIWinner);
        }

        private static int GetMyPoints(PlayerTurnContext context)
        {
            return context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;
        }

        private bool ShouldCloseGame(PlayerTurnContext context, ICollection<Card> cards)
        {
            throw new NotImplementedException("Implement close game");

            var trumpsInPower = cards
                                    .Where(x => x.Suit == context.TrumpCard.Suit)
                                    .OrderBy(x => x.Type);

            if (GetMyPoints(context) + trumpsInPower.Sum(x => (long?)x.Type) >= 66)
            {

            }

            return false;
        }

    }
}
