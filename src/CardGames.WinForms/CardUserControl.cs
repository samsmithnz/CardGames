using CardGames.Core;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    /// <summary>
    /// Windows Forms UserControl for displaying playing cards
    /// </summary>
    public partial class CardUserControl : UserControl
    {
        private Card _card;
        private bool _isFaceUp = true;
        private bool _isDragging = false;
        private Point _lastPosition;

        /// <summary>
        /// Gets or sets the card associated with this control
        /// </summary>
        public Card Card 
        { 
            get { return _card; }
            set 
            { 
                _card = value;
                UpdateCardDisplay();
            }
        }

        /// <summary>
        /// Gets or sets whether the card is face up or face down
        /// </summary>
        public bool IsFaceUp
        {
            get { return _isFaceUp; }
            set
            {
                _isFaceUp = value;
                UpdateCardDisplay();
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

        public CardUserControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        /// <summary>
        /// Initializes the component and sets up the control
        /// </summary>
        private void InitializeComponent()
        {
            Size = new Size((int)CardVisualConstants.CardWidth, (int)CardVisualConstants.CardHeight);
            AllowDrop = true;
            BackColor = Color.Transparent;
            
            // Set up mouse events for dragging
            MouseDown += CardUserControl_MouseDown;
            MouseMove += CardUserControl_MouseMove;
            MouseUp += CardUserControl_MouseUp;
            
            // Set up drag and drop events
            DragEnter += CardUserControl_DragEnter;
            DragOver += CardUserControl_DragOver;
            DragDrop += CardUserControl_DragDrop;
        }

        /// <summary>
        /// Sets up the card with the specified card data
        /// </summary>
        public void SetupCard(Card card)
        {
            Card = card;
        }

        /// <summary>
        /// Updates the card display based on current card and face up state
        /// </summary>
        private void UpdateCardDisplay()
        {
            Invalidate(); // Trigger repaint
        }

        /// <summary>
        /// Paints the card on the control
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_card == null)
            {
                return;
            }

            try
            {
                Image cardImage = _isFaceUp ? GetCardFaceImage() : GetCardBackImage();
                
                if (cardImage != null)
                {
                    e.Graphics.DrawImage(cardImage, 0, 0, Width, Height);
                }
            }
            catch (Exception)
            {
                // Fallback: draw a simple rectangle with text
                DrawFallbackCard(e.Graphics);
            }
        }

        /// <summary>
        /// Gets the face image for the current card
        /// </summary>
        private Image GetCardFaceImage()
        {
            if (_card == null)
            {
                return null;
            }

            string fileName = "1920px-Playing_card_" + _card.Suite.ToString().ToLower() + "_" + _card.Number.ToString().Replace("_", "") + ".svg.png";
            return LoadImageFromResource(fileName);
        }

        /// <summary>
        /// Gets the back image for cards
        /// </summary>
        private Image GetCardBackImage()
        {
            return LoadImageFromResource("cardback1.jpg");
        }

        /// <summary>
        /// Loads an image from embedded resources
        /// </summary>
        private Image LoadImageFromResource(string fileName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = $"CardGames.WinForms.Images.{fileName}";
                
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        return Image.FromStream(stream);
                    }
                }
            }
            catch (Exception)
            {
                // Resource not found, will use fallback
            }
            
            return null;
        }

        /// <summary>
        /// Draws a fallback card representation when images are not available
        /// </summary>
        private void DrawFallbackCard(Graphics g)
        {
            // Draw card background
            g.FillRectangle(Brushes.White, 0, 0, Width, Height);
            g.DrawRectangle(Pens.Black, 0, 0, Width - 1, Height - 1);
            
            if (_isFaceUp && _card != null)
            {
                // Draw card details
                string cardText = _card.Number.ToString().Replace("_", "") + "\n" + _card.Suite.ToString();
                Font font = new Font("Arial", 8, FontStyle.Bold);
                SizeF textSize = g.MeasureString(cardText, font);
                
                float x = (Width - textSize.Width) / 2;
                float y = (Height - textSize.Height) / 2;
                
                // Choose color based on suit
                Brush textBrush = (_card.Suite == Card.CardSuite.Heart || _card.Suite == Card.CardSuite.Diamond) 
                    ? Brushes.Red : Brushes.Black;
                
                g.DrawString(cardText, font, textBrush, x, y);
            }
            else
            {
                // Draw card back pattern
                g.FillRectangle(Brushes.DarkBlue, 5, 5, Width - 10, Height - 10);
                g.DrawString("CARD", new Font("Arial", 10, FontStyle.Bold), Brushes.White, 15, Height / 2 - 10);
            }
        }

        #region Mouse and Drag Events

        private void CardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _card != null)
            {
                _isDragging = false;
                _lastPosition = e.Location;
            }
        }

        private void CardUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _card != null && !_isDragging)
            {
                // Check if we've moved enough to start dragging
                int deltaX = Math.Abs(e.X - _lastPosition.X);
                int deltaY = Math.Abs(e.Y - _lastPosition.Y);
                
                if (deltaX > 5 || deltaY > 5)
                {
                    _isDragging = true;
                    CardDragStarted?.Invoke(this, _card);
                    DoDragDrop(_card, DragDropEffects.Move);
                }
            }
        }

        private void CardUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_isDragging && _card != null)
            {
                // This was a click, not a drag
                CardClicked?.Invoke(this, _card);
            }
            _isDragging = false;
        }

        private void CardUserControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                ValidateDropEventArgs args = new ValidateDropEventArgs
                {
                    SourceCard = (Card)e.Data.GetData(typeof(Card)),
                    TargetControl = this,
                    IsValid = false
                };
                
                ValidateDrop?.Invoke(this, args);
                e.Effect = args.IsValid ? DragDropEffects.Move : DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void CardUserControl_DragOver(object sender, DragEventArgs e)
        {
            CardUserControl_DragEnter(sender, e); // Same validation logic
        }

        private void CardUserControl_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                Card sourceCard = (Card)e.Data.GetData(typeof(Card));
                CardDropEventArgs args = new CardDropEventArgs
                {
                    SourceCard = sourceCard,
                    TargetControl = this
                };
                
                CardDropped?.Invoke(this, args);
            }
        }

        #endregion
    }

    /// <summary>
    /// Event arguments for card drop operations
    /// </summary>
    public class CardDropEventArgs : EventArgs
    {
        public Card SourceCard { get; set; }
        public CardUserControl TargetControl { get; set; }
    }

    /// <summary>
    /// Event arguments for validating drop operations
    /// </summary>
    public class ValidateDropEventArgs : EventArgs
    {
        public Card SourceCard { get; set; }
        public CardUserControl TargetControl { get; set; }
        public bool IsValid { get; set; }
    }
}