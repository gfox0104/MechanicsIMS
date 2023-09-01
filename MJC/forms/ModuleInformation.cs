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

namespace MJC.forms
{
    public partial class ModuleInformation : GlobalLayout
    {

        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private FGroupLabel QuantityGroup = new FGroupLabel("Basic Information");
        private FlabelConstant QtyAvailable = new FlabelConstant("Post to Journal:", 220);
        private FInputBox CostMethod = new FInputBox("Cost Method:", 220);
        private FInputBox TypeOfSKU = new FInputBox("Type of SKU Labels:", 220);
        private FInputBox DefaultScCode = new FInputBox("Default S/C Code:", 220);
        private FInputBox YtdBasedOn = new FInputBox("YTD Based on:", 220);

        private FGroupLabel AccountNumbers = new FGroupLabel("Account Numbers");
        private FlabelConstant Inventory = new FlabelConstant("Inventory:", 220);
        private FInputBox InventoryPayable = new FInputBox("Inventory Payable:", 220);
        private FInputBox InventoryExpense = new FInputBox("Inventory Expense:", 220);
        private FInputBox NewCoreInventory = new FInputBox("New Core Inventory:", 220);
        private FInputBox OldCoreInventory = new FInputBox("Old Core Inventory:", 220);
        private FInputBox SKUAssetAcct = new FInputBox("SKU Asset Acct:", 220);

        private FGroupLabel OrdersGroup = new FGroupLabel("Orders");
        private FCheckBox PrintMemos = new FCheckBox("Print memos in columns");


        public ModuleInformation() : base("Inventory Settings", "Manage inventory settings")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[1] { hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitForms();
        }

        private void AddHotKeyEvents()
        {
        }

        private void InitForms()
        {
            Panel _panel = new System.Windows.Forms.Panel();
            _panel.BackColor = System.Drawing.Color.Transparent;
            _panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            _panel.Location = new System.Drawing.Point(0, 95);
            _panel.Size = new Size(this.Width - 15, this.Height - 340);
            _panel.AutoScroll = true;
            this.Controls.Add(_panel);

            List<dynamic> FormComponents = new List<dynamic>();
            FormComponents.Add(QuantityGroup);
            FormComponents.Add(QtyAvailable);
            FormComponents.Add(CostMethod);
            FormComponents.Add(TypeOfSKU);
            FormComponents.Add(DefaultScCode);
            FormComponents.Add(YtdBasedOn);
            FormComponents.Add(SKUAssetAcct);
            _addFormInputs(FormComponents, 90, 40, 200, 43, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents1 = new List<dynamic>();
            FormComponents1.Add(AccountNumbers);
            FormComponents1.Add(Inventory);
            FormComponents1.Add(InventoryPayable);
            FormComponents1.Add(InventoryExpense);
            FormComponents1.Add(NewCoreInventory);
            FormComponents1.Add(OldCoreInventory);
            _addFormInputs(FormComponents1, 90, 360, 200, 43, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents2 = new List<dynamic>();
            FormComponents2.Add(OrdersGroup);
            FormComponents2.Add(PrintMemos);
            _addFormInputs(FormComponents2, 1050, 40, 200, 43, int.MaxValue, _panel.Controls);

        }
    }
}
