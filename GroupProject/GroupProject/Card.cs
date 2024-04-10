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

        public string FaceValue
        {
            get
            {
                switch (Value)
                {
                    case 11:
                        return "Jack";
                    case 12:
                        return "Queen";
                    case 13:
                        return "King";
                    case 14:
                        return "Ace";
                    default:
                        return Value.ToString();
                }
            }
        }
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
