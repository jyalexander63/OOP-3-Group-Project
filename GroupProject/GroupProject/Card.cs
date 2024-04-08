using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject
{
    internal class Card
    {
        public int Value { get; }
        public Suit Suit { get; }

        public Card(int value, Suit suit)
        {
            Value = value;
            Suit = suit;
        }
    }

    internal enum Suit
    {
        Spades,
        Hearts,
        Diamonds,
        Clubs
    }
}
