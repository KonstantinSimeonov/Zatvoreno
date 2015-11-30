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

    public class ZatvorenoAI : BasePlayer
    {
        private const string AIName = "Zatvoreno";

        private static readonly IReport GameReport = new Report();

        private static readonly ICardTracer Tracer = new CardTracer();

        private static readonly ICardEval Evaluator = new CardEval(Tracer);

        private static readonly ICardEval EvaluatorT = new CardEvaluatorFirsPlayer(Tracer);

        public override string Name
        {
            get
            {
                return AIName;
            }
        }

        public static string GetReports ()
        {
            return GameReport.ToString();
        }

        public override void StartRound (ICollection<Card> cards, Card trumpCard, int myTotalPoints, int opponentTotalPoints)
        {
            Tracer.CurrentTrumpCard = trumpCard;
            base.StartRound(cards, trumpCard, myTotalPoints, opponentTotalPoints);
        }

        public override void EndTurn (PlayerTurnContext context)
        {
            Tracer.TraceTurn(context);

            base.EndTurn(context);
        }

        public override void EndRound ()
        {
            Tracer.Empty();

            base.EndRound();
        }

        public override PlayerAction GetTurn (PlayerTurnContext context)
        {
            var myPoints = GetMyPoints(context);

            if (this.PlayerActionValidator.IsValid(PlayerAction.ChangeTrump(), context, this.Cards))
            {
                GameReport.TrumpsChanged++;
                return this.ChangeTrump(context.TrumpCard);
            }

            // TODO: closing logic

            var cardsToPlay = this.PlayerActionValidator.GetPossibleCardsToPlay(context, this.Cards);

            var announceCards = cardsToPlay
                                    .Where(c => c.Type == CardType.King || c.Type == CardType.Queen)
                                    .GroupBy(c => c.Suit)
                                    .Where(g => g.Count() > 1)
                                    .ToList();

            var possibleCardsToPlay = this.AnnounceValidator.GetPossibleAnnounce(this.Cards, announceCards.FirstOrDefault() == null ? null : announceCards.FirstOrDefault().FirstOrDefault(), context.TrumpCard, context.IsFirstPlayerTurn);

            if (possibleCardsToPlay == Santase.Logic.Announce.Forty || possibleCardsToPlay == Santase.Logic.Announce.Twenty)
            {
                if (this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(announceCards.FirstOrDefault().First()), context, this.Cards))
                {
                    GameReport.AnnounceStatistics[possibleCardsToPlay]++;
                    return this.PlayCard(announceCards.FirstOrDefault().First());
                }
            }

            if (!context.IsFirstPlayerTurn)
            {
                if (context.FirstPlayedCard.Type == CardType.Ace || context.FirstPlayedCard.Type == CardType.Ten)
                {
                    if (context.TrumpCard.Suit != context.FirstPlayedCard.Suit)
                    {
                        var trump = cardsToPlay.Where(x => x.Suit == context.TrumpCard.Suit).OrderBy(x => x.GetValue()).FirstOrDefault();

                        if (trump != null && this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(trump), context, this.Cards))
                        {
                            GameReport.TrumpedHighCards++;
                            return this.PlayCard(trump);
                        }
                    }
                }

                var card = cardsToPlay.OrderByDescending(c => Evaluator.CardScore(c, context, cardsToPlay)).First();

                if (card != null)
                {
                    return this.PlayCard(card);
                }
            }

            var cardToPlay = this.PlayCard(this
                .PlayerActionValidator
                .GetPossibleCardsToPlay(context, this.Cards)
                .OrderBy(c => EvaluatorT.CardScore(c, context, cardsToPlay))
                .First());

            return cardToPlay;
        }

        private static int GetMyPoints (PlayerTurnContext context)
        {
            return context.IsFirstPlayerTurn ? context.FirstPlayerRoundPoints : context.SecondPlayerRoundPoints;
        }

        public override void EndGame (bool amIWinner)
        {
            if (amIWinner)
            {
                ++GameReport.Wins;
            }

            base.EndGame(amIWinner);
        }

        private bool ShouldCloseGame (PlayerTurnContext context, ICollection<Card> cards)
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
