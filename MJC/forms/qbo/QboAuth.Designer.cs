namespace MJC.forms.qbo
{
    partial class QboAuth
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
        #endregion

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QboAuth));
            this.WebView = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.WebView)).BeginInit();
            this.SuspendLayout();
            // 
            // WebView
            // 
            this.WebView.AllowExternalDrop = true;
            this.WebView.CreationProperties = null;
            this.WebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.WebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebView.Location = new System.Drawing.Point(0, 0);
            this.WebView.Name = "WebView";
            this.WebView.Size = new System.Drawing.Size(600, 800);
            this.WebView.TabIndex = 0;
            this.WebView.ZoomFactor = 1D;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 800);
            this.Controls.Add(this.WebView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Quickbooks Online OAuth2 - WinForms";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QboAuth_FormClosing);
            this.Load += new System.EventHandler(this.QboAuth_Load);
            ((System.ComponentModel.ISupportInitialize)(this.WebView)).EndInit();
            this.ResumeLayout(false);
        }

        private Microsoft.Web.WebView2.WinForms.WebView2 WebView;
    }
}