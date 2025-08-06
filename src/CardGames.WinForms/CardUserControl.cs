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
        }

        //TODO: https://stackoverflow.com/questions/31193787/net-drag-and-drop-show-dragged-borders-or-image-such-as-windows-do
        public void SetupCard(Card card)
        {
            lblTopLeftNumber.Text = card.Number.ToString().Replace("_", "");
            lblSuite.Text = card.Suite.ToString();
            string fileName = "1920px-Playing_card_" + card.Suite.ToString().ToLower() + "_" + card.Number.ToString().Replace("_", "") + ".svg.png";
            
            string imagePath = System.IO.Path.Combine(Application.StartupPath, "Images", fileName);
            string backImagePath = System.IO.Path.Combine(Application.StartupPath, "Images", "cardback1.jpg");
            
            try
            {
                if (System.IO.File.Exists(imagePath))
                {
                    picCard.BackgroundImage = Image.FromFile(imagePath);
                }
                else
                {
                    // Try relative path from project directory
                    string projectPath = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Application.StartupPath)));
                    imagePath = System.IO.Path.Combine(projectPath, "CardGames.WinForms", "Images", fileName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        picCard.BackgroundImage = Image.FromFile(imagePath);
                    }
                }
            }
            catch (System.Exception)
            {
                // Handle image loading errors gracefully
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

        //Start the drap drop
        private void CardUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(sender, DragDropEffects.Move);
        }

        private void PicBack_MouseDown_1(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(sender, DragDropEffects.Move);
        }

        private void PicCard_MouseDown(object sender, MouseEventArgs e)
        {
            this.DoDragDrop(sender, DragDropEffects.Move);
        }
    }
}
