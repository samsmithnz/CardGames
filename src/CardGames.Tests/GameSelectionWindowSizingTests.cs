using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Markup;
using System.IO;
using System.Xml;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify GameSelectionWindow has consistent sizing
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class GameSelectionWindowSizingTests
    {
        /// <summary>
        /// Test that GameSelectionWindow XAML specifies the correct height
        /// </summary>
        [TestMethod]
        public void GameSelectionWindow_XamlDefinition_ShouldHaveCorrectHeight()
        {
            // Arrange - Load and parse the XAML file
            string xamlPath = Path.Combine("..", "..", "..", "..", "CardGames", "GameSelectionWindow.xaml");
            string xamlContent = File.ReadAllText(xamlPath);
            
            // Act - Parse XAML to verify Height attribute
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xamlContent);
            
            // Assert - Verify the Window element has Height="336"
            XmlNode windowNode = doc.DocumentElement;
            Assert.IsNotNull(windowNode, "Should find Window element");
            
            XmlAttribute heightAttribute = windowNode.Attributes["Height"];
            Assert.IsNotNull(heightAttribute, "Should have Height attribute");
            Assert.AreEqual("336", heightAttribute.Value, "Height should be exactly 336");
        }
        
        /// <summary>
        /// Test that GameSelectionWindow XAML has SizeToContent="Manual" for consistent sizing
        /// </summary>
        [TestMethod]
        public void GameSelectionWindow_XamlDefinition_ShouldHaveSizeToContentManual()
        {
            // Arrange - Load and parse the XAML file
            string xamlPath = Path.Combine("..", "..", "..", "..", "CardGames", "GameSelectionWindow.xaml");
            string xamlContent = File.ReadAllText(xamlPath);
            
            // Act - Parse XAML to verify SizeToContent attribute
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xamlContent);
            
            // Assert - Verify the Window element has SizeToContent="Manual"
            XmlNode windowNode = doc.DocumentElement;
            Assert.IsNotNull(windowNode, "Should find Window element");
            
            XmlAttribute sizeToContentAttribute = windowNode.Attributes["SizeToContent"];
            Assert.IsNotNull(sizeToContentAttribute, "Should have SizeToContent attribute");
            Assert.AreEqual("Manual", sizeToContentAttribute.Value, "SizeToContent should be Manual to ensure fixed height");
        }
        
        /// <summary>
        /// Test that GameSelectionWindow Grid uses fixed heights instead of star sizing
        /// </summary>
        [TestMethod]
        public void GameSelectionWindow_GridRowDefinitions_ShouldUseFixedHeights()
        {
            // Arrange - Load and parse the XAML file
            string xamlPath = Path.Combine("..", "..", "..", "..", "CardGames", "GameSelectionWindow.xaml");
            string xamlContent = File.ReadAllText(xamlPath);
            
            // Act - Parse XAML to verify Grid.RowDefinitions
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xamlContent);
            
            // Create namespace manager for XAML parsing
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            
            // Find all RowDefinition elements
            XmlNodeList rowDefinitions = doc.SelectNodes("//x:RowDefinition", nsManager);
            Assert.IsTrue(rowDefinitions.Count >= 4, "Should have at least 4 row definitions");
            
            // Assert - Check that no row definition uses Height="*" 
            foreach (XmlNode rowDef in rowDefinitions)
            {
                XmlAttribute heightAttr = rowDef.Attributes["Height"];
                if (heightAttr != null)
                {
                    Assert.AreNotEqual("*", heightAttr.Value, 
                        "No row definition should use Height='*' as it can cause inconsistent sizing");
                }
            }
        }
    }
}