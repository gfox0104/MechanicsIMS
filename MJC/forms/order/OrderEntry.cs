﻿using MJC.common.components;
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
using MJC.forms.sku;
using QboLib;
using MJC.qbo;

namespace MJC.forms.order
{
    public partial class OrderEntry : GlobalLayout
    {
        private HotkeyButton hkAdds = new HotkeyButton("Ins", "Adds", Keys.Insert);
        private HotkeyButton hkSelects = new HotkeyButton("Enter", "Selects", Keys.Enter);
        //private HotkeyButton hkSwitchColumn = new HotkeyButton("Alt + S", "Switch column", Keys.S);
        private HotkeyButton hkOpenCustomer = new HotkeyButton("F5", "Open Customer", Keys.F5);
        private HotkeyButton hkCheckStok = new HotkeyButton("F6", "Stock", Keys.F6);
        private HotkeyButton hkHeldOrders = new HotkeyButton("F7", "Held Orders", Keys.F7);
        private HotkeyButton hkProfiler = new HotkeyButton("F8", "Profiler", Keys.F8);
        private HotkeyButton hkHeldOrdersForCustomer = new HotkeyButton("F9", "Held Orders for Customer", Keys.F9);

        GridViewOrigin OrderEntryLookupGrid = new GridViewOrigin();
        private DataGridView OEGridRefer;
        private int OEGridSelectedIndex = 0;

        private string searchKey;


        public OrderEntry() : base("Order Entry - Select a Customer", "Select a customer to start an order for")
        {
            InitializeComponent();
            _initBasicSize();

            HotkeyButton[] hkButtons = new HotkeyButton[7] { hkAdds, hkSelects, hkOpenCustomer, hkCheckStok, hkHeldOrders, hkProfiler, hkHeldOrdersForCustomer };
            _initializeHKButtons(hkButtons);
            AddHotKeyEvents();

            InitOrderItemsList();
            //InitGridFooter();
            this.VisibleChanged += (s, e) =>
            {
                //this.getcu();
            };

            //ComboBox_SelectedIndexChanged(Customer.GetComboBox(), EventArgs.Empty);
        }

        private void AddHotKeyEvents()
        {
            hkAdds.GetButton().Click += (s, e) =>
            {
                if (OEGridRefer.RowCount > 0)
                {
                    int rowIndex = OEGridRefer.CurrentCell.RowIndex;
                    DataGridViewRow row = OEGridRefer.Rows[rowIndex];

                    int customerId = (int)row.Cells[0].Value;
                    addProcessOrder(s, e, customerId);
                }
            };
            hkSelects.GetButton().Click += (s, e) =>
            {
                int customerId = (int)OEGridRefer.SelectedRows[0].Cells[0].Value;
                addProcessOrder(s, e, customerId);
            };
            hkOpenCustomer.GetButton().Click += (sender, e) =>
            {
                //QboApiService apiService = new QboApiService();
                //apiService.LoadCustomers();
                int customerId = (int)OEGridRefer.SelectedRows[0].Cells[0].Value;
                CustomerInformation customerInfoModal = new CustomerInformation(true);
                customerInfoModal.setDetails(customerId);
                _navigateToForm(sender, e, customerInfoModal);
                this.Hide();
            };
            hkCheckStok.GetButton().Click += (sender, e) =>
            {
                SKUList skuListModal = new SKUList();
                _navigateToForm(sender, e, skuListModal);
                this.Hide();
            };
            hkProfiler.GetButton().Click += (sender, e) =>
            {
                int customerId = (int)OEGridRefer.SelectedRows[0].Cells[0].Value;
                CustomerProfile customerProfileModal = new CustomerProfile(customerId);
                _navigateToForm(sender, e, customerProfileModal);
                this.Hide();
            };
            hkHeldOrders.GetButton().Click += (sender, e) =>
            {
                HeldOrder heldOrderModal = new HeldOrder();
                _navigateToForm(sender, e, heldOrderModal);
                this.Hide();
            };
            hkHeldOrdersForCustomer.GetButton().Click += (sender, e) =>
            {
                if(OEGridRefer.RowCount > 0)
                {
                    int rowIndex = OEGridRefer.SelectedRows[0].Index;
                    DataGridViewRow row = OEGridRefer.Rows[rowIndex];
                    int customerId = (int)row.Cells[0].Value;
                    string customerName = row.Cells[2].Value.ToString();

                    HeldOrder heldOrderModal = new HeldOrder(customerId, customerName);
                    _navigateToForm(sender, e, heldOrderModal);
                    this.Hide();
                }
            };
        }

        private void InitOrderItemsList()
        {
            OEGridRefer = OrderEntryLookupGrid.GetGrid();
            OEGridRefer.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(157, 196, 235);
            OEGridRefer.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 63, 96);
            OEGridRefer.ColumnHeadersDefaultCellStyle.Padding = new Padding(12);
            OEGridRefer.Location = new Point(0, 95);
            OEGridRefer.Width = this.Width;
            OEGridRefer.Height = 745;
            OEGridRefer.AllowUserToAddRows = false;

            //OEGridRefer.EditingControlShowing += OEGridRefer_EditingControlShowing;
            this.OEGridRefer.CellDoubleClick += (s, e) =>
            {
                int sRId = (int)OEGridRefer.SelectedRows[0].Cells[0].Value;
                addProcessOrder(s, e, sRId);
            };

            this.Controls.Add(OEGridRefer);

            this.LoadSKUList();
        }

        private void InitGridFooter()
        {
            List<dynamic> GridFooterComponents = new List<dynamic>();

            _addFormInputs(GridFooterComponents, 30, 720, 650, 42, 820);

            List<dynamic> GridFooterComponents1 = new List<dynamic>();

            _addFormInputs(GridFooterComponents1, 680, 720, 650, 42, 820);
        }

        public void LoadSKUList(bool keepSelection = true)
        {
            DataTable dataTable = Session.CustomersModelObj.LoadCustomerTable();

            OEGridRefer.DataSource = dataTable;
            OEGridRefer.Columns[0].Visible = false;
            OEGridRefer.Columns[1].HeaderText = "Customer #";
            OEGridRefer.Columns[1].Width = 200;
            OEGridRefer.Columns[2].HeaderText = "Customer Name";
            OEGridRefer.Columns[2].Width = 300;
            OEGridRefer.Columns[3].HeaderText = "Address";
            OEGridRefer.Columns[3].Width = 400;
            OEGridRefer.Columns[4].HeaderText = "City";
            OEGridRefer.Columns[4].Width = 200;
            OEGridRefer.Columns[5].HeaderText = "State";
            OEGridRefer.Columns[5].Width = 200;
            OEGridRefer.Columns[6].HeaderText = "ZipCode";
            OEGridRefer.Columns[6].Width = 200;
        }

        private void OEGridRefer_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewComboBoxEditingControl comboBoxEditingControl = e.Control as DataGridViewComboBoxEditingControl;

            if (comboBoxEditingControl != null)
            {
                comboBoxEditingControl.DropDownHeight = 300; // Set the desired height for the drop-down menu
            }
        }

        private void addProcessOrder(object sender, EventArgs e, int customerId = 0)
        {
            ProcessOrder processForm = new ProcessOrder(customerId, 0, true);
            _navigateToForm(sender, e, processForm);
            this.Hide();
        }

 

        private void OrderEntry_Load(object sender, EventArgs e)
        {
           
        }

        private void OrderEntry_Activated(object sender, EventArgs e)
        {
            InitOrderItemsList();



        }

        private void OrderEntry_Validated(object sender, EventArgs e)
        {

        }
    }
}
