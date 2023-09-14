using Antlr4.Runtime.Tree;
using MJC.common;
using MJC.common.components;
using MJC.forms.sku;
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

namespace MJC.forms.vendor
{
    public partial class VendorCostsDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FComboBox VendorName = new FComboBox("Vendor");
        private FInputBox Manuf = new FInputBox("Manufacturer");
        private FInputBox Memo = new FInputBox("Memmo");
        private FInputBox PackageQty = new FInputBox("Pkg Qty");
        private FInputBox Cost = new FInputBox("Cost");
        private FInputBox Core = new FInputBox("Core");

        private int vendorCostId = 0;
        private int vendorId = 0;
        private int skuId = 0;

        public VendorCostsDetail(int skuId = 0) : base("Add Vendor Cost")
        {
            InitializeComponent();
            this.Size = new Size(600, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.skuId = skuId;
            InitMBOKButton();
            InitInputBox();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 500);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            VendorName.SetPosition(new Point(30, 30));
            VendorsModel VendorCostModelObj = new VendorsModel();
            //VendorName.GetTextBox().Values = VendorCostModelObj.GetVendorNameList().ToArray();
            this.Controls.Add(VendorName.GetLabel());
            this.Controls.Add(VendorName.GetComboBox());
            List<KeyValuePair<int, string>> VendorList = new List<KeyValuePair<int, string>>();
            VendorList = Session.VendorsModelObj.GetVendorList();
            foreach (KeyValuePair<int, string> item in VendorList)
            {
                int id = item.Key;
                string name = item.Value;
                VendorName.GetComboBox().Items.Add(new FComboBoxItem(id, name));
            }
            //VendorName.GetComboBox().SelectedIndexChanged += new EventHandler(Vendor_SelectedIndexChanged);


            Manuf.SetPosition(new Point(30, 80));
            this.Controls.Add(Manuf.GetLabel());
            this.Controls.Add(Manuf.GetTextBox());

            Memo.SetPosition(new Point(30, 130));
            this.Controls.Add(Memo.GetLabel());
            this.Controls.Add(Memo.GetTextBox());

            PackageQty.SetPosition(new Point(30, 180));
            this.Controls.Add(PackageQty.GetLabel());
            this.Controls.Add(PackageQty.GetTextBox());

            Cost.SetPosition(new Point(30, 230));
            this.Controls.Add(Cost.GetLabel());
            this.Controls.Add(Cost.GetTextBox());

            Core.SetPosition(new Point(30, 280));
            this.Controls.Add(Core.GetLabel());
            this.Controls.Add(Core.GetTextBox());
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            int skuId = this.skuId;
            int vendorId = this.vendorId;
            FComboBoxItem vendorSelectedItem = (FComboBoxItem)VendorName.GetComboBox().SelectedItem;
            string vendorName = this.VendorName.GetComboBox().Text;
            string manuf = this.Manuf.GetTextBox().Text;
            string memo = this.Memo.GetTextBox().Text;
            int pkgQty = int.Parse(this.PackageQty.GetTextBox().Text);
            decimal cost = decimal.Parse(this.Cost.GetTextBox().Text);
            decimal core = decimal.Parse(this.Core.GetTextBox().Text);

            bool refreshData = false;

            if (this.vendorCostId == 0) refreshData = Session.VendorCostModelObj.AddVendorCost(skuId, vendorId, vendorName, manuf, memo, pkgQty, cost, core);
            else refreshData = Session.VendorCostModelObj.UpdateVendorCost(skuId, vendorId, vendorName, manuf, memo, pkgQty, cost, core, this.vendorCostId);

            string modeText = vendorCostId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the vendor.");
        }

        public void setDetails(VendorCost vendorCost)
        {
            this.vendorCostId = vendorCost.id;
            this.vendorId = vendorCost.vendorId;
            int index = VendorName.GetComboBox().Items.Cast<FComboBoxItem>().ToList().FindIndex(item => item.Id == vendorId);
            VendorName.GetComboBox().SelectedIndex = index;
            this.Manuf.GetTextBox().Text = vendorCost.manufacturer;
            this.Memo.GetTextBox().Text = vendorCost.memo;
            this.PackageQty.GetTextBox().Text = vendorCost.packageQuantity.ToString();
            this.Cost.GetTextBox().Text = vendorCost.cost.ToString();
            this.Core.GetTextBox().Text = vendorCost.core.ToString();
        }
    }
}
