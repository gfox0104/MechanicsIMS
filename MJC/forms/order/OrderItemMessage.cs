using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.order
{
    public partial class OrderItemMessage : BasicModal
    {
        private ModalButton MBOk = new ModalButton("(Enter) OK", Keys.Enter);
        private Button MBOk_button;
        private FInputBox Message = new FInputBox("Message:");

        private int orderItemId = 0;
        private OrderItemsModel OrderItemModelObj = new OrderItemsModel();

        public OrderItemMessage() : base("Add Message")
        {
            InitializeComponent();

            this.Size = new Size(600, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitMBOKButton();
            InitInputBox();
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
            var orderItem = OrderItemModelObj.GetOrderItemMessageById(_id);

            this.Message.GetTextBox().Text = orderItem.message;
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            string message = this.Message.GetTextBox().Text;
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("please enter Message.");
                this.Message.GetTextBox().Select();
                return;
            }

            bool refreshData = false;
            if (orderItemId == 0)
                Console.WriteLine("Create Order Message");
            else refreshData = OrderItemModelObj.UpdateOrderItemMessageById(message, this.orderItemId);

            string modeText = orderItemId == 0 ? "creating" : "updating";

            if (refreshData)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("An Error occured while " + modeText + " the Order item message.");
            }
        }
    }
}
