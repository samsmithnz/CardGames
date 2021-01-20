
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
            this.lblBottomRightNumber = new System.Windows.Forms.Label();
            this.lblTopLeftNumber = new System.Windows.Forms.Label();
            this.lblSuite = new System.Windows.Forms.Label();
            this.picBack = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).BeginInit();
            this.SuspendLayout();
            // 
            // lblBottomRightNumber
            // 
            this.lblBottomRightNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBottomRightNumber.AutoSize = true;
            this.lblBottomRightNumber.BackColor = System.Drawing.Color.Transparent;
            this.lblBottomRightNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBottomRightNumber.Location = new System.Drawing.Point(234, 340);
            this.lblBottomRightNumber.Name = "lblBottomRightNumber";
            this.lblBottomRightNumber.Size = new System.Drawing.Size(40, 42);
            this.lblBottomRightNumber.TabIndex = 6;
            this.lblBottomRightNumber.Text = "9";
            this.lblBottomRightNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblTopLeftNumber
            // 
            this.lblTopLeftNumber.AutoSize = true;
            this.lblTopLeftNumber.BackColor = System.Drawing.Color.Transparent;
            this.lblTopLeftNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTopLeftNumber.Location = new System.Drawing.Point(6, 22);
            this.lblTopLeftNumber.Name = "lblTopLeftNumber";
            this.lblTopLeftNumber.Size = new System.Drawing.Size(40, 42);
            this.lblTopLeftNumber.TabIndex = 5;
            this.lblTopLeftNumber.Text = "9";
            // 
            // lblSuite
            // 
            this.lblSuite.BackColor = System.Drawing.Color.Transparent;
            this.lblSuite.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSuite.Location = new System.Drawing.Point(75, 179);
            this.lblSuite.Name = "lblSuite";
            this.lblSuite.Size = new System.Drawing.Size(150, 42);
            this.lblSuite.TabIndex = 7;
            this.lblSuite.Text = "Hearts";
            this.lblSuite.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picBack
            // 
            this.picBack.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("picBack.BackgroundImage")));
            this.picBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picBack.Location = new System.Drawing.Point(-1, -1);
            this.picBack.Name = "picBack";
            this.picBack.Size = new System.Drawing.Size(300, 400);
            this.picBack.TabIndex = 8;
            this.picBack.TabStop = false;
            this.picBack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picBack_MouseDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 400);
            this.panel1.TabIndex = 9;
            // 
            // CardUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblSuite);
            this.Controls.Add(this.lblTopLeftNumber);
            this.Controls.Add(this.lblBottomRightNumber);
            this.Controls.Add(this.picBack);
            this.Controls.Add(this.panel1);
            this.Name = "CardUserControl";
            this.Size = new System.Drawing.Size(300, 400);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CardUserControl_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblBottomRightNumber;
        private System.Windows.Forms.Label lblTopLeftNumber;
        private System.Windows.Forms.Label lblSuite;
        private System.Windows.Forms.PictureBox picBack;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel1;
    }
}
