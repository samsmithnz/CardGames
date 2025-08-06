using System;

namespace CardGames.Core
{
    public class Card : IEquatable<Card>
    {
        public CardNumber Number { get; set; }
        public CardSuite Suite { get; set; }

        public enum CardNumber
        {
            A,
            _2,
            _3,
            _4,
            _5,
            _6,
            _7,
            _8,
            _9,
            _10,
            J,
            Q,
            K                
        }

        public enum CardSuite
        {
            Heart,
            Club,
            Diamond,
            Spade
        }

        public bool Equals(Card other)
        {
            if (other == null)
            {
                return false;
            }
            return Number == other.Number && Suite == other.Suite;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Card);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Number, Suite);
        }

        public static bool operator ==(Card left, Card right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(Card left, Card right)
        {
            return !(left == right);
        }

        //TODO: Add jokers
    }
}
