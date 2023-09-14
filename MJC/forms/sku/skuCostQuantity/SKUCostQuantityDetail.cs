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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJC.forms.sku
{
    public partial class SKUCostQuantityDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FDateTime CostDate = new FDateTime("Date");
        private FInputBox Method = new FInputBox("Method");
        private FInputBox Qty = new FInputBox("Qty");
        private FInputBox Cost = new FInputBox("Cost");
        private FInputBox Core = new FInputBox("Core");

        
        private int skuId = 0;
        private int skuCostQtyId = 0;
        private bool readOnly = false;

        public SKUCostQuantityDetail(int skuId, bool readOnly = false) : base("Add SKU Cost Qty")
        {
            InitializeComponent();
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.skuId = skuId;
            this.readOnly = readOnly;

            InitMBOKButton();
            InitInputBox();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 350);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            CostDate.SetPosition(new Point(30, 80));
            this.Controls.Add(CostDate.GetLabel());
            this.Controls.Add(CostDate.GetDateTimePicker());

            Method.SetPosition(new Point(30, 130));
            this.Controls.Add(Method.GetLabel());
            this.Controls.Add(Method.GetTextBox());

            Qty.SetPosition(new Point(30, 180));
            this.Controls.Add(Qty.GetLabel());
            this.Controls.Add(Qty.GetTextBox());

            Cost.SetPosition(new Point(30, 230));
            this.Controls.Add(Cost.GetLabel());
            this.Controls.Add(Cost.GetTextBox());

            Core.SetPosition(new Point(30, 280));
            this.Controls.Add(Core.GetLabel());
            this.Controls.Add(Core.GetTextBox());

            if (this.readOnly)
            {
                CostDate.GetDateTimePicker().Enabled = false;
                Method.GetTextBox().Enabled = false;
                Qty.GetTextBox().Enabled = false;
                Cost.GetTextBox().Enabled = false;
                Core.GetTextBox().Enabled = false;
            }
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            int skuId = this.skuId;
            DateTime costDate = this.CostDate.GetDateTimePicker().Value;
            string method = this.Method.GetTextBox().Text;
            int? qty = null;
            if(!string.IsNullOrEmpty(this.Qty.GetTextBox().Text))
                qty = int.Parse(this.Qty.GetTextBox().Text);
            decimal? cost = null;
            if(!string.IsNullOrEmpty(this.Cost.GetTextBox().Text))
                cost = decimal.Parse(this.Cost.GetTextBox().Text);
            decimal? core = null;
            if (!string.IsNullOrEmpty(this.Core.GetTextBox().Text))
                core = decimal.Parse(this.Core.GetTextBox().Text);

            bool refreshData = false;

            if (this.skuCostQtyId == 0) refreshData = Session.SKUCostQtyModelObj.AddSKUCostQty(skuId, costDate, method, qty, cost, core);
            else refreshData = Session.SKUCostQtyModelObj.UpdateSKUCostQty(skuId, costDate, method, qty, cost, core, this.skuCostQtyId);

            string modeText = skuCostQtyId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the vendor.");
        }

        public void setDetails(SKUCostQty skuCostQty)
        {
            this.skuCostQtyId = skuCostQty.id;
            if (skuCostQty.costDate.HasValue)
            {
                this.CostDate.GetDateTimePicker().Value = skuCostQty.costDate.Value;
            }
            this.Method.GetTextBox().Text = skuCostQty.method;
            this.Qty.GetTextBox().Text = skuCostQty.qty.ToString();
            this.Cost.GetTextBox().Text = skuCostQty.cost.ToString();
            this.Core.GetTextBox().Text = skuCostQty.core.ToString();
        }
    }
}
