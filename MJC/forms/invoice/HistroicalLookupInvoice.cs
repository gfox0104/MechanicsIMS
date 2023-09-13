using MJC.common.components;
using MJC.common;
using MJC.model;
using MJC.forms.payment;
using MJC.forms.order;

namespace MJC.forms.invoice
{
    public partial class HistroicalLookupInvoice : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkInvocieDetails = new HotkeyButton("F2", "Invoice Details", Keys.F2);
        private HotkeyButton hkCustomers = new HotkeyButton("F4", "Customers", Keys.F4);
        private HotkeyButton hkRecurringPymt = new HotkeyButton("F6", "Recurring Payment", Keys.F6);
        private HotkeyButton hkOrderEntry = new HotkeyButton("F8", "Order Entry", Keys.F8);
        private HotkeyButton hkPymtHistory = new HotkeyButton("F9", "Pymt History", Keys.F9);

        private FlabelConstant CustomerNumber = new FlabelConstant("Customer#");
        private FlabelConstant CustomerName = new FlabelConstant("Name");
        private FlabelConstant Address = new FlabelConstant("Address");
        private FlabelConstant City = new FlabelConstant("City");
        private FlabelConstant State = new FlabelConstant("State");
        private FlabelConstant Zipcode = new FlabelConstant("Zip");
        private FlabelConstant AccountBalance = new FlabelConstant("Account Balance");
        private FlabelConstant YTDPurchases = new FlabelConstant("YTD Purchases");
        private FlabelConstant YTDInterest = new FlabelConstant("YTDInterest");

        private GridViewOrigin InvoiceLookupGrid = new GridViewOrigin();
        private DataGridView ILGridRefer;
        
        private int customerId;

        public HistroicalLookupInvoice(int customerId) : base("Historical Invoice Lookup", "Displays invoices of a customer")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[8] { hkAdds, hkDeletes, hkEdits, hkInvocieDetails, hkCustomers, hkRecurringPymt, hkOrderEntry, hkPymtHistory };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            this.customerId = customerId;
            InitInputBox();
            InitInvoiceList();
        }

        private void AddHotKeyEvents()
        {
            hkInvocieDetails.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;

                if (ILGridRefer.Rows.Count > 0)
                {
                    int rowIndex = ILGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = ILGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[1].Value;
                }

                InvoiceInformation InvoiceInfoModal = new InvoiceInformation();
                InvoiceInfoModal.setDetails(selectedCustomerId);
                _navigateToForm(sender, e, InvoiceInfoModal);
                this.Hide();
            };
            hkPymtHistory.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;

                if (ILGridRefer.Rows.Count > 0)
                {
                    int rowIndex = ILGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = ILGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[1].Value;
                }

                PaymentHistory PaymentHistoryModal = new PaymentHistory(selectedCustomerId);
                _navigateToForm(sender, e, PaymentHistoryModal);
                this.Hide();
            };
            hkOrderEntry.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;

                if (ILGridRefer.Rows.Count > 0)
                {
                    int rowIndex = ILGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = ILGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[1].Value;
                }

                ProcessOrder processForm = new ProcessOrder();
                //processForm.LoadOrderListByCustomerId(selectedCustomerId);
                _navigateToForm(sender, e, processForm);
            };
        }

        private void InitInputBox()
        {
            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(CustomerNumber);
            FormComponents.Add(CustomerName);
            FormComponents.Add(Address);

            List<dynamic> LineComponents = new List<dynamic>();

            City.GetLabel().Width = 60;
            LineComponents.Add(City);

            State.GetLabel().Width = 60;
            LineComponents.Add(State);

            Zipcode.GetLabel().Width = 60;
            LineComponents.Add(Zipcode);

            FormComponents.Add(LineComponents);
            FormComponents.Add(AccountBalance);
            FormComponents.Add(YTDPurchases);
            FormComponents.Add(YTDInterest);

            _addFormInputs(FormComponents, 30, 110, 800, 42, 250);

            setDetails(this.customerId);
        }

        private void InitInvoiceList()
        {
            ILGridRefer = InvoiceLookupGrid.GetGrid();
            ILGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            ILGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            ILGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            ILGridRefer.Location = new Point(0, 300);
            ILGridRefer.Width = this.Width;
            ILGridRefer.Height = this.Height - 295;
            this.Controls.Add(ILGridRefer);

            LoadInvoiceList();
        }

        private void LoadInvoiceList()
        {
            var refreshData = Session.InvoicesModelObj.LoadHistoricalInvoiceData(this.customerId);
            if (refreshData)
            {
                ILGridRefer.DataSource = Session.InvoicesModelObj.HistoricalInvoiceDataList;
                ILGridRefer.Columns[0].Visible = false;
                ILGridRefer.Columns[1].HeaderText = "Customer id";
                ILGridRefer.Columns[1].Visible = false;
                ILGridRefer.Columns[2].HeaderText = "Invoice #";
                ILGridRefer.Columns[2].Width = 300;
                ILGridRefer.Columns[3].HeaderText = "Date";
                ILGridRefer.Columns[3].Width = 300;
                ILGridRefer.Columns[4].HeaderText = "Date Paid";
                ILGridRefer.Columns[4].Width = 300;
                ILGridRefer.Columns[5].HeaderText = "Days";
                ILGridRefer.Columns[5].Width = 300;
                ILGridRefer.Columns[6].HeaderText = "Description";
                ILGridRefer.Columns[6].Width = 500;
            }
        }

        public void setDetails(int customerId)
        {
            var customerData = Session.CustomersModelObj.GetCustomerData(customerId);
            if (customerData != null)
            {
                if (!string.IsNullOrEmpty(customerData.customerNumber)) CustomerNumber.SetContext(customerData.customerNumber.ToString());
                else CustomerName.SetContext("N/A");

                if (customerData.customerName != "") CustomerName.SetContext(customerData.customerName);
                else CustomerName.SetContext("N/A");

                if (customerData.address1 != "") this.Address.SetContext(customerData.address1);
                else this.Address.SetContext("N/A");

                if (customerData.city != "") this.Zipcode.SetContext(customerData.city);
                else this.City.SetContext("N/A");

                if (customerData.state != "") this.State.SetContext(customerData.state);
                else this.State.SetContext("N/A");

                if (customerData.zipcode != "") this.Zipcode.SetContext(customerData.zipcode);
                else this.Zipcode.SetContext("N/A");

                if (customerData.accountBalance != "") this.AccountBalance.SetContext(customerData.accountBalance);
                else this.AccountBalance.SetContext("N/A");

                if (customerData.yearToDatePurchases != "") this.YTDPurchases.SetContext(customerData.yearToDatePurchases);
                else this.YTDPurchases.SetContext("N/A");

                if (customerData.yearToDateInterest != "") this.YTDInterest.SetContext(customerData.yearToDateInterest);
                else this.YTDInterest.SetContext("N/A");
            }
        }
    }
}
