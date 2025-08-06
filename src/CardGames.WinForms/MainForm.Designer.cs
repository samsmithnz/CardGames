
namespace CardGames.WinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblTotalCards = new System.Windows.Forms.ToolStripStatusLabel();
            this.DeckPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.AutoSize = false;
            this.statusStrip.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.statusStrip.Font = new System.Drawing.Font("Segoe UI", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblTotalCards});
            this.statusStrip.Location = new System.Drawing.Point(0, 805);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1699, 40);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblTotalCards
            // 
            this.lblTotalCards.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalCards.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTotalCards.Name = "lblTotalCards";
            this.lblTotalCards.Size = new System.Drawing.Size(152, 30);
            this.lblTotalCards.Text = "Total cards: 0";
            // 
            // DeckPanel
            // 
            this.DeckPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DeckPanel.Location = new System.Drawing.Point(20, 20);
            this.DeckPanel.Name = "DeckPanel";
            this.DeckPanel.Size = new System.Drawing.Size(220, 320);
            this.DeckPanel.TabIndex = 3;
            this.DeckPanel.Tag = "Deck";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(260, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(220, 320);
            this.panel1.TabIndex = 4;
            this.panel1.Tag = "DeckDiscard";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Location = new System.Drawing.Point(1460, 20);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(220, 320);
            this.panel4.TabIndex = 5;
            this.panel4.Tag = "Ace4";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Location = new System.Drawing.Point(1220, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(220, 320);
            this.panel2.TabIndex = 6;
            this.panel2.Tag = "Ace3";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(980, 20);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(220, 320);
            this.panel3.TabIndex = 6;
            this.panel3.Tag = "Ace2";
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Location = new System.Drawing.Point(740, 20);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(220, 320);
            this.panel5.TabIndex = 6;
            this.panel5.Tag = "Ace1";
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Location = new System.Drawing.Point(20, 400);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(220, 320);
            this.panel6.TabIndex = 4;
            this.panel6.Tag = "PlayingArea1";
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Location = new System.Drawing.Point(260, 400);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(220, 320);
            this.panel7.TabIndex = 5;
            this.panel7.Tag = "PlayingArea2";
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Location = new System.Drawing.Point(500, 400);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(220, 320);
            this.panel8.TabIndex = 6;
            this.panel8.Tag = "PlayingArea3";
            // 
            // panel9
            // 
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Location = new System.Drawing.Point(740, 400);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(220, 320);
            this.panel9.TabIndex = 6;
            this.panel9.Tag = "PlayingArea4";
            // 
            // panel10
            // 
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Location = new System.Drawing.Point(980, 400);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(220, 320);
            this.panel10.TabIndex = 6;
            this.panel10.Tag = "PlayingArea5";
            // 
            // panel11
            // 
            this.panel11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel11.Location = new System.Drawing.Point(1220, 400);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(220, 320);
            this.panel11.TabIndex = 6;
            this.panel11.Tag = "PlayingArea6";
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Location = new System.Drawing.Point(1460, 400);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(220, 320);
            this.panel12.TabIndex = 6;
            this.panel12.Tag = "PlayingArea7";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleGreen;
            this.ClientSize = new System.Drawing.Size(1699, 845);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.panel11);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.DeckPanel);
            this.Controls.Add(this.statusStrip);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Card Games";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblTotalCards;
        private System.Windows.Forms.Panel DeckPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel panel12;
    }
}

