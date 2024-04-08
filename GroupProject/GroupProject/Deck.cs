using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject
{
    internal class Deck
    {
        private List<Card> cards;
        private Random random;
        public List<Card> Cards => cards;
        public Deck()
        {
            InitializeDeck();
            random = new Random();
        }

        private void InitializeDeck()
        {
            cards = new List<Card>();

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (int value = 2; value <= 14; value++)
                {
                    cards.Add(new Card(value, suit));
                }
            }
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public Card DealCard()
        {
            if (cards.Count == 0)
            {
                throw new InvalidOperationException("The deck is empty, Cards cannot be dealt");
            }

            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }
    }
}
