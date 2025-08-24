import React from 'react';
import { SolitaireGame } from './components/SolitaireGame';

const App: React.FC = () => {
  return (
    <div className="App">
      <header className="App-header">
        <h1>CardGames.Web</h1>
        <p>ASP.NET Core + React implementation of card games</p>
      </header>
      <div className="game-container">
        <SolitaireGame />
      </div>
    </div>
  );
};

export default App;