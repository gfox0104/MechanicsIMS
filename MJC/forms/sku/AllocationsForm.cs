using MJC.common.components;
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

namespace MJC.forms.sku
{
    public partial class AllocationsForm : GlobalLayout
    {
        private HotkeyButton hkPrevScreen = new HotkeyButton("Esc", "Previous screen", Keys.Escape);
        private HotkeyButton OpenOrder = new HotkeyButton("Enter", "Open order", Keys.Enter);


        public AllocationsForm() : base("Allocations for SKU#", "Review held orders with sku quantity allocated")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkPrevScreen, OpenOrder };
            _initializeHKButtons(hkButtons);
        }
    }
}
