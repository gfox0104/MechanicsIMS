﻿using MJC.common.components;
using MJC.common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJC.forms.price
{
    public partial class PriceChange : GlobalLayout
    {

        private HotkeyButton hkReceiveInv = new HotkeyButton("F8", "Change Price", Keys.F8);
        private HotkeyButton hkCancel = new HotkeyButton("Esc", "Cancel", Keys.Escape);

        public PriceChange() : base("Price Change", "Change prices")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkReceiveInv, hkCancel };
            _initializeHKButtons(hkButtons);
            //_addComingSoon();

            //foreach (HotkeyButton button in hkButtons)
            //{
            //    if (button != hkCancel)
            //        button.GetButton().Click += new EventHandler(_hotkeyTest);
            //}
            hkCancel.GetButton().Click += new EventHandler(_navigateToPrev);
        }
    }
}
