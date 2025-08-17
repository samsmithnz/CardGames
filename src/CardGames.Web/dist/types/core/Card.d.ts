/**
 * Represents the rank/number of a playing card
 */
export declare enum CardNumber {
    A = "A",
    _2 = "2",
    _3 = "3",
    _4 = "4",
    _5 = "5",
    _6 = "6",
    _7 = "7",
    _8 = "8",
    _9 = "9",
    _10 = "10",
    J = "J",
    Q = "Q",
    K = "K"
}
/**
 * Represents the suit of a playing card
 */
export declare enum CardSuite {
    Heart = "Heart",
    Club = "Club",
    Diamond = "Diamond",
    Spade = "Spade"
}
/**
 * Represents a playing card with a number/rank and suit
 */
export declare class Card {
    /**
     * Gets or sets the number/rank of the card
     */
    number: CardNumber;
    /**
     * Gets or sets the suit of the card
     */
    suite: CardSuite;
    constructor(number: CardNumber, suite: CardSuite);
    /**
     * Checks if this card equals another card
     */
    equals(other: Card | null): boolean;
    /**
     * Returns a string representation of the card
     */
    toString(): string;
    /**
     * Gets the numeric value of the card for comparison (Ace = 1, Jack = 11, Queen = 12, King = 13)
     */
    getNumericValue(): number;
    /**
     * Checks if this card is red (Hearts or Diamonds)
     */
    isRed(): boolean;
    /**
     * Checks if this card is black (Clubs or Spades)
     */
    isBlack(): boolean;
}
