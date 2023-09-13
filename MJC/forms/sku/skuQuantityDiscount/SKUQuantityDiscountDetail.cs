using MJC.common;
using MJC.common.components;
using MJC.model;
using RestSharp;
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

namespace MJC.forms.sku.skuQuantityDiscount
{
    public partial class SKUQuantityDiscountDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FInputBox FromQty = new FInputBox("From Qty");
        private FInputBox ToQty = new FInputBox("To Qty");
        private FInputBox PriceTier = new FInputBox("Price Tier");
        private FInputBox DiscountType = new FInputBox("Discount Type");
        private FInputBox Discount0 = new FInputBox("Discount0");
        private FInputBox Discount1 = new FInputBox("Discount1");

        private SKUQtyDiscountModel SKUQtyDiscountModelObj = new SKUQtyDiscountModel();

        private int skuId = 0;
        private int skuQtyDiscountId = 0;
        private bool readOnly = false;

        public SKUQuantityDiscountDetail(int skuId, bool readOnly = false) : base("Add SKU Quantity Discount")
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
            FromQty.SetPosition(new Point(30, 80));
            this.Controls.Add(FromQty.GetLabel());
            this.Controls.Add(FromQty.GetTextBox());

            ToQty.SetPosition(new Point(30, 130));
            this.Controls.Add(ToQty.GetLabel());
            this.Controls.Add(ToQty.GetTextBox());

            PriceTier.SetPosition(new Point(30, 180));
            this.Controls.Add(PriceTier.GetLabel());
            this.Controls.Add(PriceTier.GetTextBox());

            DiscountType.SetPosition(new Point(30, 230));
            this.Controls.Add(DiscountType.GetLabel());
            this.Controls.Add(DiscountType.GetTextBox());

            Discount0.SetPosition(new Point(30, 280));
            this.Controls.Add(Discount0.GetLabel());
            this.Controls.Add(Discount0.GetTextBox());

            Discount1.SetPosition(new Point(30, 280));
            this.Controls.Add(Discount1.GetLabel());
            this.Controls.Add(Discount1.GetTextBox());

            if(this.readOnly)
            {
                FromQty.GetTextBox().Enabled = false;
                ToQty.GetTextBox().Enabled = false;
                PriceTier.GetTextBox().Enabled = false;
                DiscountType.GetTextBox().Enabled = false;
                Discount0.GetTextBox().Enabled = false;
                Discount1.GetTextBox().Enabled = false;
            }
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            int skuId = this.skuId;
            int? fromQty = null;
            if (!string.IsNullOrEmpty(this.FromQty.GetTextBox().Text))
                fromQty = int.Parse(this.FromQty.GetTextBox().Text);
            int? toQty = null;
            if (!string.IsNullOrEmpty(this.ToQty.GetTextBox().Text))
                toQty = int.Parse(this.ToQty.GetTextBox().Text);
            int? priceTier = null;
            if (!string.IsNullOrEmpty(this.PriceTier.GetTextBox().Text))
                priceTier = int.Parse(this.PriceTier.GetTextBox().Text);
            int? discountType = null;
            if (!string.IsNullOrEmpty(this.DiscountType.GetTextBox().Text))
                discountType = int.Parse(this.DiscountType.GetTextBox().Text);
            decimal? discount0 = null;
            if (!string.IsNullOrEmpty(this.Discount0.GetTextBox().Text))
                discount0 = decimal.Parse(this.Discount0.GetTextBox().Text);
            decimal? discount1 = null;
            if (!string.IsNullOrEmpty(this.Discount1.GetTextBox().Text))
                discount1 = decimal.Parse(this.Discount1.GetTextBox().Text);

            bool refreshData = false;

            if (this.skuQtyDiscountId == 0) refreshData = SKUQtyDiscountModelObj.AddSkuQtyDiscount(skuId, fromQty, toQty, priceTier, discountType, discount0, discount1);
            else refreshData = SKUQtyDiscountModelObj.UpdateSkuQtyDiscount(skuId, fromQty, toQty, priceTier, discountType, discount0, discount1, this.skuQtyDiscountId);

            string modeText = skuQtyDiscountId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the vendor.");
        }

        public void setDetails(SKUQtyDiscount skuQtyDiscount)
        {
            this.skuQtyDiscountId = skuQtyDiscount.id;

            this.FromQty.GetTextBox().Text = skuQtyDiscount.fromQty.ToString();
            this.ToQty.GetTextBox().Text = skuQtyDiscount.toQty.ToString();
            this.PriceTier.GetTextBox().Text = skuQtyDiscount.priceTier.ToString();
            this.DiscountType.GetTextBox().Text = skuQtyDiscount.discountType.ToString();
            this.Discount0.GetTextBox().Text = skuQtyDiscount.discount0.ToString();
            this.Discount1.GetTextBox().Text = skuQtyDiscount.discount1.ToString();
        }
    }
}
