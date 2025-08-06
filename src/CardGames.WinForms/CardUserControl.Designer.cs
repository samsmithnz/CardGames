
namespace CardGames.WinForms
{
    partial class CardUserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardUserControl));
            lblTopLeftNumber = new System.Windows.Forms.Label();
            lblSuite = new System.Windows.Forms.Label();
            picBack = new System.Windows.Forms.PictureBox();
            picCard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)picBack).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCard).BeginInit();
            SuspendLayout();
            // 
            // lblTopLeftNumber
            // 
            lblTopLeftNumber.AutoSize = true;
            lblTopLeftNumber.BackColor = System.Drawing.Color.Transparent;
            lblTopLeftNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblTopLeftNumber.Location = new System.Drawing.Point(11, 15);
            lblTopLeftNumber.Name = "lblTopLeftNumber";
            lblTopLeftNumber.Size = new System.Drawing.Size(40, 42);
            lblTopLeftNumber.TabIndex = 5;
            lblTopLeftNumber.Text = "9";
            // 
            // lblSuite
            // 
            lblSuite.BackColor = System.Drawing.Color.Transparent;
            lblSuite.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblSuite.Location = new System.Drawing.Point(57, 15);
            lblSuite.Name = "lblSuite";
            lblSuite.Size = new System.Drawing.Size(243, 42);
            lblSuite.TabIndex = 7;
            lblSuite.Text = "Hearts";
            lblSuite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picBack
            // 
            picBack.BackgroundImage = (System.Drawing.Image)resources.GetObject("picBack.BackgroundImage");
            picBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            picBack.Location = new System.Drawing.Point(0, 0);
            picBack.Name = "picBack";
            picBack.Size = new System.Drawing.Size(300, 400);
            picBack.TabIndex = 8;
            picBack.TabStop = false;
            picBack.MouseDown += PicBack_MouseDown_1;
            // 
            // picCard
            // 
            picCard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            picCard.Location = new System.Drawing.Point(0, 0);
            picCard.Name = "picCard";
            picCard.Size = new System.Drawing.Size(300, 400);
            picCard.TabIndex = 9;
            picCard.TabStop = false;
            picCard.MouseDown += PicCard_MouseDown;
            // 
            // CardUserControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            BackColor = System.Drawing.Color.White;
            Controls.Add(picBack);
            Controls.Add(picCard);
            Controls.Add(lblSuite);
            Controls.Add(lblTopLeftNumber);
            Name = "CardUserControl";
            Size = new System.Drawing.Size(300, 400);
            MouseDown += CardUserControl_MouseDown;
            ((System.ComponentModel.ISupportInitialize)picBack).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCard).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label lblTopLeftNumber;
        private System.Windows.Forms.Label lblSuite;
        private System.Windows.Forms.PictureBox picBack;
        private System.Windows.Forms.PictureBox picCard;
    }
}
