
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
            this.lblTopLeftNumber = new System.Windows.Forms.Label();
            this.lblSuite = new System.Windows.Forms.Label();
            this.picBack = new System.Windows.Forms.PictureBox();
            this.picCard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCard)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTopLeftNumber
            // 
            this.lblTopLeftNumber.AutoSize = true;
            this.lblTopLeftNumber.BackColor = System.Drawing.Color.Transparent;
            this.lblTopLeftNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTopLeftNumber.Location = new System.Drawing.Point(11, 15);
            this.lblTopLeftNumber.Name = "lblTopLeftNumber";
            this.lblTopLeftNumber.Size = new System.Drawing.Size(40, 42);
            this.lblTopLeftNumber.TabIndex = 5;
            this.lblTopLeftNumber.Text = "9";
            // 
            // lblSuite
            // 
            this.lblSuite.BackColor = System.Drawing.Color.Transparent;
            this.lblSuite.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSuite.Location = new System.Drawing.Point(57, 15);
            this.lblSuite.Name = "lblSuite";
            this.lblSuite.Size = new System.Drawing.Size(243, 42);
            this.lblSuite.TabIndex = 7;
            this.lblSuite.Text = "Hearts";
            this.lblSuite.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picBack
            // 
            this.picBack.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picBack.BackgroundImage")));
            this.picBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBack.Location = new System.Drawing.Point(0, 0);
            this.picBack.Name = "picBack";
            this.picBack.Size = new System.Drawing.Size(300, 400);
            this.picBack.TabIndex = 8;
            this.picBack.TabStop = false;
            this.picBack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PicBack_MouseDown_1);
            // 
            // picCard
            // 
            this.picCard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picCard.Location = new System.Drawing.Point(0, 0);
            this.picCard.Name = "picCard";
            this.picCard.Size = new System.Drawing.Size(300, 400);
            this.picCard.TabIndex = 9;
            this.picCard.TabStop = false;
            this.picCard.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PicCard_MouseDown);
            // 
            // CardUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.picBack);
            this.Controls.Add(this.picCard);
            this.Controls.Add(this.lblSuite);
            this.Controls.Add(this.lblTopLeftNumber);
            this.Name = "CardUserControl";
            this.Size = new System.Drawing.Size(300, 400);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CardUserControl_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCard)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblTopLeftNumber;
        private System.Windows.Forms.Label lblSuite;
        private System.Windows.Forms.PictureBox picBack;
        private System.Windows.Forms.PictureBox picCard;
    }
}
