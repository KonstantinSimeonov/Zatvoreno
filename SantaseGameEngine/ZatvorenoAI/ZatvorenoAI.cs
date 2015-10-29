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

        private static readonly ICardEvaluator Evaluator = new CardEval(Tracer);

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
                        var trump = cardsToPlay.FirstOrDefault(x => x.Suit == context.TrumpCard.Suit);

                        if (trump != null && this.PlayerActionValidator.IsValid(PlayerAction.PlayCard(trump), context, this.Cards))
                        {
                            GameReport.TrumpedHighCards++;
                            return this.PlayCard(trump);
                        }

                    }

                }

                //var cardsToPlayWhenSecond = cardsToPlay
                //                    .Where(c => context.FirstPlayedCard.Suit == c.Suit && context.FirstPlayedCard.Type.CompareTo(c.Type) < 0)
                //                    .OrderByDescending(c => c.Type)
                //                    .ToList();

                var card = cardsToPlay.OrderByDescending(c => Evaluator.CardPlayabilityValue(c, context)).First();

                if (card != null)
                {
                    return this.PlayCard(card);
                }
            }

            var cardToPlay = this.PlayCard(this
                .PlayerActionValidator
                .GetPossibleCardsToPlay(context, this.Cards)
                .OrderByDescending(c => Evaluator.CardPlayabilityValue(c, context))
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
                GameReport.Wins++;
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
