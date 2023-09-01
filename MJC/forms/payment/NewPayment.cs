using MJC.common.components;
using MJC.common;

namespace MJC.forms.payment
{
    public partial class NewPayment : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;
        private FInputBox CustomerName = new FInputBox("Customer Name:");
        private FDateTime DateReceived = new FDateTime("Date Received:");
        private FInputBox AmtReceived = new FInputBox("Amt Received:");

        private int customerId = 0;

        public NewPayment(int customerId) : base("New Payment")
        {
            InitializeComponent();
            this.customerId = customerId;

            this.Size = new Size(600, 320);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitMBOKButton();
            InitInputBox();

            this.Load += (s, e) => CustomerInfo_Load(s, e);
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 220);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            CustomerName.SetPosition(new Point(30, 30));
            this.Controls.Add(CustomerName.GetLabel());
            this.Controls.Add(CustomerName.GetTextBox());

            DateReceived.SetPosition(new Point(30, 80));
            this.Controls.Add(DateReceived.GetLabel());
            this.Controls.Add(DateReceived.GetDateTimePicker());

            AmtReceived.SetPosition(new Point(30, 130));
            this.Controls.Add(AmtReceived.GetLabel());
            this.Controls.Add(AmtReceived.GetTextBox());
        }

        public void CustomerInfo_Load(object sender, EventArgs e)
        {
            CustomerName.GetTextBox().Select();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string customerName = this.CustomerName.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(customerName))
            {
                MessageBox.Show("please enter Customer Name");
                this.CustomerName.GetTextBox().Select();
                return;
            }
            //DateTime dateReceived = this.DateReceived.GetDateTimePicker().Value;

            string amtReceived = this.AmtReceived.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(amtReceived))
            {
                MessageBox.Show("please enter Amt Received");
                this.AmtReceived.GetTextBox().Select();
                return;
            }

            bool refreshData = true;
            //if (creditCodeId == 0)
            //    refreshData = CreditCodeModelObj.AddCreditCode(creditCode, paymentAllowed, creditLimit);
            //else refreshData = CreditCodeModelObj.UpdateCreditCode(creditCode, paymentAllowed, creditLimit, creditCodeId);

            //string modeText = creditCodeId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + "modeText" + " the credit Code.");
        }
    }
}
