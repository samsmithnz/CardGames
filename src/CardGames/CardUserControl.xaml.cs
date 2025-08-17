using CardGames.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CardGames
{
    /// <summary>
    /// Interaction logic for CardUserControl.xaml
    /// </summary>
    public partial class CardUserControl : UserControl
    {
        private Card _card;
        public Card Card 
        { 
            get { return _card; }
            set 
            { 
                _card = value;
                // Update visibility when card changes
                UpdateVisibility();
            }
        }

        /// <summary>
        /// Event raised when a card is being dragged from this control
        /// </summary>
        public event EventHandler<Card> CardDragStarted;

        /// <summary>
        /// Event raised when a card is dropped onto this control
        /// </summary>
        public event EventHandler<CardDropEventArgs> CardDropped;

        /// <summary>
        /// Event raised to validate if a drop is allowed
        /// </summary>
        public event EventHandler<ValidateDropEventArgs> ValidateDrop;

        /// <summary>
        /// Event raised when a card is clicked (not dragged)
        /// </summary>
        public event EventHandler<Card> CardClicked;

        /// <summary>
        /// Event raised when the stock pile card is clicked to draw a card
        /// </summary>
        public event EventHandler<EventArgs> StockPileClicked;

        /// <summary>
        /// Indicates if this card control represents the stock pile
        /// </summary>
        public bool IsStockPile { get; set; }

        /// <summary>
        /// Gets or sets whether this is a tableau drop target for empty columns
        /// </summary>
        public bool IsTableauDropTarget { get; set; }

        /// <summary>
        /// Gets or sets whether debug mode is enabled for troubleshooting drag and drop issues
        /// </summary>
        public bool IsDebugMode { get; set; }

        /// <summary>
        /// Gets or sets the expected visible height for this card when stacked.
        /// This helps limit hit testing to only the visible portion of stacked cards.
        /// Default is full card height (120px). For stacked cards, this should be set to TableauVerticalOffset (20px).
        /// </summary>
        public double VisibleHeight { get; set; } = CardGames.Core.CardVisualConstants.CardHeight;

        /// <summary>
        /// Event raised for debug logging to help troubleshoot drag and drop issues
        /// </summary>
        public event EventHandler<string> DebugLog;

        public CardUserControl()
        {
            InitializeComponent();
            AllowDrop = true;
            IsDebugMode = false; // Debug mode off by default
        }

        public void SetupCard(Card card)
        {
            Card = card;
            //lblTopLeftNumber.Text = card.Number.ToString().Replace("_", "");
            //lblSuite.Text = card.Suite.ToString();
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";
            
            try
            {
                PicCard.Source = new BitmapImage(new Uri($"Images/{fileName}", UriKind.Relative));
                PicBack.Source = new BitmapImage(new Uri("Images/cardback1.jpg", UriKind.Relative));
            }
            catch (System.Exception)
            {
                // Fallback to pack URI if relative path doesn't work
                PicCard.Source = new BitmapImage(new Uri($"pack://application:,,,/Images/{fileName}"));
                PicBack.Source = new BitmapImage(new Uri("pack://application:,,,/Images/cardback1.jpg"));
            }
        }

        private bool _cardVisible;
        public bool IsFaceUp
        {
            get
            {
                return _cardVisible;
            }
            set
            {
                _cardVisible = value;
                UpdateVisibility();
            }
        }

        /// <summary>
        /// Updates the visibility of card images based on current state
        /// </summary>
        private void UpdateVisibility()
        {
            // If there's no card, hide both images to show empty spot
            if (Card == null)
            {
                // Show empty spot for all empty positions, including stock pile
                // The stock pile remains clickable through the underlying Grid element
                PicBack.Visibility = Visibility.Hidden;
                PicCard.Visibility = Visibility.Hidden;
                
                // For empty tableau drop targets, show a subtle background to indicate droppable area
                // This helps users see where they can drop Kings on empty tableau columns
                if (IsTableauDropTarget)
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255)); // Very light semi-transparent white
                    this.BorderBrush = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)); // Slightly more visible border
                    this.BorderThickness = new Thickness(1);
                }
                else
                {
                    this.Background = Brushes.Transparent;
                    this.BorderBrush = null;
                    this.BorderThickness = new Thickness(0);
                }
            }
            else if (_cardVisible == true)
            {
                PicBack.Visibility = Visibility.Hidden;
                PicCard.Visibility = Visibility.Visible;
            }
            else
            {
                PicBack.Visibility = Visibility.Visible;
                PicCard.Visibility = Visibility.Hidden;
            }
            
            // Update debug visual indicators if debug mode is enabled
            UpdateDebugVisuals();
        }

        /// <summary>
        /// Helper method to emit debug log messages when debug mode is enabled
        /// </summary>
        private void LogDebug(string message)
        {
            if (IsDebugMode)
            {
                string cardInfo = Card != null ? $"{Card.Number} of {Card.Suite}s" : "Empty";
                string fullMessage = $"[{cardInfo}] {message}";
                DebugLog?.Invoke(this, fullMessage);
            }
        }

        /// <summary>
        /// Checks if a mouse position is within the visible (clickable) area of this card.
        /// This prevents interactions with hidden portions of stacked cards.
        /// </summary>
        private bool IsWithinVisibleArea(Point mousePosition)
        {
            // Check horizontal bounds (always full width)
            if (mousePosition.X < 0 || mousePosition.X > this.ActualWidth)
            {
                return false;
            }
            
            // Check vertical bounds (limited by VisibleHeight)
            if (mousePosition.Y < 0 || mousePosition.Y > VisibleHeight)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Updates debug visual indicators when debug mode is enabled
        /// </summary>
        private void UpdateDebugVisuals()
        {
            if (IsDebugMode && Card != null)
            {
                // Set up debug border rectangle to show actual clickable boundaries
                DebugBorder.Width = CardGames.Core.CardVisualConstants.CardWidth;
                DebugBorder.Height = VisibleHeight;
                DebugBorder.Visibility = Visibility.Visible;
                
                // Add debug background to show draggable state, but only in visible area
                if (IsFaceUp)
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(20, 0, 255, 0)); // Light green for draggable
                }
                else
                {
                    this.Background = new SolidColorBrush(Color.FromArgb(20, 255, 0, 0)); // Light red for non-draggable
                }
                
                // Add a visual indicator to show the visible hit area
                // This will help users see exactly where they can click
                if (VisibleHeight < CardGames.Core.CardVisualConstants.CardHeight)
                {
                    // Create a clip to show only the visible portion
                    RectangleGeometry clip = new RectangleGeometry();
                    clip.Rect = new Rect(0, 0, this.Width, VisibleHeight);
                    this.Clip = clip;
                    
                    LogDebug($"Debug: Visible hit area clipped to {this.Width:F1} × {VisibleHeight:F1} (was {this.Width:F1} × {CardGames.Core.CardVisualConstants.CardHeight:F1})");
                }
                else
                {
                    this.Clip = null; // Full card is visible
                }
            }
            else if (!IsTableauDropTarget)
            {
                // Clear debug visuals when debug mode is off
                this.Background = Brushes.Transparent;
                DebugBorder.Visibility = Visibility.Collapsed;
                this.Clip = null;
            }
        }

        private bool _isDragging = false;
        private Point _startPoint;

        private void PicBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            LogDebug($"MouseDown - Button: {e.LeftButton}, ClickCount: {e.ClickCount}, IsFaceUp: {IsFaceUp}, Position: ({mousePosition.X:F1}, {mousePosition.Y:F1})");
            
            // Check if the click is within the visible area
            if (!IsWithinVisibleArea(mousePosition))
            {
                LogDebug($"MouseDown ignored - Click outside visible area (visible height: {VisibleHeight:F1})");
                return;
            }
            
            // Handle double-click using ClickCount since Image does not expose MouseDoubleClick in XAML
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                LogDebug("Double-click detected, handling as double-click");
                PicBack_DoubleClick(sender, e);
                e.Handled = true;
                return;
            }

            // Only allow starting a drag for face-up cards
            if (e.LeftButton == MouseButtonState.Pressed && Card != null && IsFaceUp)
            {
                _startPoint = mousePosition;
                _isDragging = false;
                LogDebug($"Drag start point recorded: ({_startPoint.X:F1}, {_startPoint.Y:F1})");
            }
            else
            {
                LogDebug($"MouseDown ignored - Card: {(Card != null ? "Present" : "Null")}, IsFaceUp: {IsFaceUp}");
            }
        }

        private void PicBack_MouseMove(object sender, MouseEventArgs e)
        {
            // Only allow dragging for face-up cards
            if (e.LeftButton == MouseButtonState.Pressed && Card != null && IsFaceUp && !_isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                Vector diff = _startPoint - currentPosition;
                
                double horizontalDistance = Math.Abs(diff.X);
                double verticalDistance = Math.Abs(diff.Y);
                double minHorizontal = SystemParameters.MinimumHorizontalDragDistance;
                double minVertical = SystemParameters.MinimumVerticalDragDistance;
                
                LogDebug($"MouseMove - Current: ({currentPosition.X:F1}, {currentPosition.Y:F1}), " +
                        $"Distance: ({horizontalDistance:F1}, {verticalDistance:F1}), " +
                        $"MinRequired: ({minHorizontal:F1}, {minVertical:F1})");
                
                if (horizontalDistance > minHorizontal || verticalDistance > minVertical)
                {
                    _isDragging = true;
                    LogDebug("Drag threshold exceeded - starting drag operation");
                    
                    // Raise the drag started event
                    CardDragStarted?.Invoke(this, Card);
                    
                    // Initiate drag and drop operation
                    DragDrop.DoDragDrop(this, Card, DragDropEffects.Move);
                    
                    LogDebug("Drag operation completed");
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Log why dragging is not allowed
                if (Card == null)
                {
                    LogDebug("MouseMove ignored - No card present");
                }
                else if (!IsFaceUp)
                {
                    LogDebug("MouseMove ignored - Card is face down");
                }
                else if (_isDragging)
                {
                    LogDebug("MouseMove ignored - Already dragging");
                }
            }
        }

        private void PicBack_MouseUp(object sender, MouseButtonEventArgs e)
        {
            LogDebug($"MouseUp - Button: {e.LeftButton}, WasDragging: {_isDragging}, IsStockPile: {IsStockPile}");
            
            // Handle single-click for stock pile when no drag occurred
            if (e.LeftButton == MouseButtonState.Released && !_isDragging && IsStockPile)
            {
                LogDebug("Stock pile single-click detected");
                // For stock pile, raise the stock pile clicked event on single-click
                StockPileClicked?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
            }
            
            // Reset dragging state
            _isDragging = false;
        }

        private void PicBack_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Reset dragging state
            _isDragging = false;
            
            if (IsStockPile)
            {
                // For stock pile, raise the stock pile clicked event instead of flipping
                StockPileClicked?.Invoke(this, EventArgs.Empty);
            }
            else if (Card != null && IsFaceUp)
            {
                // Raise card clicked event for face-up cards
                CardClicked?.Invoke(this, Card);
            }
            else
            {
                // Flip face-down cards
                IsFaceUp = !IsFaceUp;
            }
        }

        private void CardUserControl_DragEnter(object sender, DragEventArgs e)
        {
            LogDebug("DragEnter event triggered");
            
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                Card draggedCard = (Card)e.Data.GetData(typeof(Card));
                LogDebug($"DragEnter with card: {draggedCard.Number} of {draggedCard.Suite}s");
                
                // Request validation from parent
                ValidateDropEventArgs args = new ValidateDropEventArgs(draggedCard, this);
                ValidateDrop?.Invoke(this, args);
                
                LogDebug($"Drop validation result: {args.IsValid}");
                
                // Show visual indicators on all sides based on validation result
                SolidColorBrush indicatorBrush;
                if (args.IsValid)
                {
                    indicatorBrush = new SolidColorBrush(Colors.LightGreen);
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    indicatorBrush = new SolidColorBrush(Colors.LightCoral);
                    e.Effects = DragDropEffects.None;
                }
                
                // Set all side indicators
                LeftIndicator.Fill = indicatorBrush;
                RightIndicator.Fill = indicatorBrush;
                TopIndicator.Fill = indicatorBrush;
                BottomIndicator.Fill = indicatorBrush;
                
                // Make indicators visible
                LeftIndicator.Visibility = Visibility.Visible;
                RightIndicator.Visibility = Visibility.Visible;
                TopIndicator.Visibility = Visibility.Visible;
                BottomIndicator.Visibility = Visibility.Visible;
                
                this.Opacity = 0.8;
            }
            else
            {
                LogDebug("DragEnter with non-card data");
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void CardUserControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void CardUserControl_DragLeave(object sender, DragEventArgs e)
        {
            LogDebug("DragLeave event triggered");
            
            // Hide all side indicators when drag leaves the control
            LeftIndicator.Visibility = Visibility.Collapsed;
            RightIndicator.Visibility = Visibility.Collapsed;
            TopIndicator.Visibility = Visibility.Collapsed;
            BottomIndicator.Visibility = Visibility.Collapsed;
            
            this.Opacity = 1.0;
            e.Handled = true;
        }

        private void CardUserControl_Drop(object sender, DragEventArgs e)
        {
            LogDebug("Drop event triggered");
            
            // Hide all side indicators after drop
            LeftIndicator.Visibility = Visibility.Collapsed;
            RightIndicator.Visibility = Visibility.Collapsed;
            TopIndicator.Visibility = Visibility.Collapsed;
            BottomIndicator.Visibility = Visibility.Collapsed;
            
            this.Opacity = 1.0;
            
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                Card droppedCard = (Card)e.Data.GetData(typeof(Card));
                LogDebug($"Drop with card: {droppedCard.Number} of {droppedCard.Suite}s");
                CardDropped?.Invoke(this, new CardDropEventArgs(droppedCard, this));
            }
            else
            {
                LogDebug("Drop with non-card data");
            }
            e.Handled = true;
        }
    }

    /// <summary>
    /// Event arguments for card drop operations
    /// </summary>
    public class CardDropEventArgs : EventArgs
    {
        public Card DroppedCard { get; }
        public CardUserControl TargetControl { get; }

        public CardDropEventArgs(Card droppedCard, CardUserControl targetControl)
        {
            DroppedCard = droppedCard;
            TargetControl = targetControl;
        }
    }

    /// <summary>
    /// Event arguments for validating drop operations
    /// </summary>
    public class ValidateDropEventArgs : EventArgs
    {
        public Card DraggedCard { get; }
        public CardUserControl TargetControl { get; }
        public bool IsValid { get; set; }

        public ValidateDropEventArgs(Card draggedCard, CardUserControl targetControl)
        {
            DraggedCard = draggedCard;
            TargetControl = targetControl;
            IsValid = false; // Default to invalid
        }
    }
}
