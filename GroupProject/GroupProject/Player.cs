using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject
{
    internal class Player
    {
        public string Name { get; }
        private List<Card> hand;
        private Card lastPlayedCard;

        public Player(string name)
        {
            Name = name;
            hand = new List<Card>();
            Score = 0;
        }

        public void AddCardToHand(Card card)
        {
            hand.Add(card);
        }

        public void PlayCard(Card card)
        {
            if (!hand.Contains(card))
            {
                throw new InvalidOperationException("Cannot Play Card. Card not found in players hand.");
            }
            hand.Remove(card);
            lastPlayedCard = card;
        }

        public List<Card> Hand
        {
            get { return hand; }
        }

        public Card LastPlayedCard
        {
            get { return lastPlayedCard; }
        }

        public Card PlayCard(List<Card> validCards)
        {
            if (validCards.Count == 0)
            {
                throw new InvalidOperationException("No valid cards to play.");
            }

            Card cardToPlay = validCards[0];

            if (!hand.Contains(cardToPlay))
            {
                throw new InvalidOperationException("Cannot play card. Card not found in player's hand.");
            }

            hand.Remove(cardToPlay);
            lastPlayedCard = cardToPlay;

            return cardToPlay;
        }

        public int Score { get; set; }
    }
}
