import { SolitaireRules } from '../core/SolitaireRules';
import { Card, CardNumber, CardSuite } from '../core/Card';
import { Deck } from '../core/Deck';

describe('SolitaireRules', () => {
  let rules: SolitaireRules;

  beforeEach(() => {
    rules = new SolitaireRules();
  });

  describe('constructor', () => {
    it('should initialize empty game state', () => {
      expect(rules.tableauColumns).toHaveLength(7);
      expect(rules.foundationPiles).toHaveLength(4);
      expect(rules.stockPile).toHaveLength(0);
      expect(rules.wastePile).toHaveLength(0);

      // All tableau columns should be empty
      for (const column of rules.tableauColumns) {
        expect(column).toHaveLength(0);
      }

      // All foundation piles should be empty
      for (const foundation of rules.foundationPiles) {
        expect(foundation).toHaveLength(0);
      }
    });
  });

  describe('dealCards', () => {
    it('should deal cards according to solitaire rules', () => {
      const deck = new Deck();
      
      rules.dealCards(deck);

      // Check tableau distribution (1, 2, 3, 4, 5, 6, 7 cards)
      for (let i = 0; i < 7; i++) {
        expect(rules.tableauColumns[i]).toHaveLength(i + 1);
      }

      // Total tableau cards should be 28 (1+2+3+4+5+6+7)
      const totalTableauCards = rules.tableauColumns.reduce((sum, column) => sum + column.length, 0);
      expect(totalTableauCards).toBe(28);

      // Remaining 24 cards should be in stock pile
      expect(rules.stockPile).toHaveLength(24);

      // Waste pile should be empty
      expect(rules.wastePile).toHaveLength(0);

      // Foundation piles should be empty
      for (const foundation of rules.foundationPiles) {
        expect(foundation).toHaveLength(0);
      }
    });

    it('should throw error for invalid deck', () => {
      const invalidDeck = new Deck();
      invalidDeck.dealCard(); // Remove one card

      expect(() => rules.dealCards(invalidDeck)).toThrow('Deck must contain exactly 52 cards');
    });

    it('should clear existing game state when dealing', () => {
      // Set up some initial state
      rules.stockPile.push(new Card(CardNumber.A, CardSuite.Heart));
      rules.wastePile.push(new Card(CardNumber._2, CardSuite.Club));
      rules.tableauColumns[0].push(new Card(CardNumber._3, CardSuite.Diamond));
      rules.foundationPiles[0].push(new Card(CardNumber._4, CardSuite.Spade));

      const deck = new Deck();
      rules.dealCards(deck);

      // All should be cleared and repopulated according to solitaire rules
      expect(rules.stockPile).toHaveLength(24);
      expect(rules.wastePile).toHaveLength(0);
      expect(rules.tableauColumns[0]).toHaveLength(1); // First column has 1 card
      expect(rules.foundationPiles[0]).toHaveLength(0);
    });
  });

  describe('canPlaceCardOnTableau', () => {
    beforeEach(() => {
      // Set up a simple test scenario
      const redCard = new Card(CardNumber._7, CardSuite.Heart);
      const blackCard = new Card(CardNumber._6, CardSuite.Spade);
      rules.tableauColumns[0].push(redCard);
    });

    it('should allow placing a card one rank lower and opposite color', () => {
      const validCard = new Card(CardNumber._6, CardSuite.Club); // Black 6 on red 7
      
      expect(rules.canPlaceCardOnTableau(validCard, 0)).toBe(true);
    });

    it('should reject same color cards', () => {
      const invalidCard = new Card(CardNumber._6, CardSuite.Diamond); // Red 6 on red 7
      
      expect(rules.canPlaceCardOnTableau(invalidCard, 0)).toBe(false);
    });

    it('should reject wrong rank cards', () => {
      const invalidCard = new Card(CardNumber._5, CardSuite.Club); // Black 5 on red 7 (not adjacent)
      
      expect(rules.canPlaceCardOnTableau(invalidCard, 0)).toBe(false);
    });

    it('should only allow Kings on empty columns', () => {
      const king = new Card(CardNumber.K, CardSuite.Heart);
      const notKing = new Card(CardNumber.Q, CardSuite.Heart);

      expect(rules.canPlaceCardOnTableau(king, 1)).toBe(true); // Empty column
      expect(rules.canPlaceCardOnTableau(notKing, 1)).toBe(false); // Empty column
    });

    it('should reject invalid column indices', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      
      expect(rules.canPlaceCardOnTableau(card, -1)).toBe(false);
      expect(rules.canPlaceCardOnTableau(card, 7)).toBe(false);
    });

    it('should reject null cards', () => {
      expect(rules.canPlaceCardOnTableau(null as any, 0)).toBe(false);
    });
  });

  describe('canPlaceCardOnFoundation', () => {
    it('should only allow Aces on empty foundations', () => {
      const ace = new Card(CardNumber.A, CardSuite.Heart);
      const notAce = new Card(CardNumber._2, CardSuite.Heart);

      expect(rules.canPlaceCardOnFoundation(ace, 0)).toBe(true);
      expect(rules.canPlaceCardOnFoundation(notAce, 0)).toBe(false);
    });

    it('should allow same suit cards one rank higher', () => {
      // Place Ace of Hearts on foundation
      const ace = new Card(CardNumber.A, CardSuite.Heart);
      rules.foundationPiles[0].push(ace);

      const validCard = new Card(CardNumber._2, CardSuite.Heart); // Same suit, one higher
      const wrongSuit = new Card(CardNumber._2, CardSuite.Club); // Different suit
      const wrongRank = new Card(CardNumber._3, CardSuite.Heart); // Same suit, wrong rank

      expect(rules.canPlaceCardOnFoundation(validCard, 0)).toBe(true);
      expect(rules.canPlaceCardOnFoundation(wrongSuit, 0)).toBe(false);
      expect(rules.canPlaceCardOnFoundation(wrongRank, 0)).toBe(false);
    });

    it('should reject invalid foundation indices', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      
      expect(rules.canPlaceCardOnFoundation(card, -1)).toBe(false);
      expect(rules.canPlaceCardOnFoundation(card, 4)).toBe(false);
    });

    it('should reject null cards', () => {
      expect(rules.canPlaceCardOnFoundation(null as any, 0)).toBe(false);
    });
  });

  describe('isGameWon', () => {
    it('should return false for empty foundations', () => {
      expect(rules.isGameWon()).toBe(false);
    });

    it('should return false for partially filled foundations', () => {
      // Fill some cards in foundations but not complete
      for (let i = 0; i < 5; i++) {
        rules.foundationPiles[0].push(new Card(CardNumber.A, CardSuite.Heart));
      }
      
      expect(rules.isGameWon()).toBe(false);
    });

    it('should return true when all foundations have 13 cards', () => {
      // Fill all foundations with 13 cards each
      for (let foundation = 0; foundation < 4; foundation++) {
        for (let i = 0; i < 13; i++) {
          rules.foundationPiles[foundation].push(new Card(CardNumber.A, CardSuite.Heart));
        }
      }

      expect(rules.isGameWon()).toBe(true);
    });
  });

  describe('findAvailableFoundationPile', () => {
    it('should return correct foundation index for Ace', () => {
      const aceOfHearts = new Card(CardNumber.A, CardSuite.Heart);
      const aceOfClubs = new Card(CardNumber.A, CardSuite.Club);
      
      expect(rules.findAvailableFoundationPile(aceOfHearts)).toBeGreaterThanOrEqual(0);
      expect(rules.findAvailableFoundationPile(aceOfClubs)).toBeGreaterThanOrEqual(0);
    });

    it('should return correct foundation index for valid next card', () => {
      // Place Ace of Hearts in foundation
      const ace = new Card(CardNumber.A, CardSuite.Heart);
      const foundationIndex = rules.findAvailableFoundationPile(ace);
      rules.foundationPiles[foundationIndex].push(ace);

      // Two of Hearts should go to same foundation
      const two = new Card(CardNumber._2, CardSuite.Heart);
      expect(rules.findAvailableFoundationPile(two)).toBe(foundationIndex);
    });

    it('should return -1 for invalid placements', () => {
      const two = new Card(CardNumber._2, CardSuite.Heart); // Can't place 2 on empty foundation
      
      expect(rules.findAvailableFoundationPile(two)).toBe(-1);
    });

    it('should return -1 for null card', () => {
      expect(rules.findAvailableFoundationPile(null as any)).toBe(-1);
    });
  });

  describe('drawFromStock', () => {
    it('should move card from stock to waste', () => {
      // Add cards to stock pile
      const card1 = new Card(CardNumber.A, CardSuite.Heart);
      const card2 = new Card(CardNumber._2, CardSuite.Club);
      rules.stockPile.push(card1, card2);

      const result = rules.drawFromStock();

      expect(result).toBe(true);
      expect(rules.stockPile).toHaveLength(1);
      expect(rules.wastePile).toHaveLength(1);
      expect(rules.wastePile[0]).toBe(card2); // Last card added should be drawn first
    });

    it('should return false when stock is empty', () => {
      const result = rules.drawFromStock();

      expect(result).toBe(false);
      expect(rules.stockPile).toHaveLength(0);
      expect(rules.wastePile).toHaveLength(0);
    });
  });

  describe('resetStock', () => {
    it('should move waste pile back to stock in reverse order', () => {
      // Add cards to waste pile
      const card1 = new Card(CardNumber.A, CardSuite.Heart);
      const card2 = new Card(CardNumber._2, CardSuite.Club);
      const card3 = new Card(CardNumber._3, CardSuite.Diamond);
      rules.wastePile.push(card1, card2, card3);

      rules.resetStock();

      expect(rules.wastePile).toHaveLength(0);
      expect(rules.stockPile).toHaveLength(3);
      expect(rules.stockPile[0]).toBe(card3); // Last waste card becomes first stock card
      expect(rules.stockPile[1]).toBe(card2);
      expect(rules.stockPile[2]).toBe(card1); // First waste card becomes last stock card
    });

    it('should do nothing if stock is not empty', () => {
      const stockCard = new Card(CardNumber.K, CardSuite.Spade);
      const wasteCard = new Card(CardNumber.Q, CardSuite.Heart);
      
      rules.stockPile.push(stockCard);
      rules.wastePile.push(wasteCard);

      rules.resetStock();

      // No change should occur
      expect(rules.stockPile).toHaveLength(1);
      expect(rules.wastePile).toHaveLength(1);
      expect(rules.stockPile[0]).toBe(stockCard);
      expect(rules.wastePile[0]).toBe(wasteCard);
    });

    it('should do nothing if waste pile is empty', () => {
      rules.resetStock();

      expect(rules.stockPile).toHaveLength(0);
      expect(rules.wastePile).toHaveLength(0);
    });
  });
});