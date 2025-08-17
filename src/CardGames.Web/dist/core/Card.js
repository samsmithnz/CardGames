/**
 * Represents the rank/number of a playing card
 */
export var CardNumber;
(function (CardNumber) {
    CardNumber["A"] = "A";
    CardNumber["_2"] = "2";
    CardNumber["_3"] = "3";
    CardNumber["_4"] = "4";
    CardNumber["_5"] = "5";
    CardNumber["_6"] = "6";
    CardNumber["_7"] = "7";
    CardNumber["_8"] = "8";
    CardNumber["_9"] = "9";
    CardNumber["_10"] = "10";
    CardNumber["J"] = "J";
    CardNumber["Q"] = "Q";
    CardNumber["K"] = "K";
})(CardNumber || (CardNumber = {}));
/**
 * Represents the suit of a playing card
 */
export var CardSuite;
(function (CardSuite) {
    CardSuite["Heart"] = "Heart";
    CardSuite["Club"] = "Club";
    CardSuite["Diamond"] = "Diamond";
    CardSuite["Spade"] = "Spade";
})(CardSuite || (CardSuite = {}));
/**
 * Represents a playing card with a number/rank and suit
 */
var Card = /** @class */ (function () {
    function Card(number, suite) {
        this.number = number;
        this.suite = suite;
    }
    /**
     * Checks if this card equals another card
     */
    Card.prototype.equals = function (other) {
        if (other === null || other === undefined) {
            return false;
        }
        return this.number === other.number && this.suite === other.suite;
    };
    /**
     * Returns a string representation of the card
     */
    Card.prototype.toString = function () {
        return "".concat(this.number, " of ").concat(this.suite, "s");
    };
    /**
     * Gets the numeric value of the card for comparison (Ace = 1, Jack = 11, Queen = 12, King = 13)
     */
    Card.prototype.getNumericValue = function () {
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
    };
    /**
     * Checks if this card is red (Hearts or Diamonds)
     */
    Card.prototype.isRed = function () {
        return this.suite === CardSuite.Heart || this.suite === CardSuite.Diamond;
    };
    /**
     * Checks if this card is black (Clubs or Spades)
     */
    Card.prototype.isBlack = function () {
        return this.suite === CardSuite.Club || this.suite === CardSuite.Spade;
    };
    return Card;
}());
export { Card };
