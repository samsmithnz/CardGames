import { Card, CardNumber, CardSuite } from './Card';

/**
 * Represents a deck of playing cards
 */
export class Deck {
  /**
   * Gets the list of cards in the deck
   */
  public cards: Card[];
  
  private rng: () => number;

  /**
   * Initializes a new instance of the Deck class with a standard 52-card deck
   * @param randomGenerator Optional random number generator for testing purposes
   */
  constructor(randomGenerator?: () => number) {
    this.rng = randomGenerator || Math.random;
    
    // Add all of the cards to the deck
    this.cards = [];
    for (const suite of Object.values(CardSuite)) {
      for (const number of Object.values(CardNumber)) {
        this.cards.push(new Card(number, suite));
      }
    }
  }

  /**
   * Shuffles the cards in the deck using the Fisher-Yates shuffle algorithm
   */
  public shuffle(): void {
    let n = this.cards.length;
    while (n > 1) {
      n--;
      const k = Math.floor(this.rng() * (n + 1));
      const value = this.cards[k];
      this.cards[k] = this.cards[n];
      this.cards[n] = value;
    }
  }

  /**
   * Gets the number of cards in the deck
   */
  public get count(): number {
    return this.cards.length;
  }

  /**
   * Deals (removes and returns) the top card from the deck
   * @returns The top card, or null if the deck is empty
   */
  public dealCard(): Card | null {
    if (this.cards.length === 0) {
      return null;
    }
    return this.cards.pop() || null;
  }

  /**
   * Adds a card to the top of the deck
   * @param card The card to add
   */
  public addCard(card: Card): void {
    this.cards.push(card);
  }

  /**
   * Checks if the deck is empty
   */
  public isEmpty(): boolean {
    return this.cards.length === 0;
  }

  /**
   * Returns a copy of the deck
   */
  public clone(): Deck {
    const newDeck = new Deck(this.rng);
    newDeck.cards = this.cards.map(card => new Card(card.number, card.suite));
    return newDeck;
  }
}