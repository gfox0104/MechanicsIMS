using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.payment
{
    public partial class OrderDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;

        private FInputBox checkNumber = new FInputBox("Check#");
        private FInputBox amount = new FInputBox("Amount");
        private FInputBox discount = new FInputBox("Discount");

        private int paymentId;
        
        public OrderDetail() : base("Edit PaymentDetail")
        {
            InitializeComponent();
            this.Size = new Size(600, 310);
            this.StartPosition = FormStartPosition.CenterScreen;
            InitMBOKButton();
            InitInputBox();

            //this.Load += (s, e) => PriceTierDetail_Load(s, e);
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 200);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            checkNumber.SetPosition(new Point(30, 30));
            this.Controls.Add(checkNumber.GetLabel());
            this.Controls.Add(checkNumber.GetTextBox());
            checkNumber.GetTextBox().KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                {
                    SelectNextControl((Control)s, true, true, true, true);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift))
                {
                    SelectNextControl((Control)s, false, true, true, true);
                    e.Handled = true;
                }
            };

            amount.SetPosition(new Point(30, 80));
            this.Controls.Add(amount.GetLabel());
            this.Controls.Add(amount.GetTextBox());
            amount.GetTextBox().KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                {
                    SelectNextControl((Control)s, true, true, true, true);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift))
                {
                    SelectNextControl((Control)s, false, true, true, true);
                    e.Handled = true;
                }
            };

            discount.SetPosition(new Point(30, 130));
            this.Controls.Add(discount.GetLabel());
            this.Controls.Add(discount.GetTextBox());
            discount.GetTextBox().KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter)
                {
                    SelectNextControl((Control)s, true, true, true, true);
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.Up || (e.KeyCode == Keys.Enter && e.Modifiers == Keys.Shift))
                {
                    SelectNextControl((Control)s, false, true, true, true);
                    e.Handled = true;
                }
            };
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string checkNumber = this.checkNumber.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(checkNumber))
            {
                MessageBox.Show("please enter Price Tier Name");
                this.checkNumber.GetTextBox().Select();
                return;
            }
            if (!double.TryParse(this.amount.GetTextBox().Text, out double amount))
            {
                MessageBox.Show("please enter a number");
                this.amount.GetTextBox().Text = "";
                this.amount.GetTextBox().Focus();
                return;
            }
            if (!double.TryParse(this.discount.GetTextBox().Text, out double discount))
            {
                MessageBox.Show("please enter a number");
                this.discount.GetTextBox().Text = "";
                this.discount.GetTextBox().Focus();
                return;
            }

            bool refreshData = false;

            if (paymentId == 0)
                Console.WriteLine("Create payment");
            else refreshData = Session.paymentDetailModelObj.UpdatePayment(checkNumber, amount, discount, paymentId);

            string modeText = paymentId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the pricetier.");
        }

        public void setDetails(string checkNumber, double amount, double discount, int paymentId)
        {
            this.checkNumber.GetTextBox().Text = checkNumber;
            this.amount.GetTextBox().Text = amount.ToString();
            this.discount.GetTextBox().Text = discount.ToString();
            this.paymentId = paymentId;
        }
    }
}
