using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;

namespace CardGames.Tests
{
    /// <summary>
    /// Tests to verify MainWindow free cells have consistent sizing with other card areas
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class MainWindowFreeCellSizingTests
    {
        /// <summary>
        /// Test that all free cell borders have the same dimensions as foundation/stock/waste piles (90x130)
        /// </summary>
        [TestMethod]
        public void MainWindow_FreeCellBorders_ShouldHaveConsistentSizeWithOtherCardAreas()
        {
            // Arrange - Load and parse the XAML file
            string xamlPath = Path.Combine("..", "..", "..", "..", "CardGames", "MainWindow.xaml");
            string xamlContent = File.ReadAllText(xamlPath);
            
            // Act & Assert - Check that all free cell borders have Width="90" Height="130"
            for (int i = 1; i <= 4; i++)
            {
                string borderName = $"FreeCellBorder{i}";
                Assert.IsTrue(xamlContent.Contains($"x:Name=\"{borderName}\""), 
                    $"Should find {borderName} element in XAML");
                
                // Find the border line and verify it has the correct dimensions
                string[] lines = xamlContent.Split('\n');
                string borderLine = null;
                foreach (string line in lines)
                {
                    if (line.Contains($"x:Name=\"{borderName}\""))
                    {
                        borderLine = line;
                        break;
                    }
                }
                
                Assert.IsNotNull(borderLine, $"Should find line containing {borderName}");
                Assert.IsTrue(borderLine.Contains("Width=\"90\""), 
                    $"{borderName} should have Width='90' to match other card areas");
                Assert.IsTrue(borderLine.Contains("Height=\"130\""), 
                    $"{borderName} should have Height='130' to match other card areas");
            }
        }
        
        /// <summary>
        /// Test that foundation pile borders have the expected dimensions (90x130) for comparison
        /// </summary>
        [TestMethod]
        public void MainWindow_FoundationBorders_ShouldHave90x130Dimensions()
        {
            // Arrange - Load and parse the XAML file
            string xamlPath = Path.Combine("..", "..", "..", "..", "CardGames", "MainWindow.xaml");
            string xamlContent = File.ReadAllText(xamlPath);
            
            // Act & Assert - Check foundation pile borders for comparison
            string[] lines = xamlContent.Split('\n');
            int foundationBorderCount = 0;
            
            foreach (string line in lines)
            {
                if (line.Contains("x:Name=\"Foundation") && line.Contains("Border"))
                {
                    // This is a line that references a foundation control, check its parent border
                    continue;
                }
                
                // Look for foundation pile border lines (they come before the Foundation controls)
                if (line.Contains("Canvas.Left=\"850\"") || line.Contains("Canvas.Left=\"960\"") || 
                    line.Contains("Canvas.Left=\"1070\"") || line.Contains("Canvas.Left=\"1180\""))
                {
                    if (line.Contains("Width=\"90\"") && line.Contains("Height=\"130\""))
                    {
                        foundationBorderCount++;
                    }
                }
            }
            
            Assert.AreEqual(4, foundationBorderCount, "Should find 4 foundation borders with 90x130 dimensions");
        }
        
        /// <summary>
        /// Test that free cell labels have proper positioning and width to match the new border size
        /// </summary>
        [TestMethod]
        public void MainWindow_FreeCellLabels_ShouldHaveConsistentWidthWithBorders()
        {
            // Arrange - Load and parse the XAML file
            string xamlPath = Path.Combine("..", "..", "..", "..", "CardGames", "MainWindow.xaml");
            string xamlContent = File.ReadAllText(xamlPath);
            
            // Act & Assert - Check that all free cell labels have Width="90"
            for (int i = 1; i <= 4; i++)
            {
                string labelName = $"FreeCellLabel{i}";
                Assert.IsTrue(xamlContent.Contains($"x:Name=\"{labelName}\""), 
                    $"Should find {labelName} element in XAML");
                
                // Find the section containing the label and verify it has the correct width
                int labelIndex = xamlContent.IndexOf($"x:Name=\"{labelName}\"");
                Assert.IsTrue(labelIndex >= 0, $"Should find {labelName} in XAML");
                
                // Get a reasonable section around the label to check for Width="90"
                int startIndex = Math.Max(0, labelIndex - 100);
                int endIndex = Math.Min(xamlContent.Length - 1, labelIndex + 200);
                string labelSection = xamlContent.Substring(startIndex, endIndex - startIndex);
                
                Assert.IsTrue(labelSection.Contains("Width=\"90\""), 
                    $"{labelName} should have Width='90' to match border width. Section: {labelSection}");
            }
        }
    }
}