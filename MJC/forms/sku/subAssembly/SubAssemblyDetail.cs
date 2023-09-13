using Antlr4.Runtime.Tree;
using MJC.common;
using MJC.common.components;
using MJC.forms.sku;
using MJC.forms.vendor;
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

namespace MJC.forms.sku.subAssembly
{
    public partial class SubAssemblyDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FlabelConstant TargetSKU = new FlabelConstant("Target SKU");
        private FComboBox SubAssembly = new FComboBox("Sub Assembly SKU");
        private FlabelConstant Category = new FlabelConstant("Category");
        private FlabelConstant Description = new FlabelConstant("Description");
        private FInputBox Quantity = new FInputBox("Quantity");

        private FlabelConstant Status = new FlabelConstant("Status");
        private FlabelConstant InvoicePrint = new FlabelConstant("Invoice Print");

        private int subAssemblyId = 0;
        private int targetSkuId = 0;
        private int subAssemblySkuId = 0;
        private int categoryId = 0;
        private string targetSKUName = "";
        private string categoryName = "";
        private string description = "";

        private bool status = false;
        private bool invoicePrint = false;
        private bool readOnly = false;

        public SubAssemblyDetail(int skuId, bool readOnly = false) : base("Add Sub Assembly")
        {
            InitializeComponent();
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.targetSkuId = skuId;
            this.readOnly = readOnly;

            if (skuId != 0)
            {
                SKUDetail skuDetail = Session.SKUModelObj.GetSKUById(skuId);
                this.targetSKUName = skuDetail.Name;
            }

            InitMBOKButton();
            InitInputBox();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 450);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            TargetSKU.SetPosition(new Point(30, 80));
            this.Controls.Add(TargetSKU.GetLabel());
            this.Controls.Add(TargetSKU.GetConstant());
            this.TargetSKU.GetConstant().Text = this.targetSKUName;

            SubAssembly.SetPosition(new Point(30, 130));
            this.Controls.Add(SubAssembly.GetLabel());
            this.Controls.Add(SubAssembly.GetComboBox());

            List<KeyValuePair<int, string>> CustomerList = new List<KeyValuePair<int, string>>();
            CustomerList = Session.SKUModelObj.GetSKUNameList();
            foreach (KeyValuePair<int, string> item in CustomerList)
            {
                int id = item.Key;
                string name = item.Value;
                SubAssembly.GetComboBox().Items.Add(new FComboBoxItem(id, name));
            }
            SubAssembly.GetComboBox().SelectedIndexChanged += new EventHandler(SKU_SelectedIndexChanged);

            Category.SetPosition(new Point(30, 180));
            this.Controls.Add(Category.GetLabel());
            this.Controls.Add(Category.GetConstant());
            this.Category.GetConstant().Text = this.categoryName;

            Description.SetPosition(new Point(30, 230));
            this.Controls.Add(Description.GetLabel());
            this.Controls.Add(Description.GetConstant());
            this.Description.GetConstant().Text = this.description;

            Quantity.SetPosition(new Point(30, 280));
            this.Controls.Add(Quantity.GetLabel());
            this.Controls.Add(Quantity.GetTextBox());

            if (this.readOnly)
            {
                SubAssembly.GetComboBox().Enabled = false;
                Quantity.GetTextBox().Enabled = false;
            }
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            int targetSkuId = this.targetSkuId;
            int categoryId = this.categoryId;
            int subAssemblySkuId = this.subAssemblySkuId;
            string description = this.Description.GetConstant().Text;
            
            int? qty = null;
            if (!string.IsNullOrEmpty(this.Quantity.GetTextBox().Text))
                qty = int.Parse(this.Quantity.GetTextBox().Text);

            bool refreshData = false;

            if (this.subAssemblyId == 0) refreshData = Session.SubAssemblyModelObj.AddSubAssembly(targetSkuId, subAssemblySkuId, categoryId, status, invoicePrint, description, qty);
            else refreshData = Session.SubAssemblyModelObj.UpdateSubAssembly(targetSkuId, subAssemblySkuId, categoryId, status, invoicePrint, description, qty, this.subAssemblyId);

            string modeText = this.subAssemblyId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the Sub Assembly.");
        }

        public void setDetails(SubAssembly subAssembly)
        {
            this.targetSkuId = subAssembly.targetSKUId;
            this.subAssemblyId = subAssembly.id;
            this.subAssemblySkuId = subAssembly.subAssemblySkuId;
            this.categoryId = subAssembly.categoryId;
            this.TargetSKU.SetContext(subAssembly.targetSKU);
            this.Category.SetContext(subAssembly.categoryName);
            this.Description.SetContext(subAssembly.description);
            this.Quantity.GetTextBox().Text = subAssembly.qty.ToString();

            this.status = subAssembly.status;
            this.invoicePrint = subAssembly.invoicePrint;
            if(!this.status)
                this.Status.SetContext("Information Only");
            if(this.invoicePrint)
                this.InvoicePrint.SetContext("Yes");
            else this.InvoicePrint.SetContext("No");
        }

        public void SKU_SelectedIndexChanged(object sender, EventArgs e)
        {
            FComboBoxItem selectedItem = (FComboBoxItem)SubAssembly.GetComboBox().SelectedItem;
            int skuId = selectedItem.Id;
            var skuInfo = Session.SKUModelObj.LoadSkuInfoById(skuId);
            
            if (skuInfo != null)
            {
                this.subAssemblySkuId = skuInfo.skuId;
                this.categoryId = skuInfo.categoryId;
                Category.SetContext(skuInfo.categoryName);
                Description.SetContext(skuInfo.desc);
            }
            else
            {
                Description.SetContext("N/A");
                Category.SetContext("N/A");
            }
        }
    }
}
