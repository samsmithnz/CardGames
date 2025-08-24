/**
 * Represents the rank/number of a playing card
 */
export enum CardNumber {
  A = 'A',
  _2 = '2',
  _3 = '3',
  _4 = '4',
  _5 = '5',
  _6 = '6',
  _7 = '7',
  _8 = '8',
  _9 = '9',
  _10 = '10',
  J = 'J',
  Q = 'Q',
  K = 'K'
}

/**
 * Represents the suit of a playing card
 */
export enum CardSuite {
  Heart = 'Heart',
  Club = 'Club',
  Diamond = 'Diamond',
  Spade = 'Spade'
}

/**
 * Represents a playing card with a number/rank and suit
 */
export class Card {
  /**
   * Gets or sets the number/rank of the card
   */
  public number: CardNumber;
  
  /**
   * Gets or sets the suit of the card
   */
  public suite: CardSuite;

  constructor(number: CardNumber, suite: CardSuite) {
    this.number = number;
    this.suite = suite;
  }

  /**
   * Checks if this card equals another card
   */
  public equals(other: Card | null): boolean {
    if (other === null || other === undefined) {
      return false;
    }
    return this.number === other.number && this.suite === other.suite;
  }

  /**
   * Returns a string representation of the card
   */
  public toString(): string {
    return `${this.number} of ${this.suite}s`;
  }

  /**
   * Gets the numeric value of the card for comparison (Ace = 1, Jack = 11, Queen = 12, King = 13)
   */
  public getNumericValue(): number {
    switch (this.number) {
      case CardNumber.A: return 1;
      case CardNumber._2: return 2;
      case CardNumber._3: return 3;
      case CardNumber._4: return 4;
      case CardNumber._5: return 5;
      case CardNumber._6: return 6;
      case CardNumber._7: return 7;
      case CardNumber._8: return 8;
      case CardNumber._9: return 9;
      case CardNumber._10: return 10;
      case CardNumber.J: return 11;
      case CardNumber.Q: return 12;
      case CardNumber.K: return 13;
      default: return 0;
    }
  }

  /**
   * Checks if this card is red (Hearts or Diamonds)
   */
  public isRed(): boolean {
    return this.suite === CardSuite.Heart || this.suite === CardSuite.Diamond;
  }

  /**
   * Checks if this card is black (Clubs or Spades)
   */
  public isBlack(): boolean {
    return this.suite === CardSuite.Club || this.suite === CardSuite.Spade;
  }
}