using MJC.common.components;
using MJC.common;
using MJC.model;
using System.Data;
using MJC.forms.customer;
using MJC.forms.payment;
using MJC.forms.order;

namespace MJC.forms.invoice
{
    public partial class LookupInvoice : GlobalLayout
    {

        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkEdits = new HotkeyButton("Enter", "Edits", Keys.Enter);
        private HotkeyButton hkInvocieDetails = new HotkeyButton("F2", "Invoice Details", Keys.F2);
        private HotkeyButton hkCustomers = new HotkeyButton("F4", "Customers", Keys.F4);
        private HotkeyButton hkRecurringPymt = new HotkeyButton("F6", "Receive Payment", Keys.F6);
        private HotkeyButton hkInvocies = new HotkeyButton("F7", "Historical Inv", Keys.F7);
        private HotkeyButton hkOrderEntry = new HotkeyButton("F8", "Order Entry", Keys.F8);
        private HotkeyButton hkPymtHistory = new HotkeyButton("F9", "Pymt History", Keys.F9);

        private FComboBox CustomerNum_ComBo = new FComboBox("Customer#:", 150);
        private FlabelConstant CustomerName = new FlabelConstant("Name:");
        private FlabelConstant Address = new FlabelConstant("Address:");
        private FlabelConstant City = new FlabelConstant("City:");
        private FlabelConstant State = new FlabelConstant("State:");
        private FlabelConstant Zipcode = new FlabelConstant("Zip:");
        private FlabelConstant AccountBalance = new FlabelConstant("Account Balance:");
        private FlabelConstant YTDPurchases = new FlabelConstant("YTD Purchases:");
        private FlabelConstant YTDInterest = new FlabelConstant("YTDInterest:");

        private GridViewOrigin InvoiceLookupGrid = new GridViewOrigin();
        private DataGridView ILGridRefer;


        private int customerId;

        public LookupInvoice(int customerId = 0) : base("Invoice Lookup", "Displays invoices of a customer")
        {
            InitializeComponent();
            _initBasicSize();
            this.customerId = customerId;

            HotkeyButton[] hkButtons = new HotkeyButton[9] { hkAdds, hkDeletes, hkEdits, hkInvocieDetails, hkCustomers, hkRecurringPymt, hkInvocies, hkOrderEntry, hkPymtHistory };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitInvoiceList();
            InitInputBox();
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                InvoiceInformation invoiceDetail = new InvoiceInformation();
                _navigateToForm(sender, e, invoiceDetail);
                this.Hide();
            };
            hkInvocieDetails.GetButton().Click += (sender, e) =>
            {
                updateInvoice(sender, e);
            };
            hkInvocies.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;

                if (ILGridRefer.Rows.Count > 0)
                {
                    int rowIndex = ILGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = ILGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[1].Value;
                }
                HistroicalLookupInvoice HistoricalInvoiceModal = new HistroicalLookupInvoice(selectedCustomerId);
                _navigateToForm(sender, e, HistoricalInvoiceModal);
                this.Hide();
            };
            hkCustomers.GetButton().Click += (sender, e) =>
            {
                CustomerList CustomerListModal = new CustomerList();
                _navigateToForm(sender, e, CustomerListModal);
                this.Hide();
            };
            hkRecurringPymt.GetButton().Click += (sender, e) =>
            {
                int customerId = int.Parse(CustomerNum_ComBo.GetComboBox().SelectedValue.ToString());
                ReceivePayment ReceivePaymentModal = new ReceivePayment(customerId);
                _navigateToForm(sender, e, ReceivePaymentModal);
                this.Hide();
            };
            hkPymtHistory.GetButton().Click += (sender, e) =>
            {
                int customerId = int.Parse(CustomerNum_ComBo.GetComboBox().SelectedValue.ToString());
                PaymentHistory PymtHistoryModal = new PaymentHistory(customerId);
                _navigateToForm(sender, e, PymtHistoryModal);
                this.Hide();
            };
            hkOrderEntry.GetButton().Click += (sender, e) =>
            {
                int customerId = int.Parse(CustomerNum_ComBo.GetComboBox().SelectedValue.ToString());
                ProcessOrder processOrderModal = new ProcessOrder(customerId, 0);
                _navigateToForm(sender, e, processOrderModal);
                this.Hide();
            };
        }

        private void InitInputBox()
        {
            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(CustomerNum_ComBo);
            FormComponents.Add(CustomerName);
            FormComponents.Add(Address);

            List<dynamic> LineComponents = new List<dynamic>();

            City.GetLabel().Width = 60;
            //City.GetTextBox().Width = 160;
            LineComponents.Add(City);

            State.GetLabel().Width = 60;
            //State.GetTextBox().Width = 100;
            LineComponents.Add(State);

            Zipcode.GetLabel().Width = 60;
            //Zipcode.GetTextBox().Width = 100;
            LineComponents.Add(Zipcode);

            FormComponents.Add(LineComponents);
            FormComponents.Add(AccountBalance);
            FormComponents.Add(YTDPurchases);
            FormComponents.Add(YTDInterest);

            _addFormInputs(FormComponents, 30, 110, 800, 42, 250);

            List<CustomerData> customerList = new List<CustomerData>();
            bool refreshData = Session.CustomersModelObj.LoadCustomerData("", false);
            CustomerNum_ComBo.GetComboBox().DataSource = Session.CustomersModelObj.CustomerDataList;
            CustomerNum_ComBo.GetComboBox().ValueMember = "Id";
            CustomerNum_ComBo.GetComboBox().DisplayMember = "Num";

            CustomerNum_ComBo.GetComboBox().SelectedIndexChanged += new EventHandler(CustomerNumComBo_SelectedIndexChanged);

            if(CustomerNum_ComBo.GetComboBox().Items.Count > 0)
            {
                if (customerId == 0)
                {
                    CustomerNum_ComBo.GetComboBox().SelectedIndex = 0;
                    CustomerNumComBo_SelectedIndexChanged(CustomerNum_ComBo.GetComboBox(), EventArgs.Empty);
                }
                else
                {
                    int index = CustomerNum_ComBo.GetComboBox().Items.Cast<FComboBoxItem>().ToList().FindIndex(item => item.Id == customerId);
                    CustomerNum_ComBo.GetComboBox().SelectedIndex = index;
                    CustomerNumComBo_SelectedIndexChanged(CustomerNum_ComBo.GetComboBox(), EventArgs.Empty);
                }
            }
        }

        private void CustomerNumComBo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int customerId = (int)CustomerNum_ComBo.GetComboBox().SelectedValue;
            setDetails(customerId);
            LoadInvoiceList(customerId);
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
            ILGridRefer.VirtualMode = false;
            this.Controls.Add(ILGridRefer);

            this.ILGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateInvoice(sender, e);
            };

            LoadInvoiceList();
        }

        private void LoadInvoiceList(int customerId = 0)
        {
            List<InvoiceData> InvoiceDataList = Session.InvoicesModelObj.LoadInvoiceData(customerId);
       
            ILGridRefer.DataSource = InvoiceDataList;
            ILGridRefer.Columns[0].Visible = false;
            ILGridRefer.Columns[1].HeaderText = "CustomerId";
            ILGridRefer.Columns[1].Visible = false;
            ILGridRefer.Columns[2].HeaderText = "Invoice #";
            ILGridRefer.Columns[2].Width = 300;
            ILGridRefer.Columns[3].HeaderText = "Date";
            ILGridRefer.Columns[3].Width = 300;
            ILGridRefer.Columns[4].HeaderText = "Description";
            ILGridRefer.Columns[4].Width = 500;
            ILGridRefer.Columns[5].HeaderText = "Total";
            ILGridRefer.Columns[5].Width = 200;
            ILGridRefer.Columns[6].HeaderText = "InvoiceBalance";
            ILGridRefer.Columns[6].Visible = false;
        }

        private void updateInvoice(object sender, EventArgs e)
        {
            InvoiceInformation detailModal = new InvoiceInformation();
            if (ILGridRefer.Rows.Count > 0)
            {
                int rowIndex = ILGridRefer.CurrentCell.RowIndex;
                DataGridViewRow row = ILGridRefer.Rows[rowIndex];

                int id = (int)row.Cells[0].Value;
                this.Hide();
                detailModal.setDetails(id);
                _navigateToForm(sender, e, detailModal);

                //if (detailModal.ShowDialog() == DialogResult.OK)
                //{
                //    LoadCustomerList();
                //}
            }
        }

        public void setDetails(int customerId)
        {
            var customerData = Session.CustomersModelObj.GetCustomerData(customerId);
            if (customerData != null)
            {
                CustomerNum_ComBo.GetComboBox().SelectedValue = customerId;
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
