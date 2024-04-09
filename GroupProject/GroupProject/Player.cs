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

        public void ResetLastPlayedCard()
        {
            lastPlayedCard = null;
        }


        public int Score { get; set; }
    }
}