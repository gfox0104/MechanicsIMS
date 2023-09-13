using MJC.common.components;
using MJC.common;
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
using MJC.forms.customer;
using Antlr4.Runtime.Tree;
using MJC.qbo;

namespace MJC.forms
{
    public partial class ReceivePayment : GlobalLayout
    {

        private HotkeyButton hkReceivePayment = new HotkeyButton("F8", "Receive Payment", Keys.F8);
        private HotkeyButton hkCancel = new HotkeyButton("Esc", "Cancel", Keys.Escape);
        private HotkeyButton hkSearchCustomers = new HotkeyButton("F2", "Search Customers", Keys.F2);

        //private FInputBox CustomerName = new FInputBox("Name#:");
        private FComboBox CustomerName = new FComboBox("Customer:", 200);
        private FDateTime DateReceived = new FDateTime("Date Received:");
        private FInputBox AmtReceived = new FInputBox("Amt Received:");
        private FlabelConstant AccountBalance = new FlabelConstant("Account Balance:");
        private FlabelConstant Remaining = new FlabelConstant("Remaining:");

        private GridViewOrigin ReceivePaymentGrid = new GridViewOrigin();
        private DataGridView ReceivePymtGridRefer;

        private int customerId = 0;
        private InvoicesModel InvoicesModelObj = new InvoicesModel();
        private CustomersModel CustomersModelObj = new CustomersModel();
        private PaymentDetailModel PymtDetailModelObj = new PaymentDetailModel();

        public ReceivePayment(int customerId = 0) : base("Receive Payment", "Fill out to receive payment")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[3] { hkReceivePayment, hkCancel, hkSearchCustomers };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            this.customerId = customerId;
            InitInputBox();
            InitPaymentList();

            this.VisibleChanged += (s, e) =>
            {
                this.LoadPaymentList();
            };
        }

        private void AddHotKeyEvents()
        {
            hkReceivePayment.GetButton().Click += async (sender, e) =>
            {
                int rowCount = ReceivePymtGridRefer.Rows.Count;
                if(rowCount > 0)
                {
                    CustomerData customer = (CustomerData)this.CustomerName.GetComboBox().SelectedItem;
                    int customerId = customer.Id;

                    DateTime dateReceived = DateReceived.GetDateTimePicker().Value;
                    double amtReceived;
                    double.TryParse(AmtReceived.GetTextBox().Text, out amtReceived);

                    DataGridViewRow row = ReceivePymtGridRefer.SelectedRows[0];
                    int orderId = int.Parse(row.Cells["id"].Value.ToString());

                    QboApiService qboApiService = new QboApiService();
                    bool res = await qboApiService.CreatePayment(customerId, customer.Name, customer.QboId, dateReceived, amtReceived, orderId);

                    if (res)
                    {
                        this.LoadPaymentList();
                    }
                }
            };
            hkSearchCustomers.GetButton().Click += (sender, e) =>
            {
                CustomerList customerListModal = new CustomerList();
                _navigateToForm(sender, e, customerListModal);
                this.Hide();
            };
        }

        private void InitInputBox()
        {
            List<dynamic> FormComponents = new List<dynamic>();

            FormComponents.Add(CustomerName);
            FormComponents.Add(DateReceived);
            FormComponents.Add(AmtReceived);

            FormComponents.Add(AccountBalance);
            FormComponents.Add(Remaining);

            _addFormInputs(FormComponents, 30, 110, 800, 42, 200);


            var refreshData = CustomersModelObj.LoadCustomerData("", false);


            CustomerName.GetComboBox().DisplayMember = "Name";
            CustomerName.GetComboBox().ValueMember = "Id";
            CustomerName.GetComboBox().DataSource = CustomersModelObj.CustomerDataList;

            CustomerName.GetComboBox().SelectedValueChanged += CustomerChanged;
        }

        private void CustomerChanged(object sender, EventArgs e)
        {
            int customerId = int.Parse(CustomerName.GetComboBox().SelectedValue.ToString());
            LoadPaymentList(customerId);
        }

        private void InitPaymentList()
        {
            ReceivePymtGridRefer = ReceivePaymentGrid.GetGrid();
            ReceivePymtGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            ReceivePymtGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            ReceivePymtGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            ReceivePymtGridRefer.Location = new Point(0, 250);
            ReceivePymtGridRefer.Width = this.Width;
            ReceivePymtGridRefer.Height = this.Height - 295;
            ReceivePymtGridRefer.MultiSelect = false;
            ReceivePymtGridRefer.AllowUserToAddRows = false;
            this.Controls.Add(ReceivePymtGridRefer);

            //this.ReceivePymtGridRefer.CellDoubleClick += (sender, e) =>
            //{
            //    updateCustomer(sender, e);
            //};

            LoadPaymentList(this.customerId);
        }

        private void LoadPaymentList(int customerId = 0)
        {
            List<InvoiceData> InvoiceDataList = InvoicesModelObj.LoadInvoiceData(customerId);

            ReceivePymtGridRefer.DataSource = InvoiceDataList;
            ReceivePymtGridRefer.Columns[0].Visible = false;
            ReceivePymtGridRefer.Columns[1].HeaderText = "CustomerId";
            ReceivePymtGridRefer.Columns[1].Visible = false;
            ReceivePymtGridRefer.Columns[2].HeaderText = "Invoice";
            ReceivePymtGridRefer.Columns[2].Width = 300;
            ReceivePymtGridRefer.Columns[3].HeaderText = "Date";
            ReceivePymtGridRefer.Columns[3].Width = 300;
            ReceivePymtGridRefer.Columns[4].HeaderText = "Description";
            ReceivePymtGridRefer.Columns[4].Width = 500;
            ReceivePymtGridRefer.Columns[5].HeaderText = "Total";
            ReceivePymtGridRefer.Columns[5].Width = 200;
            ReceivePymtGridRefer.Columns[6].HeaderText = "Balance";
            ReceivePymtGridRefer.Columns[6].Visible = false;
        }
    }
}
