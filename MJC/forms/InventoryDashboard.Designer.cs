﻿namespace MJC.forms
{
    partial class InventoryDashboard
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
            this.SuspendLayout();
            // 
            // _header
            // 
            this._header.Size = new System.Drawing.Size(800, 95);
            // 
            // _footer
            // 
            this._footer.Location = new System.Drawing.Point(0, 250);
            this._footer.Size = new System.Drawing.Size(800, 200);
            // 
            // InventoryDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "InventoryDashboard";
            this.Text = "InventoryDashboard";
            this.Activated += new System.EventHandler(this.InventoryDashboard_Activated);
            this.ResumeLayout(false);

        }

        #endregion
    }
}