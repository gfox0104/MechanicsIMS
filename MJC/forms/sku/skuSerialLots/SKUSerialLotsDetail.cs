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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJC.forms.sku.skuSerialLots
{
    public partial class SKUSerialLotsDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FInputBox SerialNumber = new FInputBox("Serial Number");
        private FInputBox LotNumber = new FInputBox("Lot Number");
        private FDateTime DateReceived = new FDateTime("Date Received");
        private FInputBox Cost = new FInputBox("Cost");
        private FInputBox InvoiceNumber = new FInputBox("Invoice #");
        private FInputBox PurchaseFrom = new FInputBox("Purchased from");

        private SKUSerialLotsModel SKUSerialLotsModelObj = new SKUSerialLotsModel();

        private int skuId = 0;
        private int skuSerialLotId = 0;

        public SKUSerialLotsDetail(int skuId) : base("Add SKU SerialLots")
        {
            InitializeComponent();
            this.Size = new Size(600, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.skuId = skuId;

            InitMBOKButton();
            InitInputBox();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 400);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            SerialNumber.SetPosition(new Point(30, 80));
            this.Controls.Add(SerialNumber.GetLabel());
            this.Controls.Add(SerialNumber.GetTextBox());

            LotNumber.SetPosition(new Point(30, 130));
            this.Controls.Add(LotNumber.GetLabel());
            this.Controls.Add(LotNumber.GetTextBox());

            DateReceived.SetPosition(new Point(30, 180));
            this.Controls.Add(DateReceived.GetLabel());
            this.Controls.Add(DateReceived.GetDateTimePicker());

            Cost.SetPosition(new Point(30, 230));
            this.Controls.Add(Cost.GetLabel());
            this.Controls.Add(Cost.GetTextBox());

            InvoiceNumber.SetPosition(new Point(30, 280));
            this.Controls.Add(InvoiceNumber.GetLabel());
            this.Controls.Add(InvoiceNumber.GetTextBox());

            PurchaseFrom.SetPosition(new Point(30, 330));
            this.Controls.Add(PurchaseFrom.GetLabel());
            this.Controls.Add(PurchaseFrom.GetTextBox());
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            int skuId = this.skuId;
            string serialNumber = this.SerialNumber.GetTextBox().Text;
            string lotNumber = this.LotNumber.GetTextBox().Text;
            DateTime dateReceived = this.DateReceived.GetDateTimePicker().Value;
            decimal? cost = null;
            if (!string.IsNullOrEmpty(this.Cost.GetTextBox().Text))
                cost = decimal.Parse(this.Cost.GetTextBox().Text);
            string invoiceId = this.InvoiceNumber.GetTextBox().Text;
            string purchaseFrom = this.PurchaseFrom.GetTextBox().Text;

            bool refreshData = false;

            if (this.skuSerialLotId == 0) refreshData = SKUSerialLotsModelObj.AddSKUSerialLot(skuId, serialNumber, lotNumber, dateReceived, cost, invoiceId, purchaseFrom);
            else refreshData = SKUSerialLotsModelObj.UpdateSKUSerialLot(skuId, serialNumber, lotNumber, dateReceived, cost, invoiceId, purchaseFrom, this.skuSerialLotId);

            string modeText = this.skuSerialLotId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the vendor.");
        }

        public void setDetails(SKUSerialLot skuSerialLot)
        {
            this.skuSerialLotId = skuSerialLot.id;
            if (skuSerialLot.dateReceived.HasValue)
            {
                this.DateReceived.GetDateTimePicker().Value = skuSerialLot.dateReceived.Value;
            }

            this.SerialNumber.GetTextBox().Text = skuSerialLot.serialNumber;
            this.LotNumber.GetTextBox().Text = skuSerialLot.lotNumber;
            this.Cost.GetTextBox().Text = skuSerialLot.cost.ToString();
            this.InvoiceNumber.GetTextBox().Text = skuSerialLot.invoiceId;
            this.PurchaseFrom.GetTextBox().Text = skuSerialLot.purchaseFrom;
        }
    }
}
