using CardGames.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    public partial class CardUserControl : UserControl
    {
        public Card Card { get; private set; }
        
        public CardUserControl()
        {
            InitializeComponent();
            CardVisible = false;
        }

        //TODO: https://stackoverflow.com/questions/31193787/net-drag-and-drop-show-dragged-borders-or-image-such-as-windows-do
        public void SetupCard(Card card)
        {
            Card = card;
            lblTopLeftNumber.Text = card.Number.ToString().Replace("_", "");
            lblSuite.Text = card.Suite.ToString();
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";
            
            // Use relative path instead of hardcoded path
            string basePath = System.IO.Path.Combine(Application.StartupPath, "Images");
            string fullPath = System.IO.Path.Combine(basePath, fileName);
            
            if (System.IO.File.Exists(fullPath))
            {
                picCard.BackgroundImage = Image.FromFile(fullPath);
            }
            CardVisible = false;
        }

        private bool _cardVisible;
        public bool CardVisible
        {
            get
            {
                return _cardVisible;
            }
            set
            {
                _cardVisible = value;
                lblTopLeftNumber.Visible = _cardVisible;
                lblSuite.Visible = _cardVisible;
                picBack.Visible = !_cardVisible; //Note this is reversed
            }
        }

        //Start the drag drop - pass the CardUserControl itself for proper handling
        private void CardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Card != null)
            {
                // Provide visual feedback that drag is starting
                this.Cursor = Cursors.Hand;
                this.DoDragDrop(this, DragDropEffects.Move);
                this.Cursor = Cursors.Default;
            }
        }

        private void PicBack_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Card != null)
            {
                this.Cursor = Cursors.Hand;
                this.DoDragDrop(this, DragDropEffects.Move);
                this.Cursor = Cursors.Default;
            }
        }

        private void PicCard_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Card != null)
            {
                this.Cursor = Cursors.Hand;
                this.DoDragDrop(this, DragDropEffects.Move);
                this.Cursor = Cursors.Default;
            }
        }
    }
}
