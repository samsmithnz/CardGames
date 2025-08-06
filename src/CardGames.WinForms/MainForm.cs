using CardGames.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    public partial class MainForm : Form
    {
        private Deck Deck = null;
        public MainForm()
        {
            InitializeComponent();
            Deck = new Deck();
            Deck.Shuffle();
            LoadCards(Deck);

            //button1.Text = new Rune(0x1F0DE).ToString();
            //this.button1.Font = new System.Drawing.Font("Segoe UI", 100F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            //this.button1.Location = new System.Drawing.Point(1089, 358);
            //this.button1.Name = "button1";
            //this.button1.Size = new System.Drawing.Size(300, 400);
            //this.button1.TabIndex = 3;
            //this.button1.Text = "0";
            //this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //this.button1.UseVisualStyleBackColor = true;
        }

        private void LoadCards(Deck deck)
        {
            int xOffset = 0;
            int yOffset = 0;
            foreach (Card card in deck.Cards)
            {
                CardUserControl cardUserControl = new CardUserControl
                {
                    Size = new System.Drawing.Size(200, 300),
                    Location = new Point(0 + xOffset, 0+ yOffset)
                };
                DeckPanel.Controls.Add(cardUserControl);
                cardUserControl.SetupCard(card);
                //xOffset += 4;
                //yOffset += 4;
            }
            statusStrip.Items[0].Text = "Total cards: " + deck.Cards.Count.ToString();
        }

        //Check if the card is allowed to be dropped
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Move) != 0)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        //Drop the card
        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] formats = e.Data.GetFormats();
            if (formats.Length > 0)
            {
                CardUserControl destination;
                if (formats[0] == "System.Windows.Forms.PictureBox")
                {
                    PictureBox picBoxDestination = (PictureBox)e.Data.GetData(typeof(PictureBox));
                    destination = (CardUserControl)picBoxDestination.Parent;
                }
                else
                {
                    destination = (CardUserControl)e.Data.GetData(typeof(CardUserControl));
                }
                if (destination != null)
                {
                    destination.Location = this.PointToClient(Cursor.Position);
                    destination.CardVisible = true;
                    destination.Parent.Controls.SetChildIndex(destination, 0);
                }
                else
                {
                    foreach (string item in e.Data.GetFormats())
                    {
                        Debug.WriteLine(item.ToString());
                    }
                }

            }

        }
    }
}
