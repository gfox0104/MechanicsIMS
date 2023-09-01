﻿using MJC.common.components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJC.common
{
    public partial class BasicModal : Form
    {

        protected int _accountId = 1;

        public BasicModal()
        {
            InitializeComponent();
            this._initLayout(true);
        }
        public BasicModal(string title, bool defaultEsc = true) : base()
        {
            this._initLayout(defaultEsc);
            this.Text = title;
        }
        private void _initLayout(bool defaultEsc)
        {
            this.BackColor = System.Drawing.Color.FromArgb(38, 77, 118);
            this.KeyPreview = true;
            this.ShowIcon = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyDown += (s, e) => {
                if (defaultEsc && e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    return;
                }
            };
        }

        protected void _setModalStyle2()
        {
            this.BackColor = System.Drawing.Color.FromArgb(35, 102, 169);
            this.KeyPreview = true;
            this.ShowIcon = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        protected void ModalButton_HotKeyDown(ModalButton modalButton)
        {
            this.KeyDown += (s, e) => {
                if (e.KeyCode == modalButton.GetKeys())
                {
                    modalButton.GetButton().PerformClick();
                    return;
                }
            };
        }


    }
}
