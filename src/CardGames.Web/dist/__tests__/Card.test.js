import { Card, CardNumber, CardSuite } from '../core/Card';
describe('Card', function () {
    describe('constructor', function () {
        it('should create a card with the specified number and suite', function () {
            var card = new Card(CardNumber.A, CardSuite.Heart);
            expect(card.number).toBe(CardNumber.A);
            expect(card.suite).toBe(CardSuite.Heart);
        });
    });
    describe('equals', function () {
        it('should return true for cards with same number and suite', function () {
            var card1 = new Card(CardNumber.A, CardSuite.Heart);
            var card2 = new Card(CardNumber.A, CardSuite.Heart);
            expect(card1.equals(card2)).toBe(true);
        });
        it('should return false for cards with different numbers', function () {
            var card1 = new Card(CardNumber.A, CardSuite.Heart);
            var card2 = new Card(CardNumber.K, CardSuite.Heart);
            expect(card1.equals(card2)).toBe(false);
        });
        it('should return false for cards with different suites', function () {
            var card1 = new Card(CardNumber.A, CardSuite.Heart);
            var card2 = new Card(CardNumber.A, CardSuite.Spade);
            expect(card1.equals(card2)).toBe(false);
        });
        it('should return false for null card', function () {
            var card = new Card(CardNumber.A, CardSuite.Heart);
            expect(card.equals(null)).toBe(false);
        });
    });
    describe('toString', function () {
        it('should return correct string representation', function () {
            var card = new Card(CardNumber.A, CardSuite.Heart);
            expect(card.toString()).toBe('A of Hearts');
        });
        it('should handle face cards correctly', function () {
            var jack = new Card(CardNumber.J, CardSuite.Club);
            var queen = new Card(CardNumber.Q, CardSuite.Diamond);
            var king = new Card(CardNumber.K, CardSuite.Spade);
            expect(jack.toString()).toBe('J of Clubs');
            expect(queen.toString()).toBe('Q of Diamonds');
            expect(king.toString()).toBe('K of Spades');
        });
    });
    describe('getNumericValue', function () {
        it('should return 1 for Ace', function () {
            var card = new Card(CardNumber.A, CardSuite.Heart);
            expect(card.getNumericValue()).toBe(1);
        });
        it('should return correct values for number cards', function () {
            expect(new Card(CardNumber._2, CardSuite.Heart).getNumericValue()).toBe(2);
            expect(new Card(CardNumber._5, CardSuite.Heart).getNumericValue()).toBe(5);
            expect(new Card(CardNumber._10, CardSuite.Heart).getNumericValue()).toBe(10);
        });
        it('should return correct values for face cards', function () {
            expect(new Card(CardNumber.J, CardSuite.Heart).getNumericValue()).toBe(11);
            expect(new Card(CardNumber.Q, CardSuite.Heart).getNumericValue()).toBe(12);
            expect(new Card(CardNumber.K, CardSuite.Heart).getNumericValue()).toBe(13);
        });
    });
    describe('isRed', function () {
        it('should return true for Hearts', function () {
            var card = new Card(CardNumber.A, CardSuite.Heart);
            expect(card.isRed()).toBe(true);
        });
        it('should return true for Diamonds', function () {
            var card = new Card(CardNumber.A, CardSuite.Diamond);
            expect(card.isRed()).toBe(true);
        });
        it('should return false for Clubs', function () {
            var card = new Card(CardNumber.A, CardSuite.Club);
            expect(card.isRed()).toBe(false);
        });
        it('should return false for Spades', function () {
            var card = new Card(CardNumber.A, CardSuite.Spade);
            expect(card.isRed()).toBe(false);
        });
    });
    describe('isBlack', function () {
        it('should return true for Clubs', function () {
            var card = new Card(CardNumber.A, CardSuite.Club);
            expect(card.isBlack()).toBe(true);
        });
        it('should return true for Spades', function () {
            var card = new Card(CardNumber.A, CardSuite.Spade);
            expect(card.isBlack()).toBe(true);
        });
        it('should return false for Hearts', function () {
            var card = new Card(CardNumber.A, CardSuite.Heart);
            expect(card.isBlack()).toBe(false);
        });
        it('should return false for Diamonds', function () {
            var card = new Card(CardNumber.A, CardSuite.Diamond);
            expect(card.isBlack()).toBe(false);
        });
    });
});
