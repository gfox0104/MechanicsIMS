using MJC.common.components;
using MJC.common;
using MJC.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MJC.forms.price;
using Antlr4.Runtime.Tree;

namespace MJC.forms.inventory
{
    public partial class ReceiveInventory : GlobalLayout
    {

        private HotkeyButton hkReceiveInv = new HotkeyButton("F8", "Receive Inv", Keys.F8);
        private HotkeyButton hkCancel = new HotkeyButton("Esc", "Cancel", Keys.Escape);

        private FComboBox SKU = new FComboBox("SKU#:");
        private FlabelConstant Description = new FlabelConstant("Description:");
        private FGroupLabel QuantityGroup = new FGroupLabel("Quantity");
        private FInputBox QtyReceived = new FInputBox("Qty Received:");
        private FlabelConstant OldQtyOnHand = new FlabelConstant("Old Qty on Hand:");
        private FlabelConstant NewQtyOnHand = new FlabelConstant("New Qty on Hand:");
        private FGroupLabel PriceGroup = new FGroupLabel("Price");
        private FInputBox NewCost = new FInputBox("Cost:");
        private FlabelConstant LastCost = new FlabelConstant("Last Cost:");
        private FInputBox[] priceTiers;

        private int skuId = 0;

        SKUModel SkuModelObj = new SKUModel();
        PriceTiersModel PriceTierModelObj = new PriceTiersModel();
        SKUPricesModel SkuPriceTierModelObj = new SKUPricesModel();

        public ReceiveInventory() : base("Recieve Inventory", "Fill out to receive inventory")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkReceiveInv, hkCancel };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitForms();
        }

        private void AddHotKeyEvents()
        {
            hkReceiveInv.GetButton().Click += (sender, e) =>
            {
                InsertSKUCost();
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

            QtyReceived.GetTextBox().KeyUp += (sender, e) =>
            {
                TextBox textBox = sender as TextBox;
                string qtyStr = textBox.Text;

                UpdateReceiveQty();
            };
            QtyReceived.GetTextBox().KeyPress += KeyValidateNumber;
            FormComponents.Add(SKU);
            FormComponents.Add(Description);
            _addFormInputs(FormComponents, 750, 20, 800, 43, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents1 = new List<dynamic>();
            FormComponents1.Add(QuantityGroup);
            FormComponents1.Add(QtyReceived);
            FormComponents1.Add(OldQtyOnHand);
            FormComponents1.Add(NewQtyOnHand);

            _addFormInputs(FormComponents1, 750, 140, 800, 43, int.MaxValue, _panel.Controls);

            List<dynamic> FormComponents2 = new List<dynamic>();
            FormComponents2.Add(PriceGroup);
            FormComponents2.Add(NewCost);
            //NewCost.GetTextBox().KeyPress += TextValidateNumber;
            FormComponents2.Add(LastCost);

            var refreshData = PriceTierModelObj.LoadPriceTierData();
            if (refreshData)
            {
                List<PriceTierData> PriceTierDataList = PriceTierModelObj.PriceTierDataList;
                int index = 0;

                priceTiers = new FInputBox[PriceTierDataList.Count];
                for (int i = 0; i < PriceTierDataList.Count; i++)
                {
                    priceTiers[i] = new FInputBox(PriceTierDataList[i].Name.ToString(), 200, PriceTierDataList[i].Id);
                    FormComponents2.Add(priceTiers[i]);
                }
            }

            _addFormInputs(FormComponents2, 750, 340, 800, 43, int.MaxValue, _panel.Controls);
        }

        private void KeyValidateNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                int qtyReceived = 0;
                int.TryParse(QtyReceived.GetTextBox().Text, out qtyReceived);
                int oldQty = 0;
                int.TryParse(OldQtyOnHand.GetConstant().Text, out oldQty);
                int newQty = qtyReceived + oldQty;
                NewQtyOnHand.GetConstant().Text = newQty.ToString();

            }
        }

        //private void TextValidateNumber(object sender, EventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    string str = textBox.Text;
        //    if (double.TryParse(str, out double doubleValue))
        //    {
        //        //MessageBox.Show("Text is numeric.");
        //    }
        //    else
        //    {
        //        // Here
        //        MessageBox.Show("Text is not numeric.");
        //    }
        //}

        public void UpdateReceiveQty()
        {
            int qtyReceived = 0;
            int oldQty = 0;
            if (!string.IsNullOrEmpty(QtyReceived.GetTextBox().Text))
                qtyReceived = int.Parse(QtyReceived.GetTextBox().Text);
            if (!OldQtyOnHand.GetConstant().Text.Equals("N/A"))
                oldQty = int.Parse(OldQtyOnHand.GetConstant().Text);
            NewQtyOnHand.GetConstant().Text = (qtyReceived + oldQty).ToString();
        }

        public void InsertSKUCost()
        {
            Boolean active = true;
            int skuId = this.skuId;
            int qty = int.Parse(QtyReceived.GetTextBox().Text);
            int totalQty = int.Parse(NewQtyOnHand.GetConstant().Text);
            double cost = double.Parse(NewCost.GetTextBox().Text);
            DateTime costDate = DateTime.Now;
            Dictionary<int, double> priceTierDict = new Dictionary<int, double>();

            for (int i = 0; i < priceTiers.Length; i++)
            {
                double priceData; bool parse_succeed = double.TryParse(priceTiers[i].GetTextBox().Text, out priceData);
                if (parse_succeed) priceTierDict.Add(priceTiers[i].GetId(), priceData);
            }

            int createdBy = 1;
            int updatedBy = 1;

            var refreshData = SkuModelObj.InsertSKUCostQty(active, skuId, costDate, qty, cost, priceTierDict, createdBy, updatedBy);

            refreshData = SkuModelObj.UpdateSKUQty(totalQty, skuId);


            ShowInformation("SKU Inventory updated successfully.");
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
                if (skuInfo.qty != null)
                    OldQtyOnHand.SetContext(skuInfo.qty.ToString());
                else OldQtyOnHand.SetContext("N/A");
                if (skuInfo.qty != null)
                    NewQtyOnHand.SetContext(skuInfo.qty.ToString());
                else NewQtyOnHand.SetContext("N/A");
                if (skuInfo.cost != null)
                    LastCost.SetContext(skuInfo.cost.ToString());
                else LastCost.SetContext("N/A");

                QtyReceived.GetTextBox().Text = "";
                NewCost.GetTextBox().Text = "";

                List<KeyValuePair<int, double>> skuPriceData = new List<KeyValuePair<int, double>>();
                skuPriceData = SkuPriceTierModelObj.LoadPriceTierDataBySKUId(skuId);


                for (int i = 0; i < priceTiers.Length; i++)
                {
                    priceTiers[i].GetTextBox().Text = "0.00";
                }
                foreach (KeyValuePair<int, double> pair in skuPriceData)
                {
                    for (int i = 0; i < priceTiers.Length; i++)
                        if (priceTiers[i].GetId() == pair.Key)
                            priceTiers[i].GetTextBox().Text = pair.Value.ToString("#0.00");
                }
            }
            else
            {
                Console.WriteLine("Non-existed SKU");
                Description.SetContext("N/A");
                NewQtyOnHand.SetContext("N/A");
                OldQtyOnHand.SetContext("N/A");
                LastCost.SetContext("N/A");
            }
        }
    }
}
