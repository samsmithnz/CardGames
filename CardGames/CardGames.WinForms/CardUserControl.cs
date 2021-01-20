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
        public CardUserControl()
        {
            InitializeComponent();
            CardVisible = false;
            //this.Size = new System.Drawing.Size(300, 400);
        }

        public void SetupCard(Card card)
        {
            lblTopLeftNumber.Text = card.Number.ToString().Replace("_","");
            lblBottomRightNumber.Text = card.Number.ToString().Replace("_", "");
            lblSuite.Text = card.Suite.ToString();
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
                lblBottomRightNumber.Visible = _cardVisible;
                lblSuite.Visible = _cardVisible;
                picBack.Visible = !_cardVisible; //Note this is reversed
            }
        }

        //Start the drap drop
        private void CardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(sender, DragDropEffects.Move);
        }

        private void picBack_MouseDown(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(sender, DragDropEffects.Move);
        }
    }
}
