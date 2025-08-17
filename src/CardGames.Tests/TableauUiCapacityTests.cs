using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using CardGames;
using CardGames.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardGames.Tests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class TableauUiCapacityTests
    {
        [TestMethod]
        public void RefreshTableauColumn_WhenColumnExceedsInitialUiControls_ShouldCreateMoreControlsAndShowAllCards()
        {
            Exception threadException = null;
            ManualResetEvent done = new ManualResetEvent(false);

            Thread t = new Thread(() =>
            {
                try
                {
                    // Arrange: create window (runs InitializeGame and deals a new game)
                    MainWindow window = new MainWindow();

                    // Access private rules field
                    FieldInfo rulesField = typeof(MainWindow).GetField("solitaireRules", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.IsNotNull(rulesField, "Could not access solitaireRules field via reflection");
                    SolitaireRules rules = (SolitaireRules)rulesField.GetValue(window);
                    Assert.IsNotNull(rules, "Rules instance should not be null");

                    // Force tableau column 0 to have more cards than initial placeholders (column 0 initially has 1 control)
                    int requiredCards = 6;
                    rules.TableauColumns[0].Clear();
                    for (int i = 0; i < requiredCards; i++)
                    {
                        Card c = new Card
                        {
                            Number = Card.CardNumber._10, // arbitrary
                            Suite = (i % 2 == 0) ? Card.CardSuite.Heart : Card.CardSuite.Club
                        };
                        rules.TableauColumns[0].Add(c);
                    }

                    // Act: invoke private RefreshTableauColumn(0)
                    MethodInfo refreshMethod = typeof(MainWindow).GetMethod("RefreshTableauColumn", BindingFlags.Instance | BindingFlags.NonPublic);
                    Assert.IsNotNull(refreshMethod, "Could not access RefreshTableauColumn method via reflection");
                    refreshMethod.Invoke(window, new object[] { 0 });

                    // Get the Canvas by name and inspect its Children via reflection to avoid WPF compile-time deps
                    object canvas = window.FindName("TableauColumn1");
                    Assert.IsNotNull(canvas, "TableauColumn1 Canvas should be found by name");

                    PropertyInfo childrenProp = canvas.GetType().GetProperty("Children");
                    Assert.IsNotNull(childrenProp, "Canvas.Children property not found");
                    object children = childrenProp.GetValue(canvas);
                    PropertyInfo countProp = children.GetType().GetProperty("Count");
                    Assert.IsNotNull(countProp, "Children.Count property not found");
                    int childCount = (int)countProp.GetValue(children);

                    // There should be at least as many child controls as cards
                    Assert.IsTrue(childCount >= requiredCards, $"Canvas should have at least {requiredCards} children, had {childCount}");

                    // Count visible children cards (Visibility == Visible)
                    int visible = 0;
                    PropertyInfo indexer = children.GetType().GetProperty("Item");
                    for (int i = 0; i < childCount; i++)
                    {
                        object child = indexer.GetValue(children, new object[] { i });
                        if (child == null)
                        {
                            continue;
                        }
                        PropertyInfo visibilityProp = child.GetType().GetProperty("Visibility");
                        if (visibilityProp != null)
                        {
                            object vis = visibilityProp.GetValue(child);
                            if (vis != null && vis.ToString() == "Visible")
                            {
                                visible++;
                            }
                        }
                    }

                    Assert.AreEqual(requiredCards, visible, "All cards in the column should be visible after refresh");
                }
                catch (Exception ex)
                {
                    threadException = ex;
                }
                finally
                {
                    done.Set();
                }
            });

            t.IsBackground = true;
            t.SetApartmentState(ApartmentState.STA); // WPF requires STA
            t.Start();
            done.WaitOne(TimeSpan.FromSeconds(10));

            if (threadException != null)
            {
                throw threadException;
            }
        }
    }
}
