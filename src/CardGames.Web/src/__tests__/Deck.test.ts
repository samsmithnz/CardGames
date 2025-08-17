import { Deck } from '../core/Deck';
import { Card, CardNumber, CardSuite } from '../core/Card';

describe('Deck', () => {
  describe('constructor', () => {
    it('should create a deck with 52 cards', () => {
      const deck = new Deck();
      
      expect(deck.count).toBe(52);
    });

    it('should have all 4 suits', () => {
      const deck = new Deck();
      const suites = new Set(deck.cards.map(card => card.suite));
      
      expect(suites.size).toBe(4);
      expect(suites.has(CardSuite.Heart)).toBe(true);
      expect(suites.has(CardSuite.Club)).toBe(true);
      expect(suites.has(CardSuite.Diamond)).toBe(true);
      expect(suites.has(CardSuite.Spade)).toBe(true);
    });

    it('should have all 13 card numbers for each suit', () => {
      const deck = new Deck();
      
      for (const suite of Object.values(CardSuite)) {
        const cardsInSuite = deck.cards.filter(card => card.suite === suite);
        expect(cardsInSuite.length).toBe(13);
        
        const numbers = new Set(cardsInSuite.map(card => card.number));
        expect(numbers.size).toBe(13);
      }
    });

    it('should have exactly one card of each number and suite combination', () => {
      const deck = new Deck();
      
      for (const suite of Object.values(CardSuite)) {
        for (const number of Object.values(CardNumber)) {
          const matchingCards = deck.cards.filter(
            card => card.suite === suite && card.number === number
          );
          expect(matchingCards.length).toBe(1);
        }
      }
    });
  });

  describe('shuffle', () => {
    it('should change the order of cards', () => {
      // Use a predictable random number generator for testing
      let callCount = 0;
      const mockRandom = () => {
        callCount++;
        return 0.5; // Always return same value to make shuffle predictable
      };
      
      const deck = new Deck(mockRandom);
      const originalOrder = deck.cards.map(card => `${card.number}-${card.suite}`);
      
      deck.shuffle();
      
      const shuffledOrder = deck.cards.map(card => `${card.number}-${card.suite}`);
      
      // The order should be different (unless extremely unlucky)
      expect(shuffledOrder).not.toEqual(originalOrder);
      expect(callCount).toBeGreaterThan(0);
    });

    it('should not change the number of cards', () => {
      const deck = new Deck();
      const originalCount = deck.count;
      
      deck.shuffle();
      
      expect(deck.count).toBe(originalCount);
    });

    it('should preserve all cards', () => {
      const deck = new Deck();
      const originalCards = deck.cards.map(card => `${card.number}-${card.suite}`).sort();
      
      deck.shuffle();
      
      const shuffledCards = deck.cards.map(card => `${card.number}-${card.suite}`).sort();
      expect(shuffledCards).toEqual(originalCards);
    });
  });

  describe('dealCard', () => {
    it('should return and remove the top card', () => {
      const deck = new Deck();
      const originalCount = deck.count;
      const expectedCard = deck.cards[deck.cards.length - 1];
      
      const dealtCard = deck.dealCard();
      
      expect(dealtCard).toEqual(expectedCard);
      expect(deck.count).toBe(originalCount - 1);
    });

    it('should return null when deck is empty', () => {
      const deck = new Deck();
      // Empty the deck
      while (!deck.isEmpty()) {
        deck.dealCard();
      }
      
      const dealtCard = deck.dealCard();
      
      expect(dealtCard).toBeNull();
      expect(deck.count).toBe(0);
    });
  });

  describe('addCard', () => {
    it('should add a card to the deck', () => {
      const deck = new Deck();
      const originalCount = deck.count;
      const newCard = new Card(CardNumber.A, CardSuite.Heart);
      
      deck.addCard(newCard);
      
      expect(deck.count).toBe(originalCount + 1);
      expect(deck.cards[deck.cards.length - 1]).toEqual(newCard);
    });
  });

  describe('isEmpty', () => {
    it('should return false for a full deck', () => {
      const deck = new Deck();
      
      expect(deck.isEmpty()).toBe(false);
    });

    it('should return true for an empty deck', () => {
      const deck = new Deck();
      // Empty the deck
      while (deck.count > 0) {
        deck.dealCard();
      }
      
      expect(deck.isEmpty()).toBe(true);
    });
  });

  describe('clone', () => {
    it('should create a copy with the same cards', () => {
      const deck = new Deck();
      const clonedDeck = deck.clone();
      
      expect(clonedDeck.count).toBe(deck.count);
      
      for (let i = 0; i < deck.count; i++) {
        expect(clonedDeck.cards[i].equals(deck.cards[i])).toBe(true);
        // Ensure they are not the same object reference
        expect(clonedDeck.cards[i]).not.toBe(deck.cards[i]);
      }
    });

    it('should create independent copies', () => {
      const deck = new Deck();
      const clonedDeck = deck.clone();
      
      // Modify original deck
      deck.dealCard();
      
      // Clone should be unaffected
      expect(clonedDeck.count).toBe(52);
      expect(deck.count).toBe(51);
    });
  });
});