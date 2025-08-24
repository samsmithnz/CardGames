import { Card, CardNumber, CardSuite } from './Card';
import { Deck } from './Deck';

/**
 * Implements the rules and logic for Klondike Solitaire card game
 */
export class SolitaireRules {
  /**
   * The seven tableau columns where cards are initially dealt and can be built in descending order with alternating colors
   */
  public tableauColumns: Card[][];

  /**
   * The four foundation piles where cards are built up by suit from Ace to King
   */
  public foundationPiles: Card[][];

  /**
   * The stock pile containing face-down cards that can be drawn
   */
  public stockPile: Card[];

  /**
   * The waste pile containing face-up cards drawn from the stock
   */
  public wastePile: Card[];

  /**
   * Initializes a new Solitaire game with empty game state
   */
  constructor() {
    this.tableauColumns = [];
    for (let i = 0; i < 7; i++) {
      this.tableauColumns.push([]);
    }

    this.foundationPiles = [];
    for (let i = 0; i < 4; i++) {
      this.foundationPiles.push([]);
    }

    this.stockPile = [];
    this.wastePile = [];
  }

  /**
   * Sets up a new Solitaire game with the provided deck
   * @param deck The shuffled deck to deal from
   */
  public dealCards(deck: Deck): void {
    if (!deck || deck.count !== 52) {
      throw new Error('Deck must contain exactly 52 cards');
    }

    // Clear existing game state
    for (const column of this.tableauColumns) {
      column.length = 0;
    }
    for (const foundation of this.foundationPiles) {
      foundation.length = 0;
    }
    this.stockPile.length = 0;
    this.wastePile.length = 0;

    let cardIndex = 0;

    // Deal cards to tableau columns (1 to first column, 2 to second, etc.)
    for (let column = 0; column < 7; column++) {
      for (let card = 0; card <= column; card++) {
        this.tableauColumns[column].push(deck.cards[cardIndex]);
        cardIndex++;
      }
    }

    // Remaining cards go to stock pile
    for (let i = cardIndex; i < deck.cards.length; i++) {
      this.stockPile.push(deck.cards[i]);
    }
  }

  /**
   * Checks if a card can be placed on a tableau column
   * @param card The card to place
   * @param columnIndex The tableau column index (0-6)
   * @returns True if the move is valid, false otherwise
   */
  public canPlaceCardOnTableau(card: Card, columnIndex: number): boolean {
    if (columnIndex < 0 || columnIndex >= 7 || !card) {
      return false;
    }

    const column = this.tableauColumns[columnIndex];

    // Empty column can only accept Kings
    if (column.length === 0) {
      return card.number === CardNumber.K;
    }

    // Get the top card of the column
    const topCard = column[column.length - 1];

    // Check if card is one rank lower and opposite color
    return this.isOneRankLower(card.number, topCard.number) && this.isOppositeColor(card, topCard);
  }

  /**
   * Checks if a card can be placed on a foundation pile
   * @param card The card to place
   * @param foundationIndex The foundation pile index (0-3)
   * @returns True if the move is valid, false otherwise
   */
  public canPlaceCardOnFoundation(card: Card, foundationIndex: number): boolean {
    if (foundationIndex < 0 || foundationIndex >= 4 || !card) {
      return false;
    }

    const foundation = this.foundationPiles[foundationIndex];

    // Empty foundation can only accept Aces
    if (foundation.length === 0) {
      return card.number === CardNumber.A;
    }

    // Get the top card of the foundation
    const topCard = foundation[foundation.length - 1];

    // Check if card is same suit and one rank higher
    return card.suite === topCard.suite && this.isOneRankHigher(card.number, topCard.number);
  }

  /**
   * Checks if the game has been won (all cards moved to foundations)
   * @returns True if the game is won, false otherwise
   */
  public isGameWon(): boolean {
    for (const foundation of this.foundationPiles) {
      if (foundation.length !== 13) {
        return false;
      }
    }
    return true;
  }

  /**
   * Finds the foundation pile for a card's specific suite
   * @param card The card to place
   * @returns The foundation index (0-3) if a valid placement is found, -1 otherwise
   */
  public findAvailableFoundationPile(card: Card): number {
    if (!card) {
      return -1;
    }

    // Each suite maps to a specific foundation pile based on the enum index
    const suiteFoundationIndex = this.getSuiteIndex(card.suite);
    
    if (this.canPlaceCardOnFoundation(card, suiteFoundationIndex)) {
      return suiteFoundationIndex;
    }
    
    return -1;
  }

  /**
   * Draws a card from the stock pile to the waste pile
   * @returns True if a card was drawn, false if stock pile is empty
   */
  public drawFromStock(): boolean {
    if (this.stockPile.length === 0) {
      return false;
    }

    const drawnCard = this.stockPile.pop()!;
    this.wastePile.push(drawnCard);
    return true;
  }

  /**
   * Resets the waste pile back to the stock pile when stock is empty
   */
  public resetStock(): void {
    if (this.stockPile.length === 0 && this.wastePile.length > 0) {
      // Move all waste cards back to stock in reverse order
      for (let i = this.wastePile.length - 1; i >= 0; i--) {
        this.stockPile.push(this.wastePile[i]);
      }
      this.wastePile.length = 0;
    }
  }

  /**
   * Checks if one card number is exactly one rank lower than another
   */
  private isOneRankLower(lower: CardNumber, higher: CardNumber): boolean {
    return this.getCardNumberValue(lower) === this.getCardNumberValue(higher) - 1;
  }

  /**
   * Checks if one card number is exactly one rank higher than another
   */
  private isOneRankHigher(higher: CardNumber, lower: CardNumber): boolean {
    return this.getCardNumberValue(higher) === this.getCardNumberValue(lower) + 1;
  }

  /**
   * Checks if two cards are opposite colors (red vs black)
   */
  private isOppositeColor(card1: Card, card2: Card): boolean {
    return card1.isRed() !== card2.isRed();
  }

  /**
   * Gets the numeric value for card comparison (A=1, 2=2, ..., K=13)
   */
  private getCardNumberValue(number: CardNumber): number {
    const card = new Card(number, CardSuite.Heart); // Suite doesn't matter for value
    return card.getNumericValue();
  }

  /**
   * Gets the index for a card suite (for foundation pile mapping)
   */
  private getSuiteIndex(suite: CardSuite): number {
    const suites = Object.values(CardSuite);
    return suites.indexOf(suite);
  }
}