import { Card, CardNumber, CardSuite } from './Card';
/**
 * Implements the rules and logic for Klondike Solitaire card game
 */
var SolitaireRules = /** @class */ (function () {
    /**
     * Initializes a new Solitaire game with empty game state
     */
    function SolitaireRules() {
        this.tableauColumns = [];
        for (var i = 0; i < 7; i++) {
            this.tableauColumns.push([]);
        }
        this.foundationPiles = [];
        for (var i = 0; i < 4; i++) {
            this.foundationPiles.push([]);
        }
        this.stockPile = [];
        this.wastePile = [];
    }
    /**
     * Sets up a new Solitaire game with the provided deck
     * @param deck The shuffled deck to deal from
     */
    SolitaireRules.prototype.dealCards = function (deck) {
        if (!deck || deck.count !== 52) {
            throw new Error('Deck must contain exactly 52 cards');
        }
        // Clear existing game state
        for (var _i = 0, _a = this.tableauColumns; _i < _a.length; _i++) {
            var column = _a[_i];
            column.length = 0;
        }
        for (var _b = 0, _c = this.foundationPiles; _b < _c.length; _b++) {
            var foundation = _c[_b];
            foundation.length = 0;
        }
        this.stockPile.length = 0;
        this.wastePile.length = 0;
        var cardIndex = 0;
        // Deal cards to tableau columns (1 to first column, 2 to second, etc.)
        for (var column = 0; column < 7; column++) {
            for (var card = 0; card <= column; card++) {
                this.tableauColumns[column].push(deck.cards[cardIndex]);
                cardIndex++;
            }
        }
        // Remaining cards go to stock pile
        for (var i = cardIndex; i < deck.cards.length; i++) {
            this.stockPile.push(deck.cards[i]);
        }
    };
    /**
     * Checks if a card can be placed on a tableau column
     * @param card The card to place
     * @param columnIndex The tableau column index (0-6)
     * @returns True if the move is valid, false otherwise
     */
    SolitaireRules.prototype.canPlaceCardOnTableau = function (card, columnIndex) {
        if (columnIndex < 0 || columnIndex >= 7 || !card) {
            return false;
        }
        var column = this.tableauColumns[columnIndex];
        // Empty column can only accept Kings
        if (column.length === 0) {
            return card.number === CardNumber.K;
        }
        // Get the top card of the column
        var topCard = column[column.length - 1];
        // Check if card is one rank lower and opposite color
        return this.isOneRankLower(card.number, topCard.number) && this.isOppositeColor(card, topCard);
    };
    /**
     * Checks if a card can be placed on a foundation pile
     * @param card The card to place
     * @param foundationIndex The foundation pile index (0-3)
     * @returns True if the move is valid, false otherwise
     */
    SolitaireRules.prototype.canPlaceCardOnFoundation = function (card, foundationIndex) {
        if (foundationIndex < 0 || foundationIndex >= 4 || !card) {
            return false;
        }
        var foundation = this.foundationPiles[foundationIndex];
        // Empty foundation can only accept Aces
        if (foundation.length === 0) {
            return card.number === CardNumber.A;
        }
        // Get the top card of the foundation
        var topCard = foundation[foundation.length - 1];
        // Check if card is same suit and one rank higher
        return card.suite === topCard.suite && this.isOneRankHigher(card.number, topCard.number);
    };
    /**
     * Checks if the game has been won (all cards moved to foundations)
     * @returns True if the game is won, false otherwise
     */
    SolitaireRules.prototype.isGameWon = function () {
        for (var _i = 0, _a = this.foundationPiles; _i < _a.length; _i++) {
            var foundation = _a[_i];
            if (foundation.length !== 13) {
                return false;
            }
        }
        return true;
    };
    /**
     * Finds the foundation pile for a card's specific suite
     * @param card The card to place
     * @returns The foundation index (0-3) if a valid placement is found, -1 otherwise
     */
    SolitaireRules.prototype.findAvailableFoundationPile = function (card) {
        if (!card) {
            return -1;
        }
        // Each suite maps to a specific foundation pile based on the enum index
        var suiteFoundationIndex = this.getSuiteIndex(card.suite);
        if (this.canPlaceCardOnFoundation(card, suiteFoundationIndex)) {
            return suiteFoundationIndex;
        }
        return -1;
    };
    /**
     * Draws a card from the stock pile to the waste pile
     * @returns True if a card was drawn, false if stock pile is empty
     */
    SolitaireRules.prototype.drawFromStock = function () {
        if (this.stockPile.length === 0) {
            return false;
        }
        var drawnCard = this.stockPile.pop();
        this.wastePile.push(drawnCard);
        return true;
    };
    /**
     * Resets the waste pile back to the stock pile when stock is empty
     */
    SolitaireRules.prototype.resetStock = function () {
        if (this.stockPile.length === 0 && this.wastePile.length > 0) {
            // Move all waste cards back to stock in reverse order
            for (var i = this.wastePile.length - 1; i >= 0; i--) {
                this.stockPile.push(this.wastePile[i]);
            }
            this.wastePile.length = 0;
        }
    };
    /**
     * Checks if one card number is exactly one rank lower than another
     */
    SolitaireRules.prototype.isOneRankLower = function (lower, higher) {
        return this.getCardNumberValue(lower) === this.getCardNumberValue(higher) - 1;
    };
    /**
     * Checks if one card number is exactly one rank higher than another
     */
    SolitaireRules.prototype.isOneRankHigher = function (higher, lower) {
        return this.getCardNumberValue(higher) === this.getCardNumberValue(lower) + 1;
    };
    /**
     * Checks if two cards are opposite colors (red vs black)
     */
    SolitaireRules.prototype.isOppositeColor = function (card1, card2) {
        return card1.isRed() !== card2.isRed();
    };
    /**
     * Gets the numeric value for card comparison (A=1, 2=2, ..., K=13)
     */
    SolitaireRules.prototype.getCardNumberValue = function (number) {
        var card = new Card(number, CardSuite.Heart); // Suite doesn't matter for value
        return card.getNumericValue();
    };
    /**
     * Gets the index for a card suite (for foundation pile mapping)
     */
    SolitaireRules.prototype.getSuiteIndex = function (suite) {
        var suites = Object.values(CardSuite);
        return suites.indexOf(suite);
    };
    return SolitaireRules;
}());
export { SolitaireRules };
