import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { SolitaireGame } from '../components/SolitaireGame';

describe('SolitaireGame Component', () => {
  it('should render initial state correctly', () => {
    render(<SolitaireGame />);
    
    expect(screen.getByText('Card Games Web - Solitaire Demo')).toBeInTheDocument();
    expect(screen.getByText('Click "New Game" to start playing!')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'New Game' })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Draw Card' })).toBeDisabled();
  });

  it('should start a new game when New Game button is clicked', () => {
    render(<SolitaireGame />);
    
    const newGameButton = screen.getByRole('button', { name: 'New Game' });
    fireEvent.click(newGameButton);
    
    expect(screen.getByText('Game started! This is a basic demo of the TypeScript game engine.')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Draw Card' })).toBeEnabled();
    expect(screen.getByText('Game Statistics')).toBeInTheDocument();
  });

  it('should display game statistics after starting a game', () => {
    render(<SolitaireGame />);
    
    const newGameButton = screen.getByRole('button', { name: 'New Game' });
    fireEvent.click(newGameButton);
    
    // Check that game statistics are displayed
    expect(screen.getByText('Cards in foundation piles: 0/52')).toBeInTheDocument();
    expect(screen.getByText(/Cards in stock pile:/)).toBeInTheDocument();
    expect(screen.getByText(/Cards in waste pile:/)).toBeInTheDocument();
    expect(screen.getByText('Game won: No')).toBeInTheDocument();
  });

  it('should draw cards when Draw Card button is clicked', () => {
    render(<SolitaireGame />);
    
    // Start a new game first
    fireEvent.click(screen.getByRole('button', { name: 'New Game' }));
    
    // Draw a card
    const drawButton = screen.getByRole('button', { name: 'Draw Card' });
    fireEvent.click(drawButton);
    
    // Should show some feedback about drawing a card
    expect(screen.getByText(/Card drawn from stock to waste pile|Stock empty - reset from waste pile/)).toBeInTheDocument();
  });

  it('should display tableau and foundation information', () => {
    render(<SolitaireGame />);
    
    fireEvent.click(screen.getByRole('button', { name: 'New Game' }));
    
    expect(screen.getByText('Tableau Columns:')).toBeInTheDocument();
    expect(screen.getByText('Foundation Piles:')).toBeInTheDocument();
    
    // Should show all 7 tableau columns
    for (let i = 1; i <= 7; i++) {
      expect(screen.getByText(new RegExp(`Column ${i}:`))).toBeInTheDocument();
    }
    
    // Should show all 4 foundation piles
    for (let i = 1; i <= 4; i++) {
      expect(screen.getByText(new RegExp(`Foundation ${i}:`))).toBeInTheDocument();
    }
  });

  it('should display features list', () => {
    render(<SolitaireGame />);
    
    expect(screen.getByText('Features Demonstrated:')).toBeInTheDocument();
    expect(screen.getByText(/Complete TypeScript translation of C# game logic/)).toBeInTheDocument();
    expect(screen.getByText(/Full test coverage/)).toBeInTheDocument();
    expect(screen.getByText(/React integration with game engine/)).toBeInTheDocument();
  });
});