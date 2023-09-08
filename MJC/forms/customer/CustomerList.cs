using MJC.common.components;
using MJC.common;
using MJC.model;
using MJC.forms.invoice;
using MJC.forms.payment;
using MJC.forms.order;
using MJC.qbo;

namespace MJC.forms.customer
{
    public partial class CustomerList : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkDeletes = new HotkeyButton("Del", "Deletes", Keys.Delete);
        private HotkeyButton hkSelects = new HotkeyButton("Enter", "Selects", Keys.Enter);
        private HotkeyButton hkCustSummary = new HotkeyButton("F2", "Cust Summary", Keys.F2);
        private HotkeyButton hkInvocies = new HotkeyButton("F5", "Invoices", Keys.F5);
        private HotkeyButton hkHistoryInv = new HotkeyButton("F6", "Historical Inv", Keys.F6);
        private HotkeyButton hkOrderEntry = new HotkeyButton("F7", "Order Entry", Keys.F7);
        private HotkeyButton hkPaymentHistory = new HotkeyButton("F8", "Payment History", Keys.F8);
        private HotkeyButton hkReceivePayment = new HotkeyButton("F9", "Receive Payment", Keys.F9);
        private HotkeyButton hkArchivedCustomers = new HotkeyButton("F11", "Archived Customers", Keys.F11);

        private GridViewOrigin customerListGrid = new GridViewOrigin();
        private DataGridView CLGridRefer;

        private CustomersModel CustomersModelObj = new CustomersModel();
        private bool archievedView = false;

        public CustomerList() : base("Customer List", "Manage customers in the system")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[10] { hkAdds, hkDeletes, hkSelects, hkCustSummary, hkInvocies, hkHistoryInv, hkOrderEntry, hkPaymentHistory, hkReceivePayment, hkArchivedCustomers };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitCustomerList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadCustomerList();
            };
        }
        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (sender, e) =>
            {
                CustomerInformation detailModal = new CustomerInformation();
                _navigateToForm(sender, e, detailModal);
                this.Hide();
                //                if (detailModal.ShowDialog() == DialogResult.OK)
                //                {
                //                    LoadCustomerList("");
                //                }
            };
            hkDeletes.GetButton().Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int selectedCustomerId = 0;
                    if (CLGridRefer.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in CLGridRefer.SelectedRows)
                        {
                            selectedCustomerId = (int)row.Cells[0].Value;
                        }
                    }
                    bool refreshData = CustomersModelObj.DeleteCustomer(selectedCustomerId);
                    if (refreshData)
                    {
                        LoadCustomerList();
                    }
                }
            };
            hkSelects.GetButton().Click += (sender, e) =>
            {
                updateCustomer(sender, e);
            };
            hkCustSummary.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;
                string customerName = "";

                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[0].Value;
                    customerName = row.Cells[2].Value.ToString();
                }

                CustomerSummary CustomerSumyModal = new CustomerSummary(selectedCustomerId, customerName);
                _navigateToForm(sender, e, CustomerSumyModal);
                this.Hide();
            };
            hkHistoryInv.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;
                string customerName = "";

                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[0].Value;
                    customerName = row.Cells[1].Value.ToString();
                }

                HistroicalLookupInvoice HistoricalLookupInvoiceModal = new HistroicalLookupInvoice(selectedCustomerId);
                _navigateToForm(sender, e, HistoricalLookupInvoiceModal);
                HistoricalLookupInvoiceModal.FormClosed += (ss, sargs) =>
                {
                    this.LoadCustomerList();
                };
                this.Hide();
            };
            hkPaymentHistory.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;
                string customerName = "";

                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[0].Value;
                    customerName = row.Cells[2].Value.ToString();
                }

                PaymentHistory PaymentHistoryModal = new PaymentHistory(selectedCustomerId);
                _navigateToForm(sender, e, PaymentHistoryModal);
                this.Hide();
            };
            hkReceivePayment.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;
                string customerName = "";

                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[0].Value;
                    customerName = row.Cells[2].Value.ToString();
                }

                ReceivePayment ReceivePymtModal = new ReceivePayment(selectedCustomerId);
                _navigateToForm(sender, e, ReceivePymtModal);
                this.Hide();
            };
            hkInvocies.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;

                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[0].Value;
                }

                LookupInvoice LookupInvoiceModal = new LookupInvoice();
                LookupInvoiceModal.setDetails(selectedCustomerId);
                _navigateToForm(sender, e, LookupInvoiceModal);
                this.Hide();
            };
            hkOrderEntry.GetButton().Click += (sender, e) =>
            {
                int selectedCustomerId = 0;
                string customerName = "";

                if (CLGridRefer.Rows.Count > 0)
                {
                    int rowIndex = CLGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = CLGridRefer.Rows[rowIndex];
                    selectedCustomerId = (int)row.Cells[0].Value;
                    customerName = row.Cells[2].Value.ToString();
                }

                ProcessOrder processForm = new ProcessOrder(selectedCustomerId, 0);
                _navigateToForm(sender, e, processForm);
            };
            hkArchivedCustomers.GetButton().Click += (sender, e) =>
            {
                if (archievedView)
                {
                    hkArchivedCustomers.GetLabel().Text = "Archived Customers";
                    this._changeFormText("Customer List");
                    archievedView = false;
                }
                else
                {
                    hkArchivedCustomers.GetLabel().Text = "Active Customers";
                    this._changeFormText("Archived - Customer List");
                    archievedView = true;
                }
                LoadCustomerList(archievedView);
            };
        }

        private void InitCustomerList()
        {
            CLGridRefer = customerListGrid.GetGrid();
            CLGridRefer.Location = new Point(0, 95);
            CLGridRefer.Width = this.Width;
            CLGridRefer.Height = this.Height - 295;
            CLGridRefer.MultiSelect = false;

            this.Controls.Add(CLGridRefer);
            this.CLGridRefer.CellDoubleClick += (sender, e) =>
            {
                updateCustomer(sender, e);
            };

            LoadCustomerList(true);
        }

        private void LoadCustomerList(bool archivedView = false)
        {
            string filter = "";
            var refreshData = CustomersModelObj.LoadCustomerData(filter, archivedView);
            if (refreshData)
            {
                CLGridRefer.DataSource = CustomersModelObj.CustomerDataList;
                CLGridRefer.Columns[0].Visible = false;
                CLGridRefer.Columns[1].HeaderText = "Customer #";
                CLGridRefer.Columns[1].DataPropertyName = "Num";
                CLGridRefer.Columns[1].Width = 300;

                CLGridRefer.Columns[2].HeaderText = "Name";

                CLGridRefer.Columns[2].Width = 300;
                CLGridRefer.Columns[3].HeaderText = "Address";
                CLGridRefer.Columns[3].Width = 500;
                CLGridRefer.Columns[4].HeaderText = "City";
                CLGridRefer.Columns[4].Width = 200;
                CLGridRefer.Columns[5].HeaderText = "State";
                CLGridRefer.Columns[5].Width = 200;
                CLGridRefer.Columns[6].HeaderText = "Zipcode";
                CLGridRefer.Columns[6].Width = 200;
                CLGridRefer.Columns[7].HeaderText = "QboId";
                CLGridRefer.Columns[7].Visible = false;
                CLGridRefer.Columns[8].HeaderText = "Email";
                CLGridRefer.Columns[8].Visible = false;
            }
        }

        private void updateCustomer(object sender, EventArgs e)
        {
            CustomerInformation detailModal = new CustomerInformation();

            if (CLGridRefer.Rows.Count > 0)
            {
                int rowIndex = CLGridRefer.CurrentCell.RowIndex;
                DataGridViewRow row = CLGridRefer.Rows[rowIndex];

                int id = (int)row.Cells[0].Value;
                this.Hide();
                detailModal.setDetails(id);
                _navigateToForm(sender, e, detailModal);

                //            if (detailModal.ShowDialog() == DialogResult.OK)
                //          {
                //            LoadCustomerList();
                //      }
            }
        }

        //private void LoadArchivedCustomerList()
        //{
        //    var refreshData = CustomersModelObj.GetCustomerArchived();
        //    if (refreshData)
        //    {
        //        CLGridRefer.DataSource = CustomersModelObj.CustomerDataList;
        //        CLGridRefer.Columns[0].Visible = false;
        //        CLGridRefer.Columns[1].HeaderText = "Customer #";
        //        CLGridRefer.Columns[1].Width = 300;
        //        CLGridRefer.Columns[2].HeaderText = "Name";
        //        CLGridRefer.Columns[2].Width = 300;
        //        CLGridRefer.Columns[3].HeaderText = "Address";
        //        CLGridRefer.Columns[3].Width = 500;
        //        CLGridRefer.Columns[4].HeaderText = "City";
        //        CLGridRefer.Columns[4].Width = 200;
        //        CLGridRefer.Columns[5].HeaderText = "State";
        //        CLGridRefer.Columns[5].Width = 200;
        //        CLGridRefer.Columns[6].HeaderText = "Zipcode";
        //        CLGridRefer.Columns[6].Width = 200;
        //    }
        //}
    }
}
