namespace MJC.forms
{
    partial class Splash
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splash));
            label1 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(540, 484);
            label1.Name = "label1";
            label1.Size = new Size(278, 25);
            label1.TabIndex = 1;
            label1.Text = "Proudly powered by Paradynamix";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(540, 509);
            label2.Name = "label2";
            label2.Size = new Size(300, 25);
            label2.TabIndex = 2;
            label2.Text = "For support, contact (866) 280-2237";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.White;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(22, 22);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(512, 512);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Bernard MT Condensed", 48F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(526, 171);
            label3.Name = "label3";
            label3.Size = new Size(441, 228);
            label3.TabIndex = 4;
            label3.Text = "Mechanic's\r\nIMS";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(540, 459);
            label4.Name = "label4";
            label4.Size = new Size(327, 25);
            label4.TabIndex = 5;
            label4.Text = "Developed for Marietta Joint and Clutch";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(540, 425);
            label5.Name = "label5";
            label5.Size = new Size(88, 25);
            label5.TabIndex = 6;
            label5.Text = "Loading...";
            // 
            // Splash
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1029, 559);
            ControlBox = false;
            Controls.Add(label5);
            Controls.Add(pictureBox1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Splash";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private PictureBox pictureBox1;
        private Label label3;
        private Label label4;
        private Label label5;
        private System.Windows.Forms.Timer timer1;
    }
}