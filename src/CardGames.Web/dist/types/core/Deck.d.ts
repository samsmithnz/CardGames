import { Card } from './Card';
/**
 * Represents a deck of playing cards
 */
export declare class Deck {
    /**
     * Gets the list of cards in the deck
     */
    cards: Card[];
    private rng;
    /**
     * Initializes a new instance of the Deck class with a standard 52-card deck
     * @param randomGenerator Optional random number generator for testing purposes
     */
    constructor(randomGenerator?: () => number);
    /**
     * Shuffles the cards in the deck using the Fisher-Yates shuffle algorithm
     */
    shuffle(): void;
    /**
     * Gets the number of cards in the deck
     */
    get count(): number;
    /**
     * Deals (removes and returns) the top card from the deck
     * @returns The top card, or null if the deck is empty
     */
    dealCard(): Card | null;
    /**
     * Adds a card to the top of the deck
     * @param card The card to add
     */
    addCard(card: Card): void;
    /**
     * Checks if the deck is empty
     */
    isEmpty(): boolean;
    /**
     * Returns a copy of the deck
     */
    clone(): Deck;
}
