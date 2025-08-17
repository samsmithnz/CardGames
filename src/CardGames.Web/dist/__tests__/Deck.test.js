import { Deck } from '../core/Deck';
import { Card, CardNumber, CardSuite } from '../core/Card';
describe('Deck', function () {
    describe('constructor', function () {
        it('should create a deck with 52 cards', function () {
            var deck = new Deck();
            expect(deck.count).toBe(52);
        });
        it('should have all 4 suits', function () {
            var deck = new Deck();
            var suites = new Set(deck.cards.map(function (card) { return card.suite; }));
            expect(suites.size).toBe(4);
            expect(suites.has(CardSuite.Heart)).toBe(true);
            expect(suites.has(CardSuite.Club)).toBe(true);
            expect(suites.has(CardSuite.Diamond)).toBe(true);
            expect(suites.has(CardSuite.Spade)).toBe(true);
        });
        it('should have all 13 card numbers for each suit', function () {
            var deck = new Deck();
            var _loop_1 = function (suite) {
                var cardsInSuite = deck.cards.filter(function (card) { return card.suite === suite; });
                expect(cardsInSuite.length).toBe(13);
                var numbers = new Set(cardsInSuite.map(function (card) { return card.number; }));
                expect(numbers.size).toBe(13);
            };
            for (var _i = 0, _a = Object.values(CardSuite); _i < _a.length; _i++) {
                var suite = _a[_i];
                _loop_1(suite);
            }
        });
        it('should have exactly one card of each number and suite combination', function () {
            var deck = new Deck();
            var _loop_2 = function (suite) {
                var _loop_3 = function (number) {
                    var matchingCards = deck.cards.filter(function (card) { return card.suite === suite && card.number === number; });
                    expect(matchingCards.length).toBe(1);
                };
                for (var _b = 0, _c = Object.values(CardNumber); _b < _c.length; _b++) {
                    var number = _c[_b];
                    _loop_3(number);
                }
            };
            for (var _i = 0, _a = Object.values(CardSuite); _i < _a.length; _i++) {
                var suite = _a[_i];
                _loop_2(suite);
            }
        });
    });
    describe('shuffle', function () {
        it('should change the order of cards', function () {
            // Use a predictable random number generator for testing
            var callCount = 0;
            var mockRandom = function () {
                callCount++;
                return 0.5; // Always return same value to make shuffle predictable
            };
            var deck = new Deck(mockRandom);
            var originalOrder = deck.cards.map(function (card) { return "".concat(card.number, "-").concat(card.suite); });
            deck.shuffle();
            var shuffledOrder = deck.cards.map(function (card) { return "".concat(card.number, "-").concat(card.suite); });
            // The order should be different (unless extremely unlucky)
            expect(shuffledOrder).not.toEqual(originalOrder);
            expect(callCount).toBeGreaterThan(0);
        });
        it('should not change the number of cards', function () {
            var deck = new Deck();
            var originalCount = deck.count;
            deck.shuffle();
            expect(deck.count).toBe(originalCount);
        });
        it('should preserve all cards', function () {
            var deck = new Deck();
            var originalCards = deck.cards.map(function (card) { return "".concat(card.number, "-").concat(card.suite); }).sort();
            deck.shuffle();
            var shuffledCards = deck.cards.map(function (card) { return "".concat(card.number, "-").concat(card.suite); }).sort();
            expect(shuffledCards).toEqual(originalCards);
        });
    });
    describe('dealCard', function () {
        it('should return and remove the top card', function () {
            var deck = new Deck();
            var originalCount = deck.count;
            var expectedCard = deck.cards[deck.cards.length - 1];
            var dealtCard = deck.dealCard();
            expect(dealtCard).toEqual(expectedCard);
            expect(deck.count).toBe(originalCount - 1);
        });
        it('should return null when deck is empty', function () {
            var deck = new Deck();
            // Empty the deck
            while (!deck.isEmpty()) {
                deck.dealCard();
            }
            var dealtCard = deck.dealCard();
            expect(dealtCard).toBeNull();
            expect(deck.count).toBe(0);
        });
    });
    describe('addCard', function () {
        it('should add a card to the deck', function () {
            var deck = new Deck();
            var originalCount = deck.count;
            var newCard = new Card(CardNumber.A, CardSuite.Heart);
            deck.addCard(newCard);
            expect(deck.count).toBe(originalCount + 1);
            expect(deck.cards[deck.cards.length - 1]).toEqual(newCard);
        });
    });
    describe('isEmpty', function () {
        it('should return false for a full deck', function () {
            var deck = new Deck();
            expect(deck.isEmpty()).toBe(false);
        });
        it('should return true for an empty deck', function () {
            var deck = new Deck();
            // Empty the deck
            while (deck.count > 0) {
                deck.dealCard();
            }
            expect(deck.isEmpty()).toBe(true);
        });
    });
    describe('clone', function () {
        it('should create a copy with the same cards', function () {
            var deck = new Deck();
            var clonedDeck = deck.clone();
            expect(clonedDeck.count).toBe(deck.count);
            for (var i = 0; i < deck.count; i++) {
                expect(clonedDeck.cards[i].equals(deck.cards[i])).toBe(true);
                // Ensure they are not the same object reference
                expect(clonedDeck.cards[i]).not.toBe(deck.cards[i]);
            }
        });
        it('should create independent copies', function () {
            var deck = new Deck();
            var clonedDeck = deck.clone();
            // Modify original deck
            deck.dealCard();
            // Clone should be unaffected
            expect(clonedDeck.count).toBe(52);
            expect(deck.count).toBe(51);
        });
    });
});
