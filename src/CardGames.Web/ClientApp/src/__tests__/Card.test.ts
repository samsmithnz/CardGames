import { Card, CardNumber, CardSuite } from '../core/Card';

describe('Card', () => {
  describe('constructor', () => {
    it('should create a card with the specified number and suite', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      
      expect(card.number).toBe(CardNumber.A);
      expect(card.suite).toBe(CardSuite.Heart);
    });
  });

  describe('equals', () => {
    it('should return true for cards with same number and suite', () => {
      const card1 = new Card(CardNumber.A, CardSuite.Heart);
      const card2 = new Card(CardNumber.A, CardSuite.Heart);
      
      expect(card1.equals(card2)).toBe(true);
    });

    it('should return false for cards with different numbers', () => {
      const card1 = new Card(CardNumber.A, CardSuite.Heart);
      const card2 = new Card(CardNumber.K, CardSuite.Heart);
      
      expect(card1.equals(card2)).toBe(false);
    });

    it('should return false for cards with different suites', () => {
      const card1 = new Card(CardNumber.A, CardSuite.Heart);
      const card2 = new Card(CardNumber.A, CardSuite.Spade);
      
      expect(card1.equals(card2)).toBe(false);
    });

    it('should return false for null card', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      
      expect(card.equals(null)).toBe(false);
    });
  });

  describe('toString', () => {
    it('should return correct string representation', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      
      expect(card.toString()).toBe('A of Hearts');
    });

    it('should handle face cards correctly', () => {
      const jack = new Card(CardNumber.J, CardSuite.Club);
      const queen = new Card(CardNumber.Q, CardSuite.Diamond);
      const king = new Card(CardNumber.K, CardSuite.Spade);
      
      expect(jack.toString()).toBe('J of Clubs');
      expect(queen.toString()).toBe('Q of Diamonds');
      expect(king.toString()).toBe('K of Spades');
    });
  });

  describe('getNumericValue', () => {
    it('should return 1 for Ace', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      expect(card.getNumericValue()).toBe(1);
    });

    it('should return correct values for number cards', () => {
      expect(new Card(CardNumber._2, CardSuite.Heart).getNumericValue()).toBe(2);
      expect(new Card(CardNumber._5, CardSuite.Heart).getNumericValue()).toBe(5);
      expect(new Card(CardNumber._10, CardSuite.Heart).getNumericValue()).toBe(10);
    });

    it('should return correct values for face cards', () => {
      expect(new Card(CardNumber.J, CardSuite.Heart).getNumericValue()).toBe(11);
      expect(new Card(CardNumber.Q, CardSuite.Heart).getNumericValue()).toBe(12);
      expect(new Card(CardNumber.K, CardSuite.Heart).getNumericValue()).toBe(13);
    });
  });

  describe('isRed', () => {
    it('should return true for Hearts', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      expect(card.isRed()).toBe(true);
    });

    it('should return true for Diamonds', () => {
      const card = new Card(CardNumber.A, CardSuite.Diamond);
      expect(card.isRed()).toBe(true);
    });

    it('should return false for Clubs', () => {
      const card = new Card(CardNumber.A, CardSuite.Club);
      expect(card.isRed()).toBe(false);
    });

    it('should return false for Spades', () => {
      const card = new Card(CardNumber.A, CardSuite.Spade);
      expect(card.isRed()).toBe(false);
    });
  });

  describe('isBlack', () => {
    it('should return true for Clubs', () => {
      const card = new Card(CardNumber.A, CardSuite.Club);
      expect(card.isBlack()).toBe(true);
    });

    it('should return true for Spades', () => {
      const card = new Card(CardNumber.A, CardSuite.Spade);
      expect(card.isBlack()).toBe(true);
    });

    it('should return false for Hearts', () => {
      const card = new Card(CardNumber.A, CardSuite.Heart);
      expect(card.isBlack()).toBe(false);
    });

    it('should return false for Diamonds', () => {
      const card = new Card(CardNumber.A, CardSuite.Diamond);
      expect(card.isBlack()).toBe(false);
    });
  });
});