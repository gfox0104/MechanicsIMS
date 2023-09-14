using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.order
{
    public partial class OrderItemMessage : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;
        public FInputBox Message = new FInputBox("Message:");

        private int orderItemId = 0;
 
        public OrderItemMessage() : base("Add Message")
        {
            InitializeComponent();

            this.Size = new Size(600, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitMBOKButton();
            InitInputBox();

            Message.GetTextBox().TabIndex = 0;
            Message.GetTextBox().Focus();
            Message.GetTextBox().Select();
        }

        private void InitMBOKButton()
        {
            ModalButton_HotKeyDown(MBOk);
            MBOk_button = MBOk.GetButton();
            MBOk_button.Location = new Point(425, 120);
            MBOk_button.Click += (sender, e) => ok_button_Click(sender, e);
            this.Controls.Add(MBOk_button);
        }

        private void InitInputBox()
        {
            Message.SetPosition(new Point(30, 30));
            this.Controls.Add(Message.GetLabel());
            this.Controls.Add(Message.GetTextBox());

        }

        public void setDetails(int _id)
        {
            orderItemId = _id;
            var orderItem = Session.OrderItemModelObj.GetOrderItemMessageById(_id);
            if (orderItem != null)
            {
                this.Message.GetTextBox().Text = orderItem.message;
                this.Message.GetTextBox().Select();
                this.Message.GetTextBox().Focus();
            }
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string message = this.Message.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Please enter a message.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Message.GetTextBox().Select();
                return;
            }

            string modeText = orderItemId == 0 ? "creating" : "updating";
 
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
