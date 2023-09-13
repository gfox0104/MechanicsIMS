using MJC.common.components;
using MJC.common;
using MJC.forms.creditcode;
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
using static MJC.model.CreditCardModel;

namespace MJC.forms.creditcard
{
    public partial class CreditCardsDetail : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;
        private FInputBox CardNumber = new FInputBox("Card Number: ");
        private FInputBox CardType = new FInputBox("Card Type: ");
        private FDateTime Expires = new FDateTime("Expires: ");
        private FInputBox Priority = new FInputBox("Priority: ");

        private int creditCardId = 0;
        private CreditCardModel CreditCardModelObj = new CreditCardModel();
        private int customerId = 0;

        public CreditCardsDetail(int customerId) : base("Add Credit Card")
        {
            InitializeComponent();

            this.Size = new Size(600, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.customerId = customerId;

            InitMBOKButton();
            InitInputBox();

            this.Load += (s, e) => CreditCardDetail_Load(s, e);
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 250);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            CardNumber.SetPosition(new Point(30, 30));
            CardNumber.GetTextBox().MaxLength = 20;
            this.Controls.Add(CardNumber.GetLabel());
            this.Controls.Add(CardNumber.GetTextBox());

            CardType.SetPosition(new Point(30, 80));
            this.Controls.Add(CardType.GetLabel());
            this.Controls.Add(CardType.GetTextBox());

            Expires.SetPosition(new Point(30, 130));
            this.Controls.Add(Expires.GetLabel());
            this.Controls.Add(Expires.GetDateTimePicker());

            Priority.SetPosition(new Point(30, 180));
            this.Controls.Add(Priority.GetLabel());
            this.Controls.Add(Priority.GetTextBox());

            Priority.GetTextBox().KeyPress += validateNumber;
        }

        private void CreditCardDetail_Load(object sender, EventArgs e)
        {
            CardNumber.GetTextBox().Select();
        }

        private void validateNumber(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        public void setDetails(int _id)
        {
            this.creditCardId = _id;
            CreditCardData creditCardData = CreditCardModelObj.GetCreditCardData(_id);

            this.CardNumber.GetTextBox().Text = creditCardData.CardNumber;
            this.CardType.GetTextBox().Text = creditCardData.CardType;
            this.Expires.GetDateTimePicker().Value = creditCardData.Expires;
            this.Priority.GetTextBox().Text = creditCardData.Priority.ToString();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string cardNumber = this.CardNumber.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                MessageBox.Show("please enter Credit Card.");
                this.CardNumber.GetTextBox().Select();
                return;
            }
            string cardType = this.CardType.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(cardType))
            {
                MessageBox.Show("please enter Card Type");
                this.CardType.GetTextBox().Select();
                return;
            }
            DateTime expires = this.Expires.GetDateTimePicker().Value;
           
            string priorityStr = this.Priority.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(priorityStr))
            {
                MessageBox.Show("please enter Priority");
                this.Priority.GetTextBox().Select();
                return;
            }
            int priority = int.Parse(priorityStr);

            bool active = true;
            int createdBy = 1;
            int updatedBy = 1;

            bool refreshData = false;
            if (creditCardId == 0)
                refreshData = CreditCardModelObj.AddCreditCard(customerId, cardNumber, cardType, expires, priority, active, createdBy, updatedBy);
            else refreshData = CreditCardModelObj.UpdateCreditCard(cardNumber, cardType, expires, priority, active, createdBy, updatedBy, creditCardId);

            string modeText = creditCardId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else MessageBox.Show("An Error occured while " + modeText + " the credit Card.");
        }
    }
}
