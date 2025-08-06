using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames.Core
{
    public class SolitaireRules
    {
        Card.CardNumber[] CardsThatCanBePlacedOnPanel = new Card.CardNumber[] { Card.CardNumber.K };
        bool CardsDescendingOrder = true;
        bool CardsDescendingOrderAlternateSuite = true;
        bool CardsAscendingOrder = true;
        bool CardsAscendingOrderAlternateSuite = false;
    }
}
