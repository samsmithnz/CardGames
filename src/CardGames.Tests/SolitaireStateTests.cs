using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class SolitaireStateTests
    {
        [TestMethod]
        public void ExportAndImportState_Roundtrip_ShouldPreserveAllPiles()
        {
            // Arrange
            SolitaireRules rules = new SolitaireRules();

            // Populate a small custom state
            rules.StockPile.Add(new Card { Number = Card.CardNumber._9, Suite = Card.CardSuite.Club });
            rules.WastePile.Add(new Card { Number = Card.CardNumber.J, Suite = Card.CardSuite.Heart });
            rules.FoundationPiles[0].Add(new Card { Number = Card.CardNumber.A, Suite = Card.CardSuite.Spade });
            rules.TableauColumns[0].Add(new Card { Number = Card.CardNumber.K, Suite = Card.CardSuite.Diamond });

            // Act
            string json = rules.ExportState("test").ToJson();

            SolitaireRules rules2 = new SolitaireRules();
            rules2.ImportState(SolitaireState.FromJson(json));

            // Assert basic preservation
            Assert.AreEqual(1, rules2.StockPile.Count);
            Assert.AreEqual(1, rules2.WastePile.Count);
            Assert.AreEqual(1, rules2.FoundationPiles[0].Count);
            Assert.AreEqual(1, rules2.TableauColumns[0].Count);

            Assert.AreEqual(Card.CardNumber._9, rules2.StockPile[0].Number);
            Assert.AreEqual(Card.CardSuite.Club, rules2.StockPile[0].Suite);
        }
    }
}
