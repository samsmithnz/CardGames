import { Card } from './Card';
import { Deck } from './Deck';
/**
 * Implements the rules and logic for Klondike Solitaire card game
 */
export declare class SolitaireRules {
    /**
     * The seven tableau columns where cards are initially dealt and can be built in descending order with alternating colors
     */
    tableauColumns: Card[][];
    /**
     * The four foundation piles where cards are built up by suit from Ace to King
     */
    foundationPiles: Card[][];
    /**
     * The stock pile containing face-down cards that can be drawn
     */
    stockPile: Card[];
    /**
     * The waste pile containing face-up cards drawn from the stock
     */
    wastePile: Card[];
    /**
     * Initializes a new Solitaire game with empty game state
     */
    constructor();
    /**
     * Sets up a new Solitaire game with the provided deck
     * @param deck The shuffled deck to deal from
     */
    dealCards(deck: Deck): void;
    /**
     * Checks if a card can be placed on a tableau column
     * @param card The card to place
     * @param columnIndex The tableau column index (0-6)
     * @returns True if the move is valid, false otherwise
     */
    canPlaceCardOnTableau(card: Card, columnIndex: number): boolean;
    /**
     * Checks if a card can be placed on a foundation pile
     * @param card The card to place
     * @param foundationIndex The foundation pile index (0-3)
     * @returns True if the move is valid, false otherwise
     */
    canPlaceCardOnFoundation(card: Card, foundationIndex: number): boolean;
    /**
     * Checks if the game has been won (all cards moved to foundations)
     * @returns True if the game is won, false otherwise
     */
    isGameWon(): boolean;
    /**
     * Finds the foundation pile for a card's specific suite
     * @param card The card to place
     * @returns The foundation index (0-3) if a valid placement is found, -1 otherwise
     */
    findAvailableFoundationPile(card: Card): number;
    /**
     * Draws a card from the stock pile to the waste pile
     * @returns True if a card was drawn, false if stock pile is empty
     */
    drawFromStock(): boolean;
    /**
     * Resets the waste pile back to the stock pile when stock is empty
     */
    resetStock(): void;
    /**
     * Checks if one card number is exactly one rank lower than another
     */
    private isOneRankLower;
    /**
     * Checks if one card number is exactly one rank higher than another
     */
    private isOneRankHigher;
    /**
     * Checks if two cards are opposite colors (red vs black)
     */
    private isOppositeColor;
    /**
     * Gets the numeric value for card comparison (A=1, 2=2, ..., K=13)
     */
    private getCardNumberValue;
    /**
     * Gets the index for a card suite (for foundation pile mapping)
     */
    private getSuiteIndex;
}
