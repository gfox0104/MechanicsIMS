using MJC.common.components;
using MJC.common;
using MJC.model;

namespace MJC.forms.payment
{
    public partial class PaymentHistory : GlobalLayout
    {
        private HotkeyButton hkPreviousScreen = new HotkeyButton("Esc", "Previous Screen", Keys.Escape);

        private HotkeyButton hkOpenDetail = new HotkeyButton("Enter", "Open Details", Keys.Enter);
        private HotkeyButton hkReversePymt = new HotkeyButton("F4", "Reverse payment", Keys.F4);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Delete Info", Keys.Delete);

        private FlabelConstant CustomerNo = new FlabelConstant("Customer#:", 120);
        private FlabelConstant CustomerName = new FlabelConstant("Name:", 75);

        private GridViewOrigin PaymentHistoryGrid = new GridViewOrigin();
        private DataGridView PHGridRefer;

        
        private string customerNumber = "";
        private string customerName = "";
        private int customerId = 0;

        public PaymentHistory(int customerId) : base("Payment History", "History of payments")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[4] { hkOpenDetail, hkReversePymt, hkDeletes, hkPreviousScreen };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            dynamic customerData = Session.CustomerModelObj.GetCustomerDataById(customerId);
            this.customerId = customerId;
            this.customerNumber = customerData.customerNumber;
            this.customerName = customerData.displayName;

            InitInputBox();
            InitPymtHistoryList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadPaymentHistory();
            };
            this.Load += (s, e) =>
            {
                this.LoadPaymentHistory();
            };
        }

        private void AddHotKeyEvents()
        {
            hkOpenDetail.GetButton().Click += (sender, e) =>
            {
                string checkNumber = "";
                string datePaid = "";

                if (PHGridRefer.Rows.Count > 0)
                {
                    int rowIndex = PHGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = PHGridRefer.Rows[rowIndex];
                    checkNumber = row.Cells[1].Value.ToString();
                    DateTime dt = (DateTime)row.Cells[2].Value;
                    datePaid = dt.ToString("MM/dd/yyyy");
                }
                PaymentDetail PaymentDetailModal = new PaymentDetail(checkNumber, datePaid);
                _navigateToForm(sender, e, PaymentDetailModal);
                this.Hide();
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                if (PHGridRefer.SelectedRows.Count > 0)
                {
                    int selectedPymtHistoryId = 0;
                    DataGridViewRow row = PHGridRefer.SelectedRows[0];
                    selectedPymtHistoryId = (int)row.Cells[0].Value;
                    string syncToken = row.Cells[5].Value.ToString();
                    string qboPaymentId = row.Cells[6].Value.ToString();

                }

                //bool refreshData = PymtHistoryModelObj.DeletePaymentHistory(selectedPymtHistoryId);
                //if (refreshData)
                //{
                //    LoadPaymentHistory();
                //}
            };
            hkReversePymt.GetButton().Click += (sender, e) =>
            {
                int selectedPymtHistoryId = 0;
                bool reversed = false;
                if (PHGridRefer.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in PHGridRefer.SelectedRows)
                    {
                        selectedPymtHistoryId = (int)row.Cells[0].Value;
                        var reversedFlag = row.Cells[4].Value;
                        reversed = !Convert.ToBoolean(reversedFlag);
                    }
                }
                bool refreshData = Session.PymtHistoryModelObj.UpdateReverse(reversed, selectedPymtHistoryId);
                if (refreshData)
                {
                    LoadPaymentHistory();
                }
            };
        }

        private void InitInputBox()
        {
            List<dynamic> FormComponents = new List<dynamic>();

            CustomerNo.SetContext(this.customerNumber);
            CustomerName.SetContext(this.customerName);
            FormComponents.Add(CustomerNo);
            FormComponents.Add(CustomerName);

            _addFormInputs(FormComponents, 30, 110, 180, 42, 80);
        }

        private void InitPymtHistoryList()
        {
            PHGridRefer = PaymentHistoryGrid.GetGrid();
            PHGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            PHGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            PHGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            PHGridRefer.Location = new Point(0, 160);
            PHGridRefer.Width = this.Width;
            PHGridRefer.Height = this.Height - 295;
            PHGridRefer.VirtualMode = true;
            this.Controls.Add(PHGridRefer);
        }

        private void LoadPaymentHistory()
        {
            int filter = this.customerId;
            var refreshData = Session.PymtHistoryModelObj.LoadPaymentHistoryData(filter);
            if (refreshData)
            {
                PHGridRefer.DataSource = Session.PymtHistoryModelObj.PaymentHistoryDataList;
                PHGridRefer.Columns[0].Visible = false;
                PHGridRefer.Columns[1].HeaderText = "Check#";
                PHGridRefer.Columns[1].Width = 300;
                PHGridRefer.Columns[2].HeaderText = "Date";
                PHGridRefer.Columns[2].Width = 400;
                PHGridRefer.Columns[3].HeaderText = "Amount";
                PHGridRefer.Columns[3].Width = 200;
                PHGridRefer.Columns[4].HeaderText = "Reversed?";
                PHGridRefer.Columns[4].Width = 200;
                PHGridRefer.Columns[5].HeaderText = "SyncToken?";
                PHGridRefer.Columns[5].Visible = false;
                PHGridRefer.Columns[6].HeaderText = "QboPaymentId?";
                PHGridRefer.Columns[6].Visible = false;
            }
        }
    }
}
