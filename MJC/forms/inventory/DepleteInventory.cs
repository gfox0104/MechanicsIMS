using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.inventory
{
    public partial class DepleteInventory : GlobalLayout
    {

        private HotkeyButton hkDepleteInv = new HotkeyButton("F8", "Deplete Inv", Keys.F8);
        private HotkeyButton hkCancel = new HotkeyButton("Esc", "Cancel", Keys.Escape);

        private FComboBox SKU = new FComboBox("SKU#:");
        private FlabelConstant Description = new FlabelConstant("Description:");

        private FGroupLabel QuantityGroup = new FGroupLabel("Quantity");
        private FlabelConstant QtyAvailable = new FlabelConstant("Qty Available:");
        private FInputBox QtyDeplete = new FInputBox("Qty to Deplete:");
        private int skuId = 0;

        SKUModel SkuModelObj = new SKUModel();

        public DepleteInventory() : base("Deplete Inventory", "Fill out to deplete inventory")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkDepleteInv, hkCancel };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitForms();
        }

        private void AddHotKeyEvents()
        {
            hkDepleteInv.GetButton().Click += (sender, e) =>
            {
                UpdateInventory();
            };
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

            List<KeyValuePair<int, string>> CustomerList = new List<KeyValuePair<int, string>>();
            CustomerList = SkuModelObj.GetSKUNameList();
            foreach (KeyValuePair<int, string> item in CustomerList)
            {
                int id = item.Key;
                string name = item.Value;
                SKU.GetComboBox().Items.Add(new FComboBoxItem(id, name));
            }
            SKU.GetComboBox().SelectedIndexChanged += new EventHandler(SKU_SelectedIndexChanged);

            FormComponents.Add(SKU);
            FormComponents.Add(Description);
            _addFormInputs(FormComponents, 750, 200, 800, 43, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents1 = new List<dynamic>();
            FormComponents1.Add(QuantityGroup);
            FormComponents1.Add(QtyAvailable);
            FormComponents1.Add(QtyDeplete);

            _addFormInputs(FormComponents1, 750, 320, 800, 43, int.MaxValue, _panel.Controls);
        }

        private void UpdateInventory()
        {
            int qtyAvailable = 0;
            if (!string.IsNullOrEmpty(QtyAvailable.GetConstant().Text))
                qtyAvailable = int.Parse(QtyAvailable.GetConstant().Text);
            int qtyDeplete = int.Parse(QtyDeplete.GetTextBox().Text);

            if (qtyDeplete <= qtyAvailable)
            {
                int skuId = this.skuId;
                int qty = qtyAvailable - qtyDeplete;
                var refreshData = SkuModelObj.UpdateSKUQty(qty, skuId);

                //if (refreshData)
                //{
                //    UpdateSkuInfo();
                //}
            }
            else
            {
                MessageBox.Show("Deplete Count must be less than Qty Available");
            }
        }

        private void SKU_SelectedIndexChanged(object sender, EventArgs e)
        {
            FComboBoxItem selectedItem = (FComboBoxItem)SKU.GetComboBox().SelectedItem;
            int skuId = selectedItem.Id;

            var skuInfo = SkuModelObj.LoadSkuInfoById(skuId);

            if (skuInfo != null)
            {
                this.skuId = skuInfo.skuId;
                Description.SetContext(skuInfo.desc);
                if (skuInfo.qtyAvailable != null)
                    QtyAvailable.SetContext(skuInfo.qtyAvailable.ToString());
                else QtyAvailable.SetContext("N/A");
            }
            else
            {
                Description.SetContext("N/A");
                QtyAvailable.SetContext("N/A");
            }
        }
    }
}
