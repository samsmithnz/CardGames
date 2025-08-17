import { Card, CardNumber, CardSuite } from './Card';
/**
 * Represents a deck of playing cards
 */
var Deck = /** @class */ (function () {
    /**
     * Initializes a new instance of the Deck class with a standard 52-card deck
     * @param randomGenerator Optional random number generator for testing purposes
     */
    function Deck(randomGenerator) {
        this.rng = randomGenerator || Math.random;
        // Add all of the cards to the deck
        this.cards = [];
        for (var _i = 0, _a = Object.values(CardSuite); _i < _a.length; _i++) {
            var suite = _a[_i];
            for (var _b = 0, _c = Object.values(CardNumber); _b < _c.length; _b++) {
                var number = _c[_b];
                this.cards.push(new Card(number, suite));
            }
        }
    }
    /**
     * Shuffles the cards in the deck using the Fisher-Yates shuffle algorithm
     */
    Deck.prototype.shuffle = function () {
        var n = this.cards.length;
        while (n > 1) {
            n--;
            var k = Math.floor(this.rng() * (n + 1));
            var value = this.cards[k];
            this.cards[k] = this.cards[n];
            this.cards[n] = value;
        }
    };
    Object.defineProperty(Deck.prototype, "count", {
        /**
         * Gets the number of cards in the deck
         */
        get: function () {
            return this.cards.length;
        },
        enumerable: false,
        configurable: true
    });
    /**
     * Deals (removes and returns) the top card from the deck
     * @returns The top card, or null if the deck is empty
     */
    Deck.prototype.dealCard = function () {
        if (this.cards.length === 0) {
            return null;
        }
        return this.cards.pop() || null;
    };
    /**
     * Adds a card to the top of the deck
     * @param card The card to add
     */
    Deck.prototype.addCard = function (card) {
        this.cards.push(card);
    };
    /**
     * Checks if the deck is empty
     */
    Deck.prototype.isEmpty = function () {
        return this.cards.length === 0;
    };
    /**
     * Returns a copy of the deck
     */
    Deck.prototype.clone = function () {
        var newDeck = new Deck(this.rng);
        newDeck.cards = this.cards.map(function (card) { return new Card(card.number, card.suite); });
        return newDeck;
    };
    return Deck;
}());
export { Deck };
