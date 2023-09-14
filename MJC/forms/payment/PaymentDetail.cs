using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.payment
{
    public partial class PaymentDetail : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private HotkeyButton hkEdits = new HotkeyButton("F4", "Edit Inv#", Keys.F4);

        private FlabelConstant CheckNumber = new FlabelConstant("Check#:", 110);
        private FlabelConstant DatePaid = new FlabelConstant("Date:", 75);

        private GridViewOrigin PaymentDetailGrid = new GridViewOrigin();
        private DataGridView PDGridRefer;

        
        private string checkNumber = "";
        private string datePaid = "";

        public PaymentDetail(string checkNumber, string datePaid) : base("Payment Details", "Review payments on an invoice")
        {
            InitializeComponent();
            _initBasicSize();
            this.checkNumber = checkNumber;
            this.datePaid = datePaid;

            HotkeyButton[] hkButtons = new HotkeyButton[2] { hkEdits, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitInputBox();
            InitPymtDetailList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadPaymentDetailList();
            };
        }

        private void AddHotKeyEvents()
        {
            hkEdits.GetButton().Click += (sender, e) =>
            {
                updatePayment();
            };
        }

        private void InitInputBox()
        {
            List<dynamic> FormComponents = new List<dynamic>();
            CheckNumber.SetContext(this.checkNumber.ToString());
            DatePaid.SetContext(this.datePaid.ToString());

            FormComponents.Add(CheckNumber);
            FormComponents.Add(DatePaid);

            _addFormInputs(FormComponents, 30, 110, 180, 42, 80);
        }

        private void InitPymtDetailList()
        {
            PDGridRefer = PaymentDetailGrid.GetGrid();
            PDGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            PDGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            PDGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            PDGridRefer.Location = new Point(0, 160);
            PDGridRefer.Width = this.Width;
            PDGridRefer.Height = this.Height - 295;
            PDGridRefer.VirtualMode = true;
            this.Controls.Add(PDGridRefer);
            this.PDGridRefer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.paymentDetailGridView_CellDoubleClick);
        }

        private void LoadPaymentDetailList()
        {
            string filter = "";
            var refreshData = Session.PymtDetailModelObj.LoadPaymentHistoryData(filter);
            if (refreshData)
            {
                PDGridRefer.DataSource = Session.PymtDetailModelObj.PaymentDetailDataList;
                PDGridRefer.Columns[0].Visible = false;
                PDGridRefer.Columns[1].HeaderText = "Invoice#";
                PDGridRefer.Columns[1].Width = 300;
                PDGridRefer.Columns[2].HeaderText = "Amount";
                PDGridRefer.Columns[2].Width = 400;
                PDGridRefer.Columns[3].HeaderText = "Discount";
                PDGridRefer.Columns[3].Width = 200;
            }
        }

        private void updatePayment()
        {
            OrderDetail detailModal = new OrderDetail();

            int rowIndex = PDGridRefer.CurrentCell.RowIndex;
            DataGridViewRow row = PDGridRefer.Rows[rowIndex];

            int paymentId = (int)row.Cells[0].Value;

            string checkNumber = row.Cells[1].Value.ToString();
            double amount = Convert.ToDouble(row.Cells[2].Value.ToString());
            double discount = Convert.ToDouble(row.Cells[3].Value.ToString());

            detailModal.setDetails(checkNumber, amount, discount, paymentId);

            if (detailModal.ShowDialog() == DialogResult.OK)
            {
                LoadPaymentDetailList();
            }
        }

        private void paymentDetailGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            updatePayment();
        }
    }
}
